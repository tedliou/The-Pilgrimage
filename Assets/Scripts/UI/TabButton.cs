using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IGameButton
{
    #region Inspector
    [Header("State")] [SerializeField] private bool isHover = false;
    [SerializeField] private bool isSelected = false;

    [Header("Group")] [SerializeField] private string group;
    
    [Header("Background: Hover")] [SerializeField]
    private bool autoFitHoverBackgroundSize = true;
    [SerializeField] private bool maximumHoverBackgroundSize = false;
    [SerializeField] private RectTransform hoverBackground;
    [SerializeField] private Padding hoverBackgroundPadding;
    [Header("Text")] [SerializeField] private RectTransform text;

    [Header("Highlight: Selected")] [SerializeField]
    private bool autoFitSelectedSize = true;
    [SerializeField] private RectTransform selected;
    [SerializeField][Range(0, 1f)] private float selectedWidth;
    [SerializeField] private float selectedMarginTop;

    [Header("Animation")] [SerializeField] private float fadeInDuration = .2f;
    [SerializeField] private float fadeOutDuration = .2f;

    [Header("Input")] [SerializeField] private InputActionAsset inputActionAsset;
    #endregion

    #region Private
    private Image _hoverBackgroundImage;
    private InputActionAsset _inputActionAsset;
    private InputAction _inputAction;
    private RectTransform _rectTransform;
    #endregion

    #region Static
    // <GroupID, TabButton>
    public static readonly UnityEvent<string, TabButton> onClick = new();
    public UnityEvent onEntryClick = new();
    #endregion

    #region Unity Messages
#if UNITY_EDITOR
    private void OnValidate()
    {
        SetHoverBackgroundSize();
        SetHighlightSelectedSize();
    }
#endif

    private void Awake()
    {
        _hoverBackgroundImage = hoverBackground.GetComponent<Image>();
        _hoverBackgroundImage.DOFade(0, 0);
        selected.DOScaleX(0, 0);
        _inputActionAsset = FindObjectOfType<InputSystemUIInputModule>().actionsAsset;
        _inputAction = _inputActionAsset.FindActionMap("UI").FindAction("Submit");
    }

    private void OnEnable()
    {
        onClick.AddListener(OnClick);
        _inputAction.performed += OnClickFromInputAction;
    }

    private void OnDisable()
    {
        onClick.RemoveListener(OnClick);
        _inputAction.performed -= OnClickFromInputAction;
        if (isSelected)
            Deselect();
    }
    #endregion

    #region Events
    public void OnSelect(BaseEventData eventData)
    {
        MouseEnter();
        Debug.Log($"{name} Mouse Enter (Select)");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        MouseExit();
        Debug.Log($"{name} Mouse Exit (Deselect)");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseEnter();
        Debug.Log($"{name} Mouse Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseExit();
        Debug.Log($"{name} Mouse Exit");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke(group, this);
        Debug.Log($"{name} Mouse Click");
    }

    public void Click()
    {
        onClick.Invoke(group, this);
    }
    #endregion
    
    private void SetHoverBackgroundSize()
    {
        if (maximumHoverBackgroundSize)
        {
            _rectTransform ??= GetComponent<RectTransform>();
            hoverBackground.sizeDelta = _rectTransform.sizeDelta;
            return;
        }
        
        if (!autoFitHoverBackgroundSize)
            return;
        
        // Top, Right
        var sizeDelta = text.sizeDelta + new Vector2(hoverBackgroundPadding.right, hoverBackgroundPadding.top);
        var anchoredPosition = new Vector2(hoverBackgroundPadding.right, hoverBackgroundPadding.top) * .5f;

        // Bottom, Left
        sizeDelta += new Vector2(hoverBackgroundPadding.left, hoverBackgroundPadding.right);
        anchoredPosition -= new Vector2(hoverBackgroundPadding.left, hoverBackgroundPadding.bottom) * .5f;
        
        hoverBackground.anchoredPosition = anchoredPosition;
        hoverBackground.sizeDelta = sizeDelta;
    }

    private void SetHighlightSelectedSize()
    {
        if (!autoFitSelectedSize)
            return;
        
        // Size
        var sizeDelta = selected.sizeDelta;
        sizeDelta.x = text.sizeDelta.x * selectedWidth;
        selected.sizeDelta = sizeDelta;
        
        // Position
        var pos = text.anchoredPosition - new Vector2(0, text.sizeDelta.y * .5f + selectedMarginTop);
        selected.anchoredPosition = pos;
    }
    
    private void OnClick(string groupID, TabButton entry)
    {
        if (groupID == group && entry == this)
        {
            isSelected = true;
            onEntryClick.Invoke();
            Select();
            Debug.Log($"{name} Selected");
        }
        else
        {
            if (groupID != group)
                return;
            
            if (!isSelected)
                return;

            isSelected = false;
            Deselect();
            Debug.Log($"{name} Deselected");
        }
    }

    private void OnClickFromInputAction(InputAction.CallbackContext ctx)
    {
        if (isHover)
            onClick.Invoke(group, this);
    }
    
    private void Select()
    {
        selected.DOScaleX(1, fadeInDuration);
    }

    private void Deselect()
    {
        selected.DOScaleX(0, fadeInDuration);
    }

    private void MouseEnter()
    {
        isHover = true;
        _hoverBackgroundImage.DOFade(1, fadeInDuration);
    }

    private void MouseExit()
    {
        isHover = false;
        _hoverBackgroundImage.DOFade(0, fadeOutDuration);
    }
}