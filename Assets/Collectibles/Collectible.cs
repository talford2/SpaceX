﻿using UnityEngine;

public class Collectible : MonoBehaviour
{
    public AudioClip SoundClip;
    public float RotateSpeed;

    private bool isCollected;
    private Transform collectorTransform;
    private Shiftable shiftable;
    private Vector3 velocity;
    private Vector3 rotateSpeed;

    private float followAcceleration = 250f;
    private float followSpeed;
    private Vector3 lastTo;

    public Shiftable Shiftable { get { return shiftable; } }

    private void Awake()
    {
        shiftable = GetComponent<Shiftable>();
        rotateSpeed = Random.insideUnitSphere*RotateSpeed;
    }

    public void Collect(GameObject collector)
    {
        if (collector.transform == PlayerController.Current.VehicleInstance.transform)
        {
            isCollected = true;
            collectorTransform = collector.transform;
        }
    }

    private void Update()
    {
        if (isCollected && collectorTransform != null)
        {
            var toCollector = transform.position - collectorTransform.position;
            followSpeed += followAcceleration * Time.deltaTime;
            velocity -= followSpeed * Time.deltaTime * toCollector.normalized;

            toCollector = transform.position - collectorTransform.position;
            var dotProd = Vector3.Dot(lastTo, toCollector);
            if (dotProd < 0f)
            {
                Debug.Log("COLLECTED!");
                if (SoundClip != null)
                    AudioSource.PlayClipAtPoint(SoundClip, transform.position);
                if (PlayerController.Current != null)
                {
                    PlayerController.Current.SpaceJunkCount++;
                }
                Destroy(gameObject);
            }
            lastTo = toCollector;
        }
        transform.rotation*= Quaternion.Euler(rotateSpeed*Time.deltaTime);
        var displacement = velocity*Time.deltaTime;
        shiftable.Translate(displacement);
    }

    public void SetVelocity(Vector3 value)
    {
        velocity = value;
    }
}
