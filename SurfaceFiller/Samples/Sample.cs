using SurfaceFiller.Components;
using System.Collections;

namespace SurfaceFiller.Samples
{
    internal abstract class BasicSample
    {
        public string Name { get; set; }

        public BasicSample(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    internal abstract class Sample : BasicSample, IComboItem
    {
        protected Sample(string name) : base(name)
        {
        }

        public abstract Image GetThumbnail(int width, int height);
    }

    internal class PictureSample : Sample
    {
        public Bitmap? Image { get; private set; }

        public PictureSample(Bitmap? image, string name) : base(name)
        {
            Image = image;
        }

        public override Image GetThumbnail(int width, int height)
        {
            if (Image == null)
                return new Bitmap(1, 1);

            var thumbnail = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var color = Image.GetPixel(i % Image.Width, j % Image.Height);
                    thumbnail.SetPixel(i, j, color);
                }
            }

            return thumbnail;
        }

    }

    internal class ColorSample : Sample
    {
        public Color Color { get; set; }

        public ColorSample(Color color, string name) : base(name)
        {
            Color = color;
        }

        public override Image GetThumbnail(int width, int height)
        {
            var thumbnail = new Bitmap(width, height);

            using (Graphics gfx = Graphics.FromImage(thumbnail))
            using (SolidBrush brush = new SolidBrush(Color))
            {
                gfx.FillRectangle(brush, 0, 0, width, height);
            }

            return thumbnail;
        }
    }
}
