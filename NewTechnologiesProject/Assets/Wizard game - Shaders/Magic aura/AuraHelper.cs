using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraHelper : MonoBehaviour {

    [Header("Texture scrolling")]
    Material mat;
    public float speed;

    [Header("Color pulsing")]
    public float pulseModifier;
    float actualMod;

    [Header("Distortion")]
    public bool isDistortion;
    public Vector2 xDistortRange;
    public Vector2 yDistortRange;
    public float sizeOscillation;
    public float sizeOscSpeed;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

	void FixedUpdate()
    {
        mat.SetFloat("_time", Time.time * speed);
        actualMod = Mathf.Clamp01(Mathf.Sin(Time.time) * pulseModifier);
        mat.SetFloat("_colorModifier", actualMod);
        if (isDistortion)
        {
            SetDistortion();
        }
    }

    void SetDistortion()
    {
        float tempFloat = 0;
        tempFloat = Mathf.PingPong(tempFloat + Time.time * sizeOscSpeed, sizeOscillation) + 1;
        Vector3 newScale = transform.localScale;
        newScale.x = tempFloat;
        newScale.y = tempFloat;
        newScale.z = tempFloat;
        transform.localScale = newScale;



        mat.SetFloat("_xDistortionOne", xDistortRange.x);
        mat.SetFloat("_xDistortionTwo", xDistortRange.y);
        mat.SetFloat("_yDistortionOne", yDistortRange.x);
        mat.SetFloat("_yDistortionTwo", yDistortRange.y);
    }

}
