using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngineX;

public class ItemContextMenuDisplaySystem : GamePresentationSystem<ItemContextMenuDisplaySystem>, IPointerEnterHandler , IPointerExitHandler
{
    public override bool SystemReady { get => true; }

    [SerializeField] private GameObject _contextMenuDisplay;

    [SerializeField] private Transform _contextMenuActionsContainer;
    [SerializeField] private GameObject _contextMenuActionPrefab;

    [SerializeField] private float _displacementX = 0;
    [SerializeField] private float _displacementY = 0;

    private bool _IsMouseOutsideContextMenu = false;

    protected override void Awake()
    {
        base.Awake();

        _contextMenuDisplay.SetActive(false);
    }

    protected override void OnGamePresentationUpdate() 
    {
        if (_IsMouseOutsideContextMenu && _contextMenuDisplay.activeSelf &&(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)))
        {
            DeactivateContextMenuDisplay();
        }
    }

    public void ActivateContextMenuDisplay(Action<int> actionSelected, params string[] actionsName)
    {
        Clear();

        _IsMouseOutsideContextMenu = false;

        transform.position = Input.mousePosition + new Vector3(_displacementX, _displacementY, 0);

        for (int i = 0; i < actionsName.Length; i++)
        {
            string name = actionsName[i];
            ContextMenuActionDisplay contextMenuActionDisplay = Instantiate(_contextMenuActionPrefab, _contextMenuActionsContainer).GetComponent<ContextMenuActionDisplay>();
            int choiceIndex = i;
            contextMenuActionDisplay.Init(name, () =>
            {
                actionSelected(choiceIndex);
                DeactivateContextMenuDisplay(true);
            });
        }

        _contextMenuDisplay.SetActive(true);
    }

    public void DeactivateContextMenuDisplay(bool force = false)
    {
        if (force || _IsMouseOutsideContextMenu)
        {
            _contextMenuDisplay.SetActive(false);
        }
    }

    private void Clear()
    {
        ContextMenuActionDisplay[] actions = _contextMenuActionsContainer.GetComponentsInChildren<ContextMenuActionDisplay>();
        for (int i = 0; i < actions.Length; i++)
        {
            Destroy(actions[i].gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _IsMouseOutsideContextMenu = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _IsMouseOutsideContextMenu = true;
    }
}
