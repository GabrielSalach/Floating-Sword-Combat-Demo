using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
    public float slashDuration = 0.5f;
    public float slashCooldown = 0.3f;
    public AnimationCurve SwordAnimationCurve;

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
        _swordModel.transform.DORotate(new Vector3(90, 90, -90), slashDuration);

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[0] is null)
                continue;

            Vector3 startSlashPos, startSlashIn, startSlashOut;
            Vector3 endSlashPos, endSlashIn, endSlashOut;
            
            // Computing slash direction and positions
            Vector3 slashDirection = (transform.position - enemies[i].position).normalized;
            Vector3 slashOffset = Vector3.Cross(slashDirection, Vector3.up) * 1f; // Décalage latéral
            
            if (i % 2 == 0)
            {
                startSlashPos = enemies[i].position + slashOffset + Vector3.up * (enemies[i].transform.localScale.y/4);
                endSlashPos = enemies[i].position - slashOffset - Vector3.up * (enemies[i].transform.localScale.y/4);
            }
            else
            {
                startSlashPos = enemies[i].position - slashOffset + Vector3.up * (enemies[i].transform.localScale.y/4);
                endSlashPos = enemies[i].position + slashOffset - Vector3.up * (enemies[i].transform.localScale.y/4);
            }
            
            // Computing Bezier control points
            Vector3 firstVec = startSlashPos - transform.position;
            Vector3 secondVec = startSlashPos - endSlashPos;
            Vector3 projection = Vector3.Dot(secondVec, firstVec) / firstVec.sqrMagnitude * firstVec;
            Vector3 controlDirection = (secondVec - projection).normalized;
            startSlashIn = Vector3.Lerp(transform.position, startSlashPos, 0.3f) + controlDirection;
            startSlashOut = Vector3.Lerp(transform.position, startSlashPos, 0.7f) + controlDirection;
            endSlashIn = Vector3.Lerp(transform.position, startSlashPos, 0.3f);
            endSlashOut = Vector3.Lerp(transform.position, startSlashPos, 0.7f);
            
            

            
            // Move towards enemy
            transform.DOLookAt(enemies[i].position, slashDuration);
            // seq.Append(transform.DOMove(startSlashPos, slashDelay));
            Tweener tween = transform
                .DOPath(
                    new Vector3[] { startSlashPos, startSlashIn, startSlashOut, endSlashPos, endSlashIn, endSlashOut },
                    slashDuration, PathType.CubicBezier)
                .SetEase(SwordAnimationCurve)
                .OnWaypointChange(_value =>
                {
                    if (_value != 3) return;
                    
                    _audioSource.pitch = Random.Range(0.7f, 1.3f);
                    _audioSource.Play();
                })
                .SetDelay(slashCooldown);
            
            // Slash
            // seq.Append(
            //     transform.DOMove(endSlashPos, slashSpeed)
            //     .SetEase(Ease.OutBack)
            //     .SetDelay(slashDelay)
            //     .OnStart(() =>
            //     {
            //         _audioSource.pitch = Random.Range(0.7f, 1.3f);
            //         _audioSource.Play();
            //     })
            // );
            yield return tween.WaitForCompletion();
        }
        
        // Return to player 
        Tweener tweener = transform.DOMove(player.position + offset, swordSpeed);
        // tweener.OnUpdate(() =>
        // {
        //     if (!(Vector3.Distance(transform.position, player.position + offset) > 0.5f)) return;
        //     
        //     Vector3 newPosition = new Vector3(player.position.x, 0, player.position.z) + offset;
        //     newPosition = RotatePointAroundPivot(newPosition, player.position, player.eulerAngles);
        //     tweener.ChangeEndValue(newPosition, false);
        // });

        transform.DORotate(Vector3.zero, slashDuration);
        _swordModel.transform.DORotate(Vector3.zero, slashDuration);
        
        _isSlashing = false;
    }
}