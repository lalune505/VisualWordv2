using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisualWordController : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    [SerializeField] private VisualText visualText;

    void Start()
    {
        SetFeatures(visualText.Text.LastOrDefault());
    }
    public void SetFeatures(VisualWord visualWord)
    {
        rend.material.SetFloat("_Speed", visualWord.Speed);
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
}
