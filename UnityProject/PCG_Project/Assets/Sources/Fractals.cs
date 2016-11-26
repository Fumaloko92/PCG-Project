using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class Fractals : MonoBehaviour
{
    public float seed;
    public float variation;
    public float roughness;

    private int iterations;
    private float _variation;
    private Terrain terrain;

    private float[,] heightmap;
    private int h_size;
    private int step;
    void Awake()
    {
        terrain = GetComponent<Terrain>();
        h_size = terrain.terrainData.heightmapResolution;
        iterations = (int)Mathf.Log(h_size - 1, 2);
        heightmap = new float[h_size, h_size];
        _variation = variation;

        DiamondSquareLoop();
       
       
    }

    private void DiamondSquareLoop()
    {
        step = h_size - 1;
        InitializeDiamondSquare();
        for (int i = 0; i < iterations; i++, step /= 2)
        {
            DiamondStep();
            
            SquareStep();
            UpdateVariation();
            terrain.terrainData.SetHeights(0, 0, heightmap);
        }
    }

    private void InitializeDiamondSquare()
    {
        heightmap[0, h_size - 1] = seed;
        heightmap[h_size - 1, 0] = seed;
        heightmap[0, 0] = seed;
        heightmap[h_size - 1, h_size - 1] = seed;
    }

    private void DiamondStep()
    {
        int step = this.step / 2;
        for (int i = step; i < h_size - 1; i += this.step)
            for (int k = step; k < h_size - 1; k += this.step)
                heightmap[i, k] = SquareSum(i - step, k - step, this.step) + RandomAmount();
    }

    private float SquareSum(int i, int k, int step)
    {
        return (heightmap[i, k] + heightmap[i + step, k] + heightmap[i, k + step] + heightmap[i + step, k + step]) / 4;
    }

    private void SquareStep()
    {
        int step = this.step / 2;
        for (int i = step; i < h_size - 1; i += this.step)
            for (int k = step; k < h_size - 1; k += this.step)
            {
                if (InsideMap(i, k + step))
                {
                    heightmap[i, k + step] = DiamondSum(i, k + step, step) + RandomAmount();
                }
                if (InsideMap(i, k - step))
                {
                    heightmap[i, k - step] = DiamondSum(i, k - step, step) + RandomAmount();
                }
                if (InsideMap(i + step, k))
                {
                    heightmap[i + step, k] = DiamondSum(i + step, k, step) + RandomAmount();
                }
                if (InsideMap(i - step, k))
                {
                    heightmap[i - step, k] = DiamondSum(i - step, k, step) + RandomAmount();
                }
            }
    }

    private float RandomAmount()
    {
       return (float)StaticRandom.Sample() * _variation*2-_variation;
    }

    private float DiamondSum(int i, int k, int step)
    {
        float sum = 0; int counter = 0;
        if (InsideMap(i, k + step))
        {
            sum += heightmap[i, k + step];
            counter++;
        }
        if (InsideMap(i, k - step))
        {
            sum += heightmap[i, k - step];
            counter++;
        }
        if (InsideMap(i + step, k))
        {
            sum += heightmap[i + step, k];
            counter++;
        }
        if (InsideMap(i - step, k))
        {
            sum += heightmap[i - step, k];
            counter++;
        }
        return sum / counter;
    }

    private bool InsideMap(int i, int k)
    {
        return i < heightmap.GetLength(0) && i >= 0 && k < heightmap.GetLength(1) && k >= 0;
    }

    private void UpdateVariation()
    {
        _variation = _variation * Mathf.Pow(2f, -roughness);
    }

    private void PrintHeightmap()
    {
        string s = "";
        for (int j = 0; j < heightmap.GetLength(0); j++)
        {
            s += "[";
            for (int k = 0; k < heightmap.GetLength(1); k++)
                s += heightmap[j, k] + " ";
            s += "]" + System.Environment.NewLine;
        }
        Debug.Log(s);
    }
}

