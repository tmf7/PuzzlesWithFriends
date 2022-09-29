using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JPWF
{
    [RequireComponent(typeof(Image))]
    public class BackgroundImageButton : MonoBehaviour, IPointerClickHandler
    {
        private BackgroundSelectMenu _backgroundSelectMenu;
        private Image _backgroundImage;
        private Color _primaryColor; // for support UI (eg: the puzzle piece organizer strip)

        public Sprite Sprite => _backgroundImage.sprite;
        public Color PrimaryColor => _primaryColor;

        private void Awake()
        {
            _backgroundSelectMenu = GetComponentInParent<BackgroundSelectMenu>();
            _backgroundImage = GetComponent<Image>();

            // account for sharply varied background colors
            _primaryColor = Color.Lerp(_backgroundImage.sprite.texture.GetPixelBilinear(0.5f, 0.5f), 
                                       _backgroundImage.sprite.texture.GetPixelBilinear(0.6f, 0.5f),
                                       0.5f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _backgroundSelectMenu.ChangeBackgroundImage(Sprite, PrimaryColor);
        }
    }
}