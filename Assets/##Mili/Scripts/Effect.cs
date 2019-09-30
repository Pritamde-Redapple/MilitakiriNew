using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public EffectType effectType;
    public ParticleSystem particleSystem;


    public void SetPS(Mesh newMesh)
    {
        var p = particleSystem.shape;
        p.mesh = newMesh;
    }
}
