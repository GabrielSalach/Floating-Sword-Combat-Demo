using System.Collections;
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
    
    public float speed = 20f;
    public float stopDistance = 0.5f;
    public float slashDelay = 0.2f;

    private bool isSlashing = false;

    private void Start()
    {
        Time.timeScale = 0.3f;
        Time.fixedDeltaTime = Time.timeScale * 0.3f;
    }
    
    private void Update()
    {
        if (isSlashing == false)
        {
            transform.position = player.position + offset;
        }
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
            isSlashing = true;
            StartCoroutine(ExecuteSlashes(_targets));
        }
    }

    private void LockMode()
    {
        
    }
    
    private IEnumerator ReleaseMode() 
    {
        yield return null;
    }
    
    public IEnumerator ExecuteSlashes(List<Transform> enemies)
    {
        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;

            while (Vector3.Distance(transform.position, enemy.position) > stopDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, enemy.position, speed * Time.deltaTime);
                transform.LookAt(enemy);
                yield return null;
            }

            // Déclenchement de l'animation procédurale
            StartCoroutine(SlashAnimation(enemy));

            yield return new WaitForSeconds(slashDelay);
        }

        // Retour à la position idle
        while (Vector3.Distance(transform.position, player.position + offset) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position + offset, speed * Time.deltaTime);
            transform.LookAt(player.position + offset);
            yield return null;
        }

        isSlashing = false;
    }
    IEnumerator SlashAnimation(Transform enemy)
    {
        Vector3 startPos = transform.position;
        Vector3 direction = (transform.position - enemy.position).normalized;
        Vector3 slashOffset = Vector3.Cross(direction, Vector3.up) * 1f; // Décalage latéral

        Vector3 midPos = enemy.position + slashOffset;
        Vector3 endPos = enemy.position - slashOffset;

        float duration = 0.1f;
        float elapsed = 0f;

        // Slash de gauche à droite
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(midPos, endPos, elapsed / duration);
            transform.LookAt(enemy);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        // Tu peux ajouter ici un effet de particules, son, etc.
    }
}