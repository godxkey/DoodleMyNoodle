using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorX;
using UnityEngine;
using UnityEngineX;


public static class SmartScript
{
    public static SmartScriptResolver s_Resolver = new DefaultSmartScriptResolver();

    [MenuItem("Assets/Create/C# Script (Smart Templates)", priority = 80)]
    public static void CreateSmartScript()
    {
        string selectionAssetPath = GetSelectionAssetPath();
        GetScriptTemplateForPath(selectionAssetPath, out string templateContent, out string defaultName);

        if (string.IsNullOrEmpty(templateContent) || string.IsNullOrEmpty(defaultName))
        {
            return;
        }

        if (!defaultName.EndsWith(".cs"))
        {
            defaultName += ".cs";
        }

        // Unity needs a text file like: MyScriptTemplate.txt
        // so we make one in the Temp folder.
        string templateFilePath = MakeScriptTemplateTxtFile(templateContent);

        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templateFilePath, defaultName);

        PostCreateProcessor.s_NewSmartScriptPath = selectionAssetPath;
    }

    private static string MakeScriptTemplateTxtFile(string scriptTemplateContent)
    {
        string templateFolder = $"{Application.dataPath.RemoveLast("Assets")}Temp";
        string templatePath = $"{templateFolder}/SmartScriptTemplateTemp.txt";

        if (!Directory.Exists(templateFolder))
        {
            Directory.CreateDirectory(templateFolder);
        }

        File.WriteAllText(templatePath, scriptTemplateContent);

        return templatePath;
    }

    private static void GetScriptTemplateForPath(string path, out string content, out string defaultName)
    {
        s_Resolver.GetNewScriptContent(path, out content, out defaultName);
    }

    private static string GetSelectionAssetPath()
    {
        UnityEngine.Object currentSelection = (Selection.objects != null && Selection.objects.Length > 0) ? Selection.objects[0] : null;

        if (currentSelection != null)
        {
            return AssetDatabase.GetAssetPath(currentSelection);
        }
        else
        {
            return "Assets";
        }
    }

    /// <summary>
    /// This class takes care of post-processing any newly created script templates
    /// </summary>
    public class PostCreateProcessor : AssetPostprocessor
    {
        public static string s_NewSmartScriptPath;

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (s_NewSmartScriptPath == null)
                return;

            string newScriptTemplatePath = null;

            // Find new script path among imported assets
            foreach (string importedAsset in importedAssets)
            {
                if (importedAsset.Contains(s_NewSmartScriptPath))
                {
                    newScriptTemplatePath = importedAsset;
                    break;
                }
            }

            s_NewSmartScriptPath = null;

            // Apply post processing !
            if (newScriptTemplatePath != null)
            {
                string fileContent = File.ReadAllText(newScriptTemplatePath);

                fileContent = PostProcessNewTemplate(fileContent);

                File.WriteAllText(newScriptTemplatePath, fileContent);
            }
        }

        private static string PostProcessNewTemplate(string content)
        {
            return content.Replace("%STRIPME%", "");
        }
    }
}


public abstract class SmartScriptResolver
{
    public abstract void GetNewScriptContent(string path, out string content, out string defaultName);

    protected bool TryGetAssemblyName(string path, out string asmName)
    {
        // If the path is a directory, go 1 level deeper so 'GetAssemblyNameFromScriptPath(..)' picks up the correct assembly
        if (Directory.Exists(path))
        {
            string[] fileInDirectory = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
            if (fileInDirectory.Length > 0)
            {
                path = fileInDirectory[0];
            }
        }

        asmName = CompilationPipeline.GetAssemblyNameFromScriptPath(path).RemoveLast(".dll");

        return !string.IsNullOrEmpty(asmName);
    }

    protected bool TryGetAssembly(string path, out UnityEditor.Compilation.Assembly assembly)
    {
        if (TryGetAssemblyName(path, out string asmName))
        {
            assembly = Array.Find(CompilationPipeline.GetAssemblies(), (x) => x.name == asmName);
        }
        else
        {
            assembly = null;
        }

        return assembly != null;
    }

    protected bool TryGetAssembly(string path, out System.Reflection.Assembly assembly)
    {
        if (TryGetAssemblyName(path, out string asmName))
        {
            assembly = Array.Find(AppDomain.CurrentDomain.GetAssemblies(), (x) => x.GetName().Name == asmName);
        }
        else
        {
            assembly = null;
        }

        return assembly != null;
    }
}

public abstract class ScriptTemplate
{
    public abstract string GetScriptDefaultName();
    public abstract string GetScriptContent();
}

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

        if (scriptTemplateAsset == null)
        {
            content = GetDefaultScriptTemplate();
            defaultName = GetDefaultScriptName();
        }
        else
        {
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
        }

        // disabled for now, not sure we want that feature anymore :/
        //content = InsertAdditionalUsings(content, GetAdditionalProvidedUsings(path));
    }

    protected string InsertAdditionalUsings(string scriptContent, string[] additionalUsings)
    {
        StringBuilder stringBuilder = StringBuilderPool.Take();

        foreach (string additionalUsing in additionalUsings)
        {
            if (!scriptContent.Contains(additionalUsing))
                stringBuilder.AppendLine(additionalUsing);
        }

        stringBuilder.AppendLine(scriptContent);

        string result = stringBuilder.ToString();

        StringBuilderPool.Release(stringBuilder);

        return result;
    }

    private static TextAsset FindScriptTemplateAsset(string path)
    {
        var templateAssets = AssetDatabase.FindAssets("t:TextAsset", new string[] { "Assets/Script Templates" });

        int record = 0;
        string recordHolder = null;
        foreach (string item in templateAssets)
        {
            string templatePath = AssetDatabase.GUIDToAssetPath(item);

            string templateDirectory = templatePath.RemoveFirst("Script Templates/");

            templateDirectory = templateDirectory.Remove(templateDirectory.LastIndexOf('/'));

            if (path.StartsWith(templateDirectory) && templateDirectory.Length > record)
            {
                record = templateDirectory.Length;
                recordHolder = templatePath;
            }
        }

        if (recordHolder != null)
        {
            return AssetDatabase.LoadAssetAtPath<TextAsset>(recordHolder);
        }
        else
        {
            return null;
        }
    }

    protected virtual string GetDefaultScriptTemplate()
    {
        return $@"using System;
using Unity.Collections;
using Unity.Collections.Generic;

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