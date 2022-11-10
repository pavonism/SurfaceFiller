﻿using SketcherControl.Geometrics;
using SketcherControl.Shapes;

namespace SketcherControl.Filling
{
    public class ColorPicker
    {
        public event Action? ParametersChanged;

        private Interpolation interpolationMode;
        private Vector targetColor = SketcherConstants.ThemeColor.ToVector();
        private float kD = 0.5f;
        private float kS = 0.5f;
        private int m = 4;
        private LightSource lightSource;
        private Vector v = new(0, 0, 1);
        private DirectBitmap? texture;

        public Interpolation InterpolationMode
        {
            get => this.interpolationMode;
            set
            {
                if (this.interpolationMode == value)
                    return;

                this.interpolationMode = value;
                ParametersChanged?.Invoke();
            }
        }

        public float KD
        {
            get => this.kD;
            set
            {
                if (this.kD == value)
                    return;

                this.kD = value;
                ParametersChanged?.Invoke();
            }
        }

        public float KS
        {
            get => this.kS;
            set
            {
                if (this.kS == value)
                    return;

                this.kS = value;
                ParametersChanged?.Invoke();
            }
        }

        public int M
        {
            get => this.m;
            set
            {
                if (this.m == value)
                    return;

                this.m = value;
                ParametersChanged?.Invoke();
            }
        }

        public Color TargetColor
        {
            get => this.targetColor.ToColor();
            set
            {
                if (this.targetColor == value.ToVector())
                    return;

                texture?.Dispose();
                texture = null;
                this.targetColor = value.ToVector();
                ParametersChanged?.Invoke();
            }
        }

        public Bitmap? Pattern
        {
            get => this.texture?.Bitmap;
            set
            {
                if (value == null)
                {
                    this.texture = null;
                }
                else
                {
                    this.texture = new DirectBitmap(value.Width, value.Height);

                    for (int i = 0; i < value.Width; i++)
                    {
                        for (int j = 0; j < value.Height; j++)
                        {
                            this.texture.SetPixel(i, value.Height - j, value.GetPixel(i, j));
                        }
                    }
                }

                ParametersChanged?.Invoke();
            }
        }

        public ColorPicker(LightSource lightSource)
        {
            this.lightSource = lightSource;
        }

        public Color GetColor(Polygon polygon, int x, int y)
        {
            if (polygon.VertexCount < 3)
                return Color.Empty;

            float[]? coefficients;

            if (!polygon.CoefficientsCache.TryGetValue((x, y), out coefficients))
            {
                coefficients = CalculateCoefficients(polygon, x, y);
            }

            switch (InterpolationMode)
            {
                case Interpolation.Color:
                    return InterpolateColor(polygon, coefficients);
                case Interpolation.NormalVector:
                    var normalVector = InterpolateNormalVector(polygon, x, y, coefficients);
                    var textureColor = GetTextureColor(x, y);
                    var z = InterpolateZ(polygon, coefficients);
                    return CalculateColorInPoint(this.lightSource.Renderer.Unscale(x, y, z), normalVector, textureColor);
                case Interpolation.NormalMap:
                    normalVector = InterpolateNormalVector(polygon, x, y, coefficients);
                    return CalculateVectorMapColor(polygon, x, y, normalVector, coefficients);
            }

            return Color.Empty;
        }

        private Color CalculateVectorMapColor(Polygon polygon, int x, int y, Vector NSurface, float[] coefficients)
        {
            var textureColor = this.texture!.GetPixel(x % texture.Width, y % texture.Height);
            var NTexture = textureColor.ToNormalMapVector();
            var B = !(NSurface | new Vector(0, 0, 1));
            var T = !(B | NSurface);

            var N = new Vector()
            {
                X = T.X * NTexture.X + B.X * NTexture.Y + NSurface.X * NTexture.Z,
                Y = T.Y * NTexture.X + B.Y * NTexture.Y + NSurface.Y * NTexture.Z,
                Z = T.Z * NTexture.X + B.Z * NTexture.Y + NSurface.Z * NTexture.Z,
            };

            var z = InterpolateZ(polygon, coefficients);

            return CalculateColorInPoint(this.lightSource.Renderer.Unscale(x, y, z), N, textureColor.ToVector());
        }

        public void StartFillingTriangle(IEnumerable<Vertex> vertices)
        {

            switch (InterpolationMode)
            {
                case Interpolation.Color:
                    foreach (var vertex in vertices)
                    {
                        var textureColor = texture?.GetPixel(((int)vertex.RenderX + texture.Width / 2) % texture.Width, ((int)vertex.RenderY + texture.Height / 2) % texture.Height).ToVector();
                        vertex.Color = CalculateColorInPoint(vertex.Location, vertex.NormalVector, textureColor);
                    }
                    break;
                case Interpolation.NormalVector:
                    break;
            }
        }

