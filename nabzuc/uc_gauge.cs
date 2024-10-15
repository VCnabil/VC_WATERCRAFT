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
    public partial class uc_gauge : uc_BaseControl
    {
        private const int BorderThickness = 2;
        private const int minimumWidth = 400;
        private const int minimumHeight = 400;
        public event EventHandler<int> RezValueChanged;
        private int _g_OFFSET_ROT_ANG = 270;
        private int _g_startAngle = 48;
        private int _g_endAngle = 315;
        private int _g_numTicks = 6;
        private int _g_minValue = 0;
        private int _g_maxValue = 200;
        private int _g_gaueValue = 0;
        private bool _isUpdating = false;

        #region Exposed Props
        [Category("0 Gauge")]
        [Description("The starting angle of the gauge.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int G_StartAngle
        {
            get { return _g_startAngle; }
            set { _g_startAngle = value; Invalidate(); }
        }

        [Category("0 Gauge")]
        [Description("The ending angle of the gauge.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int G_EndAngle
        {
            get { return _g_endAngle; }
            set { _g_endAngle = value; Invalidate(); }
        }

        [Category("0 Gauge")]
        [Description("The number of ticks on the gauge.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int G_NumTicks
        {
            get { return _g_numTicks; }
            set { _g_numTicks = value; Invalidate(); }
        }

        [Category("0 Gauge")]
        [Description("The minimum value of the gauge.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int G_MinValue
        {
            get { return _g_minValue; }
            set { _g_minValue = value; Invalidate(); }
        }

        [Category("0 Gauge")]
        [Description("The maximum value of the gauge.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int G_MaxValue
        {
            get { return _g_maxValue; }
            set { _g_maxValue = value; Invalidate(); }
        }

        [Category("0 Gauge")]
        [Description("The current value of the gauge.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int G_GaugeValue
        {
            get { return _g_gaueValue; }
            set
            {
                if (_g_gaueValue != value)
                {
                    _g_gaueValue = value;
                    Invalidate();
                    RezValueChanged?.Invoke(this, _g_gaueValue);
                }
            }
        }

        [Category("0 Gauge")]
        [Description("The offset rotation angle of the gauge.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int G_OffsetRotationAngle
        {
            get { return _g_OFFSET_ROT_ANG; }
            set { _g_OFFSET_ROT_ANG = value; Invalidate(); }
        }
        #endregion

        public uc_gauge()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;

            // Do not set NumberOfBytes here; let the designer or calling code set it

            // SetDefaults(); // Ensure defaults are set without overwriting MaxValue
           // UpdateValue(currentValue);
            InitializeSPN(); // SPN will be initialized with the updated priority
            this.Load += Uc_gauge_Load;

        }

        protected override void InitializeSPN()
        {
            if (string.IsNullOrEmpty(SpnName))
            {
                SpnName = "DefaultSPNName";  // Ensure a valid name is set before using it
            }

            _spnLite = new spnLite(SpnName, 0, _numberOfBytes, _firstByteIndex, _isLowByteFirst);
            _spnLite.SetPGNComponents(this.Priority, 0xFF69, 0x01);
        }

        protected override void UpdateValue(int value)
        {
            if (_isUpdating)
                return;

            try
            {
                _isUpdating = true;
                base.UpdateValue(value); // Call base class's UpdateValue to handle clamping by MaxValue
                Invalidate();
                UpdateInfoLabel(); // Update the label to show the latest information
            }
            finally
            {
                _isUpdating = false;
            }
        }
        protected override void OnMaxValueChanged()
        {
            base.OnMaxValueChanged();
            Invalidate(); // Redraw control when MaxValue changes

        }
  

        public void SetValue(int value)
        {
            UpdateValue(value);
        }


        private void Uc_gauge_Load(object sender, EventArgs e)
        {
            Invalidate(); // Force redraw when control is loaded
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate(); // Redraw control on resize to adjust layout
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            UpdateValueFromMouse(e.Location);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                UpdateValueFromMouse(e.Location);
            }
        }

        private void UpdateValueFromMouse(Point mouseLocation)
        {
            Point center = new Point(Width / 2, Height / 2);
            double dx = mouseLocation.X - center.X;
            double dy = mouseLocation.Y - center.Y;
            double angle = Math.Atan2(dy, dx) * 180 / Math.PI;

            if (angle < 0) angle += 360;
            angle = (angle + _g_OFFSET_ROT_ANG) % 360;

            double angleRange = _g_endAngle - _g_startAngle;
            double normalizedAngle = angle;

            if (_g_endAngle > _g_startAngle)
            {
                if (angle < _g_startAngle)
                    normalizedAngle = _g_startAngle;
                else if (angle > _g_endAngle)
                    normalizedAngle = _g_endAngle;
            }
            else
            {
                if (angle > _g_startAngle)
                    normalizedAngle = _g_startAngle;
                else if (angle < _g_endAngle)
                    normalizedAngle = _g_endAngle;
            }

            int valueRange = MaxValue - _g_minValue;
            int value = _g_minValue + (int)((normalizedAngle - _g_startAngle) / angleRange * valueRange);
            SetValue(value);

            if (_spnLite != null)
            {
                _spnLite.Value = currentValue;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            try
            {
                if (Width < 10 || Height < 10)
                {
                    return;
                }
                DrawGauge(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error drawing gauge: " + ex.Message);
            }
        }

        void DrawGauge(PaintEventArgs e)
        {
            int radius = Math.Min(Width, Height) / 2 - 1;
            Point center = new Point(Width / 2, Height / 2);

            // Draw gauge background
            e.Graphics.FillEllipse(Brushes.LightGray, center.X - radius, center.Y - radius, radius * 2, radius * 2);

            // Draw gauge ticks
            for (int i = 0; i <= _g_numTicks; i++)
            {
                float angle = _g_startAngle + (float)i / _g_numTicks * (_g_endAngle - _g_startAngle);
                float adjustedAngle = (angle - _g_OFFSET_ROT_ANG) % 360;
                float radian = adjustedAngle * (float)Math.PI / 180f;

                Point tickStart = new Point(
                    (int)(center.X + Math.Cos(radian) * (radius - 10)),
                    (int)(center.Y + Math.Sin(radian) * (radius - 10))
                );
                Point tickEnd = new Point(
                    (int)(center.X + Math.Cos(radian) * radius),
                    (int)(center.Y + Math.Sin(radian) * radius)
                );

                e.Graphics.DrawLine(Pens.Black, tickStart, tickEnd);

                int tickValue = _g_minValue + (int)((float)i / _g_numTicks * (MaxValue - _g_minValue));
                SizeF labelSize = e.Graphics.MeasureString(tickValue.ToString(), this.Font);

                Point labelPosition = new Point(
                    (int)(center.X + Math.Cos(radian) * (radius - 20) - labelSize.Width / 2),
                    (int)(center.Y + Math.Sin(radian) * (radius - 20) - labelSize.Height / 2)
                );

                e.Graphics.DrawString(tickValue.ToString(), this.Font, Brushes.Black, labelPosition);
            }

            // Draw the needle
            float valueAngle = _g_startAngle + (float)(currentValue - _g_minValue) / (MaxValue - _g_minValue) * (_g_endAngle - _g_startAngle);
            float adjustedValueAngle = (valueAngle - _g_OFFSET_ROT_ANG) % 360;
            float needleRadian = adjustedValueAngle * (float)Math.PI / 180f;

            Point needleEnd = new Point(
                (int)(center.X + Math.Cos(needleRadian) * radius),
                (int)(center.Y + Math.Sin(needleRadian) * radius)
            );

            e.Graphics.DrawLine(new Pen(Color.Red, 4), center, needleEnd);

            // Draw the value label at the bottom left
            string valueText = currentValue.ToString();
            SizeF valueLabelSize = e.Graphics.MeasureString(valueText, this.Font);

            // Calculate bottom-left position (adjust with padding if necessary)
            Point valueLabelPosition = new Point(
                5,  // Add a small left padding
                this.Height - (int)valueLabelSize.Height - 5 // Bottom-left with padding
            );

            e.Graphics.DrawString(valueText, this.Font, Brushes.Black, valueLabelPosition);
        }





    }
}
