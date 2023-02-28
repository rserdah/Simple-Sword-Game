using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    Animation anim;
    AnimationClip clip;

    public float health = 100f;
    public float maxHealth = 100f;
    public Material select;
    public int frameRate = 10;
    public int frame;


    private void FixedUpdate()
    {
        select.SetColor("_EmissionColor", Color.Lerp(Color.red, Color.green, Mathf.Clamp01(health / maxHealth))); //Make selection color based on selected target's health
    }
}
