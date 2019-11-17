using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using GeneticSharp.Domain.Fitnesses;

namespace ChinskiListonosz
{
    public class CPSetup
    {
        private IChromosome Chromosome { get; set; }
        private IPopulation Population { get; set; }
        private IFitness Fitness { get; set; }
        private ISelection Selection { get; set; }
        private ICrossover Crossover { get; set; }
        private IMutation Mutation { get; set; }
        private ITermination Termination { get; set; }
        private GeneticAlgorithm GA { get; set; }
        public static Graph Graph { get; set; }

        public CPSetup(IPopulation population, ISelection selection, ICrossover crossover, IMutation mutation, ITermination termination,
            string filename="")
        {
            if (String.IsNullOrEmpty(filename))
            {
                Console.WriteLine("Podaj nazwę pliku z grafem: ");
                var builder = new StringBuilder();
                builder.Append("Data/");
                builder.Append(Console.ReadLine());
                filename = builder.ToString();
            }
            Graph = new Graph(@filename);
            int[] startValues = Graph.OddVertices.ToArray();
            Fitness = new CPFitness();
            Chromosome = new CPChromosome(startValues.Length, startValues, population.MaxSize);
            Population = population;
            Selection = selection;
            Crossover = crossover;
            Mutation = mutation;
            Termination = termination;

            GA = new GeneticAlgorithm(Population, Fitness, Selection, Crossover, Mutation);
            GA.Termination = Termination;
        }

        public CPSetup(int populationSize, string filename = "")
        {
            if (String.IsNullOrEmpty(filename))
            {
                Console.WriteLine("Podaj nazwę pliku z grafem: ");
                var builder = new StringBuilder();
                builder.Append("Data/");
                builder.Append(Console.ReadLine());
                filename = builder.ToString();
            }
            Graph = new Graph(@filename);
            int[] startValues = Graph.OddVertices.ToArray();
            Fitness = new CPFitness();
            Chromosome = new CPChromosome(startValues.Length, startValues, populationSize);
            Population = new Population(populationSize, populationSize, Chromosome);
            Selection = new EliteSelection();
            Crossover = new CPCrossover();
            Mutation = new CPMutation();
            Termination = new FitnessStagnationTermination(50);

            GA = new GeneticAlgorithm(Population, Fitness, Selection, Crossover, Mutation);
            GA.Termination = Termination;
        }

        private void PrintShortestPath(int startingVertex=1)
        {
            var eulerianPath = Graph.FindEulerianPath(startingVertex);
            var shortestPath = Graph.FindShortestPath(eulerianPath);
            var pathLength = Graph.GetPathLength(shortestPath);
            Console.WriteLine("\nDługość najkrótszej ścieżki: {0}", pathLength);
            Console.WriteLine("Najkrótsza ścieżka:");
            Console.WriteLine(Graph.PathToString(shortestPath));
        }
        public void Run()
        {
            if (Graph.IsEulerian())
            {
                Console.WriteLine("Graf jest eulerowski, wiec algorytm genetyczny nie jest potrzebny");
            }
            else
            {
                var latestFitness = int.MinValue;
                GA.GenerationRan += (sender, e) =>
                {
                    var bestInd = GA.BestChromosome as CPChromosome;
                    var bestFitness = (int)-bestInd.Fitness.Value;

                    if (bestFitness != latestFitness)
                    {
                        latestFitness = bestFitness;
                        Console.WriteLine("Generation {0}: {1}", GA.GenerationsNumber, bestFitness);
                    }
                };
                GA.Start();
                var bestChromosome = GA.BestChromosome as CPChromosome;
                var phenotype = bestChromosome.GetValues();
                Graph.AddEdgesToGraph(phenotype);
            }

            PrintShortestPath();
        }

    }
}
