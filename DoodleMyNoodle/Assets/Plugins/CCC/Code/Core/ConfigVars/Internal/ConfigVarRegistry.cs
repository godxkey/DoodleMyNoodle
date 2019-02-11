using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CCC.ConfigVarInterals
{
    public class ConfigVarRegistry
    {
        public Dictionary<string, ConfigVarBase> ConfigVars = new Dictionary<string, ConfigVarBase>();

        private readonly static Regex validVarNameRegex = new Regex(@"^[a-z_+-][a-z0-9_+.-]*$");

        public void RegisterConfigVar(ConfigVarBase cvar)
        {
            if (ConfigVars.ContainsKey(cvar.name))
            {
                DebugService.LogError("Trying to register cvar " + cvar.name + " twice");
                return;
            }
            if (!validVarNameRegex.IsMatch(cvar.name))
            {
                DebugService.LogError("Trying to register cvar with invalid name: " + cvar.name);
                return;
            }
            ConfigVars.Add(cvar.name, cvar);
        }
    }
}