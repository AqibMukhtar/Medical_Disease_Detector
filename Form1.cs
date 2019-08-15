using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Expression.Encoder.Live;
using Microsoft.Expression.Encoder;

namespace Diseasedetector
{
    /*This one is lateset*/
    public partial class Form1 : Form
    {
        private LiveJob _job;
        private LiveDeviceSource _deviceSource;
        string strGrabFileName = "No picture provided";

        OleDbConnection con = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;Data Source=C:/MDD Data Bases/Patients database.mdb;jet OleDb:Database Password =AUA");
        OleDbDataAdapter adap = new OleDbDataAdapter("select * from [DiseaseDetector]", "Provider=Microsoft.JET.OLEDB.4.0;Data Source=C:/MDD Data Bases/Patients database.mdb;jet OleDb:Database Password =AUA");
        DataSet D1 = new DataSet("DiseaseDetector");

        OleDbConnection connection_for_lodding_disease_symptom_medicine = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;Data Source=C:/MDD Data Bases/Disease_Medicine_Symptom_DataBase.mdb;Jet OleDb:Database Password =AUA");
        OleDbDataAdapter adap4datadands = new OleDbDataAdapter("SELECT * FROM [Diseases&Symptoms]", "Provider=Microsoft.JET.OLEDB.4.0;Data Source=C:/MDD Data Bases/Disease_Medicine_Symptom_DataBase.mdb;Jet OleDb:Database Password =AUA");
        OleDbDataAdapter adap4s = new OleDbDataAdapter("SELECT DISTINCT Symptoms FROM [Symptoms]", "Provider=Microsoft.JET.OLEDB.4.0;Data Source=C:/MDD Data Bases/Disease_Medicine_Symptom_DataBase.mdb;Jet OleDb:Database Password =AUA");
        DataSet dands = new DataSet("Diseases&Symptoms");
        DataSet onlys = new DataSet("Symptoms");

        string gender = "";
        int nooftimesindexchanged = 0;
        Disease YourDisease = new Disease();
        string[] YourSymptoms = new string[6];
        string ResultDisease = "";
        string ResultMedicine = "";
        SpeechSynthesizer synth = new SpeechSynthesizer();

        public static string passingtext;
        bool spaceallowed = false;
       
