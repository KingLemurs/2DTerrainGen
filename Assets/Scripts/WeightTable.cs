using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class WeightTable : ScriptableObject
{
    [Serializable]
    public class WeightEntry
    {
        public Tile tile;
        public float weight;
    }
    
    public List<WeightEntry> Palette;
}