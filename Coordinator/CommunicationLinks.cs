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
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Coordinator
{
    public partial class CommunicationLinks : Form
    {

        delegate void TextBoxDelegate(string message);

        //Global Variables
        string CommName = "";
        string typee = "";
        string IpAddress = "";
        string Portt = "";
        string missionn = "";

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
            public string Heading;
            public bool Occupied;

        }

        public struct MissionInfo
        {
            public string MissionName;
            public string MissionPath;
        }

        public MissionInfo[] MissionList = new MissionInfo[163];
        public int CounterMission = 0;

        public UAVStatus[] UAVinfo = new UAVStatus[163];     //From this array we'll know the state of every UAV to pass to map 
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

            foreach (DataRow row in DT.Rows)
            {
                UAVinfo[CounterUAV].N_UAV = row["ConnectionName"].ToString();
                UAVinfo[CounterUAV].CommuName = row["ConnectionName"].ToString();
                UAVinfo[CounterUAV].Type = row["Type"].ToString();
                UAVinfo[CounterUAV].IP = row["IPAddress"].ToString();
                UAVinfo[CounterUAV].Port = row["Port"].ToString();

                CounterUAV += 1;
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
            /*
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.ShowDialog();
            string FilePath = openFile.FileName;  //Getting the full path of the mission file
            FileInfo fi = new FileInfo(FilePath);

            string FileName = fi.Name;
            missionn = @"Missions\" + FileName; //This parameter will be passed to the script so it can know where the mission file is, and then, upload it to the vehicle
            */

            int indexx = Array.FindIndex(MissionList, s => s.MissionName == ltbDemands.SelectedItem.ToString());
            missionn = @"Missions\" + MissionList[indexx].MissionPath; //This parameter will be passed to the script so it can know where the mission file is, and then, upload it to the vehicle

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

                if (myPythonApp == "upload-mission.py")
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
            Connection_Handler();
        }

        //Adds to DataBase a communication info 
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string txtQuery = "insert into tbcomm (ConnectionName,Type, IPAddress, Port)values('" + txtName.Text + "','" + cbxType.Text + "','" + txtIP.Text + "','" + txtPort.Text + "')";
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

            int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == CommName);
            UAVinfo[indexx].N_UAV = txtName.Text;
            UAVinfo[indexx].CommuName = txtName.Text;
            UAVinfo[indexx].Type = cbxType.Text;
            UAVinfo[indexx].IP = txtIP.Text;
            UAVinfo[indexx].Port = txtPort.Text;


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

        System.Timers.Timer aTimer = new System.Timers.Timer(2000);
        //Launches the UAV and gets the current state of it (latitude, longitude etc)
        private void btnLaunch_Click(object sender, EventArgs e)
        {

            aTimer.Elapsed += new ElapsedEventHandler(TimerCall);
            aTimer.Enabled = true;
            aTimer.AutoReset = true;

        }

        public void TimerCall(object sender, ElapsedEventArgs e)
        {

            for (int j = 0; j <= CounterUAV; j++)
            {

                string con = UAVinfo[j].Type;
                string ip = UAVinfo[j].IP;
                string port = UAVinfo[j].Port;

                UAVEstate(con, ip, port);
                //Thread.Sleep(200);
            }

            Thread.Sleep(100);
        }

        public void UAVEstate(string t, string i, string p)
        {
            //var thread = new Thread(new ThreadStart(() =>
            //{

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
                    string heading = StateList[8];

                    int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == CommName);
                    UAVinfo[indexx].Lat = latitude;         //Convert to decimal or whatever later
                    UAVinfo[indexx].Lon = longitude;
                    UAVinfo[indexx].Alt = altitude;
                    UAVinfo[indexx].Groundspeed = groundspeed;
                    UAVinfo[indexx].Heading = heading;

                    //UpdatingTextBoxLatitude(latitude);
                    //UpdatingTextBoxLongitude(longitude);
                    //UpdatingTextBoxaltitude(altitude);
                    //UpdatingTextBoxGroundspeed(groundspeed);
                }
                Thread.Sleep(100);

            }
            //}));

            //thread.Start();

        }
        //Does nothing, probably I double clicked on some element from the design and now if a delete this code, my design gives some error
        private void rtbScript_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            aTimer.Stop();

        }

        public void PauseMission(string typee, string IpAddress, string Portt)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                string python = @"C:\Python27\python.exe";
                string myPythonApp = "PauseMission.py";
                string con = typee;
                string ip = IpAddress;
                string port = Portt;
                string arg = "";

                arg = myPythonApp + " " + con + " " + ip + " " + port;   //Final String that will passed to Dronekit

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

        public void ResumeMission(string typee, string IpAddress, string Portt)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                string python = @"C:\Python27\python.exe";
                string myPythonApp = "ResumeMission.py";
                string con = typee;
                string ip = IpAddress;
                string port = Portt;
                string arg = "";

                arg = myPythonApp + " " + con + " " + ip + " " + port;   //Final String that will passed to Dronekit

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

        public void ReturnToHome(string typee, string IpAddress, string Portt)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                string python = @"C:\Python27\python.exe";
                string myPythonApp = "ReturnToHome.py";
                string con = typee;
                string ip = IpAddress;
                string port = Portt;
                string arg = "";

                arg = myPythonApp + " " + con + " " + ip + " " + port;   //Final String that will passed to Dronekit

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

        private void btnPause_Click(object sender, EventArgs e)
        {
            PauseMission(typee, IpAddress, Portt);
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            ResumeMission(typee, IpAddress, Portt);
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            ReturnToHome(typee, IpAddress, Portt);
        }

        private void btnLoadMission_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.ShowDialog();
            string FilePath = openFile.FileName;  //Getting the full path of the mission file
            FileInfo fi = new FileInfo(FilePath);

            string FileName = fi.Name;
            missionn = @"Missions\" + FileName; //This parameter will be passed to the script so it can know where the mission file is, and then, upload it to the vehicle

            MissionList[CounterMission].MissionName = FileName;
            MissionList[CounterMission].MissionPath = missionn;

            ltbDemands.Items.Add(MissionList[CounterMission].MissionName);

            CounterMission += 1;


        }

        Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // Creating the listener channel on the server side and implemetation of threads when therer is more than one GCS
        public void Connection_Handler()
        {
            var thread = new Thread(new ThreadStart(() =>
            {

                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
                listenerSocket.Bind(ipEnd);
                listenerSocket.Listen(0);

                while (true)
                {
                    Socket clientSocket = listenerSocket.Accept();
                    //THREADS
                    Thread MPthread;
                    MPthread = new Thread(() => ReceivingMissions(clientSocket));
                    MPthread.Start();
                }


            }));

            thread.Start();
        }

        public void ReceivingMissions(Socket clientSocket)
        {
            byte[] clientData = new byte[1024 * 5000];
            string receivedPath = @"Missions\";
            int receivedBytesLen;

            receivedBytesLen = clientSocket.Receive(clientData);

            if (receivedBytesLen != 0)
            {
                int fileNameLen = BitConverter.ToInt32(clientData, 0);
                string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);
                BinaryWriter bWrite = new BinaryWriter(File.Open(receivedPath + fileName, FileMode.Append)); ;
                bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);

                missionn = @"Missions\" + fileName; //This parameter will be passed to the script so it can know where the mission file is, and then, upload it to the vehicle

                MissionList[CounterMission].MissionName = fileName;
                MissionList[CounterMission].MissionPath = missionn;

                UpdatingTListboxMission(MissionList[CounterMission].MissionName);
                //ltbDemands.Items.Add(MissionList[CounterMission].MissionName);

                CounterMission += 1;

                bWrite.Close();
                clientSocket.Close();

            }


        }

        public void UpdatingTListboxMission(string mission)
        {
            if (this.ltbDemands.InvokeRequired)
            {
                this.ltbDemands.Invoke(new TextBoxDelegate(UpdatingTListboxMission), new object[] { mission });
            }
            else
            {
                this.ltbDemands.Items.Add(mission);
            }
        }

        private void CommunicationLinks_FormClosed(object sender, FormClosedEventArgs e)
        {
            listenerSocket.Close();
        }

        private void btnStartMission_Click(object sender, EventArgs e)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                string python = @"C:\Python27\python.exe";
                string myPythonApp = "AutoCoord.py";
                string con = typee;
                string ip = IpAddress;
                string port = Portt;
                string arg = "";

                arg = myPythonApp + " " + con + " " + ip + " " + port;   //Final String that will passed to Dronekit

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

                int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == CommName);
                UAVinfo[indexx].Occupied = true;



            }));

            thread.Start();
        }

    }
    
}
