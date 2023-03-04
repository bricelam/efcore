namespace Microsoft.EntityFrameworkCore.VisualStudio
{
    partial class NewDbContextForm
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
            this._chooseButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._connectionTextBox = new System.Windows.Forms.TextBox();
            this._providerComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // _chooseButton
            // 
            this._chooseButton.Location = new System.Drawing.Point(393, 10);
            this._chooseButton.Name = "_chooseButton";
            this._chooseButton.Size = new System.Drawing.Size(75, 23);
            this._chooseButton.TabIndex = 0;
            this._chooseButton.Text = "Choose...";
            this._chooseButton.UseVisualStyleBackColor = true;
            this._chooseButton.Click += new System.EventHandler(this._chooseButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Connection string";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Provider";
            // 
            // _connectionTextBox
            // 
            this._connectionTextBox.Location = new System.Drawing.Point(107, 12);
            this._connectionTextBox.Name = "_connectionTextBox";
            this._connectionTextBox.Size = new System.Drawing.Size(280, 20);
            this._connectionTextBox.TabIndex = 3;
            // 
            // _providerComboBox
            // 
            this._providerComboBox.FormattingEnabled = true;
            this._providerComboBox.Location = new System.Drawing.Point(107, 38);
            this._providerComboBox.Name = "_providerComboBox";
            this._providerComboBox.Size = new System.Drawing.Size(280, 21);
            this._providerComboBox.TabIndex = 4;
            // 
            // NewDbContextForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 485);
            this.Controls.Add(this._providerComboBox);
            this.Controls.Add(this._connectionTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._chooseButton);
            this.Name = "NewDbContextForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New DbContext";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _chooseButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _connectionTextBox;
        private System.Windows.Forms.ComboBox _providerComboBox;
    }
}
