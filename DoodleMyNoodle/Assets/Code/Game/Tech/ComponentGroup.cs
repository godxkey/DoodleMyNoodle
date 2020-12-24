using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

// CODED ADDED FROM : https://forum.unity.com/threads/group-components-in-inspector.513931/
// USED TO BETTER ORGANIZE HUGE LIST OF COMPONENTS IN INSPECTOR

// TODO : Change components order so that when you enable a group, it displays them below the current component from which you toggled them on.
// Use UnityEditorInternal.ComponentUtility.MoveComponentUp(target_component);

public class ComponentGroup : MonoBehaviour
{
    public string GroupName;

    public bool Visibility;
    public bool IsEditing;

    public ComponentGroup Parent;
    public List<Component> Comps = new List<Component>();
}

#if UNITY_EDITOR
[CustomEditor(typeof(ComponentGroup))]
public class ComponentGroupEditor : Editor
{
    private ComponentGroup _componentGroup;

    private List<Component> _allComponents;
    private List<ComponentGroup> _allComponentGroups;

    private Color _standardElementColor;

    private static int s_numOfGroupsToCollapseAtFirstLaunch = 0;

    private void Awake()
    {
        _standardElementColor = GUI.color;

        _componentGroup = (ComponentGroup)target;

        UpdateGroupData();

        CollapseGroupAtFirstLaunch();
    }

    /// <summary>
    /// Collapse group only on FIRST launch (for example, after launch Unity engine).
    /// Since there is no method that tracks only the FIRST run,
    /// we use a static variable that counts the number of all Awake() methods run
    /// </summary>
    private void CollapseGroupAtFirstLaunch()
    {
        if (s_numOfGroupsToCollapseAtFirstLaunch >= _allComponentGroups.Count)
            return;

        SetGroupVisibility(_componentGroup, false);
        _componentGroup.IsEditing = false;

        s_numOfGroupsToCollapseAtFirstLaunch++;
    }

    private void OnEnable()
    {
        for (int i = _componentGroup.Comps.Count - 1; i >= 0; i--)
        {
            TryRemoveNullComponents(_componentGroup, _componentGroup.Comps[i]);
        }
    }

    private static bool TryRemoveNullComponents(ComponentGroup group, Component component)
    {
        if (component != null)
            return false;

        group.Comps.Remove(component);

        return true;
    }

    private static void SetGroupVisibility(ComponentGroup group, bool visibility)
    {
        for (int i = group.Comps.Count - 1; i >= 0; i--)
        {
            Component component = group.Comps[i];

            if (TryRemoveNullComponents(group, component))
                continue;

            // Change visibility of child groups if they exist
            if (component is ComponentGroup childGroup)
            {
                // If you collapse the parent group, then the next time you expand the parent group,
                // the child groups will be collapsed
                SetGroupVisibility(childGroup, false);
            }

            SetComponentVisibility(component, visibility);
        }

        group.Visibility = visibility;
    }

    private static void SetComponentVisibility(Component component, bool visibility)
    {
        if (visibility)
        {
            component.hideFlags &= ~HideFlags.HideInInspector;
            // required if the object was deselected in between
            Editor.CreateEditor(component);
        }
        else
        {
            component.hideFlags |= HideFlags.HideInInspector;
        }
    }

    public override void OnInspectorGUI()
    {
        if (_componentGroup.IsEditing)
        {
            // TODO: heavy operation. Better update data only when they could change
            UpdateGroupData();

            Draw_EditMode();
        }
        else
        {
            Draw_StandardMode();
        }
    }

    private void UpdateGroupData()
    {
        _allComponents = _componentGroup.gameObject.GetComponents<Component>().ToList();
        _allComponentGroups = _componentGroup.gameObject.GetComponents<ComponentGroup>().ToList();

        SetParentForChildGroups();
    }

