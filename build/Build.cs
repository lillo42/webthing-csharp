using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
using static Nuke.Common.Tools.Git.GitTasks;


[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
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
    
    [Parameter("Nuget API key")] 
    readonly string ApiKey;

    [Parameter("NuGet Source for Packages", Name = "nuget-source")]
    readonly string NugetSource = "https://api.nuget.org/v3/index.json";

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;
    [CI] readonly AzurePipelines AzurePipelines;

    AbsolutePath SourceDirectory => RootDirectory/ "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

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
    
    [Partition(2)] readonly Partition TestPartition;
    IEnumerable<Project> TestProjects => TestPartition.GetCurrent(Solution.GetProjects("*.Test"));
    AbsolutePath TestResultDirectory => ArtifactsDirectory / "test-results";
    
    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")
        .Partition(() => TestPartition)
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetNoRestore(InvokedTargets.Contains(Restore))
                .SetResultsDirectory(TestResultDirectory)
                .When(InvokedTargets.Contains(Coverage) || IsServerBuild, _ => _
                    .EnableCollectCoverage()
                    .SetCoverletOutputFormat(CoverletOutputFormat.opencover)
                    .When(IsServerBuild, _ => _.EnableUseSourceLink()))
                .CombineWith(TestProjects, (_, v) => _
                    .SetProjectFile(v)
                    .SetLogger($"trx;LogFileName={v.Name}.trx")
                    .SetCoverletOutput(TestResultDirectory / $"{v.Name}.xml"))
            );
            
            TestResultDirectory.GlobFiles("*.trx").ForEach(x =>
                AzurePipelines?.PublishTestResults(
                    type: AzurePipelinesTestResultsType.XUnit,
                    title: $"{Path.GetFileNameWithoutExtension(x)} ({AzurePipelines.StageDisplayName})",
                    files: new string[] { x }));
        });

    string CoverageReportDirectory => ArtifactsDirectory / "coverage-report";
    string CoverageReportArchive => ArtifactsDirectory / "coverage-report.zip";
    
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
    
    
    Target AcceptanceTest => _ => _
        .DependsOn(Test)
        .Executes(async () =>
        {
            Git("clone https://github.com/mozilla-iot/webthing-tester");
            
            var pip3 = (Tool) new PathExecutableAttribute("pip3").GetValue(null, null);
            pip3("install --user -r webthing-tester/requirements.txt");

            var source = new CancellationTokenSource();
            var dotnet = Task.Factory.StartNew(() => DotNetRun(_ => _
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution.GetProject("TestThing"))
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetNoRestore(InvokedTargets.Contains(Restore))), source.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            await Task.Delay(TimeSpan.FromSeconds(10));
            
            var webThingTest = (Tool) new PathExecutableAttribute("./webthing-tester/test-client.py").GetValue(null, null);
            webThingTest("--path-prefix \"/things/my-lamp-1234\"  --host localhost --port 5000");
            
            source.Cancel();
        });
    
    
    AbsolutePath PackageDirectory => ArtifactsDirectory / "packages";
    Target Pack => _ => _
        .DependsOn(Compile)
        .Produces(PackageDirectory / "*.nupkg")
        .Produces(PackageDirectory / "*.snupkg")
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetNoRestore(InvokedTargets.Contains(Restore))
                .SetConfiguration(Configuration)
                .SetOutputDirectory(PackageDirectory)
                .SetVersion(GitVersion.NuGetVersionV2)
                .EnableIncludeSource()
                .EnableIncludeSymbols());
        });
    
    Target Publish => _ => _
        .DependsOn(Pack)
        .Consumes(Pack)
        .Requires(() => ApiKey)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
        {
            DotNetNuGetPush(s => s
                .SetApiKey(ApiKey)
                .SetSkipDuplicate(true)
                .SetSource(NugetSource)
                .SetTargetPath(PackageDirectory / "*.nupkg"));
            
            DotNetNuGetPush(s => s
                .SetApiKey(ApiKey)
                .SetSkipDuplicate(true)
                .SetSource(NugetSource)
                .SetTargetPath(PackageDirectory / "*.snupkg"));
        });

}
