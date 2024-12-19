using UnityEngine;
using UnityEngine.UI;

namespace Watermelon.SquadShooter
{
    public class CharacterTab : MonoBehaviour
    {
        [SerializeField] Image tabImage;
        [SerializeField] Color defaultColor;
        [SerializeField] Color notificationColor;
        [SerializeField] Color disabledColor;
        [SerializeField] GameObject notificationObject;

        private UICharactersPanel characterPanel;

        private TweenCase movementTweenCase;

        private Vector2 defaultAnchoredPosition;

        private RectTransform rectTransform;
        public RectTransform RectTransform => rectTransform;

        private Button button;
        public Button Button => button;

        private UIGamepadButton gamepadButton;
        public UIGamepadButton GamepadButton => gamepadButton;

        private CanvasGroup canvasGroup;

        private bool isActive;

        public void Initialise()
        {
            button = GetComponent<Button>();
            canvasGroup = GetComponent<CanvasGroup>();
            gamepadButton = GetComponent<UIGamepadButton>();

            rectTransform = (RectTransform)transform;

            characterPanel = UIController.GetPage<UICharactersPanel>();

            defaultAnchoredPosition = rectTransform.anchoredPosition;

            isActive = true;
        }

        public void OnWindowOpened()
        {
            if (!isActive)
                return;

            movementTweenCase.KillActive();

            rectTransform.anchoredPosition = defaultAnchoredPosition;
            tabImage.color = defaultColor;

            if (characterPanel.IsAnyActionAvailable())
            {
                notificationObject.SetActive(true);

                movementTweenCase = tabImage.DOColor(notificationColor, 0.3f, 0.3f).OnComplete(delegate
                {
                    movementTweenCase = new TabAnimation(rectTransform, new Vector2(defaultAnchoredPosition.x, defaultAnchoredPosition.y + 30)).SetDuration(1.2f).SetUnscaledMode(false).SetUpdateMethod(UpdateMethod.Update).SetEasing(Ease.Type.QuadOutIn).StartTween();
                });
            }
            else
            {
                notificationObject.SetActive(false);
            }
        }

        public void OnWindowClosed()
        {
            movementTweenCase.KillActive();

            rectTransform.anchoredPosition = defaultAnchoredPosition;
        }

        public void Disable()
        {
            isActive = false;

            tabImage.color = disabledColor;
            rectTransform.anchoredPosition = defaultAnchoredPosition;

            notificationObject.SetActive(false);

            canvasGroup.alpha = 0.5f;

            movementTweenCase.KillActive();
        }

        public void Activate()
        {
            isActive = true;

            canvasGroup.alpha = 1.0f;

            OnWindowOpened();
        }

        public void OnButtonClicked()
        {
            UIController.HidePage<UIMainMenu>(() =>
            {
                UIController.ShowPage<UICharactersPanel>();
            });

            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }
    }
}