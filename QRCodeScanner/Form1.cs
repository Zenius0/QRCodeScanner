using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;

namespace QRCodeScanner
{
    public partial class Form1 : Form
    {
        SqlConnection con;
        SqlDataReader dr;
        SqlCommand com;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string user = textBox1.Text;
            string password = textBox2.Text;
            con = new SqlConnection("Data Source=localhost;Initial Catalog=Auth;User ID=sa;Password=1;TrustServerCertificate=True");
            com = new SqlCommand();
            con.Open();
            com.Connection = con;
            com.CommandText="Select*From Kullanici_Bilgi where kullanici_adi='" +textBox1.Text + 
                "'And sifre='" +textBox2.Text + "'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                MessageBox.Show("You have successfully logged in!");
                Form2 gecis = new Form2();
                gecis.Show();
                this.Hide();
            }
            
            else
            {
                MessageBox.Show("Login Failed!");
            }
            con.Close();
        }    
    }
}
