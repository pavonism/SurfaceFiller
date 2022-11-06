﻿
namespace SurfaceFiller.Components
{
    /// <summary>
    /// Umożliwia tworzenie paska z opcjami i guzikami
    /// </summary>
    public class Toolbar : FlowLayoutPanel
    {
        public Toolbar()
        {
            Dock = DockStyle.Fill;
            Padding = Padding.Empty;
            Height = FormConstants.MinimumControlSize;
        }

        public void AddLabel(string text)
        {
            var label = new Label()
            {
                Font = new Font(DefaultFont, FontStyle.Bold),
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = FormConstants.MinimumControlSize,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
            };

            var table = new TableLayoutPanel()
            {
                Dock = DockStyle.Top,
                Height = label.Height,
            };

            table.Controls.Add(label);
            Controls.Add(table);
        }

        public void AddDivider()
        {
            Controls.Add(new Divider());
        }

        private T AddSlider<T>(string labelText) where T : Slider, new()
        {
            var slider = new T()
            {
                Dock = DockStyle.Top,
                Width = this.Width,
                LabelText = labelText,
            };

            Controls.Add(slider);
            return slider;
        }

        public void AddColorSlider(Action<int> handler, string labelText, int defaultValue = 0)
        {
            var slider = AddSlider<ColorSlider>(labelText);
            slider.ValueChanged += handler;
            slider.Value = defaultValue;
        }


        public void AddFractSlider(Action<float> handler, string labelText, float defaultValue = 0)
        {
            var slider = AddSlider<FractSlider>(labelText);
            slider.ValueChanged += handler;
            slider.Value = defaultValue;
        }

        public void AddSlider(Action<float> handler, string labelText, float defaultValue = 0)
        {
            var slider = AddSlider<PercentageSlider>(labelText);
            slider.ValueChanged += handler;
            slider.Value = defaultValue;
        }

        public void AddPlayPouse(Action<bool> handler, string hint, bool defaultState)
        {
            var button = new PlayPouseButton();
            button.Lock = defaultState;
            button.OnOptionChanged += handler;
            Controls.Add(button);
        }

        public Button AddButton(EventHandler handler, string glyph, string hint)
        {
            var button = new OptionButton()
            {
                Text = glyph,
                Margin = new Padding(2, 2, 2, 2),
            };

            var tooltip = new ToolTip();
            tooltip.SetToolTip(button, hint);
            button.Click += handler;
            Controls.Add(button);

            return button;
        }

        public CheckButton AddTool(Action<bool> handler, string glyph, string hint)
        {
            var button = new CheckButton()
            {
                Width = FormConstants.MinimumControlSize,
                Height = FormConstants.MinimumControlSize,
                Margin = new Padding(2, 2, 2, 2),
                Text = glyph,
            };

            button.OnOptionChanged += handler;



            var tooltip = new ToolTip();
            tooltip.SetToolTip(button, hint);
            Controls.Add(button);
            return button;
        }

        public CheckBox AddOption(string text, EventHandler onOptionChanged, string? hint = null)
        {
            var checkBox = new CheckBox()
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = text,
            };

            var table = new TableLayoutPanel()
            {
                Dock = DockStyle.Top,
                Height = checkBox.Height,
            };
            table.Controls.Add(checkBox);


            if (hint != null)
            {
                var tooltipControl = new ToolTip();
                tooltipControl.SetToolTip(checkBox, hint);
            }

            checkBox.CheckedChanged += onOptionChanged;
            Controls.Add(table);
            return checkBox;
        }
    }
}
