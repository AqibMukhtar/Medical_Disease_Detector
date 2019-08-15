using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Diseasedetector
{
    public partial class DiseaseDetector : Form
    {
        public DiseaseDetector()
        {
            InitializeComponent();
        }

        OleDbConnection con = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;Data Source=C:/MDD Data Bases/Patients database.mdb;jet OleDb:Database Password =AUA");
        OleDbDataAdapter adap = new OleDbDataAdapter("select * from DiseaseDetector", "Provider=Microsoft.JET.OLEDB.4.0;Data Source=C:/MDD Data Bases/Patients database.mdb;jet OleDb:Database Password =AUA");
        DataSet D1 = new DataSet("DiseaseDetector");

        private void DiseaseDetector_Load(object sender, EventArgs e)
        {
            //SearchtextBox.Text = Form1.passingtext;
                con.Open();
                OleDbDataAdapter adap = new OleDbDataAdapter("Select Age,PhoneNo,Disease,Medicine,Date,PictureLink from DiseaseDetector where CNIC like '" + Form1.passingtext + "'", con);
                DataTable dt = new DataTable();
                adap.Fill(dt);
                dataGridView1.DataSource = dt;
                con.Close();
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

      
    }
}
