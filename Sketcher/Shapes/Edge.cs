using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SketcherControl.Shapes
{
    public class Edge
    {
        public Vertex From;
        public Vertex To;

        public float YMax => Math.Max(From.RenderY, To.RenderY);
        public float YMin => Math.Min(From.RenderY, To.RenderY);
        public float XMax => Math.Max(From.RenderX, To.RenderX);
        public float XMin => Math.Min(From.RenderX, To.RenderX);
        public float Slope { get; private set; }
        public float DrawingX { get; set; }

        public Edge(Vertex from, Vertex to)
        {
            From = from;
            To = to;

            var lowerVertex = From.Y < To.Y ? From : To;
            var higherVertex = From != lowerVertex ? From : To;
            //Slope = (To.X - From.X) / (To.Y - From.Y);
            Slope = (higherVertex.X - lowerVertex.X) / (higherVertex.Y - lowerVertex.Y);
        }

        public void Render(DirectBitmap bitmap)
        {
            using (Graphics g = Graphics.FromImage(bitmap.Bitmap))
            {
                g.DrawLine(Pens.Black, From.RenderX, bitmap.Height - From.RenderY, To.RenderX, bitmap.Height - To.RenderY);
            }
        }
    }
}
