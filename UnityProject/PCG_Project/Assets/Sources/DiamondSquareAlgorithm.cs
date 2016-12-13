using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerrainCreation
{
    class DiamondSquareAlgorithm
    {
        private float[,] heightmap;
        private int h_size;
        private int step;
        private int iterations;
        private float seed;
        private float variation;
        private float roughness;

        private int x_start_flat;
        private int y_start_flat;
        private int size_flat;
        private float terraform_var;


        /// <summary>
        /// Calculates the heightmap of the terrain using the DiamondSquare algorithm
        /// </summary>
        /// <param name="size">The size of the terrain</param>
        /// <param name="seed">The starting value of the 4 corners of the heightmap</param>
        /// <param name="variation">The variation</param>
        /// <param name="roughness">The roughness</param>
        /// <param name="x_start_flat">The x coordinate of the flat square</param>
        /// <param name="y_start_flat">The y coordinate of the flat square</param>
        /// <param name="size_flat">The size of the flat square</param>
        /// <param name="terraform_var">The variation of the terraformed square</param>
        /// <returns>The heightmap</returns>
        public float[,] DiamondSquareLoop(int size, float seed, float variation, float roughness, int x_start_flat, int y_start_flat, int size_flat, float terraform_var)
        {
            this.x_start_flat = x_start_flat;
            this.y_start_flat = y_start_flat;
            this.size_flat = size_flat;
            this.terraform_var = terraform_var;
            heightmap = new float[size, size];
            h_size = size;
            this.seed = seed;
            this.variation = variation;
            this.roughness = roughness;
            iterations = (int)Math.Log(h_size - 1, 2);
            step = h_size - 1;
            InitializeDiamondSquare();
            for (int i = 0; i < iterations; i++, step /= 2)
            {
                DiamondStep();

                SquareStep();
                UpdateVariation();
            }
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

        private void DiamondStep()
        {
            int step = this.step / 2;
            for (int i = step; i < h_size - 1; i += this.step)
                for (int k = step; k < h_size - 1; k += this.step)
                    heightmap[i, k] = SquareSum(i - step, k - step, this.step) + RandomAmount(i - step, k - step);
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
                        heightmap[i, k + step] = DiamondSum(i, k + step, step) + RandomAmount(i, k + step);
                    }
                    if (InsideMap(i, k - step))
                    {
                        heightmap[i, k - step] = DiamondSum(i, k - step, step) + RandomAmount(i, k - step);
                    }
                    if (InsideMap(i + step, k))
                    {
                        heightmap[i + step, k] = DiamondSum(i + step, k, step) + RandomAmount(i + step, k);
                    }
                    if (InsideMap(i - step, k))
                    {
                        heightmap[i - step, k] = DiamondSum(i - step, k, step) + RandomAmount(i - step, k);
                    }
                }
        }

        private float RandomAmount(int i, int k)
        {
            if (i >= x_start_flat && i < (x_start_flat + size_flat) && k >= y_start_flat && k < (y_start_flat + size_flat))
                return (float)StaticRandom.Sample() * terraform_var * 2 - terraform_var;
            else

                return (float)StaticRandom.Sample() * variation * 2 - variation;
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
