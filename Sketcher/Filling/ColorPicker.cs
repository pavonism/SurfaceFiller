using SketcherControl.Geometrics;
using SketcherControl.Shapes;
using System.Reflection.Metadata.Ecma335;

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

                this.targetColor = value.ToVector();
                ParametersChanged?.Invoke();
            }
        }

        private LightSource lightSource;
        private Vector v = new(0, 0, 1);

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
                    return InterpolateColor(polygon, x, y, coefficients);
                case Interpolation.NormalVector:
                    return InterpolateNormalVector(polygon, x, y, coefficients);
            }

            return Color.Empty;
        }

        public void StartFillingTriangle(IEnumerable<Vertex> vertices)
        {

            switch (InterpolationMode)
            {
                case Interpolation.Color:
                    foreach (var vertex in vertices)
                    {
                        vertex.Color = CalculateColorInPoint(vertex.Location, vertex.NormalVector);
                    }
                    break;
                case Interpolation.NormalVector:
                    break;
            }
        }

        private Color CalculateColorInPoint(Vector location, Vector normalVector)
        {
            Vector IL = this.lightSource.LightSourceVector;
            Vector IO = this.targetColor;
            Vector L = !(this.lightSource.Location - location);
            Vector R = !((2 * normalVector & L) * normalVector - L);

            var angleNL = normalVector & L;
            if (angleNL < 0) angleNL = 0;

            var angleVR = v & R;
            if (angleVR < 0) angleVR = 0;

            return ((KD * IL * IO * angleNL) + (KS * IL * IO * (float)Math.Pow(angleVR, m))).ToColor();
        }

        private Color InterpolateNormalVector(Polygon polygon, int x, int y, float[] coefficients)
        {
            Vector normalVector;

            if (!polygon.NormalVectorsCache.TryGetValue((x, y), out normalVector))
            {
                normalVector = InterpolateNormalVector(polygon, coefficients);
                polygon.NormalVectorsCache.Add((x, y), normalVector);
            }

            var z = polygon.Vertices[0].Location.Z * coefficients[0] + polygon.Vertices[1].Location.Z * coefficients[1] + polygon.Vertices[2].Location.Z * coefficients[2];

            return CalculateColorInPoint(this.lightSource.Renderer.Unscale(x, y, z), normalVector);
        }

        private Color InterpolateColor(Polygon polygon, int x, int y, float[] coefficients)
        {
            var rc = polygon.Vertices[0].Color.R * coefficients[0] / 255 + polygon.Vertices[1].Color.R * coefficients[1] / 255 + polygon.Vertices[2].Color.R * coefficients[2] / 255;
            var gc = polygon.Vertices[0].Color.G * coefficients[0] / 255 + polygon.Vertices[1].Color.G * coefficients[1] / 255 + polygon.Vertices[2].Color.G * coefficients[2] / 255;
            var bc = polygon.Vertices[0].Color.B * coefficients[0] / 255 + polygon.Vertices[1].Color.B * coefficients[1] / 255 + polygon.Vertices[2].Color.B * coefficients[2] / 255;

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
            var xn = polygon.Vertices[0].NormalVector.X * coefficients[0] + polygon.Vertices[1].NormalVector.X * coefficients[1] + polygon.Vertices[2].NormalVector.X * coefficients[2];
            var yn = polygon.Vertices[0].NormalVector.Y * coefficients[0] + polygon.Vertices[1].NormalVector.Y * coefficients[1] + polygon.Vertices[2].NormalVector.Y * coefficients[2];
            var zn = polygon.Vertices[0].NormalVector.Z * coefficients[0] + polygon.Vertices[1].NormalVector.Z * coefficients[1] + polygon.Vertices[2].NormalVector.Z * coefficients[2];

            return !new Vector(xn, yn, zn);
        }
    }
}
