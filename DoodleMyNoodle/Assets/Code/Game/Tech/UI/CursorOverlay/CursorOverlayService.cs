using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineX;
using System.Linq;
using System.Runtime.Serialization.Formatters;

// Can add other global mouse stuff here -> vfx following mouse
public class CursorOverlayService : MonoCoreService<CursorOverlayService>
{
    public enum CursorType
    {
        Default,
        Loading,
        BigBrush,
        SmallBrush,
        Fill,
        Erase,
        Move,
        Grab,
        Target
    }

    [Serializable]
    public class CursorSetting
    {
        public Sprite Icon;
        public Sprite ClickedIcon;
        public CursorType Type = CursorType.Default;
        public Vector2 Displacement = new Vector2(0, 0);
        public float Scale = 1;
        public bool DisplaySecondaryIcon = false;
    }

    [SerializeField] private List<CursorSetting> CursorSettings = new List<CursorSetting>();

    [SerializeField] private GameObject _cursorPrefab;
    [SerializeField] private GameObject _tooltipPrefab;
    [SerializeField] private float _tooltipUpdateDelay;
    [SerializeField] private float _tooltipScreenEdgeToolTipLimit = 200.0f;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _tooltipDisplacementRatioXRight = 0;
    [Range(-1.0f, 0.0f)]
    [SerializeField] private float _tooltipDisplacementRatioXLeft = 0;
    [Range(-1.0f, 1.0f)]
    [SerializeField] private float _tooltipDisplacementRatioY = 0;

    private CursorSetting _previousSetting;
    private CursorSetting _currentSetting;

    private CursorDisplay _currentCursor;
    private GameObject _currentTooltip;

    private Vector3 _previousMousePosition;

    private List<GameObject> _currentHoverGameObject = new List<GameObject>();

    private bool _isInClickMode = false;
    private bool _isColored = false;
    private Color _lastColor;

    private bool _isLocked = false;
    private bool _isLockedInWorldPos = false;
    private Vector2 _lockedPosition;

    public override void Initialize(Action<ICoreService> onComplete)
    {
        this.DelayedCall(_tooltipUpdateDelay, UpdateOverlay, true);

        _currentSetting = CursorSettings[0];

        onComplete(this);
    }

