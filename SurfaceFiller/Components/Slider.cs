using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfaceFiller.Components
{
    public class Slider : UserControl
    {
        private string labelText = string.Empty;
        public string LabelText
        {
            get => this.labelText;
            set
            {
                this.labelText = value;
                UpdateLabelText();
            }
        }

        private readonly TrackBar trackBar = new()
        {
            Minimum = 0,
            Maximum = 100,
            MaximumSize = new Size(int.MaxValue, FormConstants.MinimumControlSize),
            AutoSize = false,
            TickStyle = TickStyle.None,
        };

        private readonly TableLayoutPanel mainTable = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
        };

        private readonly Label label = new()
        {
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Anchor = AnchorStyles.Left,
            Font = new Font(Label.DefaultFont.Name, 11),
            Dock = DockStyle.Top,
        };

        public Slider()
        {
            this.Height = FormConstants.MinimumControlSize;
            this.Margin = new Padding(0, 5, 0, 5);

            this.trackBar.ValueChanged += TrackBar_ValueChanged;
            UpdateLabelText();

            mainTable.Controls.Add(trackBar);
            mainTable.Controls.Add(label);
            mainTable.SetColumn(trackBar, 0);
            mainTable.SetColumn(label, 1);
            this.Controls.Add(mainTable);
        }

        private void TrackBar_ValueChanged(object? sender, EventArgs e)
        {
            UpdateLabelText();
        }

        private void UpdateLabelText()
        {
            this.label.Text = $"{LabelText} {(float)this.trackBar.Value / 100}";
        }
    }
}
