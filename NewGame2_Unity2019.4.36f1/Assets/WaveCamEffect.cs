using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WaveCamEffect : MonoBehaviour
{
    public enum EffectType
    {
        DRUNKEFFECT, HEARTBEAT
    }

    public CinemachineFreeLook cam;

    public EffectType effectType;

    [Serializable]
    public struct MinMaxValues
    {
        public float min;
        public float max;
        public float speed;
    }

    public MinMaxValues FOV;
    public MinMaxValues dutch;
    public MinMaxValues screenX;
    public MinMaxValues screenY;


    private void Update()
    {
        if(cam)
        {
            cam.m_Lens.Dutch = GetSine(dutch);
            cam.m_Lens.FieldOfView = GetSine(FOV);
        }
    }

    private float GetSine(MinMaxValues m)
    {
        return ((m.max - m.min) / 2) * Mathf.Sin(Time.time * m.speed) + ((m.min + m.max) / 2);
    }
}
