namespace QRCodeScanner
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            comboBox1 = new ComboBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            textBox1 = new TextBox();
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(241, 23);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(335, 28);
            comboBox1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(125, 71);
            button1.Name = "button1";
            button1.Size = new Size(123, 36);
            button1.TabIndex = 1;
            button1.Text = "kamera aç";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(353, 71);
            button2.Name = "button2";
            button2.Size = new Size(123, 36);
            button2.TabIndex = 2;
            button2.Text = "QR tara";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(588, 71);
            button3.Name = "button3";
            button3.Size = new Size(123, 36);
            button3.TabIndex = 3;
            button3.Text = "QR ekle";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(96, 26);
            label1.Name = "label1";
            label1.Size = new Size(58, 20);
            label1.TabIndex = 4;
            label1.Text = "kamera";
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(227, 127);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(367, 218);
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(227, 368);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(367, 27);
            textBox1.TabIndex = 6;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(837, 470);
            Controls.Add(textBox1);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(comboBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboBox1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Label label1;
        private PictureBox pictureBox1;
        private TextBox textBox1;
        private System.Windows.Forms.Timer timer1;
    }
}
