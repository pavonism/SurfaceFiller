using SketcherControl;
using SurfaceFiller.Components;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
            this.toolbar.AddButton(OpenFileHandler, "📂", string.Empty);
            this.toolbar.AddTool(FillHandler, "🪣", string.Empty);
            this.toolbar.AddTool(SunHandler, "☀", string.Empty);
            this.toolbar.AddButton(ColorButton, "🎨", string.Empty);
            this.toolbar.AddTool(ShowTrackHandler, "T", string.Empty);
            this.toolbar.AddButton(ResetPositionButton, "R", string.Empty);
            this.toolbar.AddDivider();
            this.toolbar.AddOption("Hide lines", ShowLinesHandler);
            this.toolbar.AddDivider();
            this.toolbar.AddLabel("Light Parameters");
            this.toolbar.AddFractSlider(KDParameterHandler, "KD =", 0.5f);
            this.toolbar.AddFractSlider(KDParameterHandler, "KS =", 0.5f);
            this.toolbar.AddSlider(KDParameterHandler, "M =", 0.5f);
            this.toolbar.AddDivider();
            this.toolbar.AddLabel("Sun Parameters");
            this.toolbar.AddSlider(SunSpeedHanlder, "Speed", 0.5f);
            this.toolbar.AddSlider(SunXLocationHandler, "X", 0.5f);
            this.toolbar.AddSlider(SunYLocationHandler, "Y", 0.5f);
            this.toolbar.AddSlider(SunZLocationHandler, "Z", 0.5f);
            this.toolbar.AddDivider();
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

        private void RColorHandler(int obj)
        {
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

        private void KDParameterHandler(float newValue)
        {
        }

        private void SunHandler(bool obj)
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