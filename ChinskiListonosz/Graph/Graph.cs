using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        // AssocMatrix[i, j] cointains a list of all connections between vertices i and j
        public List<Path>[,] AssocMatrix;
        // Contains degrees of each vertex of the graph, i.e. Degrees[i] is the degree of vertex i
        public int[] Degrees;
        // Contains a matrix that specifies the path and it's cost between each pair of odd vertices
        public Path[,] PathMatrix;
        // Contains a list of all vertices of odd degrees
        public List<int> OddVertices;


        // Constructs a graph and it's path matrix from a file
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

        // Creates an empty Graph object
        public Graph() { }

        // Function loads data from given file and puts it into the Data array
        private void LoadData()
        {
            int dataFormat=0;
            var reader = new StreamReader(@Filename);
            var lineCount = File.ReadLines(@Filename).Count();
            int i = 0;
            Data = new int[lineCount, 3];
            NumOfEdges = lineCount;
            if (reader.Peek() == '<')
                dataFormat = 1;
            else if (Regex.IsMatch(reader.Peek().ToString(), "[0-9]"))
                dataFormat = 2;
            else throw new Exception("Niepoprawny format danych");

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if(dataFormat == 2)
                {
                    var values = line.Split(' ');
                    if(values.Length != 3)
                    {
                        throw new Exception("Niepoprawny format danych");
                    }
                    for(var k = 0; k < values.Length; k++)
                    {
                        Data[i, k] = int.Parse(values[k]);
                    }
                } 
                else if (dataFormat == 1)
                {
                    string pattern = "<[0-9]+>";
                    StringBuilder valuesBuilder = new StringBuilder();
                    if (Regex.Matches(line, pattern).Count != 3)
                    {
                        throw new Exception("Niepoprawny format danych");
                    }
                    foreach (Match match in Regex.Matches(line, pattern))
                        valuesBuilder.Append(match.Value.Trim('<').Replace('>', ' '));
                    var valuesString = valuesBuilder.ToString().Trim(' ');
                    var values = valuesString.Split(' ');
                    for (var k = 0; k < values.Length; k++)
                    {
                        Data[i, k] = int.Parse(values[k]);
                    }
                }
                i++;
            }
        }

        // Function calculates degrees of each vertex and puts them into Degrees array
        private void CalculateDegrees()
        {
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
        
        // Function adds edges between pair of vertices to association matrix based on path matrix
        // Argument vertices is a set of pairs of vertices, i.e [2,3,4,5] will add edges between 2 - 3 and 4 - 5
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

        // Checks if graph is Eulerian, i.e. every vertex's degree is even
        public bool IsEulerian()
        {
            for (var i = 1; i < NumOfVertices + 1; i++)
                if (Degrees[i] % 2 != 0)
                    return false;

            return true;
        }

        // Function finds Eulerian path in the graph and returns it as a list of vertices
        public List<int> FindEulerianPath(int startingVertex)
        {
            if (!IsEulerian())
                throw new Exception("Graf nie jest eulerowski");
            
            var stack = new Stack<int>();
            var path = new List<int>();
            int currentVertex = startingVertex;
            int[] degrees = (int[])Degrees.Clone();
            int[,] count = new int[NumOfVertices + 1, NumOfVertices + 1];
            for (var i = 0; i < NumOfVertices + 1; i++)
                for (var j = 0; j < NumOfVertices + 1; j++)
                    count[i, j] = AssocMatrix[i, j].Count;

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

            return path;
        }


        // Function transforms a path through the graph given as a list of subpaths to a string
        public string PathToString(List<Path> path)
        {
            var pathString = new string("");
            path.ForEach(p =>
            {
                p.Vertices.ForEach(v => pathString = string.Concat(pathString, v.ToString()));
            });

            return pathString;
        }

        // Function changes a path given by a list of vertices to a list of subpaths
        public List<Path> FindShortestPath(List<int> eulerPath)
        {
            var shortestPath = new List<Path>();
            var pathArray = eulerPath.ToArray();
            shortestPath.Add(new Path(0, new List<int> { pathArray[0] }));
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

        // Function returns total path length
        public int GetPathLength(List<Path> path)
        {
            int length=0;
            path.ForEach(p => length += p.Distance);
            return length;
        }
    }
}
