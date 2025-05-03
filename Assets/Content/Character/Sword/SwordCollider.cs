using System;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    private SwordController controller;

    private void Awake()
    {
        controller = GetComponentInParent<SwordController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        controller.Contact();
    }
}
