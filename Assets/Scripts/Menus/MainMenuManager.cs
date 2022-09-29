using UnityEngine;

namespace JPWF
{
    public class MainMenuManager : MonoBehaviour
    {
        private static MainMenuManager _instance;

        [SerializeField] private Canvas _categoryMenuCanvas;
        [SerializeField] private Canvas _puzzleMenuCanvas;
        [SerializeField] private RectTransform _puzzleMenuContent;
        [SerializeField] private GameObject _backButton;
        [SerializeField] private PuzzleOptionButton _puzzleOptionButtonPrefab;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                ShowCategoryMenu();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void HideAll()
        {
            _categoryMenuCanvas.enabled = false;
            _puzzleMenuCanvas.enabled = false;
            _backButton.SetActive(false);
        }


        public static void ShowCategoryMenu()
        {
            _instance.HideAll();
            _instance._categoryMenuCanvas.enabled = true;
        }

        public static void ShowPuzzleMenu(PuzzleCategoryData puzzleCategory)
        {
            _instance.HideAll();
            _instance.ClearPuzzleMenuContent();
            _instance.SetPuzzleMenuContent(puzzleCategory);
            _instance._puzzleMenuCanvas.enabled = true;
            _instance._backButton.SetActive(true);
        }

        private void ClearPuzzleMenuContent()
        {
            for (int i = _puzzleMenuContent.childCount - 1; i >= 0; --i)
            {
                Destroy(_puzzleMenuContent.GetChild(i).gameObject);
            }
        }

        private void SetPuzzleMenuContent(PuzzleCategoryData puzzleCategory)
        {
            for (int i = 0; i < puzzleCategory.Puzzles.Length; ++i)
            {
                PuzzleOptionButton puzzleOption = Instantiate(_puzzleOptionButtonPrefab, _puzzleMenuContent);
                puzzleOption.Configure(puzzleCategory.Puzzles[i]);
            }
        }
    }
}