using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class LockMode : MonoBehaviour
{
    private bool _isCapturing;
    private Dictionary<Transform, GameObject> _capturedTargets;
    private Camera _camera;
    public GameObject markerPrefab;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(_camera.transform.forward + transform.parent.position);
        transform.Rotate(Vector3.right * 90);
    }

    public void StartCapture(int maxTargets)
    {
        _capturedTargets = new Dictionary<Transform, GameObject>();
        _isCapturing = true;
    }

    public List<Transform> StopCapture()
    {
        List<Transform> returnTargets = _capturedTargets.Keys.ToList();
        returnTargets.Sort(comparison: (o, o1) =>
            Vector3.Distance(transform.position, o.transform.position)
                .CompareTo(Vector3.Distance(transform.position, o1.transform.position)));
        
        return returnTargets;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!_isCapturing && !other.CompareTag(Tags.EnemyTag)) return;
        if (_capturedTargets.ContainsKey(other.transform)) return;
        Debug.Log("Coucou");
        
        EnemyMarker marker = Instantiate(markerPrefab).GetComponent<EnemyMarker>();
        marker.SetTarget(other.transform, _camera);
        _capturedTargets.Add(other.transform, other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isCapturing && !other.CompareTag(Tags.EnemyTag)) return;
        if (_capturedTargets.ContainsKey(other.transform)) return;
        Debug.Log("byebye");
        
        Destroy(_capturedTargets[other.transform]);
        _capturedTargets.Remove(other.transform);
    }
}
