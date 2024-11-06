using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Runtime.InteropServices;
using System.Drawing.Imaging;

using Emgu.CV.UI;
using Emgu.Util;

using AForge.Imaging.ColorReduction;
using AForge.Imaging.ComplexFilters;
using AForge.Imaging.Filters;
using AForge.Imaging.Textures;
using AForge.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections.Specialized;

namespace PIA_PI
{
    public partial class EditorVideo : Form
    {
        VideoCapture Video;
        Image<Bgr, byte> currentFrame;
        double duracion;
        double frameCount;
        bool isVideo = false;
        bool isplay = false;
        int opcion_filtro = 0;
        Mat math;
        Bitmap bmp;

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        public EditorVideo()
        {
            InitializeComponent();
        }

        private void VideoFrameCapture(object sender, EventArgs e)
        {
            // Verificar que el video esté abierto y en reproducción
            if (Video != null && Video.IsOpened)
            {
                // Obtener el número actual de fotograma
                frameCount = (int)Video.GetCaptureProperty(CapProp.PosFrames);

                // Si el video ha llegado al final, reiniciar el video al primer fotograma para hacerlo en loop
                if (frameCount >= duracion - 2)
                {
                    // Reiniciar el video al primer fotograma para loop
                    Video.SetCaptureProperty(CapProp.PosFrames, 0);
                    frameCount = 0; // Reiniciar el contador de fotogramas
                }

                // Verificar si el video está en reproducción
                if (isplay)
                {
                    // Capturar el fotograma actual
                    math = Video.QueryFrame();
                    if (math != null) // Asegúrate de que el fotograma es válido
                    {
                        currentFrame = math.ToImage<Bgr, byte>();

                        // Aplicar el filtro dependiendo de la opción seleccionada
                        switch (opcion_filtro)
                        {
                            /*
                            1-Efecto Solarizado
                            2-Filtro de Calor
                            3-Efecto PopArt
                            4-Efecto Emboss
                            5-Efecto Vignette
                            6-Efecto Glitch
                            7-Efecto Negativo
                            8-Detección de Bordes
                            9-Efecto Cómic
                            10-Efecto Gradiente Arcoiris
                                */

                            case 1: //Efecto Solarizado
                                ApplySolarizationEffect(currentFrame.Bitmap);
                                break;
                            case 2: //Efecto Filtro de Calor
                                ApplyHeatEffect(currentFrame.Bitmap);
                                break;
                            case 3: //Efecto PopArt
                                ApplyPopArtEffect(currentFrame.Bitmap);
                                break;
                            case 4: //Efecto Emboss
                                ApplyEmbossEffect(currentFrame.Bitmap);
                                break;
                            case 5: //Efecto Vignette
                                ApplyVignetteEffect(currentFrame.Bitmap);
                                break;
                            case 6: //Efecto Glitch
                                ApplyGlitchEffect(currentFrame.Bitmap);
                                break;
                            case 7: //Efecto Negativo
                                ApplyNegativeEffect(currentFrame.Bitmap);
                                break;
                            case 8: //Efecto Detección de Bordes
                                ApplyEdgeDetectionEffect(currentFrame.Bitmap);
                                break;
                            case 9: //Efecto Comic
                                ApplyComicEffect(currentFrame.Bitmap);
                                break;
                            case 10: //Efecto Gradiente Arcoiris
                                ApplyRainbowGradientEffect(currentFrame.Bitmap);
                                break;

                            default:
                                pictureBox1.Image = currentFrame.Bitmap;
                                break;

                        }
                      
                    }

                }
            }
            else
 
            {
                // Si no hay un video o el video ha terminado, limpiar la imagen del PictureBox
                pictureBox1.Image = null; // Limpia la imagen
            }


        }

    

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Archivos de video|*.mp4;*.avi;*.wmv;*.mkv|Todos los archivos|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string rutaArchivo = openFileDialog.FileName;
                Video = new VideoCapture(openFileDialog.FileName);
                math = new Mat();
                Video.Read(math);
                currentFrame = math.ToImage<Bgr, byte>();
                pictureBox1.Image = currentFrame.Bitmap;
                opcion_filtro = 0; // Restablecer el filtro (si es necesario)
                isVideo = true;
                isplay = false;
                duracion = Video.GetCaptureProperty(CapProp.FrameCount);
                frameCount = Video.GetCaptureProperty(CapProp.PosFrames);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            isplay = true;
            if (isVideo)
            {
                Application.Idle += new EventHandler(VideoFrameCapture);
            }
            else
            {
                MessageBox.Show("No es video", "Aviso", MessageBoxButtons.OK);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            isplay = false;
        }



