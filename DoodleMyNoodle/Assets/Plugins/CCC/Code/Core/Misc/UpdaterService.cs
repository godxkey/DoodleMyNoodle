using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Scripting;
using UnityEngineX;

internal class UpdaterService : MonoCoreService<UpdaterService>
{
    public override void Initialize(Action<ICoreService> onComplete) => onComplete(this);

    private void Awake()
    {
        if (Updater.s_StaticMethodRegistered == false)
            Updater.RegisterStaticUpdateMethods();
    }

    void Update()
    {
        Updater.Internal_RaiseUpdate();
    }
    void FixedUpdate()
    {
        Updater.Internal_RaiseFixedUpdate();
    }
    void OnGUI()
    {
        Updater.Internal_RaiseGUI();
    }

    private void LateUpdate()
    {
        Updater.Internal_RaiseLateUpdate();
    }
}

public enum UpdateType
{
    GUI,
    Update,
    FixedUpdate,
    LateUpdate
}

public static class Updater
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class StaticUpdateMethodAttribute : PreserveAttribute
    {
        public UpdateType Update;

        public StaticUpdateMethodAttribute(UpdateType update)
        {
            this.Update = update;
        }
    }

    public static event Action GUI;
    public static event Action Update;
    public static event Action LateUpdate;
    public static event Action FixedUpdate;


    internal static bool s_StaticMethodRegistered = false;

    internal static void RegisterStaticUpdateMethods()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        const string ASSEMBLY_NAME = "CCC.Core";
        Assembly currentAssembly = assemblies.Find(x => x.GetName().Name == ASSEMBLY_NAME);

        IEnumerable<MethodInfo> staticMethods =

            // Join results
            Concat(assemblies

                // Assemblies that reference CCC.Core (or CCC.Core itself)
                .Where(assembly => assembly == currentAssembly || assembly.GetReferencedAssemblies().Contains(x => x.Name == ASSEMBLY_NAME))

                    // Get types in assembly
                    .Select(assembly => assembly.GetTypes()

                        // Get all static methods in types
                        .Select(type => type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)

                            // with the [StaticUpdateMethod] attribute
                            .Where(method => Attribute.IsDefined(method, typeof(StaticUpdateMethodAttribute))))));


        foreach (MethodInfo method in staticMethods)
        {
            StaticUpdateMethodAttribute attribute = method.GetCustomAttribute<StaticUpdateMethodAttribute>();
            Action action = (Action)Delegate.CreateDelegate(typeof(Action), method);

            switch (attribute.Update)
            {
                case UpdateType.GUI:
                    GUI += action;
                    break;

                case UpdateType.Update:
                    Update += action;
                    break;

                case UpdateType.FixedUpdate:
                    FixedUpdate += action;
                    break;

                case UpdateType.LateUpdate:
                    LateUpdate += action;
                    break;
            }
        }

        s_StaticMethodRegistered = true;
    }

    private static IEnumerable<T> Concat<T>(IEnumerable<IEnumerable<T>> sequences)
    {
        return sequences.SelectMany(x => x);
    }
    private static IEnumerable<T> Concat<T>(IEnumerable<IEnumerable<IEnumerable<T>>> sequences)
    {
        return sequences.SelectMany(x => Concat(x));
    }

    internal static void Internal_RaiseGUI()
    {
        GUI?.Invoke();
    }

    internal static void Internal_RaiseUpdate()
    {
        Update?.Invoke();
    }

    internal static void Internal_RaiseFixedUpdate()
    {
        FixedUpdate?.Invoke();
    }

    internal static void Internal_RaiseLateUpdate()
    {
        LateUpdate?.Invoke();
    }
}