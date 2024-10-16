namespace VC_WATERCRAFT.Forms.Templates
{
    partial class formtest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.vCinc_spn1 = new VC_WATERCRAFT.spnuc.VCinc_spn();
            this.vCinc_spn2 = new VC_WATERCRAFT.spnuc.VCinc_spn();
            this.SuspendLayout();
            // 
            // vCinc_spn1
            // 
            this.vCinc_spn1.Address = "00";
            this.vCinc_spn1.Bit0Title = null;
            this.vCinc_spn1.Bit1Title = null;
            this.vCinc_spn1.Bit2Title = null;
            this.vCinc_spn1.Bit3Title = null;
            this.vCinc_spn1.Bit4Title = null;
            this.vCinc_spn1.Bit5Title = null;
            this.vCinc_spn1.Bit6Title = null;
            this.vCinc_spn1.Bit7Title = null;
            this.vCinc_spn1.ControlMode = VC_WATERCRAFT.spnuc.VCinc_spn.ControlModes.Gauge;
            this.vCinc_spn1.Font = new System.Drawing.Font("Arial Narrow", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vCinc_spn1.Location = new System.Drawing.Point(318, 135);
            this.vCinc_spn1.Margin = new System.Windows.Forms.Padding(0);
            this.vCinc_spn1.Name = "vCinc_spn1";
            this.vCinc_spn1.PGN = "0000";
            this.vCinc_spn1.Size = new System.Drawing.Size(200, 260);
            this.vCinc_spn1.TabIndex = 0;
            // 
            // vCinc_spn2
            // 
            this.vCinc_spn2.Address = "00";
            this.vCinc_spn2.Bit0Title = null;
            this.vCinc_spn2.Bit1Title = null;
            this.vCinc_spn2.Bit2Title = null;
            this.vCinc_spn2.Bit3Title = null;
            this.vCinc_spn2.Bit4Title = null;
            this.vCinc_spn2.Bit5Title = null;
            this.vCinc_spn2.Bit6Title = null;
            this.vCinc_spn2.Bit7Title = null;
            this.vCinc_spn2.Font = new System.Drawing.Font("Arial Narrow", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vCinc_spn2.Location = new System.Drawing.Point(659, 154);
            this.vCinc_spn2.Margin = new System.Windows.Forms.Padding(0);
            this.vCinc_spn2.Name = "vCinc_spn2";
            this.vCinc_spn2.PGN = "0000";
            this.vCinc_spn2.Size = new System.Drawing.Size(200, 260);
            this.vCinc_spn2.TabIndex = 1;
            // 
            // formtest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1600, 1433);
            this.Controls.Add(this.vCinc_spn2);
            this.Controls.Add(this.vCinc_spn1);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "formtest";
            this.Text = "formtest";
            this.ResumeLayout(false);

        }


        #endregion

        private spnuc.VCinc_spn vCinc_spn1;
        private spnuc.VCinc_spn vCinc_spn2;
    }
}