    // Input and fluid position update
    private void Update()
    {
        Cursor.visible = false;

        if (_isLocked)
        {
            if (_currentCursor != null)
            {
                if (_isLockedInWorldPos)
                {
                    Vector3 screemPos = CameraService.Instance.ActiveCamera.WorldToScreenPoint(_lockedPosition);
                    _currentCursor.SetCursorsPosition(new Vector2(screemPos.x, screemPos.y));
                }
                else
                {
                    _currentCursor.SetCursorsPosition(_lockedPosition);
                }
            }
        }
        else
        {
            UpdateAllPosition();
        }
        

        if (_currentSetting != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (!_isInClickMode)
                {
                    _isInClickMode = true;
                    SetCursorClickMode();
                }
            }
            else
            {
                if (_isInClickMode)
                {
                    _isInClickMode = false;
                    SetCursorNormalMode();
                }
            }
        }
    }

    // all the others updates
    public void UpdateOverlay()
    {
        if (_currentTooltip == null)
        {
            _currentTooltip = Instantiate(_tooltipPrefab);
        }

        if (Input.mousePosition != _previousMousePosition)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            bool detectedTooltip = false;
            _currentHoverGameObject.Clear();
            foreach (RaycastResult result in results)
            {
                _currentHoverGameObject.Add(result.gameObject);

                // Verify if we need to display a tooltip
                if (!detectedTooltip)
                {
                    TooltipDescription tooltipDescription = result.gameObject.GetComponent<TooltipDescription>();
                    if (tooltipDescription != null)
                    {
                        detectedTooltip = true;
                        _currentTooltip.GetComponentInChildren<TextMeshProUGUI>().text = tooltipDescription.Description;
                        _currentTooltip.SetActive(true);
                    }
                }
            }

            if (!detectedTooltip)
            {
                _currentTooltip.SetActive(false);
            }
        }

        _previousMousePosition = Input.mousePosition;

        this.DelayedCall(_tooltipUpdateDelay, UpdateOverlay, true);
    }

    public bool IsHovering(GameObject target)
    {
        foreach (var gameObject in _currentHoverGameObject)
        {
            if (target == gameObject)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsHovering(Transform target) => IsHovering(target.gameObject);

    public bool IsHoveringAny(List<GameObject> targets)
    {
        foreach (var gameObject in targets)
        {
            if (IsHovering(gameObject))
                return true;
        }

        return false;
    }

    public bool IsHoveringAny(List<Transform> targets)
    {
        foreach (var transform in targets)
        {
            if (IsHovering(transform))
                return true;
        }

        return false;
    }

    private void UpdateAllPosition()
    {
        if (_currentTooltip == null)
        {
            _currentTooltip = Instantiate(_tooltipPrefab);
        }

        if (_currentCursor == null)
        {
            _currentCursor = Instantiate(_cursorPrefab).GetComponent<CursorDisplay>();
        }

        bool exitRight = Input.mousePosition.x >= (Screen.width - _tooltipScreenEdgeToolTipLimit);

        float displacementRatioX = exitRight ? _tooltipDisplacementRatioXLeft : _tooltipDisplacementRatioXRight;
        float displacementRatioY = _tooltipDisplacementRatioY;

        displacementRatioX *= Screen.width;
        displacementRatioY *= Screen.height;

        Transform tooltipTransform = _currentTooltip.transform.GetComponentInChildren<Image>().transform;
        if (tooltipTransform != null)
        {
            tooltipTransform.position = Input.mousePosition + new Vector3(displacementRatioX, displacementRatioY, 0);
        }

        if (_currentCursor != null)
        {
            _currentCursor.SetCursorsPosition(Input.mousePosition + new Vector3(_currentSetting.Displacement.x, _currentSetting.Displacement.y, 0));
        }
    }

    public void ResetCursorToDefault()
    {
        SetCursorType(CursorType.Default);
        SetCursorColor(Color.white);
    }

    public void RevertToPreviousSetting()
    {
        SetCursor(_previousSetting);
    }

    public void SetCursorType(CursorType cursorType, bool updateCurrentSetting = true)
    {
        CursorSetting setting = FindSettingByType(cursorType);
        SetCursor(setting, updateCurrentSetting);
    }

    public void SetCursor(CursorSetting setting, bool updateCurrentSetting = true)
    {
        if (setting != null)
        {
            if (updateCurrentSetting)
            {
                SetCurrentSetting(setting);
            }

            _isColored = false;
            SetCursorIcon(setting.Icon, _currentSetting.Scale);

            _currentCursor.ChangeDisplaySecondaryCursor(setting.DisplaySecondaryIcon);
        }
    }

    public void SetCursorColor(Color color)
    {
        _currentCursor.SetPrimaryCursorColor(color);
        _lastColor = color;
        _isColored = true;
    }

    public void SetCursorClickMode()
    {
        if (_currentSetting.ClickedIcon != null)
        {
            if (_isColored)
            {
                SetCursorColor(_lastColor);
            }
            else
            {
                SetCursorIcon(_currentSetting.ClickedIcon, _currentSetting.Scale);
            }
        }
    }

    public void SetCursorNormalMode()
    {
        if (_currentSetting != null)
        {
            if (_isColored)
            {
                SetCursorColor(_lastColor);
            }
            else
            {
                SetCursorIcon(_currentSetting.Icon, _currentSetting.Scale);
            }
        }
    }

    public void SetCursorLockAtPosition(Vector2 position, bool lockOnWorldPos = false) 
    {
        _isLocked = true;
        _isLockedInWorldPos = lockOnWorldPos;
        if (lockOnWorldPos)
        {
            Vector3 worldPos = CameraService.Instance.ActiveCamera.ScreenToWorldPoint(position);
            _lockedPosition = new Vector2(worldPos.x, worldPos.y);
        }
        else
        {
            _lockedPosition = position;
        }
    }

    public void UnlockCursorPosition()
    {
        _isLocked = false;
    }

    private CursorSetting FindSettingByType(CursorType type)
    {
        foreach (CursorSetting setting in CursorSettings)
        {
            if (setting.Type == type)
            {
                return setting;
            }
        }

        return null;
    }

    private void SetCurrentSetting(CursorSetting setting)
    {
        _previousSetting = _currentSetting;
        _currentSetting = setting;
    }

    public CursorSetting GetCurrentSettings()
    {
        return _currentSetting;
    }

    private void SetCursorIcon(Sprite sprite, float scale)
    {
        _currentCursor.SetPrimaryCursorSize(new Vector3(scale, scale, scale));
        _currentCursor.SetPrimaryCursorIcon(sprite);
    }
}