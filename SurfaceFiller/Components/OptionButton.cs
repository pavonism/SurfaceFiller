using System.Drawing.Drawing2D;

namespace SurfaceFiller.Components
{
    /// <summary>
    /// Implementuje okrągły przycisk w formie checkboxa
    /// </summary>
    public class CheckButton : OptionButton
    {
        #region Fields and Properties
        private bool ticked;

        public bool Lock
        {
            get => ticked;
            set
            {
                ticked = value;
                BackColor = value ? Color.LightGray : Color.Transparent;
                FlatAppearance.BorderSize = value ? 1 : 0;
            }
        }
        #endregion

        #region Events
        public event Action<bool>? OnOptionChanged;

        private void OptionChanged(object? sender, EventArgs e)
        {
            Lock = !Lock;
            OnOptionChanged?.Invoke(ticked);
        }
        #endregion

        public CheckButton()
        {

            this.Click += OptionChanged;
        }
    }

    /// <summary>
    /// Implementuje okrągły przycisk
    /// </summary>
    public class OptionButton : Button
    {
        //protected override void OnPaint(PaintEventArgs pevent)
        //{
        //    var ellipseRect = new RectangleF(0, 0, Width, Height);

        //    using (var path = new GraphicsPath())
        //    {
        //        path.AddEllipse(ellipseRect);
        //        Region = new Region(path);
        //    }


        //    base.OnPaint(pevent);
        //}

        public OptionButton()
        {
            Width = FormConstants.MinimumControlSize;
            Height = FormConstants.MinimumControlSize;
            BackColor = Color.Transparent;
            ForeColor = Color.Black;
            FlatStyle = FlatStyle.Flat;
            TextAlign = ContentAlignment.MiddleCenter;
            Font = new Font("Arial", 14);
            FlatAppearance.BorderSize = 0;
        }
    }
}
