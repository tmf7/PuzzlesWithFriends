using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JPWF
{
    [RequireComponent(typeof(MeshRenderer), typeof(BoxCollider2D))]
    public class PuzzlePiece : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerClickHandler
    {
        /// <summary>
        /// Describes a neighboring piece in the constructed puzzle, 
        /// and this piece's localspace offset from it.
        /// </summary>
        private struct SolutionPose
        {
            // all puzzles are initialized in a solved state, with pieces at 0 rotation about the z-axis
            // and 0 rotation relative to their neighbors, so the local rotation offset must always be zero.
            public PuzzlePiece self;
            public PuzzlePiece neighbor;
            public Vector2 localOffset;

            public const float SOLVE_TOLERANCE = 0.1f; // TODO: scale this by screen size?

            /// <summary> 
            /// Returns true this pose hasn't already been solved, and this piece is close enough to being solved. 
            /// Returns false otherwise. 
            /// </summary>
            public bool ReadyToSolve()
            {
                // already solved for this group
                if (self.m_puzzlePieceGroup != null && neighbor.m_puzzlePieceGroup == self.m_puzzlePieceGroup)
                {
                    return false;
                }

                var currentOffset = (Vector2)neighbor.transform.InverseTransformPoint(self.transform.position);

                return (Mathf.Abs(localOffset.x - currentOffset.x) <= SOLVE_TOLERANCE &&
                        Mathf.Abs(localOffset.y - currentOffset.y) <= SOLVE_TOLERANCE) &&
                        Mathf.Abs(neighbor.RigidBody.rotation - self.RigidBody.rotation) % 360.0f <= SOLVE_TOLERANCE;
            }
        }

        private PuzzlePieceGroup m_puzzlePieceGroup = null;
        private Camera m_mainCamera;
        private MeshRenderer m_meshRenderer;
        private Rigidbody2D m_rigidBody;
        private BoxCollider2D m_touchCollider; // detect nearby pieces, incuding solved position pieces
        private SolutionPose[] m_solutionPoses = null;
        private List<PuzzlePiece> m_currentTouchingPieces = new List<PuzzlePiece>();
        private float m_unstackedDepth; // not overlapping any pieces
        private Vector3 m_initialTouchOffset;
        private float m_targetRotation;
        private bool m_isDragged = false;
        private bool m_isRotating = false;

        private static Collider2D[] _tempNeighborColliders = new Collider2D[8];

        public const float ROTATION_INCREMENT = 90.0f;
        public const float ROTATION_SPEED = 450.0f;
        public const float ROTATION_TOLERANCE = 0.001f;

        private const float PIECE_THICKNESS = 0.01f;

        /// <summary> 
        /// Get returns true if this piece or its PuzzlePieceGroup is being dragged. 
        /// Set only configures the local drag state.
        /// </summary>
        public bool IsDragged 
        {
            get => m_puzzlePieceGroup == null ? m_isDragged : m_puzzlePieceGroup.IsDragged;
            set => m_isDragged = value;
        }

        /// <summary> 
        /// Get returns true if this piece or its <see cref="PuzzlePieceGroup"/> is being rotated. 
        /// Set only configures the local rotating state.
        /// </summary>
        public bool IsRotating
        {
            get => m_puzzlePieceGroup == null ? m_isRotating : m_puzzlePieceGroup.IsRotating;
            set => m_isRotating = value;
        }

        /// <summary> 
        /// Rigibody of the piece, or the group if this belongs to one. 
        /// NOTE: this piece's Rigidbody is destroyed when joining a <see cref="PuzzlePieceGroup"/>. 
        /// </summary>
        private Rigidbody2D RigidBody => m_puzzlePieceGroup == null ? m_rigidBody : m_puzzlePieceGroup.RigidBody;

        /// <summary>
        /// Sets the z-position of this piece or its <see cref="PuzzlePieceGroup"/> to affect
        /// render order and raycast priority.
        /// </summary>
        private float StackDepth
        {
            get => m_puzzlePieceGroup == null ? transform.position.z : m_puzzlePieceGroup.StackDepth;
            set
            {
                if (m_puzzlePieceGroup == null)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, value);
                }
                else
                {
                    m_puzzlePieceGroup.StackDepth = value;
                }
            }
        }

        private void Awake()
        {
            m_meshRenderer = GetComponent<MeshRenderer>();
            m_rigidBody = GetComponent<Rigidbody2D>();
            m_touchCollider = GetComponent<BoxCollider2D>();

            m_unstackedDepth = StackDepth;
            m_touchCollider.offset = Vector2.zero;
            m_touchCollider.isTrigger = true;
            m_rigidBody.useFullKinematicContacts = true;
            m_rigidBody.isKinematic = true;
        }

        /// <param name="puzzlePieceMaterial"> Draws the puzzle piece. </param>
        /// <param name="bounds"> Trigger Collider 2D bounds to determine neighbors and interlock conditions. </param>
        /// <param name="camera"> Camera used to translate cursor positions from screen to worldspace when moving thie piece. </param>
        public void Init(Material puzzlePieceMaterial, Bounds bounds, Camera camera)
        {
            m_mainCamera = camera;
            m_meshRenderer.material = puzzlePieceMaterial;
            m_touchCollider.size = bounds.size;

            // LIVE ONLINE SESSION:
            // - if a player picks up a piece, then that piece is linked to the player in the database
            // - that player sends authoritative updates for that piece
            // - other/all players constantly query database (server-authority) for all puzzle piece positions
            // - OR the server runs a script to send connected players info only about updated pieces

            // - POSSIBLY: when a player is online, they can send a session-code for others to join live (really just meaning that part of the DB will be jointly updated)
            // - OR: allow different people to modify the data asyncronously, if a piece is put in a solved position then that is the authority 
            // Offline => GIT-STYLE: time-stamped + player-id moves, saved in a local database, submit the database and apply versioning rules
            // Online => same moves submitted to server would imply alot more lag time instead of direct peer-to-peer updates
            // TODO: test this by calling a local .php script to access a local database
        }

        /// <summary> 
        /// Called at the end of <see cref="PuzzlePieceGenerator.GeneratePuzzlePieces(PuzzleTemplateData, Texture)"/> when all pieces
        /// are placed in their solved world poses. Performs an overlap test to determine local space neighbors and offset poses.
        /// Solved rotation is always identity.
        /// </summary>
        public void InitSolutionNeighborhood()
        {
            var contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(~(2 << gameObject.layer));
            contactFilter.useTriggers = true;

            int neighborCount = m_touchCollider.OverlapCollider(contactFilter, _tempNeighborColliders);
            m_solutionPoses = new SolutionPose[neighborCount];

            if (neighborCount > 0)
            {
                for (int i = 0; i < m_solutionPoses.Length; ++i)
                {
                    m_solutionPoses[i] = new SolutionPose
                    {
                        self = this,
                        neighbor = _tempNeighborColliders[i].GetComponent<PuzzlePiece>(),
                        localOffset = (Vector2)_tempNeighborColliders[i].transform.InverseTransformPoint(transform.position)
                    };
                }
            }
        }

        private void FixedUpdate()
        {
            // rotate via the group once it's set
            if (m_puzzlePieceGroup != null)
            {
                m_currentTouchingPieces.Clear();
                return;
            }

            bool wasRotating = IsRotating;

            if (IsRotating)
            {
                float rotation = Mathf.MoveTowards(m_rigidBody.rotation, m_targetRotation, ROTATION_SPEED * Time.deltaTime);
                m_rigidBody.SetRotation(rotation);
                IsRotating = (Mathf.Abs(rotation - m_targetRotation) > ROTATION_TOLERANCE);
            }

            if (wasRotating && !IsRotating)
            {
                // FIXME: if rotating into a solved position, then the rotation is still a bit wonky
                // FIXME: if moving a GROUP into an individual piece, then the translation can sometimes be wonky too
                // snap to orient along the current facing direction
                m_rigidBody.SetRotation(Mathf.RoundToInt(m_rigidBody.rotation / ROTATION_INCREMENT) * ROTATION_INCREMENT);
                CheckSolutionPoses();
            }

            m_currentTouchingPieces.Clear();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            var hitPiece = collision.GetComponent<PuzzlePiece>();

            if (hitPiece != null && !m_currentTouchingPieces.Contains(hitPiece))
            {
                m_currentTouchingPieces.Add(hitPiece);
            }
        }

        private void Update()
        {
            UpdateStackOrder();
        }

        /// <summary> 
        /// Ensure the most recently interacted-with piece/group is the nearest 
        /// visible to the camera and the first hit by any physics raycasts.
        /// </summary>
        private void UpdateStackOrder()
        {
            if (IsDragged || IsRotating)
            {
                float stackDepth = m_unstackedDepth;
                for (int i = 0; i < m_currentTouchingPieces.Count; ++i)
                {
                    if (m_currentTouchingPieces[i].StackDepth <= stackDepth)
                    {
                        stackDepth = m_currentTouchingPieces[i].StackDepth - PIECE_THICKNESS;
                    }
                }

                StackDepth = stackDepth;
            }
        }

        /// <summary>
        /// Interlocks pieces in a near-engough solution pose.
        /// Checked at the end of every user-initiated translation and rotation.
        /// </summary>
        public void CheckSolutionPoses()
        {
            if (m_currentTouchingPieces.Count > 0)
            {
                for (int i = 0; i < m_solutionPoses.Length; ++i)
                {
                    // The migragion of their RigidBody2D to the common parent
                    // ensures _currentTouchingPieces never contains siblings of a
                    // PuzzlePieceGroup so that doesn't need to be checked here
                    if (m_currentTouchingPieces.Contains(m_solutionPoses[i].neighbor) &&
                        m_solutionPoses[i].ReadyToSolve())
                    {
                        // TODO: particle system and snap sound

                        // FIXME: a shimmer effect should occur at every merge point
                        // ie: if two vertical groups of 3 pieces merge, then there should be 3 shimmers at once
                        // PROBLEM: although the group is performing a CheckSolutionPoses for ALL pieces in its control
                        // the first of its pieces that see it'll merge will cascade SetGroup for all its fellow members
                        // ...THEN ReadyToSolve will return false for the other members and there won't be a satisfying cascade of shimmer along the merge line
                        // SOLUTION: make an entire piece shimmer as it is added to a group, and delay the start of each shimmer by a few frames
                        // ...in a nearest-neighbor way...? to a limit?

                        // PROBLEM(?): adding one piece in the middle of a group...who shimmers? probably both the piece and the group...to a limit
                        InterlockPieces(ref m_solutionPoses[i]);
                    }
                }
            }
        }

        public void SetGroup(PuzzlePieceGroup puzzlePieceGroup)
        {
            m_puzzlePieceGroup = puzzlePieceGroup;
            m_puzzlePieceGroup.AddPiece(this);
        }

        /// <summary>
        /// Adds one or both pieces to a <see cref="PuzzlePieceGroup"/> after snapping
        /// the piece or group into position.
        /// </summary>
        private void InterlockPieces(ref SolutionPose solutionPose)
        {
            Vector3 pieceSnapWorldPosition = (solutionPose.neighbor.transform.position + (Vector3)solutionPose.localOffset);

            if (m_puzzlePieceGroup == null)
            {
                // snap the individual piece into position
                transform.position = pieceSnapWorldPosition;

                if (solutionPose.neighbor.m_puzzlePieceGroup == null)
                {
                    var newPuzzlePieceGroup = new GameObject($"{nameof(PuzzlePieceGroup)} ({PuzzlePieceGroup.GroupNumber})").AddComponent<PuzzlePieceGroup>();
                    newPuzzlePieceGroup.transform.position = transform.position; // center on the originating piece

                    SetGroup(newPuzzlePieceGroup);
                    solutionPose.neighbor.SetGroup(newPuzzlePieceGroup);
                }
                else
                {
                    SetGroup(solutionPose.neighbor.m_puzzlePieceGroup);
                }
            }
            else // _puzzlePieceGroup != null
            {
                // snap the entire group into position
                m_puzzlePieceGroup.transform.position = pieceSnapWorldPosition - transform.localPosition;

                if (solutionPose.neighbor.m_puzzlePieceGroup == null)
                {
                    solutionPose.neighbor.SetGroup(m_puzzlePieceGroup);
                }
                else
                {
                    // FIXME: a shimmer effect should occur at every merge point
                    // ie: if two vertical groups of 3 pieces merge, then there should be 3 shimmers at once (along the seam)
                    m_puzzlePieceGroup.AddGroup(solutionPose.neighbor.m_puzzlePieceGroup);
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            BeginTouch(m_mainCamera.ScreenToWorldPoint(eventData.position));
        }

        public void OnDrag(PointerEventData eventData)
        {
            Drag(m_mainCamera.ScreenToWorldPoint(eventData.position));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            FinishTouch();
        }

        private void BeginTouch(Vector3 touchWorldPosition)
        {
            if (m_puzzlePieceGroup == null)
            {
                m_initialTouchOffset = (transform.position - touchWorldPosition) + (Vector3.forward * StackDepth);
            }
            else
            {
                m_puzzlePieceGroup.BeginTouch(touchWorldPosition);
            }
        }

        private void Drag(Vector3 dragWorldPosition)
        {
            if (m_puzzlePieceGroup == null)
            {
                IsDragged = true;
                transform.position = m_initialTouchOffset + dragWorldPosition + (Vector3.forward * StackDepth);
            }
            else
            {
                m_puzzlePieceGroup.Drag(dragWorldPosition);
            }
        }

        private void FinishTouch()
        {
            if (m_puzzlePieceGroup == null)
            {
                if (!IsRotating && !IsDragged)
                {
                    IsRotating = true;
                    m_targetRotation = m_rigidBody.rotation + ROTATION_INCREMENT;
                }

                if (IsDragged)
                {
                    IsDragged = false;
                    CheckSolutionPoses();
                }
            }
            else
            {
                m_puzzlePieceGroup.FinishTouch(transform.position);
            }
        }
    }
}
