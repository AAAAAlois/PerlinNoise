using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightingSettings : MonoBehaviour
{
    public Slider FogS;
    public Slider LightingR;
    public Slider LightingG;
    public Slider LightingB;
    public Slider LightingIntensive;
    public Light derecteLight;
    public GameObject LightP;
    public Slider LightDirection;
    // Start is called before the first frame update
    void Start()
    {
        FogS.maxValue = 20000;
        FogS.minValue = 1200f;
        LightingR .maxValue =1.0f;
        LightingR.minValue = 0.0f;
        LightingB .maxValue = 1.0f;
        LightingG .maxValue = 1.0f;
        LightingG .minValue = 0.0f;
        LightingB .minValue = 0.0f;
        LightingIntensive.maxValue = 3f;
        LightingIntensive.minValue = 0.1f;
        LightDirection.maxValue = Mathf.PI/2;
        LightDirection.minValue = -Mathf.PI / 2;
    }

    // Update is called once per frame
    void Update()
    {
        derecteLight .color =new Color (LightingR .value ,LightingG .value ,LightingB .value,1.0f );
        derecteLight.intensity = LightingIntensive.value;
        RenderSettings .fogEndDistance = FogS.value;
        LightP.transform.rotation = new Quaternion(LightP.transform.rotation.x, LightP.transform.rotation.y, LightDirection.value, LightP.transform.rotation.w);
    }
    
}
