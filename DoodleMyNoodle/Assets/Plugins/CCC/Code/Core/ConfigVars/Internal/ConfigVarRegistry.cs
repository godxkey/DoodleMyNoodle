/*using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngineX;

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
                Log.Error("Trying to register cvar " + cvar.name + " twice");
                return;
            }
            if (!validVarNameRegex.IsMatch(cvar.name))
            {
                Log.Error("Trying to register cvar with invalid name: " + cvar.name);
                return;
            }
            ConfigVars.Add(cvar.name, cvar);
        }
    }
}*/