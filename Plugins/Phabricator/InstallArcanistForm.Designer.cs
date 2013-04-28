namespace Phabricator
{
    partial class InstallArcanistForm
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
            this._installLabel = new System.Windows.Forms.Label();
            this._installButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _installLabel
            // 
            this._installLabel.AutoSize = true;
            this._installLabel.Location = new System.Drawing.Point(12, 16);
            this._installLabel.Name = "_installLabel";
            this._installLabel.Size = new System.Drawing.Size(180, 13);
            this._installLabel.TabIndex = 0;
            this._installLabel.Text = "This tool will help you install Arcanist.";
            // 
            // _installButton
            // 
            this._installButton.Location = new System.Drawing.Point(430, 10);
            this._installButton.Name = "_installButton";
            this._installButton.Size = new System.Drawing.Size(99, 25);
            this._installButton.TabIndex = 1;
            this._installButton.Text = "Install";
            this._installButton.UseVisualStyleBackColor = true;
            this._installButton.Click += new System.EventHandler(this._installButton_Click);
            // 
            // InstallArcanistForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 46);
            this.Controls.Add(this._installButton);
            this.Controls.Add(this._installLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallArcanistForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Install Arcanist";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstallArcanistForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _installLabel;
        private System.Windows.Forms.Button _installButton;
    }
}