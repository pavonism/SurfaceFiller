using SketcherControl;
using SketcherControl.Filling;
using SurfaceFiller.Components;
using System.Drawing.Imaging;

namespace SurfaceFiller
{
    public partial class MainForm : Form
    {
        private TableLayoutPanel mainTableLayout = new();
        private Toolbar toolbar = new() { Width = FormConstants.ToolbarWidth };
        private Sketcher sketcher = new();

        public MainForm()
        {
            InitializeToolbar();
            ArrangeComponents();
            InitializeForm();
        }

        private void InitializeToolbar()
        {
            this.toolbar.AddLabel(Resources.ProgramTitle);
            this.toolbar.AddDivider();
            this.toolbar.AddButton(OpenFileHandler, Glyphs.File, Hints.OpenOBJ);
            this.toolbar.AddTool(FillHandler, Glyphs.Bucket, Hints.Fill);
            this.toolbar.AddSlider(ThreadsSlidrerHandler, Labels.ThreadSlider , 0.01f);
            this.toolbar.AddDivider();
            this.toolbar.AddOption(Labels.ShowLinesOption, ShowLinesHandler);
            this.toolbar.AddOption(Labels.NormalVectorsOption, NormalVectorsHandler);
            this.toolbar.AddDivider();
            this.toolbar.AddLabel(Labels.ModelParameters);
            this.toolbar.AddFractSlider(KDParameterHandler, Labels.KDParameter, 0.5f);
            this.toolbar.AddFractSlider(KSParameterHandler, Labels.KSParameter, 0.5f);
            this.toolbar.AddSlider(MParameterHandler, Labels.MParameter, 0.5f);
            this.toolbar.AddDivider();
            this.toolbar.AddLabel(Labels.LightSection);
            this.toolbar.AddPlayPouse(SunHandler, string.Empty, true);
            this.toolbar.AddButton(ColorButton, Glyphs.Palette, Hints.ChangeLightColor);
            this.toolbar.AddTool(ShowTrackHandler, Glyphs.Spiral, Hints.ShowTrack);
            this.toolbar.AddButton(ResetPositionButton, Glyphs.Reset, Hints.ResetPosition);
            this.toolbar.AddSlider(SunSpeedHanlder, Labels.Speed, 0.1f);
            this.toolbar.AddSlider(SunXLocationHandler, Labels.XLocation, 0.5f);
            this.toolbar.AddSlider(SunYLocationHandler, Labels.YLocation, 0.5f);
            this.toolbar.AddSlider(SunZLocationHandler, Labels.ZLocation, 0.5f);
            this.toolbar.AddDivider();
            this.toolbar.AddLabel(Labels.ObjectSection);
            this.toolbar.AddButton(ObjectColorButton, Glyphs.Palette, Hints.ChangeObjectColor);
            this.toolbar.AddButton(LoadTextureHandlar, Glyphs.File, Hints.LoadObjectPattern);
        }

        private void LoadTextureHandlar(object? sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Object files (*.obj)|*.obj|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                var codecs = ImageCodecInfo.GetImageEncoders();
                var codecFilter = "Image Files|";
                foreach (var codec in codecs)
                {
                    codecFilter += codec.FilenameExtension + ";";
                }
                openFileDialog.Filter = codecFilter;


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                }
            }

            this.sketcher.ColorPicker.Pattern = new Bitmap(Image.FromFile(filePath));
        }

        private void NormalVectorsHandler(object? sender, EventArgs e)
        {
            this.sketcher.ColorPicker.InterpolationMode = this.sketcher.ColorPicker.InterpolationMode == Interpolation.Color ? Interpolation.NormalVector : Interpolation.Color;
        }

        private void ThreadsSlidrerHandler(float value)
        {
            this.sketcher.RenderThreads = (int)(value * 100);
        }

        private void ObjectColorButton(object? sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;
            MyDialog.Color = this.sketcher.LightSource.LightSourceColor;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
                this.sketcher.ColorPicker.TargetColor = MyDialog.Color;
        }

        private void ResetPositionButton(object? sender, EventArgs e)
        {
            this.sketcher.LightSource.Reset();
        }

        private void ShowTrackHandler(bool obj)
        {
            this.sketcher.LightSource.ShowTrack = !this.sketcher.LightSource.ShowTrack;
        }

        private void ColorButton(object? sender, EventArgs e)
        {

            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;
            MyDialog.Color = this.sketcher.LightSource.LightSourceColor;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
                this.sketcher.LightSource.LightSourceColor = MyDialog.Color;
        }

        private void MParameterHandler(float newValue)
        {
            this.sketcher.ColorPicker.M = (int)(100 * newValue);
        }

        private void KSParameterHandler(float newValue)
        {
            this.sketcher.ColorPicker.KS = newValue;
        }

        private void KDParameterHandler(float newValue)
        {
            this.sketcher.ColorPicker.KD = newValue;
        }

        private void SunZLocationHandler(float newValue)
        {
            this.sketcher.LightSource.LightLocationZ = newValue;
        }

        private void SunYLocationHandler(float newValue)
        {
            this.sketcher.LightSource.LightLocationY = newValue;
        }

        private void SunXLocationHandler(float newValue)
        {
            this.sketcher.LightSource.LightLocationX = newValue;
        }

        private void SunSpeedHanlder(float newValue)
        {
            this.sketcher.LightSource.LightSpeed = newValue;
        }

        private void SunHandler(bool newValue)
        {
            this.sketcher.LightSource.LightAnimation = !this.sketcher.LightSource.LightAnimation;
        }

        private void ShowLinesHandler(object? sender, EventArgs e)
        {
            this.sketcher.ShowLines = !this.sketcher.ShowLines;
            this.sketcher.Refresh();
        }

        private void FillHandler(bool obj)
        {
            this.sketcher.Fill = !this.sketcher.Fill;
            this.sketcher.Refresh();
        }

        private void OpenFileHandler(object? sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Object files (*.obj)|*.obj|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;

                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }

            this.sketcher.LoadObject(fileContent);
        }

        private void InitializeForm()
        {
            this.Text = Resources.ProgramTitle;
            this.MinimumSize = new Size(FormConstants.MinimumWindowSizeX, FormConstants.MinimumWindowSizeY);
            this.Size = new Size(FormConstants.InitialWindowSizeX, FormConstants.InitialWindowSizeY);
        }

        private void ArrangeComponents()
        {
            this.mainTableLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            this.mainTableLayout.ColumnCount = FormConstants.MainFormColumnCount;
            //this.mainTableLayout.ColumnStyles.Add(new ColumnStyle() { Width = FormConstants.ToolbarWidth });
            //this.mainTableLayout.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.AutoSize });

            this.mainTableLayout.Controls.Add(this.sketcher, 1, 0);
            this.mainTableLayout.Controls.Add(this.toolbar, 0, 0);
            this.mainTableLayout.Dock = DockStyle.Fill;
            this.Controls.Add(mainTableLayout);
        }
    }
}