        private Color CalculateColorInPoint(Vector location, Vector normalVector, Vector? textureColor = null)
        {
            Vector IL = this.lightSource.LightSourceVector;
            Vector IO = textureColor ?? this.targetColor;
            Vector L = !(this.lightSource.Location - location);
            Vector R = (2 * normalVector & L) * normalVector - L;

            var angleNL = normalVector & L;
            if (angleNL < 0) angleNL = 0;

            var angleVR = v & R;
            if (angleVR < 0) angleVR = 0;

            return ((KD * IL * IO * angleNL) + (KS * IL * IO * (float)Math.Pow(angleVR, m))).ToColor();
        }

        private Vector InterpolateNormalVector(Polygon polygon, int x, int y, float[] coefficients)
        {
            Vector normalVector;

            if (!polygon.NormalVectorsCache.TryGetValue((x, y), out normalVector))
            {
                normalVector = InterpolateNormalVector(polygon, coefficients);
                polygon.NormalVectorsCache.Add((x, y), normalVector);
            }

            return normalVector;
        }

        private float InterpolateZ(Polygon polygon, float[] coefficients)
        {
            return polygon.Vertices[0].Location.Z * coefficients[1] + polygon.Vertices[1].Location.Z * coefficients[2] + polygon.Vertices[2].Location.Z * coefficients[0];
        }

        private Vector? GetTextureColor(int x, int y)
        {
            return this.texture?.GetPixel(x % texture.Width, y % texture.Height).ToVector();
        }

        private Color InterpolateColor(Polygon polygon, float[] coefficients)
        {
            var rc = polygon.Vertices[0].Color.R * coefficients[1] / 255 + polygon.Vertices[1].Color.R * coefficients[2] / 255 + polygon.Vertices[2].Color.R * coefficients[0] / 255;
            var gc = polygon.Vertices[0].Color.G * coefficients[1] / 255 + polygon.Vertices[1].Color.G * coefficients[2] / 255 + polygon.Vertices[2].Color.G * coefficients[0] / 255;
            var bc = polygon.Vertices[0].Color.B * coefficients[1] / 255 + polygon.Vertices[1].Color.B * coefficients[2] / 255 + polygon.Vertices[2].Color.B * coefficients[0] / 255;

            if (rc < 0) rc = 0;
            if (rc > 1) rc = 1;
            if (gc < 0) gc = 0;
            if (gc > 1) gc = 1;
            if (bc < 0) bc = 0;
            if (bc > 1) bc = 1;

            return Color.FromArgb((int)(rc * 255), (int)(gc * 255), (int)(bc * 255));
            //Vector color = vertex[0].Color.ToVector() * coefficients[0] + vertex[1].Color.ToVector() * coefficients[1] + vertex[2].Color.ToVector() * coefficients[2];

            //return color.ToColor();
        }

        private float[] CalculateCoefficients(Polygon polygon, int x, int y)
        {
            var pixelLocation = new Vector(x, y, 0);
            var coefficients = new float[polygon.VertexCount];
            for (int i = 0; i < polygon.Vertices.Length; i++)
            {
                var a = (polygon.Vertices[i].RenderLocation - pixelLocation).Length;
                var b = (polygon.Vertices[(i + 1) % polygon.VertexCount].RenderLocation - pixelLocation).Length;
                var c = (polygon.Vertices[i].RenderLocation - polygon.Vertices[(i + 1) % polygon.VertexCount].RenderLocation).Length;
                var s = (a + b + c) / 2;
                var smallArea = (float)Math.Sqrt(s * (s - a) * (s - b) * (s - c));
                //var smallArea = ((vertex[i].RenderLocation - pixelLocation) | (vertex[(i + 1) % vertex.Length].RenderLocation - pixelLocation)).Length / 2;
                coefficients[i] = smallArea / polygon.Area;

                if (s * (s - a) * (s - b) * (s - c) < 0)
                    coefficients[i] = 0;
            }

            polygon.CoefficientsCache.Add((x, y), coefficients);
            return coefficients;
        }

        private Vector InterpolateNormalVector(Polygon polygon, float[] coefficients)
        {
            var xn = polygon.Vertices[0].NormalVector.X * coefficients[1] + polygon.Vertices[1].NormalVector.X * coefficients[2] + polygon.Vertices[2].NormalVector.X * coefficients[0];
            var yn = polygon.Vertices[0].NormalVector.Y * coefficients[1] + polygon.Vertices[1].NormalVector.Y * coefficients[2] + polygon.Vertices[2].NormalVector.Y * coefficients[0];
            var zn = polygon.Vertices[0].NormalVector.Z * coefficients[1] + polygon.Vertices[1].NormalVector.Z * coefficients[2] + polygon.Vertices[2].NormalVector.Z * coefficients[0];

            return !new Vector(xn, yn, zn);
        }
    }
}
