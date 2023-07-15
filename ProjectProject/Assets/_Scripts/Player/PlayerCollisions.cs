using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    [SerializeField] private Bounds _characterBounds;

    private enum CollisionCorrectionStyle { Instant, Lerp}
    [SerializeField] private CollisionCorrectionStyle _collisionCorrectionStyle;

    [SerializeField] private bool secondaryCollisionCorrection = true;

    private const float _collisionCorrectionSpeed = 45f;

    private const float _collisionMainRayLenght = 0.1f;

    private const float _collisionRayLenght = 0.3f;

    private Vector3 _targetTranformPosition;

    private Vector3 RelativeCenter => transform.position + _characterBounds.center;

    private RaycastHit _upColHit, _upLeftColHit, _upRightColHit,
                       _downColHit, _downLeftColHit, _downRightColHit,
                       _leftColHit, _leftUpperColHit, _leftLowerColHit,
                       _rightColHit, _rightUpperColHit, _rightLowerColHit;

    private bool _isCollidingUpMain, _isCollidingUpLeft, _isCollidingUpRight,
                 _isCollidingDownMain, _isCollidingDownLeft, _isCollidingDownRight,
                 _isCollidingLeftLower, _isCollidingLeftUpper,
                 _isCollidingLeftMain, _isCollidingRightMain,
                 _isCollidingRightLower, _isCollidingRightUpper;

    private Vector3 LeftRelative => RelativeCenter + new Vector3(-_characterBounds.extents.x / 2, 0);
    private Vector3 RightRelative => RelativeCenter + new Vector3(_characterBounds.extents.x / 2, 0);
    private Vector3 UpperRelative => RelativeCenter + new Vector3(0, _characterBounds.extents.y / 2);
    private Vector3 LowerRelative => RelativeCenter + new Vector3(0, -_characterBounds.extents.y / 2);

    private Ray UpperLeftRay => new Ray(LeftRelative, Vector3.up);
    private Ray UpperRightRay => new Ray(RightRelative, Vector3.up);
    private Ray LowerLeftRay => new Ray(LeftRelative, Vector3.down);
    private Ray LowerRightRay => new Ray(RightRelative, Vector3.down);
    private Ray LeftUpperRay => new Ray(UpperRelative, Vector3.left);
    private Ray RightUpperRay => new Ray(UpperRelative, Vector3.right);
    private Ray LeftLowerRay => new Ray(LowerRelative, Vector3.left);
    private Ray RightLowerRay => new Ray(LowerRelative, Vector3.right);

    private void RaycastForCollisions()
    {
        _isCollidingUpMain = Physics.Raycast(RelativeCenter, Vector3.up, out _upColHit,
            _characterBounds.extents.y + _collisionMainRayLenght, 3);

        _isCollidingDownMain = Physics.Raycast(RelativeCenter, Vector3.down, out _downColHit,
            _characterBounds.extents.y + _collisionMainRayLenght, 3);

        _isCollidingLeftMain = Physics.Raycast(RelativeCenter, Vector3.left, out _leftColHit,
            _characterBounds.extents.x + _collisionMainRayLenght, 3);

        _isCollidingRightMain = Physics.Raycast(RelativeCenter, Vector3.right, out _rightColHit,
            _characterBounds.extents.x + _collisionMainRayLenght, 3);

        
        
        _isCollidingUpLeft = Physics.Raycast(UpperLeftRay, out _upLeftColHit,
            _characterBounds.extents.y + _collisionRayLenght, 3);

        _isCollidingUpRight = Physics.Raycast(UpperRightRay, out _rightColHit,
            _characterBounds.extents.y + _collisionRayLenght, 3);

        _isCollidingDownLeft = Physics.Raycast(LowerLeftRay, out _downLeftColHit,
            _characterBounds.extents.y + _collisionRayLenght, 3);

        _isCollidingDownRight = Physics.Raycast(LowerRightRay, out _downRightColHit,
            _characterBounds.extents.y + _collisionRayLenght, 3);

        _isCollidingLeftUpper = Physics.Raycast(LeftUpperRay, out _leftUpperColHit,
            _characterBounds.extents.x + _collisionRayLenght, 3);

        _isCollidingRightUpper = Physics.Raycast(RightUpperRay, out _rightUpperColHit,
            _characterBounds.extents.x + _collisionRayLenght, 3);

        _isCollidingLeftLower = Physics.Raycast(LeftLowerRay, out _leftLowerColHit,
            _characterBounds.extents.x + _collisionRayLenght, 3);

        _isCollidingRightLower = Physics.Raycast(RightLowerRay, out _rightLowerColHit,
            _characterBounds.extents.x + _collisionRayLenght, 3);
    }

    private void CollisionCorrections()
    {
        _targetTranformPosition = transform.position;

        bool lerpThisFrame = Application.isPlaying && _collisionCorrectionStyle == CollisionCorrectionStyle.Lerp;

        _targetTranformPosition = lerpThisFrame ? Vector3.Lerp(_targetTranformPosition,
            _targetTranformPosition + new Vector3(0, _characterBounds.extents.y - _downColHit.distance),
            Time.deltaTime * _collisionCorrectionSpeed) : _targetTranformPosition += new Vector3(0,
            _characterBounds.extents.y - _downColHit.distance);

        if (_isCollidingDownMain && _downColHit.distance < _characterBounds.extents.y)
        {
            _targetTranformPosition = lerpThisFrame ? Vector3.Lerp(
                _targetTranformPosition,
                _targetTranformPosition += new Vector3(0,
                _characterBounds.extents.y - _downColHit.distance),
                Time.deltaTime * _collisionCorrectionSpeed) :
                _targetTranformPosition += new Vector3(0, _characterBounds.extents.y - _downColHit.distance);
        }
        else if (secondaryCollisionCorrection && (_isCollidingDownLeft || _isCollidingDownRight))
        {
            if (_isCollidingDownLeft && _downLeftColHit.distance < _characterBounds.extents.y)
            {
                _targetTranformPosition = lerpThisFrame ? Vector3.Lerp(
                    _targetTranformPosition,
                    _targetTranformPosition += new Vector3(0,
                    _characterBounds.extents.y - _downLeftColHit.distance),
                    Time.deltaTime * _collisionCorrectionSpeed) :
                    _targetTranformPosition += new Vector3(0,
                    _characterBounds.extents.y - _downLeftColHit.distance);
            }
            else if (_isCollidingDownRight && _downRightColHit.distance < _characterBounds.extents.y)
            {
                _targetTranformPosition = lerpThisFrame ? Vector3.Lerp(
                    _targetTranformPosition,
                    _targetTranformPosition += new Vector3(0,
                    _characterBounds.extents.y - _downRightColHit.distance),
                    Time.deltaTime * _collisionCorrectionSpeed) :
                    _targetTranformPosition += new Vector3(0,
                    _characterBounds.extents.y - _downRightColHit.distance);
            }
        }
    }

    private void Update()
    {
        RaycastForCollisions();
        CollisionCorrections();
    }
}
