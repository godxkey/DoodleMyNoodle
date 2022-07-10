using System;
using System.Collections.Generic;
using Unity.Serialization.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LaunchCommandsWindow : ToolsWindowBase
{
    [Serializable]
    private class CommandSaveData
    {
        public bool Active;
        public string Name;
        public string[] Parameters;
    }
    [Serializable]
    private class CommandSaveDataContainer
    {
        public CommandSaveData[] CommandSaveDatas;
    }

    protected override string UssGuid => "b45b2f33fe8c2264896819e989527c87";
    protected override string UxmlGuid => "be1ba00f8d7124848af15887fcd0b99d";

    private ListView _listView;
    private List<WidgetDataLaunchCommandElement> _widgetDatas = new List<WidgetDataLaunchCommandElement>();

    protected override void OnEnable()
    {
        base.OnEnable();
        Load();
    }

    protected override void Rebuild(VisualElement root)
    {
        _listView = root.Q<ListView>("invokablesContainer");
        _listView.fixedItemHeight = 18;
        _listView.makeItem = () => new LaunchCommandElement();
        _listView.bindItem = (v, index) => Bind((LaunchCommandElement)v, index);
        _listView.itemsSource = _widgetDatas;
        _listView.style.flexGrow = 1;
        _listView.selectionType = SelectionType.None;


        var addButton = root.Q<Button>("addButton");
        addButton.clicked -= OnAddButtonClicked;
        addButton.clicked += OnAddButtonClicked;
    }

    private void Bind(LaunchCommandElement v, int index)
    {
        WidgetDataLaunchCommandElement widgetData = _widgetDatas[index];

        v.SetData(widgetData);
    }

    private void OnAddButtonClicked()
    {
        CommandsBrowserWindow.ShowWindow(IsCommandAdded, AddCommand);
    }

    private bool IsCommandAdded(IGameConsoleInvokable invokable)
    {
        foreach (var item in _widgetDatas)
        {
            if (ReferenceEquals(item.GameConsoleInvokable, invokable))
                return true;
        }

        return false;
    }

    private void AddCommand(IGameConsoleInvokable invokable)
    {
        var widgetData = CreateWidgetDataElement();
        widgetData.GameConsoleInvokable = invokable;
        widgetData.Active = true;
        _widgetDatas.Add(widgetData);
        _listView.Rebuild();

        Save();
    }

    private WidgetDataLaunchCommandElement CreateWidgetDataElement()
    {
        var widgetData = new WidgetDataLaunchCommandElement();
        widgetData.OnActiveClickCallback = OnToggleActiveClicked;
        widgetData.OnRemoveClickCallback = OnRemoveClicked;
        widgetData.OnParameterModified = (x) => Save();
        return widgetData;
    }

    private void OnRemoveClicked(LaunchCommandElement obj)
    {
        _widgetDatas.Remove(obj.GetData());
        _listView.Rebuild();

        Save();
    }

    private void OnToggleActiveClicked(LaunchCommandElement widget, bool active)
    {
        var widgetData = widget.GetData();
        widgetData.Active = active;
        widget.SetData(widgetData);

        Save();
    }

    private void Save()
    {
        List<string> commandTexts = new List<string>(_widgetDatas.Count);
        CommandSaveData[] saveData = new CommandSaveData[_widgetDatas.Count];
        for (int i = 0; i < _widgetDatas.Count; i++)
        {
            var item = _widgetDatas[i];
            saveData[i] = new CommandSaveData()
            {
                Active = item.Active,
                Name = item.GameConsoleInvokable.DisplayName,
                Parameters = item.Parameters
            };

            if (item.Active)
            {
                if (item.Parameters != null && item.Parameters.Length > 0)
                {
                    commandTexts.Add($"-{item.GameConsoleInvokable.DisplayName} {string.Join(" ", item.Parameters)}");
                }
                else
                {
                    commandTexts.Add($"-{item.GameConsoleInvokable.DisplayName}");
                }
            }
        }
        string json = JsonSerialization.ToJson(new CommandSaveDataContainer() { CommandSaveDatas = saveData });
        EditorPrefs.SetString("launch-commands-window", json);
        GameConsole.EditorPlayCommands = commandTexts.ToArray();
    }

    private void Load()
    {
        _widgetDatas.Clear();
        var allInvokables = GameConsole.Invokables.ToArray();

        string json = EditorPrefs.GetString("launch-commands-window", "");
        var saveData = JsonSerialization.FromJson<CommandSaveDataContainer>(json).CommandSaveDatas;
        if (saveData == null)
            saveData = new CommandSaveData[0];
        foreach (var item in saveData)
        {
            var invokable = FindInvokable(item.Name);

            if (invokable != null)
            {
                var widgetData = CreateWidgetDataElement();
                widgetData.GameConsoleInvokable = invokable;
                widgetData.Active = item.Active;
                widgetData.Parameters = item.Parameters;
                _widgetDatas.Add(widgetData);
            }
        }

        IGameConsoleInvokable FindInvokable(string name)
        {
            for (int i = 0; i < allInvokables.Length; i++)
            {
                if (string.Equals(allInvokables[i].DisplayName, name))
                {
                    return allInvokables[i];
                }
            }

            return null;
        }
    }

    [MenuItem("Tools/Launch Commands", priority = 101)]
    public static void ShowWindow()
    {
        LaunchCommandsWindow wnd = GetWindow<LaunchCommandsWindow>();
        wnd.titleContent = new GUIContent("LaunchCommands");
    }
}
