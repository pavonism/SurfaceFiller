using SketcherControl.Geometrics;
using System;
using System.Collections.Concurrent;
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
        public Vertex[] Vertices { get; protected set; } = new Vertex[0];
        protected readonly List<Edge> edges = new();

        public int VertexCount { get; protected set; }
        public int EdgesCount => this.edges.Count;
        public IEnumerable<Edge> Edges => this.edges;
        public float Area { get; protected set; }
        public readonly Dictionary<(int, int), float[]> CoefficientsCache = new();
        public readonly Dictionary<(int, int), Vector> NormalVectorsCache = new();


        public virtual void GetMaxPoints(out Point max, out Point min)
        {
            var maxPoint = new PointF(float.MinValue, float.MinValue);
            var minPoint = new PointF(float.MaxValue, float.MaxValue);

            foreach (var vertex in Vertices)
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
        public Triangle()
        {
            this.Vertices = new Vertex[3];
        }

        public void AddVertex(Vertex vertex, Vector? normalVector = null)
        {
            if (normalVector.HasValue)
                vertex.NormalVector = !normalVector.Value;

            if (VertexCount < 3)
            {
                if(VertexCount > 0)
                {
                    this.edges.Add(new Edge(Vertices[VertexCount - 1], vertex));
                }
                if(VertexCount == 2)
                {
                    this.edges.Add(new Edge(vertex, Vertices[0]));
                }
                Vertices[VertexCount] = vertex;
            }

            VertexCount++;
            vertex.RenderCoordinatesChanged += RenderCoordinatesChangedHandler;
        }

        private void RenderCoordinatesChangedHandler()
        {
            Area = ((Vertices[1].RenderLocation - Vertices[0].RenderLocation) | (Vertices[2].RenderLocation - Vertices[0].RenderLocation)).Length / 2;
            CoefficientsCache.Clear();
            NormalVectorsCache.Clear();
        }

        public void SetRenderScale(float scale, float offsetX, float ofssetY)
        {
            foreach (var vertex in Vertices)
            {
                vertex.SetRenderSize(scale, offsetX, ofssetY);
            }
        }

        public void Render(DirectBitmap canvas)
        {
            foreach (var vertex in Vertices)
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
