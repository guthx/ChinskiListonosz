using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ChinskiListonosz
{
    public class CPCrossover : CrossoverBase, ICrossover
    {
        private readonly IRandomization m_rnd;
        public CPCrossover() : base(2,2)
        {
            IsOrdered = true;
            m_rnd = RandomizationProvider.Current;
        }

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            var p1 = parents[0] as CPChromosome;
            var p2 = parents[1] as CPChromosome;

            var index1 = m_rnd.GetInt(0, p1.Length / 2);
            var index2 = m_rnd.GetInt(0, p2.Length / 2);

            var pair1 = new List<int>() { p1.GetValues()[index1], p1.GetValues()[index1 + 1] };
            var pair2 = new List<int>() { p2.GetValues()[index2], p2.GetValues()[index2 + 1] };

            var child1 = CreateChild(p1, pair2);
            var child2 = CreateChild(p2, pair1);

            return new List<IChromosome> { child1, child2 };

        }

        private IChromosome CreateChild(IChromosome parent, List<int> pair)
        {
            var parentCP = parent as CPChromosome;
            var parentList = parentCP.GetValues().ToList();
            var childList = new List<int>();
            childList.Add(pair[0]);
            childList.Add(pair[1]);

            parentList.ForEach(v =>
            {
                if (!childList.Contains(v))
                {
                    childList.Add(v);
                }
            });

            var child = new CPChromosome(childList.Count, childList.ToArray());

            return child;

        }
    }
}
