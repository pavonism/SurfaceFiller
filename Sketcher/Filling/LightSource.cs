﻿using SketcherControl.Geometrics;
using Timer = System.Windows.Forms.Timer;

namespace SketcherControl.Filling
{
    public class LightSource
    {
        public IRenderer Renderer { get; private set; }

        public event Action? LightSourceChanged;

        private Vector lightSourceColor = new Vector(1, 1, 1);

        /// <summary>
        /// Współrzędne względem rozmiaru bitmapy
        /// </summary>
        private float lightLocationX = 0.5f, lightLocationY = 0.5f, lightLocationZ = 0.5f;

        float lightAngle = 0;
        int xSun;
        int ySun;
        private bool showTrack;
        private Timer timer = new();

        public Vector Location => Renderer.Unscale(xSun, Renderer.Size.Height - ySun, 1f + 5f*lightLocationZ);

        public bool ShowTrack
        {
            get => this.showTrack;
            set
            {
                if (this.showTrack == value)
                    return;

                this.showTrack = value;
                this.LightSourceChanged?.Invoke();
            }
        }

        public Color LightSourceColor
        {
            get => this.lightSourceColor.ToColor();
            set
            {
                if (this.lightSourceColor.ToColor() == value)
                    return;

                this.lightSourceColor = value.ToVector();
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

        public float LightLocationX
        {
            get => this.lightLocationX;
            set
            {
                if (this.lightLocationX == value)
                    return;

                this.lightLocationX = value;
                RecalculateLightCoordinates();
                this.LightSourceChanged?.Invoke();
            }
        }

        public float LightLocationY
        {
            get => this.lightLocationY;
            set
            {
                if (this.lightLocationY == 1 - value)
                    return;

                this.lightLocationY = 1 - value;
                RecalculateLightCoordinates();
                this.LightSourceChanged?.Invoke();
            }
        }

        public float LightLocationZ
        {
            get => this.lightLocationZ;
            set
            {
                if (this.lightLocationZ == value)
                    return;

                this.lightLocationZ = value;
                this.LightSourceChanged?.Invoke();
            }
        }

        public Vector LightSourceVector => this.lightSourceColor;
        public Vector CanvasCoordinates => new Vector(xSun, ySun, LightLocationZ);

        public LightSource(IRenderer renderer)
        {
            this.Renderer = renderer;
            this.xSun = Renderer.Size.Width;
            this.ySun = Renderer.Size.Height;

            this.timer.Interval = 32;
            this.timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Render(DirectBitmap canvas)
        {
            if (Renderer.Size.Width > xSun && xSun > 0 && ySun > 0 && ySun < Renderer.Size.Height)
                using (var g = Graphics.FromImage(canvas.Bitmap))
                {
                    var size = TextRenderer.MeasureText(SketcherConstants.LightSource, new Font(Control.DefaultFont.Name, 20, FontStyle.Bold));
                    var brush = /*LightSourceColor == Color.White ? */Brushes.Gold/* : new SolidBrush(LightSourceColor)*/;
                    g.DrawString(SketcherConstants.LightSource, new Font(Control.DefaultFont.Name, 20, FontStyle.Bold), brush, xSun - size.Width / 2, ySun - size.Height / 2);
                }

            if (ShowTrack)
            {
                float currentAngle = 0f;
                int currentXLight = (int)(LightLocationX * Renderer.Size.Width), currentYLight = (int)(Renderer.Size.Height * LightLocationY);
                while (currentAngle < this.lightAngle)
                {
                    var omega = SketcherConstants.LightSourceSpeedCoefficient * LightSpeed / (float)(2 * Math.PI * Math.Sqrt(Math.Pow(currentXLight - Renderer.Size.Width * LightLocationX, 2) + Math.Pow(currentYLight - Renderer.Size.Height * (1 - LightLocationY), 2)));
                    currentAngle += Math.Max(omega, SketcherConstants.MinSunAngleIncrease) * this.timer.Interval / 1000;
                    currentXLight = 4 * (int)(Math.Cos(currentAngle) * currentAngle) + (int)(Renderer.Size.Width * LightLocationX);
                    currentYLight = 4 * (int)(Math.Sin(currentAngle) * currentAngle) + (int)(Renderer.Size.Height * LightLocationY);

                    if (Renderer.Size.Width <= currentXLight || currentXLight <= 0 || currentYLight <= 0 || currentYLight >= Renderer.Size.Height)
                        continue;
                    canvas.SetPixel(currentXLight, canvas.Height - currentYLight, LightSourceColor == Color.White ? Color.Gold : LightSourceColor);
                }
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            MoveLight();
            this.LightSourceChanged?.Invoke();
        }

        private void MoveLight()
        {
            var omega = SketcherConstants.LightSourceSpeedCoefficient * Math.Min(Renderer.Size.Width, Renderer.Size.Height) * LightSpeed / (float)(2 * Math.PI * Math.Sqrt(Math.Pow(xSun - Renderer.Size.Width * LightLocationX, 2) + Math.Pow(ySun - Renderer.Size.Height * LightLocationY, 2)));
            this.lightAngle += Math.Max(omega, SketcherConstants.MinSunAngleIncrease) * this.timer.Interval / 1000;
            RecalculateLightCoordinates();
        }

        private void RecalculateLightCoordinates()
        {
            xSun = 4 * (int)(Math.Cos(lightAngle) * this.lightAngle) + (int)(Renderer.Size.Width * LightLocationX);
            ySun = 4 * (int)(Math.Sin(lightAngle) * this.lightAngle) + (int)(Renderer.Size.Height * LightLocationY);
        }

        public void Reset()
        {
            xSun = (int)(LightLocationX * Renderer.Size.Width);
            ySun = (int)(LightLocationY * Renderer.Size.Height);

            this.lightAngle = 0f;
            this.LightSourceChanged?.Invoke();
        }
    }
}
