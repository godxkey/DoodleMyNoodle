using UnityEngine;
using UnityEngine.SceneManagement;
using CCC.InspectorDisplay;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
#endif

[CreateAssetMenu(fileName = "NewScene_SI", menuName = "CCC/Scenes/Scene Info")]
public class SceneInfo : ScriptableObject
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset scene;
    public SceneAsset Editor_GetScene()
    {
        return scene;
    }
    public void Editor_RefreshSceneName()
    {
        sceneName = scene.name;
    }
#endif

    [SerializeField, ReadOnlyAlways]
    private string sceneName;

    public string SceneName { get { return sceneName; } }

#if UNITY_EDITOR
    [OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        SceneInfo sceneInfo = EditorUtility.InstanceIDToObject(instanceID) as SceneInfo;
        if (sceneInfo != null)
        {
            if (sceneInfo.Editor_GetScene() != null)
            {
                Scene[] allScenes = new Scene[SceneManager.sceneCount];
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    allScenes[i] = SceneManager.GetSceneAt(i);
                }
                if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(allScenes))
                {
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(sceneInfo.Editor_GetScene()));
                }
                return true;
            }
        }

        return false; // we did not handle the open
    }
#endif
}
