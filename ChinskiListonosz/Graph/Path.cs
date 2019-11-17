    using System;
using System.Collections.Generic;
using System.Text;

namespace ChinskiListonosz
{
    public class Path
    {
        public int Distance;
        public List<int> Vertices;

        public Path(int dist, List<int> vert)
        {
            Distance = dist;
            Vertices = vert;
        }
    }
}
