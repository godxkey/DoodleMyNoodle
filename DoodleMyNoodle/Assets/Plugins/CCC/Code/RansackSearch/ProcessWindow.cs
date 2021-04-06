using System.IO;
using UnityEditor;
using UnityEngine;

public static partial class RansackSearch
{
    public class ProcessWindow : EditorWindow
    {
        private RansackQuery _query;
        private bool _done = false;
        private RansackQuery.Result _result;
        private Vector2 _scrollPosition;

        private static GUIStyle s_doneStyle;
        private static GUIStyle s_searchingStyle;
        private static GUIStyle s_fileDirectoryStyle;
        private static GUIStyle s_fileNameStyle;
        private static GUIStyle s_fileStyle;

        private static void InitStyles()
        {
            if (EditorStyles.label == null)
                return;

            Color transparentTextColor = EditorStyles.label.normal.textColor;
            transparentTextColor.a = 0.5f;

            if (s_doneStyle == null)
            {
                s_doneStyle = new GUIStyle(EditorStyles.label);
                s_doneStyle.normal.textColor = Color.Lerp(EditorStyles.label.normal.textColor, Color.green, 0.5f);
                s_doneStyle.fontStyle = FontStyle.Bold;
            }

            if (s_searchingStyle == null)
            {
                s_searchingStyle = new GUIStyle(EditorStyles.label);
                s_searchingStyle.normal.textColor = transparentTextColor;
                s_searchingStyle.fontStyle = FontStyle.Bold;
            }

            if (s_fileDirectoryStyle == null)
            {
                s_fileDirectoryStyle = new GUIStyle(EditorStyles.label);
                s_fileDirectoryStyle.normal.textColor = transparentTextColor;
                s_fileDirectoryStyle.fontStyle = FontStyle.Italic;
            }

            if (s_fileNameStyle == null)
            {
                s_fileNameStyle = new GUIStyle(EditorStyles.boldLabel);
            }

            if (s_fileStyle == null)
            {
                s_fileStyle = new GUIStyle(EditorStyles.toolbarButton);
                s_fileStyle.fixedHeight = 0;
            }
        }

        void OnGUI()
        {
            InitStyles();

            if (_query != null)
            {
                var fileName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(_query.Search));
                EditorGUILayout.LabelField($"References to: {fileName}");
            }

            if (_done)
            {
                Rect rect = new Rect(position.width - 50, 0, 50, 20);
                GUI.Label(rect, "Done ✓", s_doneStyle);
            }
            else
            {
                Rect rect = new Rect(position.width - 75, 0, 75, 20);
                string searchingString = "Searching";
                for (uint i = 0; i < (uint)(EditorApplication.timeSinceStartup * 2) % 4; i++)
                {
                    searchingString += '.';
                }
                GUI.Label(rect, searchingString, s_searchingStyle);
            }

            if (_result != null)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                foreach (var item in _result.ResultEntries)
                {
                    var rect = EditorGUILayout.BeginVertical(s_fileStyle);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(item.File, s_fileNameStyle, GUILayout.ExpandWidth(false));
                    GUILayout.Space(20);
                    GUILayout.Label($"({item.CompleteProjectPath})", s_fileDirectoryStyle);
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel++;
                    foreach (var line in item.Lines)
                    {
                        EditorGUILayout.LabelField(line);
                    }
                    EditorGUI.indentLevel--;

                    EditorGUILayout.EndVertical();

                    if (Event.current != null && Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                    {
                        PingAsset(item);
                    }

                    EditorGUILayout.Space();
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void PingAsset(RansackQuery.ResultEntry item)
        {
            string assetPath = item.AssetPath;
            if (assetPath.EndsWith(".meta"))
                assetPath = AssetDatabase.GetAssetPathFromTextMetaFilePath(assetPath);

            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

            if (asset != null)
            {
                EditorGUIUtility.PingObject(asset);
                Selection.SetActiveObjectWithContext(asset, null);
            }
        }

        private void OnInspectorUpdate()
        {
            if (_query == null)
            {
                Close();
                return;
            }

            // update result
            if (!_done)
                _result = _query.Read();

            _done = _query.IsDone();

            Repaint();
        }

        private void OnDisable()
        {
            _query?.Dispose();
        }

        public static void Show(RansackQuery ransackQuery)
        {
            var window = GetWindow<ProcessWindow>(utility: true, title: "Ransack Search");
            window._query?.Dispose();
            window._query = ransackQuery;
            window.Show();
        }
    }
}
