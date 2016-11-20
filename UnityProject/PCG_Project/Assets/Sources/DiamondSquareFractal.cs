using UnityEngine;
using System.Collections;

public class DiamondSquareFractal : MonoBehaviour {
    public float seed;
    public int iterations;
    public float variation;
    public float roughness;

    private int _iterations;
    private float _variation;
    private float _roughness;
    private float _seed;

    private int i;
    private Terrain terrain;
	void Awake () {
        gameObject.AddComponent<Terrain>();
        gameObject.AddComponent<TerrainCollider>();
        terrain = GetComponent <Terrain>(); ;
        TerrainCollider terrainCollider = GetComponent<TerrainCollider>();
        float[,] map;
        int d = (int)Mathf.Pow(2, iterations) + 1;
        map = new float[d, d];
        map[0,0] = seed; map[0,d - 1] = seed; map[d - 1,0] = seed; map[d - 1,d - 1] = seed;
        i = 0;
        TerrainData data = new TerrainData();
        data.size = new Vector3(1, 1, 1);
        data.heightmapResolution = d;
        data.SetHeights(0, 0, map);
        terrain.terrainData = data;
        terrainCollider.terrainData = terrain.terrainData;
        _iterations = iterations;
        _variation = variation;
        _roughness = roughness;
        _seed = seed;
        RunDiamondSquare();
    }

    private void RunDiamondSquare()
    {

        iterations = _iterations;
        variation = _variation;
        roughness = _roughness;
        seed = _seed;
        for (int i = 0; i < iterations; i++)
        {
            //yield return new WaitForSeconds(3f);
            float[,] map;
            map = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight);
            map = diamondStep(i, map);
            map = squareStep(i, map);
            UpdateVariation();
            terrain.terrainData.SetHeights(0, 0, map);
        }
    }
 
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            _iterations = iterations;
            _variation = variation;
            _roughness = roughness;
            _seed = seed;
        }
        
        if (Input.GetMouseButton(0))
            RunDiamondSquare();
    }


    private float[,] diamondStep(int iteration, float[,] map)
    {
        int step = map.GetLength(0) - 1;

        for (int i = 0; i < iteration; i++)
            step /= 2;

        for (int i = step / 2; i + step / 2 < map.GetLength(0); i += step)
        {
            for (int k = step / 2; k + step / 2 < map.GetLength(0); k += step)
            {
                map[i,k] = (map[i - step / 2,k - step / 2] + map[i + step / 2,k + step / 2] + map[i - step / 2,k + step / 2] + map[i + step / 2,k - step / 2]) / 4 + RandomNumber() * variation * 2 - variation;
            }
        }

        return map;
    }

    private float RandomNumber()
    {
        float f = Random.Range(0f,1f);
        
        return f;
    }

    private float[,] squareStep(int iteration, float[,] map)
    {
        int step = map.GetLength(0) - 1;
        for (int i = 0; i < iteration; i++)
            step /= 2;


        for (int i = 0; i < map.GetLength(0); i += step)
        {
            for (int k = step / 2; k < map.GetLength(0); k += step)
            {
                float v = diamondSum(map, step / 2, i, k);
                map[i,k] = v + (RandomNumber() * variation * 2 - variation);
            }
        }

        for (int i = step / 2; i < map.GetLength(0); i += step)
        {
            for (int k = 0; k < map.GetLength(0); k += step)
            {

                float v = diamondSum(map, step / 2, i, k);
                map[i,k] = v + (RandomNumber() * variation * 2 - variation);
            }
        }
        return map;
    }

    private float diamondSum(float[,] map, int step, int i, int k)
    {
        float sum = 0;
        int counter = 0;
        if (k - step >= 0)
        {
            sum += map[i,k - step];
            counter++;
        }
        if (k + step < map.GetLength(0))
        {
            sum += map[i,k + step];
            counter++;
        }
        if (i - step >= 0)
        {
            sum += map[i - step,k];
            counter++;
        }
        if (i + step < map.GetLength(0))
        {
            sum += map[i + step,k];
            counter++;
        }

        return sum / counter;
    }

    private void UpdateVariation()
    {
        variation = variation * Mathf.Pow(2f, -roughness);
    }

}
