﻿using UnityEngine;

public class Detectable : MonoBehaviour
{
    public Transform TargetTransform;
    public float Radius;

    private void Awake()
    {
        var sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
            Radius = sphereCollider.radius;
    }
}
