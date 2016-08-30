using UnityEngine;
using System.Collections.Generic;

public class ShipGenerator : MonoBehaviour
{
    public bool GenerateOnAwake;
    public Transform NearParent;
    public Transform DistantParent;

    private static ShipGenerator _current;

    public static ShipGenerator Current { get { return _current; } }

    private Dictionary<ShipPartType, List<GameObject>> partPool;
    private Transform generatedParent;

    private Dictionary<ShipPartType, List<GameObject>> parts;

    private void Awake()
    {
        _current = this;

        parts = new Dictionary<ShipPartType, List<GameObject>>();
        parts.Add(ShipPartType.Front, LoadParts("Front"));
        parts.Add(ShipPartType.Centre, LoadParts("Centre"));
        parts.Add(ShipPartType.Back, LoadParts("Back"));

        if (GenerateOnAwake)
            Generate();
    }

    private List<GameObject> LoadParts(string folder)
    {
        var loadParts = Resources.LoadAll<GameObject>(string.Format("CapitalShips/Parts/{0}", folder));
        var parts = new List<GameObject>();
        foreach (var part in loadParts)
        {
            Debug.LogFormat("{0} loaded.", part.name);
            parts.Add(part);
        }
        return parts;
    }

    private GameObject GetRandomPart(ShipPartType type)
    {
        var typedParts = parts[type];
        return typedParts[Random.Range(0, typedParts.Count)];
    }

    private void BuildPartPool()
    {
        partPool = new Dictionary<ShipPartType, List<GameObject>>();
        var frontPart = GetRandomPart(ShipPartType.Front);
        var backPart = GetRandomPart(ShipPartType.Back);
        partPool.Add(ShipPartType.Front, new List<GameObject> { frontPart });
        partPool.Add(ShipPartType.Back, new List<GameObject> { backPart });
        partPool.Add(ShipPartType.Centre, new List<GameObject>());
        for (var i = 0; i < 10; i++)
        {
            partPool[ShipPartType.Centre].Add(GetRandomPart(ShipPartType.Centre));
        }
    }

    private GameObject GetFromPartPool(ShipPartType type)
    {
        if (!partPool.ContainsKey(type) || partPool[type].Count == 0)
            return null;
        var typedParts = partPool[type];
        var part = typedParts[Random.Range(0, typedParts.Count)];
        typedParts.Remove(part);
        return part;
    }

    private GameObject GetFromPartPool(List<ShipPartType> types)
    {
        var useType = types[Random.Range(0, types.Count)];
        if (!partPool.ContainsKey(useType) || partPool[useType].Count == 0)
        {
            for (var i = 0; i < types.Count; i++)
            {
                if (partPool.ContainsKey(types[i]) && partPool[types[i]].Count > 0)
                {
                    useType = types[i];
                    break;
                }
            }
        }
        if (!partPool.ContainsKey(useType) || partPool[useType].Count == 0)
            return null;
        return GetFromPartPool(useType);
    }

    public void Generate()
    {
        if (generatedParent != null)
            Destroy(generatedParent.gameObject);
        generatedParent = new GameObject("Generated").transform;

        BuildPartPool();

        var firstPart = ((GameObject)Instantiate(GetFromPartPool(ShipPartType.Front), Vector3.zero, Quaternion.identity)).GetComponent<ShipPart>();
        firstPart.transform.parent = generatedParent.transform;

        var partInstances = new List<ShipPart>();
        partInstances.Add(firstPart);

        // Attach new parts to connectors
        var availableConnectors = new Stack<ShipPartConnector>();

        foreach (var connector in firstPart.Connectors)
        {
            availableConnectors.Push(connector);
        }

        while (availableConnectors.Count > 0)
        {
            var popped = availableConnectors.Pop();
            if (popped != null)
            {
                var choosePart = GetFromPartPool(popped.AllowTypes);
                if (choosePart != null)
                {
                    var addPart = ((GameObject)Instantiate(choosePart, Vector3.zero, Quaternion.identity)).GetComponent<ShipPart>();
                    partInstances.Add(addPart);
                    var addPartConnector = addPart.Connectors[Random.Range(0, addPart.Connectors.Count)];
                    foreach (var connector in addPart.Connectors)
                    {
                        if (connector != addPartConnector)
                            availableConnectors.Push(connector);
                    }

                    addPart.transform.rotation = Quaternion.FromToRotation(popped.transform.forward, -addPartConnector.transform.forward);

                    addPart.transform.position = popped.transform.position - addPartConnector.transform.position;
                    addPart.transform.parent = generatedParent.transform;
                }
            }
        }

        var positionSum = Vector3.zero;
        foreach (var partInstance in partInstances)
        {
            positionSum += partInstance.transform.position;
        }
        var offset = positionSum / partInstances.Count;
        foreach (var partInstance in partInstances)
        {
            partInstance.transform.localPosition -= offset;
        }
        //generatedParent.position -= positionSum / partInstances.Count;

        var nearInstance = Instantiate(generatedParent.gameObject);
        nearInstance.transform.parent = NearParent;
        nearInstance.transform.localPosition = Vector3.zero;

        var distantInstance = Instantiate(generatedParent.gameObject);
        distantInstance.transform.parent = DistantParent;
        distantInstance.transform.localPosition = Vector3.zero;
        Utility.SetLayerRecursively(distantInstance, LayerMask.NameToLayer("Distant"));

        Destroy(generatedParent.gameObject);
    }
}