using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChinskiListonosz
{
    public class CPMutation : MutationBase, IMutation
    {
        private readonly IRandomization m_rnd;

        public CPMutation()
        {
            m_rnd = RandomizationProvider.Current;
        }
        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            var cpChromosome = chromosome as CPChromosome;

            if(m_rnd.GetDouble() <= probability)
            {
                var i = m_rnd.GetInt(0, chromosome.Length);
                var j = m_rnd.GetInt(0, chromosome.Length);
                var v1 = cpChromosome.GetGene(i);
                var v2 = cpChromosome.GetGene(j);
                cpChromosome.ReplaceGene(i, v2);
                cpChromosome.ReplaceGene(j, v1);
            }
        }
    }
}
