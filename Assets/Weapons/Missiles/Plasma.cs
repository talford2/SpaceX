using UnityEngine;
using System.Collections;

public class Plasma : Laser {
    [Header("Plasma")]
    public MeshRenderer PlasmaMesh;

    public override void Shoot(Vector3 shootFrom, Vector3 direction, Vector3 initVelocity)
    {
        base.Shoot(shootFrom, direction, initVelocity);
        PlasmaMesh.enabled = true;
    }

    public override void Stop()
    {
        base.Stop();
        PlasmaMesh.enabled = false;
    }
}
