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
    public partial class uc_8bitTrack : uc_BaseControl
    {
        private TrackBar verticalTrackBar;
        private Label valueLabel;
        public uc_8bitTrack()
        {
            InitializeComponent();
            _numberOfBytes = 1;  // Set the number of bytes
            this.Priority = 3;  // Set the default priority
            InitializeCustomComponents();  // Initialize custom components like TrackBar and Label
            SetDefaults();  // Apply default settings to the control
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

        private void InitializeCustomComponents()
        {
            verticalTrackBar = new TrackBar
            {
                Orientation = Orientation.Vertical,
                Minimum = 0,
                Maximum = GetMaxValueFromBytes(),
                TickStyle = TickStyle.Both,
                TickFrequency = 10,
                Width = 50,
                Location = new Point(4, 10)
            };
            verticalTrackBar.ValueChanged += VerticalTrackBar_ValueChanged;
            valueLabel = new Label
            {
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Location = new Point(28, 38),
                Text = currentValue.ToString(),
                Font = new Font(Font.FontFamily, 6, FontStyle.Bold),
                Height = 30
            };
            this.MinimumSize = new Size(150, 200);
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(verticalTrackBar);
            this.Controls.Add(valueLabel);
            AdjustControlLayout();
        }
        private void VerticalTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (sender is TrackBar trackBar)
            {
                if (trackBar.Value != currentValue)  // Prevent redundant updates
                {
                    SetValue(trackBar.Value);
                }
            }
        }
        protected override void UpdateValue(int value)
        {
            if (currentValue == value) return;
            base.UpdateValue(value);
            if (verticalTrackBar != null && verticalTrackBar.Value != value)
            {
                verticalTrackBar.Value = value;
            }
            if (valueLabel != null)
            {
                valueLabel.Text = currentValue.ToString();
            }
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
                int availableHeight = Height - 40;
                if (availableHeight > 0)  
                {
                    verticalTrackBar.Height = availableHeight; 
                }
                verticalTrackBar.Location = new Point(10, 10); 
            }

            if (valueLabel != null)
            {
                valueLabel.Location = new Point(verticalTrackBar.Right + 2, verticalTrackBar.Top + 20);
            }
        }

        public void SetValue(int value)
        {
            UpdateValue(value);
            if (_spnLite != null)
            {
                _spnLite.Value = value;
            }
        }
    }
}