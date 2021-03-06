﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class UniverseGenerator : MonoBehaviour
{
    #region Private Members

    private GameObject _universeObj;

    private Transform _parent;

    private Camera _renderCamera;

    private bool _hasGenerated = false;

    private Material _mat;

    private Vector3 _sunDirection;

    private bool _hasStartedGenerating = false;

    private int _backgroundLayer;

    private int _backgroundLayerMask;

    #endregion

    #region Public Variables

    public int BackgroundLayer;

    public int Seed = 32;

    public LevelDefinition Level;

    public int FlatResolution = 2048;

    public Shader BaseShader;

    public Shader GradientShader;

    public Shader CubemapShader;

    public Color BackgroundColor = Color.black;

    public List<ScatterSettings> ScatterObjects;

    public bool HasGenerated { get { return _hasGenerated; } }

    // Sun
    public Light SunLight;

    public Texture SunTexture;

    public GameObject SunModel;

    public GameObject SunAttachPrefab;

    public event System.Action FinishedRendering;

    #endregion

    #region Public Properties

    public Material Background { get { return _mat; } }

    public Vector3 SunDirection { get { return _sunDirection; } }

    public float SunIntensity;

    public Color SunColour;

    #endregion

    public UniverseGenerator()
    {
        if (ScatterObjects == null)
        {
            ScatterObjects = new List<ScatterSettings>();
        }
    }

    private void Awake()
    {
        _backgroundLayer = BackgroundLayer;
        _backgroundLayerMask = LayerMask.GetMask(LayerMask.LayerToName(_backgroundLayer));
    }

    private void Start()
    {
        Random.InitState(Seed);
        if (ScatterObjects == null)
        {
            ScatterObjects = new List<ScatterSettings>();
        }

        SunLight.color = SunColour;
        //Generate();
        var s = GetMaterial();
    }

    private void LateUpdate()
    {
        if (_hasStartedGenerating)
        {
            Debug.Log("Done: " + gameObject.name);
            DestroyImmediate(_parent.gameObject);
            _hasGenerated = true;
            _hasStartedGenerating = false;
            if (FinishedRendering != null)
            {
                FinishedRendering();
            }
            // Save it to a file.
            Debug.Log("Start saving file..." + Application.dataPath);

            //var rrr = Random.Range(0, 999999);
            SaveCubemap(Path.Combine(Application.dataPath, @"Environment\Backgrounds\" + gameObject.name + ".png"), 4096);
            Debug.Log("Sun direction = " + SunDirection);
            Debug.Log(SunDirection);
            Debug.Log(SunLight.transform.forward);
            Debug.LogFormat("SUN DIR: ({0:f3}, {1:f3}, {2:f3})", SunDirection.x, SunDirection.y, SunDirection.z);

            Debug.LogFormat("Rotation: ({0:f4}, {1:f4}, {2:f4})",
                SunLight.transform.eulerAngles.x,
                SunLight.transform.eulerAngles.y,
                SunLight.transform.eulerAngles.z);

            /* Manipulate Scriptable Objects Directly! */
            Level.SystemName = NameGenerator.GetRandomSystemName(Seed);
            Level.LightColour = SunColour;
            Level.LightDirection = SunDirection;
            //EditorUtility.SetDirty(Level);

            Debug.DrawLine(Vector3.zero, SunDirection * 1000f, Color.magenta, 600);
        }
    }

    #region Public Methods

    public void Generate()
    {
        Destroy(_universeObj);
        _universeObj = new GameObject("UniverseObject");
        _parent = _universeObj.transform;

        // Construct Camera
        var camObj = new GameObject("BackgroundCamera");
        camObj.transform.SetParent(_parent);
        _renderCamera = camObj.AddComponent<Camera>();
        // VERY IMPORTANT - must set clear to color before creating universe, then set to skybox after clearing
        _renderCamera.clearFlags = CameraClearFlags.Color;
        _renderCamera.renderingPath = RenderingPath.DeferredShading;
        _renderCamera.hdr = true;
        _renderCamera.farClipPlane = 20000;
        _renderCamera.cullingMask = _backgroundLayerMask;
        _renderCamera.backgroundColor = BackgroundColor;

        // Sun
        var sunObj = Instantiate(SunModel);
        sunObj.transform.SetParent(_parent);
        sunObj.layer = _backgroundLayer;
        sunObj.transform.position = Random.onUnitSphere * 10000;
        sunObj.transform.localScale = Vector3.one * 2000;
        sunObj.GetComponent<Renderer>().material = CreateMaterial(SunTexture, Color.white);
        sunObj.transform.rotation = LookAtWithRandomTwist(sunObj.transform.position, Vector3.zero);
        SunLight.transform.position = sunObj.transform.position;
        SunLight.transform.forward = Vector3.zero - sunObj.transform.position;
        _sunDirection = (Vector3.zero - sunObj.transform.position).normalized;

        if (SunAttachPrefab != null)
        {
            var attachedPrefab = Instantiate<GameObject>(SunAttachPrefab);
            attachedPrefab.transform.SetParent(_parent);
            attachedPrefab.layer = _backgroundLayer;
            attachedPrefab.transform.position = Vector3.zero;
            attachedPrefab.transform.localScale = Vector3.one;// * 300f;
            attachedPrefab.transform.rotation = sunObj.transform.rotation;
        }

        foreach (var sg in ScatterObjects)
        {
            if (sg.IsActive)
            {
                Scatter(sg);
            }
        }

        Flatten();
        _hasGenerated = true;
    }

    public Material GetMaterial()
    {
        Debug.Log("Get material : " + gameObject.name);
        Generate();
        Flatten();
        _hasStartedGenerating = true;
        return _mat;
    }

    public void Flatten()
    {
        var renderTexture = new RenderTexture(FlatResolution, FlatResolution, 24);
        renderTexture.wrapMode = TextureWrapMode.Repeat;
        renderTexture.antiAliasing = 2;
        renderTexture.anisoLevel = 9;
        renderTexture.filterMode = FilterMode.Trilinear;
        renderTexture.autoGenerateMips = false;
        renderTexture.isCubemap = true;

        _mat = new Material(CubemapShader);
        _mat.SetTexture("_Tex", renderTexture);

        RenderSettings.skybox = _mat;

        _renderCamera.RenderToCubemap(renderTexture);

        _renderCamera.enabled = false;
    }

    #endregion

    #region Private Methods

    private Quaternion RandomEuler(float variation)
    {
        return Quaternion.Euler(Random.Range(-variation, variation), Random.Range(-variation, variation), Random.Range(-variation, variation));
    }

    private void Scatter(ScatterSettings settings)
    {
        var clusters = new List<Vector3>();
        if (settings.UseClustering)
        {
            for (var i = 0; i < Random.Range(settings.ClusterCountMin, settings.ClusterCountMax); i++)
            {
                clusters.Add(Random.onUnitSphere);
            }
        }

        for (var i = 0; i < Random.Range(settings.CountMin, settings.CountMax); i++)
        {
            var model = Instantiate<GameObject>(settings.Model);
            model.layer = _backgroundLayer;

            if (settings.RadiusMax == 0 && settings.RadiusMin == 0)
            {
                model.transform.position = Vector3.zero;
                model.transform.rotation = Random.rotation;
            }
            else
            {
                if (settings.UseClustering)
                {
                    var clusterCentre = clusters[Random.Range(0, clusters.Count)];
                    model.transform.position = RandomEuler(settings.ClusterScatter) * clusterCentre * Random.Range(settings.RadiusMin, settings.RadiusMax);
                }
                else
                {
                    model.transform.position = Random.onUnitSphere * Random.Range(settings.RadiusMin, settings.RadiusMax);
                }

                if (settings.LookAtCenter)
                {
                    model.transform.rotation = LookAtWithRandomTwist(model.transform.position, Vector3.zero);
                }
                else
                {
                    model.transform.rotation = Random.rotation;
                }
            }

            model.transform.localScale = Vector3.one * Random.Range(settings.ScaleMin, settings.ScaleMax);
            model.transform.SetParent(_parent);

            if (settings.UseMaterials)
            {
                model.GetComponent<Renderer>().material = settings.Materials[Random.Range(0, settings.Materials.Count)];
            }
            else
            {
                if (settings.Textures != null && settings.Textures.Any())
                {
                    var tex = settings.Textures[Random.Range(0, settings.Textures.Count)];
                    //var colr = settings.Colors[Random.Range(0, settings.Colors.Count)].GetRandom();
                    //model.GetComponent<Renderer>().material = CreateMaterial(tex, colr);

                    var r = settings.Colors[Random.Range(0, settings.Colors.Count)];
                    model.GetComponent<Renderer>().material = CreateGradientMaterial(tex, r.Color1, r.Color2);
                }
            }
        }
    }

    private Quaternion LookAtWithRandomTwist(Vector3 positon, Vector3 target)
    {
        var relativeForward = target - positon;
        var lookat = Quaternion.LookRotation(relativeForward, Random.onUnitSphere);

        // This isn't right yet
        //lookat = Quaternion.AngleAxis(Random.Range(0f, 360f), forwardS);

        return lookat;
    }

    private Material CreateMaterial(Texture tex, Color color)
    {
        var mat = new Material(BaseShader);
        mat.SetTexture("_MainTex", tex);
        mat.SetColor("_Color", color);
        return mat;
    }

    private Material CreateGradientMaterial(Texture tex, Color grey, Color white)
    {
        var mat = new Material(GradientShader);
        mat.SetTexture("_MainTex", tex);
        mat.SetColor("_Black", Color.black);
        mat.SetColor("_Grey", grey);
        mat.SetColor("_White", white);
        return mat;
    }

    public void SaveCubemap(string filename, int faceResolution)
    {
        var camObj = new GameObject("BackgroundCamera");
        var cam = camObj.AddComponent<Camera>();
        cam.name = "CaptureCam1";
        //cam.cullingMask = LayerMask.GetMask("Universe Background");
        cam.fieldOfView = 90;
        //cam.transform.rotation = Quaternion.identity;
        cam.transform.rotation = Quaternion.Euler(0, 270, 0);
        cam.transform.position = Vector3.zero;

        var cubeMapImage = new Texture2D(faceResolution * 4, faceResolution * 3);

        cubeMapImage.SetPixels32(0, faceResolution, faceResolution, faceResolution, GetTexture2D(cam, faceResolution).GetPixels32());

        cam.transform.Rotate(0, 90, 0);
        cubeMapImage.SetPixels32(faceResolution, faceResolution, faceResolution, faceResolution, GetTexture2D(cam, faceResolution).GetPixels32());

        cam.transform.Rotate(0, 90, 0);
        cubeMapImage.SetPixels32(faceResolution * 2, faceResolution, faceResolution, faceResolution, GetTexture2D(cam, faceResolution).GetPixels32());

        cam.transform.Rotate(0, 90, 0);
        cubeMapImage.SetPixels32(faceResolution * 3, faceResolution, faceResolution, faceResolution, GetTexture2D(cam, faceResolution).GetPixels32());

        cam.transform.Rotate(0, 180, 0);
        cam.transform.Rotate(90, 0, 0);
        cubeMapImage.SetPixels32(faceResolution, 0, faceResolution, faceResolution, GetTexture2D(cam, faceResolution).GetPixels32());

        cam.transform.Rotate(-180, 0, 0);
        cubeMapImage.SetPixels32(faceResolution, faceResolution * 2, faceResolution, faceResolution, GetTexture2D(cam, faceResolution).GetPixels32());

        File.WriteAllBytes(filename, cubeMapImage.EncodeToPNG());
    }

    #endregion

    private Texture2D GetTexture2D(Camera cam, int res)
    {
        cam.orthographic = true;
        var renTex = new RenderTexture(res, res, 24);
        cam.targetTexture = renTex;
        cam.targetTexture = renTex;
        var text2d = new Texture2D(res, res, TextureFormat.ARGB32, false);
        cam.Render();
        RenderTexture.active = renTex;
        text2d.ReadPixels(new Rect(0, 0, res, res), 0, 0);
        //var b = text2d.EncodeToPNG();
        //RenderTexture.active = null;
        return text2d;
    }
}

[System.Serializable]
public class ScatterSettings
{
    public string Name;

    public bool IsActive;

    public int CountMin;

    public int CountMax;

    public float ScaleMin;

    public float ScaleMax;

    public float RadiusMin;

    public float RadiusMax;

    public GameObject Model;

    public bool UseClustering;

    public int ClusterCountMin;

    public int ClusterCountMax;

    public float ClusterScatter;

    public bool LookAtCenter;

    public List<Texture> Textures;

    public List<ColorRange> Colors;

    public bool UseMaterials;

    public List<Material> Materials;

    public ScatterSettings()
    {
        IsActive = true;
        Colors = new List<ColorRange> { new ColorRange() };
    }
}

[System.Serializable]
public class ColorRange
{
    public Color Color1;

    public Color Color2;

    public ColorRange()
    {
        Color1 = Color.white;
        Color2 = Color.white;
    }

    public Color GetRandom()
    {
        return Utility.GetRandomColor(Color1, Color2);
    }
}