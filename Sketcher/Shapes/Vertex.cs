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


        public float RenderX { get; private set; }
        public float RenderY { get; private set; }


        private NormalVector normalVector;

        public Vertex(float x, float y)
        {
            X = x;
            Y = y;

            RenderX = x;
            RenderY = y;
        }

        public void SetRenderSize(float scale, float offsetX, float ofssetY)
        {
            RenderX = X * scale + offsetX;
            RenderY = Y * scale + ofssetY;
        }

        public void Render(DirectBitmap canvas)
        {
            RectangleF rectangle = new RectangleF(RenderX - SketcherConstants.VertexPointRadius, canvas.Height - RenderY - SketcherConstants.VertexPointRadius,
                2 * SketcherConstants.VertexPointRadius, 2 * SketcherConstants.VertexPointRadius);

            using (var g = Graphics.FromImage(canvas.Bitmap))
            {
                g.FillEllipse(Brushes.Black, rectangle);
                g.DrawEllipse(Pens.Black, rectangle);
            }
        }
    }
}
