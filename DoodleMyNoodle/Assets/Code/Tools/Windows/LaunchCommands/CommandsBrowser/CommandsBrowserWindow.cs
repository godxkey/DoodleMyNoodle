using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorX;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngineX;

public class CommandsBrowserWindow : ToolsWindowBase
{
    private class CommandVisualElement : ToolsVisualElementBase
    {
        private bool _mouseIn;

        public Label Label { get; private set; }
        public Button AddButton { get; private set; }
        public Label AddedLabel { get; private set; }

        public int Index { get; set; }
        public Func<CommandVisualElement, bool> IsAddedFunc { get; set; }

        public override string UxmlGuid => "61b26d60d852aa74db02bfb5f828e1b8";
        public override string UssGuid => "7fec3481a0d44f6438350a5cec638151";

        public CommandVisualElement()
        {
            Label = this.Q<Label>("label");
            AddButton = this.Q<Button>("addButton");
            AddedLabel = this.Q<Label>("addedLabel");

            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            RefreshAddIndicators();
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            _mouseIn = true;
            RefreshAddIndicators();
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            _mouseIn = false;
            RefreshAddIndicators();
        }

        public void RefreshAddIndicators()
        {
            ShowAddButon(_mouseIn && IsAddedFunc != null && !IsAddedFunc.Invoke(this));
            ShowAddedLabel(_mouseIn && IsAddedFunc != null && IsAddedFunc.Invoke(this));
        }

        private void ShowAddButon(bool shown)
        {
            AddButton.visible = shown;
            //if (shown)
            //{
            //    if (!Contains(AddButton))
            //        Add(AddButton);
            //}
            //else
            //    Remove(AddButton);

            //Label.BringToFront();
        }

        private void ShowAddedLabel(bool shown)
        {
            AddedLabel.visible = shown;
            //if (shown)
            //{
            //    if (!Contains(AddedLabel))
            //        Add(AddedLabel);
            //}
            //else
            //    Remove(AddedLabel);

            //Label.BringToFront();
        }

        private new void Remove(VisualElement e)
        {
            int i = IndexOf(e);
            if (i != -1)
                RemoveAt(i);
        }
    }


    public delegate bool IsCommandAdded(IGameConsoleInvokable invokable);
    public delegate void AddCommand(IGameConsoleInvokable invokable);

    private IsCommandAdded _isCommandAdded;
    private AddCommand _addCommand;

    protected override string UssGuid => "8846fcb6999963c4a85b55f365a444c8";
    protected override string UxmlGuid => "af695a517a030c1489d3e5f69eeb21f9";

    private static IGameConsoleInvokable[] s_invokables;

    private List<IGameConsoleInvokable> _displayedInvokables = new List<IGameConsoleInvokable>();
    private ToolbarSearchField _searchField;
    private GameConsoleInvokableSearcher _searcher = new GameConsoleInvokableSearcher();
    private ListView _listView;

    protected override void Rebuild(VisualElement root)
    {
        if (s_invokables == null)
        {
            s_invokables = GameConsole.Invokables.ToArray();
        }

        _searchField = root.Q<ToolbarSearchField>("searchField");
        _searcher.FilterDisabled = false;
        _searchField.RegisterValueChangedCallback((e) => OnSearchValueChanged(e.newValue));

        _listView = root.Q<ListView>("invokablesContainer");
        _listView.selectionType = SelectionType.None;
        _listView.itemHeight = 18;
        _listView.makeItem = CreateElement;
        _listView.bindItem = (v, i) => Bind((CommandVisualElement)v, i);
        _listView.itemsSource = _displayedInvokables;
        _listView.style.flexGrow = 1;
        
        OnSearchValueChanged(_searchField.value);
    }

    private void OnSearchValueChanged(string txt)
    {
        if(string.IsNullOrEmpty(txt) || string.IsNullOrWhiteSpace(txt))
        {
            _displayedInvokables.Clear();
            _displayedInvokables.AddRange(s_invokables);
        }
        else
        {
            _searcher.GetSuggestions(s_invokables, txt, _displayedInvokables);
        }

        _listView.Refresh();
    }

    private VisualElement CreateElement()
    {
        var element = new CommandVisualElement();
        element.AddButton.clicked += () => OnVisualElementClicked(element);
        if (_isCommandAdded != null)
            element.IsAddedFunc = IsAdded;
        return element;
    }

    private bool IsAdded(CommandVisualElement element)
    {
        if (_isCommandAdded != null)
            return _isCommandAdded.Invoke(_displayedInvokables[element.Index]);
        return false;
    }

    private void OnVisualElementClicked(CommandVisualElement element)
    {
        _addCommand?.Invoke(_displayedInvokables[element.Index]);
        UpdateView(element, element.Index);
    }

    private void Bind(CommandVisualElement visualElement, int index)
    {
        visualElement.Index = index;
        UpdateView(visualElement, index);
    }

    private void UpdateView(CommandVisualElement visualElement, int index)
    {
        visualElement.Label.text = _displayedInvokables[index].DisplayName;
        visualElement.RefreshAddIndicators();
    }

    public static void ShowWindow(IsCommandAdded isCommandAdded, AddCommand addCommand)
    {
        CommandsBrowserWindow wnd = GetWindow<CommandsBrowserWindow>(true);
        wnd._isCommandAdded = isCommandAdded;
        wnd._addCommand = addCommand;
        wnd.ShowPopup();
        wnd._searchField.Q("unity-text-input").Focus();
    }
}