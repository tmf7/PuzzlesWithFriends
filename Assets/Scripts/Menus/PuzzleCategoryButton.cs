using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace JPWF
{
    public class PuzzleCategoryButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _categoryName;
        [SerializeField] private RawImage _categoryImage;
        [SerializeField] private PuzzleCategoryData _puzzleCategory;

        private void Awake()
        {
            _categoryName.text = _puzzleCategory.CategoryName;
            _categoryImage.texture = _puzzleCategory.Puzzles[0].PuzzleTexture;

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TransitionManager.Transition(true, _categoryImage.mainTexture, complete: ShowAvailablePuzzles);
        }

        private void ShowAvailablePuzzles()
        {
            MainMenuManager.ShowPuzzleMenu(_puzzleCategory);
            TransitionManager.Transition(false);
        }
    }
}