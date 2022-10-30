namespace SketcherControl.Filling
{
    public interface IRenderer
    {
        Size Size { get; }
        void Refresh();
    }
}