using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JPWF
{
    [RequireComponent(typeof(Image))]
    public class BackgroundImageButton : MonoBehaviour, IPointerClickHandler
    {
        private BackgroundSelectMenu m_backgroundSelectMenu;
        private Image m_backgroundImage;
        private BackgroundImageOptionData m_backgroundImageOptionData;

        public void Init(BackgroundSelectMenu owner, BackgroundImageOptionData data)
        {
            m_backgroundSelectMenu = owner;
            m_backgroundImage = GetComponent<Image>();

            m_backgroundImage.sprite = data.Background;
            m_backgroundImageOptionData = data;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            m_backgroundSelectMenu.ChangeBackgroundImage(m_backgroundImageOptionData);
        }
    }
}