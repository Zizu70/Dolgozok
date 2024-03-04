using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GUI
{
    public partial class Form1 : Form
    {
        string endPoint = ReadSetting("endPoint");
        HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            listaFrissitese();
        }

        private async void listaFrissitese()
        {
            listBox1.Items.Clear();
            try
            {
                HttpResponseMessage response = await client.GetAsync(endPoint);
                if (response.IsSuccessStatusCode)
                {
                    // Válasz szöveg beolvasása JSON formátumban
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    var dolgozok = Dolgozo.FromJson(jsonResponse);

                    foreach (Dolgozo item in dolgozok)
                    {
                        listBox1.Items.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show("Sikertelen API kérés: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Sikertelen API kérés: " + ex.Message);
            }
        }

        static string ReadSetting(string key)
        {
            string result = string.Empty;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? "Not Found";
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
            return result;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dolgozo dolgozo = (Dolgozo)listBox1.SelectedItem;
            textBox1.Text = dolgozo.Id.ToString();
            textBox2.Text = dolgozo.Name.ToString();
            numericUpDown1.Value = dolgozo.Salary;
            textBox4.Text = dolgozo.Position.ToString();
           
           

            //MessageBox.Show(JsonConvert.SerializeObject(dolgozo));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listaFrissitese();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //-- Update a dolgozó adatait a PUT metódussal frissítjük a REST API-n keresztül
            Dolgozo dolgozo = new Dolgozo();
            dolgozo.Id = Convert.ToInt32(textBox1.Text);
            if (dolgozo.Id > 0)
            {
                if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox4.Text))
                {
                    MessageBox.Show("Név és email mezők kitöltése kötelező");
                    return;
                }
                dolgozo.Name = textBox2.Text;
                dolgozo.Salary = Convert.ToInt32(numericUpDown1.Value);
                dolgozo.Position = textBox4.Text;
                
                var json = JsonConvert.SerializeObject(dolgozo);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PutAsync(endPoint + "/" + dolgozo.Id, data).Result;
                if (response.IsSuccessStatusCode)    // App.config beírva!!!!!!!!!!!!
                {
                    MessageBox.Show("Sikeres frissítés");
                    listaFrissitese();
                }
                else
                {
                    MessageBox.Show("Sikertelen frissítés: " + response.ReasonPhrase);
                }
            }
            mezokTorlese();
        }

        private void mezokTorlese()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox4.Text = "";
            numericUpDown1.Value = numericUpDown1.Minimum;
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //-- Új dolgozó adatainak felvitele a REST API-n keresztül
            Dolgozo dolgozo = new Dolgozo();
            if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Név és email mezők kitöltése kötelező");
                return;
            }
            dolgozo.Name = textBox2.Text;
            dolgozo.Position = textBox4.Text;
            dolgozo.Salary = Convert.ToInt32(numericUpDown1.Value);
           
            var json = JsonConvert.SerializeObject(dolgozo);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(endPoint, data).Result;
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Sikeres felvitel");
                listaFrissitese();
            }
            else
            {
                MessageBox.Show("Sikertelen felvitel: " + response.ReasonPhrase);
            }
            mezokTorlese();
            listaFrissitese();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            //-- Dolgozó törlése a REST API-n keresztül
            Dolgozo dolgozo = (Dolgozo)listBox1.SelectedItem;
            if (dolgozo != null)
            {
                var response = client.DeleteAsync(endPoint + "/" + dolgozo.Id).Result;
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Sikeres törlés");
                    listaFrissitese();
                }
                else
                {
                    MessageBox.Show("Sikertelen törlés: " + response.ReasonPhrase);
                }
            }
            mezokTorlese();
            listaFrissitese();
        }
    }
}

