using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VC_WATERCRAFT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
           // this.AutoScaleMode = AutoScaleMode.Dpi;
            //this.AutoScaleMode = AutoScaleMode.None;
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // this.Scale(new SizeF(1.0f, 1.0f));  // Reset any DPI scaling
          //  this.Scale(new SizeF(0.5f, 0.5f));  // Reset any DPI scaling
        }
    }
}
