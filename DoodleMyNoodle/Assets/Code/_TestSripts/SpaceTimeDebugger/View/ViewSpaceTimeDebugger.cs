using CCC.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCC.Debug
{
    public class ViewSpaceTimeDebugger
    {
        private ModelSpaceTimeDebugger _model;

        private DropdownButton _currentClockButton = null;
        private List<ViewDisplayedStreamSettings> _viewDisplayedStreamSettings = new List<ViewDisplayedStreamSettings>();
        private GraphDrawer _graphDrawer = new GraphDrawer();
        private Rect _graphRect;

        private float _inspectorWidth = 70;

        private static GUIStyle s_graphBoxStyle;
        private static bool s_initStyle = false;

        public ViewSpaceTimeDebugger(ModelSpaceTimeDebugger model)
        {
            _model = model;

            var window = WindowManager.CreateWindow(WindowSettings.Default("Space Time Debugger", new Vector2(450, 200)), Draw);
            _graphDrawer.Points.Add(new ColoredPoint() { position = Vector2.one * 10, color = Color.white });
            _graphDrawer.Points.Add(new ColoredPoint() { position = Vector2.one * 20, color = Color.white });
            _graphDrawer.Points.Add(new ColoredPoint() { position = Vector2.one * 30, color = Color.white });

            _currentClockButton = new DropdownButton("Clock", _model.AvailableClocks.Select(c => c.Name).ToArray());
            _currentClockButton.ItemPickedCallback = OnClockPicked;
        }

        private void OnClockPicked(int i)
        {
            _model.DisplayedClock = _model.AvailableClocks[i];
        }

        private void Draw(IWindow window)
        {
            if (!s_initStyle)
            {
                InitStyle();
            }

            using(new GUILayout.HorizontalScope())
            {
                GUILayout.Box(GUIContent.none, s_graphBoxStyle, GUILayout.ExpandHeight(true));

                if (Event.current.type == EventType.Repaint)
                {
                    _graphRect = GUILayoutUtility.GetLastRect();
                    _graphRect.position += window.Rect.position;
                }

                _graphDrawer.ScreenDisplayRect = _graphRect;
                _graphDrawer.Draw();

                using (new GUILayout.VerticalScope(GUILayout.Width(_inspectorWidth)))
                {
                    _currentClockButton.CurrentItemIndex = _model.AvailableClocks.IndexOf(_model.DisplayedClock);
                    _currentClockButton.Draw(window);

                    _model.FixedHorizontalSpacing = GUILayout.Toggle(_model.FixedHorizontalSpacing, "Fixed Horizontal Spacing");

                    GUILayout.Label($"Displayed Streams");


                    for (int i = 0; i < _model.DisplayedStreams.Count; i++)
                    {
                        if (i >= _viewDisplayedStreamSettings.Count)
                            _viewDisplayedStreamSettings.Add(new ViewDisplayedStreamSettings());

                        _viewDisplayedStreamSettings[i].Draw(_model, _model.DisplayedStreams[i], window);
                    }
                }
            }
        }

        private static void InitStyle()
        {
            s_initStyle = true;
            s_graphBoxStyle = new GUIStyle(GUI.skin.box);
            s_graphBoxStyle.padding.left = 0;
            s_graphBoxStyle.padding.top = 0;
            s_graphBoxStyle.padding.bottom = 0;
            s_graphBoxStyle.padding.right = 0;
            s_graphBoxStyle.margin.left = 0;
            s_graphBoxStyle.margin.top = 0;
            s_graphBoxStyle.margin.bottom = 0;
            s_graphBoxStyle.margin.right = 0;
        }
    }

    public class ViewDisplayedStreamSettings
    {
        private DropdownButton _verticalSpaceDropdownButton;

        private static readonly GUIContent s_settingsButtonOn = new GUIContent("v");
        private static readonly GUIContent s_settingsButtonOff = new GUIContent(">");
        
        private static readonly GUIContent s_removeButtonContent = new GUIContent("x");

        private static readonly string[] s_verticalSpaceOptions = new string[] { "Global", "Local" };
        private static readonly GUIContent s_verticalSpaceLabel = new GUIContent("Vertical Space");

        public ViewDisplayedStreamSettings()
        {
            _verticalSpaceDropdownButton = new DropdownButton(s_verticalSpaceLabel, s_verticalSpaceOptions);
        }

        public void Draw(ModelSpaceTimeDebugger modelSpaceTimeDebugger, ModelStream modelStream, IWindow window)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(modelStream.DisplaySettings ? s_settingsButtonOn : s_settingsButtonOff, GUILayout.Width(18)))
            {
                modelStream.DisplaySettings = !modelStream.DisplaySettings;
            }

            GUILayout.Label(modelStream.Name);

            if (GUILayout.Button(s_removeButtonContent, GUILayout.Width(18)))
            {
                modelSpaceTimeDebugger.DisplayedStreams.Remove(modelStream);
            }

            GUILayout.EndHorizontal();

            if (modelStream.DisplaySettings)
            {
                _verticalSpaceDropdownButton.Draw(window);
            }
        }
    }

    [System.Serializable]
    public class ModelSpaceTimeDebugger
    {
        public bool FixedHorizontalSpacing;

        public List<ModelClock> AvailableClocks = new List<ModelClock>();
        public ModelClock DisplayedClock;

        public List<ModelStream> AvailableStreams = new List<ModelStream>();
        public List<ModelStream> DisplayedStreams = new List<ModelStream>();

        public List<ModelLog> Logs = new List<ModelLog>();
    }

    [System.Serializable]
    public class ModelClock
    {
        public string Name;
    }

    [System.Serializable]
    public class ModelStream
    {
        public string Name;
        public bool GlobalVerticalSpace;
        public bool DisplaySettings;

        public float GetValueAt(float time) { return 1; } // todo
    }

    [System.Serializable]
    public class ModelLog
    {
        public string Text;
        public LogType LogType;
        public float Time;
    }
}

