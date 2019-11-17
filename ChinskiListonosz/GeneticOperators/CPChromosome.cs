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

        private int[] RandomizeGenes(int[] geneValues)
        {
            Random rnd = new Random();
            int amount = geneValues.Length;
            var vertexList = new List<int>();
            vertexList = geneValues.ToList();
            vertexList.Sort();

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
            m_geneValues = geneValues;
            m_population = population-1;
            CreateGenes();
        }

        public override IChromosome CreateNew()
        {
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