        public Form1()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                connection_for_lodding_disease_symptom_medicine.Open();
            }
            catch
            {
                MessageBox.Show("Could not find all required files.\nKindly read READ ME file.", "Error loading files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            adap4s.Fill(onlys, "Symptoms");
            adap.Fill(D1, "DiseaseDetector");
            adap4datadands.Fill(dands, "Diseases&Symptoms");
            string[] symptoms = new string[onlys.Tables["Symptoms"].Rows.Count];
            string[] disease = new string[dands.Tables["Diseases&Symptoms"].Rows.Count];
            for (int i = 0; i < onlys.Tables["Symptoms"].Rows.Count; i++)
            {
                comboBox1.Items.Add(onlys.Tables["Symptoms"].Rows[i]["Symptoms"].ToString());//For adding symptoms in combobox
                symptoms[i] = onlys.Tables["Symptoms"].Rows[i]["Symptoms"].ToString();
            }
            for (int i = 0; i < dands.Tables["Diseases&Symptoms"].Rows.Count; i++)
            {
                disease[i] = dands.Tables["Diseases&Symptoms"].Rows[i]["Disease"].ToString();
            }
            YourDisease.setSypmtoms(symptoms);//For updating symptoms in class
            YourDisease.setDisease(disease);//For updating diseases in class
            YourDisease.initializeDiseasePercentages(dands.Tables["Diseases&Symptoms"].Rows.Count);


            panel7.Show();
            panel1.Hide();
            panel2.Hide();
            panel3.Hide();
            panel8.Hide();
            label2.BackColor = Color.FromArgb(0, 230, 230);
        }

        //Introduction
        private void button5_Click(object sender, EventArgs e)
        {
            panel7.Hide();
            panel1.Show();
            panel3.Show();
        }//Continue from introduction
        //End of Introduction

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (nooftimesindexchanged <= 2)
            {
                Reportbutton.Enabled = false;
                Printbutton.Enabled = false;
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            nooftimesindexchanged++;
            if (nooftimesindexchanged == 6)
            {
               
                synth.Speak("You can maximum select five symptoms");
                MessageBox.Show("You can maximum select 5 symptoms", "Over Selection", MessageBoxButtons.OK,MessageBoxIcon.Error);   
                nooftimesindexchanged = 5;
            }


            else
            {
                if (nooftimesindexchanged == 1)
                {
                    if (symptom1.Text == comboBox1.SelectedItem.ToString() || symptom2.Text == comboBox1.SelectedItem.ToString() || symptom3.Text == comboBox1.SelectedItem.ToString() || symptom4.Text == comboBox1.SelectedItem.ToString() || symptom5.Text == comboBox1.SelectedItem.ToString())
                    {
                        synth.Speak("You selected a symptom which is already selected");
                        MessageBox.Show("You selected a symptom which is already selected!", "Error", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                        nooftimesindexchanged--;
                    }
                    else
                    {
                        symptom1.Text = comboBox1.SelectedItem.ToString();
                        L1.Enabled = true;
                        symptom1.Enabled = true;
                    }
                }


                else if (nooftimesindexchanged == 2)
                {
                    if (symptom1.Text == comboBox1.SelectedItem.ToString() || symptom2.Text == comboBox1.SelectedItem.ToString() || symptom3.Text == comboBox1.SelectedItem.ToString() || symptom4.Text == comboBox1.SelectedItem.ToString() || symptom5.Text == comboBox1.SelectedItem.ToString())
                    {
                        synth.Speak("You selected a symptom which is already selected");
                        MessageBox.Show("You selected a symptom which is already selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        nooftimesindexchanged--;
                    }
                    else
                    {
                        symptom2.Text = comboBox1.SelectedItem.ToString();
                        L2.Enabled = true;
                        symptom2.Enabled = true;
                    }
                }


                else if (nooftimesindexchanged == 3)
                {
                    if (symptom1.Text == comboBox1.SelectedItem.ToString() || symptom2.Text == comboBox1.SelectedItem.ToString() || symptom3.Text == comboBox1.SelectedItem.ToString() || symptom4.Text == comboBox1.SelectedItem.ToString() || symptom5.Text == comboBox1.SelectedItem.ToString())
                    {
                        synth.Speak("You selected a symptom which is already selected");
                        MessageBox.Show("You selected a symptom which is already selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        nooftimesindexchanged--;
                    }
                    else
                    {
                        symptom3.Text = comboBox1.SelectedItem.ToString();
                        L3.Enabled = true;
                        symptom3.Enabled = true;
                    }
                }


                else if (nooftimesindexchanged == 4)
                {
                    if (symptom1.Text == comboBox1.SelectedItem.ToString() || symptom2.Text == comboBox1.SelectedItem.ToString() || symptom3.Text == comboBox1.SelectedItem.ToString() || symptom4.Text == comboBox1.SelectedItem.ToString() || symptom5.Text == comboBox1.SelectedItem.ToString())
                    {
                        synth.Speak("You selected a symptom which is already selected");
                        MessageBox.Show("You selected a symptom which is already selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        nooftimesindexchanged--;
                    }
                    else
                    {
                        symptom4.Text = comboBox1.SelectedItem.ToString();
                        L4.Enabled = true;
                        symptom4.Enabled = true;
                    }
                }


                else if (nooftimesindexchanged == 5)
                {
                    if (symptom1.Text == comboBox1.SelectedItem.ToString() || symptom2.Text == comboBox1.SelectedItem.ToString() || symptom3.Text == comboBox1.SelectedItem.ToString() || symptom4.Text == comboBox1.SelectedItem.ToString() || symptom5.Text == comboBox1.SelectedItem.ToString())
                    {
                        synth.Speak("You selected a symptom which is already selected");
                        MessageBox.Show("You selected a symptom which is already selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        nooftimesindexchanged--;
                    }
                    else
                    {
                        symptom5.Text = comboBox1.SelectedItem.ToString();
                        L5.Enabled = true;
                        symptom5.Enabled = true;
                    }
                }
            }//End of else body
        }//End of Combobox1 event body

   
        private void button2_Click(object sender, EventArgs e)
        {
            if (nooftimesindexchanged == 0)
            {
                synth.Speak("No symptom to delete");
                MessageBox.Show("No symptom to delete", "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {
                if (nooftimesindexchanged == 5)
                {
                    symptom5.Text = "Symptom 5";
                    L5.Enabled = false;
                    symptom5.Enabled = false;
                    nooftimesindexchanged--;
                }
                else if (nooftimesindexchanged == 4)
                {
                    symptom4.Text = "Symptom 4";
                    L4.Enabled = false;
                    symptom4.Enabled = false;
                    nooftimesindexchanged--;
                }
                else if (nooftimesindexchanged == 3)
                {
                    symptom3.Text = "Symptom 3";
                    L3.Enabled = false;
                    symptom3.Enabled = false;
                    nooftimesindexchanged--;
                }
                else if (nooftimesindexchanged == 2)
                {
                    symptom2.Text = "Symptom 2";
                    L2.Enabled = false;
                    symptom2.Enabled = false;
                    nooftimesindexchanged--;
                }
                else if (nooftimesindexchanged == 1)
                {
                    symptom1.Text = "Symptom 1";
                    L1.Enabled = false;
                    symptom1.Enabled = false;
                    nooftimesindexchanged--;
                }
            }
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Reportbutton.Enabled = true;
            Searchbutton.Enabled = true;
            Printbutton.Enabled = true;
            if (nooftimesindexchanged <= 2)
            {
                synth.Speak("You should select at least three symptoms");
                MessageBox.Show("You should select atleast 3 symptoms", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
             
            }
              
            else
            {
                label7.BackColor = Color.Transparent;
                label8.BackColor = Color.FromArgb(0, 230, 230);

                YourSymptoms[1] = symptom1.Text;
                YourSymptoms[2] = symptom2.Text;
                YourSymptoms[3] = symptom3.Text;
                YourSymptoms[4] = symptom4.Text;
                YourSymptoms[5] = symptom5.Text;
                //
             

                //

                YourDisease.setDiseasePercentages(YourSymptoms, dands);
                ResultDisease = YourDisease.getDisease();
                ResultMedicine = YourDisease.getMedicine(dands);
                

                synth.Speak("Disease detected is " + ResultDisease);
                MessageBox.Show("Disease(s) detected is/are \n" + ResultDisease,"Disease",MessageBoxButtons.OK,MessageBoxIcon.Information);
                if (ResultMedicine == "No definite medicine")
                {
                    synth.Speak("Please consult your doctor");
                    MessageBox.Show(ResultMedicine, "Suggestion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    synth.Speak("Suggested medicine are " + ResultMedicine);
                    MessageBox.Show("Suggested medicine(s) is/are " + ResultMedicine,"Medicine",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }//End of else body
           
            if (nooftimesindexchanged >=3)
              {
                  //con.Open();
                  OleDbCommand com1 = new OleDbCommand("insert into DiseaseDetector(Name,Age,PhoneNo,Disease,Gender,Medicine,CNIC,PictureLink)values('" + NametextBox.Text + "','" + AgetextBox.Text + "','" + PhonetextBox.Text + "','" + ResultDisease + "','" + gender + "','" + ResultMedicine + "','"+CNICtextBox.Text+"','" + strGrabFileName + "')", con);
                  com1.ExecuteNonQuery();
                  MessageBox.Show("Your record is saved", "Record", MessageBoxButtons.OK, MessageBoxIcon.Information);          
              }
              
        }//End of button3 body


        private void button1_Click(object sender, EventArgs e)
        {
            Dashlabel.Hide();
            if (pictureBox3.Image == null)
            {
                MessageBox.Show("Kindly take your current picture","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                button9.Focus();
                return;
            }
            if (radioButton1.Checked == true)
            {
                gender = "Male";
            }
            if (radioButton2.Checked == true)
            {
                gender = "Female";
            }
            if (radioButton1.Checked == false && radioButton2.Checked == false)
            {
                MessageBox.Show("Please Select Your gender", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                radioButton1.BackColor = Color.LightPink;
                radioButton2.BackColor = Color.LightPink;
                return;
            }
            if (NametextBox.Text == "")
            {
                NametextBox.BackColor = Color.LightPink;
                MessageBox.Show("Please enter name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                NametextBox.Focus();
                NametextBox.Clear();
                return;
            }
            else
            {
                int outage;
                if(int.TryParse(NametextBox.Text,out outage))
                {
                    NametextBox.BackColor = Color.LightPink;
                    MessageBox.Show("Name should not be in digits", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    NametextBox.Focus();
                    NametextBox.Clear();
                    return;
                }
            }
            if (AgetextBox.Text == "")
            {
                AgetextBox.BackColor = Color.LightPink;
                MessageBox.Show("Please enter age", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AgetextBox.Focus();
                AgetextBox.Clear();
                return;
            }
            else
            {
                int outage;
                if (!int.TryParse(AgetextBox.Text, out outage))
                {
                    AgetextBox.BackColor = Color.LightPink;
                    MessageBox.Show("Age must be in digits","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    AgetextBox.Focus();
                    AgetextBox.Clear();
                    return;
                }
            }
            if (PhonetextBox.Text == "")
            {
                PhonetextBox.BackColor = Color.LightPink;
                MessageBox.Show("Please enter phone number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                PhonetextBox.Focus();
                PhonetextBox.Clear();
                return;
            }
            else
            {
                double  outage;
                if (!double.TryParse(PhonetextBox.Text, out outage))
                {
                    PhonetextBox.BackColor = Color.LightPink;
                    MessageBox.Show("Phone number must be in digits ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    PhonetextBox.Focus();
                    PhonetextBox.Clear();
                    return;
                }
                if (PhonetextBox.TextLength < 11)
                {
                    PhonetextBox.BackColor = Color.LightPink;
                    MessageBox.Show("Please enter complete phone number ","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    PhonetextBox.Focus();
                  
                    return;
                }
            }
            if (CNICtextBox.Text == "")
            {
                CNICtextBox.BackColor = Color.LightPink;
                MessageBox.Show("Kindly enter your CNIC number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CNICtextBox.Focus();
                CNICtextBox.Clear();
                return;
            }
            else
            {
                /*double outage;
                if (!double.TryParse(CNICtextBox.Text, out outage))
                {
                    CNICtextBox.BackColor = Color.LightPink;
                    MessageBox.Show("CNIC must be in digits ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CNICtextBox.Focus();
                    CNICtextBox.Clear();
                    return;
                }*/
                if (CNICtextBox.TextLength < 15)
                {
                    CNICtextBox.BackColor = Color.LightPink;
                    MessageBox.Show("Please enter complete CNIC ","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    CNICtextBox.Focus();                  
                    return;
                }
            }
            for (int i = 0; i < D1.Tables["DiseaseDetector"].Rows.Count; i++)
            {
                if (CNICtextBox.Text == D1.Tables["DiseaseDetector"].Rows[i]["CNIC"].ToString() && NametextBox.Text != D1.Tables["DiseaseDetector"].Rows[i]["Name"].ToString())
                {
                    MessageBox.Show("The name you entered mismatches with CNIC you entered earlier. Note that name is case sensitive. If you didnot login with your CNIC earlier, then kindly contact developers", "Name and CNIC mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    CNICtextBox.Focus();
                    return;                
                }               
            }
            for (int i = 0; i < D1.Tables["DiseaseDetector"].Rows.Count; i++)
            {
                if (CNICtextBox.Text == D1.Tables["DiseaseDetector"].Rows[i]["CNIC"].ToString() && gender != D1.Tables["DiseaseDetector"].Rows[i]["Gender"].ToString())
                {
                    MessageBox.Show("The gender you entered mismatches with gender you entered earlier. If you didnot login with your CNIC earlier, then kindly contact developers","Gender mismatch",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    radioButton1.Focus();
                    return;
                }
            }
            for (int i = 0; i < D1.Tables["DiseaseDetector"].Rows.Count; i++)
            {
                if (CNICtextBox.Text == D1.Tables["DiseaseDetector"].Rows[i]["CNIC"].ToString() && Convert.ToInt32(AgetextBox.Text) < Convert.ToInt32(D1.Tables["DiseaseDetector"].Rows[i]["Age"].ToString()))
                {
                    MessageBox.Show("The age you entered is less than age you entered previously.","Age error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    AgetextBox.Focus();
                    return;
                }
            }
            panel1.Hide();
            panel7.Hide();
            panel2.Show();
            panel3.Show();
            label7.BackColor = Color.FromArgb(0, 230, 230);
            label2.BackColor = Color.Transparent;
           
        }

        //Taking name input
        private void NametextBox_TextChanged(object sender, EventArgs e)
        {
            NametextBox.BackColor = Color.White;
        }
        private void NametextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && (!char.IsWhiteSpace(e.KeyChar)) && (!char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
            else
            {
                if (char.IsWhiteSpace(e.KeyChar) && spaceallowed)
                {
                    spaceallowed = false;
                }
                else if (!char.IsWhiteSpace(e.KeyChar))
                {
                    spaceallowed = true;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }
        //End of name taking input

        //Taking input of age
        private void AgetextBox_TextChanged(object sender, EventArgs e)
        {
            AgetextBox.BackColor = Color.White;            
        }
        private void AgetextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (AgetextBox.Text.Length == 0 && e.KeyChar == '0')
            {
                e.Handled = true;
            }
            if (AgetextBox.Text.Length > 1 && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
            if ((!char.IsDigit(e.KeyChar) && (!char.IsControl(e.KeyChar))))
            {
                e.Handled = true;
            }
        }
        //End of age input

        //Phone number input
        private void PhonetextBox_TextChanged(object sender, EventArgs e)
        {
            PhonetextBox.BackColor = Color.White;
            if (PhonetextBox.TextLength == 4 && (PhonetextBox.Text != "0300" && PhonetextBox.Text != "0301" && PhonetextBox.Text != "0302" && PhonetextBox.Text != "0303" && PhonetextBox.Text != "0304" && PhonetextBox.Text != "0305" && PhonetextBox.Text != "0306" && PhonetextBox.Text != "0307" && PhonetextBox.Text != "0308" && PhonetextBox.Text != "0309" && PhonetextBox.Text != "0310" && PhonetextBox.Text != "0311" && PhonetextBox.Text != "0312" && PhonetextBox.Text != "0313" && PhonetextBox.Text != "0314" && PhonetextBox.Text != "0315" && PhonetextBox.Text != "0316" && PhonetextBox.Text != "0317" && PhonetextBox.Text != "0320" && PhonetextBox.Text != "0321" && PhonetextBox.Text != "0322" && PhonetextBox.Text != "0323" && PhonetextBox.Text != "0324" && PhonetextBox.Text != "0331" && PhonetextBox.Text != "0332" && PhonetextBox.Text != "0333" && PhonetextBox.Text != "0334" && PhonetextBox.Text != "0335" && PhonetextBox.Text != "0336" && PhonetextBox.Text != "0340" && PhonetextBox.Text != "0341" && PhonetextBox.Text != "0342" && PhonetextBox.Text != "0343" && PhonetextBox.Text != "0344" && PhonetextBox.Text != "0345" && PhonetextBox.Text != "0346" && PhonetextBox.Text != "0347" && PhonetextBox.Text != "0348" && PhonetextBox.Text != "0349" && PhonetextBox.Text != "0340" && PhonetextBox.Text != "0355"))
            {
                MessageBox.Show("Kindly enter correct number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                PhonetextBox.Clear();
                PhonetextBox.Focus();
                return;
            }
            if (PhonetextBox.Text.Length == 11)
            {
                PhonetextBox.ForeColor = Color.Green; 
            }
            if (PhonetextBox.Text.Length < 11)
            {
                PhonetextBox.ForeColor = Color.Black ;
            }
        }
        private void PhonetextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (PhonetextBox.Text.Length > 10 && e.KeyChar != '\b')
            {
                e.KeyChar = '\0';
            }

            if (char.IsWhiteSpace(e.KeyChar) || char.IsLetter(e.KeyChar) || char.IsSymbol(e.KeyChar) || char.IsPunctuation(e.KeyChar))
            {
                e.Handled = true;
                PhonetextBox.Focus();
            }
        }
        //End of phone input
        

        private void Reportbutton_Click(object sender, EventArgs e)
        {
            label7.BackColor = Color.Transparent;
            label8.BackColor = Color.FromArgb(0, 230, 230);
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(pictureBox3.Image, 570, 150, 250, 197);
            e.Graphics.DrawString("Medical Report", new Font("Cambria", 30, FontStyle.Underline), Brushes.Black, new Point(300, 50));
            e.Graphics.DrawString("Date: " + DateTime.Now, new Font("Cambria", 15, FontStyle.Regular), Brushes.Black, new Point(25, 150));
            e.Graphics.DrawString("Name : " + NametextBox.Text, new Font("Cambria", 20, FontStyle.Regular), Brushes.Black, new Point(25, 220));
            e.Graphics.DrawString("Age :" + AgetextBox.Text, new Font("Cambria", 20, FontStyle.Regular), Brushes.Black, new Point(25, 270));
            e.Graphics.DrawString("Contact No :" + PhonetextBox.Text, new Font("Cambria", 20, FontStyle.Regular), Brushes.Black, new Point(25, 320));
            e.Graphics.DrawString("Gender : " + gender, new Font("Cambria", 20, FontStyle.Regular), Brushes.Black, new Point(25, 370));
            e.Graphics.DrawString("CNIC : " + CNICtextBox.Text, new Font("Cambria", 20, FontStyle.Regular), Brushes.Black, new Point(25, 420));
            e.Graphics.DrawString(Dashlabel.Text, new Font("Cambria", 12, FontStyle.Regular), Brushes.Black, new Point(0, 470));
            e.Graphics.DrawString("Your Sympotms are as follows:", new Font("Cambria", 20, FontStyle.Regular), Brushes.Black, new Point(25, 520));
            e.Graphics.DrawString(symptom1.Text, new Font("Cambria", 15, FontStyle.Regular), Brushes.Black, new Point(25, 570));
            e.Graphics.DrawString(symptom2.Text, new Font("Cambria", 15, FontStyle.Regular), Brushes.Black, new Point(25, 620));
            e.Graphics.DrawString(symptom3.Text, new Font("Cambria", 15, FontStyle.Regular), Brushes.Black, new Point(25, 670));
            e.Graphics.DrawString(symptom4.Text, new Font("Cambria", 15, FontStyle.Regular), Brushes.Black, new Point(25, 720));
            e.Graphics.DrawString(symptom5.Text, new Font("Cambria", 15, FontStyle.Regular), Brushes.Black, new Point(25, 770));
            e.Graphics.DrawString(Dashlabel.Text, new Font("Cambria", 12, FontStyle.Regular), Brushes.Black, new Point(0, 820));
            e.Graphics.DrawString("Disease Detectd is " + ResultDisease, new Font("Cambria", 16, FontStyle.Regular), Brushes.Black, new Point(25, 870));
            e.Graphics.DrawString("Medicine Recommended: ", new Font("Cambria", 18, FontStyle.Regular), Brushes.Black, new Point(25, 920));
            e.Graphics.DrawString(ResultMedicine, new Font("Cambria", 15, FontStyle.Regular), Brushes.Black, new Point(25, 970));
        }

        private void Printbutton_Click(object sender, EventArgs e)
        {
            try
            {
                printDocument1.Print();
            }
            catch
            {
                MessageBox.Show("There could be a driver missing in your device","Printing error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }


        private void NametextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void AgetextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{TAB}"); 
        }



        private void Exitbutton_Click(object sender, EventArgs e)
        {
            synth.Speak("Thank you for using this app. Insha Allah    you will get well soon");
            con.Close();
            connection_for_lodding_disease_symptom_medicine.Close();    
            comboBox1.Items.Clear();
            Application.Exit();
        }


        //For taking input of CNIC
        private void CNICinput_TextChanged(object sender, EventArgs e)
        {
            CNICtextBox.BackColor = Color.White;
            if (CNICtextBox.Text.Length == 15)
            {
                CNICtextBox.ForeColor = Color.Green;
            }
            if (CNICtextBox.Text.Length < 15)
            {
                CNICtextBox.ForeColor = Color.Black;
            }
          
        }
        private void CNICinput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((CNICtextBox.Text.Length == 5 || CNICtextBox.Text.Length == 13) && e.KeyChar == '-')
            {
                return;
            }
            if (char.IsLetter(e.KeyChar) || char.IsSymbol(e.KeyChar) || char.IsWhiteSpace(e.KeyChar) || char.IsPunctuation(e.KeyChar))
            {
                e.Handled = true;
                CNICtextBox.Focus();
            }
            if ((CNICtextBox.Text.Length == 5 || CNICtextBox.Text.Length == 13) && e.KeyChar != '\b' && e.KeyChar != '-')
            {
                CNICtextBox.Text = CNICtextBox.Text + "-";
                CNICtextBox.Focus();
                CNICtextBox.SelectionStart = CNICtextBox.Text.Length;
                return;
            }
            if ((CNICtextBox.Text.Length == 0 || CNICtextBox.Text.Length == 6 || CNICtextBox.Text.Length == 14) && e.KeyChar == '0')
            {
                e.Handled = true;
                return;
            }
            if (CNICtextBox.Text.Length > 14 && e.KeyChar != '\b')
            {
                e.KeyChar = '\0';
                CNICtextBox.ForeColor = Color.Green ;
            }
       }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            gender = "Male";
            radioButton1.BackColor = Color.Transparent;
            radioButton2.BackColor = Color.Transparent;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            gender = "Female";
            radioButton1.BackColor = Color.Transparent;
            radioButton2.BackColor = Color.Transparent;
        }

        private void Searchbutton_Click(object sender, EventArgs e)
        {
            passingtext = CNICtextBox.Text;
            DiseaseDetector dd = new DiseaseDetector();
            dd.Show();
        }

      

       //End of CNIC input 



        
        public void GetSelectedVideoAndAudioDevices(out EncoderDevice video, out EncoderDevice audio)
        {
            video = null;
            audio = null;
            foreach (EncoderDevice edv in EncoderDevices.FindDevices(EncoderDeviceType.Video))
            {
                video = edv;
                string name = "integrated Webcam";
                name = edv.Name;
                break;
            }
            foreach (EncoderDevice eda in EncoderDevices.FindDevices(EncoderDeviceType.Audio))
            {
                audio = eda;
                string aname = "Microphone Array (Realtec High Definition Audio";
                aname = eda.Name;
                break;
            }
        }

        public void StopJob()
        {
            if (_job != null)
            {
                _job.StopEncoding();
                _job.RemoveDeviceSource(_deviceSource);
                _deviceSource.PreviewWindow = null;
                _deviceSource = null;
            }
        }

        public void Open_Cam()//To open camera
        {
            EncoderDevice video = null;
            EncoderDevice audio = null;

            GetSelectedVideoAndAudioDevices(out video, out audio);
            //StopJob();
            /*if (video == null)
            {
                return;
            }*/
            _job = new LiveJob();
            if (video != null && audio != null)
            {
                _deviceSource = _job.AddDeviceSource(video, audio);
                SourceProperties sp = _deviceSource.SourcePropertiesSnapshot();
               // panel9.Size = new Size(sp.Size.Width, sp.Size.Height);
                _job.OutputFormat.VideoProfile.Size = new Size(panel9.Width, panel9.Height);
                _deviceSource.PreviewWindow = new PreviewWindow(new HandleRef(panel9, panel9.Handle));
                _job.ActivateSource(_deviceSource);
            }
            else
            {
                DialogResult re = MessageBox.Show("No camera found in your device.\nDo you want to select picture from your device", "Camera not found", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (re == DialogResult.Yes)
                {
                    OpenFileDialog pic_upload = new OpenFileDialog();
                    pic_upload.Title = "Select your picture";
                    pic_upload.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
                    if (DialogResult.OK == pic_upload.ShowDialog())
                    {
                        strGrabFileName = pic_upload.FileName;
                        pictureBox3.Image = new Bitmap(pic_upload.FileName);
                        pictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    else
                    {
                        if (gender == "Male")
                        {
                            pictureBox3.Image = Diseasedetector.Properties.Resources.male;
                        }
                        else
                        {
                            pictureBox3.Image = Diseasedetector.Properties.Resources.female;
                        } 
                    }
                    panel1.Show();
                    panel1.Enabled = true;
                    pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
                    panel8.Hide();
                    button9.Enabled = false;
                }
                else
                {
                    if (gender == "Male")
                    {
                        pictureBox3.Image = Diseasedetector.Properties.Resources.male;
                    }
                    else
                    {
                        pictureBox3.Image = Diseasedetector.Properties.Resources.female;
                    }
                    panel1.Show();
                    panel1.Enabled = true;
                    pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
                    panel8.Hide();
                    button9.Enabled = false;
                }
            }
        }

        public void Capture_Image()//To capture image
        {
            using (Bitmap bitmap = new Bitmap(panel9.Width , panel9.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    Rectangle rectanglePanelVideoPreview = panel9.Bounds;
                    Point sourcePoints = panel9.PointToScreen(new Point(panel9.ClientRectangle.X , panel9.ClientRectangle.Y));
                    g.CopyFromScreen(sourcePoints, Point.Empty, rectanglePanelVideoPreview.Size);
                }

                strGrabFileName = String.Format("C:/MDD Data Bases/Pictures/"+NametextBox.Text+"_{0:yyyyMMdd_hhmmss}.jpg", DateTime.Now);
                bitmap.Save(strGrabFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                StopJob();
                panel9.BackgroundImage = Image.FromFile(strGrabFileName);
            }
        }



        private void button9_Click(object sender, EventArgs e)//For opening camera
        {
            if (NametextBox.Text == "" || gender == "")
            {
                MessageBox.Show("Kindly fill name and gender first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                NametextBox.Enabled = false;
                panel1.Hide();
                panel8.Show();
                panel8.Enabled = true;
                button8.Hide();
                button7.Hide();
                picturenotOK.Hide();
                Open_Cam();
                button7.Show();
            }
        }

        private void button7_Click_1(object sender, EventArgs e)//For capturing image 
        {
            Capture_Image();
            button8.Show();
            picturenotOK.Show();
            button7.Hide();
        }

        private void button8_Click(object sender, EventArgs e)//For verifyng image
        {
            panel8.Hide();
            button9.Enabled = false;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.Image = Image.FromFile(strGrabFileName);
            panel1.Show();
            panel1.Enabled = true;
        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void picturenotOK_Click(object sender, EventArgs e)//For retaking image
        {
            panel8.Show();
            panel8.Enabled = true;
            button8.Hide();
            button7.Hide();
            picturenotOK.Hide();
            Open_Cam();
            button7.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            con.Close();
            connection_for_lodding_disease_symptom_medicine.Close();
            comboBox1.Items.Clear();
            Form1 form1 = new Form1();
            DiseaseDetector DD1 = new DiseaseDetector();
            DD1.Close();
            form1.Close();
            Application.Restart();
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }



    }
}


     

    




