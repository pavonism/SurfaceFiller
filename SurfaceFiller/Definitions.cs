
using SurfaceFiller.Components;

namespace SurfaceFiller
{
    public static class Resources
    {
        public const string ProgramTitle = "SurfaceFiller  \u25B2";
    }

    public static class FormConstants
    {
        public const int MinimumWindowSizeX = 700;
        public const int MinimumWindowSizeY = 800;

        public const int InitialWindowSizeX = 900;
        public const int InitialWindowSizeY = 800;

        public const int MainFormColumnCount = 2;
        public const int MinimumControlSize = 32;
        public const int ToolbarWidth = 7 * MinimumControlSize;
        public const int SliderWidth = 3 * MinimumControlSize;
        public const int LabelWidth = 2 * MinimumControlSize;
    }

    public static class Labels
    {
        public const string ThreadSlider = "Threads";
        public const string ShowLinesOption = "Show lines";
        public const string NormalVectorsOption = "Normal Vectors";
        public const string ModelParameters = "Model Parameters";
        public const string KDParameter = "KD =";
        public const string KSParameter = "KS =";
        public const string MParameter = "M =";
        public const string LightSection = "Sun Parameters";
        public const string Speed = "Speed";
        public const string XLocation = "X";
        public const string YLocation = "Y";
        public const string ZLocation = "Z";
        public const string ObjectSection = "Object Parameters";
    }

    public static class Glyphs
    {
        public const string Palette = "\U0001F3A8";
        public const string Spiral = "\U0001F300";
        public const string Reset = "\U0001F504";
        public const string File = "\U0001F4C2";
        public const string Bucket = "\U0001FAA3";
    }

    public static class Hints
    {
        public const string OpenOBJ = "Załaduj powierzchnię z pliku *.obj";
        public const string Fill = "Włącz / wyłącz wypełenienie powierzchni";
        public const string ChangeLightColor = "Zmień kolor źródła światła";
        public const string ShowTrack = "Pokaż / ukryj trajektorię poruszania się światła";
        public const string ResetPosition = "Resetuj położenie źródła światła do punktu początkowego";
        public const string ChangeObjectColor = "Zmień kolor powierzchni";
        public const string LoadObjectPattern = "Wczytaj z pliku teksturę powierzchni";
    }
}
