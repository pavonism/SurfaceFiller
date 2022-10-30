using SketcherControl.Geometrics;

namespace SketcherControl.Filling
{
    public interface IRenderer
    {
        Size Size { get; }
        void Refresh();
        Vector Unscale(float x, float y, float z);
    }
}