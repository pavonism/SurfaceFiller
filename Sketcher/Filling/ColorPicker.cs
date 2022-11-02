using SketcherControl.Geometrics;
using SketcherControl.Shapes;

namespace SketcherControl.Filling
{
    public class ColorPicker
    {
        public event Action? ParametersChanged;

        private Color targetColor = SketcherConstants.ThemeColor;
        private Color lightSourceColor;
        private float kD = 0.5f;
        private float kS = 0.5f;
        private int m = 4;

        public float KD
        {
            get => kD;
            set
            {
                if (kD == value)
                    return;

                kD = value;
                ParametersChanged?.Invoke();
            }
        }

        public float KS
        {
            get => kS;
            set
            {
                if (kS == value)
                    return;

                kS = value;
                ParametersChanged?.Invoke();
            }
        }

        public int M
        {
            get => m;
            set
            {
                if (m == value)
                    return;

                m = value;
                ParametersChanged?.Invoke();
            }
        }

        public Color LightSourceColor
        {
            get => lightSourceColor;
            set
            {
                if (lightSourceColor == value)
                    return;

                lightSourceColor = value;
                ParametersChanged?.Invoke();
            }
        }

        public Color TargetColor
        {
            get => targetColor;
            set
            {
                if (targetColor == value)
                    return;

                targetColor = value;
                ParametersChanged?.Invoke();
            }
        }

        private readonly List<Color> verticesColors = new();
        private LightSource lightSource;
        private Vector v = new(0, 0, 1);

        public ColorPicker(LightSource lightSource)
        {
            this.lightSource = lightSource;
        }

        public void StartFillingTriangle(IEnumerable<Vertex> vertices)
        {
            verticesColors.Clear();

            Vector IL = this.lightSource.LightSourceColor.ToVector();
            Vector IO = TargetColor.ToVector();

            foreach (var vertex in vertices)
            {
                Vector L = !(this.lightSource.Location - vertex.Location);
                Vector R = !((2 * vertex.NormalVector & L) * vertex.NormalVector - L);

                var angleNL = vertex.NormalVector & L;
                if (angleNL < 0) angleNL = 0;

                var angleVR = v & R;
                if (angleVR < 0) angleVR = 0;

                vertex.Color = ((KD * IL * IO * angleNL) + (KS * IL * IO * (float)Math.Pow(angleVR, m))).ToColor();
            }
        }

        public Color GetColor(Polygon polygon, int x, int y)
        {
            if (polygon.VertexCount < 3)
                return Color.Empty;

            float[]? coefficients;

            if (!polygon.CoefficientsCache.TryGetValue((x, y), out coefficients))
            {
                var pixelLocation = new Vector(x, y, 0);
                coefficients = new float[polygon.VertexCount];
                for (int i = 0; i < polygon.Vertices.Length; i++)
                {
                    var a = (polygon.Vertices[i].RenderLocation - pixelLocation).Length;
                    var b = (polygon.Vertices[(i + 1) % polygon.VertexCount].RenderLocation - pixelLocation).Length;
                    var c = (polygon.Vertices[i].RenderLocation - polygon.Vertices[(i + 1) % polygon.VertexCount].RenderLocation).Length;
                    var s = (a + b + c) / 2;
                    var smallArea = (float)Math.Sqrt(s * (s - a) * (s - b) * (s - c));
                    //var smallArea = ((vertex[i].RenderLocation - pixelLocation) | (vertex[(i + 1) % vertex.Length].RenderLocation - pixelLocation)).Length / 2;
                    coefficients[i] = smallArea / polygon.Area / 255;

                    if (s * (s - a) * (s - b) * (s - c) < 0)
                        coefficients[i] = 0;
                }

                polygon.CoefficientsCache.TryAdd((x, y), coefficients);
            }

            var rc = polygon.Vertices[0].Color.R * coefficients[0] + polygon.Vertices[1].Color.R * coefficients[1] + polygon.Vertices[2].Color.R * coefficients[2];
            var gc = polygon.Vertices[0].Color.G * coefficients[0] + polygon.Vertices[1].Color.G * coefficients[1] + polygon.Vertices[2].Color.G * coefficients[2];
            var bc = polygon.Vertices[0].Color.B * coefficients[0] + polygon.Vertices[1].Color.B * coefficients[1] + polygon.Vertices[2].Color.B * coefficients[2];

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
    }
}
