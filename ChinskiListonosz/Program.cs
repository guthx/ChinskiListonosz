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
            
            Globals.graph = new Graph("Data/data.txt");
            int[] startValues = new int[Globals.graph.NumOfOddVertices];
            startValues = Globals.graph.OddVertices.ToArray();
            int populationSize = 10;
            var chromosome = new CPChromosome (startValues.Length, startValues, populationSize);
            var population = new Population(populationSize, populationSize, chromosome);
            var fitness = new CPFitness();
            var selection = new RouletteWheelSelection();
            var crossover = new OnePointCrossover();
            var mutation = new DisplacementMutation();
            var termination = new FitnessStagnationTermination(50);
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
            
            Globals.graph.AddEdgesToGraph(phenotype);
            var eulerianPath = Globals.graph.FindEulerianPath(1);
            var shortestPath = Globals.graph.FindShortesPath(eulerianPath);
            var pathLength = Globals.graph.GetPathLength(shortestPath);
            Console.WriteLine(Globals.graph.PathToString(shortestPath));
            Console.WriteLine(pathLength);
            

          
        }

    }
}
