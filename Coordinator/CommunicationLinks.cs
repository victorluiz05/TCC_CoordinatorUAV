using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Globalization;
using System.Timers;

namespace Coordinator
{
    public partial class CommunicationLinks : Form
    {

        delegate void TextBoxDelegate(string message);

        //Global Variables
        static string CommName = "";
        static string typee = "";
        static string IpAddress = "";
        static string Portt = "";
        static string missionn = "";

        //Struct of a connection
        public struct CommSet
        {
            public string CommuName;
            public string Type;
            public string IP;
            public string Port;
        }

        public struct UAVStatus
        {
            public string CommuName;
            public string Type;
            public string IP;
            public string Port;
            public string N_UAV;
            public string Lat;          
            public string Lon;
            public string Alt;
            public string Groundspeed;

        }

        public CommSet[] CommunicationParameters = new CommSet[163];   //Array that contains all de info's of all connections - USELESS

        public static UAVStatus[] UAVinfo = new UAVStatus[163];     //From this array we'll know the state of every UAV to pass to map 
        public int CounterUAV = 0;
 
        public CommunicationLinks()
        {
            InitializeComponent();
           
            LoadData();
        }

        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();

        //Connecting with the DataBase
        private void SetConnetcion()
        {
            sql_con = new SQLiteConnection("Data Source=CommunicationLinks.db;Version=3;New=False;Compress=True");
        }

        //Set executequery (add, update or delete some information from DataBase)
        private void ExecuteQuery(string txtQuery)
        {
            SetConnetcion();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQuery;
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }

        //Loading DataBase (brings the database set in the files to the application when starts running)
        private void LoadData()
        {
            SetConnetcion();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            string CommandText = "select * from tbcomm";  //tbcomm is name of the database
            DB = new SQLiteDataAdapter(CommandText, sql_con);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            dtvCommunication.DataSource = DT;

            foreach(DataRow row in DT.Rows)
            {
                UAVinfo[CounterUAV].N_UAV = row["ConnectionName"].ToString();
                UAVinfo[CounterUAV].CommuName = row["ConnectionName"].ToString();
                UAVinfo[CounterUAV].Type = row["Type"].ToString();
                UAVinfo[CounterUAV].IP = row["IPAddress"].ToString();
                UAVinfo[CounterUAV].Port = row["Port"].ToString();

                CounterUAV +=  1;
            }

            sql_con.Close();
            
        }

        //Button that calls the function which runs the DroneKit script selected
        private void btnConnect_Click(object sender, EventArgs e)
        {
            Run_Script(cbxScript.Text);
        }

        //Event clickon the DataGridView that selects the connection
        private void dtvCommunication_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            txtName.Text = dtvCommunication.SelectedRows[0].Cells[0].Value.ToString();
            cbxType.Text = dtvCommunication.SelectedRows[0].Cells[1].Value.ToString();
            txtIP.Text = dtvCommunication.SelectedRows[0].Cells[2].Value.ToString();
            txtPort.Text = dtvCommunication.SelectedRows[0].Cells[3].Value.ToString();

            int Index = e.RowIndex;
            DataGridViewRow selectedRow = dtvCommunication.Rows[Index];

            //Once you select a row in the table, the global variables are filled with the respective information, and from this, the values are passed to the thread that will connect with DroneKit
            CommName = selectedRow.Cells[0].Value.ToString();
            typee = selectedRow.Cells[1].Value.ToString();
            IpAddress = selectedRow.Cells[2].Value.ToString();
            Portt = selectedRow.Cells[3].Value.ToString();


