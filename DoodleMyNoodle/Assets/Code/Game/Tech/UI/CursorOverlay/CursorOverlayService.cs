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
        Move
    }

    [Serializable]
    public class CursorSetting
    {
        public Texture2D Icon;
        public Texture2D ClickedIcon;
        public Vector2 HotSpotRatio;
        public CursorType Type = CursorType.Default;
        public CursorMode Mode = CursorMode.Auto;
    }

    [SerializeField] private List<CursorSetting> CursorSettings = new List<CursorSetting>();

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

    private GameObject _currentTooltip;

    private Vector3 _previousMousePosition;

    private List<GameObject> _currentHoverGameObject = new List<GameObject>();

    private bool _isInClickMode = false;
    private bool _isColored = false;
    private Color _lastColor;

    public override void Initialize(Action<ICoreService> onComplete)
    {
        this.DelayedCall(_tooltipUpdateDelay, UpdateOverlay, true);

        _currentSetting = CursorSettings[0];

        onComplete(this);
    }

    // Input and fluid position update
    private void Update()
    {
        UpdateTooltipPosition();

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

    private void UpdateTooltipPosition()
    {
        if (_currentTooltip == null)
        {
            _currentTooltip = Instantiate(_tooltipPrefab);
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
    }

    public void ResetCursorToDefault()
    {
        SetCursorType(CursorType.Default);
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
            Cursor.SetCursor(setting.Icon, GetHotSpot(setting), setting.Mode);
        }
    }

    public void SetCursorColor(Color color)
    {
        // Copy texture
        Texture2D coloredTexture;
        if (_isInClickMode && _currentSetting.ClickedIcon != null)
        {
            coloredTexture = new Texture2D(_currentSetting.ClickedIcon.width, _currentSetting.ClickedIcon.height, TextureFormat.RGBA32, false);
            coloredTexture.SetPixels(_currentSetting.ClickedIcon.GetPixels());
        }
        else
        {
            coloredTexture = new Texture2D(_currentSetting.Icon.width, _currentSetting.Icon.height, TextureFormat.RGBA32, false);
            coloredTexture.SetPixels(_currentSetting.Icon.GetPixels());
        }

        // Color It
        for (int y = 0; y < coloredTexture.height; y++)
        {
            for (int x = 0; x < coloredTexture.width; x++)
            {
                Color currentColor = coloredTexture.GetPixel(x, y);
                if (currentColor == Color.white)
                {
                    coloredTexture.SetPixel(x, y, color);
                }
            }
        }

        //coloredTexture.alphaIsTransparency = true;

        // Apply changes
        coloredTexture.Apply();

        // Change Cursor
        _lastColor = color;
        _isColored = true;
        Cursor.SetCursor(coloredTexture, GetHotSpot(_currentSetting), _currentSetting.Mode);
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
                Cursor.SetCursor(_currentSetting.ClickedIcon, GetHotSpot(_currentSetting), _currentSetting.Mode);
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
                Cursor.SetCursor(_currentSetting.Icon, GetHotSpot(_currentSetting), _currentSetting.Mode);
            }
        }
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

    private Vector2 GetHotSpot(CursorSetting setting)
    {
        return new Vector2(setting.Icon.width / setting.HotSpotRatio.x, setting.Icon.height / setting.HotSpotRatio.y);
    }

    private void SetCurrentSetting(CursorSetting setting)
    {
        _previousSetting = _currentSetting;
        _currentSetting = setting;
    }
}