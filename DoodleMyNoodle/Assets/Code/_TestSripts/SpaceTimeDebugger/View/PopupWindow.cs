using System;
using UnityEngine;

namespace CCC.Debug
{
    public class PopupWindow : IDisposable
    {
        private readonly IWindow _window;
        private readonly string[] _items;
        private readonly Action<int> _onItemPicked;

        private static GUIStyle s_buttonStyle = null;

        public PopupWindow(Rect buttonRect, string[] items, Action<int> onItemPicked)
        {
            Vector2 size = new Vector2(buttonRect.width, items.Length * 21);
            Vector2 position = buttonRect.position + Vector2.up * buttonRect.height;

            _window = WindowManager.CreateWindow(WindowSettings.DropdownPopup(position, size), Draw);
            _items = items;
            _onItemPicked = onItemPicked;

            if (s_buttonStyle == null)
            {
                s_buttonStyle = new GUIStyle(GUI.skin.button);
                s_buttonStyle.margin.top = 0;
                s_buttonStyle.margin.left = 0;
                s_buttonStyle.margin.right = 0;
                s_buttonStyle.margin.bottom = 0;
            }
        }

        private void Draw(IWindow window)
        {
            int itemPicked = -1;
            for (int i = 0; i < _items.Length; i++)
            {
                if (GUILayout.Button(_items[i], s_buttonStyle))
                {
                    itemPicked = i;
                }
            }

            if (itemPicked != -1)
            {
                _onItemPicked.Invoke(itemPicked);
                window.Close();
            }
        }

        public void Dispose()
        {
            _window.Close();
        }
    }
}
