using System.Collections.Generic;
using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace ExamAI.Shared.Logging
{
    public class SensitiveDataScrubber : IDestructuringPolicy
    {
        private static readonly string[] SensitiveProperties = { "password", "token", "cardnumber", "secret", "accesstoken" };

        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            result = null;
            if (value is IDictionary<string, object> dict)
            {
                var scrubbedDict = dict.ToDictionary(
                    kvp => kvp.Key,
                    kvp => SensitiveProperties.Contains(kvp.Key.ToLower()) ? "*** SCRUBBED ***" : kvp.Value
                );
                result = propertyValueFactory.CreatePropertyValue(scrubbedDict, true);
                return true;
            }
            return false;
        }
    }
}