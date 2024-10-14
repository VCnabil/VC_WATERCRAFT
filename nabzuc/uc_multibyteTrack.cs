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
    public partial class uc_multibyteTrack : uc_BaseControl
    {
        private TrackBar horizontalTrackBar;
        private Label valueLabel;
        private bool _isUpdating = false;
        public event EventHandler<int> ValueChanged;

        public uc_multibyteTrack()
        {
            InitializeComponent();
            _numberOfBytes = 2; // Set this before initializing custom components
            InitializeCustomComponents();
            UpdateValue(currentValue); // Update controls with the correct value
            AdjustControlLayout(); // Set initial positions
            //UpdateInfoLabel();  // Update info label based on SPN
        }

        // Optionally override to provide custom SPN initialization


        private void InitializeCustomComponents()
        {
            // Initialize valueLabel
            valueLabel = new Label
            {
                AutoSize = true,
                Text = $"val: {currentValue}",
                Font = new Font(this.Font.FontFamily, 7, FontStyle.Bold)
            };

            // Initialize horizontalTrackBar
            horizontalTrackBar = new TrackBar
            {
                Orientation = Orientation.Horizontal,
                Minimum = 0,
                Maximum = GetMaxValueFromBytes(),
                TickFrequency = GetMaxValueFromBytes() / 10,
            };
            horizontalTrackBar.ValueChanged += HorizontalTrackBar_ValueChanged;

            // Add controls to the UserControl
            this.Controls.Add(valueLabel);
            this.Controls.Add(horizontalTrackBar);
        }

        private void HorizontalTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdating) return;

            if (sender is TrackBar trackBar)
            {
                SetValue(trackBar.Value);
            }
        }

        // Override the UpdateValue method to ensure the TrackBar and label update
        protected override void UpdateValue(int value)
        {
            if (_isUpdating) return;

            try
            {
                _isUpdating = true;
                base.UpdateValue(value);

                // Update the TrackBar maximum based on the current MaxValue
                if (horizontalTrackBar != null)
                {
                    horizontalTrackBar.Maximum = MaxValue;  // Use the custom MaxValue
                    if (horizontalTrackBar.Value != value)
                    {
                        horizontalTrackBar.Value = value;
                    }
                }

                // Update the label
                if (valueLabel != null)
                {
                    valueLabel.Text = $"Value: {currentValue}";
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        // Ensure the TrackBar updates when the MaxValue changes
        public new int MaxValue
        {
            get => base.MaxValue;
            set
            {
                if (base.MaxValue != value)
                {
                    base.MaxValue = value;

                    // Update the TrackBar and label with the new MaxValue
                    horizontalTrackBar.Maximum = value;
                    horizontalTrackBar.TickFrequency = value / 10;
                    UpdateValue(currentValue);  // Ensure current value is clamped to new max
                }
            }
        }

        public void SetValue(int value)
        {
            UpdateValue(value);

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
            int padding = 10;
            int currentY = padding;

            // Adjust lbl_Info from base class
            if (lbl_Info != null)
            {
                lbl_Info.Location = new Point(padding, currentY);
                lbl_Info.AutoSize = true;
                currentY += lbl_Info.Height + padding;
            }

            // Adjust valueLabel
            if (valueLabel != null)
            {
                valueLabel.Location = new Point(padding, currentY);
                valueLabel.AutoSize = true;
                currentY += valueLabel.Height + padding;
            }

            // Adjust horizontalTrackBar
            if (horizontalTrackBar != null)
            {
                horizontalTrackBar.Width = this.Width - 2 * padding;
                horizontalTrackBar.Location = new Point(padding, currentY);
                // The Height of TrackBar is fixed; no need to adjust
            }
        }
    }
}
