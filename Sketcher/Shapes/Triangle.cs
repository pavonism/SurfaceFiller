using SketcherControl.Geometrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace SketcherControl.Shapes
{
    public abstract class Polygon
    {
        protected readonly List<Vertex> vertices = new();
        protected readonly List<Edge> edges = new();

        public int VertexCount => vertices.Count;
        public int EdgesCount => this.edges.Count;
        public IEnumerable<Edge> Edges => this.edges;
        public IEnumerable<Vertex> Vertices => this.vertices;
        
        public virtual void GetMaxPoints(out Point max, out Point min)
        {
            var maxPoint = new PointF(float.MinValue, float.MinValue);
            var minPoint = new PointF(float.MaxValue, float.MaxValue);

            foreach (var vertex in vertices)
            {
                maxPoint.X = Math.Max(maxPoint.X, vertex.RenderX);
                maxPoint.Y = Math.Max(maxPoint.Y, vertex.RenderY);
                minPoint.X = Math.Min(minPoint.X, vertex.RenderX);
                minPoint.Y = Math.Min(minPoint.Y, vertex.RenderY);
            }

            max = new Point((int)Math.Ceiling(maxPoint.X), (int)Math.Ceiling(maxPoint.Y));
            min = new Point((int)minPoint.X, (int)minPoint.Y);
        }
    }

    public class Triangle : Polygon
    {
        public void AddVertex(Vertex vertex, Vector? normalVector = null)
        {
            if (normalVector.HasValue)
                vertex.NormalVector = !-normalVector.Value;

            if (vertices.Count < 3)
            {
                if(vertices.Any())
                {
                    this.edges.Add(new Edge(vertices.Last(), vertex));
                }
                if(vertices.Count == 2)
                {
                    this.edges.Add(new Edge(vertex, vertices.First()));
                }
                vertices.Add(vertex);
            }
        }

        public void SetRenderScale(float scale, float offsetX, float ofssetY)
        {
            foreach (var vertex in vertices)
            {
                vertex.SetRenderSize(scale, offsetX, ofssetY);
            }
        }

        public void Render(DirectBitmap canvas)
        {
            foreach (var vertex in vertices)
            {
                vertex.Render(canvas);
            }

            foreach (var edge in edges)
            {
                edge.Render(canvas);
            }
        }
    }
}
