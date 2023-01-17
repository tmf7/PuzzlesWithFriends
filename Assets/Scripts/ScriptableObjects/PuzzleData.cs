using UnityEngine;

namespace JPWF
{
    [CreateAssetMenu(fileName = "NewPuzzleData", menuName = "Data/Puzzle Data")]
    public class PuzzleData : ScriptableObject
    {
        [SerializeField] private Texture _puzzleTexture;
        [SerializeField] private string _puzzleName;

        public Texture PuzzleTexture => _puzzleTexture;
        public string PuzzleName => _puzzleName;
    }
}