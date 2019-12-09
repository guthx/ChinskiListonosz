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
using System.Timers;
using System.Diagnostics;

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
        private string GraphFilename { get; set; }
        private string StatDir { get; set; }
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
                GraphFilename = @filename;
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
                GraphFilename = filename;

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

            var ticks = DateTime.Now.Ticks;
            StatDir = "stats/" + ticks.ToString();
            System.IO.Directory.CreateDirectory(StatDir);
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

                var genStatWriter = new System.IO.StreamWriter(StatDir + "/genStats.csv");
                genStatWriter.WriteLine("generation,best fitness");
                var latestFitness = int.MinValue;
                GA.GenerationRan += (sender, e) =>
                {
                    
                    var bestInd = GA.BestChromosome as CPChromosome;
                    var bestFitness = (int)-bestInd.Fitness.Value;
                    genStatWriter.WriteLine(bestFitness.ToString() + "," + GA.GenerationsNumber.ToString());
                    
                    if (bestFitness != latestFitness)
                    {
                        latestFitness = bestFitness;
                        Console.WriteLine("Generation {0}: {1}", GA.GenerationsNumber, bestFitness);
                    }
                };
                var timer = new Stopwatch();
                timer.Start();
                GA.Start();
                timer.Stop();
                genStatWriter.Close();
                var time = timer.Elapsed;
                var runStatWriter = new System.IO.StreamWriter(StatDir + "/runStats.txt");
                var bestChromosome = GA.BestChromosome as CPChromosome;
                var phenotype = bestChromosome.GetValues();

                Graph.AddEdgesToGraph(phenotype);

                var eulerianPath = Graph.FindEulerianPath(1);
                var shortestPath = Graph.FindShortestPath(eulerianPath);
                var pathLength = Graph.GetPathLength(shortestPath);

                runStatWriter.WriteLine("Plik grafu: " + GraphFilename);
                runStatWriter.WriteLine("Ilość generacji: " + GA.GenerationsNumber.ToString());
                runStatWriter.WriteLine("Czas przetwarzania algorytmu genetycznego: " + time.TotalSeconds.ToString() + "s");
                runStatWriter.WriteLine("Prawdopodobieństwo krzyżowania: " + GA.CrossoverProbability.ToString());
                runStatWriter.WriteLine("Prawdopodobieństwo mutacji: " + GA.MutationProbability.ToString());
                runStatWriter.WriteLine("Najlepsze przystosowanie: " + bestChromosome.Fitness.ToString());
                runStatWriter.WriteLine("Najlepsza ścieżka: " + Graph.PathToString(shortestPath));
                runStatWriter.WriteLine("Długość najlepszej ścieżki: " + pathLength.ToString());

                runStatWriter.Close();

                Console.WriteLine("\nDługość najkrótszej ścieżki: {0}", pathLength);
                Console.WriteLine("Najkrótsza ścieżka:");
                Console.WriteLine(Graph.PathToString(shortestPath));
            }

           // PrintShortestPath();
        }

    }
}
