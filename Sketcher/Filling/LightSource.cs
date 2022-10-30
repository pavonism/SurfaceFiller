using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace SketcherControl.Filling
{
    public class LightSource
    {
        public IRenderer Renderer { get; private set; }

        public event Action? LightSourceChanged;

        private Color lightSourceColor = Color.White;
        
        /// <summary>
        /// Współrzędne względem rozmiaru bitmapy
        /// </summary>
        private float sunLocationX = 0.5f, sunLocationY = 0.5f, sunLocationZ = 0.5f;

        float fiSun = 0;
        int xSun;
        int ySun;
        private Timer timer = new();

        public Color LightSourceColor
        {
            get => this.lightSourceColor;
            set
            {
                if (this.lightSourceColor == value)
                    return;

                this.lightSourceColor = value;
                this.LightSourceChanged?.Invoke();
            }
        }


        public bool LightAnimation
        {
            get => timer.Enabled;
            set
            {
                if (value)
                    timer.Start();
                else
                    timer.Stop();
            }
        }

        private float lightSpeed = 0.5f;
        public float LightSpeed
        {
            get => lightSpeed;
            set
            {
                this.lightSpeed = value;
            }
        }

        public float SunLocationX
        {
            get => this.sunLocationX;
            set
            {
                this.sunLocationX = value;
                this.xSun = (int)(Renderer.Size.Width * value);
                this.fiSun = 0;
                this.LightSourceChanged?.Invoke();
            }
        }

        public float SunLocationY
        {
            get => this.sunLocationY;
            set
            {
                this.sunLocationY = value;
                this.ySun = (int)(Renderer.Size.Height * (1 - value));
                this.fiSun = 0;
                this.LightSourceChanged?.Invoke();
            }
        }

        public float SunLocationZ
        {
            get => this.sunLocationZ;
            set
            {
                this.sunLocationZ = value;
                this.LightSourceChanged?.Invoke();
            }
        }

        public LightSource(IRenderer renderer)
        {
            this.Renderer = renderer;
            this.xSun = Renderer.Size.Width;
            this.ySun = Renderer.Size.Height;

            this.timer.Interval = 16;
            this.timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Render(DirectBitmap canvas)
        {
            using (var g = Graphics.FromImage(canvas.Bitmap))
            {
                var size = TextRenderer.MeasureText(SketcherConstants.LightSource, new Font(Control.DefaultFont.Name, 20, FontStyle.Bold));
                var brush = LightSourceColor == Color.White ? Brushes.Gold : new SolidBrush(LightSourceColor);
                g.DrawString(SketcherConstants.LightSource, new Font(Control.DefaultFont.Name, 20, FontStyle.Bold), brush, xSun - size.Width / 2, canvas.Height - (ySun - size.Height / 2));
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateSunCoordinates();
            this.LightSourceChanged?.Invoke();
        }

        private void UpdateSunCoordinates()
        {
            var omega = SketcherConstants.MaxSunAngleIncrease * LightSpeed / (float)(2 * Math.PI * Math.Sqrt(Math.Pow(xSun - Renderer.Size.Width * SunLocationX, 2) + Math.Pow(ySun - Renderer.Size.Height * (1 - SunLocationY), 2)));
            this.fiSun += Math.Max(omega, SketcherConstants.MinSunAngleIncrease) * this.timer.Interval / 1000;
            xSun = 4 * (int)(Math.Cos(fiSun) * this.fiSun) + (int)(Renderer.Size.Width * SunLocationX);
            ySun = 4 * (int)(Math.Sin(fiSun) * this.fiSun) + (int)(Renderer.Size.Height * (1 - SunLocationY));

            if (Renderer.Size.Width < xSun || xSun < 0 || ySun < 0 || ySun > Renderer.Size.Height)
            {
                this.fiSun = 0;
                UpdateSunCoordinates();
            }
        }
    }
}
