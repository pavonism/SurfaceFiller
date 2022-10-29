using SketcherControl.Shapes;
using Timer = System.Windows.Forms.Timer;

namespace SketcherControl
{
    public class Sketcher : PictureBox
    {
        DirectBitmap canvas;
        private readonly List<Triangle> triangles = new();

        public bool SunAnimation
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

        private float sunSped = 0.5f;
        public float SunSped
        {
            get => sunSped;
            set
            {
                this.sunSped = value;
            }
        }

        public float SunLocationX
        {
            get => this.sunLocationX;
            set
            {
                this.sunLocationX = value;
                this.xSun = (int)(this.Width * value);
                this.fiSun = 0;
                Refresh();
            }
        } 
        public float SunLocationY
        {
            get => this.sunLocationY;
            set
            {
                this.sunLocationY = value;
                this.ySun = (int)(this.Height * (1 - value));
                this.fiSun = 0;
                Refresh();
            }
        }
        public float SunLocationZ
        {
            get => this.sunLocationZ;
            set
            {
                this.sunLocationZ = value;
                Refresh();
            }
        }


        private float sunLocationX = 0.5f, sunLocationY = 0.5f, sunLocationZ = 0.5f;

        float fiSun = 0;
        int xSun;
        int ySun;
        private Timer timer = new();

        public bool Fill { get; set; }

        public Size BitmpapSize
        {
            get => new(this.canvas.Width, this.canvas.Height);
            set
            {
                this.canvas.Dispose();
                this.canvas = new DirectBitmap(value.Width, value.Height);
                this.Image = this.canvas.Bitmap;
                Refresh();
            }
        }

        public bool ShowLines { get; set; } = true;
        public Color SunColor { get; set; } = Color.White;

        public Sketcher()
        {
            this.canvas = new DirectBitmap(this.Width, this.Height);
            this.Dock = DockStyle.Fill;

            this.xSun = this.Width;
            this.ySun = this.Height;
            this.Image = this.canvas.Bitmap;
            this.timer.Interval = 100;
            this.timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateSunCoordinates();
            Refresh();
        }

        private void UpdateSunCoordinates()
        {
            this.fiSun += SketcherConstants.MaxSunAngleIncrease * SunSped/* / (float)(2 * Math.PI * Math.Sqrt(Math.Pow(xSun - SunLocationX, 2) + Math.Pow(ySun - SunLocationY, 2)))*/;
            xSun = 4 * (int)(Math.Cos(fiSun) * this.fiSun) + (int)(this.Width * SunLocationX);
            ySun = 4 * (int)(Math.Sin(fiSun) * this.fiSun) + (int)(this.Height * (1 - SunLocationY));

            if (this.Width < xSun || xSun < 0 || ySun < 0 || ySun > this.Height)
            {
                this.fiSun = 0;
                UpdateSunCoordinates();
            }
        }



        public void LoadObject(string shapeObject)
        {
            this.triangles.Clear();
            List<Vertex> vertices = new List<Vertex>();
            string[] lines = shapeObject.Split("\n");

            foreach (var line in lines.Where((line) => line.StartsWith("v ")))
            {
                var values = line.Split(" ");
                vertices.Add(new Vertex(float.Parse(values[1]), float.Parse(values[2])));
            }

            Triangle triangle = new();

            foreach (var line in lines.Where((line) => line.StartsWith("f")))
            {
                var faces = line.TrimStart('f', ' ').Split(" ");

                foreach (var face in faces)
                {
                    var vertexIndex = face.Split("//");
                    triangle.AddVertex(vertices[int.Parse(vertexIndex[0]) - 1]);
                }

                this.triangles.Add(triangle);

                triangle = new();
            }

            SetRenderScale();
            Refresh();
        }

        public override void Refresh()
        {
            using (var g = Graphics.FromImage(this.canvas.Bitmap))
            {
                g.Clear(Color.White);
            }

            if (Fill)
                foreach (var trianle in this.triangles)
                {
                    ScanLine.Fill(trianle, canvas);
                }

            if (ShowLines)
                foreach (var triangle in triangles)
                {
                    triangle.Render(this.canvas);
                }

            var size  = TextRenderer.MeasureText(SketcherConstants.LightSource, new Font(DefaultFont.Name, 20, FontStyle.Bold));


            using (var g = Graphics.FromImage(this.canvas.Bitmap))
            {
                var brush = SunColor == Color.White ? Brushes.Gold : new SolidBrush(SunColor);
                g.DrawString(SketcherConstants.LightSource, new Font(DefaultFont.Name, 20, FontStyle.Bold), brush, xSun - size.Width / 2, ySun - size.Height / 2);
            }

            base.Refresh();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            BitmpapSize = new Size(this.Width, this.Height);
            SetRenderScale();
        }

        private void SetRenderScale()
        {
            foreach (var triangle in triangles)
            {
                triangle.SetRenderScale(Math.Min(canvas.Width, canvas.Height) / 3, canvas.Width / 2, canvas.Height / 2);
            }
        }
    }
}