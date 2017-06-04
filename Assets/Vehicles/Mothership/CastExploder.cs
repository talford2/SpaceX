using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastExploder : MonoBehaviour
{
    public bool IsExploding = false;

    public GameObject ProjectExplosionPrefab;
    public Transform ProjectExplosionVertex1;
    public Transform ProjectExplosionVertex2;
    public float ProjectExplosionRadius;

    private bool _isExploding;
    private float _explodeCooldown;

    private void Awake()
    {
        _isExploding = IsExploding;
    }

    private void Update()
    {
        if (_isExploding)
        {
            if (_explodeCooldown >= 0f)
            {
                _explodeCooldown -= Time.deltaTime;
                if (_explodeCooldown < 0f)
                {
                    Debug.Log("TRIGGGER BOOM!");
                    ProjectedExplosion();
                    _explodeCooldown = Random.Range(0.005f, 0.01f);
                }
            }
        }
    }

    private void ProjectedExplosion()
    {
        var castFrom = transform.position + Random.onUnitSphere * ProjectExplosionRadius;
        var castDelta = ProjectExplosionVertex2.position - ProjectExplosionVertex1.position;
        var castTo = ProjectExplosionVertex1.position + Random.value * castDelta.magnitude * castDelta.normalized;
        var cast = new Ray(castFrom, castTo - castFrom);
        RaycastHit castHit;

        Debug.DrawLine(castFrom, castTo, Color.yellow, 1f);
        if (Physics.Raycast(cast, out castHit, ProjectExplosionRadius, LayerMask.GetMask("ExplodeCast", "Distant")))
        {
            Debug.Log("EXPLODE AT: " + castHit.point);
            var explosion = ResourcePoolManager.GetAvailable(ProjectExplosionPrefab, castHit.point - castHit.normal, Quaternion.identity);
            var shakeSource = explosion.GetComponent<ScreenShakeSource>();
            if (shakeSource != null)
                shakeSource.Trigger();
            SplashDamage.ExplodeAt(castHit.point, 100f, 40f, 200f, 200f, LayerMask.GetMask("Detectable"), null);
        }
        else
        {
            Debug.Log("NO CAST HIT");
        }
    }

    public void Trigger()
    {
        _isExploding = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ProjectExplosionRadius);
    }
}
