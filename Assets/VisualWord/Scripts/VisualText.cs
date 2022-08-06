using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/VisualText", order = 1)]
public class VisualText : ScriptableObject
{
    [SerializeField] private List<VisualWord> text = new List<VisualWord>();
    
    public List<VisualWord> Text
    {
        get => text;
        set => text = value;
    }

    public void AddWord(VisualWord word)
    {
        text.Add(word);
    }

    public void Clear()
    {
        text.Clear();
    }
}
