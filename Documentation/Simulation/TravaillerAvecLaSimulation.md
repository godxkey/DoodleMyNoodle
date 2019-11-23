# Travailler avec la simulation

1. Pas de float
float = Fix64
Vector2 = FixVector2
etc.

Mathf.X = FixMath.X


2. Event Unity vs. Event Sim
Awake = OnSimAwake
Start = OnSimStart
OnDestroy OnSimDestroy
 = OnAddedToRuntime
 = OnRemovingFromRuntime
 
 
3. Saved data
 = SerializeToDataStack(SimComponentDataStack dataStack)
 = DeserializeFromDataStack(SimComponentDataStack dataStack)
 

4. Unity API vs Sim API
GameObject.Instantiate = Simulation.Instantiate
GameObject.Destroy = Simulation.Destroy
SceneManager.LoadScene = Simulation.LoadScene
Random.X = Simulation.Random.X