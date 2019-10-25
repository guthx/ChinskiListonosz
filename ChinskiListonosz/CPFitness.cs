using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace ChinskiListonosz
{
    public class CPFitness : IFitness
    {
        public double Evaluate (IChromosome chromosome)
        {
            var ic = chromosome as CPChromosome;
            var values = ic.GetValues();
            int vertexCount = values.Length;
  
            int graphAug = 0;
            for (var i = 0; i < vertexCount; i += 2)
            {
                int v1 = values[i];
                int v2 = values[i + 1];
                int pathLength = Globals.graph.PathMatrix[v1, v2].Distance;

                graphAug += pathLength;
            }

            return graphAug;
            
        }
    }
}
