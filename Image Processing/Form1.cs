using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCamLib;
using HNUDIP;
using ImageProcess2;
using static System.Net.Mime.MediaTypeNames;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Image_Processing
{
    public partial class Form1 : Form
    {
        Bitmap load, processed, background;
        Device[] camera;
        Device selected;
        FilterInfoCollection FilterInfoCollection;
        VideoCaptureDevice VideoCaptureDevice;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            load = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = load;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            
        }

        private void basicCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(load.Width, load.Height);
            for (int x = 0; x < load.Width; x++)
            {
                for (int y = 0; y < load.Height; y++)
                {
                    Color pix = load.GetPixel(x, y);
                    processed.SetPixel(x, y, pix);
                }
            }
            pictureBox2.Image = processed;
        }

        private void greyscaleToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            processed = new Bitmap(load.Width, load.Height);
            for (int x = 0; x < load.Width; x++)
            {
                for (int y = 0; y < load.Height; y++)
                {
                    Color pix = load.GetPixel(x, y);
                    int grey = (pix.R + pix.G + pix.B) / 3;
                    processed.SetPixel(x, y, Color.FromArgb(grey, grey, grey));
                }
            }
            pictureBox2.Image = processed;
        }

        private void colorInversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(load.Width, load.Height);
            for (int x = 0; x < load.Width; x++)
            {
                for (int y = 0; y < load.Height; y++)
                {
                    Color pix = load.GetPixel(x, y);
                    processed.SetPixel(x, y, Color.FromArgb(255-pix.R, 255-pix.G, 255-pix.B));
                }
            }
            pictureBox2.Image = processed;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(load.Width, load.Height);
            for (int x = 0; x < load.Width; x++)
            {
                for (int y = 0; y < load.Height; y++)
                {
                    Color pix = load.GetPixel(x, y);
                    int grey = (pix.R + pix.G + pix.B) / 3;
                    processed.SetPixel(x, y, Color.FromArgb(grey, grey, grey));
                }
            }
            Color value;
            int[] histogram = new int[256];
            
            for (int x = 0; x < load.Width; x++)
            {
                for (int y = 0; y < load.Height; y++)
                {
                    value = processed.GetPixel(x, y);
                    histogram[value.R] ++;
                }
            }
            Bitmap data = new Bitmap(256, 600);
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 600; y++)
                {
                    data.SetPixel(x, y, Color.White);
                }
            }
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < Math.Min(histogram[x]/5, 600); y++)
                {
                    data.SetPixel(x, 599-y, Color.Black);
                }
            }
            pictureBox2.Image = data;
        }

        private void septToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(load.Width, load.Height);
            for (int x = 0; x < load.Width; x++)
            {
                for (int y = 0; y < load.Height; y++)
                {
                    Color pix = load.GetPixel(x, y);
                    int red, green, blue;
                    red = (int)(Math.Min(pix.R * .393 + pix.G * .769 + pix.B * .189, 255));
                    green = (int)(Math.Min(pix.R * .349 + pix.G * .686 + pix.B * .168, 255));
                    blue = (int)(Math.Min(pix.R * .272 + pix.G * .534 + pix.B * .131, 255));

                    processed.SetPixel(x, y, Color.FromArgb(red, green, blue));

                }
            }
            pictureBox2.Image = processed;
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            processed.Save(saveFileDialog1.FileName);
        }

        private void flipVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(load.Width, load.Height);
            for (int x = 0; x < load.Width; x++)
            {
                for (int y = 0; y < load.Height; y++)
                {
                    Color pix = load.GetPixel(x, y);
                    processed.SetPixel(load.Width - x - 1, y, pix);
                }
            }
            pictureBox2.Image = processed;
        }

        private void loadBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void subtractionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(load.Width, load.Height);
            Color imagegreen = load.GetPixel(0, 0);
            int greenscale = (imagegreen.R + imagegreen.G + imagegreen.B) / 3;
            int threshold = 5;

            for(int x = 0; x < load.Width; x++)
            {
                for(int y = 0; y < load.Height; y++)
                {
                    Color pixel = load.GetPixel(x, y);
                    Color backpixel = background.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractvalue = Math.Abs(grey - greenscale);
                    if(subtractvalue > threshold)
                    {
                        processed.SetPixel(x,y, pixel);
                    }
                    else
                    {
                        processed.SetPixel(x,y, backpixel);
                    }
                }
            }
            pictureBox2.Image = processed;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            background = new Bitmap(openFileDialog2.FileName);
            pictureBox3.Image = background;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            selected.Sendmessage();
            pictureBox2.Image = Clipboard.GetImage();
        }

     
        private void loadCameraToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            camera = DeviceManager.GetAllDevices();
            selected = camera[0];
            selected.ShowWindow(pictureBox1);

        }

        private void flipHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(load.Width, load.Height);
            for (int x = 0; x < load.Width; x++)
            {
                for (int y = 0; y < load.Height; y++)
                {
                    Color pix = load.GetPixel(x, y);
                    processed.SetPixel(x, load.Height - y - 1, pix);
                }
            }
            pictureBox2.Image = processed;
        }

        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
    }
}
