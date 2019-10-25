using GeneticSharp.Domain.Chromosomes;
using System;
using System.Collections.Generic;
using System.Text;


namespace ChinskiListonosz
{
    public class CPChromosome : ChromosomeBase, IChromosome
    {
        private readonly int[] m_geneValues;
        private readonly int m_length;

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
        public CPChromosome(int length, int[] geneValues) : base(length)
        {
            m_geneValues = RandomizeGenes(geneValues);
            m_length = length;
            CreateGenes();
        }

        public override IChromosome CreateNew()
        {
            return new CPChromosome(m_length, m_geneValues);
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
