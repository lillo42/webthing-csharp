using System.Collections.Generic;
using System.IO;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.CompressionTasks;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Nuke.Common.Logger;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[AzurePipelines(
    AzurePipelinesImage.UbuntuLatest,
    TriggerBranchesInclude = new[]{"master", "release-*"},
    PullRequestsBranchesInclude = new[]{"master", "release-*"},
    InvokedTargets = new[] { nameof(Test), nameof(Pack) },
    NonEntryTargets = new[] { nameof(Restore) },
    ExcludedTargets = new[] { nameof(Clean), nameof(Coverage) }
)]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    [Parameter("Nuget API key", Name = "api-key")] 
    readonly string ApiKey;

    [Parameter("NuGet Source for Packages", Name = "nuget-source")]
    readonly string NugetSource = "https://api.nuget.org/v3/index.json";

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;
    [CI] readonly AzurePipelines AzurePipelines;
    
    
    [Partition(2)] readonly Partition TestPartition;
    IEnumerable<Project> TestProjects => TestPartition.GetCurrent(Solution.GetProjects("*.Tests"));

    AbsolutePath SourceDirectory => RootDirectory/ "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    
    AbsolutePath PackageDirectory => ArtifactsDirectory / "packages";
    
    AbsolutePath TestResultDirectory => ArtifactsDirectory / "test-results";
    
    string CoverageReportDirectory => ArtifactsDirectory / "coverage-report";
    string CoverageReportArchive => ArtifactsDirectory / "coverage-report.zip";

    const string ReleaseBranchPrefix = "release-";
    

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetClean(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration));
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });
    
    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")
        .Partition(() => TestPartition)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetResultsDirectory(TestResultDirectory)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .When(InvokedTargets.Contains(Coverage) || IsServerBuild, _ => _
                    .EnableCollectCoverage()
                    .SetCoverletOutputFormat(CoverletOutputFormat.opencover)
                    .When(IsServerBuild, _ => _.EnableUseSourceLink()))
                .CombineWith(TestProjects, (_, v) => _
                    .SetProjectFile(v)
                    .SetLogger($"trx;LogFileName={v.Name}.trx")
                    .SetCoverletOutput(TestResultDirectory / $"{v.Name}.xml")));
            
            TestResultDirectory.GlobFiles("*.trx").ForEach(x =>
                AzurePipelines?.PublishTestResults(
                    type: AzurePipelinesTestResultsType.XUnit,
                    title: $"{Path.GetFileNameWithoutExtension(x)} ({AzurePipelines.StageDisplayName})",
                    files: new string[] { x }));
        });

    Target Coverage => _ => _
        .DependsOn(Test)
        .TriggeredBy(Test)
        .Consumes(Test)
        .Produces(CoverageReportArchive)
        .Executes(() =>
        {
            if (InvokedTargets.Contains(Coverage) || IsServerBuild)
            {
                ReportGenerator(_ => _
                    .SetReports(TestResultDirectory / "*.xml")
                    .SetReportTypes(ReportTypes.HtmlInline)
                    .SetTargetDirectory(CoverageReportDirectory)
                    .SetFramework("netcoreapp2.1"));
                
                TestResultDirectory.GlobFiles("*.xml").ForEach(x =>
                    AzurePipelines?.PublishCodeCoverage(
                        AzurePipelinesCodeCoverageToolType.Cobertura,
                        x,
                        CoverageReportDirectory));
                
                CompressZip(
                    directory: CoverageReportDirectory,
                    archiveFile: CoverageReportArchive,
                    fileMode: FileMode.Create);
            }
        });


    Target Pack => _ => _
        .DependsOn(Compile, Test)
        .Produces(PackageDirectory / "*.nupkg")
        .Produces(PackageDirectory / "*.snupkg")
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetConfiguration(Configuration)
                .SetOutputDirectory(PackageDirectory)
                .SetVersion(GitVersion.NuGetVersionV2)
                .EnableIncludeSource()
                .EnableIncludeSymbols()
                .EnableNoRestore());
        });
    
    Target Publish => _ => _
        .DependsOn(Clean, Test, Pack)
        .Consumes(Pack)
        .Requires(() => ApiKey)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Requires(() => GitRepository.Branch.StartsWith(ReleaseBranchPrefix))
        .Executes(() =>
        {
            DotNetNuGetPush(s => s
                .SetApiKey(ApiKey)
                .SetSkipDuplicate(true)
                .SetSource(NugetSource));
        });

}
