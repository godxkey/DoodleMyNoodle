using System;
using System.Linq;
using UnityEngine;

namespace UnityX.FixMath
{
    public class ScriptTemplateAdditionalIncludes
    {
        [DefaultSmartScriptResolver.AdditionalUsingsProvider]
        public static string[] GetAdditionalIncludes(DefaultSmartScriptResolver.Info info)
        {
            if (info.Assembly.GetReferencedAssemblies().Any((asm) => asm.Name == "CCC.FixMath"))
            {
                return new string[]
                {
                    $"using static {nameof(fixMath)};"
                };
            }

            return null;
        }
    }
}