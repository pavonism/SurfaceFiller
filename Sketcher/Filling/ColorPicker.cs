using SketcherControl.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SketcherControl.Filling
{
    public class ColorPicker
    {
        public event Action? ParametersChanged;

        private Color targetColor;
        private Color lightSourceColor;
        private float kD;
        private float kS;
        private int m;

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

        public ColorPicker(LightSource lightSource)
        {
            this.lightSource = lightSource;
        }

        public void StartFillingTriangle(List<Vertex> vertices)
        {
            verticesColors.Clear();

            foreach (var vertex in vertices)
            {

            }
        }

        public Color GetColor()
        {

            return Color.Empty;
        }
    }
}
