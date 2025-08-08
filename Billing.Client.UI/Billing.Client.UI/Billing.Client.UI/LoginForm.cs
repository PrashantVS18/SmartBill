using Biling.DataModels.LoginModels;
using Billing.Client.UI.Helper;
using Billing.Utility;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Billing.Client.UI.Login
{
    public partial class LoginForm : Form
    {
        private  string API_BASE_URL { get; } = Properties.Settings.Default.API_Base_URL;
        public LoginForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            // Enable double buffering for smooth painting
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.DoubleBuffer |
                         ControlStyles.ResizeRedraw, true);

            // Setup custom controls
            SetupCustomControls();

            // Center the main panel on form load
            this.Load += LoginForm_Load;
            this.Resize += LoginForm_Resize;

            // Add paint event handlers
            this.Paint += LoginForm_Paint;

            // Add keyboard support
            this.KeyPreview = true;
            this.KeyDown += LoginForm_KeyDown;
        }

        private void SetupCustomControls()
        {
            // Setup textboxes for custom painting
            UIHelper.SetupCustomTextBox(txtUsername);
            UIHelper.SetupCustomTextBox(txtPassword);

            // Add paint events for textboxes
            txtUsername.Paint += (s, e) => UIHelper.PaintTextBox(e, s as TextBox);
            txtPassword.Paint += (s, e) => UIHelper.PaintTextBox(e, s as TextBox);

            // Setup login button
            UIHelper.SetupCustomButton(btnLogin, UIHelper.primaryGreen, UIHelper.white);
            btnLogin.Paint += (s, e) => UIHelper.PaintButton(e, s as Button);
            UIHelper.AddButtonHoverEffects(btnLogin, UIHelper.primaryGreen, UIHelper.darkGreen);

            // Setup exit button
            UIHelper.SetupCustomButton(btnExit, Color.Transparent, UIHelper.darkGrey);
            btnExit.Paint += (s, e) => UIHelper.PaintExitButton(e, s as Button);
            UIHelper.AddButtonHoverEffects(btnExit, Color.Transparent, Color.FromArgb(244, 67, 54));

            // Additional hover effect for exit button
            btnExit.MouseEnter += (s, e) => { btnExit.ForeColor = UIHelper.white; };
            btnExit.MouseLeave += (s, e) => { btnExit.ForeColor = UIHelper.darkGrey; };

            // Setup main panel
            pnlMain.BackColor = Color.Transparent;
            pnlMain.Paint += PnlMain_Paint;

            // Add focus events for textbox repainting
            txtUsername.Enter += (s, e) => txtUsername.Invalidate();
            txtUsername.Leave += (s, e) => txtUsername.Invalidate();
            txtPassword.Enter += (s, e) => txtPassword.Invalidate();
            txtPassword.Leave += (s, e) => txtPassword.Invalidate();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            CenterLoginPanel();
            PositionExitButton();
        }

        private void LoginForm_Resize(object sender, EventArgs e)
        {
            CenterLoginPanel();
            PositionExitButton();
            this.Invalidate(); // Repaint the form background
        }

        private void CenterLoginPanel()
        {
            // Center the main panel in the form
            int x = (this.ClientSize.Width - pnlMain.Width) / 2;
            int y = (this.ClientSize.Height - pnlMain.Height) / 2;
            pnlMain.Location = new Point(x, y);
        }

        private void PositionExitButton()
        {
            // Position exit button at top-right corner
            btnExit.Location = new Point(this.ClientSize.Width - 50, 10);
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle keyboard shortcuts
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                BtnLogin_Click(this, EventArgs.Empty);
            }
        }

        private void LoginForm_Paint(object sender, PaintEventArgs e)
        {
            // Use UIHelper for form background painting
            UIHelper.PaintFormBackground(e, this);

            // Draw main panel with shadow
            UIHelper.PaintPanelWithShadow(e.Graphics, pnlMain);
        }

        private void PnlMain_Paint(object sender, PaintEventArgs e)
        {
            // The panel background is handled by the form's paint method
            // This ensures the panel remains transparent
        }

        // Event handlers
        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Login Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Logging in...";

            try
            {
                // Create login request object
                var loginRequest = new
                {
                    Username = username,
                    Password = password
                };

                // Send POST request with body
                var response = await ApiHelper.PostAsync<LoginResponse>(
                    $"{API_BASE_URL}/Login/login",
                    body: loginRequest
                );


                // Handle response (same as above)
                if (response.Success)
                {
                    //Properties.Settings.Default.AuthToken = response.Token;
                    //Properties.Settings.Default.CurrentUser = response.User.Username;
                    //Properties.Settings.Default.Save();

                    MessageBox.Show($"Welcome {response.User.Username}!", "Login Successful",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(response.Message ?? "Login failed", "Login Failed",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Clean up resources
            base.OnFormClosing(e);
        }
    }
}