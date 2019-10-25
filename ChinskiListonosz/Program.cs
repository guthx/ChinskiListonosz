using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GeneticSharp.Domain.Chromosomes;
using ExtensionMethods;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain;
namespace ChinskiListonosz
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Globals.graph = new Graph("Data/data.txt");
            int[] startValues = new int[Globals.graph.NumOfOddVertices];
            startValues = Globals.graph.OddVertices.ToArray();

            var chromosome = new CPChromosome (startValues.Length, startValues);
            var population = new Population(50, 50, chromosome);
            var fitness = new CPFitness();
            var selection = new RouletteWheelSelection();
            var crossover = new OnePointCrossover();
            var mutation = new DisplacementMutation();
            var termination = new FitnessStagnationTermination(30);
            var ga = new GeneticAlgorithm(
                    population,
                    fitness,
                    selection,
                    crossover,
                    mutation);
            ga.Termination = termination;
            ga.Start();
            var bestChromosome = ga.BestChromosome as CPChromosome;
            var phenotype = bestChromosome.GetValues();
            var lowestAug = bestChromosome.Fitness.GetValueOrDefault();

            var augmentedGraph = Globals.graph.CopyGraph();
            augmentedGraph.AddEdgesToGraph(new int[] { 2, 3, 4, 5 });
            var path = augmentedGraph.FindEulerianPath(2);
            var realPath = Globals.graph.FindShortestPath(path);
            realPath.ForEach(v => Console.Write(v-1));
            int cost = 0;
            var realPathArray = realPath.ToArray();
            for(var i=0; i<realPathArray.Length-1; i++)
            {
                cost += Globals.graph.AssocMatrix[realPathArray[i], realPathArray[i+1]];
            }
            Console.WriteLine("\n");
            Console.WriteLine(cost);

          //  Console.WriteLine(realPathString);
            */
            var test = new GraphNew("Data/data.txt");
            test.AddEdgesToGraph(new int[] { 2, 3, 4, 5 });
            var eulerianPath = test.FindEulerianPath(2);
            var shortestPath = test.FindShortesPath(eulerianPath);
            var pathLength = test.GetPathLength(shortestPath);
            Console.WriteLine(test.PathToString(shortestPath));
            Console.WriteLine(pathLength);

        }
    }
}
