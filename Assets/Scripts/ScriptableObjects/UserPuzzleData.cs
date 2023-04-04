using UnityEngine;

namespace JPWF
{
    [CreateAssetMenu(fileName = "NewUserPuzzleData", menuName = "Data/User Puzzle Data")]
    public class UserPuzzleData : ScriptableObject
    {
        [SerializeField] private PuzzleData _puzzleData;
        [SerializeField] private PuzzleTemplateData _puzzleTemplate; // template user selected for this puzzle
        [SerializeField] private Vector3 _texturePose; // (xy) offset and (z) scale of the texture within the template area

        public Texture PuzzleTexture => _puzzleData.PuzzleTexture;
        public string PuzzleName => _puzzleData.PuzzleName;
        public int NumPieces => _puzzleTemplate.NumPieces;
        //public PuzzlePieceLocation[] // TODO: ScriptableObject tracking the current place (on deck/table, solo/grouped, xy position)
    }
}