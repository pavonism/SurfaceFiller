using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SketcherControl.Shapes
{
    public struct NormalVector
    {
        public float X;
        public float Y;
        public float Z;
    }

    public class Vertex
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        private NormalVector normalVector;

        public Vertex(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void Render(DirectBitmap canvas, float scale)
        {
            RectangleF rectangle = new RectangleF(X * scale + (float)canvas.Width / 2 - SketcherConstants.VertexPointRadius, Y * scale + (float)canvas.Height / 2 - SketcherConstants.VertexPointRadius,
                2 * SketcherConstants.VertexPointRadius, 2 * SketcherConstants.VertexPointRadius);

            using (var g = Graphics.FromImage(canvas.Bitmap))
            {
                g.FillEllipse(Brushes.Black, rectangle);
                g.DrawEllipse(Pens.Black, rectangle);
            }
        }
    }
}
