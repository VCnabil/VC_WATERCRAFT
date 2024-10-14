using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VC_WATERCRAFT.nabzuc
{
    public partial class uc_8bitTrack : uc_BaseControl
    {
        private TrackBar verticalTrackBar;
        private Label valueLabel;

        public uc_8bitTrack()
        {
            InitializeComponent();
            _numberOfBytes = 1;
            this.Priority = 0x18;
            InitializeCustomComponents();
            SetDefaults();

        }
        // Optionally override to provide custom SPN initialization



        private void InitializeCustomComponents()
        {
            // Initialize TrackBar
            verticalTrackBar = new TrackBar
            {
                Orientation = Orientation.Vertical,
                Minimum = 0,
                Maximum = GetMaxValueFromBytes(),

                TickStyle = TickStyle.Both,
                TickFrequency = 10,
                Width = 50, // Fixed width for better control over layout
                Height = Height - 10,
                Location = new Point(4, 10)
            };
            verticalTrackBar.ValueChanged += VerticalTrackBar_ValueChanged;

            // Initialize Label
            valueLabel = new Label
            {
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Location = new Point(28, 28),
                Text = currentValue.ToString(),
                Font = new Font(Font.FontFamily, 6, FontStyle.Bold),
                Height = 30
            };

            // Set minimum size for the UserControl
            this.MinimumSize = new Size(150, 200);
            this.BorderStyle = BorderStyle.FixedSingle;

            // Add controls
            this.Controls.Add(verticalTrackBar);
            this.Controls.Add(valueLabel);

            AdjustControlLayout();
        }

        private void VerticalTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (sender is TrackBar trackBar)
            {
                SetValue(trackBar.Value);
            }
        }

        protected override void UpdateValue(int value)
        {
            base.UpdateValue(value);

            if (verticalTrackBar != null && verticalTrackBar.Value != value)
            {
                verticalTrackBar.Value = value;
            }

            if (valueLabel != null)
            {
                valueLabel.Text = currentValue.ToString();
            }

            // Update SPN value to reflect the current value
            if (_spnLite != null)
            {
                _spnLite.Value = currentValue;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustControlLayout();
        }

        private void AdjustControlLayout()
        {
            if (verticalTrackBar != null)
            {
                verticalTrackBar.Height = Height - 40; // Ensure TrackBar height matches the control height
                verticalTrackBar.Location = new Point(10, 10);
            }

            if (valueLabel != null)
            {
                valueLabel.Location = new Point(verticalTrackBar.Right + 10, verticalTrackBar.Top);
            }
        }

        public void SetValue(int value)
        {
            UpdateValue(value);
            // Ensure the SPN value is updated when SetValue is called
            if (_spnLite != null)
            {
                _spnLite.Value = value;
            }
        }
    }
}
