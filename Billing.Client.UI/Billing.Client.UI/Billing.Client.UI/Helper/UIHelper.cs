using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Billing.Client.UI.Helper
{
    public static class UIHelper
    {
        // Color definitions
        public static readonly Color primaryGreen = Color.FromArgb(76, 175, 80);
        public static readonly Color darkGreen = Color.FromArgb(67, 160, 71);
        public static readonly Color lightGrey = Color.FromArgb(250, 250, 250);
        public static readonly Color mediumGrey = Color.FromArgb(189, 189, 189);
        public static readonly Color darkGrey = Color.FromArgb(97, 97, 97);
        public static readonly Color white = Color.White;

        #region Button Painting

        /// <summary>
        /// Paints a modern button with gradient background and highlight
        /// </summary>
        public static void PaintButton(PaintEventArgs e, Button btn, int cornerRadius = 12)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, btn.Width, btn.Height);

            using (GraphicsPath path = CreateRoundedRectangle(rect, cornerRadius))
            {
                // Gradient background
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect,
                    btn.BackColor,
                    Color.FromArgb(Math.Max(0, btn.BackColor.R - 15),
                                 Math.Max(0, btn.BackColor.G - 15),
                                 Math.Max(0, btn.BackColor.B - 15)),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                // Subtle highlight at the top
                using (Pen highlightPen = new Pen(Color.FromArgb(80, Color.White), 1))
                {
                    Rectangle highlightRect = new Rectangle(rect.X + 1, rect.Y + 1,
                                                          rect.Width - 2, rect.Height - 2);
                    using (GraphicsPath highlightPath = CreateRoundedRectangle(highlightRect, cornerRadius - 1))
                    {
                        g.DrawPath(highlightPen, highlightPath);
                    }
                }
            }

            // Draw the text (centered)
            TextRenderer.DrawText(
                g,
                btn.Text,
                btn.Font,
                rect,
                btn.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );
        }

        /// <summary>
        /// Paints a circular exit button
        /// </summary>
        public static void PaintExitButton(PaintEventArgs e, Button btn)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (btn.BackColor != Color.Transparent)
            {
                Rectangle rect = new Rectangle(2, 2, btn.Width, btn.Height);
                using (SolidBrush brush = new SolidBrush(btn.BackColor))
                {
                    g.FillEllipse(brush, rect);
                }
            }
        }

        #endregion

        #region TextBox Painting

        /// <summary>
        /// Paints a modern textbox with rounded corners and focus effect
        /// </summary>
        public static void PaintTextBox(PaintEventArgs e, TextBox txt, int cornerRadius = 8)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, txt.Width, txt.Height);

            using (GraphicsPath path = CreateRoundedRectangle(rect, cornerRadius))
            {
                // Background
                using (SolidBrush bgBrush = new SolidBrush(white))
                {
                    g.FillPath(bgBrush, path);
                }

                // Border
                Color borderColor = txt.Focused ? darkGreen : mediumGrey;
                int borderWidth = txt.Focused ? 3 : 2;
                using (Pen borderPen = new Pen(borderColor, borderWidth))
                {
                    g.DrawPath(borderPen, path);
                }
            }
        }

        #endregion

        #region Form Background Painting

        /// <summary>
        /// Paints the login form background with corner accents
        /// </summary>
        public static void PaintFormBackground(PaintEventArgs e, Form form)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Simple background
            using (SolidBrush brush = new SolidBrush(lightGrey))
            {
                g.FillRectangle(brush, form.ClientRectangle);
            }

            // Draw subtle corner accents
            DrawCornerAccents(g, form.ClientSize);
        }

        /// <summary>
        /// Paints a panel with shadow and rounded corners
        /// </summary>
        public static void PaintPanelWithShadow(Graphics g, Panel panel, int cornerRadius = 20)
        {
            Rectangle panelRect = new Rectangle(panel.Location.X - 15, panel.Location.Y - 15,
                                              panel.Width + 30, panel.Height + 30);

            using (GraphicsPath path = CreateRoundedRectangle(panelRect, cornerRadius))
            {
                // Draw shadow
                Rectangle shadowRect = panelRect;
                shadowRect.Offset(5, 5);
                using (GraphicsPath shadowPath = CreateRoundedRectangle(shadowRect, cornerRadius))
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }

                // Draw panel background
                using (SolidBrush panelBrush = new SolidBrush(white))
                {
                    g.FillPath(panelBrush, path);
                }

                // Draw panel border
                using (Pen borderPen = new Pen(mediumGrey, 2))
                {
                    g.DrawPath(borderPen, path);
                }

                // Draw accent line
                using (Pen accentPen = new Pen(primaryGreen, 4))
                {
                    g.DrawLine(accentPen, panelRect.X + 20, panelRect.Y + 100,
                              panelRect.Right - 20, panelRect.Y + 100);
                }
            }
        }

        #endregion

        #region Control Setup Helpers

        /// <summary>
        /// Sets up a button for custom painting
        /// </summary>
        public static void SetupCustomButton(Button button, Color backColor, Color foreColor)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.UseVisualStyleBackColor = false;


        }

        /// <summary>
        /// Sets up a textbox for custom painting
        /// </summary>
        public static void SetupCustomTextBox(TextBox textBox)
        {
            textBox.BackColor = lightGrey;
            // Add padding for better text positioning
            textBox.Padding = new Padding(10, 8, 10, 8);
        }

        /// <summary>
        /// Adds hover effects to a button
        /// </summary>
        public static void AddButtonHoverEffects(Button button, Color normalColor, Color hoverColor)
        {
            Color originalForeColor = button.ForeColor;

            button.MouseEnter += (s, e) =>
            {
                button.BackColor = hoverColor;
                button.Invalidate();
            };

            button.MouseLeave += (s, e) =>
            {
                button.BackColor = normalColor;
                button.ForeColor = originalForeColor;
                button.Invalidate();
            };
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Creates a rounded rectangle graphics path
        /// </summary>
        private static GraphicsPath CreateRoundedRectangle(Rectangle rect, int cornerRadius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = cornerRadius * 2;

            // Optimize by reducing calculations
            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);

            // Top-left arc
            path.AddArc(arc, 180, 90);

            // Top-right arc
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom-right arc
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom-left arc
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Draws corner accents on the form
        /// </summary>
        private static void DrawCornerAccents(Graphics g, Size clientSize)
        {
            int size = 50;

            // Top-left accent
            using (LinearGradientBrush brush = new LinearGradientBrush(
                new Rectangle(0, 0, size, size),
                Color.FromArgb(30, primaryGreen),
                Color.Transparent,
                LinearGradientMode.ForwardDiagonal))
            {
                g.FillRectangle(brush, 0, 0, size, size);
            }

            // Bottom-right accent
            using (LinearGradientBrush brush = new LinearGradientBrush(
                new Rectangle(clientSize.Width - size, clientSize.Height - size, size, size),
                Color.FromArgb(30, primaryGreen),
                Color.Transparent,
                LinearGradientMode.BackwardDiagonal))
            {
                g.FillRectangle(brush, clientSize.Width - size, clientSize.Height - size, size, size);
            }
        }

        #endregion
    }
}