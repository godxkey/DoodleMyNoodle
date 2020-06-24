using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

namespace CCC.ConfigVarInterals
{
    public static class ConfigVarSaver
    {
        /// <summary>
        /// Save the config vars to a text file. 
        /// </summary>
        /// <param name="registry">The config vars</param>
        /// <param name="filename">The text file name</param>
        /// <param name="flagFilter">Only config vars that have one of the filter flags at 1 will be saved</param>
        public static void Save(ConfigVarRegistry registry, int flagFilter, string filename)
        {
            using (var st = System.IO.File.CreateText(filename))
            {
                foreach (var cvar in registry.ConfigVars.Values)
                {
                    if ((cvar.rawFlags & flagFilter) != 0)
                        st.WriteLine("{0} \"{1}\"", cvar.name, cvar.Value);
                }
            }
            Log.Info("saved config vars: " + filename);
        }
    }
}