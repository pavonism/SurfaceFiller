﻿using SketcherControl.Geometrics;

namespace SketcherControl.Shapes
{
    public abstract class Polygon
    {
        public Vertex[] Vertices { get; protected set; } = new Vertex[0];
        protected readonly List<Edge> edges = new();

        public int VertexCount { get; protected set; }
        public int EdgesCount => this.edges.Count;
        public IEnumerable<Edge> Edges => this.edges;
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
}
