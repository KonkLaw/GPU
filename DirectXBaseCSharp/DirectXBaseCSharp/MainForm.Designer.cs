namespace DirectXBaseCSharp
{
    partial class MainForm
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
            this.panel = new System.Windows.Forms.Panel();
            this.buttonRunTest = new System.Windows.Forms.Button();
            this.renderControl = new DirectXBaseCSharp.RenderControl();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Controls.Add(this.buttonRunTest);
            this.panel.Location = new System.Drawing.Point(12, 618);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(800, 43);
            this.panel.TabIndex = 1;
            // 
            // buttonRunTest
            // 
            this.buttonRunTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRunTest.Location = new System.Drawing.Point(3, 3);
            this.buttonRunTest.Name = "buttonRunTest";
            this.buttonRunTest.Size = new System.Drawing.Size(125, 37);
            this.buttonRunTest.TabIndex = 0;
            this.buttonRunTest.Text = "Run test";
            this.buttonRunTest.UseVisualStyleBackColor = true;
            this.buttonRunTest.Click += new System.EventHandler(this.buttonRunTest_Click);
            // 
            // renderControl
            // 
            this.renderControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.renderControl.BackColor = System.Drawing.Color.White;
            this.renderControl.Location = new System.Drawing.Point(12, 12);
            this.renderControl.Name = "renderControl";
            this.renderControl.Size = new System.Drawing.Size(800, 600);
            this.renderControl.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 673);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.renderControl);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private RenderControl renderControl;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Button buttonRunTest;
    }
}

