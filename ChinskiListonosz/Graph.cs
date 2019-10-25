using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
namespace ChinskiListonosz
{
    public class Graph
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

        public Graph(string name)
        {
            Filename = name;

            LoadData();
            FillAssocMatrix();
            Degrees = new int[NumOfVertices + 1];
            CalculateDegrees();
            FindOddVertices();
            CreatePathMatrix();
        }

        public Graph() { }
        
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
            /*
            Degrees = new int[NumOfVertices + 1];
            for (var i = 0; i < NumOfEdges; i++)
            {
                int v1 = Data[i, 0];
                int v2 = Data[i, 1];
                Degrees[v1]++;
                Degrees[v2]++;
            }
            */
            Degrees = Degrees.ToList().Select(v => 0).ToArray();
            for(var i=1; i < NumOfVertices+1; i++)
                for(var j=i+1; j<NumOfVertices+1; j++)
                {
                    if(AssocMatrix[i, j] != 0)
                    {
                        Degrees[i]++;
                        Degrees[j]++;
                    }
                    
                }

        }

        // Function creates the association matrix and saves it in AssocMatrix array
        private void FillAssocMatrix()
        {
            int max = 0;
            for (var i = 0; i < NumOfEdges; i++)
            {
                if (Data[i, 0] > max)
                    max = Data[i, 0];
                if (Data[i, 1] > max)
                    max = Data[i, 1];
            }
            NumOfVertices = max;
            AssocMatrix = new int[NumOfVertices + 1, NumOfVertices + 1];
            for (var i = 0; i < NumOfEdges; i++)
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
            for (var i = 1; i < NumOfVertices + 1; i++)
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
            PathMatrix = new Path[NumOfVertices + 1, NumOfVertices + 1];

            OddVertices.ForEach(source =>
            {
                int[] dist = new int[NumOfVertices + 1];
                int[] prev = new int[NumOfVertices + 1];
                List<int> Q = new List<int>();
                for (var i = 1; i < NumOfVertices + 1; i++)
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
                        PathMatrix[start, source] = path;
                    }

                });
            });
        }

        public void AddEdgesToGraph(int[] vertices)
        {
            for(var i = 0; i<vertices.Length; i+=2)
            {
                int v1 = vertices[i];
                int v2 = vertices[i + 1];
                AssocMatrix[v1, v2] = PathMatrix[v1, v2].Distance;
                AssocMatrix[v2, v1] = PathMatrix[v1, v2].Distance;
            }
            CalculateDegrees();
        }

        private bool IsGraphEulerian()
        {
            for (var i = 1; i < NumOfVertices + 1; i++)
                if (Degrees[i] % 2 != 0)
                    return false;

            return true;
        }

        public List<int> FindEulerianPath(int startingVertex)
        {
            if (!IsGraphEulerian())
                throw new Exception("Graf nie jest eulerowski");
            var stack = new Stack<int>();
            var path = new List<int>();
            int currentVertex = startingVertex;
            int[] degrees = (int[])Degrees.Clone();
            int[,] assocMatrix = (int[,])AssocMatrix.Clone();

            while(stack.Count > 0 || degrees[currentVertex] > 0)
            {
                if(degrees[currentVertex] == 0)
                {
                    path.Add(currentVertex);
                    currentVertex = stack.Pop();
                } else
                {
                    stack.Push(currentVertex);
                    for(var i=1; i<NumOfVertices+1; i++)
                    {
                        if(assocMatrix[i, currentVertex] != 0)
                        {
                            assocMatrix[i, currentVertex] = 0;
                            assocMatrix[currentVertex, i] = 0;
                            degrees[i]--;
                            degrees[currentVertex]--;
                            currentVertex = i;
                            break;
                        }
                    }
                }
            }
            path.Add(startingVertex);
          //  path.Reverse();
            return path;
        }

        public List<int> FindShortestPath(List<int> eulerianPath)
        {
            var path = new List<int>();

            eulerianPath.ForEach(vertex =>
            {
                int prevVertex = path.LastOrDefault();
                if(AssocMatrix[vertex, prevVertex] != 0 || prevVertex == 0)
                    path.Add(vertex);
                else
                {
                    var reroute = PathMatrix[prevVertex, vertex].Vertices.Select(v=>v).ToList();
                    reroute.RemoveAt(0);
                    reroute.ForEach(v => path.Add(v));
                }
            });

            return path;
        }

        public Graph CopyGraph()
        {
            var graph = new Graph();
            graph.NumOfEdges = NumOfEdges;
            graph.NumOfOddVertices = NumOfOddVertices;
            graph.NumOfVertices = NumOfVertices;
            graph.OddVertices = OddVertices.Select(v => v).ToList();
            graph.AssocMatrix = (int[,])AssocMatrix.Clone();
            graph.Data = (int[,])Data.Clone();
            graph.Degrees = (int[])Degrees.Clone();
            graph.PathMatrix = (Path[,])PathMatrix.Clone();

            return graph;
        }
    }
}
