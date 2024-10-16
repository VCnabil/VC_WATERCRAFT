using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VC_WATERCRAFT.Forms.Templates
{
    public partial class formtest : Form
    {
        Timer Timer = new Timer();
        public formtest()
        {
           InitializeComponent();
            Timer.Interval = 100;
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Start();
      
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            vCinc_spn1.Value = vCinc_spn2.Value;
        }
    }
}
