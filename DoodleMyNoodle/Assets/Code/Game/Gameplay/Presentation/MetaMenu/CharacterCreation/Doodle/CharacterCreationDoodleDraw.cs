using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class CharacterCreationDoodleDraw : GamePresentationSystem<CharacterCreationDoodleDraw>
{
    [SerializeField] private DoodleLibrary _library;
    [SerializeField] private UPaintGUI _uPaint;

    [Header("Draw Tool References")]
    [SerializeField] private GameObject _colorContainer;
    [SerializeField] private Button _colorContainerButton;
    [SerializeField] private Image _colorButtonImage;

    [SerializeField] private GameObject _brushContainer;
    [SerializeField] private Button _brushContainerButton;
    [SerializeField] private Image _brushButtonImage;

    [SerializeField] private Button _eraserBrush;
    [SerializeField] private Button _moveBrush;
    [SerializeField] private Button _fillBrush;

    [SerializeField] private Button _undoButton;
    [SerializeField] private Button _trashCanButton;

    [Header("Draw Settings")]
    [SerializeField] private int _size = 512;
    [SerializeField] private int _history = 10;
    [SerializeField] private int _eraserSize = 10;

    private bool _libraryLoaded = false;
    private Color _previousColor = Color.white;
    private bool _wasHoveringDoodle = false;
    private bool _isUsingBrush = false;
    private bool _isUsingFill = false;

    private bool _recentlyChangedTool = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnGameStart()
    {
        base.OnGameStart();

        _uPaint.Initialize(new Vector2Int(_size, _size), FilterMode.Point, _history);
        _uPaint.AddLayer();

        // CLICK EVENTS

        _colorContainerButton.onClick.AddListener(() => 
        {
            if (_brushContainer.activeSelf)
            {
                _brushContainer.ToggleActiveState();
            }

            if (_isUsingBrush || _isUsingFill)
            {
                _colorContainer.ToggleActiveState();
            }
        });

        _brushContainerButton.onClick.AddListener(() => 
        {
            if (_colorContainer.activeSelf)
            {
                _colorContainer.ToggleActiveState();
            }

            _brushContainer.ToggleActiveState(); 
        });

        _eraserBrush.onClick.AddListener(SetEraser);
        _moveBrush.onClick.AddListener(SetMove);
        _fillBrush.onClick.AddListener(SetFill);
        _undoButton.onClick.AddListener(_uPaint.Undo);
        _trashCanButton.onClick.AddListener(EraseCurrentDoodle);

        // Default Brush
        _uPaint.SetDefaultBrush(0, 0);
        _uPaint.PaintColor = _previousColor;

        // Library -> To load previous doodle
        _libraryLoaded = false;
        _library.LoadAsync(() =>
        {
            Import();
            _libraryLoaded = true;
        });
    }

    private void Update()
    {
        if (CursorOverlayService.Instance.IsHoveringGroup(_uPaint.gameObject.GetAllChilds().ToArray()))
        {
            if (!_wasHoveringDoodle)
            {
                _wasHoveringDoodle = true;

                if (_recentlyChangedTool)
                {
                    _recentlyChangedTool = false;
                }
                else
                {
                    CursorOverlayService.Instance.RevertToPreviousSetting();
                    if (_isUsingBrush || _isUsingFill)
                    {
                        CursorOverlayService.Instance.SetCursorColor(_previousColor);
                    }
                }
            }
        }
        else 
        {
            if (_wasHoveringDoodle)
            {
                _wasHoveringDoodle = false;
                CursorOverlayService.Instance.ResetCursorToDefault();
            }  
        }
    }

    public void SetCurrentDoodle(Texture2D newTexture)
    {
        for (int i = 0; i < _uPaint.LayerCount; i++)
        {
            _uPaint.RemoveLayer(i);
        }

        _uPaint.ImportImage(newTexture);
    }

    public Texture2D ExportCurrentDoodleTexture()
    {
        Export();
        return _uPaint.GetLayerTexture(0);
    }

    public bool IsLastDoodleLoaded => _libraryLoaded;

    private void Import()
    {
        Texture2D lastDoodle = _library.GetLastDoodle();

        if (lastDoodle != null)
        {
            SetCurrentDoodle(lastDoodle);
        }
    }

    private void Export()
    {
        _library.AddDoodle(_uPaint.GetLayerTexture(0), true);
    }

    public void SetBrushColor(Color newColor)
    {
        // if you're coming from the doodle directly, make sure we don't color the default cursor
        if (!_recentlyChangedTool)
        {
            _recentlyChangedTool = true;
            CursorOverlayService.Instance.RevertToPreviousSetting();
        }

        CursorOverlayService.Instance.SetCursorColor(newColor);
        _uPaint.PaintColor = newColor;
        _previousColor = newColor;
        _colorButtonImage.color = newColor;
        _colorContainer.ToggleActiveState();
    }

    public void SetBrush(float brushSize, float brushGradient, Sprite icon, bool isSmall)
    {
        _recentlyChangedTool = true;
        _isUsingBrush = true;
        _isUsingFill = false;
        CursorOverlayService.Instance.SetCursorType(isSmall ? CursorOverlayService.CursorType.SmallBrush : CursorOverlayService.CursorType.BigBrush);
        CursorOverlayService.Instance.SetCursorColor(_previousColor);
        _uPaint.SetDefaultBrush(Mathf.RoundToInt(brushSize), brushGradient);
        _uPaint.PaintColor = _previousColor;
        _brushButtonImage.sprite = icon;
        _brushContainer.ToggleActiveState();
    }

    private void SetEraser()
    {
        _recentlyChangedTool = true;
        _isUsingFill = false;
        _isUsingBrush = false;
        CursorOverlayService.Instance.SetCursorType(CursorOverlayService.CursorType.Erase);
        _uPaint.SetEraserBrush(_eraserSize);
        _brushButtonImage.sprite = _eraserBrush.GetComponentOnlyInChildren<Image>().sprite;
        _brushContainer.ToggleActiveState();
    }

    private void SetMove()
    {
        _recentlyChangedTool = true;
        _isUsingFill = false;
        _isUsingBrush = false;
        CursorOverlayService.Instance.SetCursorType(CursorOverlayService.CursorType.Move);
        _uPaint.SetMoveBrush();
        _brushButtonImage.sprite = _moveBrush.GetComponentOnlyInChildren<Image>().sprite;
        _brushContainer.ToggleActiveState();
    }

    private void SetFill()
    {
        // if you're coming from the doodle directly, make sure we don't color the default cursor
        if (!_recentlyChangedTool)
        {
            _recentlyChangedTool = true;
            _isUsingFill = true;
            CursorOverlayService.Instance.RevertToPreviousSetting();
        }

        _isUsingBrush = false;
        CursorOverlayService.Instance.SetCursorType(CursorOverlayService.CursorType.Fill);
        CursorOverlayService.Instance.SetCursorColor(_previousColor);
        _uPaint.SetFillBrush();
        _uPaint.PaintColor = _previousColor;
        _brushButtonImage.sprite = _fillBrush.GetComponentOnlyInChildren<Image>().sprite;
        _brushContainer.ToggleActiveState();
    }

    public void EraseCurrentDoodle()
    {
        _uPaint.Initialize(new Vector2Int(_size, _size), FilterMode.Point, _history);
    }
}