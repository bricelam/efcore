using System.Windows.Forms;

namespace Microsoft.EntityFrameworkCore.VisualStudio
{
    partial class DbContextWizardConnectionPage
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
            this._providerComboBox = new System.Windows.Forms.ComboBox();
            this._connectionTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._chooseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // infoPanel
            // 
            this.infoPanel.Location = new System.Drawing.Point(0, 304);
            this.infoPanel.Size = new System.Drawing.Size(500, 0);
            // 
            // _providerComboBox
            // 
            this._providerComboBox.FormattingEnabled = true;
            this._providerComboBox.Location = new System.Drawing.Point(6, 60);
            this._providerComboBox.Name = "_providerComboBox";
            this._providerComboBox.Size = new System.Drawing.Size(220, 21);
            this._providerComboBox.TabIndex = 9;
            this._providerComboBox.Text = "Microsoft.EntityFrameworkCore.SqlServer";
            // 
            // _connectionTextBox
            // 
            this._connectionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._connectionTextBox.Location = new System.Drawing.Point(6, 16);
            this._connectionTextBox.Name = "_connectionTextBox";
            this._connectionTextBox.Size = new System.Drawing.Size(410, 20);
            this._connectionTextBox.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Provider";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Connection string";
            // 
            // _chooseButton
            // 
            this._chooseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._chooseButton.Location = new System.Drawing.Point(422, 13);
            this._chooseButton.Name = "_chooseButton";
            this._chooseButton.Size = new System.Drawing.Size(75, 23);
            this._chooseButton.TabIndex = 5;
            this._chooseButton.Text = "Choose...";
            this._chooseButton.UseVisualStyleBackColor = true;
            this._chooseButton.Click += new System.EventHandler(this._chooseButton_Click);
            // 
            // DbContextWizardConnectionPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this._chooseButton);
            this.Controls.Add(this._providerComboBox);
            this.Controls.Add(this._connectionTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Headline = "Choose Your Data Connection";
            this.Name = "DbContextWizardConnectionPage";
            this.Size = new System.Drawing.Size(500, 304);
            this.Controls.SetChildIndex(this.infoPanel, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this._connectionTextBox, 0);
            this.Controls.SetChildIndex(this._providerComboBox, 0);
            this.Controls.SetChildIndex(this._chooseButton, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _chooseButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox _providerComboBox;
        private System.Windows.Forms.TextBox _connectionTextBox;
    }
}
