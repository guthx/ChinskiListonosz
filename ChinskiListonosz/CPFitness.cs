using System;
using System.Collections.Generic;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace ChinskiListonosz
{
    public class CPFitness : IFitness
    {
        public double Evaluate (IChromosome chromosome)
        {
            var ic = chromosome as FloatingPointChromosome;
            var floatValues = ic.ToFloatingPoints();
            int[] values = new int[floatValues.Length];
            return 0;
            
        }
    }
}
