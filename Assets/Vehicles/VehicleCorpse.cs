using UnityEngine;
using System.Collections;

public class VehicleCorpse : MonoBehaviour
{
    public GameObject ExplosionPrefab;
    public GameObject DebrisPrefab;

    private Rigidbody rBody;

    private void Awake()
    {
        rBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartCoroutine(DelayedExplode(Random.Range(2f, 3f)));
    }

    private IEnumerator DelayedExplode(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (ExplosionPrefab != null)
        {
            var explodeInstance = ResourcePoolManager.GetAvailable(ExplosionPrefab, rBody.position, rBody.rotation);
            explodeInstance.transform.localScale = transform.localScale;
            var explodeShiftable = explodeInstance.GetComponent<Shiftable>();

            if (explodeShiftable != null)
                explodeShiftable.SetShiftPosition(Universe.Current.GetUniversePosition(rBody.position));
        }

        if (DebrisPrefab != null)
        {
            var debrisInstance = (GameObject)Instantiate(DebrisPrefab, rBody.position, rBody.rotation);
            var rBodies = debrisInstance.GetComponentsInChildren<Rigidbody>();
            foreach (var debrisrBody in rBodies)
            {
                debrisrBody.velocity = rBody.velocity;
                debrisrBody.AddExplosionForce(50f, transform.position, 20f, 0f, ForceMode.Impulse);
                var debrisShiftable = debrisrBody.GetComponent<Shiftable>();
                debrisShiftable.SetShiftPosition(Universe.Current.GetUniversePosition(debrisrBody.position));
            }
        }

        Destroy(gameObject);
    }
}
