using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JPWF
{
    // SOLUTION: create a new SolutionGroup, that uses a single Rigidbody to move, but they are all children of the same transform
    // ...and allow SolutionGroups to merge into a single larger SolutionGroup (in the end there'll be one big SolutionGroup w/all pieces)
    public class PuzzlePieceGroup : MonoBehaviour
    {
        private List<PuzzlePiece> _pieces = new List<PuzzlePiece>();

        public void AddPiece(PuzzlePiece piece)
        {
            if (!_pieces.Contains(piece))
            {
                _pieces.Add(piece);
                // TODO: Remove rigidbody of piece, to use the group's rigidbody
                // TODO: toggle kinematic like a piece would
                // TODO: check contacts like a piece would (except against ALL constituent pieces)
                // TODO: drag and rotate like a piece would (except about a new center/pivot, and no drag offset)
            }
        }
    }
}