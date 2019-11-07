using GeneticSharp.Domain.Chromosomes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;


namespace ChinskiListonosz
{
    public class CPChromosome : ChromosomeBase, IChromosome
    {
        private readonly int[] m_geneValues;
        private readonly int m_length;
        private int m_population;
  //      private readonly int m_startIndex;
  //      private readonly int m_populationSize;
 //       private float m_fIndex;
//        private float m_step;

        private int Factorial(int n)
        {
            if (n <= 1)
                return 1;
            else
                return Factorial(n - 1) * n;
           
        }
        /*
        private int[] SetGenes(int[] geneValues)
        {
            int n = geneValues.Length;

            int sIndex = m_startIndex;
            int[] sequence = new int[n];
            while(sIndex > 0)
            {
                if (sIndex >= Factorial(n - 1))
                {
                    sIndex -= Factorial(n - 1);
                    sequence[n - 1]++;
                }
                else
                    n--;
            }
            var sequenceL = sequence.ToList();
            sequenceL.Reverse();
            sequence = sequenceL.ToArray();
            n = geneValues.Length;

            int[] list = (int[])geneValues.Clone();
            int[] permuted = new int[n];
            bool[] set = new bool[n];

            for (int i = 0; i < n; i++)
            {
                int s = sequence[i];
                int remainingPosition = 0;
                int index;

                // Find the s'th position in the permuted list that has not been set yet.
                for (index = 0; index < n; index++)
                {
                    if (!set[index])
                    {
                        if (remainingPosition == s)
                            break;

                        remainingPosition++;
                    }
                }

                permuted[index] = list[i];
                set[index] = true;
            }
            return permuted;
        }*/
        private int[] RandomizeGenes(int[] geneValues)
        {
            Random rnd = new Random();
            int amount = geneValues.Length;
            var vertexList = new List<int>();
            vertexList = geneValues.ToList();
            vertexList.Sort();
            /*
            for(var i=0; i<amount; i++)
            {
                int swap = rnd.Next(0, 2);
                int position = rnd.Next(0, amount);
                if(swap != 0)
                {
                    int tmp = geneValues[i];
                    geneValues[i] = geneValues[position];
                    geneValues[position] = tmp;
                }
            }*/
            for(var i=0; i<amount; i++)
            {
                int vertexIndex = rnd.Next(0, vertexList.Count);
                geneValues[i] = vertexList[vertexIndex];
                vertexList.RemoveAt(vertexIndex);
            }

            return geneValues;
        }
        public CPChromosome(int length, int[] geneValues, int population=0) : base(length)
        {
            m_length = length;
            //     m_startIndex = startIndex;
            //     m_populationSize = populationSize;
            //     m_fIndex = fIndex;
            //     m_step = (float)m_length / m_populationSize;
            m_geneValues = geneValues;
            m_population = population-1;
            CreateGenes();
        }

        public override IChromosome CreateNew()
        {
            //   m_fIndex += m_step;
            //   int startIndex = ((int)Math.Round(m_fIndex))%Factorial(m_length);
            int[] geneValues = new int[m_length];
            if (m_population > 0)
            {
                geneValues = (int[])RandomizeGenes(m_geneValues).Clone();
                m_population--;
            }
            else
            {
                geneValues = (int[])m_geneValues.Clone();
            }
            return new CPChromosome(m_length, geneValues, m_population);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(m_geneValues[geneIndex]);
        }

        public int[] GetValues()
        {
            return m_geneValues;
        }

    }
}
