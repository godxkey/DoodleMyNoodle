using System;
using UnityEngine;

namespace CCC.Debug
{
    public enum GlobalGUIStyle
    {
        None,
        ScrollView,
        VerticalScrollbarDownButton,
        VerticalScrollbarThumb,
        VerticalScrollbar,
        HorizontalScrollbarRightButton,
        HorizontalScrollbarLeftButton,
        HorizontalScrollbarThumb,
        HorizontalScrollbar,
        VerticalSliderThumb,
        VerticalSlider,
        HorizontalSliderThumb,
        Window,
        Toggle,
        Button,
        TextArea,
        TextField,
        Label,
        Box,
    }

    public struct GUISkinOverride : IDisposable
    {
        private GlobalGUIStyle _style;
        private GUIStyle _oldStyle;

        public static GUISkinOverride Push(GlobalGUIStyle guiStyle, GUIStyle style)
        {
            GUIStyle oldStyle = null;
            switch (guiStyle)
            {
                case GlobalGUIStyle.ScrollView:
                    oldStyle = GUI.skin.scrollView;
                    GUI.skin.scrollView = style;
                    break;
                case GlobalGUIStyle.VerticalScrollbarDownButton:
                    oldStyle = GUI.skin.verticalScrollbarDownButton;
                    GUI.skin.verticalScrollbarDownButton = style;
                    break;
                case GlobalGUIStyle.VerticalScrollbarThumb:
                    oldStyle = GUI.skin.verticalScrollbarThumb;
                    GUI.skin.verticalScrollbarThumb = style;
                    break;
                case GlobalGUIStyle.VerticalScrollbar:
                    oldStyle = GUI.skin.verticalScrollbar;
                    GUI.skin.verticalScrollbar = style;
                    break;
                case GlobalGUIStyle.HorizontalScrollbarRightButton:
                    oldStyle = GUI.skin.horizontalScrollbarRightButton;
                    GUI.skin.horizontalScrollbarRightButton = style;
                    break;
                case GlobalGUIStyle.HorizontalScrollbarLeftButton:
                    oldStyle = GUI.skin.horizontalScrollbarLeftButton;
                    GUI.skin.horizontalScrollbarLeftButton = style;
                    break;
                case GlobalGUIStyle.HorizontalScrollbarThumb:
                    oldStyle = GUI.skin.horizontalScrollbarThumb;
                    GUI.skin.horizontalScrollbarThumb = style;
                    break;
                case GlobalGUIStyle.HorizontalScrollbar:
                    oldStyle = GUI.skin.horizontalScrollbar;
                    GUI.skin.horizontalScrollbar = style;
                    break;
                case GlobalGUIStyle.VerticalSliderThumb:
                    oldStyle = GUI.skin.verticalSliderThumb;
                    GUI.skin.verticalSliderThumb = style;
                    break;
                case GlobalGUIStyle.VerticalSlider:
                    oldStyle = GUI.skin.verticalSlider;
                    GUI.skin.verticalSlider = style;
                    break;
                case GlobalGUIStyle.HorizontalSliderThumb:
                    oldStyle = GUI.skin.horizontalSliderThumb;
                    GUI.skin.horizontalSliderThumb = style;
                    break;
                case GlobalGUIStyle.Window:
                    oldStyle = GUI.skin.window;
                    GUI.skin.window = style;
                    break;
                case GlobalGUIStyle.Toggle:
                    oldStyle = GUI.skin.toggle;
                    GUI.skin.toggle = style;
                    break;
                case GlobalGUIStyle.Button:
                    oldStyle = GUI.skin.button;
                    GUI.skin.button = style;
                    break;
                case GlobalGUIStyle.TextArea:
                    oldStyle = GUI.skin.textArea;
                    GUI.skin.textArea = style;
                    break;
                case GlobalGUIStyle.TextField:
                    oldStyle = GUI.skin.textField;
                    GUI.skin.textField = style;
                    break;
                case GlobalGUIStyle.Label:
                    oldStyle = GUI.skin.label;
                    GUI.skin.label = style;
                    break;
                case GlobalGUIStyle.Box:
                    oldStyle = GUI.skin.box;
                    GUI.skin.box = style;
                    break;
            }

            return new GUISkinOverride()
            {
                _oldStyle = oldStyle,
                _style = guiStyle
            };
        }

        public void Dispose()
        {
            switch (_style)
            {
                case GlobalGUIStyle.ScrollView:
                    GUI.skin.scrollView = _oldStyle;
                    break;
                case GlobalGUIStyle.VerticalScrollbarDownButton:
                    GUI.skin.verticalScrollbarDownButton = _oldStyle;
                    break;
                case GlobalGUIStyle.VerticalScrollbarThumb:
                    GUI.skin.verticalScrollbarThumb = _oldStyle;
                    break;
                case GlobalGUIStyle.VerticalScrollbar:
                    GUI.skin.verticalScrollbar = _oldStyle;
                    break;
                case GlobalGUIStyle.HorizontalScrollbarRightButton:
                    GUI.skin.horizontalScrollbarRightButton = _oldStyle;
                    break;
                case GlobalGUIStyle.HorizontalScrollbarLeftButton:
                    GUI.skin.horizontalScrollbarLeftButton = _oldStyle;
                    break;
                case GlobalGUIStyle.HorizontalScrollbarThumb:
                    GUI.skin.horizontalScrollbarThumb = _oldStyle;
                    break;
                case GlobalGUIStyle.HorizontalScrollbar:
                    GUI.skin.horizontalScrollbar = _oldStyle;
                    break;
                case GlobalGUIStyle.VerticalSliderThumb:
                    GUI.skin.verticalSliderThumb = _oldStyle;
                    break;
                case GlobalGUIStyle.VerticalSlider:
                    GUI.skin.verticalSlider = _oldStyle;
                    break;
                case GlobalGUIStyle.HorizontalSliderThumb:
                    GUI.skin.horizontalSliderThumb = _oldStyle;
                    break;
                case GlobalGUIStyle.Window:
                    GUI.skin.window = _oldStyle;
                    break;
                case GlobalGUIStyle.Toggle:
                    GUI.skin.toggle = _oldStyle;
                    break;
                case GlobalGUIStyle.Button:
                    GUI.skin.button = _oldStyle;
                    break;
                case GlobalGUIStyle.TextArea:
                    GUI.skin.textArea = _oldStyle;
                    break;
                case GlobalGUIStyle.TextField:
                    GUI.skin.textField = _oldStyle;
                    break;
                case GlobalGUIStyle.Label:
                    GUI.skin.label = _oldStyle;
                    break;
                case GlobalGUIStyle.Box:
                    GUI.skin.box = _oldStyle;
                    break;
            }
        }
    }
}
