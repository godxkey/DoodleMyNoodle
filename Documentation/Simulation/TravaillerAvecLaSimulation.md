# Travailler avec la simulation

## Pas de floats
|Hors Simulation|Simulation|
|-|-|
|float|Fix64|
|Vector2 |FixVector2|
|Vector3 |FixVector3|
|Vector4 |FixVector4|
|Matrix4x4 |FixMatrix|
|-|-|
|Mathf.X()|FixMath.X()|

## Unity Events vs Sim Event
|Hors Simulation|Simulation| Description
|-|-|-|
|Awake |OnSimAwake|Lorsque l'entité spawn. **NB: Si la game est sauvegardé, quitté and reloadé, ceci ne sera pas rappelé.**
|Start |OnSimStart|À la première frame après le spawn de l'entité. **NB: Si la game est sauvegardé, quitté and reloadé, ceci ne sera pas rappelé.**
|OnEnable|_pas implémenté_|-
|OnDisable|_pas implémenté_|-
|OnDestroy |OnSimDestroy|Lorsque l'entité est détruite **NB: Si la game est sauvegardé et quitté ceci ne sera pas appelé.**
|-|OnAddedToRuntime|Lorsque l'entité est ajouté au *sim runtime*. Appelé à chaque reload de partie. **NB: Ne pas utiliser pour de la logique de jeu.** |
|-|OnRemovingFromRuntime|Lorsque l'entité est retiré du *sim runtime*. Appelé à chaque unload de partie. **NB: Ne pas utiliser pour de la logique de jeu.** |
 

## Unity API vs Sim API
|Hors Simulation|Simulation|
|-|-|
|Object.Instantiate |Simulation.Instantiate|
|Object.Destroy|Simulation.Destroy|
|SceneManager.LoadScene|Simulation.LoadScene|
|Random.X()|Simulation.Random.X()|

## Saved Sim Data 
Pour que les données de jeu soit sauvegardé. Les SimComponents doivent:
```csharp
public class MySimComponent : SimComponent
{
	[System.Serializable]
	struct SerializedData
	{
		// all of my saved properties
	    public Fix64 Speed;
   	    public int Health;
   	    public SimEntity TargetedEnemy;
	}
	
	[UnityEngine.SerializeField]
	SerializedData _data = new SerializedData();
	 
	public override void SerializeToDataStack(SimComponentDataStack dataStack)
	{
	    base.SerializeToDataStack(dataStack);
	    dataStack.Add(_data);
	}
	 
	public override void DeserializeFromDataStack(SimComponentDataStack dataStack)
	{
	    _data = (SerializedData)dataStack.Get();
	    base.SerializeToDataStack(dataStack);
	}
}
``` 
Utilise le snippet ``scSimData`` dans Visual Studio pour écrire ce code plus rapidement!

Si des champs ne sont pas dans la ``struct SerializedData ``, leur modification in-game ne seront pas sauvegardé. Example
```csharp
public class MySimComponent : SimComponent
{
	public Fix64 Speed; // NE PEUT ÊTRE QU'UTILISÉ EN 'READ'
}
``` 


