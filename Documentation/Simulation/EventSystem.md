
# Event System
Les events sont toujours lancé avec un structure de donnée qui représente l'``EventData``. Example:
```csharp
public struct SimPlayerCreatedEventData
{
    public ISimPlayerInfo PlayerInfo;
}
```
NB:
- Toujours avoir le suffix EventData
- Toujours utiliser des structs afin de ne pas créer de garbage

## Lancer les *events* 
Les events peuvent être lancé globalement ou localement.
### Global
```csharp
// setup event data
SimPlayerCreatedEventData eventData;
eventData.PlayerInfo = newPlayerInfo;

// raise event
SimGlobalEventEmitter.RaiseEvent(eventData);
```
### Local
Notre classe doit hériter de SimEventComponent
```csharp
public struct HealthModifiedEventData
{
    public int PreviousHealthValue;
    public int NewHealthValue;
}

public class HealthComponent : SimEventComponent
{
    // declare event field
    public SimEvent<HealthModifiedEventData> OnHealthModified;

    public override void OnSimAwake()
    {
        base.OnSimAwake();

        // create event
        OnHealthModified = CreateLocalEvent<HealthModifiedEventData>();
    }

    void SetHealth(int newValue)
    {
        //...

        // setup event data
        HealthModifiedEventData eventData;
        eventData.PreviousHealthValue = ...
        eventData.NewHealthValue = ...
        
        // raise event
        OnHealthModified.Raise(eventData);
    }
}
```

## Écouter les *events*
Nous pouvons écouter les events globaux et les events locaux. Dans les deux cas, nous devons posséder l'interface ``ISimEventListener<TheEvenData>`` et hériter de ``SimEventComponent``

### Global
```csharp
public class MyListenerClass: SimEventComponent, ISimEventListener<SimPlayerDestroyedEventData>
{
    public override void OnSimStart()
    {
        base.OnSimStart();
        
        // listen to event
        SimGlobalEventEmitter.RegisterListener<SimPlayerCreatedEventData>(this);
    }

    public void OnEventRaised(in SimPlayerCreatedEventData eventData)
    {
        // react to event

        // CAN BE OPTIONALLY CALLED WHEN NEEDED
        SimGlobalEventEmitter.UnregisterListener<SimPlayerCreatedEventData>(this);
    }
}
```
### Local
```csharp
public class MyListenerClass: SimEventComponent, ISimEventListener<SimPlayerDestroyedEventData>
{
    // reference to the owner of the local event
    public HealthComponent MyHealthComponent;
    
    public override void OnSimAwake()
    {
        base.OnSimAwake();
        
        // listen to event
        MyHealthComponent.OnHealthModified.RegisterListener(this);
    }

    public void OnEventRaised(in SimPlayerCreatedEventData eventData)
    {
        // react to event

        // CAN BE OPTIONALLY CALLED WHEN NEEDED
        MyHealthComponent.OnHealthModified.UnregisterListener(this);
    }
}
```
### Écouter à partir de la *Vue* (code hors de la simulation)
Utilisez ``RegisterListenerFromView`` et ``UnregisterListenerFromView`` au lieu de ``RegisterListener`` et ``UnregisterListener``. Vous ne serez sauvegardé dans la simulation.

## Utilisé les events hors de SimEventComponent (Unsafe)
### Lancé les events localement
1. Au lieux d'appeler``myEvent = CreateLocalEvent<EventData>()`` dans le ``OnSimAwake()``, tout simplement faire ``myEvent = new SimEvent<T>()``.
2. Toujours ``.Dispose()`` des events lorsque le propriétaire de l'event est détruit de la simulation (de manière permanente, ne pas confondre avec ``OnRemovingFromRuntime``).

### Écouter les events
1. Utiliser ``.RegisterEventListenerUnsafe(this)``
2. Toujours s'assurer de ``.UnregisterListenerUnsafe(this)`` lorsque le listener est détruit de la simulation
