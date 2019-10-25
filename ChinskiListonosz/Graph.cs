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
        // Specifies number of edges in the graph
        public int NumOfEdges;
        // Specifies number of vertices in the graph
        public int NumOfVertices;
        // Specifies number of vertices of odd degrees in the graph
        public int NumOfOddVertices;
        // Contains raw data read from given file
        public int[,] Data;
        // Contains association matrix for the graph
        // AssocMatrix[i, j] specifies the length of the path between vertices i and j, assuming the path exists (otherwise it's 0)
        public int[,] AssocMatrix;
        // Contains degrees of each vertex of the graph, i.e. Degrees[i] is the degree of vertex i
        public int[] Degrees;
        // Contains a matrix that specifies the path and it's cost between each pair of odd vertices
        public Path[,] PathMatrix;
        // Contains a list of all vertices of odd degrees
        public List<int> OddVertices;
        
        // Function loads data from given file and puts it into the Data array
        private void LoadData()
        {
            var reader = new StreamReader(@Filename);
            var lineCount = File.ReadLines(@Filename).Count();
            int i = 0;
            Data = new int[lineCount, 3];
            NumOfEdges = lineCount;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(' ');
                int k;
                for (k = 0; k < values.Length; k++)
                {
                    Data[i, k] = int.Parse(values[k]);
                }
                i++;
            }
        }

        // Function calculates degrees of each vertex and puts them into Degrees array
        private void CalculateDegrees()
        {
            int max = 0;
            for (i = 0; i < NumOfEdges; i++)
            {
                if (Data[i, 0] > max)
                    max = Data[i, 0];
                if (Data[i, 1] > max)
                    max = Data[i, 1];
            }
            NumOfVertices = max;
            Degrees = new int[NumOfVertices + 1];
            for (i = 0; i < NumOfEdges; i++)
            {
                int v1 = Data[i, 0];
                int v2 = Data[i, 1];
                Degrees[v1]++;
                Degrees[v2]++;
            }
        }

        // Function creates the association matrix and saves it in AssocMatrix array
        private void FillAssocMatrix()
        {
            AssocMatrix = new int[NumOfVertices + 1, NumOfVertices + 1];
            for (i = 0; i < NumOfEdges; i++)
            {
                int start = Data[i, 0];
                int end = Data[i, 1];
                int weight = Data[i, 2];
                AssocMatrix[start, end] = weight;
                AssocMatrix[end, start] = weight;
            }
        }

        // Function finds all vertices of odd degrees and puts them into OddVertices list
        private void FindOddVertices()
        {
            OddVertices = new List<int>();
            for (i = 1; i < NumOfVertices + 1; i++)
            {
                if (Degrees[i] % 2 == 1)
                {
                    OddVertices.Add(i);
                }
            }
            NumOfOddVertices = OddVertices.Count();
        }
        
        // Function creates the Path matrix using Dijskstra's algorithm and puts it into PathMatrix array
        private void CreatePathMatrix()
        {
            PathMatrix = new Path[NumOfOddVertices + 1, NumOfOddVertices + 1];

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

        public Graph(string name)
        {
            Filename = name;
           
            LoadData();
            CalculateDegrees();
            FillAssocMatrix();
            FindOddVertices();
            CreatePathMatrix();                         
        }

    }
}
