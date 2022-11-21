namespace PlateformCharacterController2D
{
    
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class CharacterController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Character Parameters")] 
        
        [SerializeField]
        private Walk _walk;
        [SerializeField] 
        private Jump _jump;
        [SerializeField] 
        private CollisionBox _collisionBox;
        [SerializeField] 
        private Gravity _gravity;

        [Space(10), Header("Collisions")]
        
        [SerializeField]
        private LayerMask _groundLayerToDetect;
        [SerializeField] 
        private int _collisionDetectorCount = 3;
        [SerializeField] 
        private float _detectionRaycastLength = 0.1f;
        [SerializeField, Range(0.1f, 0.3f),  Tooltip("Prevents side detectors hitting the ground")]
        private float _rayBuffer = 0.1f;
        [Range(0, 20), SerializeField, Tooltip("Increases collision accuracy at the cost of performance")]
        private int _freeColliderIterations = 10;

        [Space(10), Header("Gizmos")]
        
        [SerializeField]
        private bool _enableGizmos = true;
        [SerializeField] 
        private Color _gizmosCollisionBoxColor = Color.white;
        
        #endregion

        #region Privates variables

        //movement
        private bool _isGrounded => _isCollidedDown;
        private float _currentHorizontalSpeed, _currentVerticalSpeed;
        private bool _canMove = true;
        private Vector3 _rawMovement;
        private Vector3 _velocity;
        private Vector3 _lastPosition;

        //collisions
        private RayRange _rayRangeUp, _rayRangeRight, _rayRangeDown, _rayRangeLeft;
        private bool _isCollidedUp, _isCollidedRight, _isCollidedDown, _isCollidedLeft;

        //other
        private GUIStyle _guiStyle = new();
        private Inputs _inputs;

        #endregion

        private void Awake()
        {
            _guiStyle.alignment = TextAnchor.MiddleCenter;
        }

        private void Update()
        {
            if (!_canMove) return;

            // Calculate velocity
            _velocity = (transform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = transform.position;

            //inputs and collisions
            GatherInput();
            RunCollisionChecks();

            //x and y speed calculation
            CalculateWalk(); // Horizontal movement
            CalculateJumpApex(); // Affects fall speed, so calculate before gravity
            CalculateGravity(); // Vertical movement
            CalculateJump(); // Possibly overrides vertical

            //moving
            MoveCharacter(); // Actually perform the axis movement
        }

        #region GatherInput

        private void GatherInput()
        {
            _inputs = new Inputs
            {
                Jump = Input.GetButtonDown("Jump"),
                X = Input.GetAxisRaw("Horizontal")
            };
        }

        #endregion

        #region Collisions

        //Checks for pre-collision information
        private void RunCollisionChecks()
        {
            //Generate ray ranges
            CalculateRayRanged();

            //assign collided booleans
            bool isCollidedDown = IsRayRangeDetectingColliding(_rayRangeDown);
            _isCollidedUp = IsRayRangeDetectingColliding(_rayRangeUp);
            _isCollidedLeft = IsRayRangeDetectingColliding(_rayRangeLeft);
            _isCollidedRight = IsRayRangeDetectingColliding(_rayRangeRight);

            bool IsRayRangeDetectingColliding(RayRange range)
            {
                return EvaluateRayRangePositions(range).Any(point =>
                    Physics2D.Raycast(point, range.Direction, _detectionRaycastLength, _groundLayerToDetect));
            }
            
            _isCollidedDown = isCollidedDown;
        }

        private void CalculateRayRanged()
        {
            Vector3 center = transform.position +
                         new Vector3(_collisionBox.CollisionBoxOffsetX, _collisionBox.CollisionBoxOffsetY, 0);
            Vector3 size = new Vector3(_collisionBox.CollisionBoxWidth, _collisionBox.CollisionBoxHeight, 1);
            Bounds bounds = new Bounds(center, size);

            _rayRangeDown = new RayRange(bounds.min.x + _rayBuffer, bounds.min.y, bounds.max.x - _rayBuffer,
                bounds.min.y,
                Vector2.down);
            _rayRangeUp = new RayRange(bounds.min.x + _rayBuffer, bounds.max.y, bounds.max.x - _rayBuffer, bounds.max.y,
                Vector2.up);
            _rayRangeLeft = new RayRange(bounds.min.x, bounds.min.y + _rayBuffer, bounds.min.x,
                bounds.max.y - _rayBuffer,
                Vector2.left);
            _rayRangeRight = new RayRange(bounds.max.x, bounds.min.y + _rayBuffer, bounds.max.x,
                bounds.max.y - _rayBuffer,
                Vector2.right);
        }


        private IEnumerable<Vector2> EvaluateRayRangePositions(RayRange range)
        {
            for (int i = 0; i < _collisionDetectorCount; i++)
            {
                float slerpInterpolate = (float)i / (_collisionDetectorCount - 1);
                yield return Vector2.Lerp(range.Start, range.End, slerpInterpolate);
            }
        }

        #endregion

        #region Walk

        private void CalculateWalk()
        {
            if (_inputs.X != 0)
            {
                //set horizontal speed and clamp it
                _currentHorizontalSpeed += _inputs.X * _walk.Acceleration * Time.deltaTime;
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_walk.MaximumSpeed, _walk.MaximumSpeed);

                // Apply bonus at the apex of a jump
                float apexBonus = Mathf.Sign(_inputs.X) * _walk.ApexBonus * _apexPoint;
                _currentHorizontalSpeed += apexBonus * Time.deltaTime;
            }
            // Slow the character down if no input
            else
            {
                _currentHorizontalSpeed =
                    Mathf.MoveTowards(_currentHorizontalSpeed, 0, _walk.Deceleration * Time.deltaTime);
            }

            // Don't walk through walls
            if (_currentHorizontalSpeed > 0 && _isCollidedRight || _currentHorizontalSpeed < 0 && _isCollidedLeft)
            {
                _currentHorizontalSpeed = 0;
            }
        }

        #endregion

        #region Gravity

        private float _fallSpeed;

        private void CalculateGravity()
        {
            if (_isCollidedDown)
            {
                // Move out of the ground
                if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
            }
            else
            {
                //Set fallSpeed and clamp it
                _currentVerticalSpeed -= _fallSpeed * Time.deltaTime;
                if (_currentVerticalSpeed < _gravity._fallClamp) _currentVerticalSpeed = _gravity._fallClamp;
            }
        }

        #endregion

        #region Jump

        private float _apexPoint; // Becomes 1 at the apex of a jump
        private float _lastJumpPressed;
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");

        private void CalculateJumpApex()
        {
            if (!_isCollidedDown)
            {
                //Increase the closer to the top of the jump
                _apexPoint = Mathf.InverseLerp(_jump.JumpApexThreshold, 0, Mathf.Abs(_velocity.y));
                _fallSpeed = Mathf.Lerp(_gravity._minFallSpeed, _gravity._maxFallSpeed, _apexPoint);
            }
            else _apexPoint = 0;
        }

        private void CalculateJump()
        {
            //jump
            if (_inputs.Jump && _isGrounded)
            {
                _currentVerticalSpeed = _jump.JumpHeight;
            }

            //up collision
            if (_isCollidedUp && _currentVerticalSpeed > 0)
            {
                _currentVerticalSpeed = 0;
            }
        }

        #endregion

        #region Move

        // We cast our bounds before moving to avoid future collisions
        private void MoveCharacter()
        {
            //collider box variables
            Vector3 colliderBoxCenterPosition = transform.position +
                                            new Vector3(_collisionBox.CollisionBoxOffsetX,
                                                _collisionBox.CollisionBoxOffsetY, 0);
            Vector3 colliderBoxSize = new Vector3(_collisionBox.CollisionBoxWidth, _collisionBox.CollisionBoxHeight, 0);

            //movement variables
            _rawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); //movement
            Vector3 move = _rawMovement * Time.deltaTime; //movement adjusted with deltaTime
            Vector3 furthestPoint = colliderBoxCenterPosition + move; //next position

            // check next position. If nothing hit, move and don't do extra checks
            Collider2D hit = Physics2D.OverlapBox(furthestPoint, colliderBoxSize, 0, _groundLayerToDetect);
            if (!hit)
            {
                transform.position += move;
                return;
            }

            // otherwise increment away from current pos; see what closest position we can move to
            Vector3 positionToMoveTo = transform.position;
            for (int i = 1; i < _freeColliderIterations; i++)
            {
                // increment to check all but furthestPoint - we did that already
                float lerpInterpolate = (float)i / _freeColliderIterations;
                Vector2 positionToTry = Vector2.Lerp(colliderBoxCenterPosition, furthestPoint, lerpInterpolate);

                if (Physics2D.OverlapBox(positionToTry, colliderBoxSize, 0, _groundLayerToDetect))
                {
                    transform.position = positionToMoveTo;

                    //If landed on a corner or hit our head on a ledge, move the player
                    if (i != 1) return;
                    if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
                    Vector3 direction = transform.position - hit.transform.position;
                    transform.position += direction.normalized * move.magnitude;

                    return;
                }

                positionToMoveTo = positionToTry;
            }
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            if (!_enableGizmos) return;

#if UNITY_EDITOR

            Vector3 position = transform.position;

            //collision
            ChangeGizmosColor(_gizmosCollisionBoxColor);
            Vector3 boxPosition =
                position + new Vector3(_collisionBox.CollisionBoxOffsetX, _collisionBox.CollisionBoxOffsetY, 0);
            Vector3 boxTextPosition = position + new Vector3(_collisionBox.CollisionBoxOffsetX,
                _collisionBox.CollisionBoxHeight / 2 + _collisionBox.CollisionBoxOffsetY + 0.5f, 0);
            Gizmos.DrawWireCube(boxPosition,
                new Vector2(_collisionBox.CollisionBoxWidth, _collisionBox.CollisionBoxHeight));
            UnityEditor.Handles.Label(boxTextPosition, "Collision Box", _guiStyle);

            //raycast colliders in each direction
            Vector2 gizmoSize = new Vector2(0.25f, 0.25f);
            Vector2 leftPosition = boxPosition + new Vector3(-1, 0),
                rightPosition = boxPosition + new Vector3(1, 0),
                upPosition = boxPosition + new Vector3(0, 1),
                downPosition = boxPosition + new Vector3(0, -1);
            //left
            ChangeGizmosColor(_isCollidedLeft ? Color.green : Color.red);
            Gizmos.DrawCube(leftPosition, gizmoSize);
            //right
            ChangeGizmosColor(_isCollidedRight ? Color.green : Color.red);
            Gizmos.DrawCube(rightPosition, gizmoSize);
            //up
            ChangeGizmosColor(_isCollidedUp ? Color.green : Color.red);
            Gizmos.DrawCube(upPosition, gizmoSize);
            //down
            ChangeGizmosColor(_isCollidedDown ? Color.green : Color.red);
            Gizmos.DrawCube(downPosition, gizmoSize);
            
            //velocity text
            ChangeGizmosColor(_gizmosCollisionBoxColor);
            Vector3 VelocityTextPosition = transform.localPosition + new Vector3(-1.85f, -0.5f, 0);
            string velocityText = $"X velocity = {System.Math.Round(_velocity.x)}\n Y velocity = {System.Math.Round(_velocity.y)}";
            UnityEditor.Handles.Label(VelocityTextPosition, velocityText, _guiStyle);
#endif
        }

        private void ChangeGizmosColor(Color color)
        {
            Gizmos.color = color;
            _guiStyle.normal.textColor = color;
        }

        #endregion
    }
}