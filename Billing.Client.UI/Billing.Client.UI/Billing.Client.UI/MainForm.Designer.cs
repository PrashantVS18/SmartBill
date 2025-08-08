namespace Billing.Client.UI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            HeaderPanel = new Panel();
            FooterPanel = new Panel();
            CopyRightLable = new Label();
            LeftPanel = new Panel();
            MenuBar = new TreeView();
            MainPanel = new Panel();
            FooterPanel.SuspendLayout();
            LeftPanel.SuspendLayout();
            SuspendLayout();
            // 
            // HeaderPanel
            // 
            HeaderPanel.BorderStyle = BorderStyle.FixedSingle;
            HeaderPanel.Dock = DockStyle.Top;
            HeaderPanel.ForeColor = Color.Transparent;
            HeaderPanel.Location = new Point(0, 0);
            HeaderPanel.Name = "HeaderPanel";
            HeaderPanel.Size = new Size(1402, 81);
            HeaderPanel.TabIndex = 1;
            // 
            // FooterPanel
            // 
            FooterPanel.BackColor = Color.LightGray;
            FooterPanel.Controls.Add(CopyRightLable);
            FooterPanel.Dock = DockStyle.Bottom;
            FooterPanel.Location = new Point(0, 677);
            FooterPanel.Name = "FooterPanel";
            FooterPanel.Size = new Size(1402, 36);
            FooterPanel.TabIndex = 2;
            // 
            // CopyRightLable
            // 
            CopyRightLable.Anchor = AnchorStyles.None;
            CopyRightLable.AutoSize = true;
            CopyRightLable.Font = new Font("Segoe UI", 10.2F, FontStyle.Italic, GraphicsUnit.Point, 0);
            CopyRightLable.ForeColor = SystemColors.ActiveCaptionText;
            CopyRightLable.Location = new Point(2, 9);
            CopyRightLable.Name = "CopyRightLable";
            CopyRightLable.Size = new Size(463, 23);
            CopyRightLable.TabIndex = 0;
            CopyRightLable.Text = "© 2025 SmartBill v1.0 | Developed by ABC. All rights reserved.";
            CopyRightLable.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // LeftPanel
            // 
            LeftPanel.BorderStyle = BorderStyle.FixedSingle;
            LeftPanel.Controls.Add(MenuBar);
            LeftPanel.Dock = DockStyle.Left;
            LeftPanel.Location = new Point(0, 81);
            LeftPanel.Name = "LeftPanel";
            LeftPanel.Size = new Size(229, 596);
            LeftPanel.TabIndex = 3;
            // 
            // MenuBar
            // 
            MenuBar.BackColor = SystemColors.Control;
            MenuBar.BorderStyle = BorderStyle.None;
            MenuBar.Dock = DockStyle.Left;
            MenuBar.FullRowSelect = true;
            MenuBar.HideSelection = false;
            MenuBar.Location = new Point(0, 0);
            MenuBar.Name = "MenuBar";
            MenuBar.ShowLines = false;
            MenuBar.ShowRootLines = false;
            MenuBar.Size = new Size(225, 594);
            MenuBar.TabIndex = 4;
            MenuBar.AfterSelect += MenuBar_AfterSelect;
            // 
            // MainPanel
            // 
            MainPanel.BackgroundImage = (Image)resources.GetObject("MainPanel.BackgroundImage");
            MainPanel.BackgroundImageLayout = ImageLayout.Zoom;
            MainPanel.Dock = DockStyle.Fill;
            MainPanel.Location = new Point(229, 81);
            MainPanel.Name = "MainPanel";
            MainPanel.Size = new Size(1173, 596);
            MainPanel.TabIndex = 4;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1402, 713);
            Controls.Add(MainPanel);
            Controls.Add(LeftPanel);
            Controls.Add(FooterPanel);
            Controls.Add(HeaderPanel);
            ForeColor = SystemColors.Control;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            Load += MainForm_Load;
            FooterPanel.ResumeLayout(false);
            FooterPanel.PerformLayout();
            LeftPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel HeaderPanel;
        private Panel FooterPanel;
        private Panel LeftPanel;
        private Panel MainPanel;
        private Panel panelInventorySubmenu;
        private Panel panelReportsSubmenu;
        private Panel panelBillingSubmenu;
        private Label CopyRightLable;
        private TreeView MenuBar;
    }
}