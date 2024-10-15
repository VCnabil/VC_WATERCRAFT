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
    public partial class uc_BaseControl : UserControl
    {

        // Control Fields
        protected Label lbl_Info;
        protected int currentValue;
        protected int _firstByteIndex = 0;
        protected int _numberOfBytes = 1;
        public event EventHandler<int> ValueChanged;

        // Configuration Fields
        private string _pgn;
        private string _address;
        protected bool _isLowByteFirst;
        private readonly string defaultPGN = "FFFF";
        private readonly string defaultAddress = "00";
        private readonly bool defaultIsLowByteFirst = false;
        private readonly int defaultNumberOfBytes = 1;
        private readonly int defaultFirstByteIndex = 0;
        protected int default_priority = 3;
        private string _myname = "";
        protected spnLite _spnLite;

        // UI Fields
        private Color _borderColor = Color.Black;
        private const int BorderThickness = 2;
        private Timer _scrollTimer;
        private int _scrollPosition = 0;
        private string _fullTitle;
        private bool _isHovered = false;

        // Default Values
        private int _maxValue = 255;
        private int _priority = 3;
        public uc_BaseControl()
        {
            InitializeComponent(); // This should be the first method in the constructor
            InitializeScrollingTitle();  // Initialize the timer immediately
            InitializeControl();
            InitializeSPN();
           // SetDefaults();
            this.Paint += DrawBorder;
        }
        private void InitializeScrollingTitle()
        {
            _scrollTimer = new Timer();
            _scrollTimer.Interval = 100;
            _scrollTimer.Tick += ScrollTitle;
            this.MouseEnter += (sender, e) => { _isHovered = true; StartScrolling(); };
            this.MouseLeave += (sender, e) => { _isHovered = false; StopScrolling(); };
            UpdateInfoLabel();
        }
        private void StopScrolling()
        {
            if (_scrollTimer != null)
            {
                _scrollTimer.Stop();
            }
            lbl_Info.Text = _fullTitle;
        }
        private void StartScrolling()
        {
            if (_scrollTimer != null)
            {
                _scrollPosition = 0;
                _fullTitle = GetFullTitle();
                _scrollTimer.Start();
            }
        }

        private void ScrollTitle(object sender, EventArgs e)
        {
            if (_fullTitle.Length > 0)
            {
                _scrollPosition = (_scrollPosition + 1) % _fullTitle.Length;
                lbl_Info.Text = _fullTitle.Substring(_scrollPosition) + " " + _fullTitle.Substring(0, _scrollPosition);
            }
        }
        private string GetFullTitle()
        {
            return $"{SpnName} - PGN: {_pgn}, Address: {_address}, Max Value: {_maxValue}";
        }
        public void SetBorderColor(int colorIndex)
        {
            switch (colorIndex)
            {
                case 0:
                    _borderColor = Color.Black;
                    break;
                case 1:
                    _borderColor = Color.Red;
                    break;
                case 2:
                    _borderColor = Color.Blue;
                    break;
                case 3:
                    _borderColor = Color.Green;
                    break;
                case 4:
                    _borderColor = Color.Yellow;
                    break;
                case 5:
                    _borderColor = Color.Orange;
                    break;
                default:
                    _borderColor = Color.Black;
                    break;
            }
            Invalidate();
        }
        private void DrawBorder(object sender, PaintEventArgs e)
        {
            using (Pen borderPen = new Pen(_borderColor, BorderThickness))
            {
                Rectangle borderRectangle = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                e.Graphics.DrawRectangle(borderPen, borderRectangle);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawBorder(this, e);
        }
        protected virtual void InitializeSPN()
        {
            if (_priority < 0 || _priority > 7)
            {
                _priority = 3;
            }
            int defaultPGN = 0xFF69;
            int defaultAddress = 0x01;
            _spnLite = new spnLite(_myname, 0, _numberOfBytes, _firstByteIndex, _isLowByteFirst);
            _spnLite.SetPGNComponents(_priority, defaultPGN, defaultAddress);
        }
        public spnLite SPN
        {
            get => _spnLite;
            set
            {
                _spnLite = value;
                OnSPNChanged();
            }
        }
        private string _spnName = "DefaultSPNName";
        private void InitializeControl()
        {
            lbl_Info = new Label
            {
                AutoSize = true,
                Text = $"{_spnName}-PGN-ADDR-#BYTES-FIRSTINDEX-HILO",
                Location = new Point(4, 4),
                Font = new Font(this.Font.FontFamily, 6, FontStyle.Regular)
            };
            this.Controls.Add(lbl_Info);
            UpdateInfoLabel();
        }
        protected virtual void OnSPNChanged()
        {
            UpdateInfoLabel();
        }
        public void SetDefaults()
        {
            currentValue = 0;

            // Only set NumberOfBytes if it hasn't been set yet
            if (_numberOfBytes == defaultNumberOfBytes)
            {
                NumberOfBytes = defaultNumberOfBytes;
            }

            // Only set MaxValue if it hasn't been set yet or is greater than the maximum possible value
            int maxValueFromBytes = GetMaxValueFromBytes();
            if (_maxValue > maxValueFromBytes || _maxValue == 0)
            {
                MaxValue = maxValueFromBytes;
            }

            _firstByteIndex = defaultFirstByteIndex;
            _pgn = defaultPGN;
            _address = defaultAddress;
            _priority = default_priority;
            _isLowByteFirst = defaultIsLowByteFirst;

            UpdateValue(currentValue);
            UpdateInfoLabel();
            Invalidate();
        }
        [Category("Control Settings")]
        [Description("The maximum value allowed for the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public int MaxValue
        {
            get => _maxValue;
            set
            {
                if (value > 0 && value <= GetMaxValueFromBytes())
                {
                    _maxValue = value;
                    if (currentValue > _maxValue)
                    {
                        currentValue = _maxValue;
                    }
                    UpdateValue(currentValue);
                    OnMaxValueChanged();
                }
            }
        }

        protected virtual void OnMaxValueChanged()
        {
            // Base implementation can be empty or include base class logic
        }
        [Category("Control Settings")]
        [Description("The number of bytes (1 to 4)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public int NumberOfBytes
        {
            get => _numberOfBytes;
            set
            {
                if (value >= 1 && value <= 4)
                {
                    _numberOfBytes = value;
                    int maxValueFromBytes = GetMaxValueFromBytes();

                    // Only adjust _maxValue if it's greater than the new maximum
                    if (_maxValue > maxValueFromBytes)
                    {
                        _maxValue = maxValueFromBytes;
                    }

                    if (currentValue > _maxValue)
                    {
                        currentValue = _maxValue;
                    }
                    UpdateValue(currentValue);
                }
            }
        }

        protected int GetMaxValueFromBytes()
        {
            return (1 << (_numberOfBytes * 8)) - 1;
        }
        protected virtual void UpdateValue(int value)
        {

            /*
            int maxValue = MaxValue;
            if (value > maxValue)
            {
                value = maxValue;
            }
            else if (value < 0)
            {
                value = 0;
            }
            currentValue = value;
            ValueChanged?.Invoke(this, value);
            foreach (Control control in this.Controls)
            {
                if (control is Label valueLabel && valueLabel != lbl_Info)
                {
                    valueLabel.Text = value.ToString();
                }
                else if (control is TrackBar trackBar)
                {
                    trackBar.Maximum = MaxValue;
                    trackBar.Value = Math.Min(trackBar.Maximum, value);
                }
                else if (control is TextBox textBox)
                {
                    textBox.Text = value.ToString();
                }
            }
            UpdateInfoLabel();

            */


            int maxValue = MaxValue;
            if (value > maxValue)
            {
                value = maxValue;
            }
            else if (value < 0)
            {
                value = 0;
            }
            currentValue = value;
            ValueChanged?.Invoke(this, value);
            UpdateInfoLabel();
        }
        protected void UpdateInfoLabel()
        {
            if (lbl_Info == null)
            {
                return; // Exit early if lbl_Info is not initialized
            }

            string hilo = _isLowByteFirst ? "lh" : "hl";
            _fullTitle = $"{_spnName}-{_pgn}-{_address}-{_numberOfBytes}-{_firstByteIndex}-{hilo} m:{_maxValue}";
            lbl_Info.Text = _fullTitle;
            StopScrolling(); // Only stop scrolling if lbl_Info is initialized
        }

        public void SetValue(int value)
        {
            UpdateValue(value);
        }

        private bool IsHex(string input) => !string.IsNullOrEmpty(input) && input.All(Uri.IsHexDigit);

        [Category("Control Settings"), Description("The current value displayed by the control."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(true)]
        public int CurrentValue
        {
            get => currentValue;
            set
            {
                if (value >= 0 && value <= _maxValue)
                {
                    currentValue = value;
                    UpdateValue(value);
                }
            }
        }

        [Category("Control Settings"), Description("the PGN"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(true)]
        public string PGN
        {
            get => _pgn;
            set
            {
                if (IsHex(value))
                {
                    if (_pgn != value) { _pgn = value.ToUpper(); UpdateInfoLabel(); }
                }
                else { MessageBox.Show("Invalid PGN value. Please enter a valid hexadecimal value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        [Category("Control Settings"), Description("The address"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(true)]
        public string Address
        {
            get => _address;
            set
            {
                if (IsHex(value))
                {
                    if (_address != value) { _address = value.ToUpper(); UpdateInfoLabel(); }
                }
                else { MessageBox.Show("Invalid Address value. Please enter a valid hexadecimal value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

       

        [Category("Control Settings"), Description("The first byte index"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(true)]
        public int FirstByteIndex
        {
            get => _firstByteIndex;
            set
            {
                if (value >= 0 && value <= 7 && _firstByteIndex != value)
                {
                    _firstByteIndex = value; UpdateInfoLabel();
                }
            }
        }

        [Category("Control Settings"), Description("Hi lo"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(true)]
        public bool IsLowByteFirst
        {
            get => _isLowByteFirst;
            set { if (_isLowByteFirst != value) { _isLowByteFirst = value; UpdateInfoLabel(); } }
        }

        [Category("Control Settings"), Description("The priority of the PGN"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(true)]
        public int Priority
        {
            get => _priority;
            set
            {
                if (value < 0 || value > 7) { MessageBox.Show("Priority must be between 0 and 255.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                else { _priority = value; UpdateInfoLabel(); }
            }
        }

        [Category("Control Settings"), Description("The SPN Name"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(true)]
        public string SpnName
        {
            get => _myname;
            set { _myname = string.IsNullOrEmpty(value) ? "DefaultName" : value; UpdateInfoLabel(); }
        }

        public void ResetToDefaults()
        {
            SetDefaults();
        }
    }
}