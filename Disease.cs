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

namespace Diseasedetector
{
    class Disease
    {

        private List<string> symptoms = new List<string>();
        private List<string> disease = new List<string>();
        private List<int> diseasepercentage = new List<int>();
        private List<int> originalplaces = new List<int>();

        public Disease()
        { }//End of constructor's body

        public void setSypmtoms(string[] symptomsfromdatabase)
        {
            symptoms.AddRange(symptomsfromdatabase);
        }

        public void setDisease(string[] diseasesfromdatabase)
        {
            disease.AddRange(diseasesfromdatabase);
        }

        public void initializeDiseasePercentages(int n)
        {
            for (int i = 0; i < n; i++)
                diseasepercentage.Add(0);
        }

        public void setDiseasePercentages(string[] symp, DataSet d)
        {
            for (int outerloop = 0; outerloop < disease.Count; outerloop++)
            {
                for (int innerloop = 1; innerloop < 6; innerloop++)
                {
                    if (d.Tables["Diseases&Symptoms"].Rows[outerloop]["Symptom1"].ToString() == symp[innerloop] || d.Tables["Diseases&Symptoms"].Rows[outerloop]["Symptom2"].ToString() == symp[innerloop] || d.Tables["Diseases&Symptoms"].Rows[outerloop]["Symptom3"].ToString() == symp[innerloop] || d.Tables["Diseases&Symptoms"].Rows[outerloop]["Symptom4"].ToString() == symp[innerloop] || d.Tables["Diseases&Symptoms"].Rows[outerloop]["Symptom5"].ToString() == symp[innerloop])
                    {
                        diseasepercentage[outerloop] += 20;
                    }
                }
            }

        }//End of setDiseasePercentages





        public string getDisease()
        {
            int i = 0;
            for (int j = 0; j < disease.Count; j++)
            {
                originalplaces.Add(j);
            }
            while (i <= disease.Count)
            {
                for (int j = 0; j <= disease.Count - 1; j++)
                {
                    if ((j + 1 != disease.Count) && (diseasepercentage[j] < diseasepercentage[j + 1]))
                    {
                        int temp = diseasepercentage[j];
                        diseasepercentage[j] = diseasepercentage[j + 1];
                        diseasepercentage[j + 1] = temp;

                        temp = originalplaces[j];
                        originalplaces[j] = originalplaces[j + 1];
                        originalplaces[j + 1] = temp;
                    }
                }//End of inner for loop
                i++;
            }//End of outer while loop

            if (diseasepercentage[0] == diseasepercentage[1])
                return disease[originalplaces[0]] + " & " + disease[originalplaces[1]];
            else
                return disease[originalplaces[0]];


        }//end of getDisease method

        public string getMedicine(DataSet d)//For suggestion of medicine
        {
            if (diseasepercentage[0] == diseasepercentage[1])
            {
                return "No definite medicine";
            }
            else
            {
                return "\n " + d.Tables["Diseases&Symptoms"].Rows[originalplaces[0]]["Medicine1"].ToString() + "\n " + d.Tables["Diseases&Symptoms"].Rows[originalplaces[0]]["Medicine2"].ToString();
            }
        }//End of method body
    }



}
