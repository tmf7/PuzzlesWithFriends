using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

namespace JPWF
{
    public class PuzzleOptionButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RawImage _puzzleImage;
        [SerializeField] private TMP_Text _puzzleName;

        public void Configure(PuzzleData puzzleData)
        {
            _puzzleImage.texture = puzzleData.PuzzleTexture;
            _puzzleName.text = puzzleData.PuzzleName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ShowPuzzleConfiguration();
        }

        private void ShowPuzzleConfiguration()
        {
            // TODO: if puzzle config menu is clicked out of then it should hide, also an "x" on the menu should hide it too
            // TODO: Show the various sizes available to begin the puzzle
            // TODO: toggle rotation enabled when starting the puzzle (as a unique puzzle? or as overwrite?)
            TransitionManager.Transition(true, _puzzleImage.texture, complete: Test);
        }

        // TODO: only begin the transition once the puzzle options have been set
        private void Test()
        { 
            var loadPuzzleAsync = SceneManager.LoadSceneAsync(1); // FIXME: don't hardcode this with a magic number
            loadPuzzleAsync.completed += ClearTransition;
        }

        private void ClearTransition(AsyncOperation op)
        {
            // TODO: pass the selected image, position, etc to the PuzzlePieceGenerator
            var puzzlePieceGenerator = GameObject.FindObjectOfType<PuzzlePieceGenerator>();
            puzzlePieceGenerator.GeneratePuzzlePieces(puzzleTemplate: null, _puzzleImage.texture);
            TransitionManager.Transition(false);
        }
    }
}