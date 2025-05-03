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
    public float swordSpeed = 1;
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
        _swordModel.transform.DORotate(new Vector3(90, 90, -90), slashDelay);

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[0] is null)
                continue;
            
            Vector3 startPos;
            Vector3 endPos;
            
            // Computing slash direction and positions
            Vector3 direction = (transform.position - enemies[i].position).normalized;
            Vector3 slashOffset = Vector3.Cross(direction, Vector3.up) * 1f; // Décalage latéral
            
            if (i % 2 == 0)
            {
                startPos = enemies[i].position + slashOffset + Vector3.up * (enemies[i].transform.localScale.y/4);
                endPos = enemies[i].position - slashOffset - Vector3.up * (enemies[i].transform.localScale.y/4);
            }
            else
            {
                startPos = enemies[i].position - slashOffset - Vector3.up * (enemies[i].transform.localScale.y/4);
                endPos = enemies[i].position + slashOffset + Vector3.up * (enemies[i].transform.localScale.y/4);
            }

            Sequence seq = DOTween.Sequence();
            
            // Move towards enemy
            transform.DOLookAt(enemies[i].position, slashDelay);
            seq.Append(transform.DOMove(startPos, i == 0 ? swordSpeed : slashDelay).SetEase(Ease.OutBack));
            
            // Slash
            seq.Append(transform.DOMove(endPos, slashSpeed));
            
            seq.Play();

            yield return seq.WaitForCompletion();
        }
        
        // Return to player 
        transform.DOMove(player.position + offset, swordSpeed);
        transform.DORotate(Vector3.zero, slashDelay);
        _swordModel.transform.DORotate(Vector3.zero, slashDelay);
        
        _isSlashing = false;
    }
}