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
            
            Globals.graph = new Graph("Data/data2.txt");
            int[] startValues = new int[Globals.graph.NumOfOddVertices];
            startValues = Globals.graph.OddVertices.ToArray();
            int populationSize = 500;
            var chromosome = new CPChromosome (startValues.Length, startValues, populationSize);
            var population = new Population(populationSize, populationSize, chromosome);
            var fitness = new CPFitness();
            var selection = new EliteSelection();
            var crossover = new CPCrossover();
            var mutation = new CPMutation();
            var termination = new FitnessStagnationTermination(50);
            var ga = new GeneticAlgorithm(
                    population,
                    fitness,
                    selection,
                    crossover,
                    mutation);
            ga.Termination = termination;
            ga.MutationProbability = 0.1f;
            var latestFitness = int.MinValue;
            ga.GenerationRan += (sender, e) =>
            {
                var bestChromosome = ga.BestChromosome as CPChromosome;
                var bestFitness = (int)-bestChromosome.Fitness.Value;
                
                if(bestFitness != latestFitness)
                {
                    latestFitness = bestFitness;
                    var phenotype = bestChromosome.GetValues();
                    Console.WriteLine("Generation {0}: {1}", ga.GenerationsNumber, bestFitness);
                }
            };
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
