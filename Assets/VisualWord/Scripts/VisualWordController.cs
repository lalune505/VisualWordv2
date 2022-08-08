using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisualWordController : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    [SerializeField] private VisualText visualText;

    private float _phase;
    private float _speed;

    private void Start()
    {
        SetFeatures(visualText.Text.FirstOrDefault());
        StartCoroutine(StartLerp());
    }

    IEnumerator StartLerp()
    {
        for (var i = 0; i < visualText.Text.Count - 1; i++)
        {
            yield return StartCoroutine(LerpFunction(visualText.Text[i+1], 10f));
        }
    }

    void Update()
    {
        _phase += Time.deltaTime * _speed;
        rend.material.SetFloat("_Speed", _phase);
    }
    IEnumerator LerpFunction(VisualWord endValue, float duration)
    {
        float time = 0;
        VisualWord startValue = GetVisualWord();
        while (time <= duration)
        {
            Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }

    private void SetFeatures(VisualWord visualWord)
    {
        //rend.material.SetFloat("_Speed", visualWord.Speed);
        _speed = visualWord.Speed;
        rend.material.SetFloat("_Size", visualWord.Size);
        rend.material.SetFloat("_Frequency", visualWord.Frequency);
        rend.material.SetFloat("_Glossiness", visualWord.Glossiness);
        rend.material.SetColor("_MaxColor", visualWord.MAXColor);
        rend.material.SetColor("_Color", visualWord.Color);
        rend.material.SetColor("_RimColor",  visualWord.RimColor);
        rend.material.SetFloat("_Metallic", visualWord.Metalness);

        var s = visualWord.Scale;
        rend.transform.localScale = new Vector3(s,s,s);
    }
    private void SetFeatures(float speed, float size, float freq, float gloss, Color maxColor, Color color, Color rimColor,
        float metallic, float scale)
    {
        rend.material.SetFloat("_Speed", speed);
        rend.material.SetFloat("_Size", size);
        rend.material.SetFloat("_Frequency", freq);
        rend.material.SetFloat("_Glossiness", gloss);
        rend.material.SetColor("_MaxColor", maxColor);
        rend.material.SetColor("_Color", color);
        rend.material.SetColor("_RimColor", rimColor);
        rend.material.SetFloat("_Metallic", metallic);

        var s = scale;
        rend.transform.localScale = new Vector3(s,s,s);
    }

    private VisualWord GetVisualWord()
    {
        var speed = rend.material.GetFloat("_Speed");
        var size =rend.material.GetFloat("_Size");
        var freq = rend.material.GetFloat("_Frequency");
        var gloss = rend.material.GetFloat("_Glossiness");
        var maxColor = rend.material.GetColor("_MaxColor");
        var color = rend.material.GetColor("_Color");
        var rimColor = rend.material.GetColor("_RimColor");
        var metallic = rend.material.GetFloat("_Metallic");

        var scale = rend.transform.localScale;

        return new VisualWord(size, scale.x, color, rimColor, maxColor,
            speed, freq, gloss, metallic);
    }

    private void Lerp(VisualWord visualWord1, VisualWord visualWord2, float t)
    {
        //rend.material.SetFloat("_Speed", Mathf.Lerp(visualWord1.Speed, visualWord2.Speed, t)); 
        _speed = Mathf.Lerp(_speed, visualWord2.Speed, t);
        rend.material.SetFloat("_Size", Mathf.Lerp(visualWord1.Size, visualWord2.Size, t));
        rend.material.SetFloat("_Frequency",Mathf.Lerp(visualWord1.Frequency, visualWord2.Frequency, t));
        rend.material.SetFloat("_Glossiness", Mathf.Lerp(visualWord1.Glossiness, visualWord2.Glossiness, t));
        rend.material.SetColor("_MaxColor", Color.Lerp(visualWord1.MAXColor, visualWord2.MAXColor, t));
        rend.material.SetColor("_Color", Color.Lerp(visualWord1.Color, visualWord2.Color, t));
        rend.material.SetColor("_RimColor", Color.Lerp(visualWord1.RimColor, visualWord2.RimColor, t));
        rend.material.SetFloat("_Metallic",Mathf.Lerp(visualWord1.Metalness, visualWord2.Metalness, t) );

        var s = Mathf.Lerp(visualWord1.Scale, visualWord2.Scale, t);
        rend.transform.localScale = new Vector3(s,s,s);
    }
    
    
}
