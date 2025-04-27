using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordController : MonoBehaviour
{
    [Header("Player Tracking")]
    public Transform player;
    public Vector3 offset;
    
    [SerializeField]
    private List<Transform> _targets;

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
            //ReleaseMode();
        }
    }

    private void LockMode()
    {
        
    }
}