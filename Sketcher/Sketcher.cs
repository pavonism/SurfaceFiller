using SketcherControl.Filling;
using SketcherControl.Geometrics;
using SketcherControl.Shapes;
using Timer = System.Windows.Forms.Timer;

namespace SketcherControl
{
    public class Sketcher : PictureBox, IRenderer
    {
        DirectBitmap canvas;
        private readonly List<Triangle> triangles = new();
        private Timer resizeTimer;
        private bool freeze;
        private bool animationTurnedOn;

        public int RenderThreads { get; set; }
        public LightSource LightSource { get; private set; }
        public ColorPicker ColorPicker { get; }
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

        public Sketcher()
        {
            LightSource = new(this);
            ColorPicker = new(LightSource);
            this.canvas = new DirectBitmap(this.Width, this.Height);
            this.Dock = DockStyle.Fill;

            this.Image = this.canvas.Bitmap;
            this.resizeTimer = new();
            this.resizeTimer.Interval = 50;
            this.resizeTimer.Tick += ResizeTimerHandler;
            LightSource.LightSourceChanged += ParametersChangedHandler;
            ColorPicker.ParametersChanged += ParametersChangedHandler;
        }

        private void ResizeTimerHandler(object? sender, EventArgs e)
        {
            this.resizeTimer.Stop();
            BitmpapSize = new Size(this.Width, this.Height);
            SetRenderScale();
            this.LightSource.LightAnimation = this.animationTurnedOn;
            this.freeze = false;
        }

        private void ParametersChangedHandler()
        {
            Refresh();
        }

        public void LoadObject(string shapeObject)
        {
            this.triangles.Clear();
            List<Vertex> vertices = new List<Vertex>();
            List<Vector> normalVectors = new List<Vector>();

            string[] lines = shapeObject.Split("\n");

            foreach (var line in lines.Where((line) => line.StartsWith("v ")))
            {
                var values = line.Split(" ");
                vertices.Add(new Vertex(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
            }

            foreach (var line in lines.Where((line) => line.StartsWith("vn")))
            {
                var values = line.Split(" ");
                normalVectors.Add(new Vector(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
            }

            Triangle triangle = new();

            foreach (var line in lines.Where((line) => line.StartsWith("f")))
            {
                var faces = line.TrimStart('f', ' ').Split(" ");

                foreach (var face in faces)
                {
                    var vertexIndex = face.Split("//");
                    triangle.AddVertex(vertices[int.Parse(vertexIndex[0]) - 1], normalVectors[int.Parse(vertexIndex[1]) - 1]);
                }

                this.triangles.Add(triangle);
                triangle = new();
            }

            SetRenderScale();
            Refresh();
        }


        private Task FillAsync(int start, int step)
        {
            return Task.Run(
        () =>
        {
            for (int j = start; j < start + step && j < this.triangles.Count; j++)
            {
                ScanLine.Fill(this.triangles[j], canvas, ColorPicker);

            }
        });
        }

        public override void Refresh()
        {
            if (freeze)
                return;

            using (var g = Graphics.FromImage(this.canvas.Bitmap))
            {
                g.Clear(Color.White);
            }

            if (Fill)
            {
                var trianglesPerThread = (int)Math.Ceiling((float)this.triangles.Count / RenderThreads);
                List<Task> tasks = new();

                for (int i = 0; i < RenderThreads; i++)
                {
                    tasks.Add(FillAsync(i * trianglesPerThread, trianglesPerThread));
                }

                Task.WaitAll(tasks.ToArray());
            }

            if (ShowLines)
                foreach (var triangle in triangles)
                {
                    triangle.Render(this.canvas);
                }

            LightSource.Render(this.canvas);

            base.Refresh();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (!this.freeze)
            {
                this.freeze = true;
                animationTurnedOn = this.LightSource.LightAnimation;
                this.LightSource.LightAnimation = false;
            }
            this.resizeTimer.Stop();
            this.resizeTimer.Start();
        }

        private void SetRenderScale()
        {
            foreach (var triangle in triangles)
            {
                triangle.SetRenderScale(Math.Min(canvas.Width, canvas.Height) / 3, canvas.Width / 2, canvas.Height / 2);
            }
        }

        public Vector Unscale(float x, float y, float z)
        {
            return new Vector()
            {
                X = (x - canvas.Width / 2) / (Math.Min(canvas.Width, canvas.Height) / 3),
                Y = (y - canvas.Height / 2) / (Math.Min(canvas.Width, canvas.Height) / 3),
                Z = z,
            };
        }
    }
}