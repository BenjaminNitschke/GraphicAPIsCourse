using System;
using System.Drawing;
using System.Windows.Forms;

namespace GraphicsEngine
{
    public partial class GraphicsEngineForm : Form
    {
        public GraphicsEngineForm()
        {
            InitializeComponent();
            BackColor = Color.Black;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
            if (e.KeyCode == Keys.Space)
                Program.ToggleAPIAndRestart();
            base.OnKeyDown(e);
        }
    }
}