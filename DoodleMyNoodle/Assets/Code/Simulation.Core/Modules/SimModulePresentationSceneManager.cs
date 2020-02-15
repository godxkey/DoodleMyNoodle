using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

internal class SimModulePresentationSceneManager : SimModuleBase
{
    // fbessette TODO: This should be OUT of the simulation

    // Adding a presentation scene should not affect the simulation whatsoever
    internal void AddPresentationScene(string scene)
    {
        if (SimModules._World.PresentationScenes == null)
            SimModules._World.PresentationScenes = new List<string>();
        SimModules._World.PresentationScenes.Add(scene);
        SceneService.LoadAsync(scene, LoadSceneMode.Additive, LocalPhysicsMode.Physics2D);
    }
    internal void RemovePresentationScene(string scene)
    {
        if (SimModules._World.PresentationScenes != null)
            SimModules._World.PresentationScenes.Remove(scene);
        
        SceneService.UnloadAsync(scene);
    }

    internal void OnDeserializedWorld()
    {
        if (SimModules._World.PresentationScenes != null)
        {
            foreach (var scene in SimModules._World.PresentationScenes)
            {
                SceneService.LoadAsync(scene, LoadSceneMode.Additive, LocalPhysicsMode.Physics2D);
            }
        }
    }
}
