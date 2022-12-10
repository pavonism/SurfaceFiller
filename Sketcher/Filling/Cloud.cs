using SketcherControl.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace SketcherControl.Filling
{
    public class Cloud
    {
        private Polygon cloudPolygon;
        private IRenderer renderer;
        private LightSource lightSource;
        private Timer timer = new();
        private MoveDirection moveDirection = MoveDirection.Forward;
        private int offsetX = 0;
        private DirectBitmap cloudTexture;

        public event Action? PropertyChanged;

        private float cloudZ = 0.2f;
        public float CloudZ
        {
            get => cloudZ;
            set
            {
                if (value == this.cloudZ)
                    return;

                this.cloudZ = value;
                foreach (var vertex in this.cloudPolygon.Vertices)
                {
                    vertex.Location.Z = value;
                }
            }
        }

        private bool hide;
        public bool Hide
        {
            get => this.hide;
            set 
            {
                if (value == this.hide)
                    return;

                this.hide = value;
                if (value)
                    this.timer.Stop();
                else
                    this.timer.Start();
                this.PropertyChanged?.Invoke();
            }
        }

        public Cloud(IRenderer renderer, LightSource lightSource)
        {
            this.renderer = renderer;
            this.lightSource = lightSource;
            var offset = renderer.Size.Height / 2;
            var widthProportion = renderer.Size.Width / 2;
            this.cloudPolygon = new Polygon();
            Bitmap bmp = new Bitmap(Image.FromFile(@"..\..\..\..\Assets\Textures\Cloud.jpg"));
            this.cloudTexture = new DirectBitmap(bmp);

            this.cloudPolygon.Vertices = new Vertex[]
            {
                new Vertex(0.1f * widthProportion, offset, 0),
                new Vertex(1f * widthProportion, offset + offset * 0.5f, 0),
                new Vertex(0.8f* widthProportion, offset + offset * 0.4f, 0),
                new Vertex(0.8f* widthProportion, offset, 0),
                new Vertex(1.3f * widthProportion, offset + offset * 0.3f, 0),
                new Vertex(1.4f * widthProportion, offset, 0),
                new Vertex(0.5f * widthProportion, offset - offset * 0.5f, 0),
            };

            for (int i = 0; i < this.cloudPolygon.Vertices.Length; i++)
            {
                this.cloudPolygon.Edges.Add(new Edge(this.cloudPolygon.Vertices[i], this.cloudPolygon.Vertices[(i + 1) % this.cloudPolygon.Vertices.Length]));

            }

            this.timer.Interval = 32;
            this.timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (renderer.Size.Width / 2 + offsetX <= 0)
                moveDirection = MoveDirection.Forward;
            if(renderer.Size.Width / 2 + offsetX >= renderer.Size.Width)
                moveDirection = MoveDirection.Backward;

            if (moveDirection == MoveDirection.Forward)
                offsetX += 1;
            else
                offsetX -= 1;

            renderer.Refresh();
        }

        public void Render(DirectBitmap canvas)
        {
            if(!Hide)
                ScanLine.Fill(cloudPolygon, canvas, null, Color.White, new Point(renderer.Size.Width / 2 + offsetX, renderer.Size.Height / 2), this.cloudTexture);
        }

        public void RenderShade(DirectBitmap canvas)
        {
            if(!Hide)
            {
                var cloudX = renderer.Size.Width / 2 + offsetX;
                var cloudY = renderer.Size.Height / 2;
                var sunLocationX = lightSource.CanvasCoordinates.X;
                var sunLocationY = renderer.Size.Height - lightSource.CanvasCoordinates.Y;
                var sunLocationZ = lightSource.Location.Z;
                var shadeOffsetX = (sunLocationX * cloudZ - sunLocationZ * cloudX) / (CloudZ - sunLocationZ);
                var shadeOffsetY = (sunLocationY * cloudZ - sunLocationZ * cloudY) / (CloudZ - sunLocationZ);

                var shiftedPolygon = new Polygon(this.cloudPolygon);
                shiftedPolygon.Shift(shadeOffsetX, shadeOffsetY);
                ScanLine.Fill(shiftedPolygon, canvas, null, Color.Black);
            }
        }
    }
}
