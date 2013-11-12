namespace Project_2._0
{
    partial class Form1
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
			this.gameControl1 = new Project_2._0.GameControl();
			this.SuspendLayout();
			// 
			// gameControl1
			// 
			this.gameControl1.BackColor = System.Drawing.Color.DarkGreen;
			this.gameControl1.Data = null;
			this.gameControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gameControl1.Location = new System.Drawing.Point(0, 0);
			this.gameControl1.Name = "gameControl1";
			this.gameControl1.Size = new System.Drawing.Size(484, 462);
			this.gameControl1.TabIndex = 0;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(484, 462);
			this.Controls.Add(this.gameControl1);
			this.Name = "Form1";
			this.Text = "Project 2.0";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

        }

        #endregion

        private GameControl gameControl1;
    }
}

