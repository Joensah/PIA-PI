using AForge.Math;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PIA_PI
{
    public partial class EditorImagenes : Form
    {
        public EditorImagenes()
        {
            InitializeComponent();
        }

        private void EditorImagenes_Load(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            panel2.Controls.Clear();
            panel3.Controls.Clear();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) //Cargar una imagen
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Todos los archivos|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string rutaArchivo = openFileDialog.FileName;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

                    Bitmap foto = new Bitmap(rutaArchivo);  // Convertimos directamente a Bitmap
                    pictureBox1.Image = foto;
                    pictureBox2.Image = foto;
                    panel1.CreateGraphics().Clear(panel1.BackColor);
                    panel2.CreateGraphics().Clear(panel2.BackColor);
                    panel3.CreateGraphics().Clear(panel3.BackColor);
                    Obtener_Colores(foto);  // Pasar el Bitmap a la función Obtener_Colores()
                }
                catch (ArgumentException)
                {
                    // Manejo de error si el archivo no es una imagen válida
                    MessageBox.Show("El archivo seleccionado no es una imagen válida. Por favor, seleccione una imagen compatible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    // Capturar cualquier otro error inesperado
                    MessageBox.Show("Ocurrió un error al cargar la imagen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

      


        private void Obtener_Colores(Bitmap bitmap)
        {
            int[] histogramR = new int[256];
            int[] histogramG = new int[256];
            int[] histogramB = new int[256];

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    histogramR[pixelColor.R]++;
                    histogramG[pixelColor.G]++;
                    histogramB[pixelColor.B]++;
                }
            }

            // Actualiza los tres paneles con los nuevos datos del histograma
            Cargar_histograma(panel1, histogramR, Color.Red);
            Cargar_histograma(panel2, histogramG, Color.Green);
            Cargar_histograma(panel3, histogramB, Color.Blue);
        }

        //Cargar el histograma
        private void Cargar_histograma(Panel panel, int[] histogram, Color color)
        {

            using (Graphics g = panel.CreateGraphics())
            {
                g.Clear(panel.BackColor);  // Limpiar el panel antes de dibujar el nuevo histograma

                using (Pen pen = new Pen(color))
                {
                    int maxValue = histogram.Max();  // Usamos Max para encontrar el valor máximo

                    float scaleFactor = maxValue > 0 ? (float)panel.Height / maxValue : 1;

                    for (int i = 0; i < histogram.Length; i++)
                    {
                        float x = i * (float)panel.Width / histogram.Length;
                        float y = panel.Height - histogram[i] * scaleFactor;

                        g.DrawLine(pen, x, panel.Height, x, y);
                    }
                }
            }
        }

        //Botón para quitar la imagen
        private void button2_Click(object sender, EventArgs e)
        {
            // Verificar si no hay imagen cargada en ninguno de los PictureBox
            if (pictureBox1.Image == null && pictureBox2.Image == null)
            {
                // Mostrar mensaje de error si no hay imágenes
                MessageBox.Show("No hay imagen cargada para quitar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Salir del método sin continuar con la eliminación
            }

            var quitarImagen = MessageBox.Show("¿Desea quitar la imagen?", "Aviso", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (quitarImagen == DialogResult.Yes)
            {
                // Limpiar y liberar la imagen de pictureBox1 si existe
                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Dispose(); // Liberar los recursos de la imagen
                    pictureBox1.Image = null;    // Eliminar la imagen del PictureBox
                }

                // Limpiar y liberar la imagen de pictureBox2 si existe
                if (pictureBox2.Image != null)
                {
                    pictureBox2.Image.Dispose(); // Liberar los recursos de la imagen
                    pictureBox2.Image = null;    // Eliminar la imagen del PictureBox
                }

                // Limpiar cualquier gráfico en los paneles
                panel1.CreateGraphics().Clear(panel1.BackColor);
                panel2.CreateGraphics().Clear(panel2.BackColor);
                panel3.CreateGraphics().Clear(panel3.BackColor);

                // Reiniciar cualquier otro control relacionado, como una barra de progreso, si es necesario
                progressBar1.Value = 0;
                //MessageBox.Show("La Imagen ha sido eliminada.", "Información", MessageBoxButtons.OK);

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        //Botón para aplicar filtros
        private void button1_Click_1(object sender, EventArgs e)
        {
            string filtro;

            if (comboBox1.SelectedIndex != -1 && pictureBox1.Image != null)
            {
                filtro = comboBox1.SelectedItem.ToString();

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
               ***10 Efecto Difuminado(Gaussiano)***ELIMINADO
               10 Efecto Gradiente Arcoiris
                */

                //Filtro 1 Efecto Solarizado
                if (filtro == "1-Efecto Solarizado")
                {
                    //pictureBox1.Image = ApplySolarizationEffect((Bitmap)pictureBox1.Image);
                    pictureBox2.Image = ApplySolarizationEffect((Bitmap)pictureBox1.Image);
                    Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

                }

                //Filtro 2 Filtro de Calor
                if (filtro == "2-Filtro de Calor")
                {
                    // Aplica el efecto de calor y muestra la imagen filtrada en pictureBox2
                    pictureBox2.Image = ApplyHeatEffect((Bitmap)pictureBox1.Image);
                    Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada
                }

                //Filtro 3 Efecto PopArt
                if (filtro == "3-Efecto PopArt")
                {
                    // Aplica el efecto de calor y muestra la imagen filtrada en pictureBox2
                    pictureBox2.Image = ApplyPopArtEffect((Bitmap)pictureBox1.Image);
                    Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada
                
                }

                //Filtro 4 Efecto Emboss
                if (filtro == "4-Efecto Emboss")
                {
                    // Aplica el efecto de calor y muestra la imagen filtrada en pictureBox2
                    pictureBox2.Image = ApplyEmbossEffect((Bitmap)pictureBox1.Image);
                    Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

                }

                //Filtro 5 Efecto Vignette
                if (filtro == "5-Efecto Vignette")
                {
                    // Aplica el efecto de calor y muestra la imagen filtrada en pictureBox2
                    pictureBox2.Image = ApplyVignetteEffect((Bitmap)pictureBox1.Image);
                    Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

                }

                //Filtro 6 Efecto Glitch
                if (filtro == "6-Efecto Glitch")
                {
                    // Aplica el efecto de calor y muestra la imagen filtrada en pictureBox2
                    pictureBox2.Image = ApplyGlitchEffect((Bitmap)pictureBox1.Image);
                    Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

                }

                //Filtro 7 Efecto Negativo
                if (filtro == "7-Efecto Negativo")
                {
                    // Aplica el efecto de calor y muestra la imagen filtrada en pictureBox2
                    pictureBox2.Image = ApplyNegativeEffect((Bitmap)pictureBox1.Image);
                    Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

                }

                //Filtro 8 Detección de Bordes
                if (filtro == "8-Detección de Bordes")
                {
                    // Aplica el efecto de calor y muestra la imagen filtrada en pictureBox2
                    pictureBox2.Image = ApplyEdgeDetectionEffect((Bitmap)pictureBox1.Image);
                    Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

                }

                //Filtro 9 Efecto Cómic
                if (filtro == "9-Efecto Cómic")
                {
                    pictureBox2.Image = ApplyComicEffect((Bitmap)pictureBox1.Image);
                    Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

                }
                /*
                //Filtro 10 Efecto Difuminado(Gaussiano) Eliminado
                if (filtro == "10-Efecto Difuminado(Gaussiano)")
                {
                    pictureBox2.Image = ApplyGaussianBlur((Bitmap)pictureBox1.Image);
                    Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

                }
                */

                //Filtro 10 Gradiente Arcoiris
                if (filtro == "10-Efecto Gradiente Arcoiris")
                {
                    pictureBox2.Image = ApplyRainbowGradientEffect((Bitmap)pictureBox1.Image);
                    Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

                }

            }
            else
            {
                MessageBox.Show("Seleccione una opción y revise si tiene una imagen cargada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Guardar las imagenes con filtro aplicado
        private void button4_Click(object sender, EventArgs e)
        {


            if (pictureBox2.Image != null)
            {

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Archivos de imagen|*.png;*.jpg;*.jpeg;*.bmp|Todos los archivos|*.*";
                saveFileDialog1.Title = "Guardar imagen";
                saveFileDialog1.ShowDialog();


                if (saveFileDialog1.FileName != "")
                {

                    string extension = System.IO.Path.GetExtension(saveFileDialog1.FileName).ToLower();


                    switch (extension)
                    {
                        case ".png":
                            pictureBox2.Image.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case ".jpg":
                        case ".jpeg":
                            pictureBox2.Image.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                        case ".bmp":
                            pictureBox2.Image.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                        default:

                            pictureBox2.Image.Save(saveFileDialog1.FileName + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("No hay una imagen con filtro aplicado para guardar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    progressBar1.Value = progressPercentage;
                    Application.DoEvents();
                }
            }
            progressBar1.Value = 0;
            Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

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

            progressBar1.Value = 0;
            Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

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
            progressBar1.Value = 0;
            Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

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
            progressBar1.Value = 0;
            Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

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
            progressBar1.Value = 0;
            Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

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

            progressBar1.Value = 0;
            Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

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

            progressBar1.Value = 0;
            Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

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

            progressBar1.Value = 0;
            Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

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

                    // Asegurarse de que el valor de progreso no exceda 100
                    if (progressPercentage <= 100)
                    {
                        progressBar1.Value = progressPercentage;
                    }

                    Application.DoEvents();
                }
            }

            progressBar1.Value = 0;
            Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

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

            progressBar1.Value = 0;
            Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

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
            Bitmap bmp = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            // Definir los colores del arcoíris (Rojo, Naranja, Amarillo, Verde, Azul, Índigo, Violeta)
            Color[] rainbowColors = new Color[]
            {
        Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet
            };

            int totalColors = rainbowColors.Length - 1;  // Último índice para interpolación

            for (int y = 0; y < sourceBitmap.Height; y++)
            {
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    // Obtener el color original del píxel
                    Color originalColor = sourceBitmap.GetPixel(x, y);

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
                    int newR = (originalColor.R + r) / 2;
                    int newG = (originalColor.G + g) / 2;
                    int newB = (originalColor.B + b) / 2;

                    // Aplicar el nuevo color al píxel
                    bmp.SetPixel(x, y, Color.FromArgb(newR, newG, newB));
                }
            }
            progressBar1.Value = 0;
            Obtener_Colores((Bitmap)pictureBox2.Image); // Recalcular y actualizar el histograma con la imagen filtrada

            return bmp;
        }

        //Cerrar ventana
        private void EditorImagenes_FormClosed(object sender, FormClosedEventArgs e)
        {
            pictureBox1.Image = null; // Quitar la imagen actual del PictureBox
            pictureBox2.Image = null; // Quitar la imagen actual del PictureBox


        }
    }
}