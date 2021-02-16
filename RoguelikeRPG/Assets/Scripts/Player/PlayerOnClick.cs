using UnityEngine;

namespace Player
{
    public class PlayerOnClick : MonoBehaviour
    {
        public float maxSpeed = 5f; // Max Speed for player
        public float turnSpeed = 15f; // Turn Speed for player

        private Animator _anim; // To control differences on animation from script
        private CharacterController _controller; // Get character controller component
        private CollisionFlags _collisionFlags = CollisionFlags.None; // To get contact info from capsule collider

        private Vector3 _playerMove = Vector3.zero; // Hold info of where player will move
        private Vector3 _targetMovePoint = Vector3.zero; // Target point

        private float _currentSpeed;
        private float _playerToPointDistance; // Distance between clicked point and player
        private float _gravity = 9.8f;
        private float _height;

        private bool _canMove; // Check if player can move
        private bool _finishedMovement = true; // True if game started or spesific skills are used
        private Vector3 _newMovePoint;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            _currentSpeed = maxSpeed;
        }

        private void Update()
        {
            CalculateHeight();
            CheckIfFinishedMovement();
        }

        private bool IsGrounded()
        {
            return _collisionFlags == CollisionFlags.CollidedBelow ? true : false;
        }

        private void CalculateHeight()
        {
            if (IsGrounded())
            {
                _height = 0f;
            }
            else
            {
                _height -= _gravity * Time.deltaTime;
            }
        }

        private void CheckIfFinishedMovement()
        {
            if (!_finishedMovement)
            {
                if (_anim.IsInTransition(0) && !_anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                    _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
                {
                    _finishedMovement = true;
                }
            }
            else
            {
                MovePlayer();
                _playerMove.y = _height * Time.deltaTime;
                _collisionFlags = _controller.Move(_playerMove);
            }
        }

        private void MovePlayer()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    _playerToPointDistance = Vector3.Distance(transform.position, hit.point);
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        if (_playerToPointDistance >= 1.0f)
                        {
                            _canMove = true;
                            _targetMovePoint = hit.point;
                        }
                    }
                }
            }

            if (_canMove)
            {
                _anim.SetFloat("Speed", 1.0f);
                _newMovePoint = new Vector3(_targetMovePoint.x, transform.position.y, _targetMovePoint.z);
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(_newMovePoint - transform.position), turnSpeed * Time.deltaTime);
                _playerMove = transform.forward * _currentSpeed * Time.deltaTime;

                if (Vector3.Distance(transform.position, _newMovePoint) < 0.6f)
                {
                    _canMove = false;
                }
            }
            else
            {
                _playerMove.Set(0f, 0f, 0f);
                _anim.SetFloat("Speed", 0f);
            }
        }
    }
}