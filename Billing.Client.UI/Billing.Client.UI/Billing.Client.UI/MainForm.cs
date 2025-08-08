using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Billing.Client.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Bounds = Screen.FromControl(this).WorkingArea;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //LoadMenuBar();
            LoadSideMenu();

        }

        private void LoadSideMenu()
        {
            AddSidebarButton("Dashboard", IconChar.TachometerAlt, Dashboard_Click!);

            // Inventory (with submenu)
            IconButton inventoryBtn = AddSidebarButton("Inventory", IconChar.Boxes, Inventory_Click!);
            panelInventorySubmenu = CreateSubmenuPanel();
            AddSubmenuButton(panelInventorySubmenu, "Add Product", AddProduct_Click!);
            AddSubmenuButton(panelInventorySubmenu, "Stock List", StockList_Click!);
            LeftPanel.Controls.Add(panelInventorySubmenu); 
            AddSidebarButton("Billing", IconChar.Receipt, Billing_Click!);

            AddSidebarButton("Reports", IconChar.ChartBar, Reports_Click!);
        }

        private IconButton AddSidebarButton(string text, IconChar icon, EventHandler clickHandler)
        {
            IconButton btn = new IconButton
            {
                Text = "  " + text,
                IconChar = icon,
                IconColor = Color.Green,
                IconSize = 24,
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                Padding = new Padding(10, 0, 20, 0),
                IconFont = IconFont.Auto
            };
            
            btn.BackColor = Color.LightBlue;
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += clickHandler;
            LeftPanel.Controls.Add(btn);

            return btn;
        }

        private Panel CreateSubmenuPanel()
        {
            Panel subPanel = new Panel
            {
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(45, 45, 60),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Visible = false
            };
            return subPanel;
        }

        private void AddSubmenuButton(Panel parent, string text, EventHandler clickHandler)
        {
            Button subBtn = new Button
            {
                Text = "     " + text,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                BackColor = Color.FromArgb(45, 45, 60)
            };
            subBtn.FlatAppearance.BorderSize = 0;
            subBtn.Click += clickHandler;
            parent.Controls.Add(subBtn);
        }


        private void Dashboard_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Dashboard_Click event");
        }


        private void AddProduct_Click(object sender, EventArgs e)
        {
            MessageBox.Show("AddProduct_Click event");
        }

        private void StockList_Click(object sender, EventArgs e)
        {
            MessageBox.Show("StockList_Click event");
        }

        private void Billing_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Billing_Click event");
        }

        private void Reports_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Reports_Click event");
        }

        private void Inventory_Click(object sender, EventArgs e)
        {
            IconButton? clickedButton = sender as IconButton;

            if (panelInventorySubmenu.Visible)
            {
                panelInventorySubmenu.Visible = false;
                return;
            }

            // First hide all other submenus
            //panelReportsSubmenu.Visible = false;  // commented because not created yet

            // Calculate position BELOW the clicked button
            Point locationInSidebar = clickedButton.Location;
            panelInventorySubmenu.Location = new Point(
                locationInSidebar.X + 10,
                locationInSidebar.Y + clickedButton.Height
            );

            panelInventorySubmenu.Visible = true;
            panelInventorySubmenu.BringToFront();
        }

        private void LoadMenuBar()
        {
            TreeNode billPrintingNode = new TreeNode("Bill Printing");
            billPrintingNode.Nodes.Add("New Bill");
            billPrintingNode.Nodes.Add("Hold Bill");

            // Reports
            TreeNode reportsNode = new TreeNode("Reports");
            reportsNode.Nodes.Add("Daily Report");
            reportsNode.Nodes.Add("Profit/Loss");

            // Inventory
            TreeNode inventoryNode = new TreeNode("Inventory Management");
            inventoryNode.Nodes.Add("Add Product");
            inventoryNode.Nodes.Add("View Stock");

            // Add to TreeView
            MenuBar.Nodes.Add(billPrintingNode);
            MenuBar.Nodes.Add(reportsNode);
            MenuBar.Nodes.Add(inventoryNode);
            MenuBar.SelectedNode = billPrintingNode.Nodes[0];

        }

        private void MenuBar_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(e.Node!.Parent ==  null)
            {
                return;
            }          
            string selected = e.Node!.Text;
            switch (selected)
            {
                case "New Bill":
                    //LoadUserControl(new UC_NewBill());
                    MessageBox.Show(selected);
                    break;
                case "Hold Bill":
                    //LoadUserControl(new UC_HoldBill());
                    MessageBox.Show(selected);
                    break;
                case "Daily Report":
                    //LoadUserControl(new UC_DailyReport());
                    MessageBox.Show(selected);
                    break;
                case "Profit/Loss":
                    //LoadUserControl(new UC_ProfitLoss());
                    MessageBox.Show(selected);
                    break;
                case "Add Product":
                    //LoadUserControl(new UC_AddProduct());
                    MessageBox.Show(selected);
                    break;
                case "View Stock":
                    //LoadUserControl(new UC_ViewStock());
                    MessageBox.Show(selected);
                    break;
                default:
                    break;
            }
            
        }
    }
}