    private void SetParentForChildGroups()
    {
        foreach (ComponentGroup componentGroup in _allComponentGroups)
        {
            foreach (Component component in componentGroup.Comps)
            {
                if (component is ComponentGroup group)
                {
                    group.Parent = componentGroup;
                }
            }
        }
    }

    private void Draw_EditMode()
    {
        List<Component> availableComponents = GetAvailableComponentsIn(_allComponents);

        GUILayout.BeginHorizontal();

        Draw_GroupNameInputBox();
        Draw_DoneButton();

        GUILayout.EndHorizontal();

        Draw_AvailableComponents(availableComponents);
    }

    private List<Component> GetAvailableComponentsIn(List<Component> components)
    {
        components.Remove(_componentGroup); // Remove from list current group (which we are editing now)
        RemoveParentsFromAvailableComponents(ref components);
        RemoveOtherChildrenFromAvailableComponents(ref components);

        return components;
    }

    private void RemoveParentsFromAvailableComponents(ref List<Component> components)
    {
        ComponentGroup tempParent = _componentGroup.Parent;

        while (tempParent != null)
        {
            components.Remove(tempParent);
            tempParent = tempParent.Parent;
        }
    }

    /// <summary>
    /// Remove components that belong to other component groups
    /// </summary>
    private void RemoveOtherChildrenFromAvailableComponents(ref List<Component> components)
    {
        foreach (ComponentGroup group in _allComponentGroups)
        {
            if (group != _componentGroup)
                components = components.Except(group.Comps).ToList();
        }
    }

    private void Draw_GroupNameInputBox()
    {
        _componentGroup.GroupName = GUILayout.TextField(_componentGroup.GroupName);
    }

    private void Draw_DoneButton()
    {
        if (GUILayout.Button("Done", GUILayout.Width(40)))
        {
            _componentGroup.IsEditing = false;
        }
    }

    private void Draw_AvailableComponents(List<Component> components)
    {
        foreach (Component component in components)
        {
            string componentName = GetComponentName(component);

            Draw_ComponentToggle(component, componentName);
        }
    }

    private static string GetComponentName(Component component)
    {
        string componentName = component.GetType().Name;

        if (component is ComponentGroup group)
            componentName = "COMPONENT GROUP(" + group.GroupName + ")";

        return componentName;
    }

    private void Draw_ComponentToggle(Component component, string componentName)
    {
        bool isInList = _componentGroup.Comps.Contains(component);
        GUI.color = isInList ? Color.green : _standardElementColor;

        bool componentToggle = GUILayout.Toggle(isInList, componentName);

        if (TryChangeToggleValue(componentToggle, isInList))
        {
            if (isInList)
                RemoveComponentFromGroup(component);

            else
                AddComponentToGroup(component);
        }
    }

    private static bool TryChangeToggleValue(bool toggle, bool value) => toggle != value;

    private void RemoveComponentFromGroup(Component component)
    {
        _componentGroup.Comps.Remove(component);

        if (component is ComponentGroup group)
        {
            group.Parent = null;
        }
    }

    private void AddComponentToGroup(Component component)
    {
        _componentGroup.Comps.Add(component);
    }

    private void Draw_StandardMode()
    {
        GUILayout.BeginHorizontal();

        Draw_CollapseOrExpandButton();
        Draw_EditButton();

        GUILayout.EndHorizontal();
    }

    private void Draw_CollapseOrExpandButton()
    {
        GUI.color = _componentGroup.Visibility ? Color.green : _standardElementColor;

        if (GUILayout.Button(_componentGroup.GroupName))
            SetGroupVisibility(_componentGroup, !_componentGroup.Visibility);
    }

    private void Draw_EditButton()
    {
        GUI.color = _standardElementColor;

        if (GUILayout.Button("Edit", GUILayout.Width(40)))
        {
            SetGroupVisibility(_componentGroup, true);
            _componentGroup.IsEditing = true;
        }
    }
}
#endif