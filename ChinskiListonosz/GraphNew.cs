using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
namespace ChinskiListonosz
{
    public class GraphNew
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
        public List<Path>[,] AssocMatrix;
        // Contains degrees of each vertex of the graph, i.e. Degrees[i] is the degree of vertex i
        public int[] Degrees;
        // Contains a matrix that specifies the path and it's cost between each pair of odd vertices
        public Path[,] PathMatrix;
        // Contains a list of all vertices of odd degrees
        public List<int> OddVertices;
        // Contains number of edges between vertices
        public int[,] EdgesMatrix;

        public GraphNew(string name)
        {
            Filename = name;

            LoadData();
            FillAssocMatrix();
            Degrees = new int[NumOfVertices + 1];
            CalculateDegrees();
            FindOddVertices();
            CreatePathMatrix();
        }

        public GraphNew() { }

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
            for (var i = 1; i < NumOfVertices + 1; i++)
                for (var j = i + 1; j < NumOfVertices + 1; j++)
                {
                    Degrees[i] += AssocMatrix[i, j].Count();
                    Degrees[j] += AssocMatrix[i, j].Count();

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
            AssocMatrix = new List<Path>[NumOfVertices + 1, NumOfVertices + 1];
            for (var i = 0; i < NumOfVertices + 1; i++)
                for (var j = 0; j < NumOfVertices + 1; j++)
                    AssocMatrix[i, j] = new List<Path>();
            //   EdgesMatrix = new int[NumOfVertices + 1, NumOfVertices + 1];
            for (var i = 0; i < NumOfEdges; i++)
            {
                int start = Data[i, 0];
                int end = Data[i, 1];
                int weight = Data[i, 2];
                var l1 = new List<int> { end };
                var l2 = new List<int> { start };
                AssocMatrix[start, end].Add(new Path(weight, l1));
                AssocMatrix[end, start].Add(new Path(weight, l2));
                AssocMatrix[start, end] = AssocMatrix[start, end].OrderBy(p => p.Distance).ToList();
                AssocMatrix[end, start] = AssocMatrix[end, start].OrderBy(p => p.Distance).ToList();
                //      EdgesMatrix[start, end]++;
                //      EdgesMatrix[end, start]++;
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
                        if (AssocMatrix[vertex, minDistV].Count > 0)
                        {
                            int alt = dist[minDistV] + AssocMatrix[vertex, minDistV].First().Distance;
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
                        path.Vertices.RemoveAt(0);
                        PathMatrix[start, source] = path;
                    }

                });
            });
        }
        
        public void AddEdgesToGraph(int[] vertices)
        {
            for (var i = 0; i < vertices.Length; i += 2)
            {
                int v1 = vertices[i];
                int v2 = vertices[i + 1];
                AssocMatrix[v2, v1].Add(PathMatrix[v2, v1]);
                AssocMatrix[v1, v2].Add(PathMatrix[v1, v2]);
                Degrees[v1]++;
                Degrees[v2]++;
              //  EdgesMatrix[v1, v2]++;
              //  EdgesMatrix[v2, v1]++;
            }
            // CalculateDegrees();
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
            int[,] count = new int[NumOfVertices + 1, NumOfVertices + 1];
            for (var i = 0; i < NumOfVertices + 1; i++)
                for (var j = 0; j < NumOfVertices + 1; j++)
                    count[i, j] = AssocMatrix[i, j].Count;
           // int[,] edgesMatrix = (int[,])EdgesMatrix.Clone();

            while (stack.Count > 0 || degrees[currentVertex] > 0)
            {
                if (degrees[currentVertex] == 0)
                {
                    path.Add(currentVertex);
                    currentVertex = stack.Pop();
                }
                else
                {
                    stack.Push(currentVertex);
                    for (var i = 1; i < NumOfVertices + 1; i++)
                    {
                        if (count[i, currentVertex] != 0)
                        {
                            count[i, currentVertex]--;
                            count[currentVertex, i]--;
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
            /*
            var path = new List<Path>();
            int currentVertex = startingVertex;
            bool pathNotDone = true;
            int bestPath;
            int maxDegree = 0;
            while (pathNotDone)
            {
                maxDegree = 0;
                bestPath = 0;
                for (var i=0; i<NumOfVertices+1; i++)
                {
                    if(AssocMatrix[currentVertex, i].Count > 0)
                    {
                        if(Degrees[i] > maxDegree)
                        {
                            maxDegree = Degrees[i];
                            bestPath = i;
                        }
                            
                    }               
                }
                if (maxDegree == 0)
                    break;
                else
                {
                    path.Add(AssocMatrix[currentVertex, bestPath].First());
                    AssocMatrix[currentVertex, bestPath].RemoveAt(0);
                    AssocMatrix[bestPath, currentVertex].RemoveAt(0);
                    Degrees[currentVertex]--;
                    Degrees[bestPath]--;
                    currentVertex = bestPath;
                }
            }
            return path;
            */
        }

        public string PathToString(List<Path> path)
        {
            var pathString = new string("");
            path.ForEach(p =>
            {
                p.Vertices.ForEach(v => pathString = string.Concat(pathString, v.ToString()));
            });

            return pathString;
        }

        public List<Path> FindShortesPath(List<int> eulerPath)
        {
            var shortestPath = new List<Path>();
            var pathArray = eulerPath.ToArray();
            for(var i=0; i<pathArray.Length-1; i++)
            {
                int v1 = pathArray[i];
                int v2 = pathArray[i + 1];
                var path = AssocMatrix[v1, v2].First();
                AssocMatrix[v1, v2].RemoveAt(0);
                AssocMatrix[v2, v1].RemoveAt(0);
                shortestPath.Add(path);
            }

            return shortestPath;
        }

        public int GetPathLength(List<Path> path)
        {
            int length=0;
            path.ForEach(p => length += p.Distance);
            return length;
        }
        /*
        public List<int> FindShortestPath(List<int> eulerianPath)
        {
            var path = new List<int>();

            eulerianPath.ForEach(vertex =>
            {
                int prevVertex = path.LastOrDefault();
                if (AssocMatrix[vertex, prevVertex] != 0 || prevVertex == 0)
                    if (PathMatrix[prevVertex, vertex] != null && PathMatrix[prevVertex, vertex].Distance < AssocMatrix[vertex, prevVertex])
                    {
                        var reroute = PathMatrix[prevVertex, vertex].Vertices.Select(v => v).ToList();
                        reroute.RemoveAt(0);
                        reroute.ForEach(v => path.Add(v));
                    }
                    else
                        path.Add(vertex);
                else
                {
                    var reroute = PathMatrix[prevVertex, vertex].Vertices.Select(v => v).ToList();
                    reroute.RemoveAt(0);
                    reroute.ForEach(v => path.Add(v));
                }
            });

            return path;
        }
        
        public GraphNew CopyGraphNew()
        {
            var graph = new GraphNew();
            graph.NumOfEdges = NumOfEdges;
            graph.NumOfOddVertices = NumOfOddVertices;
            graph.NumOfVertices = NumOfVertices;
            graph.OddVertices = OddVertices.Select(v => v).ToList();
            graph.AssocMatrix = (int[,])AssocMatrix.Clone();
            graph.Data = (int[,])Data.Clone();
            graph.Degrees = (int[])Degrees.Clone();
            graph.PathMatrix = (Path[,])PathMatrix.Clone();
            graph.EdgesMatrix = (int[,])EdgesMatrix.Clone();

            return graph;
        }*/
    }
}
