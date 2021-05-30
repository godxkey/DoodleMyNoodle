using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCC.Debug
{
    public interface IWindow
    {
        int Id { get; }
        Rect Rect { get; set; }
        WindowSettings Settings { get; set; }

        void Close();
    }

    public struct WindowSettings
    {
        public string Title;
        public Vector2 InitialSize;
        public Vector2 InitialPosition;
        public bool Draggable;
        public bool Resizable;
        public bool DestroyIfLoseFocus;
        public bool PopupWindowStyle;

        public static WindowSettings Default(string title)
        {
            return Default(title, defaultSize: new Vector2(200, 100));
        }

        public static WindowSettings Default(string title, Vector2 defaultSize)
        {
            return new WindowSettings()
            {
                Title = title,
                InitialPosition = new Vector2(100, 100),
                InitialSize = defaultSize,
                Draggable = true,
                Resizable = true,
                DestroyIfLoseFocus = false,
                PopupWindowStyle = false
            };
        }

        public static WindowSettings DropdownPopup(Vector2 position, Vector2 size)
        {
            return new WindowSettings()
            {
                Title = "",
                InitialPosition = position,
                InitialSize = size,
                Draggable = false,
                Resizable = false,
                DestroyIfLoseFocus = true,
                PopupWindowStyle = true
            };
        }
    }

    public delegate void WindowDrawDelegate(IWindow window);


    public static class WindowManager
    {
        class Window : IWindow
        {
            public int Id;
            public Rect Rect;
            public WindowDrawDelegate DrawFunction;
            public WindowSettings Settings;

            public bool IsResizingHorizontal;
            public bool IsResizingVertical;

            int IWindow.Id => Id;
            Rect IWindow.Rect { get => Rect; set => Rect = value; }
            WindowSettings IWindow.Settings { get => Settings; set => Settings = value; }

            void IWindow.Close()
            {
                WindowManager.DestroyWindow(this);
            }
        }

        private static List<Window> s_windows = new List<Window>();
        private static int s_nextWindowId = NULL_WINDOW_ID + 1;
        private static List<Window> s_postponedDestroys = new List<Window>();
        private static bool s_isIteratingOnWindows = false;
        private static bool s_init = false;
        private static int s_focusedWindow;
        private static int s_setFocusOnWindow = NULL_WINDOW_ID;

        private static GUIStyle s_styleWindow = null;
        private static GUIStyle s_stylePopupWindow = null;
        private static GUIStyle s_styleLabel = null;
        private const float WINDOW_EDGE_RADIUS = 7;
        private const int NULL_WINDOW_ID = -1;

        private static void InitGUI()
        {
            s_init = true;
            InitStyles();
        }

        [Updater.StaticUpdateMethod(UpdateType.GUI)]
        public static void OnGUI()
        {
            if (!s_init)
            {
                InitGUI();
            }

            foreach (Window window in BeginWindowIteration())
            {
                GUIStyle style = window.Settings.PopupWindowStyle ? s_stylePopupWindow : s_styleWindow;
                window.Rect = GUI.Window(window.Id, window.Rect, WindowFunction, window.Settings.Title, style);
            }
            EndWindowIteration();

            if (s_setFocusOnWindow != NULL_WINDOW_ID)
            {
                GUI.FocusWindow(s_setFocusOnWindow);
                s_setFocusOnWindow = NULL_WINDOW_ID;
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                s_focusedWindow = NULL_WINDOW_ID;
                OnWindowFocusChange();
            }
        }

        // CAREFUL! This will not be called if the user uses 'GUI.FocusWindow(id)'
        private static void OnWindowFocusChange()
        {
            foreach (Window window in BeginWindowIteration())
            {
                if (window.Settings.DestroyIfLoseFocus && s_focusedWindow != window.Id)
                {
                    DestroyWindow(window);
                }
            }
            EndWindowIteration();
        }

        private static void WindowFunction(int id)
        {
            ////////////////////////////////////////////////////////////////////////////////////////
            //      Begin
            ////////////////////////////////////////////////////////////////////////////////////////
            PushStyles();

            Window window = GetWindow(id);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                s_focusedWindow = id;
                OnWindowFocusChange();
            }


            ////////////////////////////////////////////////////////////////////////////////////////
            //      Handle Window Resize
            ////////////////////////////////////////////////////////////////////////////////////////

            if (window.Settings.Resizable)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 mouse = GetGUISpaceMousePosition();

                    if (mouse.y > window.Rect.yMin &&
                        mouse.y < window.Rect.yMax &&
                        Mathf.Abs(mouse.x - window.Rect.xMax) < WINDOW_EDGE_RADIUS)
                    {
                        window.IsResizingHorizontal = true;
                    }

                    if (mouse.x > window.Rect.xMin &&
                        mouse.x < window.Rect.xMax &&
                        Mathf.Abs(mouse.y - window.Rect.yMax) < WINDOW_EDGE_RADIUS)
                    {
                        window.IsResizingVertical = true;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                window.IsResizingHorizontal = false;
                window.IsResizingVertical = false;
            }

            if (window.Settings.Resizable)
            {
                if (window.IsResizingVertical)
                {
                    window.Rect.yMax = GetGUISpaceMousePosition().y;
                }

                if (window.IsResizingHorizontal)
                {
                    window.Rect.xMax = GetGUISpaceMousePosition().x;
                }
            }


            ////////////////////////////////////////////////////////////////////////////////////////
            //      Draw Function
            ////////////////////////////////////////////////////////////////////////////////////////
            window.DrawFunction.Invoke(window);


            ////////////////////////////////////////////////////////////////////////////////////////
            //      Handle Window Drag
            ////////////////////////////////////////////////////////////////////////////////////////
            if (window.Settings.Draggable && !window.IsResizingHorizontal && !window.IsResizingVertical)
            {
                GUI.DragWindow();
            }


            ////////////////////////////////////////////////////////////////////////////////////////
            //      End
            ////////////////////////////////////////////////////////////////////////////////////////
            PopStyles();
        }

        private static Vector2 GetGUISpaceMousePosition()
        {
            return new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        }

        #region Styles
        private static List<GUISkinOverride> s_guiSkinOverrides = new List<GUISkinOverride>();

        private static void InitStyles()
        {
            s_styleWindow = new GUIStyle(GUI.skin.window);
            s_styleLabel = new GUIStyle(GUI.skin.label);
            //s_styleLabel.padding.top = 0;
            //s_styleLabel.padding.bottom = 0;
            //s_styleLabel.margin.top = 0;
            //s_styleLabel.margin.bottom = 0;
            s_styleLabel.normal.textColor = Color.white;

            s_stylePopupWindow = new GUIStyle(GUI.skin.box);
            s_stylePopupWindow.padding.top = 0;
            s_stylePopupWindow.padding.bottom = 0;
            s_stylePopupWindow.padding.left = 0;
            s_stylePopupWindow.padding.right = 0;
        }

        private static void PushStyles()
        {
            s_guiSkinOverrides.Add(GUISkinOverride.Push(GlobalGUIStyle.Label, s_styleLabel));
        }

        private static void PopStyles()
        {
            for (int i = 0; i < s_guiSkinOverrides.Count; i++)
            {
                s_guiSkinOverrides[i].Dispose();
            }
            s_guiSkinOverrides.Clear();
        }
        #endregion

        #region Window Management
        public static IWindow CreateWindow(WindowSettings settings, WindowDrawDelegate drawFunction)
        {
            Window newWindow = new Window()
            {
                Id = s_nextWindowId++,
                Rect = new Rect(settings.InitialPosition, settings.InitialSize),
                DrawFunction = drawFunction,
                Settings = settings
            };

            s_setFocusOnWindow = newWindow.Id;

            s_windows.Add(newWindow);

            return newWindow;
        }

        public static void DestroyWindow(IWindow window)
        {
            if (window is Window w)
            {
                if (s_isIteratingOnWindows)
                {
                    s_postponedDestroys.Add(w);
                }
                else
                {
                    DestroyImmediate(w);
                }
            }
        }

        private static Window GetWindow(int id)
        {
            for (int i = 0; i < s_windows.Count; i++)
            {
                if (s_windows[i].Id == id)
                    return s_windows[i];
            }
            return null;
        }

        private static void DestroyImmediate(Window window)
        {
            s_windows.Remove(window as Window);
        }

        private static List<Window> BeginWindowIteration()
        {
            s_isIteratingOnWindows = true;
            return s_windows;
        }

        private static void EndWindowIteration()
        {
            s_isIteratingOnWindows = false;

            foreach (Window window in s_postponedDestroys)
            {
                DestroyImmediate(window);
            }
        }
        #endregion
    }
}

