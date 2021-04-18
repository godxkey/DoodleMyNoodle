using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

public class WidgetDataLaunchCommandElement
{
    public IGameConsoleInvokable GameConsoleInvokable;
    public bool Active;
    public string[] Parameters;

    public Action<LaunchCommandElement> OnRemoveClickCallback;
    public Action<LaunchCommandElement, bool> OnActiveClickCallback;
    public Action<LaunchCommandElement> OnParameterModified;
}


public class LaunchCommandElement : ToolsVisualElementBase
{
    private Toggle _activeToggle;
    private Label _nameLabel;
    private VisualElement _paramsContainer;
    private Button _removeButton;
    private WidgetDataLaunchCommandElement _widgetData;

    public override string UxmlGuid => "c92d9d6d9edd1714b9f6d4c5bccbb0e3";
    public override string UssGuid => "32770a0f68aaf4649a9c9846514b628a";

    public LaunchCommandElement()
    {
        _activeToggle = this.Q<Toggle>("activeToggle");
        _nameLabel = this.Q<Label>("nameLabel");
        _paramsContainer = this.Q("paramsContainer");
        _removeButton = this.Q<Button>("removeButton");

        _activeToggle.RegisterValueChangedCallback((evnt) =>
        {
            _widgetData?.OnActiveClickCallback?.Invoke(this, evnt.newValue);
        });

        _removeButton.clicked += () => _widgetData?.OnRemoveClickCallback?.Invoke(this);
    }

    public WidgetDataLaunchCommandElement GetData() => _widgetData;

    public void SetData(WidgetDataLaunchCommandElement data)
    {
        _widgetData = data;

        _nameLabel.text = data.GameConsoleInvokable.DisplayName;
        _activeToggle.value = data.Active;
        _paramsContainer.Clear();

        int paramIndex = 0;
        foreach (IGameConsoleParameter param in data.GameConsoleInvokable.Parameters)
        {
            if (param.Type == typeof(bool))
            {
                var field = new Toggle();
                bool.TryParse(TryPopParam(), out bool val);
                field.value = val;
                field.RegisterValueChangedCallback((evt) => OnParameterModified());
                _paramsContainer.Add(field);
            }
            else if (param.Type == typeof(int))
            {
                var field = new IntegerField();
                int.TryParse(TryPopParam(), out int val);
                field.value = val;
                field.RegisterValueChangedCallback((evt) => OnParameterModified());
                _paramsContainer.Add(field);
            }
            else if (param.Type == typeof(float))
            {
                var field = new FloatField();
                float.TryParse(TryPopParam(), out float val);
                field.value = val;
                field.RegisterValueChangedCallback((evt) => OnParameterModified());
                _paramsContainer.Add(field);
            }
            else
            {
                var field = new TextField();
                field.value = TryPopParam();
                field.RegisterValueChangedCallback((evt) => OnParameterModified());
                _paramsContainer.Add(field);
            }
        }

        _nameLabel.SetEnabled(data.Active);
        foreach (var item in _paramsContainer.Children())
        {
            item.SetEnabled(data.Active);
        }

        OnParameterModified();

        string TryPopParam()
        {
            if (data.Parameters == null)
                return default;

            if (paramIndex >= data.Parameters.Length)
                return default;
            
            return data.Parameters[paramIndex++];
        }
    }

    private void OnParameterModified()
    {
        _widgetData.Parameters = new string[_paramsContainer.childCount];

        int i = 0;
        foreach (var item in _paramsContainer.Children())
        {
            switch (item)
            {
                case Toggle toggle:
                    _widgetData.Parameters[i] = toggle.value.ToString();
                    break;
                case IntegerField intField:
                    _widgetData.Parameters[i] = intField.value.ToString();
                    break;
                case FloatField floatField:
                    _widgetData.Parameters[i] = floatField.value.ToString();
                    break;
                case TextField textField:
                    _widgetData.Parameters[i] = textField.value;
                    break;
            }
            i++;
        }

        _widgetData?.OnParameterModified?.Invoke(this);
    }
}