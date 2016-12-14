using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TerrainCreation
{
    class DiamondSquareAlgorithm
    {
        private float[,] heightmap;
        private float[,] startingHeightmap;
        private int h_size;
        private int step;
        private int iterations;
        private float seed;
        private float variation;
        private float terrain_variation;
        private float roughness;

        private int x_start_flat;
        private int y_start_flat;
        private int size_flat;
        private float terraform_var;

        public float[,] FlattenTerrain(int x, int y, int size, float variation)
        {
            x_start_flat = x;
            y_start_flat = y;
            size_flat = size;
            terraform_var = variation;
            this.variation = terrain_variation;
            h_size = startingHeightmap.GetLength(0);
            heightmap = new float[h_size, h_size];
            heightmap[0, h_size - 1] = startingHeightmap[0, h_size - 1];
            heightmap[h_size - 1, 0] = startingHeightmap[h_size - 1, 0];
            heightmap[0, 0] = startingHeightmap[0, 0];
            heightmap[h_size - 1, h_size - 1] = startingHeightmap[h_size - 1, h_size - 1];
            iterations = (int)Math.Log(h_size - 1, 2);
            step = h_size - 1;
            for (int i = 0; i < iterations; i++, step /= 2)
            {
                DiamondStep(true);
                SquareStep(true);
                UpdateVariation();
            }
            return heightmap;
        }

        public float[,] GenerateTerrain(int size, float seed, float variation, float roughness)
        {
            heightmap = new float[size, size];
            x_start_flat = 0;
            y_start_flat = 0;
            size_flat = 0;
            h_size = size;
            this.seed = seed;
            this.variation = variation;
            terrain_variation = variation;
            this.roughness = roughness;
            iterations = (int)Math.Log(h_size - 1, 2);
            step = h_size - 1;
            InitializeDiamondSquare();
            for (int i = 0; i < iterations; i++, step /= 2)
            {
                DiamondStep(false);

                SquareStep(false);
                UpdateVariation();
            }
            startingHeightmap = new float[size, size];
            for (int i = 0; i < heightmap.GetLength(0); i++)
                for (int k = 0; k < heightmap.GetLength(1); k++)
                    startingHeightmap[i, k] = heightmap[i, k];
            return heightmap;
        }

        private float CalculateLocalAverage(float[,] heightmap, int x, int y, int r)
        {
            float avg = 0;
            int c = 0;
            for (int i = x - r; i < x + r; i++)
                for (int k = y - r; k < y + r; k++)
                {
                    if (i < heightmap.GetLength(0) && k < heightmap.GetLength(0) && i >= 0 && k >= 0)
                    {
                        avg += heightmap[i, k];
                        c++;
                    }

                }
            return avg / c;
        }

        private void InitializeDiamondSquare()
        {
            heightmap[0, h_size - 1] = seed;
            heightmap[h_size - 1, 0] = seed;
            heightmap[0, 0] = seed;
            heightmap[h_size - 1, h_size - 1] = seed;
        }

        private void DiamondStep(bool IsFlattening)
        {
            int step = this.step / 2;
            if (IsFlattening)
            {
                for (int i = step; i < h_size - 1; i += this.step)
                    for (int k = step; k < h_size - 1; k += this.step)
                        if (PointInFlatteningTerain(i - step, k - step))
                            heightmap[i, k] = SquareSum(i - step, k - step, this.step) + RandomFlatteningAmount();
                        else
                        {
                            /*if (SquareInFlattenedTerrain(i - step, k - step, this.step))
                                heightmap[i, k] = SquareSum(i - step, k - step, this.step) + RandomAmount();
                            else*/
                                heightmap[i, k] = startingHeightmap[i, k];
                        }
            }
            else
            {
                for (int i = step; i < h_size - 1; i += this.step)
                    for (int k = step; k < h_size - 1; k += this.step)
                        heightmap[i, k] = SquareSum(i - step, k - step, this.step) + RandomAmount();
            }

        }

        private bool PointInFlatteningTerain(int i, int k)
        {
            return i >= x_start_flat && i < (x_start_flat + size_flat) && k >= y_start_flat && k < (y_start_flat + size_flat);
        }

        private bool SquareInFlattenedTerrain(int i, int k, int step)
        {
            return PointInFlatteningTerain(i, k) || PointInFlatteningTerain(i + step, k) || PointInFlatteningTerain(i, k + step) || PointInFlatteningTerain(i + step, k + step);
        }

        private float SquareSum(int i, int k, int step)
        {
            return (heightmap[i, k] + heightmap[i + step, k] + heightmap[i, k + step] + heightmap[i + step, k + step]) / 4;
        }

        private void SquareStep(bool IsFlattening)
        {
            int step = this.step / 2;
            if (IsFlattening)
            {
                for (int i = step; i < h_size - 1; i += this.step)
                    for (int k = step; k < h_size - 1; k += this.step)
                    {
                        if (InsideMap(i, k + step))
                        {
                            if (PointInFlatteningTerain(i, k + step))
                                heightmap[i, k + step] = DiamondSum(i, k + step, step) + RandomFlatteningAmount();
                            else
                            {
                                /*if (DiamondIsInFlattenedTerrain(i, k + step, step))
                                    heightmap[i, k + step] = DiamondSum(i, k + step, step) + RandomAmount();
                                else*/
                                    heightmap[i, k + step] = startingHeightmap[i, k + step];
                            }
                        }
                        if (InsideMap(i, k - step))
                        {
                            if (PointInFlatteningTerain(i, k - step))
                                heightmap[i, k - step] = DiamondSum(i, k - step, step) + RandomFlatteningAmount();
                            else
                            {
                                /*if (DiamondIsInFlattenedTerrain(i, k - step, step))
                                    heightmap[i, k - step] = DiamondSum(i, k - step, step) + RandomAmount();
                                else*/
                                    heightmap[i, k - step] = startingHeightmap[i, k - step];
                            }
                        }
                        if (InsideMap(i + step, k))
                        {
                            if (PointInFlatteningTerain(i + step, k))
                                heightmap[i + step, k] = DiamondSum(i + step, k, step) + RandomFlatteningAmount();
                            else
                            {
                               /* if (DiamondIsInFlattenedTerrain(i + step, k, step))
                                    heightmap[i + step, k] = DiamondSum(i + step, k, step) + RandomAmount();
                                else*/
                                    heightmap[i + step, k] = startingHeightmap[i + step, k];
                            }
                        }
                        if (InsideMap(i - step, k))
                        {
                            if (PointInFlatteningTerain(i - step, k))
                                heightmap[i - step, k] = DiamondSum(i - step, k, step) + RandomFlatteningAmount();
                            else
                            {
                               /* if (DiamondIsInFlattenedTerrain(i - step, k, step))
                                    heightmap[i - step, k] = DiamondSum(i - step, k, step) + RandomAmount();
                                else*/
                                    heightmap[i - step, k] = startingHeightmap[i - step, k];
                            }
                        }
                    }
            }
            else
            {
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
        }

        private float RandomAmount()
        {
            return (float)StaticRandom.Sample() * variation * 2 - variation;
        }

        private float RandomFlatteningAmount()
        {
            return (float)StaticRandom.Sample() * terraform_var * 2 - terraform_var;
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

        private bool DiamondIsInFlattenedTerrain(int i, int k, int step)
        {
            return PointInFlatteningTerain(i, k + step) || PointInFlatteningTerain(i, k - step) || PointInFlatteningTerain(i + step, k) || PointInFlatteningTerain(i - step, k);
        }

        private bool InsideMap(int i, int k)
        {
            return i < heightmap.GetLength(0) && i >= 0 && k < heightmap.GetLength(1) && k >= 0;
        }

        private void UpdateVariation()
        {
            variation = variation * (float)Math.Pow(2f, -roughness);
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
        }
    }
}
