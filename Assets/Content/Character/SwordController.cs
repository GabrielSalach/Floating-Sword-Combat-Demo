using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordController : MonoBehaviour
{
    public Transform player;
    public float bobbingSpeed;
    public float bobbingAmplitude;
    public Vector3 offset;

    [SerializeField]
    private List<Transform> _targets;
    
    private void FixedUpdate()
    {
        float bobbingValue = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmplitude;
        Vector3 newPosition = new Vector3(player.position.x, bobbingValue, player.position.z) + offset;
        transform.position = RotatePointAroundPivot(newPosition, player.position, player.eulerAngles);
    }
    
    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if(context.started)
            LockMode();
        else if(context.performed || context.canceled)
            ReleaseMode();
    }

    private void LockMode()
    {
        
    }

    private void ReleaseMode()
    {
        //Calculate Path
            // Each path 
    }
    
}