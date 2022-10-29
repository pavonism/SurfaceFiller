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

        public Edge(Vertex from, Vertex to)
        {
            From = from;
            To = to;
        }

        public void Render(DirectBitmap bitmap, float scale)
        {
            using (Graphics g = Graphics.FromImage(bitmap.Bitmap))
            {
                g.DrawLine(Pens.Black, From.X * scale + (float)bitmap.Width / 2, From.Y * scale + (float)bitmap.Height / 2, To.X * scale + (float)bitmap.Width / 2, To.Y * scale + (float)bitmap.Height / 2);
            }
        }
    }
}
