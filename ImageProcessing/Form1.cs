using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCamLib;

namespace ImageProcessing
{
    public partial class Form1 : Form
    {
        Bitmap loadPicture, result;
        Bitmap imageB, imageA, subtractResult;
        Device[] devices;
        PictureBox pictureBox3;
        System.Windows.Forms.Label label3;
        bool state;

        private Device selectedCamera;
        public Form1()
        {
            InitializeComponent();
            this.CenterToScreen();
            trackBar.Hide();
            radioButtonBrightness.Hide();
            radioButtonContrast.Hide();
            devices = DeviceManager.GetAllDevices();
            state = false;

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, y);
                    result.SetPixel(x, y, pixel);
                    pictureBox2.Image = result;
                }
            }
        }

        private void greyscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, y);
                    int gray = ((pixel.R + pixel.G + pixel.B) / 3);
                    pixel = Color.FromArgb(gray, gray, gray);
                    result.SetPixel(x, y, pixel);
                    pictureBox2.Image = result;
                }
            }
        }

        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, loadPicture.Height - y - 1);
                    result.SetPixel(x, y, pixel);
                    pictureBox2.Image = result;
                }
            }
        }

        private void mirrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // mirror flip X
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(loadPicture.Width - x - 1, y);
                    result.SetPixel(x, y, pixel);
                    pictureBox2.Image = result;
                }
            }
        }

        private void colorInversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, y);
                    int fixedByte = 255;
                    pixel = Color.FromArgb(fixedByte - pixel.R, fixedByte - pixel.G, fixedByte - pixel.B);
                    result.SetPixel(x, y, pixel);
                    pictureBox2.Image = result;
                }
            }
        }


        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check if subtract
            if(state)
            {
                if (saveFileDialog1 != null && result != null) // Check if they are not null.
                {
                    saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName) + ".jpg";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            subtractResult.Save(saveFileDialog1.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error saving the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    // Handle the case where the objects are null (e.g., display an error message or create new instances).
                }
            }
            else
            {
                if (saveFileDialog1 != null && result != null) // Check if they are not null.
                {
                    saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName) + ".jpg";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            result.Save(saveFileDialog1.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error saving the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    // Handle the case where the objects are null (e.g., display an error message or create new instances).
                }
            }
            
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            result = new Bitmap(loadPicture.Width, loadPicture.Height);

            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    Color pixel = loadPicture.GetPixel(x, y);

                    if (x < loadPicture.Width / 2 && y < loadPicture.Height / 2)
                    {
                        pixel = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                    }
                    else if (x >= loadPicture.Width / 2 && y < loadPicture.Height / 2)
                    {
                        int gray = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                        pixel = Color.FromArgb(gray, gray, gray);
                    }
                    else if (x < loadPicture.Width / 2 && y >= loadPicture.Height / 2)
                    {
                        result.SetPixel(x, y, pixel);
                    }
                    else if (x >= loadPicture.Width / 2 && y >= loadPicture.Height / 2)
                    {
                        int targetY = loadPicture.Height - y - 1;
                        pixel = loadPicture.GetPixel(x, targetY);
                    }

                    result.SetPixel(x, y, pixel);
                }
            }

            pictureBox2.Image = result;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, y);
                    int gray = ((pixel.R + pixel.G + pixel.B) / 3);
                    pixel = Color.FromArgb(gray, gray, gray);
                    result.SetPixel(x, y, pixel);
                }
            }
            Color sample;

            int[] histogramData = new int[256];

            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    sample = result.GetPixel(x, y);
                    histogramData[sample.R]++;
                }
            }

            Bitmap mydata = new Bitmap(256,800);

            for (int x = 0; x < mydata.Width; x++)
            {
                for (int y = 0; y < mydata.Height; y++)
                {
                    mydata.SetPixel(x, y, Color.White);
                }
            }

            //plot histogramData
            for (int x = 0; x < mydata.Width; x++)
            {
                for (int y = 0; y < Math.Min(histogramData[x]/5, mydata.Height); y++)
                {
                    mydata.SetPixel(x, (mydata.Height - 1) - y, Color.Black);
                }
            }

            pictureBox2.Image = mydata;
        }

        private void brightnessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            radioButtonBrightness.Show();
            radioButtonContrast.Show();
            trackBar.Show();
        }

        public void callBright(int val)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, y);
                    if(val >= 0 )
                    {
                        pixel = Color.FromArgb(Math.Min(pixel.R + val, 255), Math.Min(pixel.G + val, 255), Math.Min(pixel.B + val, 255));
                    }
                    else
                    {
                        pixel = Color.FromArgb(Math.Max(pixel.R + val, 0), Math.Max(pixel.G + val, 0), Math.Max(pixel.B + val, 0));
                    }
                    result.SetPixel(x, y, pixel);
                }
            }

            pictureBox2.Image = result;
        }

        public void callContrast(int val)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            float factor = (val + 100) / 100.0f;

            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, y);

                    int newR = (int)(pixel.R * factor);
                    int newG = (int)(pixel.G * factor);
                    int newB = (int)(pixel.B * factor);

                    newR = Math.Max(0, Math.Min(255, newR));
                    newG = Math.Max(0, Math.Min(255, newG));
                    newB = Math.Max(0, Math.Min(255, newB));

                    pixel = Color.FromArgb(newR, newG, newB);
                    result.SetPixel(x, y, pixel);
                }
            }

            pictureBox2.Image = result;
        }


        private void trackBar_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (radioButtonBrightness.Checked)
            {
                radioButtonBrightness.Font = new Font(radioButtonBrightness.Font, FontStyle.Bold);
                callBright(trackBar.Value);
                Console.WriteLine(trackBar.Value);
            }
            else if (radioButtonContrast.Checked)
            {
                radioButtonContrast.Font = new Font(radioButtonContrast.Font, FontStyle.Bold);
                callContrast(trackBar.Value);
            }
        }

        private void radioButtonBrightness_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonBrightness.Checked)
            {
                radioButtonBrightness.Font = new Font(radioButtonBrightness.Font, FontStyle.Bold);
                radioButtonContrast.Font = new Font(radioButtonContrast.Font, FontStyle.Regular);
            }
            else
            {
                radioButtonBrightness.Font = new Font(radioButtonBrightness.Font, FontStyle.Regular);
            }
        }

        private void radioButtonContrast_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonContrast.Checked)
            {
                radioButtonContrast.Font = new Font(radioButtonContrast.Font, FontStyle.Bold);
                radioButtonBrightness.Font = new Font(radioButtonBrightness.Font, FontStyle.Regular);
            }
            else
            {
                radioButtonContrast.Font = new Font(radioButtonContrast.Font, FontStyle.Regular);
            }
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, y);

                    int newRed = (int)(pixel.R * 0.393 + pixel.G * 0.769 + pixel.B * 0.189);
                    int newGreen = (int)(pixel.R * 0.349 + pixel.G * 0.686 + pixel.B * 0.168);
                    int newBlue = (int)(pixel.R * 0.272 + pixel.G * 0.534 + pixel.B * 0.131);

                    // Clamp values to be in the valid color range (0-255)
                    newRed = Math.Min(255, Math.Max(0, newRed));
                    newGreen = Math.Min(255, Math.Max(0, newGreen));
                    newBlue = Math.Min(255, Math.Max(0, newBlue));
                    pixel = Color.FromArgb(newRed, newGreen, newBlue);
                    result.SetPixel(x, y, pixel);
                    
                }
            }
            pictureBox2.Image = result;
        }

        private void onToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            IDataObject data;
            Image bmap;
            Device d = DeviceManager.GetDevice(devices.Length - 1);
            d.Sendmessage();
            data = Clipboard.GetDataObject();
            bmap = (Image)(data.GetData("System.Drawing.Bitmap", true));
            Bitmap b = new Bitmap(bmap);
            Color pixel;
            int greyvalue;
            result = new Bitmap(b.Width, b.Height);
            for (int x = 0; x < b.Width; x++)
            {
                for (int y = 0; y < b.Height; y++)
                {
                    pixel = b.GetPixel(x, y);
                    greyvalue = (byte)((pixel.R + pixel.G + pixel.B) / 3);
                    result.SetPixel(x, y, Color.FromArgb(greyvalue, greyvalue, greyvalue));
                }
            }
            pictureBox2.Image = result;
        }


        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void subtractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //pictureBox1 = Picture B, pictureBox2 = Picture A | B - A = Result
            SubtractOpenOnLoad();
            if(imageB.Width != imageA.Width || imageB.Height != imageA.Height)
            {
                MessageBox.Show("Error: Image dimensions do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
                pictureBox2.Image.Dispose();
                pictureBox2.Image = null;
                if(pictureBox3 != null)
                {
                    pictureBox3.Image.Dispose();
                    pictureBox3.Image = null;
                }
                if(imageB != null)
                {
                    imageB.Dispose();
                }

                if (imageA != null)
                {
                    imageA.Dispose();
                }

                if (subtractResult != null)
                {
                    subtractResult.Dispose();
                }
                state = false;
            }
            else
            {
                state = true;
                ResizeAndAddElementsInSubtract();
                Color mygreen = Color.FromArgb(0, 0, 255);
                int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;
                int threshold = 5;

                if (subtractResult != null)
                {
                    subtractResult.Dispose();
                }

                state = false;

                subtractResult = new Bitmap(imageB.Width, imageB.Height);

                for (int x = 0; x < imageB.Width; x++)
                {
                    for (int y = 0; y < imageB.Height; y++)
                    {
                        Color pixel = imageB.GetPixel(x, y);
                        Color backpixel = imageA.GetPixel(x, y);
                        int grey = (pixel.R + pixel.G + pixel.B) / 3;
                        int subtractValue = Math.Abs(grey - greygreen);
                        if (subtractValue > threshold)
                        {
                            subtractResult.SetPixel(x, y, pixel);
                        }
                        else
                        {
                            subtractResult.SetPixel(x, y, backpixel);
                        }
                    }
                }

                pictureBox3.Image = subtractResult;
            }
        }

        public void SubtractOpenOnLoad()
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null; // Set to null after disposing
            }
            //load pictureBox1 = B
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";
            openFileDialog1.Title = "Load Picture B";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Check if a file was selected
                modeToolStripMenuItem.Enabled = true;
                luminanceToolStripMenuItem.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                try
                {
                    // Dispose of the old image if one exists
                    if (pictureBox1.Image != null)
                    {
                        pictureBox1.Image.Dispose();
                    }

                    // Load and display the new image
                    imageB = new Bitmap(openFileDialog1.FileName);
                    pictureBox1.Image = imageB;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //load pictureBox2 = A

            openFileDialog2.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";
            openFileDialog2.Title = "Load Picture A";

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                // Check if a file was selected
                modeToolStripMenuItem.Enabled = true;
                luminanceToolStripMenuItem.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                try
                {
                    // Dispose of the old image if one exists
                    if (pictureBox2.Image != null)
                    {
                        pictureBox2.Image.Dispose();
                    }

                    // Load and display the new image
                    imageA = new Bitmap(openFileDialog2.FileName);
                    pictureBox2.Image = imageA;
                    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ResizeAndAddElementsInSubtract()
        {
            radioButtonBrightness.Visible = false;
            radioButtonContrast.Visible = false;
            trackBar.Visible = false;
            modeToolStripMenuItem.Enabled = false;
            luminanceToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            cameraToolStripMenuItem.Enabled = false;
            label1.Text = "Picture B";
            label2.Text = "Picture A";
            label2.ForeColor = Color.Black;
            this.Size = new Size(1190, 520);
            this.CenterToScreen();

            pictureBox3 = new PictureBox();
            pictureBox3.Size = pictureBox1.Size;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.Location = new Point(pictureBox2.Location.X + 380, pictureBox2.Location.Y);
            this.Controls.Add(pictureBox3);

            label3 = new System.Windows.Forms.Label();
            label3.Text = "Result";
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.ForeColor = Color.Green;
            label3.Font = new Font("Palatino Linotype", 12, FontStyle.Bold);
            label3.Location = new Point(label2.Location.X + 370, label2.Location.Y);
            this.Controls.Add(label3);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(state)
            {
                state = false;
                label1.Text = "Original";
                label2.Text = "Result";
                label2.ForeColor = Color.Green;
                this.Size = new Size(816, 520);
                this.CenterToScreen();
                if (pictureBox2.Image != null)
                {
                    pictureBox2.Image.Dispose();
                    pictureBox2.Image = null;
                }

                if (pictureBox3.Image != null)
                {
                    pictureBox3.Image.Dispose();
                    pictureBox3.Image = null;
                }
            }
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Check if a file was selected
                modeToolStripMenuItem.Enabled = true;
                luminanceToolStripMenuItem.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                try
                {
                    // Dispose of the old image if one exists
                    if (pictureBox1.Image != null)
                    {
                        pictureBox1.Image.Dispose();
                    }

                    // Load and display the new image
                    loadPicture = new Bitmap(openFileDialog1.FileName);
                    pictureBox1.Image = loadPicture;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
    }
}
