using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PIA_PI
{
    public partial class Form1 : Form
    {

        string img1 = "..//..//Recursos/Bienvenido.png";
 



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            menuStrip1.ForeColor = Color.AntiqueWhite;
            Image foto1 = Image.FromFile(img1);

            pictureBox1.Image = foto1;

            label2.Text = "Derechos Reservados 2024.";
        }
      

        private void editorDeImágenesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VariablesGlobales.imagenes.ShowDialog();

        }
        
        private void editorDeVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VariablesGlobales.videos.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
     

        private void detectorDeRostrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VariablesGlobales.camera.ShowDialog();
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VariablesGlobales.manual.ShowDialog();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("¿Desea salir?", "Aviso", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void editorDeVideoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            VariablesGlobales.videos.ShowDialog();

            /*
            if (VariablesGlobales.videos == null)
            {
                VariablesGlobales.videos = new EditorVideo(); // Inicializa el formulario si no está inicializado
            }
            VariablesGlobales.videos.ShowDialog();
            */
        }

        private void salirToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var result = MessageBox.Show("¿Desea salir?", "Aviso", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
