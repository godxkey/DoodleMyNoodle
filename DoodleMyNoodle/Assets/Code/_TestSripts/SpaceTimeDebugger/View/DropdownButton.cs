using System;
using UnityEngine;

namespace CCC.Debug
{
    public class DropdownButton
    {
        public string[] Items;
        public int CurrentItemIndex;
        public Action<int> ItemPickedCallback;
        public GUIContent Label;

        private static GUIStyle s_styleLabel;
        private Rect _latestButtonRect;
        
        public string CurrentItem => IsCurrentItemValid ? Items[CurrentItemIndex] : null;
        private bool IsCurrentItemValid => CurrentItemIndex >= 0 && CurrentItemIndex < Items.Length;

        public DropdownButton(string label, string[] items)
        {
            Label = new GUIContent(label);
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }
        public DropdownButton(GUIContent label, string[] items)
        {
            Label = label;
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }
        public DropdownButton(string[] items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        private static void TryInitStyle()
        {
            if (s_styleLabel == null)
            {
                s_styleLabel = new GUIStyle(GUI.skin.label);
                s_styleLabel.padding.top = 4;
                s_styleLabel.padding.bottom = 4;
                s_styleLabel.margin.top = 3;
                s_styleLabel.margin.bottom = 2;
                s_styleLabel.normal.textColor = Color.white;
            }
        }

        public void Draw(IWindow window)
        {
            TryInitStyle();

            if(Label != null)
            {
                GUILayout.BeginHorizontal();

                Vector2 labelSize = s_styleLabel.CalcSize(Label);
                GUILayout.Label(Label, s_styleLabel, GUILayout.Width(labelSize.x));
            }

            if (GUILayout.Button(IsCurrentItemValid ? Items[CurrentItemIndex] : "[Invalid]"))
            {
                new PopupWindow(_latestButtonRect, Items, OnItemPicked);
            }

            if (Event.current.type == EventType.Repaint)
            {
                _latestButtonRect = GUILayoutUtility.GetLastRect();
                _latestButtonRect.position += window.Rect.position;
            }

            if (Label != null)
            {
                GUILayout.EndHorizontal();
            }
        }

        private void OnItemPicked(int index)
        {
            CurrentItemIndex = index;
            ItemPickedCallback?.Invoke(index);
        }
    }
}
