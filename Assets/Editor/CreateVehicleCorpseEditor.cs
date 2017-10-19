using UnityEditor;
using UnityEngine;

public class CreateVehicleCorpseEditor : Editor
{
    [MenuItem("Space X/Build Corpse")]
    public static void BuildCorpse()
    {
        var selected = Selection.activeGameObject;
        if (selected != null)
        {
            var rigidBody = selected.AddComponent<Rigidbody>();
            rigidBody.mass = 400f;
            rigidBody.useGravity = false;

            var killable = selected.AddComponent<Killable>();
            killable.MaxHealth = 30f;
            killable.Health = killable.MaxHealth;
            killable.DestroyOnDie = false;

            selected.AddComponent<Shiftable>();
            selected.AddComponent<VehicleCorpse>();
        }
    }
}
