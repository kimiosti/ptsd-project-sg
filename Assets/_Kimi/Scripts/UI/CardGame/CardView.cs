using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup), typeof(Image), typeof(Button))]
class CardView: MonoBehaviour
{
    [SerializeField] private CanvasGroup _canGroup;
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    public event Action<CardView> OnCardClicked;

    private void Awake()
    {
        _canGroup = GetComponent<CanvasGroup>();
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            OnCardClicked?.Invoke(this);
        });
    }

    public void SetVisible(bool visible)
    {
        _canGroup.alpha = visible ? 1.0f : 0.0f;
    }

    public void SetClickable(bool clickable)
    {
        _canGroup.blocksRaycasts = clickable;
    }

    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }
}