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
    public partial class uc_8bits : uc_BaseControl
    {

        #region Exposed Props


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
                if (_bitCheckBoxes[0] != null)
                {
                    _bitCheckBoxes[0].Text = $"b0: {value.Substring(0, Math.Min(6, value.Length))}";
                }
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
                if (_bitCheckBoxes[1] != null)
                {
                    _bitCheckBoxes[1].Text = $"b1: {value.Substring(0, Math.Min(6, value.Length))}";
                }
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
                if (_bitCheckBoxes[2] != null)
                {
                    _bitCheckBoxes[2].Text = $"b2: {value.Substring(0, Math.Min(6, value.Length))}";
                }
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
                if (_bitCheckBoxes[3] != null)
                {
                    _bitCheckBoxes[3].Text = $"b3: {value.Substring(0, Math.Min(6, value.Length))}";
                }
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
                if (_bitCheckBoxes[4] != null)
                {
                    _bitCheckBoxes[4].Text = $"b4: {value.Substring(0, Math.Min(6, value.Length))}";
                }
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
                if (_bitCheckBoxes[5] != null)
                {
                    _bitCheckBoxes[5].Text = $"b5: {value.Substring(0, Math.Min(6, value.Length))}";
                }
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
                if (_bitCheckBoxes[6] != null)
                {
                    _bitCheckBoxes[6].Text = $"b6: {value.Substring(0, Math.Min(6, value.Length))}";
                }
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
                if (_bitCheckBoxes[7] != null)
                {
                    _bitCheckBoxes[7].Text = $"b7: {value.Substring(0, Math.Min(6, value.Length))}";
                }
            }
        }
        #endregion


        private const int BorderThickness = 2;
        private const int minimumWidth = 200;
        private const int minimumHeight = 200;
        private Label _valueLabel;
        private CheckBox[] _bitCheckBoxes = new CheckBox[8];
        private string[] _bitTitles = new string[8];
        private ToolTip _toolTip;
        private bool _isInitialized = false;
        public event EventHandler<int> RezValueChanged;

        public uc_8bits()
        {
            InitializeComponent();
            _numberOfBytes = 1;
            this.Priority = 3;
            InitializeControl();
            SetDefaults();
            _isInitialized = true;  

          
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

        private void InitializeControl()
        {
            _valueLabel = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font(this.Font.FontFamily, 6, FontStyle.Bold),

                Text = $"val: {currentValue}"
            };
            this.Controls.Add(_valueLabel);

            _toolTip = new ToolTip();

            for (int i = 0; i < 8; i++)
            {
                _bitCheckBoxes[i] = new CheckBox
                {
                    Font = new Font(this.Font.FontFamily, 6),
                    Text = $"b.{i}",
                    Tag = i,
                    AutoSize = true
                };
                _bitCheckBoxes[i].CheckedChanged += BitCheckBox_CheckedChanged;
                _bitCheckBoxes[i].MouseHover += BitCheckBox_MouseHover;
                this.Controls.Add(_bitCheckBoxes[i]);
                _bitTitles[i] = $"b.{i}";
            }
            ArrangeControlsColumn();
        }

        private void ArrangeControlsColumn()
        {
            if (_valueLabel == null || _bitCheckBoxes.Any(cb => cb == null))
            {
                return; // Skip if the controls are not initialized
            }

            int xPosition = BorderThickness;
            int yPosition = BorderThickness;

            _valueLabel.Location = new Point(xPosition, yPosition + 19);
            _valueLabel.Width = this.Width - 2 * BorderThickness;
            SizeF titlePreferredSize = _valueLabel.GetPreferredSize(new Size(_valueLabel.Width, 0));
            _valueLabel.Height = (int)Math.Ceiling(titlePreferredSize.Height);
            yPosition += _valueLabel.Height + 39;

            for (int i = 0; i < 8; i++)
            {
                if (_bitCheckBoxes[i] == null)
                {
                    continue; // Ensure _bitCheckBoxes[i] is not null
                }

                if (i == 4)
                {
                    xPosition += _bitCheckBoxes[i - 4].Width + 30;
                    yPosition = _valueLabel.Height + 20;
                }

                _bitCheckBoxes[i].Location = new Point(xPosition, yPosition);
                SizeF checkBoxPreferredSize = _bitCheckBoxes[i].GetPreferredSize(new Size(0, 0));
                _bitCheckBoxes[i].Width = this.Width / 2 - 2 * BorderThickness - 10;
                _bitCheckBoxes[i].Height = (int)Math.Ceiling(checkBoxPreferredSize.Height);
                yPosition += _bitCheckBoxes[i].Height + 5;
            }

            this.Height = Math.Max(yPosition + BorderThickness + 10, minimumHeight);
            this.Width = Math.Max(_valueLabel.Width + 2 * BorderThickness, minimumWidth);
        }
        private void BitCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox chk)
            {
                int bitIndex = (int)chk.Tag;
                if (chk.Checked)
                {
                    currentValue |= (1 << bitIndex);
                }
                else
                {
                    currentValue &= ~(1 << bitIndex);
                }
                UpdateValue(currentValue);
            }
        }

        private void BitCheckBox_MouseHover(object sender, EventArgs e)
        {
            if (sender is CheckBox chk)
            {
                _toolTip.Show(_bitTitles[(int)chk.Tag], chk, chk.Width / 2, chk.Height / 2, 1000); // Limit display time to 1 second
            }
        }

        protected override void UpdateValue(int value)
        {
            if (!_isInitialized)
                return;

            base.UpdateValue(value);
            _valueLabel.Text = $"{currentValue}";
            // Update SPN value to reflect the current value
            if (_spnLite != null)
            {
                _spnLite.Value = currentValue;
            }

            for (int i = 0; i < 8; i++)
            {
                if (_bitCheckBoxes[i] != null)
                {
                    bool bitIsSet = (currentValue & (1 << i)) != 0;
                    if (_bitCheckBoxes[i].Checked != bitIsSet)
                    {
                        _bitCheckBoxes[i].Checked = bitIsSet;
                    }
                }
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.Width < minimumWidth || this.Height < minimumHeight)
            {
                return;
            }
            ArrangeControlsColumn();
        }


    }
}
