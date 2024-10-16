namespace VC_WATERCRAFT.spnuc
{
    partial class VCinc_spn
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbl_scrollingTitle = new System.Windows.Forms.Label();
            this.textBox_value = new System.Windows.Forms.TextBox();
            this.panel_visualizer = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lbl_scrollingTitle
            // 
            this.lbl_scrollingTitle.AutoSize = true;
            this.lbl_scrollingTitle.Font = new System.Drawing.Font("Arial Narrow", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_scrollingTitle.Location = new System.Drawing.Point(0, 231);
            this.lbl_scrollingTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_scrollingTitle.Name = "lbl_scrollingTitle";
            this.lbl_scrollingTitle.Size = new System.Drawing.Size(52, 23);
            this.lbl_scrollingTitle.TabIndex = 3;
            this.lbl_scrollingTitle.Text = "label1";
            // 
            // textBox_value
            // 
            this.textBox_value.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_value.Location = new System.Drawing.Point(0, 202);
            this.textBox_value.Margin = new System.Windows.Forms.Padding(0);
            this.textBox_value.Name = "textBox_value";
            this.textBox_value.Size = new System.Drawing.Size(120, 22);
            this.textBox_value.TabIndex = 4;
            this.textBox_value.Text = "0";
            this.textBox_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panel_visualizer
            // 
            this.panel_visualizer.Location = new System.Drawing.Point(0, 0);
            this.panel_visualizer.Margin = new System.Windows.Forms.Padding(0);
            this.panel_visualizer.Name = "panel_visualizer";
            this.panel_visualizer.Size = new System.Drawing.Size(200, 200);
            this.panel_visualizer.TabIndex = 5;
            // 
            // VCinc_uc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbl_scrollingTitle);
            this.Controls.Add(this.textBox_value);
            this.Controls.Add(this.panel_visualizer);
            this.Font = new System.Drawing.Font("Arial Narrow", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "VCinc_uc";
            this.Size = new System.Drawing.Size(200, 260);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_scrollingTitle;
        private System.Windows.Forms.TextBox textBox_value;
        private System.Windows.Forms.Panel panel_visualizer;
    }
}
