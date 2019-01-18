using UnityEngine;
using UnityEditor;
using System.Collections;

namespace NestedTODO
{
    public class BurndownChartWindow : EditorWindow
    {
        const int MaxZoom = 5;
        int zoom = 1;
        float maxPoints;
        Vector2 scrollPosition;

        //velocit measured in days (the amounts of points you estimated will be completed per day)
        float estimatedVelocity = 0;
        bool showEstimationLines = true;
        
        static Texture2D contractIcon;
        static Texture2D expandIcon;

        
        //[MenuItem(DefaultConfiguration.BurndownChartWindowRoute)]
        public static void openWindow()
        {
            BurndownChartWindow window = (BurndownChartWindow)EditorWindow.GetWindow<BurndownChartWindow>();
#if UNITY_5_3_OR_NEWER
            window.titleContent.text = "Burndown Chart";
#else
            window.title = "Burndown Chart";
#endif
            window.minSize = new Vector2(280, 200);
        }

        //[MenuItem(DefaultConfiguration.BurndownChartWindowRoute, true, 2)]
        static bool ValidateOpenWindow()
        {
            return AgileBoardWindow.nestedList != null;
        }
        

        void OnFocus()
        {
            if (AgileBoardWindow.nestedList != null)
                AgileBoardWindow.nestedList.UpdateBurndownChart();
            maxPoints = GetHighestValue();
        }

        void OnEnable()
        {
            MonoScript ms = MonoScript.FromScriptableObject(this);
            var rootFolderPath = AssetDatabase.GetAssetPath(ms).Replace("Scripts/Editor/BurndownChartWindow.cs", "");

            contractIcon = AssetDatabase.LoadAssetAtPath(rootFolderPath + "Icons/contract.png", typeof(Texture2D)) as Texture2D;
            expandIcon = AssetDatabase.LoadAssetAtPath(rootFolderPath + "Icons/expand.png", typeof(Texture2D)) as Texture2D;

            if (EditorPrefs.HasKey("estimatedVelocity"))
                estimatedVelocity = Mathf.Max(EditorPrefs.GetFloat("estimatedVelocity"),0);

            if (EditorPrefs.HasKey("showEstimationLines"))
                showEstimationLines = EditorPrefs.GetBool("showEstimationLines");
        }

