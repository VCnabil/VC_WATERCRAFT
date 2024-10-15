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
    public partial class uc_lbl : uc_BaseControl
    {
        private TextBox valueTextBox;
        private bool _isUpdating = false;

        public uc_lbl()
        {
            this.Priority = 3;
            InitializeComponent();
            InitializeCustomComponents();
            SetDefaults();

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

            valueTextBox = new TextBox
            {
                Location = new Point(10, 50),
                Size = new Size(Width - 20, 20),
                Text = currentValue.ToString()
            };
            valueTextBox.TextChanged += ValueTextBox_TextChanged;

            this.MinimumSize = new Size(100, 100);
            this.BorderStyle = BorderStyle.FixedSingle;

            this.Controls.Add(valueTextBox);
        }
        private void ValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdating) return;

            if (int.TryParse(valueTextBox.Text, out int value))
            {
                SetValue(value);
            }
            else
            {
                _isUpdating = true;
                valueTextBox.Text = currentValue.ToString();
                _isUpdating = false;
            }
        }
        protected override void UpdateValue(int value)
        {
            base.UpdateValue(value);

            if (!_isUpdating)
            {
                _isUpdating = true;
                valueTextBox.Text = currentValue.ToString();
                _isUpdating = false;
            }

            if (_spnLite != null)
            {
                _spnLite.Value = currentValue;
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (valueTextBox != null)
            {
                valueTextBox.Size = new Size(this.Width - 20, valueTextBox.Height);
            }
        }
    }
}