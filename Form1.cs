using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO.Ports;

namespace UnitTimerSystem
{
    public partial class Form1 : Form
    {
        
        OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\2976709\Desktop\TimerC.mdb");

        System.Windows.Forms.Timer now = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Now_Tick(object sender, EventArgs e)
        {
            datelb.Text = DateTime.Now.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            now = new System.Windows.Forms.Timer();
            now.Interval = 1000;
            now.Tick += new EventHandler(Now_Tick);
            now.Enabled = true;

            cb1.Items.Add("Advantest");
            cb1.Items.Add("KCTEC HLA");
            cb1.Items.Add("Keysight");
            cb1.Items.Add("keysight HLA");
            cb1.Items.Add("LamMech");
            cb1.Items.Add("TESLA");

            serverlb.ForeColor = Color.Red;
            sensorlb.ForeColor = Color.Red;

            try
            {
                connection.Open();
                serverlb.Text = "Server is connected.";
                serverlb.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Failed\r\n\r\n" + ex.Message);
            }

            DateTime today = DateTime.Now;
            datelb.Text = today.ToString("g");
            if (int.Parse(DateTime.Now.ToString("%H")) >= 7 && int.Parse(DateTime.Now.ToString("%H")) < 19) //Shift A
            {
                shiftlb.Text = "Shift A";
            }
            else if (int.Parse(DateTime.Now.ToString("%H")) >= 1 && int.Parse(DateTime.Now.ToString("%H")) < 7) //Shift B 1xxam to 659am
            {
                shiftlb.Text = "Shift B";
            }
            else if (int.Parse(DateTime.Now.ToString("%H")) >= 19 && int.Parse(DateTime.Now.ToString("%H")) <= 24) //Shift B 7xxpm to 12xxam
            {
                shiftlb.Text = "Shift B";
            }
        }


        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)6)
            button1.PerformClick();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cb1.Text) && string.IsNullOrEmpty(cb2.Text) && string.IsNullOrEmpty(cb3.Text) && string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("You have missing credentials.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                connection.Close();
                return;
            }
            
            TimerCTableAdapters.WCTableAdapter user = new TimerCTableAdapters.WCTableAdapter();
            TimerC.WCDataTable wcdt = user.GetDataByWcPw(cb1.Text, int.Parse(textBox2.Text));
                
            if (wcdt.Rows.Count > 0)
            {
                MessageBox.Show("You have been logged in.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                connection.Close();
                show();
            }
            else
            {
                MessageBox.Show("Password is incorrect.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                connection.Close();
                textBox2.Focus();
                return;
            }
        }
        
        public void show()
        {
            try
            {
                using (connection)
                {
                    connection.Open();
                    OleDbCommand cmd = new OleDbCommand("select * from WM where Workcell = '" + cb1.Text + "' and Model = '" + cb3.Text + "'", connection);
                    OleDbDataReader read = cmd.ExecuteReader();
                    while(read.Read())
                    {
                        urnlb.Text = read["Model"].ToString();
                        uctlb.Text = read["CT"].ToString();
                        var uph = 3600 / (decimal.Parse(read["CT"].ToString()));
                        var uph2 = Convert.ToInt32(Math.Floor(uph));
                        uphlb.Text = uph2.ToString();
                        button2.Enabled = true;
                        cb1.Enabled = false;
                        cb2.Enabled = false; 
                        cb3.Enabled = false;
                        textBox2.Enabled = false;
                        pwcb1.Enabled = false;
                        button1.Enabled = false;


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            List<ComboBox> combobox = new List<ComboBox>();
            if( cb1.SelectedItem.ToString() == "Advantest")
            {
                for(int i = 1; i <= 5; i++ )
                {
                    combobox.Add(new ComboBox() { Id = i, Name = "SA" + i });
                }
                cb2.DataSource = combobox;
                cb2.ValueMember = "Id";
                cb2.DisplayMember = "Name";
            }
            else if (cb1.SelectedItem.ToString() == "KCTEC HLA")
            {
                for (int i = 1; i <= 5; i++)
                {
                    combobox.Add(new ComboBox() { Id = i, Name = "SKCHLA" + i });
                }
                cb2.DataSource = combobox;
                cb2.ValueMember = "Id";
                cb2.DisplayMember = "Name";
            }
            else if (cb1.SelectedItem.ToString() == "Keysight")
            {
                for (int i = 1; i <= 5; i++)
                {
                    combobox.Add(new ComboBox() { Id = i, Name = "SK" + i });
                }
                cb2.DataSource = combobox;
                cb2.ValueMember = "Id";
                cb2.DisplayMember = "Name";
            }
            else if (cb1.SelectedItem.ToString() == "Keysight HLA")
            {
                for (int i = 1; i <= 5; i++)
                {
                    combobox.Add(new ComboBox() { Id = i, Name = "SKHLA" + i });
                }
                cb2.DataSource = combobox;
                cb2.ValueMember = "Id";
                cb2.DisplayMember = "Name";
            }
            else if (cb1.SelectedItem.ToString() == "LamMech")
            {
                for (int i = 1; i <= 5; i++)
                {
                    combobox.Add(new ComboBox() { Id = i, Name = "SLM" + i });
                }
                cb2.DataSource = combobox;
                cb2.ValueMember = "Id";
                cb2.DisplayMember = "Name";
            }
            else if (cb1.SelectedItem.ToString() == "TESLA")
            {
                for (int i = 1; i <= 5; i++)
                {
                    combobox.Add(new ComboBox() { Id = i, Name = "ST" + i });
                }
                cb2.DataSource = combobox;
                cb2.ValueMember = "Id";
                cb2.DisplayMember = "Name";
            }
            
        }

       

        private void cb3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cb2_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<ComboBox> combobox = new List<ComboBox>();
            if (cb1.SelectedItem.ToString() == "Advantest" && cb2.SelectedItem != null )
            {
                for (int i = 1; i <= 5; i++)
                {
                    combobox.Add(new ComboBox() { Id = i, Name = "MA" + i });
                }
                cb3.DataSource = combobox;
                cb3.ValueMember = "Id";
                cb3.DisplayMember = "Name";
            }
            else if (cb1.SelectedItem.ToString() == "KCTEC HLA" && cb2.SelectedItem != null)
            {
                for (int i = 1; i <= 5; i++)
                {
                    combobox.Add(new ComboBox() { Id = i, Name = "MKC" + i });
                }
                cb3.DataSource = combobox;
                cb3.ValueMember = "Id";
                cb3.DisplayMember = "Name";
            }
            else if (cb1.SelectedItem.ToString() == "Keysight" && cb2.SelectedItem != null)
            {
                for (int i = 1; i <= 5; i++)
                {
                    combobox.Add(new ComboBox() { Id = i, Name = "MK" + i });
                }
                cb3.DataSource = combobox;
                cb3.ValueMember = "Id";
                cb3.DisplayMember = "Name";
            }
            else if (cb1.SelectedItem.ToString() == "Keysight HLA" && cb2.SelectedItem != null)
            {
                for (int i = 1; i <= 5; i++)
                {
                    combobox.Add(new ComboBox() { Id = i, Name = "MKLHA" + i });
                }
                cb3.DataSource = combobox;
                cb3.ValueMember = "Id";
                cb3.DisplayMember = "Name";
            }
            else if (cb1.SelectedItem.ToString() == "LamMech" && cb2.SelectedItem != null)
            {
                for (int i = 1; i <= 5; i++)
                {
                    combobox.Add(new ComboBox() { Id = i, Name = "MLM" + i });
                }
                cb3.DataSource = combobox;
                cb3.ValueMember = "Id";
                cb3.DisplayMember = "Name";
            }
            else if (cb1.SelectedItem.ToString() == "TESLA" && cb2.SelectedItem != null)
            {
                for (int i = 1; i <= 5; i++)
                {
                    combobox.Add(new ComboBox() { Id = i, Name = "MT" + i });
                }
                cb3.DataSource = combobox;
                cb3.ValueMember = "Id";
                cb3.DisplayMember = "Name";
            }

        }

        private void pwcb1_CheckedChanged(object sender, EventArgs e)
        {
            if (pwcb1.Checked)
                textBox2.PasswordChar = '\0';
            else
                textBox2.PasswordChar = '*';
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            try
            {
                 using (connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\2976709\Desktop\TimerC.mdb"))
                            {
                                connection.Open();
                                OleDbCommand cmd = new OleDbCommand("select CT from WM where Workcell = '" + cb1.Text + "' and Model = '" + cb3.Text + "'", connection);
                                OleDbDataReader r = cmd.ExecuteReader();
                                while(r.Read())
                                {
                                    var ct = r["CT"].ToString();
                                    Form2 form2 = new Form2(cb1.Text, cb2.Text, cb3.Text, ct);
                                    this.Hide();
                                    form2.ShowDialog();
                                    this.Close();
                                }
                            }
            }
            catch(Exception exc)
            {
                MessageBox.Show("Error: " + exc.Message);
            }
           

                
           

            
        }

        private void label17_Click_1(object sender, EventArgs e)
        {

        }
    }

}
