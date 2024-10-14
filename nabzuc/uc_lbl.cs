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
    public partial class uc_lbl : uc_BaseControl
    {
        private TextBox valueTextBox;
        private bool _isUpdating = false;

        public uc_lbl()
        {
            InitializeComponent();
            InitializeCustomComponents();
            SetDefaults();
            //   UpdateInfoLabel();  // Update info label based on SPN
        }

        // Optionally override to provide custom SPN initialization

        private void InitializeCustomComponents()
        {
            // Initialize TextBox for value
            valueTextBox = new TextBox
            {
                Location = new Point(10, 50),
                Size = new Size(Width - 20, 20),
                Text = currentValue.ToString()
            };
            valueTextBox.TextChanged += ValueTextBox_TextChanged;

            // Set control properties
            this.MinimumSize = new Size(100, 100);
            this.BorderStyle = BorderStyle.FixedSingle;

            // Add the TextBox to the controls collection
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
                valueTextBox.Text = currentValue.ToString(); // Revert to the valid current value
                _isUpdating = false;
            }
        }

        protected override void UpdateValue(int value)
        {
            base.UpdateValue(value);

            if (valueTextBox != null)
            {
                if (!_isUpdating)
                {
                    _isUpdating = true;
                    valueTextBox.Text = currentValue.ToString();
                    _isUpdating = false;
                }
            }

            // Update SPN value to reflect the current value
            if (_spnLite != null)
            {
                _spnLite.Value = currentValue;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Adjust the size of the valueTextBox on resizing the control
            if (valueTextBox != null)
            {
                valueTextBox.Size = new Size(this.Width - 20, valueTextBox.Height);
            }
        }
    }
}
