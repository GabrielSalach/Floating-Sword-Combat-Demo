using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordController : MonoBehaviour
{
    [Header("Player Tracking")]
    public Transform player;
    public Vector3 offset;
    
    private Animator _animator;
    [Header("Idle Animation")]
    public AnimationClip _idleClip;
    public float bobbingSpeed;
    public float bobbingAmplitude;
    [Header("Swing Animation")]
    public AnimationClip _swingClip;
    public float swingSpeed;
    public float swingCooldown;
    public Vector3 swingOffset;
    public float swingAngle;

    [SerializeField]
    private List<Transform> _targets;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        CreateIdleClip();
    }

    private void FixedUpdate()
    {
        Vector3 newPosition = new Vector3(player.position.x, 0, player.position.z) + offset;
        transform.position = RotatePointAroundPivot(newPosition, player.position, player.eulerAngles);
    }
    
    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Start");
            LockMode();
        } else if (context.canceled)
        {
            Debug.Log("End");
            ReleaseMode();
        }
    }

    private void LockMode()
    {
        
    }

    private void ReleaseMode()
    {
        _swingClip.ClearCurves();

        int index = 0;
        AnimationCurve curveMovementX = new AnimationCurve();
        AnimationCurve curveMovementY = new AnimationCurve();
        AnimationCurve curveMovementZ = new AnimationCurve();
        
        AnimationCurve curveRotationX = new AnimationCurve();
        AnimationCurve curveRotationY = new AnimationCurve();
        AnimationCurve curveRotationZ = new AnimationCurve();
        AnimationCurve curveRotationW = new AnimationCurve();
        
        foreach (Transform target in _targets)
        {
            // Key times
            float keyStart = index * swingCooldown;
            float keySwing = keyStart + 0.01f;
            
            // Compute position and rotation values
            Vector3 startPosition = target.localPosition + swingOffset;
            
            Vector3 endPosition = new Vector3(
                target.localPosition.x - swingOffset.x,
                target.localPosition.y + swingOffset.y,
                target.localPosition.z + swingOffset.z
            );
            Debug.Log($"start : {target.localPosition} {swingOffset} {startPosition}");

            Quaternion startQuaternion = Quaternion.Euler(-90, swingAngle, 0);
            Quaternion endQuaternion = Quaternion.Euler(-90, -swingAngle, 0);
            
            // Creating movement keyframes
            curveMovementX.AddKey(new Keyframe(keyStart, startPosition.x, 0, 0));
            curveMovementX.AddKey(new Keyframe(keySwing, endPosition.x, 0, 0));
            
            curveMovementY.AddKey(new Keyframe(keyStart, startPosition.y, 0, 0));
            curveMovementY.AddKey(new Keyframe(keySwing, endPosition.y, 0, 0));
            
            curveMovementZ.AddKey(new Keyframe(keyStart, startPosition.z, 0, 0));
            curveMovementZ.AddKey(new Keyframe(keySwing, endPosition.z, 0, 0));
            
            // Creating rotation keyframes
            curveRotationX.AddKey(new Keyframe(keyStart, startQuaternion.x, 0, 0));
            curveRotationX.AddKey(new Keyframe(keySwing, endQuaternion.x, 0, 0));
            
            curveRotationY.AddKey(new Keyframe(keyStart, startQuaternion.y, 0, 0));
            curveRotationY.AddKey(new Keyframe(keySwing, endQuaternion.y, 0, 0));
            
            curveRotationZ.AddKey(new Keyframe(keyStart, startQuaternion.z, 0, 0));
            curveRotationZ.AddKey(new Keyframe(keySwing, endQuaternion.z, 0, 0));
            
            curveRotationW.AddKey(new Keyframe(keyStart, startQuaternion.w, 0, 0));
            curveRotationW.AddKey(new Keyframe(keySwing, endQuaternion.w, 0, 0));

            // Setting movement curves
            _swingClip.SetCurve(
                transform.GetChild(0).gameObject.name,
                typeof(Transform),
                "m_Position.x",
                curveMovementX
            );
            _swingClip.SetCurve(
                transform.GetChild(0).gameObject.name,
                typeof(Transform),
                "m_Position.y",
                curveMovementY
            );
            _swingClip.SetCurve(
                transform.GetChild(0).gameObject.name,
                typeof(Transform),
                "m_Position.z",
                curveMovementZ
            );
            
            // Setting rotation curves
            _swingClip.SetCurve(
                transform.GetChild(0).gameObject.name,
                typeof(Transform),
                "m_Rotation.x",
                curveRotationX
            );
            _swingClip.SetCurve(
                transform.GetChild(0).gameObject.name,
                typeof(Transform),
                "m_Rotation.y",
                curveRotationY
            );
            _swingClip.SetCurve(
                transform.GetChild(0).gameObject.name,
                typeof(Transform),
                "m_Rotation.z",
                curveRotationZ
            );
            _swingClip.SetCurve(
                transform.GetChild(0).gameObject.name,
                typeof(Transform),
                "m_Rotation.w",
                curveRotationW
            );
            
            index++;
        }
        _animator.SetFloat("Blend", 1);
    }

    private void CreateIdleClip()
    {
        _idleClip.ClearCurves();
        Keyframe[] keys = new Keyframe[3];
        keys[0] = new Keyframe(0, 0, 0, 0);
        keys[1] = new Keyframe(bobbingSpeed/2, bobbingAmplitude, 0, 0);
        keys[2] = new Keyframe(bobbingSpeed, 0, 0, 0);
        AnimationCurve bobbingCurve = new AnimationCurve(keys);

        _idleClip.SetCurve(
            transform.GetChild(0).gameObject.name,
            typeof(Transform),
            "m_LocalPosition.y",
            bobbingCurve
        );
    }
    
}