        void OnGUI()
        {
            if (AgileBoardWindow.nestedList == null)
            {
                EditorGUILayout.HelpBox("There is no TODO List selected, please select one in the Agile Board Window or Checklist Window.", MessageType.Info);
                return;
            }

            if (maxPoints < 0)
                return;

            var data = AgileBoardWindow.nestedList.burndownData;
            float daysToFinish = GetEstimatedFinishDate(data);
            float realDaysToFinish = GetEstimatedFinishDate(data, false);

            float daysCount = daysToFinish > realDaysToFinish ? daysToFinish : realDaysToFinish;

            int offset = 8 * zoom;
            //draw graph
            Vector2 origin = new Vector2(offset, offset + 60);
            float baseLine = origin.y + zoom * maxPoints;
            int barWidht = 8 * zoom;
            int spaceBetweenBars = 1 * zoom;
            Rect tmpRect;
            
            scrollPosition = GUI.BeginScrollView(new Rect(0, 0, this.position.width, this.position.height), scrollPosition, new Rect(0, 0, offset + (data.dateValues.Count + daysCount) * (barWidht + spaceBetweenBars) + 2 * offset, maxPoints * zoom + 170));

            //draw axis
            EditorGUI.DrawRect(new Rect(origin.x, origin.y - offset, 1, zoom * maxPoints + offset), Color.black);   // y axis
            EditorGUI.DrawRect(new Rect(origin.x - 10, baseLine, offset + (data.dateValues.Count + daysCount) * (barWidht + spaceBetweenBars), 1), Color.black);  // x axis

            for (int i = 0; i < data.dateValues.Count; i++)
            {
                Vector2 barPos = new Vector2((origin.x + spaceBetweenBars) + (spaceBetweenBars + barWidht) * i, origin.y + zoom * (maxPoints - data.dateValues[i]));
                EditorGUI.DrawRect(new Rect(barPos.x, barPos.y, barWidht, data.dateValues[i] * zoom), Color.grey);
                
                //points marks
                if (zoom > 1)
                {
                    var tmpStyle = new GUIStyle(EditorStyles.label);
                    tmpStyle.alignment = TextAnchor.MiddleCenter;
                    tmpStyle.clipping = TextClipping.Overflow;
                    tmpStyle.contentOffset = new Vector2(0, -20);
                    EditorGUI.LabelField(new Rect(barPos.x, barPos.y, barWidht, 20), data.dateValues[i].ToString(), tmpStyle);
                }

                var d = System.DateTime.Parse(data.date[i]);
                tmpRect = new Rect(origin.x + barWidht / 2 + (spaceBetweenBars + barWidht) * i + (zoom - 1), baseLine, 60, 20);
                EditorGUIUtility.RotateAroundPivot(90, new Vector2(tmpRect.x, tmpRect.y + tmpRect.height / 2));
                EditorGUI.LabelField(tmpRect, string.Format("{0:dd/MM/yy}", d), EditorStyles.miniLabel);
                EditorGUIUtility.RotateAroundPivot(-90, new Vector2(tmpRect.x, tmpRect.y + tmpRect.height / 2));
            }
            
            //add extra dates marks
            for (int i = 1; i <= daysCount; i++)
            {
                if (i == daysToFinish)
                    EditorGUI.DrawRect(new Rect(origin.x + barWidht/2 + spaceBetweenBars + (spaceBetweenBars + barWidht) * i, baseLine, 1, -3), Color.red);
                if (i == realDaysToFinish)
                    EditorGUI.DrawRect(new Rect(origin.x + barWidht/ 2 + spaceBetweenBars + (spaceBetweenBars + barWidht) * i, baseLine, 1, -3), Color.green);

                tmpRect = new Rect(origin.x + barWidht / 2 + (spaceBetweenBars + barWidht) * (i + data.dateValues.Count-1) + (zoom-1), baseLine, 60, 20);
                EditorGUIUtility.RotateAroundPivot(90, new Vector2(tmpRect.x, tmpRect.y + tmpRect.height / 2));
                EditorGUI.LabelField(tmpRect, string.Format("{0:dd/MM/yy}", System.DateTime.Now.AddDays(i)), EditorStyles.miniLabel);
                EditorGUIUtility.RotateAroundPivot(-90, new Vector2(tmpRect.x, tmpRect.y + tmpRect.height / 2));
            }

            if (showEstimationLines && data.dateValues[0] > 0)
            {
                var initialPoints = data.dateValues[0] * zoom;
                var distanceToFinish = GetEstimatedFinishDate(data) * (barWidht + spaceBetweenBars);
                var rotationAngle = GetRotationAngle(initialPoints, distanceToFinish);
                var lineLenght = Mathf.Sqrt(Mathf.Pow(initialPoints, 2) + Mathf.Pow(distanceToFinish, 2));

                Vector2 pivot = new Vector2(origin.x + (spaceBetweenBars + barWidht / 2), origin.y + zoom * (maxPoints - data.dateValues[0]));
                EditorGUIUtility.RotateAroundPivot(rotationAngle, pivot);
                EditorGUI.DrawRect(new Rect(pivot.x, pivot.y, lineLenght, 1), Color.red);
                EditorGUIUtility.RotateAroundPivot(-rotationAngle, pivot);
                

                distanceToFinish = GetEstimatedFinishDate(data, false) * (barWidht + spaceBetweenBars);
                rotationAngle = GetRotationAngle(initialPoints, distanceToFinish);
                lineLenght = Mathf.Sqrt(Mathf.Pow(initialPoints, 2) + Mathf.Pow(distanceToFinish, 2));

                EditorGUIUtility.RotateAroundPivot(rotationAngle, pivot);
                EditorGUI.DrawRect(new Rect(pivot.x, pivot.y, lineLenght, 1), Color.green);
                EditorGUIUtility.RotateAroundPivot(-rotationAngle, pivot);
            }

            GUI.EndScrollView();
            
            //tool box
            tmpRect = new Rect(this.position.width - 265, 5, 250, 50);
            EditorGUI.DrawRect(tmpRect, Color.black);
            tmpRect.x += 1;
            tmpRect.y += 1;
            tmpRect.width -= 2;
            tmpRect.height -= 2;
            EditorGUI.DrawRect(tmpRect, EditorGUIUtility.isProSkin ? new Color(.3f, .3f, .3f, 1) : new Color(.7f, .7f, .7f, 1));
            
            var backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.gray;
            tmpRect = new Rect(this.position.width - 60, 10, 40, 40);
            if (zoom == 1)
                GUI.enabled = false;
            if (GUI.Button(tmpRect, contractIcon))
            {
                zoom--;
                zoom = Mathf.Clamp(zoom, 1, MaxZoom);
            }
            GUI.enabled = true;

            tmpRect.x -= 45;
            if (zoom == MaxZoom)
                GUI.enabled = false;
            if (GUI.Button(tmpRect, expandIcon))
            {
                zoom++;
                zoom = Mathf.Clamp(zoom, 1, MaxZoom);
            }
            GUI.enabled = true;
            GUI.backgroundColor = backgroundColor;

            tmpRect = new Rect(this.position.width - 260, 10, 120, 20);

            EditorGUI.BeginChangeCheck();
            EditorGUI.LabelField(tmpRect, "Estimated Velocity");
            tmpRect.x += 110;
            tmpRect.width = 35;
            estimatedVelocity = EditorGUI.FloatField(tmpRect, estimatedVelocity);
            if (EditorGUI.EndChangeCheck())
            {
                estimatedVelocity = Mathf.Max(estimatedVelocity, 0.1f);
                EditorPrefs.SetFloat("estimatedVelocity", estimatedVelocity);
            }

            tmpRect.x = this.position.width - 260;
            tmpRect.y = 33;
            tmpRect.width = 130;
            EditorGUI.LabelField(tmpRect, "Show Estimation Lines");
            tmpRect.x += 131;
            EditorGUI.BeginChangeCheck();
            showEstimationLines = EditorGUI.Toggle(tmpRect, showEstimationLines);
            if (EditorGUI.EndChangeCheck())
                EditorPrefs.SetBool("showEstimationLines", showEstimationLines);

            if (showEstimationLines)
            {
                tmpRect = new Rect(this.position.width - 140, 55, 130, 20);
                EditorGUI.LabelField(tmpRect, "Planned Finish Date");
                tmpRect.y += 12;
                EditorGUI.LabelField(tmpRect, "Estimated Finish Date");

                tmpRect.x -= 10;
                tmpRect.y += 8;
                tmpRect.width = 10;
                tmpRect.height = 2;
                EditorGUI.DrawRect(tmpRect, Color.green);

                tmpRect.y -= 13;
                EditorGUI.DrawRect(tmpRect, Color.red);
            }

        }

        float GetHighestValue()
        {
            if (AgileBoardWindow.nestedList == null)
                return -1;

            float value = 0;
            for (int i = 0; i < AgileBoardWindow.nestedList.burndownData.dateValues.Count; i++)
            {
                if (AgileBoardWindow.nestedList.burndownData.dateValues[i] > value)
                    value = AgileBoardWindow.nestedList.burndownData.dateValues[i];
            }

            return value;
        }

        float GetEstimatedFinishDate(NestedList.BurndownData data, bool fromStart = true)
        {
            if (data.dateValues.Count == 0 || estimatedVelocity == 0)
                return 0;

            float totalDistance = fromStart ? data.dateValues[0] : data.dateValues[data.dateValues.Count - 1];
            float days = 0;

            while (totalDistance > 0)
            {
                days++;
                totalDistance -= estimatedVelocity;
            }

            if (!fromStart)
                days += data.dateValues.Count - 1;

            return days;
        }

        float GetRotationAngle(float y1, float x2)
        {
            var m = -y1 / x2;
            return -(Mathf.Atan(m) * Mathf.Rad2Deg);
        }

    }

}