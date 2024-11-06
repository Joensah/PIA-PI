using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PIA_PI
{
    public partial class ManualUsuario : Form
    {
        string img1 = "..//..//Recursos/Manual1.png";
        string img2 = "..//..//Recursos/Manual2.png";
        string img3 = "..//..//Recursos/Manual3.png";
        string img4 = "..//..//Recursos/Manual4.png";

        int indice = 1;


        public ManualUsuario()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ManualUsuario_Load(object sender, EventArgs e)
        {
            //menuStrip1.ForeColor = Color.AntiqueWhite;
            Image foto1 = Image.FromFile(img1);

            pictureBox1.Image = foto1;

            label2.Text = 
                "Interfaz principal" +
                "\n1.- Botón para la ventana Editor de imágenes." +
                "\n2.- Botón para ventana Editor de vídeo." +
                "\n3.- Botón para la ventana Detector de Rostros." +
                "\n4.- Botón para la ventana del Manual." +
                "\n5.- Botón para salir de la aplicación.";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            indice++;
            if (indice > 4)
            {
                indice = 1;
            }
            setimage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            indice--;
            if (indice < 1)
            {
                indice = 4;
            }
            setimage();
        }


        private void setimage()
        {
            Image foto1 = Image.FromFile(img1);
            Image foto2 = Image.FromFile(img2);
            Image foto3 = Image.FromFile(img3);
            Image foto4 = Image.FromFile(img4);


            switch (indice)
            {
               
                case 1:
                    pictureBox1.Image = foto1;//Interfaz principal
                    label2.Text =
                "Interfaz principal" +
                "\n1.- Botón para la ventana Editor de imágenes." +
                "\n2.- Botón para ventana Editor de vídeo." +
                "\n3.- Botón para la ventana Detector de Rostros." +
                "\n4.- Botón para la ventana del Manual." +
                "\n5.- Botón para salir de la aplicación.";

                    break;

                case 2:
                    pictureBox1.Image = foto2;//Editor de imagenes
                    label2.Text =
                "Editor de imágenes" +
                "\n1.- Cargar una imagen." +
                "\n2.- Seleccionar el filtro y aplicar." +
                "\n3.- Guardar la imagen." +
                "\n•   También cuenta con una barra para visualizar el progreso y un histograma.";

                    break;

                case 3:
                    pictureBox1.Image = foto3;//Editor de video
                    label2.Text =
                "Editor de vídeo" +
                "\n1.- Cargar el vídeo deseado." +
                "\n2.- Seleccionar el filtro de la lista desplegada." +
                "\n3.- Aplicar el filtro para ver los cambios aplicados" +
                "\n•   Cuenta con controles para reproducir el vídeo.";

                    break;

                case 4:
                    pictureBox1.Image = foto4;//Detector de rostros
                    label2.Text =
                "Detección de rostros" +
                "\n1.- Activar la cámara." +
                "\n2.- Botón para desactivar la cámara." +
                "\n• Adicional se puede seleccionar la fuente de la cámara.";

                    break;

            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
