using System;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
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
