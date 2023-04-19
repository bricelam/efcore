using System.Windows.Forms;

namespace Microsoft.EntityFrameworkCore.VisualStudio
{
    partial class DbContextWizardStartPage
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
            this._fromDatabaseRadioButton = new System.Windows.Forms.RadioButton();
            this._emptyRadioButton = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // infoPanel
            // 
            this.infoPanel.Location = new System.Drawing.Point(0, 304);
            this.infoPanel.Size = new System.Drawing.Size(500, 0);
            // 
            // _fromDatabaseRadioButton
            // 
            this._fromDatabaseRadioButton.AutoSize = true;
            this._fromDatabaseRadioButton.Checked = true;
            this._fromDatabaseRadioButton.Location = new System.Drawing.Point(3, 3);
            this._fromDatabaseRadioButton.Name = "_fromDatabaseRadioButton";
            this._fromDatabaseRadioButton.Size = new System.Drawing.Size(142, 17);
            this._fromDatabaseRadioButton.TabIndex = 3;
            this._fromDatabaseRadioButton.TabStop = true;
            this._fromDatabaseRadioButton.Text = "Code First from database";
            this._fromDatabaseRadioButton.UseVisualStyleBackColor = true;
            // 
            // _emptyRadioButton
            // 
            this._emptyRadioButton.AutoSize = true;
            this._emptyRadioButton.Location = new System.Drawing.Point(3, 26);
            this._emptyRadioButton.Name = "_emptyRadioButton";
            this._emptyRadioButton.Size = new System.Drawing.Size(135, 17);
            this._emptyRadioButton.TabIndex = 4;
            this._emptyRadioButton.Text = "Empty Code First model";
            this._emptyRadioButton.UseVisualStyleBackColor = true;
            // 
            // DbContextWizardStartPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this._emptyRadioButton);
            this.Controls.Add(this._fromDatabaseRadioButton);
            this.Headline = "Choose Model Contents";
            this.Name = "DbContextWizardStartPage";
            this.Size = new System.Drawing.Size(500, 304);
            this.Controls.SetChildIndex(this.infoPanel, 0);
            this.Controls.SetChildIndex(this._fromDatabaseRadioButton, 0);
            this.Controls.SetChildIndex(this._emptyRadioButton, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RadioButton _fromDatabaseRadioButton;
        private RadioButton _emptyRadioButton;
    }
}
