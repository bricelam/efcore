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
            this.SuspendLayout();
            // 
            // infoPanel
            // 
            this.infoPanel.Location = new System.Drawing.Point(0, 304);
            this.infoPanel.Size = new System.Drawing.Size(500, 0);
            // 
            // _tablesTreeView
            // 
            this._tablesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tablesTreeView.CheckBoxes = true;
            this._tablesTreeView.Location = new System.Drawing.Point(0, 0);
            this._tablesTreeView.Name = "_tablesTreeView";
            this._tablesTreeView.Size = new System.Drawing.Size(500, 304);
            this._tablesTreeView.TabIndex = 3;
            // 
            // DbContextWizardTablesPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this._tablesTreeView);
            this.Headline = "Choose Your Database Objects";
            this.Name = "DbContextWizardTablesPage";
            this.Size = new System.Drawing.Size(503, 307);
            this.Controls.SetChildIndex(this._tablesTreeView, 0);
            this.Controls.SetChildIndex(this.infoPanel, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView _tablesTreeView;
    }
}
