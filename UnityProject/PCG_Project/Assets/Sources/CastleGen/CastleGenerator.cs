using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CastleGenerator
{

    public const float GENERATIONS = 10;
    public const float POPULATION_SIZE = 200;

    public const float MUTATE_PROBABILITY = 0.2F;

    List<Castle> population;

    List<List<Castle>> generations = new List<List<Castle>>();

    Vector2 maxCastleSize;
    public Vector2 Dimension { get { return this.maxCastleSize; } }

    int desiredBuildingCount;
    public int DesiredBuildingCount { get { return this.desiredBuildingCount; } }

    public CastleGenerator(Vector2 maxSize, int buildingCount)
    {
        this.maxCastleSize = maxSize;
        this.desiredBuildingCount = buildingCount;
    }

    public IEnumerator runEvolution()
    {
        Debug.Log("start");
        int curGeneration = 1;

        population = new List<Castle>();
        for (int i = 0; i < POPULATION_SIZE; i++)
        {
            Castle castle = new Castle(this, 10, 20, new int[0], desiredBuildingCount, Vector2.zero);
            castle.randomizeValues();
            population.Add(castle);
        }

        evaluateGeneration();

        //population.Sort();
        //population[0].buildCastle();

        while (curGeneration <= GENERATIONS)
        {
            //Debug
            float bestFitness = float.MinValue;
            float worstFitness = float.MaxValue;
            float avgFitness = 0;

            foreach (Castle castle in population)
            {
                if (castle.fitness > bestFitness)
                {
                    bestFitness = castle.fitness;
                }
                if (castle.fitness < worstFitness)
                {
                    worstFitness = castle.fitness;
                }
                avgFitness += castle.fitness;
            }
            avgFitness /= population.Count;

            population.Sort();
            Debug.Log("Gen#" + curGeneration + " Best: " + bestFitness + " | Worst: " + worstFitness + " | AVG: " + avgFitness + " | best output: " + population[0].output() + " F:" + population[0].fitness);
            // /Debug
            generations.Add(new List<Castle>(population));
            produceNextGeneration();
            curGeneration++;



            evaluateGeneration();

            yield return null;
        }

        //generate on map the best castle
        population.Sort();
        Debug.Log("Best Fitness: " + population[0].fitness + " output: " + population[0].output());
        generations.Add(new List<Castle>(population));
        Serialize();
        Terrain terrain = Terrain.activeTerrain;

        Fractals fac = terrain.GetComponent<Fractals>();

        Vector3 pos = new Vector3(population[0].getPos().x, 0, population[0].getPos().y);
        pos -= terrain.transform.position;
        pos.x /= terrain.terrainData.size.x;
        pos.z /= terrain.terrainData.size.z;
        int x = (int)(pos.x * terrain.terrainData.heightmapWidth);
        int y = (int)(pos.z * terrain.terrainData.heightmapHeight);
        int s = (fac.terraforming_size - 1) / 2;
        int s_x, s_y;
        if (x - s < 0)
            s_x = 0;
        else
            s_x = x - s;

        if (y - s < 0)
            s_y = 0;
        else
            s_y = y - s;
        terrain.terrainData.SetHeights(0, 0, fac.alg.FlattenTerrain(s_y, s_x, fac.terraforming_size, fac.terraforming_variation));

        

        population[0].buildCastle();

        Debug.Log("end");
    }

    public void Serialize()
    {

        string name = "log.csv";
        string serialization = "";
        serialization += "GEN_NUMB;BEST_FITNESS;MID_FITNESS;WORST_FITNESS" + System.Environment.NewLine;
        int i = 1;
        foreach (List<Castle> generation in generations)
        {
            serialization += i + ";" + generation[0].fitness + ";" + generation[(generation.Count - 1) / 2].fitness + ";" + generation[generation.Count - 1].fitness + ";" + System.Environment.NewLine;
            i++;
        }
        File.WriteAllText(name, serialization);

    }

    void evaluateGeneration()
    {
        for (int i = 0; i < POPULATION_SIZE; i++)
        {
            Castle castle = population[i];

            castle.generateCastle();
            castle.evaluate();
        }
    }

    void produceNextGeneration()
    {
        int size = population.Count;

        population.Sort();
        List<Castle> allowedToReproduce = new List<Castle>();

        foreach (Castle castle in population)
        {
            if (Random.value < castle.fitness)
            {
                allowedToReproduce.Add(castle);
            }
        }
        allowedToReproduce.Sort();

        List<Castle> nextGeneration = new List<Castle>();

        nextGeneration.AddRange(allowedToReproduce);


        int index = 0;

        while (nextGeneration.Count < size)
        {
            if (index > allowedToReproduce.Count - 1)
            {
                index = 0;
            }
            Castle[] offspring = allowedToReproduce[index].reproduce(population[Random.Range(0, population.Count)]);
            index++;

            foreach (Castle castle in offspring)
            {
                castle.mutate();
                if (nextGeneration.Count < size)
                { nextGeneration.Add(castle); }
            }
        }


        population.Clear();
        population = nextGeneration;
    }
}
