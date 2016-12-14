using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TerrainCreation;
[RequireComponent(typeof(Terrain))]
public class Fractals : MonoBehaviour
{
    public float seed;
    public float variation;
    public float roughness;
    

    private int terraforming_size = 129;
    private float terraforming_variation = 0.0002f;
    private DiamondSquareAlgorithm alg;
    private Terrain terrain;
    private int h_size;

    void Awake()
    {
        alg = new DiamondSquareAlgorithm();
        terrain = GetComponent<Terrain>();
        h_size = terrain.terrainData.heightmapResolution;
        terrain.terrainData.SetHeights(0, 0, alg.GenerateTerrain(h_size, seed, variation, roughness));
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 150, 30), "Generate terrain"))
            terrain.terrainData.SetHeights(0, 0, alg.GenerateTerrain(h_size, seed, variation, roughness));


    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (terrain.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 pos = hit.point;
                pos -= terrain.transform.position;
                pos.x /= terrain.terrainData.size.x;
                pos.z /= terrain.terrainData.size.z;
                int x = (int)(pos.x * terrain.terrainData.heightmapWidth);
                int y = (int)(pos.z * terrain.terrainData.heightmapHeight);
                int s = (terraforming_size - 1) / 2;
                int s_x, s_y;
                if (x - s < 0)
                    s_x = 0;
                else
                    s_x = x - s;

                if (y - s < 0)
                    s_y = 0;
                else
                    s_y = y - s;
                terrain.terrainData.SetHeights(0, 0, alg.FlattenTerrain(s_y, s_x, terraforming_size, terraforming_variation));
            }
        }
    }
}

