using SketcherControl.Filling;
using SketcherControl.Shapes;

namespace SketcherControl
{
    public class Sketcher : PictureBox, IRenderer
    {
        DirectBitmap canvas;
        private readonly List<Triangle> triangles = new();

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
            LightSource.LightSourceChanged += ParametersChangedHandler;
            ColorPicker.ParametersChanged += ParametersChangedHandler;
        }

        private void ParametersChangedHandler()
        {
            Refresh();
        }

        public void LoadObject(string shapeObject)
        {
            this.triangles.Clear();
            List<Vertex> vertices = new List<Vertex>();
            List<Coordinates> normalVectors = new List<Coordinates>();

            string[] lines = shapeObject.Split("\n");

            foreach (var line in lines.Where((line) => line.StartsWith("v ")))
            {
                var values = line.Split(" ");
                vertices.Add(new Vertex(float.Parse(values[1]), float.Parse(values[2])));
            }

            foreach (var line in lines.Where((line) => line.StartsWith("vn")))
            {
                var values = line.Split(" ");
                normalVectors.Add(new Coordinates(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
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

            LightSource.Render(this.canvas);

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