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
        protected Label lbl_Info;
        protected int currentValue;  // Stores the current value of the control
        protected int _firstByteIndex = 0;  // Default first byte index
        protected int _numberOfBytes = 1;  // Default to 1 byte (8 bits)
        public event EventHandler<int> ValueChanged;  // Event to notify changes in value
        private string _pgn;
        private string _address;
        private bool _isLowByteFirst;
        private readonly string defaultPGN = "FFFF";
        private readonly string defaultAddress = "00";
        private readonly bool defaultIsLowByteFirst = false;
        private readonly int defaultNumberOfBytes = 1;
        private readonly int defaultFirstByteIndex = 0;
        //init priority default to 0x18
        private int default_priority = 0x18;

        protected spnLite _spnLite;  // Protected spnLite object, accessible by derived classes

        private Color _borderColor = Color.Black;  // Default color is black
        private const int BorderThickness = 2;     // Thickness of the border

        // Constructor
        public uc_BaseControl()
        {
            InitializeComponent();
            InitializeControl();
            InitializeSPN();  // Initialize the spnLite object here
            SetDefaults();
            this.Paint += DrawBorder;  // Subscribe to the Paint event to draw the border
        }

        // Method to set the border color based on the index
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
                    _borderColor = Color.Black;  // Default to black if an invalid index is provided
                    break;
            }
            Invalidate();  // Force the control to redraw with the new border color
        }

        // Method to draw the border
        private void DrawBorder(object sender, PaintEventArgs e)
        {
            using (Pen borderPen = new Pen(_borderColor, BorderThickness))
            {
                Rectangle borderRectangle = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                e.Graphics.DrawRectangle(borderPen, borderRectangle);
            }
        }

        // Override the OnPaint method to make sure the border is drawn correctly
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawBorder(this, e);
        }

        // Initialize the spnLite object
        protected virtual void InitializeSPN()
        {
            // Validate and set default priority if necessary
            if (_priority < 0 || _priority > 7)
            {
                // Set to a default valid value to prevent crashes
                _priority = 3;  // Safe default priority within the range 0-7
            }

            // Set default PGN and address to valid values
            int defaultPGN = 0xFF69;  // A valid PGN within the range (0 - 0xFFFFF)
            int defaultAddress = 0x01;  // A valid address within the range (0 - 0xFF)

            // Initialize the SPN object with validated/default values
            _spnLite = new spnLite("DefaultSPN", 0, _numberOfBytes, _firstByteIndex, _isLowByteFirst);

            // Set PGN components with validated/default values
            _spnLite.SetPGNComponents(_priority, defaultPGN, defaultAddress);
        }


        // Property to get or set the spnLite object
        public spnLite SPN
        {
            get => _spnLite;
            set
            {
                _spnLite = value;
                OnSPNChanged();  // Optionally trigger any update when SPN changes
            }
        }

        // Initialize control properties and add the info label
        private void InitializeControl()
        {
            // Initialize lbl_Info or any other control before using them
            lbl_Info = new Label
            {
                AutoSize = true,
                Text = "PGN-ADDR-#BYTES-FIRSTINDEX-HILO",
                Location = new Point(4, 4),
                Font = new Font(this.Font.FontFamily, 6, FontStyle.Regular)
            };
            this.Controls.Add(lbl_Info);

            UpdateInfoLabel();  // Safely call this after lbl_Info is initialized
        }



        // Event or method to handle SPN updates (optional)
        protected virtual void OnSPNChanged()
        {
            // Optionally update UI or controls when SPN changes
            UpdateInfoLabel();  // Call a method to update any relevant UI info
        }
        // Set the default values for the control properties
        public void SetDefaults()
        {
            currentValue = 0;
            _numberOfBytes = 1;
            _firstByteIndex = 0;
            _pgn = "FFFF";
            _address = "00";
            _isLowByteFirst = false;
            UpdateValue(currentValue);
            UpdateInfoLabel();
            Invalidate();  // Request control to redraw itself
        }


        // Private field to store custom MaxValue
        private int _maxValue = 255;  // Default max value for 1 byte

        // Public property for MaxValue, with validation against the byte size
        [Category("Control Settings")]
        [Description("The maximum value allowed for the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public int MaxValue
        {
            get => _maxValue;
            set
            {
                // Validate the max value based on the number of bytes
                int theoreticalMax = GetMaxValueFromBytes();
                if (value > theoreticalMax)
                {
                    MessageBox.Show($"Max value cannot exceed {theoreticalMax} for the current byte size.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    _maxValue = value;
                    UpdateValue(currentValue);  // Re-validate the current value based on the new max
                    UpdateInfoLabel();
                }
            }
        }

        // Public property to get or set the number of bytes, validated to be between 1 and 4
        [Category("Control Settings")]
        [Description("The number of bytes (1 to 4)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public int NumberOfBytes
        {
            get => _numberOfBytes;
            set
            {
                if (value >= 1 && value <= 4)
                {
                    if (_numberOfBytes != value)
                    {
                        _numberOfBytes = value;

                        // Adjust the MaxValue if necessary to ensure it fits within the new byte size
                        int theoreticalMax = GetMaxValueFromBytes();
                        if (_maxValue > theoreticalMax)
                        {
                            _maxValue = theoreticalMax;  // Automatically clamp MaxValue
                        }

                        UpdateValue(currentValue);  // Recalculate the current value with the new byte limit
                        UpdateInfoLabel();
                    }
                }
                else
                {
                    // MessageBox.Show("Invalid Number of Bytes. It must be between 1 and 4.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Calculate the theoretical maximum value for the specified number of bytes
        protected int GetMaxValueFromBytes()
        {
            return (1 << (_numberOfBytes * 8)) - 1;  // Max value is (2^(NumberOfBytes * 8)) - 1
        }

        // Update the control's value, clamping it to the custom MaxValue
        protected virtual void UpdateValue(int value)
        {
            int maxValue = MaxValue;  // Use the custom MaxValue

            if (value > maxValue)
            {
                value = maxValue;  // Clamp to MaxValue
            }
            else if (value < 0)
            {
                value = 0;  // Clamp to minimum value
            }

            // Set the current value and update UI elements
            currentValue = value;
            ValueChanged?.Invoke(this, value);  // Trigger value changed event

            // Update relevant child controls
            foreach (Control control in this.Controls)
            {
                if (control is Label valueLabel && valueLabel != lbl_Info)
                {
                    valueLabel.Text = value.ToString();
                }
                else if (control is TrackBar trackBar)
                {
                    // Set the trackbar's max value to the custom MaxValue
                    trackBar.Maximum = MaxValue;
                    trackBar.Value = Math.Min(trackBar.Maximum, value);  // Ensure the trackbar value stays within the limits
                }
                else if (control is TextBox textBox)
                {
                    textBox.Text = value.ToString();
                }
            }

            // Update information label to reflect current state
            UpdateInfoLabel();
        }

        // Update info label
        protected void UpdateInfoLabel()
        {
            string hilo = _isLowByteFirst ? "lh" : "hl";
            lbl_Info.Text = $"{_pgn}-{_address}-{_numberOfBytes}-{_firstByteIndex}-{hilo} m:{_maxValue}";
        }
        // Calculate the maximum value that can be represented by the specified number of bytes

        // Public method to set the value from external sources
        public void SetValue(int value)
        {
            UpdateValue(value);
        }


        // Public property to get or set the current value
        [Category("Control Settings")]
        [Description("The current value displayed by the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public int CurrentValue
        {
            get => currentValue;
            set => SetValue(value);
        }
        // Public property to get or set the PGN, with hex validation
        [Category("Control Settings")]
        [Description("the PGN")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public string PGN
        {
            get => _pgn;
            set
            {
                if (IsHex(value))
                {
                    if (_pgn != value)
                    {
                        _pgn = value.ToUpper(); // Convert to uppercase to maintain hex format
                        UpdateInfoLabel();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid PGN value. Please enter a valid hexadecimal value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Public property to get or set the Address, with hex validation
        [Category("Control Settings")]
        [Description("The address")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public string Address
        {
            get => _address;
            set
            {
                if (IsHex(value))
                {
                    if (_address != value)
                    {
                        _address = value.ToUpper(); // Convert to uppercase to maintain hex format
                        UpdateInfoLabel();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Address value. Please enter a valid hexadecimal value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Helper method to validate if a string is a valid hexadecimal value
        private bool IsHex(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            // Check if all characters in the string are valid hex digits
            foreach (char c in input)
            {
                if (!Uri.IsHexDigit(c))
                {
                    return false; // Return false if a non-hex digit is found
                }
            }
            return true;
        }

        // Public property to get or set the FirstByteIndex, with validation
        [Category("Control Settings")]
        [Description("The first byte index")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public int FirstByteIndex
        {
            get => _firstByteIndex;
            set
            {
                if (value < 0 || value > 7) // Ensure index is between 0 and 7
                {
                    //MessageBox.Show("First Byte Index must be between 0 and 3.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_firstByteIndex != value)
                {
                    _firstByteIndex = value;
                    UpdateInfoLabel();
                }
            }
        }
        // Public property to get or set the Hilo, 
        [Category("Control Settings")]
        [Description("Hi lo")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public bool IsLowByteFirst
        {
            get => _isLowByteFirst;
            set
            {
                if (_isLowByteFirst != value)
                {
                    _isLowByteFirst = value;
                    UpdateInfoLabel();
                }
            }
        }

        //public property for PRiority in hex format
        //   private string _priority = "00";
        private int _priority = 0x18;  // Default priority set to 3, which is within the valid range (0-7).

        [Category("Control Settings")]
        [Description("The priority of the PGN")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public int Priority
        {
            get => _priority;
            set
            {
                if (value < 0 || value > 255)
                {
                    MessageBox.Show("Priority must be between 0 and 255.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    _priority = value;
                    UpdateInfoLabel();
                }
            }
        }




        // Event to reset to defaults
        public void ResetToDefaults()
        {
            SetDefaults();
        }
    }
}