            int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == CommName);
            txtLat.Text = UAVinfo[indexx].Lat;
            txtLon.Text = UAVinfo[indexx].Lon;
            txtAlt.Text = UAVinfo[indexx].Alt;
            txtGs.Text = UAVinfo[indexx].Groundspeed;

        }

        //Uploads a mission to a specific UAV (selects the file and then run the script that does that)
        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.ShowDialog();
            string FilePath = openFile.FileName;  //Getting the full path of the mission file
            FileInfo fi = new FileInfo(FilePath);

            string FileName = fi.Name;
            missionn = @"Missions\" + FileName; //This parameter will be passed to the script so it can know where the mission file is, and then, upload it to the vehicle
            
            Run_Script("upload-mission.py");
            
        }
        
        //Function that runs a DroneKit script, it gets the arguments(such as the communication info and a mission file for example) to pass to Python using ProcessStartInfo
        //It's made by threads, that way you can run more than one script at the same time. Everything that is printed in Phyton is showed in the RichTextBox
        private void Run_Script(string myPythonApp)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                string python = @"C:\Python27\python.exe";
                string mission = missionn;
                string con = typee;
                string ip = IpAddress;
                string port = Portt;
                string arg = "";

                if(myPythonApp =="upload-mission.py") 
                {
                    //In case we're sending a mission
                   arg = myPythonApp + " " + con + " " + ip + " " + port + " " + mission;   //Final String that will passed to Dronekit
                }
                else
                {
                   arg = myPythonApp + " " + con + " " + ip + " " + port;   //Final String that will passed to Dronekit
                }

                ProcessStartInfo psi = new ProcessStartInfo(python, arg);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = true;

                var proc = Process.Start(psi);

                StreamReader sr = proc.StandardOutput;

                while (!proc.HasExited)
                {
                    if (!sr.EndOfStream)
                    {
                        string procOutput = sr.ReadToEnd();
                        this.Invoke(new Action<string>(s => { rtbScript.Text += s; }), procOutput);

                    }
                    else Thread.Sleep(20);
                }
            }));

            thread.Start();
            
        }

        //Does nothing, probably I double clicked on some element from the design and now if a delete this code, my design gives some error
        //Well now it sets the timer -- NOT anymore
        private void CommunicationLinks_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        //Adds to DataBase a communication info 
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string txtQuery = "insert into tbcomm (ConnectionName,Type, IPAddress, Port)values('" + txtName.Text + "','" + cbxType.Text  + "','" + txtIP.Text + "','" + txtPort.Text + "')";
            ExecuteQuery(txtQuery);
            LoadData();

            UAVinfo[CounterUAV].N_UAV = txtName.Text;
            UAVinfo[CounterUAV].CommuName = txtName.Text;
            UAVinfo[CounterUAV].Type = cbxType.Text;
            UAVinfo[CounterUAV].IP = txtIP.Text;
            UAVinfo[CounterUAV].Port = txtPort.Text;

            CounterUAV += 1;

            txtName.Clear();
            txtIP.Clear();
            txtPort.Clear();
        }

        //Updates an info in the DataBase
        private void btnEdit_Click(object sender, EventArgs e)
        {
            string txtQuery = "update tbcomm set ConnectionName='" + txtName.Text + "', Type='" + cbxType.Text + "', IPAddress='" + txtIP.Text + "', Port='" + txtPort.Text + "' where ConnectionName='" + CommName + "' and IPAddress='" + IpAddress + "' ";
            ExecuteQuery(txtQuery);
            LoadData();


            
            txtName.Clear();
            txtIP.Clear();
            txtPort.Clear();
        }

        //Deletes an info from DataBase
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string txtQuery = "delete from tbcomm where ConnectionName='" + txtName.Text + "'";
            ExecuteQuery(txtQuery);
            LoadData();

            txtName.Clear();
            txtIP.Clear();
            txtPort.Clear();
        }

        //The dumbass code(r), I didn't figure out how to update the textbox's that shows the current state of the UAV's in one single function (because of the fact that the script ...
        //that gets the info runs in a thread, and the textboxes belong in the main thread and some conflict is caused because of this) 
        public void UpdatingTextBoxLatitude(string lat)
        {
            if (this.txtLat.InvokeRequired)
            {
                this.txtLat.Invoke(new TextBoxDelegate(UpdatingTextBoxLatitude), new object[] { lat });
            }
            else
            {
                this.txtLat.Text = lat;
            }
        }

        public void UpdatingTextBoxLongitude(string lon)
        {
            if (this.txtLon.InvokeRequired)
            {
                this.txtLon.Invoke(new TextBoxDelegate(UpdatingTextBoxLongitude), new object[] { lon });
            }
            else
            {
                this.txtLon.Text = lon;
            }
        }

        public void UpdatingTextBoxaltitude(string alt)
        {
            if (this.txtAlt.InvokeRequired)
            {
                this.txtAlt.Invoke(new TextBoxDelegate(UpdatingTextBoxaltitude), new object[] { alt });
            }
            else
            {
                this.txtAlt.Text = alt;
            }
        }

        public void UpdatingTextBoxGroundspeed(string gs)
        {
            if (this.txtGs.InvokeRequired)
            {
                this.txtGs.Invoke(new TextBoxDelegate(UpdatingTextBoxGroundspeed), new object[] { gs });
            }
            else
            {
                this.txtGs.Text = gs;
            }
        }

        //Launches the UAV and gets the current state of it (latitude, longitude etc)
        private void btnLaunch_Click(object sender, EventArgs e)
        {
            System.Timers.Timer aTimer = new System.Timers.Timer(2500);
            aTimer.Elapsed += new ElapsedEventHandler(TimerCall);
            aTimer.Enabled = true;
            aTimer.AutoReset = true;
 
        }

        public void TimerCall(object sender, ElapsedEventArgs e)
        {
            
            for(int j=0;j<= CounterUAV; j++)
            {
                
                string con = UAVinfo[j].Type;
                string ip = UAVinfo[j].IP;
                string port = UAVinfo[j].Port;

                UAVEstate(con, ip, port);
                Thread.Sleep(200);
            }

            Thread.Sleep(200);
        }

        public  void UAVEstate(string t,string i,string p)
        {

                
                string script = "UAV_Current_State.py";
                string python = @"C:\Python27\python.exe";

                string arg = "";

                arg = script + " " + t + " " + i + " " + p;   //Final String that will be passed to Dronekit

                ProcessStartInfo psi = new ProcessStartInfo(python, arg);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = true;

                var proc = Process.Start(psi);

                StreamReader sr = proc.StandardOutput;

                while (!proc.HasExited)
                {
                    if (!sr.EndOfStream)
                    {
                        string procOutput = sr.ReadToEnd();

                        string delimiter = ", ";
                        string[] StateList = procOutput.Split(delimiter.ToCharArray());

                        string latitude = StateList[0];
                        string longitude = StateList[2];
                        string altitude = StateList[4];
                        string groundspeed = StateList[6];

                        int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == CommName);
                        UAVinfo[indexx].Lat = latitude;         //Convert to decimal or whatever later
                        UAVinfo[indexx].Lon = longitude;
                        UAVinfo[indexx].Alt = altitude;
                        UAVinfo[indexx].Groundspeed = groundspeed;

                        //UpdatingTextBoxLatitude(latitude);
                        //UpdatingTextBoxLongitude(longitude);
                        //UpdatingTextBoxaltitude(altitude);
                        //UpdatingTextBoxGroundspeed(groundspeed);
                    }
                     Thread.Sleep(200);

            }

        }
        //Does nothing, probably I double clicked on some element from the design and now if a delete this code, my design gives some error
        private void rtbScript_TextChanged(object sender, EventArgs e)
        {

        }

        public void ShowCounter()
        {
            txtCounter.Text = CounterUAV.ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ShowCounter();
        }

        
    }
    
}
