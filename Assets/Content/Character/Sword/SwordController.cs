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
    private AudioSource _audioSource;

    private void Awake()
    {
        _swordModel = GetComponentInChildren<MeshRenderer>().gameObject;
        _audioSource = GetComponent<AudioSource>();
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
            Vector3 arcPoint;
            
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
                startPos = enemies[i].position - slashOffset + Vector3.up * (enemies[i].transform.localScale.y/4);
                endPos = enemies[i].position + slashOffset - Vector3.up * (enemies[i].transform.localScale.y/4);
            }
            
            
            GameObject start = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            start.transform.position = startPos;
            start.transform.localScale = Vector3.one * 0.05f;
            start.GetComponent<MeshRenderer>().material.color = Color.red;
            GameObject end = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            end.transform.position = endPos;
            end.transform.localScale = Vector3.one * 0.05f;
            end.GetComponent<MeshRenderer>().material.color = Color.green;
            // GameObject arc = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // arc.transform.position = arcPoint;
            // arc.transform.localScale = Vector3.one * 0.05f;
            // arc.GetComponent<MeshRenderer>().material.color = Color.blue;

            Sequence seq = DOTween.Sequence();
            
            // Move towards enemy
            transform.DOLookAt(enemies[i].position, slashDelay);
            seq.Append(transform.DOMove(startPos, slashDelay));
            
            // Slash
            seq.Append(
                transform.DOMove(endPos, slashSpeed)
                .SetEase(Ease.OutBack)
                .SetDelay(slashDelay)
                .OnStart(() =>
                {
                    _audioSource.pitch = Random.Range(0.7f, 1.3f);
                    _audioSource.Play();
                })
            );
            
            seq.Play();

            yield return seq.WaitForCompletion();
        }
        
        // Return to player 
        Tweener tweener = transform.DOMove(player.position + offset, swordSpeed);
        tweener.OnUpdate(() =>
        {
            if (!(Vector3.Distance(transform.position, player.position + offset) > 0.5f)) return;
            
            Vector3 newPosition = new Vector3(player.position.x, 0, player.position.z) + offset;
            newPosition = RotatePointAroundPivot(newPosition, player.position, player.eulerAngles);
            tweener.ChangeEndValue(newPosition, false);
        });

        transform.DORotate(Vector3.zero, slashDelay);
        _swordModel.transform.DORotate(Vector3.zero, slashDelay);
        
        _isSlashing = false;
    }

    public void Contact()
    {
        Debug.Log("Contact");
    }
}