using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CastleGenerator {

    public const float GENERATIONS = 100;
    public const float POPULATION_SIZE = 100;

    public const float MUTATE_PROBABILITY = 0.3F;

    List<Castle> population;

    Vector2 maxCastleSize;
    public Vector2 Dimension { get { return this.maxCastleSize; } }

    Vector2 castlePosition;
    public Vector2 Position { get { return this.castlePosition; } }

    int desiredBuildingCount;
    public int DesiredBuildingCount { get { return this.desiredBuildingCount; } }

    public CastleGenerator(Vector2 maxSize, Vector2 pos, int buildingCount)
    {
        this.maxCastleSize = maxSize;
        this.castlePosition = pos;
        this.desiredBuildingCount = buildingCount;
    }

    public IEnumerator runEvolution()
    {
        Debug.Log("start");
        int curGeneration = 1;

        population = new List<Castle>();
        for (int i = 0; i < POPULATION_SIZE; i++)
        {
            Castle castle = new Castle(this, 10, 20, new int[0], desiredBuildingCount);
            castle.randomizeValues();
            population.Add(castle);
        }

        evaluateGeneration();

        //population.Sort();
        //population[0].buildCastle();

        while(curGeneration <= GENERATIONS)
        {
            //Debug
            float bestFitness = float.MinValue;
            float worstFitness = float.MaxValue;
            float avgFitness = 0;

            foreach(Castle castle in population)
            {
                if(castle.fitness > bestFitness)
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
            Debug.Log("Gen#" + curGeneration + " Best: " + bestFitness + " | Worst: " + worstFitness + " | AVG: " + avgFitness +" | best output: "+population[0].output()+ " F:"+population[0].fitness);
            // /Debug

            produceNextGeneration();
            curGeneration++;



            evaluateGeneration();

            yield return null;
        }

        //generate on map the best castle
        population.Sort();
        Debug.Log("Best Fitness: " + population[0].fitness+ " output: "+population[0].output());
        population[0].buildCastle();

        Debug.Log("end");
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

        foreach(Castle castle in population)
        {
            if(Random.value < castle.fitness)
            {
                allowedToReproduce.Add(castle);
            }
        }
        allowedToReproduce.Sort();
        Debug.Log("Orginal: " + population.Count + " reproduce: " + allowedToReproduce.Count+ " Org Best:"+population[0].fitness+" re best"+allowedToReproduce[0].fitness);

        List<Castle> nextGeneration = new List<Castle>();

        nextGeneration.AddRange(allowedToReproduce);

        Debug.Log("nextGen Count: " + nextGeneration.Count);

        int index = 0;

        while(nextGeneration.Count < size)
        {
            if(index > allowedToReproduce.Count-1)
            {
                index = 0;
            }
            Castle[] offspring = allowedToReproduce[index].reproduce(population[Random.Range(0,population.Count)]);
            index++;

            foreach(Castle castle in offspring)
            {
                castle.mutate();
                if (nextGeneration.Count < size)
                { nextGeneration.Add(castle); }
            }
        }

        Debug.Log("nextGen after Count: " + nextGeneration.Count);

        population.Clear();
        population = nextGeneration;
    }
}
