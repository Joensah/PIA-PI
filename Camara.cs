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

using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;

using Emgu.CV;
using Emgu.CV.Structure;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;


namespace PIA_PI
{
    public partial class Camara : Form
    {

        //--Manejo de ventana 
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        private bool encendido = false;

        //------

        //---Manejo de camara
        FilterInfoCollection filterc;
        VideoCaptureDevice vidcap;

        //--

        //---Detecion de cara
        MotionDetector Detector;
        float NivelDeteccion;
        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("..//..//Recursos//haarcascade_frontalface_alt_tree.xml");
        Random random = new Random();
        Rectangle[] rectangles = null;

        //---




        public Camara()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e) //boton para apagar la camara
        {
            radioButton1.Checked = false;
            radioButton1.BackColor = Color.FromArgb(64, 64, 64);
            radioButton2.Checked = true;
            radioButton2.BackColor = Color.Red;
            vidcap.Stop();
            pictureBox1.Image = null;
            encendido = false;

            //   videoSourcePlayer1.VideoSource = null;
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Camara_Load(object sender, EventArgs e)
        {
            filterc = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo dev in filterc)
            {
                comboBox1.Items.Add(dev.Name);
                comboBox1.SelectedIndex = 0;

            }


            //Detector = new MotionDetector(new TwoFramesDifferenceDetector(), new MotionBorderHighlighting());
            //NivelDeteccion = 0;
            radioButton2.Checked = true;
            radioButton2.BackColor = Color.Red;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (encendido)
            {
                encendido = false;
                vidcap.Stop();
                this.Close();
            }
            else
            {
                this.Close();
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        //Contador de personas
        private void DetectorCamera(object sender, NewFrameEventArgs eventArgs)
        {
            int i = 1;
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();

            image.RotateFlip(RotateFlipType.RotateNoneFlipX);

            Font arial = new Font("Arial", 45, FontStyle.Regular);
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(image);
            Brush brush = new SolidBrush(Color.FromKnownColor(KnownColor.GreenYellow));
            rectangles = cascadeClassifier.DetectMultiScale(grayImage, 1.2, 1);
            Pen pen = new Pen(Color.Red, 4);

            string texto = "Persona #";

            foreach (Rectangle rectangle in rectangles)
            {
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    graphics.DrawRectangle(pen, rectangle);
                    graphics.DrawString(texto + i++.ToString(), arial, brush, rectangle);
                }
            }
            pictureBox1.Image = image;
        }

   

        private void button2_Click(object sender, EventArgs e)
        {
            filterc = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo dev in filterc)
            {
                comboBox1.Items.Add(dev.Name);
                comboBox1.SelectedIndex = 0;

            }
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btncamara_Click(object sender, EventArgs e)
        {
            vidcap = new VideoCaptureDevice(filterc[comboBox1.SelectedIndex].MonikerString);
            //      videoSourcePlayer1.VideoSource = vidcap;
            vidcap.NewFrame += new NewFrameEventHandler(DetectorCamera);

            encendido = true;
            radioButton1.Checked = true;
            radioButton1.BackColor = Color.Green;
            radioButton2.Checked = false;
            radioButton2.BackColor = Color.FromArgb(64, 64, 64);
            vidcap.Start();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton1.BackColor = Color.FromArgb(64, 64, 64);
            radioButton2.Checked = true;
            radioButton2.BackColor = Color.Red;
            vidcap.Stop();
            pictureBox1.Image = null;
            encendido = false;

            //   videoSourcePlayer1.VideoSource = null;
        }
    }
}
