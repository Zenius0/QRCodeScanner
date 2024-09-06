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
using System.Globalization;
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
            if (pictureBox1.Image != null)
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

                        // İlgili resim URL'sini indir
                        string imageUrl = "https://monitoring.e-kassa.gov.az/pks-monitoring/2.0.0/documents/BdcQ9LPwqcNy2gSyQYnpgRvodhY14c8ig1zvMjDMPzyx";
                        string imagePath = await DownloadImage(imageUrl, "C:\\Users\\Selim\\Pictures\\receipt.png");

                        // OCR işlemi yap
                        string ocrText = PerformOCR(imagePath);
                        var (total, tax, date, receiptNo) = ExtractData(ocrText);

                        // Sonuçları göster
                        MessageBox.Show(ocrText);
                        MessageBox.Show($"Total: {total}, TotalTax: {tax}, Date: {date}, ReceiptNo: {receiptNo}");

                        // Veritabanına kaydet
                        SaveToDatabase(total, tax, date, receiptNo);

                        // Linke yönlendir
                        Process.Start(new ProcessStartInfo
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
            var ocrEngine = new TesseractEngine(@"C:\\tesserract\\tesseract-5.4.1\\tessdata", "eng", EngineMode.Default);
            using (var img = Pix.LoadFromFile(imagePath))
            {
                using (var page = ocrEngine.Process(img))
                {
                    return page.GetText();
                }
            }
        }

        public (string total, string totalTax, string date, string receiptNo) ExtractData(string ocrText)
        {
            // Gerekli regex kalıplarını tanımla
            string totalPattern = @"Total:? ?(\d+[.,]?\d{1,2})";
            string totalTaxPattern = @"Total Tax:? ?(\d+[.,]?\d{1,2})";
            string datePattern = @"Date:? ?(\d{2}/\d{2}/\d{4})";
            string receiptNoPattern = @"Receipt No:? ?(\d+)";

            // OCR sonucundan değerleri regex ile al
            var totalMatch = Regex.Match(ocrText, totalPattern);
            var taxMatch = Regex.Match(ocrText, totalTaxPattern);
            var dateMatch = Regex.Match(ocrText, datePattern);
            var receiptMatch = Regex.Match(ocrText, receiptNoPattern);

            // Eşleşmeleri kontrol et
            if (!totalMatch.Success) MessageBox.Show("Total değeri bulunamadı!");
            if (!taxMatch.Success) MessageBox.Show("Total Tax değeri bulunamadı!");
            if (!dateMatch.Success) MessageBox.Show("Date değeri bulunamadı!");
            if (!receiptMatch.Success) MessageBox.Show("Receipt No değeri bulunamadı!");

            Debug.WriteLine($"OCR'dan okunan Total: {totalMatch.Groups[1].Value}");
            Debug.WriteLine($"OCR'dan okunan TotalTax: {taxMatch.Groups[1].Value}");
            Debug.WriteLine($"OCR'dan okunan Date: {dateMatch.Groups[1].Value}");
            Debug.WriteLine($"OCR'dan okunan ReceiptNo: {receiptMatch.Groups[1].Value}");

            // Sonuçları döndür
            return (
                total: totalMatch.Groups[1].Value,
                totalTax: taxMatch.Groups[1].Value,
                date: dateMatch.Groups[1].Value,
                receiptNo: receiptMatch.Groups[1].Value
            );
        }

        public void SaveToDatabase(string Total, string TotalTax, string ReceiptDate, string ReceiptNo)
        {
            // Değerleri işleyip doğrula
            Total = Total.Trim();
            TotalTax = TotalTax.Trim();

            if (!decimal.TryParse(Total, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal totalValue))
            {
                MessageBox.Show("Total değeri geçerli bir sayı değil! Lütfen veriyi kontrol edin.");
                return;
            }

            if (!decimal.TryParse(TotalTax, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal totalTaxValue))
            {
                MessageBox.Show("Total Tax değeri geçerli bir sayı değil! Lütfen veriyi kontrol edin.");
                return;
            }

            if (!int.TryParse(ReceiptNo, out int receiptNoValue))
            {
                MessageBox.Show("Receipt No değeri geçerli bir sayı değil! Lütfen veriyi kontrol edin.");
                return;
            }

            // Veritabanı bağlantısını oluştur
            string connectionString = "Server=localhost;Database=receipt_infs;User Id=sys1;Password=12;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO dbo.table1 (Total, TotalTax, ReceiptDate, ReceiptNo) VALUES (@Total, @TotalTax, @ReceiptDate, @ReceiptNo)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Total", totalValue);
                command.Parameters.AddWithValue("@TotalTax", totalTaxValue);
                command.Parameters.AddWithValue("@ReceiptDate", ReceiptDate);
                command.Parameters.AddWithValue("@ReceiptNo", receiptNoValue);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}