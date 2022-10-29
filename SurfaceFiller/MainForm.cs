using SketcherControl;
using SurfaceFiller.Components;
using System.Windows.Forms;

namespace SurfaceFiller
{
    public partial class MainForm : Form
    {
        private TableLayoutPanel mainTableLayout = new();
        private Toolbar toolbar = new();
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
            this.toolbar.AddButton(FillHandler, "🪣", string.Empty);
            this.toolbar.AddDivider();
        }

        private void FillHandler(object? sender, EventArgs e)
        {
            this.sketcher.Fill = true;
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
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
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
            this.mainTableLayout.ColumnStyles.Add(new ColumnStyle() { Width = FormConstants.ToolbarWidth });
            this.mainTableLayout.RowStyles.Add(new ColumnStyle() { SizeType = SizeType.AutoSize });

            this.mainTableLayout.Controls.Add(this.sketcher, 1, 0);
            this.mainTableLayout.Controls.Add(this.toolbar, 0, 0);
            this.mainTableLayout.Dock = DockStyle.Fill;
            this.Controls.Add(mainTableLayout);
        }
    }
}