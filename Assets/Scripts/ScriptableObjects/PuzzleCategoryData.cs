using UnityEngine;

namespace JPWF
{
    [CreateAssetMenu(fileName = "NewPuzzleCategory", menuName = "Data/Puzzle Category")]
    public class PuzzleCategoryData : ScriptableObject
    {
        [SerializeField] private string _categoryName;
        [SerializeField] private PuzzleData[] _puzzles;

        public PuzzleData[] Puzzles => _puzzles;
        public string CategoryName => _categoryName;
    }
}