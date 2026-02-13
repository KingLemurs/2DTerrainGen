using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PerlinNoiseMap : MonoBehaviour
{
    [Serializable]
    public enum LayerType
    {
        NORMAL,
        ADD,
        SUBTRACT
    }
    
    [Serializable]
    public class LayerData
    {
        public LayerType layerType;
        public float layerThreshold = 0.5f;
        public List<WeightTable> availableTiles;
        public float noiseScale;
        public FastNoiseLite.NoiseType type;
        public FastNoiseLite.FractalType fractalType;
        public float fractalGain = 1;
    }
    
    public Tilemap tilemap;
    
    // INSTEAD OF PASSING IN INDIVIDUAL TILES, ALLOW FOR A TILE PALETTE TO BE PASSED IN, WHERE A RANDOM OR WEIGHTED
    // TILE IS CHOSEN FOR EACH PALETTE. THIS ALLOWS FOR THE CREATION OF "BIOMES" OR "REGIONS"
    
    public List<LayerData> layerData;
    public float seed;
    public int mapWidth;
    public int mapHeight;
    
    private List<Dictionary<int, WeightTable>> _tileset;
    private List<List<int>> _noiseGrid = new List<List<int>>();
    private List<FastNoiseLite> _noiseLayers = new List<FastNoiseLite>();

    private static int SKIP_FLAG = -1;
    private static int SUBTRACT_FLAG = -2;
     
    void CreateTileset(int index)
    {
        _tileset.Add(new Dictionary<int, WeightTable>());
        for (int i = 0; i < layerData[index].availableTiles.Count; i++)
        {
            _tileset[index].Add(i, layerData[index].availableTiles[i]);
        }
    }

    int GetTileFromPalette(WeightTable t)
    {
        int sum = Random.Range(0, 1000);
        int weight = 0;
        for (int i = 0; i < t.Palette.Count; i++)
        {
            weight += Mathf.FloorToInt(t.Palette[i].weight * 1000);
            if (sum > weight)
            {
                return i;
            }
        }

        return t.Palette.Count - 1;
    }

    void CreateTile(int index, int tileId, int x, int y)
    {
        int tableIndex = GetTileFromPalette(_tileset[index][tileId]);
        Tile tile = _tileset[index][tileId].Palette[tableIndex].tile;
        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }

    void GenMap(int index)
    {
        for (int x = 0; x < mapWidth; x++)
        {
            _noiseGrid.Add(new List<int>());

            for (int y = 0; y < mapHeight; y++)
            {
                int tileId = GetIdFromNoise(index, x, y);

                if (tileId == SKIP_FLAG)
                {
                    continue;
                }
                else if (tileId == SUBTRACT_FLAG)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                    continue;
                }
                
                _noiseGrid[x].Add(tileId);
                CreateTile(index, tileId, x, y);
            }
        }
    }

    int GetIdFromNoise(int index, int x, int y)
    {
        float noiseScale = layerData[index].noiseScale;
        float raw = _noiseLayers[index].GetNoise((x + seed) / noiseScale, (y + seed) / noiseScale);

        if (layerData[index].layerType == LayerType.ADD && layerData[index].layerThreshold > raw)
        {
            return SKIP_FLAG;
        }
        else if (layerData[index].layerType == LayerType.SUBTRACT && layerData[index].layerThreshold > raw)
        {
            return SUBTRACT_FLAG;
        }
        else if (layerData[index].layerType == LayerType.SUBTRACT && layerData[index].layerThreshold <= raw)
        {
            return SKIP_FLAG;
        }
        
        raw = Mathf.Clamp(raw, 0f, 1f);

        float scaled = raw * _tileset[index].Count;
        if ((int)scaled == _tileset[index].Count)
        {
            scaled = _tileset[index].Count - 1;
        }

        return Mathf.FloorToInt(scaled);
    }

    private void ScrubInputs()
    {
        if (1000000 < Mathf.Abs(seed))
        {
            throw new ArgumentException("Seed must be within -1000000, 1000000");
        }

        if (layerData.Count == 0)
        {
            throw new ArgumentException("Must have at least one layer");
        }
    }

    private void Start()
    {
        ScrubInputs();
        
        _tileset = new List<Dictionary<int, WeightTable>>();
        for (int i = 0; i < layerData.Count; i++)
        {
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(layerData[i].type);
            noise.SetFractalType(layerData[i].fractalType);
            noise.SetFractalGain(layerData[i].fractalGain);
            _noiseLayers.Add(noise);
            CreateTileset(i);
            GenMap(i);
        }
    }
}
