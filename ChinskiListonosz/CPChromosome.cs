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
        private readonly int m_startIndex;

        private int Factorial(int n)
        {
            if (n <= 1)
                return 1;
            else
                return Factorial(n - 1) * n;
           
        }
        
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
        }
        private int[] RandomizeGenes(int[] geneValues)
        {
            Random rnd = new Random();
            int amount = geneValues.Length;

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
            }
            return geneValues;
        }
        public CPChromosome(int length, int[] geneValues, int startIndex = 0) : base(length)
        {
            m_length = length;
            m_startIndex = startIndex;
            m_geneValues = (int[])SetGenes(geneValues).Clone();
            CreateGenes();
        }

        public override IChromosome CreateNew()
        {
            return new CPChromosome(m_length, m_geneValues, (m_startIndex+1)%Factorial(m_length));
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
