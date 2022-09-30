using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JPWF
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PuzzlePiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerClickHandler
    {
        public struct SolutionPose
        {
            // all puzzles are initialized in a solved state, with pieces at 0 rotation about the z-axis
            // and as such all pieces are in a solved rotation if at 0 rotation (puzzle pieces' rotations are scrambled after setup)
            // therefore the only relevant explicit data is the local offset to a solution neighbor
            public PuzzlePiece piece;
            public Vector2 localOffset;
            public FixedJoint2D solvedJoint;

            public const float SOLUTION_ROTATION = 0.0f;
            public const float SOLVE_TOLERANCE = 0.02f; // TODO: scale this by screen size?

            /// <summary> 
            /// Returns true this pose hasn't already been solved, and this piece is close enough to being solved. 
            /// Returns false otherwise. 
            /// </summary>
            public bool ReadyToSolve(Vector3 worldPosition)
            {
                if (solvedJoint != null)
                {
                    return false;
                }

                var currentOffset = (Vector2)piece.transform.InverseTransformPoint(worldPosition);
                
                return (Mathf.Abs(localOffset.x - currentOffset.x) <= SOLVE_TOLERANCE &&
                        Mathf.Abs(localOffset.y - currentOffset.y) <= SOLVE_TOLERANCE);
            }
        }

        // must be an immediate child to allow local offset that accounts for texture size
        [SerializeField] private MeshRenderer _meshRenderer; 

        private Material _material;
        private RenderTexture _puzzlePieceTexture;
        private Rigidbody2D _rigidBody;
        private BoxCollider2D _touchCollider;
        private BoxCollider2D _pushCollider;
        private SolutionPose[] _solutionPoses = null;
        private List<PuzzlePiece> _currentTouchingPieces = new List<PuzzlePiece>();
        private int _unstackedRenderQueue; // not overlapping any pieces
        private bool _isDragged = false;
        private bool _isRotating = false;
        private float _targetRotation;

        private static Collider2D[] _neighborColliders = new Collider2D[8];
        private static readonly Vector2 DRAG_POSITION_OFFSET = Vector2.up * 0.7f;

        private const float ROTATION_INCREMENT = 90.0f;
        private const float ROTATION_SPEED = 450.0f;
        private const float ROTATION_TOLERANCE = 0.001f;
        
        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _rigidBody.gravityScale = 0.0f;
            _rigidBody.drag = 10.0f;
            _rigidBody.angularDrag = 10.0f;
            _rigidBody.useFullKinematicContacts = true;
            _rigidBody.isKinematic = false;

            _material = _meshRenderer.material;
            _unstackedRenderQueue = _material.renderQueue;
        }

        public void Init(RenderTexture puzzlePieceTexture, Bounds bounds)
        {
            _puzzlePieceTexture = puzzlePieceTexture;

            _material.SetTexture("_BaseMap", _puzzlePieceTexture);
            _meshRenderer.transform.localPosition = bounds.center;

            // detect nearby pieces, incuding solved position pieces
            _touchCollider = gameObject.AddComponent<BoxCollider2D>();
            _touchCollider.offset = Vector2.zero;
            _touchCollider.size = bounds.size;
            _touchCollider.isTrigger = true;

            // collide with play area walls
            _pushCollider = _meshRenderer.gameObject.AddComponent<BoxCollider2D>();
            _pushCollider.offset = -bounds.center;
            _pushCollider.size = bounds.size;
            _pushCollider.isTrigger = false;


            // TODO: 
            // (1) perform _touchCollider.OverlapCollider to check neighbors before the next physics update
            // (2) calculate relative neihbor solve positions so they can check if they are close
            // (3) get this piece's solved positions from all its neighbors (const reference)
            // (4) if released within a position & rotation tolerance of a solved position (or many) chose the closest one
            // ...ignore collisions...possibly create a parent group object such that dragging one drags the whole group as children
            // ...rather than relying on delayed snapping to solved positions every frame

            // LIVE ONLINE SESSION:
            // - if a player picks up a piece, then that piece is linked to the player in the database
            // - that player sends authoritative updates for that piece
            // - other/all players constantly query database (server-authority) for all puzzle piece positions
            // - OR the server runs a script to send connected players info only about updated pieces
            // *** if a player shoves pieces around...who has authority?***


            // HOW(?)
            // - POSSIBLY: when a player is online, they can send a session-code for others to join live (really just meaning that part of the DB will be jointly updated)
            // - OR: allow different people to modify the data asyncronously, if a piece is put in a solved position then that is the authority 
            // Offline => GIT-STYLE: time-stamped + player-id moves, saved in a local database, submit the database and apply versioning rules
            // Online => same moves submitted to server would imply alot more lag time instead of direct peer-to-peer updates
            
            // - only save when user hits save, otherwise just send up transform data
            // - serialize to a local file (don't update the file in realtime)
            // TODO: test this by calling a local .php script to access a local database

            // MAYBE:
            // - the server database only gets updated when the player hits "save"
            // - otherwise the puzzle piece positions are just locally synced (using HLAPI?)
        }

        public void InitSolutionNeighborhood()
        {
            var contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(~(2 << gameObject.layer));
            contactFilter.useTriggers = true;

            int neighborCount = _touchCollider.OverlapCollider(contactFilter, _neighborColliders);
            _solutionPoses = new SolutionPose[neighborCount];

            if (neighborCount > 0)
            {
                for (int i = 0; i < _solutionPoses.Length; ++i)
                {
                    _solutionPoses[i] = new SolutionPose
                    {
                        piece = _neighborColliders[i].GetComponent<PuzzlePiece>(),
                        localOffset = (Vector2)_neighborColliders[i].transform.InverseTransformPoint(transform.position)
                    };
                }
            }
        }

        private void FixedUpdate()
        {
            _currentTouchingPieces.Clear();

            if (_isRotating)
            {
                float rotation = Mathf.MoveTowards(_rigidBody.rotation, _targetRotation, ROTATION_SPEED * Time.deltaTime);
                _rigidBody.SetRotation(rotation);
                _isRotating = (Mathf.Abs(rotation - _targetRotation) > ROTATION_TOLERANCE);
            }
            else
            {
                // snap to orient along the current facing direction
                _rigidBody.SetRotation(Mathf.RoundToInt(_rigidBody.rotation / ROTATION_INCREMENT) * ROTATION_INCREMENT);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            var hitPiece = collision.GetComponent<PuzzlePiece>();

            if (hitPiece != null && !_currentTouchingPieces.Contains(hitPiece))
            {
                _currentTouchingPieces.Add(hitPiece);
            }
        }

        private void Update()
        {
            UpdateStackOrder();
        }

        // initial stack order 2450, up to 5000 max (will accomodata a 2550 piece stack)
        // TODO: discourage excessive stacking with push-collision above a max stack height (like 5 or 6) 
        private void UpdateStackOrder()
        {
            if (_isDragged)
            {
                int stackRenderQueue = _unstackedRenderQueue;
                for (int i = 0; i < _currentTouchingPieces.Count; ++i)
                {
                    if (_currentTouchingPieces[i]._material.renderQueue >= stackRenderQueue)
                    {
                        stackRenderQueue = _currentTouchingPieces[i]._material.renderQueue + 1;
                    }
                }

                _material.renderQueue = stackRenderQueue;
            }
        }

        private void InterlockPieces(ref SolutionPose selfSolutionPose)
        {
            // TODO: check that the pieces arent already interlocked
            // TODO: snap this transform to the solved position
            // TODO: add both pieces to a new SolutionGroup if neither is already part of a SolutionGroup,
            // otherwise add this individual piece to the other's SolutionGroup
            // TODO: SolutionGroup will have its own rigidbody (leveraging the individual piece colliders),
            // and will remove each piece's rigidbody as it joins
            _rigidBody.position = selfSolutionPose.piece._rigidBody.position + selfSolutionPose.localOffset;

            var solvedJoint = gameObject.AddComponent<FixedJoint2D>();
            solvedJoint.connectedBody = selfSolutionPose.piece._rigidBody;

            selfSolutionPose.solvedJoint = solvedJoint;
            var otherSolutionPose = selfSolutionPose.piece._solutionPoses.First(solutionPose => solutionPose.piece == this);
            otherSolutionPose.solvedJoint = solvedJoint;
        }

        private void CheckSolutionPoses()
        {
            if (_rigidBody.rotation == SolutionPose.SOLUTION_ROTATION &&
                _currentTouchingPieces.Count > 0)
            {
                for (int i = 0; i < _solutionPoses.Length; ++i)
                {
                    if (_currentTouchingPieces.Contains(_solutionPoses[i].piece) &&
                        _solutionPoses[i].ReadyToSolve(transform.position))
                    {
                        // TODO: particle system and snap sound
                       // InterlockPieces(ref _solutionPoses[i]);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            _puzzlePieceTexture.Release();
            _puzzlePieceTexture = null;
        }

        // TODO: disable all event inputs on an individual piece once its part of a group
        // use the group inputs instead
        public void OnPointerDown(PointerEventData eventData)
        {

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isDragged = false;
            CheckSolutionPoses();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isRotating && !_isDragged)
            {
                _isRotating = true;
                _targetRotation = _rigidBody.rotation + ROTATION_INCREMENT;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            // FIXME: don't use Camera.main here
            _isDragged = true;
            _rigidBody.MovePosition((Vector2)Camera.main.ScreenToWorldPoint(eventData.position));
        }
    }
}
