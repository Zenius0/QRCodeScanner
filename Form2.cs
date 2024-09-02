using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Tesseract;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;

namespace QRCodeScanner
{
    public partial class Form2 : Form
    {
        string spesificQRCode = "https://monitoring.e-kassa.gov.az/#/index?doc=BdcQ9LPwqcNy2gSyQYnpgRvodhY14c8ig1zvMjDMPzyx";
        public Form2()
        {
            InitializeComponent();
        }
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;
        private void Form2_Load(object sender, EventArgs e)
        {
           filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            for (int i = 0; i < filterInfoCollection.Count; i++)
            {
                comboBox1.Items.Add(filterInfoCollection[i].Name);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            captureDevice = new VideoCaptureDevice(filterInfoCollection[comboBox1.SelectedIndex].MonikerString);
            captureDevice.NewFrame += CaptureDevice_NewFrame;
            captureDevice.Start();
            timer1.Start();
        }

        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (captureDevice.IsRunning)
            {
                captureDevice.Stop();
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox1.Image!=null)
            {
                BarcodeReader barcodeReader = new BarcodeReader();
                Result result = barcodeReader.Decode((Bitmap)pictureBox1.Image);
                if (result != null)
                {
                    if (result.Text == spesificQRCode)

                    {
                        textBox1.Text = result.ToString();
                        timer1.Stop();
                        if (captureDevice.IsRunning)
                        {
                            captureDevice.Stop();
                        }

                        string imageUrl = "https://monitoring.e-kassa.gov.az/#/index?doc=BdcQ9LPwqcNy2gSyQYnpgRvodhY14c8ig1zvMjDMPzyx"; 
                        string imagePath = await DownloadImage(imageUrl, "C:\\Users\\Selim\\Pictures\\receipt.png");

                        string ocrText = PerformOCR(imagePath);
                        var (total, tax, date, receiptNo) = ExtractData(ocrText);

                        SaveToDatabase(total, tax, date, receiptNo);


                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = result.Text,
                            UseShellExecute = true
                        });
                    }
                    else
                    {
                        MessageBox.Show("Invalid QR Code! Please scan the correct QR code.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }  
            }
        }

        public async Task<string> DownloadImage(string imageUrl, string savePath)
        {
            HttpClient client = new HttpClient();
            var imageBytes = await client.GetByteArrayAsync(imageUrl);
            await Task.Run(() => File.WriteAllBytes(savePath, imageBytes));
            return savePath;
        }

        public string PerformOCR(string imagePath)
        {
            var ocrEngine = new TesseractEngine("C:\tesserract", "eng", EngineMode.Default);
            using (var img = Pix.LoadFromFile(imagePath))
            {
                using (var page = ocrEngine.Process(img))
                {
                    return page.GetText();
                }
            }
        }

        public (string total, string tax, string date, string receiptNo) ExtractData(string ocrText)
        {
            string totalPattern = @"Total: \$(\d+\.\d{2})";
            string taxPattern = @"Total Tax: \$(\d+\.\d{2})";
            string datePattern = @"Date: (\d{2}/\d{2}/\d{4})";
            string receiptPattern = @"Receipt No: (\d+)";

            var totalMatch = Regex.Match(ocrText, totalPattern);
            var taxMatch = Regex.Match(ocrText, taxPattern);
            var dateMatch = Regex.Match(ocrText, datePattern);
            var receiptMatch = Regex.Match(ocrText, receiptPattern);

            return (
                total: totalMatch.Groups[1].Value,
                tax: taxMatch.Groups[1].Value,
                date: dateMatch.Groups[1].Value,
                receiptNo: receiptMatch.Groups[1].Value
            );
        }

        public void SaveToDatabase(string total, string tax, string date, string receiptNo)
        {
            string connectionString = "\"Server=localhost;Database=receipt_infs;User Id=sa;Password=1;\"";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Receipts (Total, TotalTax, Date, ReceiptNo) VALUES (@total, @tax, @date, @receiptNo)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@total", total);
                command.Parameters.AddWithValue("@tax", tax);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@receiptNo", receiptNo);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}