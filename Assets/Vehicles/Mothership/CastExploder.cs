using UnityEngine;

public class CastExploder : MonoBehaviour
{
    public bool IsExploding = false;

    public GameObject ProjectExplosionPrefab;
    public Transform ProjectExplosionVertex1;
    public Transform ProjectExplosionVertex2;
    public float ProjectExplosionRadius;

    public float DestructTime = 30f;

    public delegate void OnCastExploderDestruct();
    public OnCastExploderDestruct OnDestruct;

    private bool _isExploding;
    private float _explodeCooldown;
    private float _totalDestructCooldown;

    private void Awake()
    {
        if (IsExploding)
            Trigger();
    }

    private void Update()
    {
        if (_isExploding)
        {
            var fraction = 0f;
            if (_totalDestructCooldown >=0f)
            {
                _totalDestructCooldown -= Time.deltaTime;
                fraction = Mathf.Clamp01(_totalDestructCooldown / DestructTime);
                if (_totalDestructCooldown < 0f)
                {
                    _isExploding = false;
                    if (OnDestruct != null)
                        OnDestruct();
                }
            }
            if (_explodeCooldown >= 0f)
            {
                _explodeCooldown -= Time.deltaTime;
                if (_explodeCooldown < 0f)
                {
                    ProjectedExplosion();
                    _explodeCooldown = Random.Range(0.05f * fraction, 0.1f * fraction);
                }
            }
        }
    }

    private void ProjectedExplosion()
    {
        var castFrom = transform.position + Random.onUnitSphere * ProjectExplosionRadius;
        var castTo = Vector3.Lerp(ProjectExplosionVertex1.position, ProjectExplosionVertex2.position, Random.value);
        var cast = new Ray(castFrom, castTo - castFrom);
        RaycastHit castHit;
        Debug.DrawLine(castFrom, castTo, Color.yellow, 1f);
        if (Physics.Raycast(cast, out castHit, ProjectExplosionRadius, LayerMask.GetMask("ExplodeCast")))
        {
            var explosion = ResourcePoolManager.GetAvailable(ProjectExplosionPrefab, castHit.point - castHit.normal, Quaternion.identity);
            var shakeSource = explosion.GetComponent<ScreenShakeSource>();
            if (shakeSource != null)
                shakeSource.Trigger();
            SplashDamage.ExplodeAt(castHit.point, 100f, 40f, 200f, 200f, LayerMask.GetMask("Detectable"), null);
        }
    }

    public void Trigger()
    {
        _isExploding = true;
        _totalDestructCooldown = DestructTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ProjectExplosionRadius);
    }
}
