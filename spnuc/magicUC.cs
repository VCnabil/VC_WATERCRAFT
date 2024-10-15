using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VC_WATERCRAFT.spnuc
{
    public partial class magicUC : UserControl
    {
        // Exposed properties
        private int _m_minval = 0;
        private int _m_maxval = 100;
        private int _m_curval = 50;
        private int _m_ticks = 10;
        private Color _sliderColor = Color.Red;
        private Color _startColor = Color.LightBlue;
        private Color _endColor = Color.LightGray;


        private int _g_OFFSET_ROT_ANG = 270;
        private int _g_startAngle = 48;
        private int _g_endAngle = 315;
       
      
        private bool _isUpdating = false;

        // Enum for selecting the control mode
        public enum ControlModes
        {
            Trackbar,
            Gauge
        }

        private ControlModes _controlMode = ControlModes.Trackbar;


        [Category("MagicUC Properties"), Description("Set the control mode"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public ControlModes ControlMode
        {
            get { return _controlMode; }
            set { _controlMode = value; Invalidate(); }
        }


        [Category("MagicUC Properties"), Description("Minimum value "), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public int m_minval
        {
            get { return _m_minval; }
            set { _m_minval = value; Invalidate(); }
        }


        [Category("MagicUC Properties"), Description("Maximum value "), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public int m_maxval
        {
            get { return _m_maxval; }
            set { _m_maxval = value; Invalidate(); }
        }


        [Category("MagicUC Properties"), Description("Current value "), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public int m_curval
        {
            get { return _m_curval; }
            set
            {
                _m_curval = Math.Max(_m_minval, Math.Min(_m_maxval, value));
                Invalidate(); // Redraw when the value changes
            }
        }


        [Category("MagicUC Properties"), Description("Number of ticks"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public int m_ticks
        {
            get { return _m_ticks; }
            set { _m_ticks = value; Invalidate(); }
        }


        [Category("MagicUC Properties"), Description("Slider color."), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public Color SliderColor
        {
            get { return _sliderColor; }
            set { _sliderColor = value; Invalidate(); }
        }

        [Category("MagicUC Properties"), Description("Start trackbar color"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public Color StartColor
        {
            get { return _startColor; }
            set { _startColor = value; Invalidate(); }
        }
        [Category("MagicUC Properties"), Description("End trackbar color "), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public Color EndColor
        {
            get { return _endColor; }
            set { _endColor = value; Invalidate(); }
        }
        [Category("MagicUC Properties"), Description("Offset angle "), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public int G_OffsetRotationAngle
        {
            get { return _g_OFFSET_ROT_ANG; }
            set { _g_OFFSET_ROT_ANG = value; Invalidate(); }
        }
        [Category("MagicUC Properties"), Description("Start angle "), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public int G_StartAngle
        {
            get { return _g_startAngle; }
            set { _g_startAngle = value; Invalidate(); }
        }
        [Category("MagicUC Properties"), Description("End angle "), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public int G_EndAngle
        {
            get { return _g_endAngle; }
            set { _g_endAngle = value; Invalidate(); }
        }
      


        public magicUC()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // To reduce flicker
            this.MouseDown += MagicUC_MouseDown;
            this.MouseMove += MagicUC_MouseMove;
        }

        private void MagicUC_MouseDown(object sender, MouseEventArgs e)
        {
            if (_controlMode == ControlModes.Trackbar)
            {
                UpdateSliderPosition(e.X);
            }
            else if (_controlMode == ControlModes.Gauge)
            {
                UpdateValueFromMouse(e.Location);
            }
        }

        private void MagicUC_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_controlMode == ControlModes.Trackbar)
                {
                    UpdateSliderPosition(e.X);
                }
                else if (_controlMode == ControlModes.Gauge)
                {
                    UpdateValueFromMouse(e.Location);
                }
            }
        }

        // Programmatically set the value and update the UI
        public void SetValue(int value)
        {
            m_curval = value; // This automatically triggers Invalidate() in the setter
        }

        private void UpdateSliderPosition(int mouseX)
        {
            float ratio = (float)(mouseX - 10) / (Width - 20); // Exclude padding for the slider
            SetValue((int)(_m_minval + ratio * (_m_maxval - _m_minval)));
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

            int valueRange = m_maxval - m_minval;
            int value = m_minval + (int)((normalizedAngle - _g_startAngle) / angleRange * valueRange);
            SetValue(value);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_controlMode == ControlModes.Trackbar)
            {
                DrawTrackbar(e);
            }
            else if (_controlMode == ControlModes.Gauge)
            {
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
        }

        private void DrawTrackbar(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int trackHeight = 10;
            int sliderSize = 20;
            int trackY = Height / 2 - trackHeight / 2;

            // Draw gradient track
            using (LinearGradientBrush brush = new LinearGradientBrush(
                new Rectangle(10, trackY, Width - 20, trackHeight),
                _startColor, _endColor, 0F))
            {
                g.FillRectangle(brush, 10, trackY, Width - 20, trackHeight);
            }

            // Draw ticks
            if (_m_ticks > 0)
            {
                float tickSpacing = (float)(Width - 20) / (_m_ticks - 1);
                for (int i = 0; i < _m_ticks; i++)
                {
                    int tickX = 10 + (int)(i * tickSpacing);
                    g.DrawLine(Pens.Black, tickX, trackY + trackHeight + 5, tickX, trackY + trackHeight + 15);
                }
            }

            // Draw slider
            float valueRatio = (float)(_m_curval - _m_minval) / (_m_maxval - _m_minval);
            int sliderX = 10 + (int)(valueRatio * (Width - 20)) - sliderSize / 2;

            using (Brush sliderBrush = new SolidBrush(_sliderColor))
            {
                g.FillEllipse(sliderBrush, sliderX, trackY - sliderSize / 2 + trackHeight / 2, sliderSize, sliderSize);
            }

            // Draw the value label at the bottom left
            string valueText = m_curval.ToString();
            SizeF valueLabelSize = e.Graphics.MeasureString(valueText, this.Font);

            // Calculate bottom-left position (adjust with padding if necessary)
            Point valueLabelPosition = new Point(
                5,  // Add a small left padding
                this.Height - (int)valueLabelSize.Height - 5 // Bottom-left with padding
            );

            e.Graphics.DrawString(valueText, this.Font, Brushes.Black, valueLabelPosition);
        }

        private void DrawGauge(PaintEventArgs e)
        {
            int radius = Math.Min(Width, Height) / 2 - 1;
            Point center = new Point(Width / 2, Height / 2);

            // Draw gauge background
            e.Graphics.FillEllipse(Brushes.LightGray, center.X - radius, center.Y - radius, radius * 2, radius * 2);

            // Draw gauge ticks
            for (int i = 0; i <= _m_ticks; i++)
            {
                float angle = _g_startAngle + (float)i / _m_ticks * (_g_endAngle - _g_startAngle);
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

                int tickValue = _m_minval + (int)((float)i / _m_ticks * (_m_maxval - _m_minval));
                SizeF labelSize = e.Graphics.MeasureString(tickValue.ToString(), this.Font);

                Point labelPosition = new Point(
                    (int)(center.X + Math.Cos(radian) * (radius - 20) - labelSize.Width / 2),
                    (int)(center.Y + Math.Sin(radian) * (radius - 20) - labelSize.Height / 2)
                );

                e.Graphics.DrawString(tickValue.ToString(), this.Font, Brushes.Black, labelPosition);
            }

            // Draw the needle
            float valueAngle = _g_startAngle + (float)(m_curval - _m_minval) / (_m_maxval - _m_minval) * (_g_endAngle - _g_startAngle);
            float adjustedValueAngle = (valueAngle - _g_OFFSET_ROT_ANG) % 360;
            float needleRadian = adjustedValueAngle * (float)Math.PI / 180f;

            Point needleEnd = new Point(
                (int)(center.X + Math.Cos(needleRadian) * radius),
                (int)(center.Y + Math.Sin(needleRadian) * radius)
            );

            e.Graphics.DrawLine(new Pen(Color.Red, 4), center, needleEnd);

            // Draw the value label at the bottom left
            string valueText = m_curval.ToString();
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
