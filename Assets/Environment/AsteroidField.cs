using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidField : MonoBehaviour
{
    public int AsteroidPoolCount = 1000;

    public Transform CentreTransform;

    public float FieldRadius = 300f;

    public GameObject AsteroidPrefab;

    private List<GameObject> _asteroids;

    private void Awake()
    {
        CentreTransform = PlayerController.Current.transform;
        _asteroids = new List<GameObject>();
        for (var i = 0; i < AsteroidPoolCount; i++)
        {
            var inst = Instantiate(AsteroidPrefab);
            inst.transform.position = Random.insideUnitSphere*FieldRadius + CentreTransform.position;
            inst.transform.rotation = Random.rotation;
            inst.transform.localScale = Vector3.one*Random.Range(0.3f, 2.3f);

            var levelOfDetail = inst.GetComponent<LevelOfDetail>();
            levelOfDetail.OnExitLargestDistance += MakeAvailable;
            levelOfDetail.OnEnterLargestDistance += MakeUnavailable;
        }
    }

    private void Update()
    {
        foreach (var asteroid in _asteroids)
        {
            // if (asteroid is available) {
            // position the asteroid on sphone in front of camera
            // enable asteroid
            // }
            // if (asteroid is unavailable) {
            // disable asteroid
            // }
        }
    }

    private void MakeAvailable(GameObject instance)
    {
    }

    private void MakeUnavailable(GameObject instance)
    {
    }
}
