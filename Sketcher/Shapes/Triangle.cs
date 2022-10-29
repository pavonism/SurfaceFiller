using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace SketcherControl.Shapes
{
    public class Triangle 
    {
        private readonly List<Vertex> vertices = new();
        private readonly List<Edge> edges = new();

        public int VertexCount => vertices.Count;
        public int EdgesCount => this.edges.Count;

        public void AddVertex(Vertex vertex)
        {
            if (vertices.Count < 3)
            {
                if(vertices.Any())
                {
                    this.edges.Add(new Edge(vertices.Last(), vertex));
                }
                if(vertices.Count == 2)
                {
                    this.edges.Add(new Edge(vertices.First(), vertex));
                }
                vertices.Add(vertex);
            }
        }

        public void Render(DirectBitmap canvas, float scale, bool fill)
        {
            foreach (var vertex in vertices)
            {
                vertex.Render(canvas, scale);
            }

            foreach (var edge in edges)
            {
                edge.Render(canvas, scale);
            }


        }
    }
}
