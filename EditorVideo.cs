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
using System.Threading;

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

        private Bitmap  processedImage; // Variable global para mantener la imagen procesada


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

                        // Crear una copia de la imagen para aplicar los efectos
                        Bitmap processedImage = currentFrame.Bitmap;
                        //elimianr bitmap si falla (esto es por la variable global que hice)

                        // Aplicar el filtro dependiendo de la opción seleccionada
                        switch (opcion_filtro)
                        {
                            case 1: // Efecto Solarizado
                                processedImage = ApplySolarizationEffect(processedImage);
                                break;
                            case 2: // Filtro de Calor
                                processedImage = ApplyHeatEffect(processedImage);
                                break;
                            case 3: // Efecto PopArt
                                processedImage = ApplyPopArtEffect(processedImage);
                                break;
                            case 4: // Efecto Emboss
                                processedImage = ApplyEmbossEffect(processedImage);
                                break;
                            case 5: // Efecto Vignette
                                processedImage = ApplyVignetteEffect(processedImage);
                                break;
                            case 6: // Efecto Glitch
                                processedImage = ApplyGlitchEffect(processedImage);
                                break;
                            case 7: // Efecto Negativo
                                processedImage = ApplyNegativeEffect(processedImage);
                                break;
                            case 8: // Detección de Bordes
                                processedImage = ApplyEdgeDetectionEffect(processedImage);
                                break;
                            case 9: // Efecto Cómic
                                processedImage = ApplyComicEffect(processedImage);
                                break;
                            case 10: // Efecto Gradiente Arcoiris
                                processedImage = ApplyRainbowGradientEffect(processedImage);
                                break;

                            default:
                                break;
                        }
                        // Actualizar el PictureBox con la imagen procesada
                        pictureBox1.Image = processedImage;
                    }
                    //Thread.Sleep(24);  // Son ms (1000 milisegundos = 1 segundo)

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
                try
                {

                    string rutaArchivo = openFileDialog.FileName;
                    Video = new VideoCapture(openFileDialog.FileName);
                    math = new Mat();


                    // Intentar leer el primer cuadro del video
                    Video.Read(math);
                    // Verificar si el primer cuadro está vacío, lo cual indicaría que el archivo no es válido
                    if (math.IsEmpty)
                    {
                        throw new ArgumentException("El archivo no es un video válido o está dañado.");
                    }


                    Video.Read(math);
                    currentFrame = math.ToImage<Bgr, byte>();
                  // pictureBox1.Image = null;
                    pictureBox1.Image = currentFrame.Bitmap;
                    opcion_filtro = 0; // Restablecer el filtro (si es necesario)
                    isVideo = true;
                    isplay = false;
                    duracion = Video.GetCaptureProperty(CapProp.FrameCount);
                    frameCount = Video.GetCaptureProperty(CapProp.PosFrames);
                }
                catch (ArgumentException)
                {
                    // Manejo de error si el archivo no es un video válido
                    MessageBox.Show("El archivo seleccionado no es un video válido. Por favor, seleccione un video compatible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    // Capturar cualquier otro error inesperado
                    MessageBox.Show("Ocurrió un error al cargar el video: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
        
            if (isVideo)
            {
                isplay = true;
                Application.Idle += new EventHandler(VideoFrameCapture);
            }
            else
            {
                MessageBox.Show("No hay un video cargado. No se puede realizar esta acción.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (isVideo)
            {
                isplay = false;
                //Application.Idle -= VideoFrameCapture; // Detener el evento de captura de fotogramas
            }
            else
            {
                // Mostrar advertencia si no hay video cargado.
                MessageBox.Show("No hay un video cargado. No se puede pausar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            // Verificar si hay una imagen o video cargado en el PictureBox
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No hay video para quitar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Si no hay imagen, no hacemos nada más
            }

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
            // Verificar si hay un video cargado
            if (Video == null || !isVideo)
            {
                MessageBox.Show("No hay un video cargado. No se puede realizar esta acción.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir del método si no hay video
            }

            isplay = false; // Detener la reproducción
            frameCount = 0; // Reiniciar el contador de fotogramas
            Video.SetCaptureProperty(CapProp.PosFrames, 0); // Volver al primer fotograma

            Application.Idle -= VideoFrameCapture; // Detener la captura de fotogramas

                                                   // Verificar si hay una imagen procesada para mostrar
            if (processedImage != null)
            {
                pictureBox1.Image = processedImage; // Mostrar la imagen procesada (con filtro)
            }
            else
            {
                // Si no hay imagen procesada, mostrar el fotograma inicial
                math = Video.QueryFrame();
                if (math != null)
                {
                    currentFrame = math.ToImage<Bgr, byte>(); // Capturar el fotograma actual
                    pictureBox1.Image = currentFrame.Bitmap; // Guardar el fotograma inicial procesado
                   // pictureBox1.Image = processedImage; // Mostrarlo
                }
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

                // Establecer el filtro seleccionado
                switch (filtro)
                {
                    case "1-Efecto Solarizado":
                        opcion_filtro = 1;
                        break;
                    case "2-Filtro de Calor":
                        opcion_filtro = 2;
                        break;
                    case "3-Efecto PopArt":
                        opcion_filtro = 3;
                        break;
                    case "4-Efecto Emboss":
                        opcion_filtro = 4;
                        break;
                    case "5-Efecto Vignette":
                        opcion_filtro = 5;
                        break;
                    case "6-Efecto Glitch":
                        opcion_filtro = 6;
                        break;
                    case "7-Efecto Negativo":
                        opcion_filtro = 7;
                        break;
                    case "8-Detección de Bordes":
                        opcion_filtro = 8;
                        break;
                    case "9-Efecto Cómic":
                        opcion_filtro = 9;
                        break;
                    case "10-Efecto Gradiente Arcoiris":
                        opcion_filtro = 10;
                        break;
                    default:
                        MessageBox.Show("Filtro no reconocido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                // Aplica el filtro y la lógica de reproducción
                isplay = true; // Marca que se debe reproducir el video
                Application.Idle += new EventHandler(VideoFrameCapture); // Inicia la captura de frames para reproducción

            }
            else
            
                {
                MessageBox.Show("Seleccione una opción y revise si tiene un video cargado", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
        private void EditorVideo_Load(object sender, EventArgs e)
        {

        }

        private void EditorVideo_FormClosing(object sender, FormClosingEventArgs e)
        {
            isplay = false;            // Detener la reproducción
            frameCount = 0;            // Reiniciar el contador de frames
            
            // Desvincular el evento Application.Idle para detener la captura de fotogramas
            Application.Idle -= VideoFrameCapture;

            if (Video != null && Video.IsOpened)
            {
                Video.SetCaptureProperty(CapProp.PosFrames, 0); // Volver al primer fotograma
            }

            duracion = 0;   // Reiniciar la duración del video
            pictureBox1.Image = null; // Quitar la imagen actual del PictureBox
            frameCount = 0; // Reiniciar el contador de frames
            isVideo = false; // Indicar que ya no hay un video cargado

            // Liberar el objeto VideoCapture
            if (Video != null)
            {
                Video.Dispose();
                Video = null;
            }

            // Liberar el currentFrame si ya no lo necesitas
            if (currentFrame != null)
            {
                currentFrame.Dispose();
                currentFrame = null;
            }
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

        //==========================================EFECTO DE SOLARIZADO(lock y unlock bits)========================================================================
        private Bitmap ApplySolarizationEffect(Bitmap sourceBitmap)
        {
            // Crear una copia de la imagen para trabajar en ella sin modificar la original
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format24bppRgb);

            // Bloquear los bits de la imagen fuente y destino
            BitmapData srcData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData destData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int bytesPerPixel = 3; // Para 24bpp
            int stride = srcData.Stride;
            IntPtr srcPtr = srcData.Scan0;
            IntPtr destPtr = destData.Scan0;

            byte[] srcBuffer = new byte[stride * sourceBitmap.Height];
            byte[] destBuffer = new byte[stride * bmp.Height];

            // Copiar datos de la imagen fuente al búfer de bytes
            Marshal.Copy(srcPtr, srcBuffer, 0, srcBuffer.Length);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    int index = (y * stride) + (x * bytesPerPixel);

                    // Obtener los valores de los canales BGR
                    byte b = srcBuffer[index];
                    byte g = srcBuffer[index + 1];
                    byte r = srcBuffer[index + 2];

                    // Calcular la luminosidad promedio
                    int avg = (r + g + b) / 3;

                    if (avg < 128)
                    {
                        // Invertir los colores si la luminosidad es baja
                        destBuffer[index] = (byte)(255 - b);
                        destBuffer[index + 1] = (byte)(255 - g);
                        destBuffer[index + 2] = (byte)(255 - r);
                    }
                    else
                    {
                        // Mantener el color original
                        destBuffer[index] = b;
                        destBuffer[index + 1] = g;
                        destBuffer[index + 2] = r;
                    }
                }
            }

            // Copiar el búfer modificado de vuelta a la imagen destino
            Marshal.Copy(destBuffer, 0, destPtr, destBuffer.Length);

            // Desbloquear los bits
            sourceBitmap.UnlockBits(srcData);
            bmp.UnlockBits(destData);

            return bmp;
        }

        //==========================================FILTRO DE CALOR(lock y unlick bits)======================================================================

        private Bitmap ApplyHeatEffect(Bitmap sourceBitmap)
        {
            // Crear una nueva imagen para el resultado
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format24bppRgb);

            // Bloquear los bits de la imagen fuente y de destino
            BitmapData srcData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData destData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int bytesPerPixel = 3; // Para imágenes en 24bpp
            int stride = srcData.Stride;
            IntPtr srcPtr = srcData.Scan0;
            IntPtr destPtr = destData.Scan0;

            byte[] srcBuffer = new byte[stride * sourceBitmap.Height];
            byte[] destBuffer = new byte[stride * bmp.Height];

            // Copiar los datos de la imagen fuente al búfer de bytes
            Marshal.Copy(srcPtr, srcBuffer, 0, srcBuffer.Length);

            // Procesar cada píxel
            for (int y = 0; y < sourceBitmap.Height; y++)
            {
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    int index = (y * stride) + (x * bytesPerPixel);

                    byte b = srcBuffer[index];
                    byte g = srcBuffer[index + 1];
                    byte r = srcBuffer[index + 2];

                    // Convertir el color a una intensidad de gris (promedio de R, G, B)
                    int intensity = (r + g + b) / 3;

                    // Mapeo de intensidad a una paleta de colores (heatmap)
                    Color heatmapColor;
                    if (intensity < 64) // frío
                        heatmapColor = Color.FromArgb(0, 0, (byte)(255 - (intensity * 4))); // azul a azul oscuro
                    else if (intensity < 128) // intermedio frío
                        heatmapColor = Color.FromArgb(0, (byte)((intensity - 64) * 4), (byte)255); // verde a azul
                    else if (intensity < 192) // intermedio cálido
                        heatmapColor = Color.FromArgb((byte)((intensity - 128) * 4), (byte)255, (byte)(255 - (intensity - 128) * 4)); // amarillo a verde
                    else // cálido
                        heatmapColor = Color.FromArgb(255, (byte)(255 - (intensity - 192) * 4), 0); // rojo a amarillo

                    // Asignar el color al búfer de destino
                    destBuffer[index] = heatmapColor.B;
                    destBuffer[index + 1] = heatmapColor.G;
                    destBuffer[index + 2] = heatmapColor.R;
                }
            }

            // Copiar el búfer modificado de vuelta a la imagen destino
            Marshal.Copy(destBuffer, 0, destPtr, destBuffer.Length);

            // Desbloquear los bits
            sourceBitmap.UnlockBits(srcData);
            bmp.UnlockBits(destData);

            return bmp; // Devolver la imagen con el filtro aplicado
        }

        //==========================================EFECTO POPART(lock y unlock bits)========================================================================

        private Bitmap ApplyPopArtEffect(Bitmap sourceBitmap)
        {
            // Bloquear los bits de la imagen para acceso rápido
            Rectangle rect = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData bmpData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, sourceBitmap.PixelFormat);

            // Bloquear los bits de la imagen de salida para modificarla
            BitmapData bmpDataOutput = bmp.LockBits(rect, ImageLockMode.WriteOnly, sourceBitmap.PixelFormat);

            // Definir una paleta de colores vibrantes
            Color[] vibrantColors = {
        Color.FromArgb(255, 0, 0),    // Rojo brillante
        Color.FromArgb(0, 255, 0),    // Verde brillante
        Color.FromArgb(0, 0, 255),    // Azul brillante
        Color.FromArgb(255, 255, 0),  // Amarillo brillante
        Color.FromArgb(255, 165, 0),  // Naranja brillante
        Color.FromArgb(128, 0, 128)   // Púrpura
    };

            // Punteros para recorrer los píxeles de la imagen bloqueada
            IntPtr ptr = bmpData.Scan0;
            IntPtr ptrOutput = bmpDataOutput.Scan0;

            // Obtener el tamaño de los píxeles
            int bytesPerPixel = Bitmap.GetPixelFormatSize(sourceBitmap.PixelFormat) / 8;
            int stride = bmpData.Stride;  // El paso de bytes por fila
            int width = sourceBitmap.Width;
            int height = sourceBitmap.Height;

            // Recorrer cada píxel en la imagen de entrada
            unsafe
            {
                byte* pixelPointer = (byte*)ptr;
                byte* outputPointer = (byte*)ptrOutput;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Calcular la posición del píxel
                        int pixelIndex = y * stride + x * bytesPerPixel;

                        // Obtener los valores de los colores
                        byte b = pixelPointer[pixelIndex];    // Azul
                        byte g = pixelPointer[pixelIndex + 1]; // Verde
                        byte r = pixelPointer[pixelIndex + 2]; // Rojo

                        // Posterizar el color
                        r = (byte)((r / 128) * 128); // Redondea a múltiplos de 128
                        g = (byte)((g / 128) * 128); // Redondea a múltiplos de 128
                        b = (byte)((b / 128) * 128); // Redondea a múltiplos de 128

                        // Encontrar el color más cercano en la paleta
                        Color closestColor = vibrantColors.OrderBy(c =>
                            Math.Abs(c.R - r) + Math.Abs(c.G - g) + Math.Abs(c.B - b)).First();

                        // Asignar el color a la imagen de salida
                        outputPointer[pixelIndex] = closestColor.B;    // Azul
                        outputPointer[pixelIndex + 1] = closestColor.G; // Verde
                        outputPointer[pixelIndex + 2] = closestColor.R; // Rojo
                    }
                }
            }

            // Desbloquear los bits de las imágenes
            sourceBitmap.UnlockBits(bmpData);
            bmp.UnlockBits(bmpDataOutput);

            return bmp;
        }

        //==========================================EFECTO EMBOSS(lock y unlock bits)========================================================================

        private Bitmap ApplyEmbossEffect(Bitmap sourceBitmap)
        {
            // Crear una nueva imagen con las mismas dimensiones que la fuente
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, sourceBitmap.PixelFormat);

            // Bloquear los bits de la imagen original para acceso rápido
            Rectangle rect = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData srcData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);
            BitmapData destData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(sourceBitmap.PixelFormat) / 8;
            int stride = srcData.Stride;
            IntPtr srcPtr = srcData.Scan0;
            IntPtr destPtr = destData.Scan0;

            // Definir el kernel para el efecto emboss
            int[,] embossKernel = new int[,]
            {
        { -2, -1, 0 },
        { -1,  1, 1 },
        {  0,  1, 2 }
            };

            // Realizar la operación de emboss en los píxeles
            unsafe
            {
                byte* src = (byte*)srcPtr.ToPointer();
                byte* dest = (byte*)destPtr.ToPointer();

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
                                int offsetX = (x + kx) * bytesPerPixel;
                                int offsetY = (y + ky) * stride;

                                int pixelIndex = offsetY + offsetX;

                                // Obtener los valores de R, G, B de los píxeles vecinos
                                byte red = src[pixelIndex + 2]; // R está en el índice 2
                                byte green = src[pixelIndex + 1]; // G está en el índice 1
                                byte blue = src[pixelIndex]; // B está en el índice 0

                                r += red * embossKernel[ky + 1, kx + 1];
                                g += green * embossKernel[ky + 1, kx + 1];
                                b += blue * embossKernel[ky + 1, kx + 1];
                            }
                        }

                        // Clampeo de los valores RGB
                        r = Math.Min(Math.Max(r + 128, 0), 255); // Sumar 128 para evitar negativos
                        g = Math.Min(Math.Max(g + 128, 0), 255);
                        b = Math.Min(Math.Max(b + 128, 0), 255);

                        // Escribir el nuevo valor de los píxeles en la imagen de destino
                        int destIndex = (y * stride) + (x * bytesPerPixel);
                        dest[destIndex] = (byte)b;
                        dest[destIndex + 1] = (byte)g;
                        dest[destIndex + 2] = (byte)r;
                    }
                }
            }

            // Liberar los bits de la imagen
            sourceBitmap.UnlockBits(srcData);
            bmp.UnlockBits(destData);

            return bmp;
        }

        //==========================================EFECTO VIGNETTE(lock y unlock bits)======================================================================

        private Bitmap ApplyVignetteEffect(Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, sourceBitmap.PixelFormat);

            // Bloquear los bits de la imagen fuente y la de destino
            Rectangle rect = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData srcData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);
            BitmapData destData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(sourceBitmap.PixelFormat) / 8;
            int stride = srcData.Stride;
            IntPtr srcPtr = srcData.Scan0;
            IntPtr destPtr = destData.Scan0;

            // Calcular el centro de la imagen
            int centerX = sourceBitmap.Width / 2;
            int centerY = sourceBitmap.Height / 2;

            // Incrementar el radio máximo para suavizar el viñeteado
            double maxRadius = Math.Sqrt(centerX * centerX + centerY * centerY) * 1.2;

            unsafe
            {
                byte* src = (byte*)srcPtr.ToPointer();
                byte* dest = (byte*)destPtr.ToPointer();

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
                        vignetteFactor = Math.Pow(vignetteFactor, 2.5); // Ajuste para suavizar la caída

                        // Calcular el índice de píxel en la memoria
                        int offsetX = x * bytesPerPixel;
                        int offsetY = y * stride;
                        int pixelIndex = offsetY + offsetX;

                        // Obtener los valores RGB del píxel original
                        byte blue = src[pixelIndex];
                        byte green = src[pixelIndex + 1];
                        byte red = src[pixelIndex + 2];

                        // Aplicar el efecto de viñeteado
                        int r = (int)(red * vignetteFactor);
                        int g = (int)(green * vignetteFactor);
                        int b = (int)(blue * vignetteFactor);

                        // Clampeo de los valores RGB
                        r = ClampInt(r, 0, 255);
                        g = ClampInt(g, 0, 255);
                        b = ClampInt(b, 0, 255);

                        // Escribir el nuevo valor del píxel en la imagen de destino
                        int destIndex = (y * stride) + (x * bytesPerPixel);
                        dest[destIndex] = (byte)b;
                        dest[destIndex + 1] = (byte)g;
                        dest[destIndex + 2] = (byte)r;
                    }
                }
            }

            // Liberar los bits de la imagen
            sourceBitmap.UnlockBits(srcData);
            bmp.UnlockBits(destData);

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

       //==========================================EFECTO GLITCH(lock y unlock)========================================================================

       private Bitmap ApplyGlitchEffect(Bitmap sourceBitmap)
       {
            Random random = new Random();
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, sourceBitmap.PixelFormat);

            // Bloquear los bits de la imagen fuente y la de destino
            Rectangle rect = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData srcData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);
            BitmapData destData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(sourceBitmap.PixelFormat) / 8;
            int stride = srcData.Stride;
            IntPtr srcPtr = srcData.Scan0;
            IntPtr destPtr = destData.Scan0;

            unsafe
            {
                byte* src = (byte*)srcPtr.ToPointer();
                byte* dest = (byte*)destPtr.ToPointer();

                for (int y = 0; y < sourceBitmap.Height; y++)
                {
                    for (int x = 0; x < sourceBitmap.Width; x++)
                    {
                        // Calcular el índice del píxel en la memoria
                        int offsetX = x * bytesPerPixel;
                        int offsetY = y * stride;
                        int pixelIndex = offsetY + offsetX;

                        // Obtener el color original del píxel
                        byte blue = src[pixelIndex];
                        byte green = src[pixelIndex + 1];
                        byte red = src[pixelIndex + 2];

                        // Desplazamiento aleatorio en el eje X y Y
                        int maxOffset = 10;
                        int offsetGlitchX = random.Next(-maxOffset, maxOffset + 1);
                        int offsetGlitchY = random.Next(-maxOffset / 2, maxOffset / 2 + 1);

                        // Asegurarse de que el nuevo valor de X e Y estén dentro de los límites
                        int newX = Clamp4(x + offsetGlitchX, 0, sourceBitmap.Width - 1);
                        int newY = Clamp4(y + offsetGlitchY, 0, sourceBitmap.Height - 1);

                        // Calcular el nuevo índice de píxel desplazado
                        int newOffsetX = newX * bytesPerPixel;
                        int newOffsetY = newY * stride;
                        int newPixelIndex = newOffsetY + newOffsetX;

                        // Establecer el color del píxel en la nueva posición
                        dest[newPixelIndex] = blue;
                        dest[newPixelIndex + 1] = green;
                        dest[newPixelIndex + 2] = red;

                        // Alteraciones de color aleatorias
                        if (random.Next(100) < 40) // 40% de probabilidad de distorsionar el color
                        {
                            // Desplazar los canales de color aleatoriamente
                            int rOffset = random.Next(-50, 50);
                            int gOffset = random.Next(-50, 50);
                            int bOffset = random.Next(-50, 50);

                            // Ajustar el color para distorsionar
                            int r = Clamp4(red + rOffset, 0, 255);
                            int g = Clamp4(green + gOffset, 0, 255);
                            int b = Clamp4(blue + bOffset, 0, 255);

                            // Establecer el nuevo color distorsionado en la posición desplazada
                            dest[newPixelIndex] = (byte)b;
                            dest[newPixelIndex + 1] = (byte)g;
                            dest[newPixelIndex + 2] = (byte)r;
                        }
                    }
                }
            }

            // Liberar los bits de la imagen
            sourceBitmap.UnlockBits(srcData);
            bmp.UnlockBits(destData);

            return bmp;
        }

        // Función para asegurar que los valores estén en el rango permitido
        private int Clamp4(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        //==========================================EFECTO NEGATIVO(lock y unlock)======================================================================

        private Bitmap ApplyNegativeEffect(Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, sourceBitmap.PixelFormat);

            // Bloquear los bits de la imagen fuente y la de destino
            Rectangle rect = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData srcData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);
            BitmapData destData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(sourceBitmap.PixelFormat) / 8;
            int stride = srcData.Stride;
            IntPtr srcPtr = srcData.Scan0;
            IntPtr destPtr = destData.Scan0;

            unsafe
            {
                byte* src = (byte*)srcPtr.ToPointer();
                byte* dest = (byte*)destPtr.ToPointer();

                for (int y = 0; y < sourceBitmap.Height; y++)
                {
                    for (int x = 0; x < sourceBitmap.Width; x++)
                    {
                        // Calcular el índice del píxel en la memoria
                        int offsetX = x * bytesPerPixel;
                        int offsetY = y * stride;
                        int pixelIndex = offsetY + offsetX;

                        // Obtener el color original del píxel
                        byte blue = src[pixelIndex];
                        byte green = src[pixelIndex + 1];
                        byte red = src[pixelIndex + 2];

                        // Invertir los componentes de color
                        byte invertedRed = (byte)(255 - red);
                        byte invertedGreen = (byte)(255 - green);
                        byte invertedBlue = (byte)(255 - blue);

                        // Establecer el nuevo color invertido en el píxel
                        dest[pixelIndex] = invertedBlue;
                        dest[pixelIndex + 1] = invertedGreen;
                        dest[pixelIndex + 2] = invertedRed;
                    }
                }
            }

            // Liberar los bits de la imagen
            sourceBitmap.UnlockBits(srcData);
            bmp.UnlockBits(destData);

            return bmp;
        }


        //==========================================DETECCIÓN DE BORDES(lock y unlock)==================================================================

        private Bitmap ApplyEdgeDetectionEffect(Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, sourceBitmap.PixelFormat);

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

            // Bloquear los bits de la imagen fuente y la de destino
            Rectangle rect = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData srcData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);
            BitmapData destData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(sourceBitmap.PixelFormat) / 8;
            int stride = srcData.Stride;
            IntPtr srcPtr = srcData.Scan0;
            IntPtr destPtr = destData.Scan0;

            unsafe
            {
                byte* src = (byte*)srcPtr.ToPointer();
                byte* dest = (byte*)destPtr.ToPointer();

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
                                // Calcular el índice del píxel en la memoria
                                int offsetX = (x + kx) * bytesPerPixel;
                                int offsetY = (y + ky) * stride;
                                int pixelIndex = offsetY + offsetX;

                                // Obtener el color del píxel y convertirlo a escala de grises
                                byte blue = src[pixelIndex];
                                byte green = src[pixelIndex + 1];
                                byte red = src[pixelIndex + 2];
                                int gray = (red + green + blue) / 3;

                                pixelX += gray * gx[ky + 1, kx + 1];
                                pixelY += gray * gy[ky + 1, kx + 1];
                            }
                        }

                        // Calcular la magnitud del gradiente
                        int magnitude = (int)Math.Sqrt(pixelX * pixelX + pixelY * pixelY);
                        magnitude = Clamp2(magnitude, 0, 255); // Clampear el valor

                        // Establecer el nuevo color en el píxel (escala de grises)
                        int offset = y * stride + x * bytesPerPixel;
                        dest[offset] = (byte)magnitude;
                        dest[offset + 1] = (byte)magnitude;
                        dest[offset + 2] = (byte)magnitude;
                    }
                }
            }

            // Liberar los bits de la imagen
            sourceBitmap.UnlockBits(srcData);
            bmp.UnlockBits(destData);

            return bmp;
        }

        // Función para asegurar que los valores estén en el rango permitido
        private int Clamp2(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        //==========================================EFECTO DE COMIC(lock y unlock)======================================================================
        private Bitmap ApplyComicEffect(Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, sourceBitmap.PixelFormat);

            // Bloquear los bits de la imagen fuente y la de destino
            Rectangle rect = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData srcData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);
            BitmapData destData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(sourceBitmap.PixelFormat) / 8;
            int stride = srcData.Stride;
            IntPtr srcPtr = srcData.Scan0;
            IntPtr destPtr = destData.Scan0;

            int totalPixels = sourceBitmap.Width * sourceBitmap.Height;
            int currentPixel = 0;

            unsafe
            {
                byte* src = (byte*)srcPtr.ToPointer();
                byte* dest = (byte*)destPtr.ToPointer();

                for (int y = 0; y < sourceBitmap.Height; y++)
                {
                    for (int x = 0; x < sourceBitmap.Width; x++)
                    {
                        int offset = y * stride + x * bytesPerPixel;

                        byte blue = src[offset];
                        byte green = src[offset + 1];
                        byte red = src[offset + 2];

                        // Calcular el valor promedio (escala de grises)
                        int avg = (red + green + blue) / 3;

                        // Si el valor promedio es mayor que 127, dejamos el color original, de lo contrario, lo convertimos a negro
                        if (avg > 127)
                        {
                            dest[offset] = blue;
                            dest[offset + 1] = green;
                            dest[offset + 2] = red;
                        }
                        else
                        {
                            dest[offset] = 0;      // Negro
                            dest[offset + 1] = 0;  // Negro
                            dest[offset + 2] = 0;  // Negro
                        }

                        // Actualizar el progreso
                        currentPixel++;
                        int progressPercentage = currentPixel * 100 / totalPixels;

                        // Llamada a Application.DoEvents() para permitir la actualización de la UI (si es necesario)
                        Application.DoEvents();
                    }
                }
            }

            // Liberar los bits de las imágenes
            sourceBitmap.UnlockBits(srcData);
            bmp.UnlockBits(destData);

            return bmp;
        }

        //==========================================EFECTO DIFUMINADO(GAUSSIANO)(set y getpixel) (Lo quité)=======================================================================

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

        //==========================================EFECTO GRADIENTE ARCOIRIS(lock y unlock bits)========================================================================

        private Bitmap ApplyRainbowGradientEffect(Bitmap sourceBitmap)
        {
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, sourceBitmap.PixelFormat);

            // Bloquear los bits de la imagen fuente y la de destino
            Rectangle rect = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData srcData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);
            BitmapData destData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(sourceBitmap.PixelFormat) / 8;
            int stride = srcData.Stride;
            IntPtr srcPtr = srcData.Scan0;
            IntPtr destPtr = destData.Scan0;

            // Definir los colores del arcoíris (Rojo, Naranja, Amarillo, Verde, Azul, Índigo, Violeta)
            Color[] rainbowColors = new Color[]
            {
        Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet
            };

            int totalColors = rainbowColors.Length - 1;  // Último índice para interpolación
            int totalPixels = sourceBitmap.Width * sourceBitmap.Height;
            int currentPixel = 0;

            unsafe
            {
                byte* src = (byte*)srcPtr.ToPointer();
                byte* dest = (byte*)destPtr.ToPointer();

                for (int y = 0; y < sourceBitmap.Height; y++)
                {
                    for (int x = 0; x < sourceBitmap.Width; x++)
                    {
                        int offset = y * stride + x * bytesPerPixel;

                        // Obtener el color original del píxel
                        byte blue = src[offset];
                        byte green = src[offset + 1];
                        byte red = src[offset + 2];

                        // Calcular la posición relativa en el eje Y
                        float ratio = (float)y / sourceBitmap.Height;

                        // Encontrar los dos colores del arcoíris entre los que se interpolará
                        int startColorIndex = (int)(ratio * totalColors); // Color inicial
                        int endColorIndex = startColorIndex + 1;          // Color final

                        // Asegurar que el índice final no exceda el límite
                        if (endColorIndex >= rainbowColors.Length)
                            endColorIndex = rainbowColors.Length - 1;

                        Color startColor = rainbowColors[startColorIndex];
                        Color endColor = rainbowColors[endColorIndex];

                        // Calcular la proporción para la interpolación
                        float localRatio = (ratio * totalColors) - startColorIndex;

                        // Interpolar los componentes R, G, B entre los colores inicial y final
                        int r = (int)(startColor.R * (1 - localRatio) + endColor.R * localRatio);
                        int g = (int)(startColor.G * (1 - localRatio) + endColor.G * localRatio);
                        int b = (int)(startColor.B * (1 - localRatio) + endColor.B * localRatio);

                        // Mezclar el color interpolado del arcoíris con el color original
                        int newR = (red + r) / 2;
                        int newG = (green + g) / 2;
                        int newB = (blue + b) / 2;

                        // Aplicar el nuevo color al píxel
                        dest[offset] = (byte)newB;
                        dest[offset + 1] = (byte)newG;
                        dest[offset + 2] = (byte)newR;

                        // Actualizar el progreso
                        currentPixel++;
                        int progressPercentage = currentPixel * 100 / totalPixels;

                        // Llamada a Application.DoEvents() para permitir la actualización de la UI (si es necesario)
                        Application.DoEvents();
                    }
                }
            }

            // Liberar los bits de las imágenes
            sourceBitmap.UnlockBits(srcData);
            bmp.UnlockBits(destData);

            return bmp;
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }
    }
}
