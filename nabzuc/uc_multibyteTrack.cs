using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VC_WATERCRAFT._GLobalz;
namespace VC_WATERCRAFT.nabzuc
{
    public partial class uc_multibyteTrack : uc_BaseControl
    {
        private TrackBar horizontalTrackBar;
        private Label valueLabel;
        private bool _isUpdating = false;
        private const int TrackBarMaxLimit = 65535; // Maximum allowed by TrackBar
        private double scalingFactor = 1;
        public uc_multibyteTrack()
        {
            InitializeComponent();
            InitializeCustomComponents();
            UpdateValue(currentValue);
            AdjustControlLayout();
        }

        private void InitializeCustomComponents()
        {


            valueLabel = new Label
            {
                AutoSize = true,
                Text = $"Value: {currentValue}",
                Font = new Font(this.Font.FontFamily, 7, FontStyle.Bold)
            };
            horizontalTrackBar = new TrackBar
            {
                Orientation = Orientation.Horizontal,
                Minimum = 0,
                Maximum = MaxValue, // Use MaxValue here
                TickFrequency = MaxValue / 10 > 0 ? MaxValue / 10 : 1,
            };


            // Calculate scaling factor
            if (MaxValue > TrackBarMaxLimit)
            {
                scalingFactor = (double)TrackBarMaxLimit / MaxValue;
            }
            else
            {
                scalingFactor = 1;
            }

            horizontalTrackBar.Maximum = (int)(MaxValue * scalingFactor);
            horizontalTrackBar.TickFrequency = horizontalTrackBar.Maximum / 10 > 0 ? horizontalTrackBar.Maximum / 10 : 1;



            horizontalTrackBar.ValueChanged += HorizontalTrackBar_ValueChanged;
            this.Controls.Add(valueLabel);
            this.Controls.Add(horizontalTrackBar);
        }
        private void HorizontalTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            if (sender is TrackBar trackBar)
            {
                int scaledValue = (int)(trackBar.Value / scalingFactor);
                SetValue(scaledValue);
            }
        }
        protected override void InitializeSPN()
        {
            if (string.IsNullOrEmpty(SpnName))
            {
                SpnName = "DefaultSPNName";
            }
            _spnLite = new spnLite(SpnName, 0, _numberOfBytes, _firstByteIndex, _isLowByteFirst);
            _spnLite.SetPGNComponents(this.Priority, 0xFF69, 0x01);
        }
        protected override void UpdateValue(int value)
        {
            if (_isUpdating) return;
            try
            {
                _isUpdating = true;
                base.UpdateValue(value);

                if (horizontalTrackBar != null)
                {
                    int scaledValue = (int)(value * scalingFactor);
                    // Ensure value is within the valid range
                    int safeValue = Math.Max(horizontalTrackBar.Minimum, Math.Min(scaledValue, horizontalTrackBar.Maximum));
                    if (horizontalTrackBar.Value != safeValue)
                    {
                        horizontalTrackBar.Value = safeValue;
                    }
                }

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
        protected override void OnMaxValueChanged()
        {
            base.OnMaxValueChanged();
            if (horizontalTrackBar != null)
            {
                if (MaxValue > TrackBarMaxLimit)
                {
                    scalingFactor = (double)TrackBarMaxLimit / MaxValue;
                }
                else
                {
                    scalingFactor = 1;
                }

                horizontalTrackBar.Maximum = (int)(MaxValue * scalingFactor);
                horizontalTrackBar.TickFrequency = horizontalTrackBar.Maximum / 10 > 0 ? horizontalTrackBar.Maximum / 10 : 1;
            }
        }
        public void SetValue(int value)
        {
            UpdateValue(value);
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
            if (lbl_Info != null)
            {
                lbl_Info.Location = new Point(padding, currentY);
                lbl_Info.AutoSize = true;
                currentY += lbl_Info.Height + padding;
            }
            if (valueLabel != null)
            {
                valueLabel.Location = new Point(padding, currentY);
                valueLabel.AutoSize = true;
                currentY += valueLabel.Height + padding;
            }
            if (horizontalTrackBar != null)
            {
                horizontalTrackBar.Width = this.Width - 2 * padding;
                horizontalTrackBar.Location = new Point(padding, currentY);
            }
        }
    }
}