﻿using SketcherControl.Geometrics;


namespace SketcherControl.Shapes
{
    public class Vertex
    {
        public Vector Location;
        public Vector NormalVector { get; set; }
        public Vector RenderLocation => new Vector(RenderX, RenderY, 0);

        public float RenderX { get; private set; }
        public float RenderY { get; private set; }
        public Color Color { get; set; }

        public event Action? RenderCoordinatesChanged;

        public Vertex(float x, float y, float z)
        {
            Location.X = x;
            Location.Y = y;
            Location.Z = z;

            RenderX = x;
            RenderY = y;
        }

        public void SetRenderSize(float scale, float offsetX, float ofssetY)
        {
            RenderX = Location.X * scale + offsetX;
            RenderY = Location.Y * scale + ofssetY;

            RenderCoordinatesChanged?.Invoke();
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
