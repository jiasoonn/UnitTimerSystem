using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace UnitTimerSystem
{
    public partial class Form2 : Form
    {
        OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\2976709\Desktop\TimerC.mdb");

        System.Timers.Timer t;
        System.Windows.Forms.Timer now = null;
        int m, s, totalsec;

        private void Form2_Load(object sender, EventArgs e)
        {
            now = new System.Windows.Forms.Timer();
            now.Interval = 1000;
            now.Tick += new EventHandler(Now_Tick);
            now.Enabled = true;
            

            DateTime today = DateTime.Now;
            datelb.Text = today.ToString("");
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

            Output();
            
           
           
        }

        private void Output()
        {
             var uph = 3600 / Decimal.Parse(sctlb.Text);
            uph = Convert.ToInt32(Math.Floor(uph));
            int hoc = HourlyOutput();
            hoclb.Text = hoc + "/" + uph.ToString();

            int soc = ShiftlyOutput();
            soclb.Text = soc.ToString();

            long n = long.Parse(DateTime.Now.ToString("mm"));
            decimal eff = hoc / (n * uph) * 100;
            eff = Math.Round(eff, 2);
            elb.Text = eff.ToString() + "%";
            
            RecentCT();

            bunifuGauge2.Value = Convert.ToInt32(Math.Floor(eff));
            bunifuGauge1.Value = Convert.ToInt32(Math.Floor(hoc / uph * 100));
        }

        private void RecentCT()
        {
            int j = 0, UnitCount = 0;
            for (j = 0; j < 10; j++)
            {
                tableLayoutPanel1.Controls.Remove(tableLayoutPanel1.GetControlFromPosition(j + 1, 0));
                tableLayoutPanel1.Controls.Remove(tableLayoutPanel1.GetControlFromPosition(j + 1, 1));
            }

            const string sql2 = @"SELECT COUNT(*) FROM timer WHERE ShiftLog = @sl AND ShiftStart = @ss AND ShiftEnd = @se AND ModelLog = @m";
            OleDbCommand cmd2 = new OleDbCommand(sql2, connection);

            cmd2.Parameters.Add("@sl", OleDbType.VarWChar).Value = shiftlb.Text;
            if (int.Parse(DateTime.Now.ToString("%H")) >= 7 && int.Parse(DateTime.Now.ToString("%H")) < 19) //Shift A
            {
                cmd2.Parameters.Add("@ss", OleDbType.VarWChar).Value = 7 + "&" + DateTime.Today.ToShortDateString();
                cmd2.Parameters.Add("@se", OleDbType.VarWChar).Value = 1859 + "&" + DateTime.Today.ToShortDateString();
            }
            else if (int.Parse(DateTime.Now.ToString("%H")) >= 1 && int.Parse(DateTime.Now.ToString("%H")) < 7) //Shift B 1xxam to 659am
            {
                cmd2.Parameters.Add("@ss", OleDbType.VarWChar).Value = 19 + "&" + DateTime.Today.AddDays(-1).ToShortDateString();
                cmd2.Parameters.Add("@se", OleDbType.VarWChar).Value = 659 + "&" + DateTime.Today.ToShortDateString();
            }
            else if (int.Parse(DateTime.Now.ToString("%H")) >= 19 && int.Parse(DateTime.Now.ToString("%H")) <= 24) //Shift B 7xxpm to 12xxam
            {
                cmd2.Parameters.Add("@ss", OleDbType.VarWChar).Value = 19 + "&" + DateTime.Today.ToShortDateString();
                cmd2.Parameters.Add("@se", OleDbType.VarWChar).Value = 659 + "&" + DateTime.Today.AddDays(1).ToShortDateString();
            }
            cmd2.Parameters.Add("@m", OleDbType.VarWChar).Value = mlb.Text;

            try
            {
                connection.Open();
                UnitCount = (Int32)cmd2.ExecuteScalar();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Last UnitID cannot be retrieved. Error: " + ex.Message);
                connection.Close();
            }

            if (UnitCount != 0) //if there is record in this shift
            {
                string sql = "select top 10 * from timer where ModelLog = @m and ShiftLog = @sl and ShiftStart = @ss and ShiftEnd = @se ORDER BY ID DESC"; //get last 10 records within same shift

                using (OleDbCommand cmd = new OleDbCommand(sql, connection))
                {
                    cmd.Parameters.Add("@m", OleDbType.VarWChar).Value = mlb.Text;
                    cmd.Parameters.Add("@sl", OleDbType.VarWChar).Value = shiftlb.Text;
                    if (int.Parse(DateTime.Now.ToString("%H")) >= 7 && int.Parse(DateTime.Now.ToString("%H")) < 19) //Shift A
                    {
                        cmd.Parameters.Add("@ss", OleDbType.VarWChar).Value = 7 + "&" + DateTime.Today.ToShortDateString();
                        cmd.Parameters.Add("@se", OleDbType.VarWChar).Value = 1859 + "&" + DateTime.Today.ToShortDateString();
                    }
                    else if (int.Parse(DateTime.Now.ToString("%H")) >= 1 && int.Parse(DateTime.Now.ToString("%H")) < 7) //Shift B 1xxam to 659am
                    {
                        cmd.Parameters.Add("@ss", OleDbType.VarWChar).Value = 19 + "&" + DateTime.Today.AddDays(-1).ToShortDateString();
                        cmd.Parameters.Add("@se", OleDbType.VarWChar).Value = 659 + "&" + DateTime.Today.ToShortDateString();
                    }
                    else if (int.Parse(DateTime.Now.ToString("%H")) >= 19 && int.Parse(DateTime.Now.ToString("%H")) <= 24) //Shift B 7xxpm to 12xxam
                    {
                        cmd.Parameters.Add("@ss", OleDbType.VarWChar).Value = 19 + "&" + DateTime.Today.ToShortDateString();
                        cmd.Parameters.Add("@se", OleDbType.VarWChar).Value = 659 + "&" + DateTime.Today.AddDays(1).ToShortDateString();
                    }

                    try
                    {
                        connection.Open();
                        Label[] labels = new Label[10];
                        Label[] labels2 = new Label[10];
                        int i = 0;
                        Decimal total = 0;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (int.Parse(reader["CTLog"].ToString()) > int.Parse(sctlb.Text))
                                {
                                    labels[i] = new Label
                                    {
                                        Text = reader["CTLog"].ToString(),
                                        Anchor = AnchorStyles.None,
                                        Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular),
                                        TextAlign = ContentAlignment.MiddleCenter,
                                        ForeColor = Color.White,
                                        BackColor = Color.Red
                                    };
                                    labels2[i] = new Label
                                    {
                                        Text = "Unit " + reader["UnitID"].ToString(),
                                        Anchor = AnchorStyles.None,
                                        Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular),
                                        TextAlign = ContentAlignment.MiddleCenter,
                                        ForeColor = Color.White,
                                        BackColor = Color.Red
                                    };
                                }
                                else if (int.Parse(reader["CTLog"].ToString()) <= int.Parse(sctlb.Text))
                                {
                                    labels[i] = new Label
                                    {
                                        Text = reader["CTLog"].ToString(),
                                        Anchor = AnchorStyles.None,
                                        Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular),
                                        TextAlign = ContentAlignment.MiddleCenter,
                                        ForeColor = Color.White,
                                        BackColor = Color.Green
                                    };
                                    labels2[i] = new Label
                                    {
                                        Text = "Unit " + reader["UnitID"].ToString(),
                                        Anchor = AnchorStyles.None,
                                        Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular),
                                        TextAlign = ContentAlignment.MiddleCenter,
                                        ForeColor = Color.White,
                                        BackColor = Color.Green
                                    };
                                }
                                tableLayoutPanel1.Controls.Add(labels2[i], i + 1, 0);
                                tableLayoutPanel1.Controls.Add(labels[i], i + 1, 1);
                                i++;
                                total += int.Parse(reader["CTLog"].ToString());
                            }
                            var AveCT = Math.Round(Convert.ToDecimal(total / i), 2);
                            actlb.Text = AveCT.ToString();

                        }
                        connection.Close();


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Recent cycle time records cannot be retrieved. Error: " + ex.Message);
                        connection.Close();
                    }

                }
            }
        }

        private void RunTimer()
        {
            t = new System.Timers.Timer();
            t.Interval = 1000; //1s
            t.Elapsed += OnTimeEvent;
            s = 0;
            m = 0;
            totalsec = 0;

        }

        private void Now_Tick(object sender, EventArgs e)
        {
            datelb.Text = DateTime.Now.ToString();
        }

        private void OnTimeEvent(object sender, ElapsedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                s += 1;
                totalsec += 1;
                button3.Enabled = false;
                
                if (totalsec >= 5)
                {
                    button3.Enabled = true;
                }

                if (s >= 100)
                {
                    s = 0;
                    m += 1;
                    
                }
               
                if(totalsec > int.Parse(sctlb.Text))
                {
                    timerlb.ForeColor = Color.Red;
                }
                else if(totalsec <= int.Parse(sctlb.Text))
                {
                    timerlb.ForeColor = Color.Green;
                }

                timerlb.Text = string.Format("{0}:{1}", m.ToString().PadLeft(2, '0'), s.ToString().PadLeft(2, '0'));
                totalsec = (m * 60) + s;
            }));
        }

        private int HourlyOutput()
        {

            const string sql = @"SELECT COUNT(*) FROM timer WHERE HourID = @h AND TodayDate = @td AND ModelLog = @m";
            OleDbCommand cmd = new OleDbCommand(sql, connection);

            cmd.Parameters.Add("@h", OleDbType.VarWChar).Value = DateTime.Now.ToString("%H");
            cmd.Parameters.Add("@td", OleDbType.VarWChar).Value = DateTime.Now.ToShortDateString();
            cmd.Parameters.Add("@m", OleDbType.VarWChar).Value = mlb.Text;

            try
            {
                connection.Open();
                int count = (Int32)cmd.ExecuteScalar();
                connection.Close();
                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hourly output count cannot be retrieved. Error: " + ex.Message);
                connection.Close();
                int none = 0;
                return none;
            }
        }

        private int ShiftlyOutput()
        {
            const string sql = @"SELECT COUNT(*) FROM timer WHERE ShiftLog = @sl AND ShiftStart = @ss AND ShiftEnd = @se AND ModelLog = @m";
            OleDbCommand cmd = new OleDbCommand(sql, connection);

            cmd.Parameters.Add("@sl", OleDbType.VarWChar).Value = shiftlb.Text;
            if (int.Parse(DateTime.Now.ToString("%H")) >= 7 && int.Parse(DateTime.Now.ToString("%H")) < 19) //Shift A
            {
                cmd.Parameters.Add("@ss", OleDbType.VarWChar).Value = 7 + "&" + DateTime.Today.ToShortDateString();
                cmd.Parameters.Add("@se", OleDbType.VarWChar).Value = 1859 + "&" + DateTime.Today.ToShortDateString();
            }
            else if (int.Parse(DateTime.Now.ToString("%H")) >= 1 && int.Parse(DateTime.Now.ToString("%H")) < 7) //Shift B 1xxam to 659am
            {
                cmd.Parameters.Add("@ss", OleDbType.VarWChar).Value = 19 + "&" + DateTime.Today.AddDays(-1).ToShortDateString();
                cmd.Parameters.Add("@se", OleDbType.VarWChar).Value = 659 + "&" + DateTime.Today.ToShortDateString();
            }
            else if (int.Parse(DateTime.Now.ToString("%H")) >= 19 && int.Parse(DateTime.Now.ToString("%H")) <= 24) //Shift B 7xxpm to 12xxam
            {
                cmd.Parameters.Add("@ss", OleDbType.VarWChar).Value = 19 + "&" + DateTime.Today.ToShortDateString();
                cmd.Parameters.Add("@se", OleDbType.VarWChar).Value = 659 + "&" + DateTime.Today.AddDays(1).ToShortDateString();
            }
            cmd.Parameters.Add("@m", OleDbType.VarWChar).Value = mlb.Text;


            try
            {
                connection.Open();
                int count = (Int32)cmd.ExecuteScalar();
                connection.Close();
                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Shiftly output count cannot be retrieved. Error: " + ex.Message);
                connection.Close();
                int none = 0;
                return none;
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            t.Start();
            button2.Enabled = true;
            statuslb.Text = "Timer is started.";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            t.Stop();
            statuslb.Text = "Timer is paused.";
            
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            t.Stop();
            Application.DoEvents();
            Form1 form1 = new Form1();
            form1.Close();
            
        }

        private void hoclb_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            t.Stop();
            int UnitCount = 0;

            const string sql2 = @"SELECT COUNT(*) FROM timer WHERE ShiftLog = @sl AND ShiftStart = @ss AND ShiftEnd = @se AND ModelLog = @m";
            OleDbCommand cmd2 = new OleDbCommand(sql2, connection);

            cmd2.Parameters.Add("@sl", OleDbType.VarWChar).Value = shiftlb.Text;
            if (int.Parse(DateTime.Now.ToString("%H")) >= 7 && int.Parse(DateTime.Now.ToString("%H")) < 19) //Shift A
            {
                cmd2.Parameters.Add("@ss", OleDbType.VarWChar).Value = 7 + "&" + DateTime.Today.ToShortDateString();
                cmd2.Parameters.Add("@se", OleDbType.VarWChar).Value = 1859 + "&" + DateTime.Today.ToShortDateString();
            }
            else if (int.Parse(DateTime.Now.ToString("%H")) >= 1 && int.Parse(DateTime.Now.ToString("%H")) < 7) //Shift B 1xxam to 659am
            {
                cmd2.Parameters.Add("@ss", OleDbType.VarWChar).Value = 19 + "&" + DateTime.Today.AddDays(-1).ToShortDateString();
                cmd2.Parameters.Add("@se", OleDbType.VarWChar).Value = 659 + "&" + DateTime.Today.ToShortDateString();
            }
            else if (int.Parse(DateTime.Now.ToString("%H")) >= 19 && int.Parse(DateTime.Now.ToString("%H")) <= 24) //Shift B 7xxpm to 12xxam
            {
                cmd2.Parameters.Add("@ss", OleDbType.VarWChar).Value = 19 + "&" + DateTime.Today.ToShortDateString();
                cmd2.Parameters.Add("@se", OleDbType.VarWChar).Value = 659 + "&" + DateTime.Today.AddDays(1).ToShortDateString();
            }
            cmd2.Parameters.Add("@m", OleDbType.VarWChar).Value = mlb.Text;

            try
            {
                connection.Open();
                UnitCount = (Int32)cmd2.ExecuteScalar();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Last UnitID cannot be retrieved. Error: " + ex.Message);
                connection.Close();
            }

            var d = DateTime.Now;
            var today = DateTime.Today.ToShortDateString();
            var h = DateTime.Now.ToString("%H");
            
            const string sql = @"INSERT INTO timer(DateTimeLog, ShiftLog, CTLog, WorkcellLog, ModelLog, StationLog, HourID, TodayDate, ShiftStart, ShiftEnd, UnitID)VALUES(@d, @shift, @ct, @wc, @wm, @ws, @hid, @td, @ss, @se, @uid)";
            OleDbCommand cmd = new OleDbCommand(sql, connection);

            cmd.Parameters.Add("@d", OleDbType.Date).Value = d;
            cmd.Parameters.Add("@shift", OleDbType.VarWChar).Value = shiftlb.Text;
            cmd.Parameters.Add("@ct", OleDbType.VarWChar).Value = totalsec.ToString();
            cmd.Parameters.Add("@wc", OleDbType.VarWChar).Value = wclb.Text;
            cmd.Parameters.Add("@wm", OleDbType.VarWChar).Value = mlb.Text;
            cmd.Parameters.Add("@ws", OleDbType.VarWChar).Value = slb.Text;
            cmd.Parameters.Add("@hid", OleDbType.VarWChar).Value = h;
            cmd.Parameters.Add("@td", OleDbType.VarWChar).Value = today;
            if (int.Parse(DateTime.Now.ToString("%H")) >= 7 && int.Parse(DateTime.Now.ToString("%H")) < 19) //Shift A
            {
                cmd.Parameters.Add("@ss", OleDbType.VarWChar).Value = 7 + "&" + DateTime.Today.ToShortDateString();
                cmd.Parameters.Add("@se", OleDbType.VarWChar).Value = 1859 + "&" + DateTime.Today.ToShortDateString();
            }
            else if (int.Parse(DateTime.Now.ToString("%H")) >= 1 && int.Parse(DateTime.Now.ToString("%H")) < 7) //Shift B 1xxam to 659am
            {
                cmd.Parameters.Add("@ss", OleDbType.VarWChar).Value = 19 + "&" + DateTime.Today.AddDays(-1).ToShortDateString();
                cmd.Parameters.Add("@se", OleDbType.VarWChar).Value = 659 + "&" + DateTime.Today.ToShortDateString();
            }
            else if (int.Parse(DateTime.Now.ToString("%H")) >= 19 && int.Parse(DateTime.Now.ToString("%H")) <= 24) //Shift B 7xxpm to 12xxam
            {
                cmd.Parameters.Add("@ss", OleDbType.VarWChar).Value = 19 + "&" + DateTime.Today.ToShortDateString();
                cmd.Parameters.Add("@se", OleDbType.VarWChar).Value = 659 + "&" + DateTime.Today.AddDays(1).ToShortDateString();
            }
            cmd.Parameters.Add("@uid", OleDbType.VarWChar).Value = (UnitCount + 1).ToString();

            try
            {
                connection.Open();
                if (cmd.ExecuteNonQuery() > 0)
                {
                    statuslb.Text = "Record is saved.";
                }
                connection.Close();
                timerlb.Text = "00:00";
                Output();
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = false;
                t.Dispose();
                RunTimer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cycle time cannot be saved. Error: " + ex.Message);
                connection.Close();
                Form1 form1 = new Form1();
                form1.Show();
                this.Dispose(false);
                Output();
                RunTimer();
            }
        }
        
        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void mlb_Click(object sender, EventArgs e)
        {

        }

        private void shiftlb_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void bunifuGauge1_Load(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        public Form2(String wc, String s, String m, String ct )
        {
            InitializeComponent();
            wclb.Text = wc;
            mlb.Text = m;
            slb.Text = s;
            sctlb.Text = ct;
            chlb.Text = DateTime.Now.ToString("%h");

            RunTimer();
        }
        
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
