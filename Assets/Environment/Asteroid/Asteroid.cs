using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public GameObject DieEffectPrefab;

    private Killable _killable;
    private GameObject _effectInstance;

    private void Awake()
    {
        _killable = GetComponent<Killable>();
        _killable.OnDie += OnDie;
    }

    private void OnDie(Killable sender, Vector3 position, Vector3 normal, GameObject attacker)
    {
        Debug.Log("DESTROYASTEROID!");
        PlaceEffect(transform.position, normal, null);
        Destroy(gameObject);
    }

    public void PlaceEffect(Vector3 position, Vector3 normal, Transform parent)
    {
        if (DieEffectPrefab != null)
        {
            _effectInstance = ResourcePoolManager.GetAvailable(DieEffectPrefab, position, transform.rotation);

            var hitEffectShiftable = _effectInstance.GetComponent<Shiftable>();
            if (hitEffectShiftable != null)
            {
                hitEffectShiftable.SetShiftPosition(Universe.Current.GetUniversePosition(position));
            }

            _effectInstance.transform.position = position;
            _effectInstance.transform.forward = normal;
        }
    }
}
