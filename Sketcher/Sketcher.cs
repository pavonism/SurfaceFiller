using SketcherControl.Shapes;

namespace SketcherControl
{
    public class Sketcher : PictureBox
    {
        DirectBitmap canvas;
        private readonly List<Triangle> triangles = new();

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

        public Sketcher()
        {
            this.canvas = new DirectBitmap(this.Width, this.Height);
            this.Dock = DockStyle.Fill;

            this.Image = this.canvas.Bitmap;
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

            Refresh();
        }

        public override void Refresh()
        {
            using(var g = Graphics.FromImage(this.canvas.Bitmap))
            {
                g.Clear(Color.White);
            }

            foreach (var triangle in triangles)
            {
                triangle.Render(this.canvas, Math.Min(this.Width, this.Height) / 3, Fill);
            }

            base.Refresh();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            BitmpapSize = new Size(this.Width, this.Height);
        }
    }
}