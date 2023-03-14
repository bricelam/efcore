namespace Microsoft.EntityFrameworkCore.VisualStudio
{
    partial class DbContextWizardTablesPage
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
            this._tablesTreeView = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // infoPanel
            // 
            this.infoPanel.Location = new System.Drawing.Point(0, 307);
            this.infoPanel.Size = new System.Drawing.Size(503, 0);
            // 
            // _tablesTreeView
            // 
            this._tablesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tablesTreeView.CheckBoxes = true;
            this._tablesTreeView.Location = new System.Drawing.Point(0, 16);
            this._tablesTreeView.Name = "_tablesTreeView";
            this._tablesTreeView.Size = new System.Drawing.Size(500, 288);
            this._tablesTreeView.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Which database objects do you want to include in your model?";
            // 
            // DbContextWizardTablesPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._tablesTreeView);
            this.Headline = "Choose Your Database Objects";
            this.Name = "DbContextWizardTablesPage";
            this.Size = new System.Drawing.Size(503, 315);
            this.Controls.SetChildIndex(this._tablesTreeView, 0);
            this.Controls.SetChildIndex(this.infoPanel, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView _tablesTreeView;
        private System.Windows.Forms.Label label1;
    }
}
