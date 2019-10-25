using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
namespace ChinskiListonosz
{
    class Graph
    {
        private static string Filename;
        public int NumOfEdges;
        public int NumOfVertices;
        public int NumOfOddVertices;
        public int[,] Data;
        public int[,] AssocMatrix;
        public int[] Degrees;
        public Path[,] PathMatrix;
        public List<int> OddVertices;
        
        private void LoadData()
        {

        }

        public Graph(string name)
        {
            Filename = name;
            OddVertices = new List<int>();
            // Otworzenie pliku z danymi, inicjalizacja tablicy
            var reader = new StreamReader(@Filename);
            var lineCount = File.ReadLines(@Filename).Count();
            int i = 0;
            Data = new int[lineCount, 3];
            NumOfEdges = lineCount;

            // Wypełnienie tablicy z krawędziami danymi z pliku
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(' ');             
                int k;
                for(k=0; k<values.Length; k++)
                {
                    Data[i, k] = int.Parse(values[k]);
                }               
                i++;
            }

            int max=0;
            // Sprawdzenie ilości wierzchołków
            for (i=0; i<NumOfEdges; i++)
            {
                if (Data[i,0] > max)
                    max = Data[i,0];
                if (Data[i,1] > max)
                    max = Data[i,1];
            }
            NumOfVertices = max;
            Degrees = new int[NumOfVertices + 1];
            for(i=0; i<NumOfEdges; i++)
            {
                int v1 = Data[i, 0];
                int v2 = Data[i, 1];
                Degrees[v1]++;
                Degrees[v2]++;
            }

            AssocMatrix = new int[NumOfVertices + 1, NumOfVertices + 1];
            for(i=0; i<NumOfEdges; i++)
            {
                int start = Data[i, 0];
                int end = Data[i, 1];
                int weight = Data[i, 2];
                AssocMatrix[start, end] = weight;
                AssocMatrix[end, start] = weight;
            }

            for (i = 1; i < NumOfVertices + 1; i++)
            {
                if (Degrees[i] % 2 == 1)
                {
                    OddVertices.Add(i);
                }
            }
            NumOfOddVertices = OddVertices.Count();
            PathMatrix = new Path[NumOfOddVertices + 1, NumOfOddVertices + 1];

            // algorytm Djikstry - znalezienie najkrotszej drogi dla kazdej z par wierzcholkow nieparzystych
            OddVertices.ForEach(source =>
            {
                int[] dist = new int[NumOfVertices + 1];
                int[] prev = new int[NumOfVertices + 1];
                List<int> Q = new List<int>();
                for (i = 1; i < NumOfVertices + 1; i++)
                {
                    dist[i] = int.MaxValue;
                    prev[i] = -1;
                    Q.Add(i);
                }
                dist[source] = 0;

                while (Q.Count > 0)
                {
                    int minDistV = Q.First();
                    Q.ForEach(vertex =>
                    {
                        if (dist[vertex] < dist[minDistV])
                            minDistV = vertex;
                    });
                    Q.Remove(minDistV);

                    Q.ForEach(vertex =>
                    {
                        if (AssocMatrix[vertex, minDistV] > 0)
                        {
                            int alt = dist[minDistV] + AssocMatrix[vertex, minDistV];
                            if (alt < dist[vertex])
                            {
                                dist[vertex] = alt;
                                prev[vertex] = minDistV;
                            }
                        }
                    });

                }
                OddVertices.ForEach(target =>
                {
                    var vertices = new List<int>();
                    var start = target;
                    if (prev[target] != -1)
                    {
                        var distance = dist[target];
                        while (target != -1)
                        {
                            vertices.Add(target);
                            target = prev[target];
                        }
                        var path = new Path(distance, vertices);
                        PathMatrix[source, start] = path;
                        PathMatrix[start, source] = path;
                    }

                });
            });
            

            


            
        }

    }
}
