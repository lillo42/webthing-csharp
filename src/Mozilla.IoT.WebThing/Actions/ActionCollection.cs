using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;

namespace Mozilla.IoT.WebThing.Actions
{
    public class ActionCollection : IEnumerable<ActionInfo>
    {
        private readonly ConcurrentDictionary<Guid, ActionInfo> _actions;
        private readonly ActionInfoConvert _inputConvert;
        private readonly IActionInfoFactory _actionInfoFactory;

        public event EventHandler<ActionInfo>? Change;

        public ActionCollection(ActionInfoConvert inputConvert, IActionInfoFactory actionInfoFactory)
        {
            _actionInfoFactory = actionInfoFactory ?? throw new ArgumentNullException(nameof(actionInfoFactory));
            _inputConvert = inputConvert;
            _actions = new ConcurrentDictionary<Guid, ActionInfo>();
        }

        public bool TryAdd(JsonElement element, out ActionInfo? info)
        {
            info = null;
            Dictionary<string, object>? inputValues = null;
            if (element.TryGetProperty("input", out var inputProperty))
            {
                if (inputProperty.ValueKind == JsonValueKind.Object 
                    && !_inputConvert.TryConvert(inputProperty, out inputValues))
                {
                    return false;
                }
            }

            inputValues ??= new Dictionary<string, object>();

            info = _actionInfoFactory.CreateActionInfo(inputValues);
            if (info == null)
            {
                return false;
            }
            info.StatusChanged += OnStatusChange;
            
            return _actions.TryAdd(info.GetId(), info);
        }

        public bool TryGetValue(Guid id, out ActionInfo? action)
            => _actions.TryGetValue(id, out action);
        
        public bool TryRemove(Guid id, out ActionInfo action)
        {
            var result =_actions.TryRemove(id, out action);
            if (result && action != null)
            {
                action.StatusChanged -= OnStatusChange;   
            }
            
            return result;
        }
        
        private void OnStatusChange(object? sender, EventArgs args)
        {
            var change = Change;
            change?.Invoke(this, (ActionInfo)sender);
        }

        public IEnumerator<ActionInfo> GetEnumerator() 
            => _actions.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
