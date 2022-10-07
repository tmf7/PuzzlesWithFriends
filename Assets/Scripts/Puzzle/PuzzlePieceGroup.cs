using System.Collections.Generic;
using UnityEngine;

namespace JPWF
{
    // TODO: create a new PuzzlePieceGroup when two or more pieces interlock, make all pieces children of the same transform
    // TODO: allow SolutionGroups to merge into a single larger SolutionGroup (in the end there'll be one big SolutionGroup w/all pieces)
    public class PuzzlePieceGroup : MonoBehaviour
    {
        public static int GroupNumber { get; private set; } = 1;

        private List<PuzzlePiece> m_pieces = new List<PuzzlePiece>();
        private Rigidbody2D m_rigidBody;
        private Vector3 m_initialTouchOffset;
        private float m_targetRotation;
        private bool m_isDragged = false;
        private bool m_isRotating = false;

        public bool IsDragged
        {
            get => m_isDragged;
            private set
            {
                m_isDragged = value;

                for (int i = 0; i < m_pieces.Count; ++i)
                {
                    m_pieces[i].IsDragged = m_isDragged;
                }
            }
        }

        public bool IsRotating
        {
            get => m_isRotating;
            private set
            {
                m_isRotating = value;

                for (int i = 0; i < m_pieces.Count; ++i)
                {
                    m_pieces[i].IsRotating = m_isRotating;
                }
            }
        }

        public float StackDepth
        {
            get => transform.position.z;
            set
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, value);
            }
        }

        public Rigidbody2D RigidBody => m_rigidBody;

        private void Awake()
        {
            m_rigidBody = gameObject.AddComponent<Rigidbody2D>();
            m_rigidBody.isKinematic = true;
            m_rigidBody.useFullKinematicContacts = true;
            GroupNumber++;
        }

        public void AddGroup(PuzzlePieceGroup puzzlePieceGroup)
        {
            for (int i = 0; i < puzzlePieceGroup.m_pieces.Count; ++i)
            {
                puzzlePieceGroup.m_pieces[i].SetGroup(this);
            }

            // the other group is no longer needed
            Destroy(puzzlePieceGroup.gameObject); 
        }

        public void AddPiece(PuzzlePiece piece)
        {
            if (!m_pieces.Contains(piece))
            {
                m_pieces.Add(piece);
                // ensure all pieces move together at the same depth
                piece.transform.SetParent(transform, true);
                piece.transform.localPosition = new Vector3(piece.transform.localPosition.x, piece.transform.localPosition.y, 0.0f);

                var pieceRigidBody = piece.GetComponent<Rigidbody2D>();
                if (pieceRigidBody != null)
                {
                    Destroy(pieceRigidBody); // use the group's rigidbody now, ignore sibling collisions
                }

                // FIXME(?): can a large group interlock with another large group (multiple pieces at once)? 
                // SOLUTION: probably, as long as each piece is considered dragged/rotated during a group interaction, at the end of which CheckSolutionPoses
                // would auto-call on each constituent piece.

                // TODO: check contacts like a piece would (except against ALL constituent pieces)
                // TODO: drag as a piece does (from where the user grabbed)
                // TODO: rotate around the closest touched piece of the group

                //transform.RotateAround
            }
        }

        public void BeginTouch(Vector3 touchWorldPosition)
        {
            m_initialTouchOffset = (transform.position - touchWorldPosition) + (Vector3.forward * StackDepth);
        }

        public void Drag(Vector3 dragWorldPosition)
        {
            IsDragged = true;
            transform.position = m_initialTouchOffset + dragWorldPosition + (Vector3.forward * StackDepth);
        }

        public void FinishTouch(Vector3 touchWorldPosition)
        {
            // rotate around the touched piece's pivot and check solutions
            if (!IsRotating && !IsDragged)
            {
                IsRotating = true;
                m_targetRotation = m_rigidBody.rotation + PuzzlePiece.ROTATION_INCREMENT;
            }

            if (IsDragged)
            {
                IsDragged = false;
                CheckSolutionPoses();
            }
        }

        private void CheckSolutionPoses()
        {
            for (int i = 0; i < m_pieces.Count; ++i)
            {
                m_pieces[i].CheckSolutionPoses();
            }
        }
    }
}