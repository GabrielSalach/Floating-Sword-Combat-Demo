using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordController : MonoBehaviour
{
    [Header("Player Tracking")]
    public Transform player;
    public Vector3 offset;
    [Header("Targets tracking")]
    [SerializeField]
    private List<Transform> targets;

    [Header("Animation")] 
    public float slashDelay = 0.2f;
    public float slashSpeed = 0.1f;

    private bool _isSlashing;
    private GameObject _swordModel;

    private void Awake()
    {
        _swordModel = GetComponentInChildren<MeshRenderer>().gameObject;
    }
    
    private void Update()
    {
        if (_isSlashing == false)
        {
            Vector3 newPosition = new Vector3(player.position.x, 0, player.position.z) + offset;
            transform.position = RotatePointAroundPivot(newPosition, player.position, player.eulerAngles);
            transform.rotation =  player.rotation;
        }
    }
    
    private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            LockMode();
        } else if (context.canceled)
        {
            StartCoroutine(ExecuteSlashes(targets));
        }
    }

    private static void LockMode()
    {
        
    }

    private IEnumerator ExecuteSlashes(List<Transform> enemies)
    {
        _isSlashing = true;
        _swordModel.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        
        foreach (Transform enemy in enemies.Where(enemy => enemy is not null))
        {
            // Computing slash direction and positions
            Vector3 direction = (transform.position - enemy.position).normalized;
            Vector3 slashOffset = Vector3.Cross(direction, Vector3.up) * 1f; // Décalage latéral

            Vector3 startPos = enemy.position + slashOffset;
            Vector3 endPos = enemy.position - slashOffset;


            // Move towards enemy
            transform.DOMove(startPos, slashDelay).DOTimeScale(0.1f, 0);
            transform.DOLookAt(enemy.position, slashDelay);
            
            // Slash
            transform.DOMove(endPos, slashSpeed).DOTimeScale(0.1f, 0);

            yield return new WaitForSeconds(slashDelay);
        }
        
        // Return to player 
        transform.DOMove(player.position + offset, slashDelay);
        _swordModel.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        
        _isSlashing = false;
    }
}