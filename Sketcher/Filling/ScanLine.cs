using SketcherControl.Shapes;

namespace SketcherControl.Filling
{
    public class ScanLine
    {
        private static List<Edge>[] BucketSort(Polygon polygon, int minY, int maxY)
        {
            List<Edge>[] sortedEdges = new List<Edge>[maxY - minY + 1];

            foreach (var edge in polygon.Edges)
            {
                edge.DrawingX = edge.From.RenderY == edge.YMin ? edge.From.RenderX : edge.To.RenderX;
                var index = (int)edge.YMin - minY;

                if (sortedEdges[index] == null)
                    sortedEdges[index] = new();

                sortedEdges[index].Add(edge);
            }

            return sortedEdges;
        }

        public static void Fill(Polygon polygon, DirectBitmap canvas, ColorPicker colorPicker)
        {
            colorPicker.StartFillingTriangle(polygon.Vertices);
            polygon.GetMaxPoints(out var maxPoint, out var minPoint);

            var ET = BucketSort(polygon, minPoint.Y, maxPoint.Y);
            int ETCount = polygon.EdgesCount;
            int y = 0;
            List<Edge> AET = new();

            while ((ETCount > 0 || AET.Count > 0) && y < maxPoint.Y - minPoint.Y)
            {
                if (ET[y] != null)
                {
                    AET.AddRange(ET[y]);
                    ETCount -= ET[y].Count;
                }

                if (AET.Count == 0)
                {
                    y++;
                    continue;
                }

                AET.Sort((e1, e2) => e1.DrawingX.CompareTo(e2.DrawingX));

                var currentX = AET.First().DrawingX;
                Edge? lastEdge = null;

                foreach (var edge in AET)
                {
                    if (lastEdge == null)
                    {
                        lastEdge = edge;
                        continue;
                    }

                    for (int xi = (int)lastEdge.DrawingX; xi < edge.DrawingX; xi++)
                    {
                        canvas.SetPixel(xi, y + minPoint.Y, colorPicker.GetColor(polygon.Vertices, xi, y + minPoint.Y));
                    }

                    lastEdge = edge;
                }

                AET.RemoveAll((edge) => (int)edge.YMax <= y + minPoint.Y);

                y++;

                foreach (var edge in AET)
                {
                    edge.DrawingX += edge.Slope;
                }
            }
        }
    }
}
