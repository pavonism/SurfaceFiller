using SketcherControl.Geometrics;

namespace SketcherControl.Shapes
{
    public class Polygon
    {
        public Vertex[] Vertices { get; set; } = new Vertex[0];
        protected readonly List<Edge> edges = new();

        public int VertexCount { get; protected set; }
        public int EdgesCount => this.edges.Count;
        public List<Edge> Edges => this.edges;

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

        public Polygon()
        {
        }

        public Polygon(Polygon polygon)
        {
            Vertices = new Vertex[polygon.Vertices.Length];

            for (int i = 0; i < polygon.Vertices.Length; i++)
            {
                Vertices[i] = new Vertex(polygon.Vertices[i].RenderX, polygon.Vertices[i].RenderY, polygon.Vertices[i].Location.Z);
            }

            for (int i = 0; i < Vertices.Length; i++)
            {
                Edges.Add(new Edge(Vertices[i], Vertices[(i + 1) % Vertices.Length]));

            }
        }

        public void Shift(float dx, float dy)
        {
            foreach (var vertex in Vertices)
            {
                vertex.RenderX += dx;
                vertex.RenderY += dy;
            }
        }
    }
}
