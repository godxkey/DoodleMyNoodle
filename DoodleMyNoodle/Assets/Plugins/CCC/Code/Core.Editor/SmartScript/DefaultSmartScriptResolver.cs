using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditorX;
using UnityEngine;
using UnityEngineX;

public class DefaultSmartScriptResolver : SmartScriptResolver
{
    /// <summary>
    /// Expected signature:  public static <see cref="string"/>[] GetAdditionalIncludes(<see cref="DefaultSmartScriptResolver.Info"/> info)
    /// </summary>
    public class AdditionalUsingsProviderAttribute : Attribute { }

    public class Info
    {
        public readonly string Path;
        public readonly UnityEditor.Compilation.Assembly CompilationAssembly;
        public readonly System.Reflection.Assembly Assembly;

        public Info(string path, UnityEditor.Compilation.Assembly compilationAssembly, System.Reflection.Assembly assembly)
        {
            Path = path;
            CompilationAssembly = compilationAssembly;
            Assembly = assembly;
        }
    }

    public override void GetNewScriptContent(string path, out string content, out string defaultName)
    {
        TextAsset scriptTemplateAsset = FindScriptTemplateAsset(path);

        if (scriptTemplateAsset != null)
        {
            GetNewScriptContent(scriptTemplateAsset, out content, out defaultName);
        }
        else
        {
            GetDefaultNewScriptContent(out content, out defaultName);
        }
    }

    public void GetNewScriptContent(TextAsset scriptTemplateAsset, out string content, out string defaultName)
    {
        if (scriptTemplateAsset is null)
        {
            throw new ArgumentNullException(nameof(scriptTemplateAsset));
        }

        Type[] typesInFile = CodeParsing.ParseTypesFromCode(scriptTemplateAsset.text);

        Type scriptTemplateType = null;
        foreach (Type type in typesInFile)
        {
            if (typeof(ScriptTemplate).IsAssignableFrom(type))
            {
                scriptTemplateType = type;
                break;
            }
        }

        if (scriptTemplateType == null)
        {
            Debug.LogError($"No type inheriting from {nameof(ScriptTemplate)} were found in file {scriptTemplateAsset.name}.cs.",
                context: scriptTemplateAsset);
            content = null;
            defaultName = null;
            return;
        }

        if (scriptTemplateType.IsAbstract)
        {
            Debug.LogError($"{scriptTemplateType.GetPrettyName()} is abstract.",
                context: scriptTemplateAsset);
            content = null;
            defaultName = null;
            return;
        }

        ScriptTemplate scriptTemplateInstance;

        try
        {
            scriptTemplateInstance = (ScriptTemplate)Activator.CreateInstance(scriptTemplateType);
        }
        catch
        {
            Debug.LogError($"Could not create an instance of {scriptTemplateType.Name}. It's constructor must be parameterless.",
                context: scriptTemplateAsset);
            content = null;
            defaultName = null;
            return;
        }

        content = scriptTemplateInstance.GetScriptContent();
        defaultName = scriptTemplateInstance.GetScriptDefaultName();

        // disabled for now, not sure we want that feature anymore :/
        //content = InsertAdditionalUsings(content, GetAdditionalProvidedUsings(path));
    }

    public void GetDefaultNewScriptContent(out string content, out string defaultName)
    {
        content = GetDefaultScriptTemplate();
        defaultName = GetDefaultScriptName();

        // disabled for now, not sure we want that feature anymore :/
        //content = InsertAdditionalUsings(content, GetAdditionalProvidedUsings(path));
    }

    protected string InsertAdditionalUsings(string scriptContent, string[] additionalUsings)
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach (string additionalUsing in additionalUsings)
        {
            if (!scriptContent.Contains(additionalUsing))
                stringBuilder.AppendLine(additionalUsing);
        }

        stringBuilder.AppendLine(scriptContent);

        string result = stringBuilder.ToString();

        return result;
    }

    private static TextAsset FindScriptTemplateAsset(string path)
    {
        string directory = path;

        try
        {
            if(File.Exists(Application.dataPath.RemoveLast("Assets") + path))
            {
                directory = GetDirectory(directory);
            }
        }
        catch
        {
            return null;
        }

        string[] templateAssetLinkGUIDs = AssetDatabase.FindAssets($"t:{nameof(SmartScriptTemplateReference)}");

        if (templateAssetLinkGUIDs.Length > 0)
        {
            // Find template asset with longest directory path (meaning closest to context path)
            string bestAssetPath = string.Empty;
            int longestDirectoryLength = int.MinValue;

            for (int i = 0; i < templateAssetLinkGUIDs.Length; i++)
            {
                var candidatePath = AssetDatabase.GUIDToAssetPath(templateAssetLinkGUIDs[i]);
                var candidateDirectory = GetDirectory(candidatePath);

                if (directory.Contains(candidateDirectory) && candidateDirectory.Length > longestDirectoryLength)
                {
                    longestDirectoryLength = candidateDirectory.Length;
                    bestAssetPath = candidatePath;
                }
            }

            var templateAssetLink = AssetDatabase.LoadAssetAtPath<SmartScriptTemplateReference>(bestAssetPath);

            if (templateAssetLink != null)
            {
                return templateAssetLink.TemplateScript as MonoScript;
            }
        }

        return null;

        string GetDirectory(string p)
        {
            return Path.GetDirectoryName(p).Replace('\\', '/');
        }
    }

    protected virtual string GetDefaultScriptTemplate()
    {
        return $@"using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class #SCRIPTNAME# : {nameof(MonoBehaviour)}
{{
    void Start()
    {{
        #NOTRIM#
    }}
    
    void Update()
    {{
        #NOTRIM#
    }}
}}";
    }

    protected virtual string GetDefaultScriptName()
    {
        return "New Script.cs";
    }

    protected virtual string[] GetAdditionalProvidedUsings(string path)
    {
        TypeCache.MethodCollection usingProviders = TypeCache.GetMethodsWithAttribute<AdditionalUsingsProviderAttribute>();

        TryGetAssembly(path, out UnityEditor.Compilation.Assembly compilationAssembly);
        TryGetAssembly(path, out System.Reflection.Assembly assembly);

        object[] invokeParameters = new object[] { new Info(path, compilationAssembly, assembly) };
        List<string> additionalUsings = new List<string>();

        foreach (MethodInfo providerMethod in usingProviders)
        {
            if (!providerMethod.IsStatic)
            {
                LogBadMethod(providerMethod, "Needs to be static");
                continue;
            }

            if (providerMethod.ReturnType != typeof(string[]))
            {
                LogBadMethod(providerMethod, "Return type needs to be string[]");
                continue;
            }

            var parameters = providerMethod.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(Info))
            {
                LogBadMethod(providerMethod, $"Parameter needs to be '{nameof(DefaultSmartScriptResolver)}.{nameof(Info)} info'");
                continue;
            }

            var newUsings = providerMethod.Invoke(null, invokeParameters) as string[];
            if (newUsings != null)
            {
                additionalUsings.AddRange(newUsings);
            }
        }


        void LogBadMethod(MethodInfo providerMethod, string reason)
        {
            Debug.LogWarning($"Method {providerMethod.Name} has the [{nameof(AdditionalUsingsProviderAttribute)}] attribute " +
                $"but doesn't respect the following criteria: {reason}");
        }

        return additionalUsings.Distinct().ToArray();
    }
}