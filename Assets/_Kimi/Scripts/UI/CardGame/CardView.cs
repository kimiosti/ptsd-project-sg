using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Kimi.Scripts.UI.CardGame
{
    [RequireComponent(typeof(CanvasGroup), typeof(Image), typeof(Button))]
    public class CardView: MonoBehaviour
    {
        [SerializeField] private CanvasGroup canGroup;
        [SerializeField] private Image image;
        [SerializeField] private Button button;
        public event Action<CardView> OnCardClicked;

        private void Awake()
        {
            canGroup = GetComponent<CanvasGroup>();
            image = GetComponent<Image>();
            button = GetComponent<Button>();
        }

        private void Start()
        {
            button.onClick.AddListener(() =>
            {
                OnCardClicked?.Invoke(this);
            });
        }

        public void SetVisible(bool visible)
        {
            canGroup.alpha = visible ? 1.0f : 0.0f;
        }

        public void SetClickable(bool clickable)
        {
            canGroup.blocksRaycasts = clickable;
        }

        public void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}