        private void button2_Click(object sender, EventArgs e)
        {
            // Detener la reproducción del video
            isplay = false;

            var quitarVideo = MessageBox.Show("¿Desea quitar el video?", "Aviso", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (quitarVideo == DialogResult.Yes)
            {
                // Liberar el objeto VideoCapture si existe
                if (Video != null)
                {
                    Video.Dispose();  // Libera los recursos utilizados por VideoCapture
                    Video = null;     // Opcional: asignar null para evitar referencia a un objeto liberado
                }

                // Limpiar el PictureBox
                pictureBox1.Image = null; // Quitar la imagen actual del PictureBox

                // Reiniciar los contadores
                frameCount = 0; // Reiniciar el contador de frames
                duracion = 0;   // Reiniciar la duración del video
                isVideo = false; // Indicar que ya no hay un video cargado

                // Mensaje para el usuario al quitar el video cargado
                //MessageBox.Show("El video ha sido eliminado.", "Información", MessageBoxButtons.OK);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            isplay = false; // Detener la reproducción
            frameCount = 0; // Reiniciar el contador de fotogramas
            Video.SetCaptureProperty(CapProp.PosFrames, 0); // Volver al primer fotograma

            if (currentFrame != null)
            {
                pictureBox1.Image = currentFrame.Bitmap; // Mostrar el primer fotograma (o una imagen en blanco)
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (comboBox1.SelectedIndex != -1 && pictureBox1.Image != null)
            {
                string filtro = comboBox1.SelectedItem.ToString();
            /*
            1-Efecto Solarizado
            2-Filtro de Calor
            3-Efecto PopArt
            4-Efecto Emboss
            5-Efecto Vignette
            6-Efecto Glitch
            7-Efecto Negativo
            8-Detección de Bordes
            9-Efecto Cómic
            10-Efecto Gradiente Arcoiris
                */
        

            if (filtro == "1-Efecto Solarizado")
            {
                opcion_filtro = 1;
            }

            if (filtro == "2-Filtro de Calor")
            {
                opcion_filtro = 2;

            }

            if (filtro == "3-Efecto PopArt")
            {
                opcion_filtro = 3;
            }

            if (filtro == "4-Efecto Emboss")
            {
                opcion_filtro = 4;
            }

            if (filtro == "5-Efecto Vignette")
            {
                opcion_filtro = 5;

            }

            if (filtro == "6-Efecto Glitch")
            {
                opcion_filtro = 6;

            }

            if (filtro == "7-Efecto Negativo")
            {
                opcion_filtro = 7;

            }

            if (filtro == "8-Detección de Bordes")
            {
                opcion_filtro = 8;

            }

            if (filtro == "9-Efecto Cómic")
            {
                opcion_filtro = 9;
            }

            if (filtro == "10-Efecto Gradiente Arcoiris")
            {
                opcion_filtro = 10;
                MessageBox.Show("Filtro aplicado", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                isplay = true;

                }
            }
            else
            {
                MessageBox.Show("Seleccione una opción y revise si tiene un video cargado", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }




        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }







        //========TODOS LOS EFECTOS=======
        /*
                1 Efecto Solarizado
                2 Filtro de Calor
                3 Efecto PopArt
                4 Efecto Emboss
                5 Efecto Vignette
                6 Efecto Glitch
                7 Efecto Negativo
                8 Detección de Bordes
                9 Efecto Cómic
               10 Efecto Difuminado(Gaussiano)
        */

        //==========================================EFECTO DE SOLARIZADO========================================================================
        private Bitmap ApplySolarizationEffect(Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            int totalPixels = sourceBitmap.Width * sourceBitmap.Height;
            int currentPixel = 0;

            for (int y = 0; y < sourceBitmap.Height; y++)
            {
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    Color pixelColor = sourceBitmap.GetPixel(x, y);

                    // Se calcula la luminosidad promedio
                    int avg = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    // Umbral para el efecto de solarizado
                    if (avg < 128) // Puedes ajustar este valor para cambiar la intensidad del efecto
                    {
                        // Invertir el color si la luminosidad es baja
                        int invertedR = 255 - pixelColor.R;
                        int invertedG = 255 - pixelColor.G;
                        int invertedB = 255 - pixelColor.B;

                        bmp.SetPixel(x, y, Color.FromArgb(invertedR, invertedG, invertedB));
                    }
                    else
                    {
                        // Mantener el color original si la luminosidad es alta
                        bmp.SetPixel(x, y, pixelColor);
                    }

                    currentPixel++;
                    int progressPercentage = currentPixel * 100 / totalPixels;
                    Application.DoEvents();
                }
            }

            // Recalcular y actualizar el histograma con la imagen filtrada

            return bmp;
        }

        //==========================================FILTRO DE CALOR======================================================================

        private Bitmap ApplyHeatEffect(Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            for (int y = 0; y < sourceBitmap.Height; y++)
            {
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    Color originalColor = sourceBitmap.GetPixel(x, y);

                    // Convertir el color a una intensidad de gris (promedio de R, G, B)
                    int intensity = (originalColor.R + originalColor.G + originalColor.B) / 3;

                    // Mapeo de intensidad a una paleta de colores (heatmap)
                    Color heatmapColor;
                    if (intensity < 64) // frío
                        heatmapColor = Color.FromArgb(0, 0, 255 - (intensity * 4)); // azul a azul oscuro
                    else if (intensity < 128) // intermedio frío
                        heatmapColor = Color.FromArgb(0, (intensity - 64) * 4, 255); // verde a azul
                    else if (intensity < 192) // intermedio cálido
                        heatmapColor = Color.FromArgb((intensity - 128) * 4, 255, 255 - (intensity - 128) * 4); // amarillo a verde
                    else // cálido
                        heatmapColor = Color.FromArgb(255, 255 - (intensity - 192) * 4, 0); // rojo a amarillo

                    bmp.SetPixel(x, y, heatmapColor);
                }
            }


            // Recalcular y actualizar el histograma con la imagen filtrada

            return bmp; // Devuelve la imagen con el filtro aplicado
        }

        //==========================================EFECTO POPART========================================================================

        private Bitmap ApplyPopArtEffect(Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            // Definir una paleta de colores vibrantes
            Color[] vibrantColors = {
        Color.FromArgb(255, 0, 0),    // Rojo brillante
        Color.FromArgb(0, 255, 0),    // Verde brillante
        Color.FromArgb(0, 0, 255),    // Azul brillante
        Color.FromArgb(255, 255, 0),  // Amarillo brillante
        Color.FromArgb(255, 165, 0),  // Naranja brillante
        Color.FromArgb(128, 0, 128)   // Púrpura
    };

            for (int y = 0; y < sourceBitmap.Height; y++)
            {
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    Color pixelColor = sourceBitmap.GetPixel(x, y);

                    // Posterizar el color
                    int r = (pixelColor.R / 128) * 128; // Redondea a múltiplos de 128
                    int g = (pixelColor.G / 128) * 128; // Redondea a múltiplos de 128
                    int b = (pixelColor.B / 128) * 128; // Redondea a múltiplos de 128

                    // Encontrar el color más cercano en la paleta
                    Color newColor = vibrantColors.OrderBy(c =>
                        Math.Abs(c.R - r) + Math.Abs(c.G - g) + Math.Abs(c.B - b)).First();

                    bmp.SetPixel(x, y, newColor);
                }
            }

            // Recalcular y actualizar el histograma con la imagen filtrada

            return bmp;
        }

        //==========================================EFECTO EMBOSS========================================================================

        private Bitmap ApplyEmbossEffect(Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            // Definir el kernel para el efecto emboss
            int[,] embossKernel = new int[,]
            {
        { -2, -1, 0 },
        { -1,  1, 1 },
        {  0,  1, 2 }
            };

            for (int y = 1; y < sourceBitmap.Height - 1; y++)
            {
                for (int x = 1; x < sourceBitmap.Width - 1; x++)
                {
                    int r = 0, g = 0, b = 0;

                    // Aplicar el kernel
                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            Color pixelColor = sourceBitmap.GetPixel(x + kx, y + ky);
                            r += pixelColor.R * embossKernel[ky + 1, kx + 1];
                            g += pixelColor.G * embossKernel[ky + 1, kx + 1];
                            b += pixelColor.B * embossKernel[ky + 1, kx + 1];
                        }
                    }

                    // Clampeo de los valores RGB
                    r = Math.Min(Math.Max(r + 128, 0), 255); // Sumar 128 para evitar negativos
                    g = Math.Min(Math.Max(g + 128, 0), 255);
                    b = Math.Min(Math.Max(b + 128, 0), 255);

                    bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            // Recalcular y actualizar el histograma con la imagen filtrada

            return bmp;
        }

        //==========================================EFECTO VIGNETTE======================================================================

        private Bitmap ApplyVignetteEffect(Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            // Calcular el centro de la imagen
            int centerX = sourceBitmap.Width / 2;
            int centerY = sourceBitmap.Height / 2;

            // Incrementar el radio máximo para suavizar el viñeteado
            double maxRadius = Math.Sqrt(centerX * centerX + centerY * centerY) * 1.2; // Aumenta el valor del multiplicador para suavizar más

            for (int y = 0; y < sourceBitmap.Height; y++)
            {
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    // Calcular la distancia desde el centro
                    int dx = x - centerX;
                    int dy = y - centerY;
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    // Calcular el factor de viñeteado, asegurando que esté entre 0 y 1
                    double vignetteFactor = 1 - ClampDouble(distance / maxRadius, 0.0, 1.0);
                    vignetteFactor = Math.Pow(vignetteFactor, 2.5); // Ajuste para suavizar la caída (cuanto más alto el exponente, más suave)

                    // Obtener el color original
                    Color pixelColor = sourceBitmap.GetPixel(x, y);

                    // Aplicar el efecto de viñeteado
                    int r = (int)(pixelColor.R * vignetteFactor);
                    int g = (int)(pixelColor.G * vignetteFactor);
                    int b = (int)(pixelColor.B * vignetteFactor);

                    // Clampeo de los valores RGB
                    r = ClampInt(r, 0, 255);
                    g = ClampInt(g, 0, 255);
                    b = ClampInt(b, 0, 255);

                    bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            // Recalcular y actualizar el histograma con la imagen filtrada

            return bmp;
        }

        // Función para asegurar que los valores enteros estén en el rango permitido
        private int ClampInt(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        // Función para asegurar que los valores de tipo double estén en el rango permitido
        private double ClampDouble(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        //==========================================EFECTO GLITCH========================================================================

        private Bitmap ApplyGlitchEffect(Bitmap sourceBitmap)
        {
            Random random = new Random();
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            int maxOffset = 10; // Desplazamiento máximo en píxeles

            for (int y = 0; y < sourceBitmap.Height; y++)
            {
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    // Obtener el color original
                    Color pixelColor = sourceBitmap.GetPixel(x, y);

                    // Desplazamiento aleatorio en el eje X
                    int offsetX = random.Next(-maxOffset, maxOffset + 1);
                    int offsetY = random.Next(-maxOffset / 2, maxOffset / 2 + 1); // Desplazamiento vertical más pequeño

                    // Asegurarse de que el nuevo valor de X e Y estén dentro de los límites
                    int newX = Clamp4(x + offsetX, 0, sourceBitmap.Width - 1);
                    int newY = Clamp4(y + offsetY, 0, sourceBitmap.Height - 1);

                    // Establecer el color del píxel en la nueva posición
                    bmp.SetPixel(newX, newY, pixelColor);

                    // Alteraciones de color aleatorias
                    if (random.Next(100) < 40) // 40% de probabilidad de distorsionar el color
                    {
                        // Desplazar los canales de color aleatoriamente
                        int rOffset = random.Next(-50, 50);
                        int gOffset = random.Next(-50, 50);
                        int bOffset = random.Next(-50, 50);

                        // Ajustar el color para distorsionar
                        int r = Clamp4(pixelColor.R + rOffset, 0, 255);
                        int g = Clamp4(pixelColor.G + gOffset, 0, 255);
                        int b = Clamp4(pixelColor.B + bOffset, 0, 255);

                        // Establecer el nuevo color distorsionado
                        bmp.SetPixel(newX, newY, Color.FromArgb(r, g, b));
                    }
                }
            }


            // Recalcular y actualizar el histograma con la imagen filtrada

            return bmp;
        }
        // Función para asegurar que los valores estén en el rango permitido
        private int Clamp4(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        //==========================================EFECTO NEGATIVO======================================================================

        private Bitmap ApplyNegativeEffect(Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            for (int y = 0; y < sourceBitmap.Height; y++)
            {
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    // Obtener el color original del píxel
                    Color pixelColor = sourceBitmap.GetPixel(x, y);

                    // Invertir los componentes de color
                    int r = 255 - pixelColor.R;
                    int g = 255 - pixelColor.G;
                    int b = 255 - pixelColor.B;

                    // Establecer el nuevo color en el píxel
                    bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }


            // Recalcular y actualizar el histograma con la imagen filtrada

            return bmp;
        }


        //==========================================DETECCIÓN DE BORDES==================================================================

        private Bitmap ApplyEdgeDetectionEffect(Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            // Definir los kernels de Sobel para la detección de bordes
            int[,] gx = new int[,]
            {
        { -1, 0, 1 },
        { -2, 0, 2 },
        { -1, 0, 1 }
            };

            int[,] gy = new int[,]
            {
        { 1, 2, 1 },
        { 0, 0, 0 },
        { -1, -2, -1 }
            };

            for (int y = 1; y < sourceBitmap.Height - 1; y++)
            {
                for (int x = 1; x < sourceBitmap.Width - 1; x++)
                {
                    // Inicializar los acumuladores para los componentes x e y
                    int pixelX = 0;
                    int pixelY = 0;

                    // Aplicar la convolución con los kernels
                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            Color pixelColor = sourceBitmap.GetPixel(x + kx, y + ky);
                            int gray = (pixelColor.R + pixelColor.G + pixelColor.B) / 3; // Convertir a escala de grises

                            pixelX += gray * gx[ky + 1, kx + 1];
                            pixelY += gray * gy[ky + 1, kx + 1];
                        }
                    }

                    // Calcular la magnitud del gradiente
                    int magnitude = (int)Math.Sqrt(pixelX * pixelX + pixelY * pixelY);
                    magnitude = Clamp2(magnitude, 0, 255); // Clampear el valor

                    // Establecer el nuevo color en el píxel
                    bmp.SetPixel(x, y, Color.FromArgb(magnitude, magnitude, magnitude)); // Escala de grises
                }
            }


            // Recalcular y actualizar el histograma con la imagen filtrada

            return bmp;
        }

        // Función para asegurar que los valores estén en el rango permitido
        private int Clamp2(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }


        //==========================================EFECTO DE COMIC======================================================================
        private Bitmap ApplyComicEffect(Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            // Calcular el total correcto de píxeles
            int totalPixels = sourceBitmap.Width * sourceBitmap.Height;
            int currentPixel = 0;

            for (int y = 0; y < sourceBitmap.Height; y++)
            {
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    Color pixelColor = sourceBitmap.GetPixel(x, y);

                    int avg = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    bmp.SetPixel(x, y, avg > 127 ? pixelColor : Color.Black);

                    currentPixel++;
                    int progressPercentage = currentPixel * 100 / totalPixels;



                    Application.DoEvents();
                }
            }


            // Recalcular y actualizar el histograma con la imagen filtrada

            return bmp;
        }

        //==========================================EFECTO DIFUMINADO(GAUSSIANO)========================================================================

        private Bitmap ApplyGaussianBlur(Bitmap sourceBitmap)
        {
            // Definir la matriz de convolución del desenfoque gaussiano
            double[,] kernel = {
        { 1,  4,  6,  4, 1 },
        { 4, 16, 24, 16, 4 },
        { 6, 24, 36, 24, 6 },
        { 4, 16, 24, 16, 4 },
        { 1,  4,  6,  4, 1 }
    };

            double kernelSum = 256.0; // Sumar todos los valores del kernel

            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            for (int y = 2; y < sourceBitmap.Height - 2; y++)
            {
                for (int x = 2; x < sourceBitmap.Width - 2; x++)
                {
                    double r = 0, g = 0, b = 0;

                    // Aplicar la convolución
                    for (int ky = -2; ky <= 2; ky++)
                    {
                        for (int kx = -2; kx <= 2; kx++)
                        {
                            Color pixelColor = sourceBitmap.GetPixel(x + kx, y + ky);
                            r += pixelColor.R * kernel[ky + 2, kx + 2];
                            g += pixelColor.G * kernel[ky + 2, kx + 2];
                            b += pixelColor.B * kernel[ky + 2, kx + 2];
                        }
                    }

                    // Normalizar los valores RGB
                    r /= kernelSum;
                    g /= kernelSum;
                    b /= kernelSum;

                    // Asegurarse de que los valores RGB estén en el rango de 0 a 255
                    bmp.SetPixel(x, y, Color.FromArgb(
                        Clamp3((int)r, 0, 255),
                        Clamp3((int)g, 0, 255),
                        Clamp3((int)b, 0, 255)));
                }
            }


            // Recalcular y actualizar el histograma con la imagen filtrada

            return bmp;
        }

        // Función para asegurar que los valores estén en el rango permitido
        private int Clamp3(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        //==========================================EFECTO GRADIENTE ARCOIRIS========================================================================

        private Bitmap ApplyRainbowGradientEffect(Bitmap sourceBitmap)
        {
            // Crear una nueva imagen con el mismo tamaño que la original
            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            // Iterar sobre todos los píxeles de la imagen
            for (int x = 0; x < sourceBitmap.Width; x++)
            {
                for (int y = 0; y < sourceBitmap.Height; y++)
                {
                    // Obtener el color del píxel
                    Color pixelColor = sourceBitmap.GetPixel(x, y);

                    // Invertir el color (esto crea el efecto de inversión de colores)
                    int r = 255 - pixelColor.R;
                    int g = 255 - pixelColor.G;
                    int b = 255 - pixelColor.B;

                    // Establecer el píxel en la nueva imagen con el color invertido
                    resultBitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return resultBitmap;
        }













    }
}
