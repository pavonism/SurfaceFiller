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
                Vector R = (2 * vertex.NormalVector & L) * vertex.NormalVector - L;

                var angleNL = vertex.NormalVector & L;
                if (angleNL < 0) angleNL = 0;

                var angleVR = v & R;
                if (angleVR < 0) angleVR = 0;

                vertex.Color = ((KD * IL * IO * angleNL) + (KS * IL * IO * (float)Math.Pow(angleVR, m))).ToColor();
            }
        }

        public Color GetColor(IEnumerable<Vertex> vertices, float x, float y)
        {
            if (vertices.Count() != 3)
                return Color.Empty;

            var pixelLocation = new Vector(x, y, 0);
            var vertex = vertices.ToArray();
            var wholeArea = ((vertex[1].RenderLocation - vertex[0].RenderLocation) | (vertex[2].RenderLocation - vertex[0].RenderLocation)).Length / 2;
            var coefficients = new float[vertex.Length];

            for (int i = 0; i < vertex.Length; i++)
            {
                var smallArea = ((vertex[i].RenderLocation - pixelLocation) | (vertex[(i + 1) % vertex.Length].RenderLocation - pixelLocation)).Length / 2;
                coefficients[i] = smallArea / wholeArea;
            }

            Vector color = vertex[0].Color.ToVector() * coefficients[0] + vertex[1].Color.ToVector() * coefficients[1] + vertex[2].Color.ToVector() * coefficients[2];

            return color.ToColor();
        }
    }
}
