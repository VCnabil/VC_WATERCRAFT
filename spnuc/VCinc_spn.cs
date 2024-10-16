using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VC_WATERCRAFT.spnuc
{
    public partial class VCinc_spn : UserControl
    {
        public VCinc_spn()
        {
            InitializeComponent();
            SetDoubleBuffered(panel_visualizer);
            textBox_value.TextChanged += (s, e) =>
            {
                if (int.TryParse(textBox_value.Text, out int newValue))
                {
                    Value = newValue;
                }
            };
            lbl_scrollingTitle.MouseEnter += Lbl_scrollingTitle_MouseEnter;
            lbl_scrollingTitle.MouseLeave += Lbl_scrollingTitle_MouseLeave;
            panel_visualizer.Paint += Panel_visualizer_Paint;
            panel_visualizer.MouseDown += Panel_visualizer_MouseDown;
            panel_visualizer.MouseMove += Panel_visualizer_MouseMove;
        }

        public enum ControlModes
        {
            SingleByte,
            Trackbar,
            Gauge,
            // EightBitsMode
        }
        private Timer scrollTimer;
        private ControlModes _controlMode = ControlModes.SingleByte;
        private int _m_NumberOfBytes = 1;
        private int _m_minval = 0;
        private int _m_val = 0;
        private int _m_maxval = 255;
        private int scrollIndex = 0;

        private int bitWidth = 17;
        private int bitHeight = 17;
        private int spacing = 6;

        private int _m_ticks = 10;


        private int _g_OFFSET_ROT_ANG = 270;
        private int _g_startAngle = 48;
        private int _g_endAngle = 315;

        int _firstByteIndex = 0;
        bool _isLowByteFirst = true;

        string _prio = "18";
        string _pgn = "0000";
        string _address = "00";
        string _spnName = "spn";

        private string[] _bitTitles = new string[8];
        private Rectangle[] _bitRectangles = new Rectangle[8];
        // Initialize a ToolTip component
        private ToolTip _toolTip = new ToolTip
        {
            AutoPopDelay = 1000,  // Show tooltip for 1 second
            InitialDelay = 100,   // Initial delay before showing tooltip
            ReshowDelay = 100,    // Delay before showing again if hovered quickly
            ShowAlways = true     // Show tooltip even when form is not active
        };
        [Category("0Behavior"), Description("Sets the mode of the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(ControlModes.SingleByte)]
        [RefreshProperties(RefreshProperties.All)]
        public ControlModes ControlMode
        {
            get { return _controlMode; }
            set
            {
                _controlMode = value;
                if (DesignMode)
                {
                    if (_controlMode == ControlModes.SingleByte)
                    {
                        _m_NumberOfBytes = 1;
                        RecalculateMinMax();
                        ValidateMinMax();
                        ValidateValue();
                    }
                    UpdateScrollingTitle();
                    panel_visualizer.Invalidate();
                }
            }
        }
        [Category("0Behavior"), Description("Number of bytes")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(1)]  // Set the default value for serialization
        [RefreshProperties(RefreshProperties.All)]
        public int NumberOfBytes
        {
            get { return _m_NumberOfBytes; }
            set
            {
                if (value < 1 || value > 4)
                    throw new ArgumentOutOfRangeException("NumberOfBytes must be between 1 and 4");

                _m_NumberOfBytes = value;
                if (DesignMode)
                {
                    RecalculateMinMax();
                    ValidateValue();
                }
                UpdateScrollingTitle();
                panel_visualizer.Invalidate();
            }
        }
        [Category("0Behavior"), Description("min")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(0)]
        public int MinValue
        {
            get { return _m_minval; }
            set
            {
                _m_minval = value;
                if (DesignMode)
                {
                    ValidateMinMax();
                    ValidateValue();
                }

                UpdateScrollingTitle();
                panel_visualizer.Invalidate();
            }
        }
        [Category("0Behavior"), Description("max")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(255)]
        public int MaxValue
        {
            get { return _m_maxval; }
            set
            {
                _m_maxval = value;

                if (DesignMode)
                {
                    ValidateMinMax();
                    ValidateValue();
                }
                UpdateScrollingTitle();
                panel_visualizer.Invalidate();
            }
        }
        [Category("0Behavior"), Description("curval")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(0)]
        public int Value
        {
            get { return _m_val; }
            set
            {
                if (value < _m_minval)
                    _m_val = _m_minval;
                else if (value > _m_maxval)
                    _m_val = _m_maxval;
                else
                    _m_val = value;

                textBox_value.Text = _m_val.ToString();
                UpdateScrollingTitle();
                panel_visualizer.Invalidate();
            }
        }
        [Category("0Behavior"), Description("ticks")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(10)]
        public int m_ticks
        {
            get { return _m_ticks; }
            set
            {
                _m_ticks = value;
                UpdateScrollingTitle();
                panel_visualizer.Invalidate();
            }
        }

        [Category("0Behavior"), Description("offset rot")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(270)]
        public int G_OffsetRotationAngle
        {
            get { return _g_OFFSET_ROT_ANG; }
            set
            {
                if (!DesignMode)
                {
                    _g_OFFSET_ROT_ANG = value;
                    Invalidate();
                }
                else
                {
                    _g_OFFSET_ROT_ANG = value;
                }
            }
        }
        [Category("0Behavior"), Description("startang")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(48)]
        public int G_StartAngle
        {
            get { return _g_startAngle; }
            set
            {
                if (!DesignMode)
                {
                    _g_startAngle = value;
                    Invalidate();
                }
                else
                {
                    _g_startAngle = value;
                }
            }
        }
        [Category("0Behavior"), Description("endang")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(315)]
        public int G_EndAngle
        {
            get { return _g_endAngle; }
            set
            {
                if (!DesignMode)
                {
                    _g_endAngle = value;
                    Invalidate();
                }
                else
                {
                    _g_endAngle = value;
                }
            }
        }



        [Category("0Behavior"), Description("first irstByteIndex")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(0)]
        public int A_FirstByteIndex
        {
            get => _firstByteIndex;
            set
            {
                if (value < 0) value = 0;
                int maxIndex = 8 - _m_NumberOfBytes;
                if (value > maxIndex) value = maxIndex;

                UpdateScrollingTitle();
                Invalidate();
            }
        }

        [Category("0Behavior"), Description("lo hi ")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(true)]
        public bool IsLowByteFirst
        {
            get => _isLowByteFirst;
            set
            {
                _isLowByteFirst = value;
                UpdateScrollingTitle();
                Invalidate();
            }
        }
        [Category("0Behavior"), Description("ADR")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("ff")]
        public string Address
        {
            get => _address;
            set
            {
                if (IsValidHexFormat(value, 2))
                {
                    _address = value.ToUpper();
                }
                else
                {
                    _address = "ff";
                }
                //  Invalidate();
            }
        }
        [Category("0Behavior"), Description("PGN")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("ffff")]
        public string PGN
        {
            get => _pgn;
            set
            {
                if (IsValidHexFormat(value, 4))
                {
                    _pgn = value.ToUpper();
                }
                else
                {
                    _pgn = "ffff";
                }
                UpdateScrollingTitle();
                // Invalidate();
            }
        }

        [Category("0Behavior"), Description("PRIO")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("18")]
        public string PRIO
        {
            get => _prio;
            set
            {
                if (IsValidHexFormat(value, 2))
                {
                    _prio = value.ToUpper();
                }
                else
                {
                    _prio = "18";
                }
                UpdateScrollingTitle();
                // Invalidate();
            }
        }
        [Category("0Behavior"), Description("spn name")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("spn")]
        public string SPNName
        {
            get => _spnName;
            set
            {
                _spnName = value;
                UpdateScrollingTitle();
                // Invalidate();
            }
        }

        [Category("0 8Bits")]
        [Description("Name of Bit 0.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public string Bit0Title
        {
            get => _bitTitles[0];
            set
            {
                _bitTitles[0] = value;

            }
        }

        [Category("0 8Bits")]
        [Description("Name of Bit 1.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public string Bit1Title
        {
            get => _bitTitles[1];
            set
            {
                _bitTitles[1] = value;

            }
        }

        [Category("0 8Bits")]
        [Description("Name of Bit 2.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]

        public string Bit2Title
        {
            get => _bitTitles[2];
            set
            {
                _bitTitles[2] = value;

            }
        }

        [Category("0 8Bits")]
        [Description("Name of Bit 3.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public string Bit3Title
        {
            get => _bitTitles[3];
            set
            {
                _bitTitles[3] = value;

            }
        }

        [Category("0 8Bits")]
        [Description("Name of Bit 4.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public string Bit4Title
        {
            get => _bitTitles[4];
            set
            {
                _bitTitles[4] = value;

            }
        }

        [Category("0 8Bits")]
        [Description("Name of Bit 5.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public string Bit5Title
        {
            get => _bitTitles[5];
            set
            {
                _bitTitles[5] = value;

            }
        }

        [Category("0 8Bits")]
        [Description("Name of Bit 6.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public string Bit6Title
        {
            get => _bitTitles[6];
            set
            {
                _bitTitles[6] = value;

            }
        }

        [Category("0 8Bits")]
        [Description("Name of Bit 7.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public string Bit7Title
        {
            get => _bitTitles[7];
            set
            {
                _bitTitles[7] = value;

            }
        }







        bool IsValidHexFormat(string value, int requiredLength)
        {
            return value.Length == requiredLength && Regex.IsMatch(value, @"\A\b[0-9a-fA-F]+\b\Z");
        }




        private void RecalculateMinMax()
        {
            if (DesignMode)
            {
                int maxRange = (int)Math.Pow(256, _m_NumberOfBytes) - 1;
                _m_minval = 0;
                _m_maxval = maxRange;
            }

        }

        private void ValidateMinMax()
        {
            if (_m_minval > _m_maxval)
                _m_minval = _m_maxval;
        }

        private void ValidateValue()
        {
            if (_m_val < _m_minval)
                _m_val = _m_minval;
            else if (_m_val > _m_maxval)
                _m_val = _m_maxval;
        }

        private void UpdateScrollingTitle()
        {

            lbl_scrollingTitle.Text = $"Mode: {_controlMode}, NumberOfBytes: {_m_NumberOfBytes}, Min: {_m_minval}, Max: {_m_maxval}, Value: {_m_val}, Ticks: {_m_ticks}, Offset: {_g_OFFSET_ROT_ANG}, Start: {_g_startAngle}, End: {_g_endAngle}, FirstByteIndex: {_firstByteIndex}, LowByteFirst: {_isLowByteFirst}, Address: {_address}, PGN: {_pgn}, PRIO: {_prio}, SPNName: {_spnName}";


        }

        private void SetDoubleBuffered(Control control)
        {
            System.Reflection.PropertyInfo aProp = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            aProp.SetValue(control, true, null);
        }

        private void Lbl_scrollingTitle_MouseEnter(object sender, EventArgs e)
        {
            scrollIndex = 0;
            scrollTimer = new Timer { Interval = 150 };
            string originalText = lbl_scrollingTitle.Text;
            scrollTimer.Tick += (s, args) =>
            {
                if (originalText.Length > 0)
                {
                    scrollIndex = (scrollIndex + 1) % originalText.Length;
                    lbl_scrollingTitle.Text = originalText.Substring(scrollIndex) + " " + originalText.Substring(0, scrollIndex);
                }
            };
            scrollTimer.Start();
        }

        private void Lbl_scrollingTitle_MouseLeave(object sender, EventArgs e)
        {
            if (scrollTimer != null)
            {
                scrollTimer.Stop();
                scrollTimer.Dispose();
                scrollTimer = null;
                UpdateScrollingTitle();
            }
        }

        private void Panel_visualizer_Paint(object sender, PaintEventArgs e)
        {
            switch (_controlMode)
            {
                case ControlModes.SingleByte:
                    DrawEightBitsMode(e);
                    break;
                case ControlModes.Trackbar:
                    DrawTrackbar(e);
                    break;
                case ControlModes.Gauge:
                    DrawGauge(e);
                    break;

            }
        }

        private void DrawTrackbar(PaintEventArgs e)
        {
            if (panel_visualizer.Width <= 20 || panel_visualizer.Height <= 0) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int trackHeight = 10;
            int sliderSize = 20;
            int trackY = panel_visualizer.Height / 2 - trackHeight / 2;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                new Rectangle(10, trackY, panel_visualizer.Width - 20, trackHeight),
                Color.LightGray, Color.Gray, 0F))
            {
                g.FillRectangle(brush, 10, trackY, panel_visualizer.Width - 20, trackHeight);
            }
            if (_m_maxval > _m_minval)
            {
                float valueRatio = (float)(_m_val - _m_minval) / (_m_maxval - _m_minval);
                int sliderX = 10 + (int)(valueRatio * (panel_visualizer.Width - 20)) - sliderSize / 2;
                using (Brush sliderBrush = new SolidBrush(Color.Blue))
                {
                    g.FillEllipse(sliderBrush, sliderX, trackY - sliderSize / 2 + trackHeight / 2, sliderSize, sliderSize);
                }
            }
        }

        private void DrawGauge(PaintEventArgs e)
        {
            int radius = Math.Min(panel_visualizer.Width, panel_visualizer.Height) / 2 - 1;
            Point center = new Point(panel_visualizer.Width / 2, panel_visualizer.Height / 2);
            e.Graphics.FillEllipse(Brushes.LightGray, center.X - radius, center.Y - radius, radius * 2, radius * 2);
            for (int i = 0; i <= 10; i++) // Assuming _m_ticks = 10
            {
                float angle = 135 + (float)i / 10 * 270; // Gauge from 135 to 405 degrees
                float radian = angle * (float)Math.PI / 180f;
                Point tickStart = new Point(
                    (int)(center.X + Math.Cos(radian) * (radius - 10)),
                    (int)(center.Y + Math.Sin(radian) * (radius - 10))
                );
                Point tickEnd = new Point(
                    (int)(center.X + Math.Cos(radian) * radius),
                    (int)(center.Y + Math.Sin(radian) * radius)
                );
                e.Graphics.DrawLine(Pens.Black, tickStart, tickEnd);
            }
            float valueAngle = 135 + (float)(_m_val - _m_minval) / (_m_maxval - _m_minval) * 270;
            float needleRadian = valueAngle * (float)Math.PI / 180f;
            Point needleEnd = new Point(
                (int)(center.X + Math.Cos(needleRadian) * radius),
                (int)(center.Y + Math.Sin(needleRadian) * radius)
            );
            e.Graphics.DrawLine(new Pen(Color.Red, 4), center, needleEnd);
        }

        private void DrawEightBitsMode(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int startX = 10;
            int startY = panel_visualizer.Height / 2 - bitHeight / 2;

            for (int i = 0; i < 8; i++)
            {
                bool isBitSet = (_m_val & (1 << i)) != 0;
                Color bitColor = isBitSet ? Color.Green : Color.Gray;
                Brush bitBrush = new SolidBrush(bitColor);

                Rectangle bitRect = new Rectangle(startX + i * (bitWidth + spacing), startY, bitWidth, bitHeight);
                g.FillRectangle(bitBrush, bitRect);
                g.DrawRectangle(Pens.Black, bitRect);
                // Store the rectangle for tooltip detection
                _bitRectangles[i] = bitRect;
                bitBrush.Dispose();
            }
        }

        private void Panel_visualizer_MouseDown(object sender, MouseEventArgs e)
        {
            if (_controlMode == ControlModes.SingleByte)
            {
                HandleBitToggle(e.Location);
            }
            else if (_controlMode == ControlModes.Trackbar)
            {
                UpdateSliderPosition(e.X);
            }
            else if (_controlMode == ControlModes.Gauge)
            {
                UpdateValueFromMouse(e.Location);
            }
        }

        private void HandleBitToggle(Point mouseLocation)
        {
            int startX = 2;
            int startY = panel_visualizer.Height / 2 - bitHeight / 2;
            for (int i = 0; i < 8; i++)
            {
                Rectangle bitRect = new Rectangle(startX + i * (bitWidth + spacing), startY, bitWidth, bitHeight);
                if (bitRect.Contains(mouseLocation))
                {
                    _m_val ^= (1 << i);
                    panel_visualizer.Invalidate();
                    break;
                }
            }


            SetValue(_m_val);

        }

        private void Panel_visualizer_MouseMove(object sender, MouseEventArgs e)
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
                else if (_controlMode == ControlModes.SingleByte)
                {
                    for (int i = 0; i < _bitRectangles.Length; i++)
                    {
                        if (_bitRectangles[i].Contains(e.Location))
                        {
                            // Show tooltip with the bit title
                            _toolTip.SetToolTip(panel_visualizer, $"Bit {i}: {_bitTitles[i]}");
                            return;
                        }
                    }

                    // If no square is hovered, hide the tooltip
                    _toolTip.SetToolTip(panel_visualizer, string.Empty);
                }
            }
        }

        private void UpdateSliderPosition(int mouseX)
        {
            float ratio = (float)(mouseX - 10) / (panel_visualizer.Width - 20);
            SetValue((int)(_m_minval + ratio * (_m_maxval - _m_minval)));
        }

        private void UpdateValueFromMouse(Point mouseLocation)
        {
            Point center = new Point(panel_visualizer.Width / 2, panel_visualizer.Height / 2);
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
            int valueRange = _m_maxval - _m_minval;
            int value = _m_minval + (int)((normalizedAngle - _g_startAngle) / angleRange * valueRange);
            SetValue(value);
        }

        private void SetValue(int value)
        {
            Value = value;
        }

    }
}
