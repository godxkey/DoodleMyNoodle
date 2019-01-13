# Services

Service serves the same purpose as a Singleton. It's name differently in order to represent better what it should actually be used for.
A service is not a manager, it's an independant system that deals with user request. Yes it can be monitring parts of your project, but
the whole thing should be separated and accessible only from the service. External code can do any type of request to the service that
should always respond since its a singleton.

For every service and to make the creation of them really easy you'll need to setup a few things. 

## Setup

1. Setup the Bank

In order to initialize every service available, we need to have a bank containing them. A scriptable object named "CoreServiceBank" is used for this. Create it within the Asset Menu and
make sure to place it in the Resources directory. Unity doesn't really support the resources directory and ask developer to not use it, but it's 
a powerful tool and a workaround reflection. In this case, it gives us the power to return the bank dynamicly in a scriptable object static function. Don't forget to add the prefab of your
service inside the bank list !

![](https://github.com/msfredb7/TimeDrifterLab/blob/master/TimeDrifterLab/Assets/Documentation/Images/servicetuto3.PNG)

![](https://github.com/msfredb7/TimeDrifterLab/blob/master/TimeDrifterLab/Assets/Documentation/Images/servicetuto4.PNG)

2. Add the CoreServiceManagerBootstrapper

CoreServiceManagerBootstrap is a MonoBehaviour that will create a CoreServiceManager on Awake. As the constructor execute itself, it'll get the list of service prefabs, instantiate them as
don't destroy on load and initialize them. When everything is completed, we trigger the event system related to the completion of the whole process.

![](https://github.com/msfredb7/TimeDrifterLab/blob/master/TimeDrifterLab/Assets/Documentation/Images/servicetuto1.PNG)

![](https://github.com/msfredb7/TimeDrifterLab/blob/master/TimeDrifterLab/Assets/Documentation/Images/servicetuto5.PNG)

3. Getting a Service

Once everything has been done, the accessing works flawlessly. On accessing the instance with the type, if it doesn't exist, it'll update itself and give you the right reference. You can always
access a service within the CoreServiceManager that is itself a singleton on his own, but it's just far more easy to directly go with the service type we're aiming for.

```c#
YourService.Instance.YourVariable;
YourService.Instance.YourFunction();
CoreServiceManager.Instance.GetCoreService<YourService>().YourFunction();

```

## Using Scriptable Objects as Services

In this system, you can also make Scriptable Object Service. Since the bank takes objects and we're always working with an Interface (ICoreService) to call any needed function for the initialization process,
you can also add a Scriptable object in the bank. It needs to derived from the ScriptableCoreService class. For the rest, it's the same. Note that a scriptable objects service in this case is not an instance, 
but pretty much just static code that manage itself on its own.
