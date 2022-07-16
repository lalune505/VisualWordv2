using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VisualWord
{
   [SerializeField] private float size;
   [SerializeField] private float scale;
   [SerializeField] private Color color;
   [SerializeField] private Color rimColor;
   [SerializeField] private Color maxColor;
   [SerializeField] private float speed;
   [SerializeField] private float frequency;
   [SerializeField] private float glossiness;
   [SerializeField] private float metalness;

   public VisualWord(float size, float scale, Color color, Color rimColor, Color maxColor, float speed, float frequency, float glossiness, float metalness)
   {
      this.Size = size;
      this.Scale = scale;
      this.Color = color;
      this.RimColor = rimColor;
      this.MAXColor = maxColor;
      this.Speed = speed;
      this.Frequency = frequency;
      this.Glossiness = glossiness;
      this.Metalness = metalness;
   }

   public float Size
   {
      get => size;
      set => size = value;
   }

   public Color Color
   {
      get => color;
      set => color = value;
   }

   public Color RimColor
   {
      get => rimColor;
      set => rimColor = value;
   }

   public Color MAXColor
   {
      get => maxColor;
      set => maxColor = value;
   }

   public float Speed
   {
      get => speed;
      set => speed = value;
   }

   public float Frequency
   {
      get => frequency;
      set => frequency = value;
   }

   public float Glossiness
   {
      get => glossiness;
      set => glossiness = value;
   }

   public float Metalness
   {
      get => metalness;
      set => metalness = value;
   }

   public float Scale
   {
      get => scale;
      set => scale = value;
   }
}
