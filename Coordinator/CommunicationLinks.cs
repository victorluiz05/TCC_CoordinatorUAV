using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Data.SQLite;
using System.Timers;
using System.Net.Sockets;
using System.Text;
using GMap.NET;
using static CoordinatorMap.Utils;

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
        private Logger log = new Logger("CommunicationLinks");

        public struct UAVStatus
        {
            //public string CommuName;
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
            public string CurrentWP;

        }

        public struct MissionInfo
        {
            public string MissionName;
            public string MissionPath;
        }

        public struct WPLatLon
        {
            public string Lat;
            public string Lon;
        }

        public MissionInfo[] MissionList = new MissionInfo[163];
        public int CounterMission = 0;

        public UAVStatus[] UAVinfo = new UAVStatus[163];     //From this array we'll know the state of every UAV to pass to map 
        public int CounterUAV = 0;

        public CommunicationLinks()
        {
            map = new CoordinatorMap.MapSetup();
            map.TopLevel = false;
            map.Visible = true;

            InitializeComponent();

            map.ClientSize = panel3.ClientSize;
            panel3.Controls.Add(map);

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
                //UAVinfo[CounterUAV].CommuName = row["ConnectionName"].ToString();
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
            int Index = e.RowIndex;
            if (Index < 0 || Index >= dtvCommunication.RowCount - 1) return;
            dtvCommunication.ClearSelection();
            dtvCommunication.Rows[Index].Selected = true;

            txtName.Text = dtvCommunication.Rows[Index].Cells[0].Value.ToString();
            cbxType.Text = dtvCommunication.Rows[Index].Cells[1].Value.ToString();
            txtIP.Text = dtvCommunication.Rows[Index].Cells[2].Value.ToString();
            txtPort.Text = dtvCommunication.Rows[Index].Cells[3].Value.ToString();

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

            if (map.MapControl != null) map.MapControl.SelectedUavId = indexx;
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

            if (dtvCommunication.RowCount < 2) return;

            if (map.MapControl == null)
            {
                MessageBox.Show("You must set up the map before uploading a mission", "No map settings applied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int indexx = Array.FindIndex(MissionList, s => s.MissionName == ltbDemands.SelectedItem.ToString());
            missionn = @MissionList[indexx].MissionPath; //This parameter will be passed to the script so it can know where the mission file is, and then, upload it to the vehicle

            int indexUAV = Array.FindIndex(UAVinfo, s => s.N_UAV == CommName);
            if (indexUAV == -1)
            {
                MessageBox.Show("You must select an UAV before adding a mission", "No UAV selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Run_Script("upload-mission.py");

            map.MapControl.MissionChanged(
                map.MapControl.GetUavById(indexUAV),
                GetWpList(MissionList[indexx].MissionPath)
            );

            map.MapControl.SelectedUavId = indexUAV;
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

            if (CounterUAV > 0) dtvCommunication_CellClick(this, new DataGridViewCellEventArgs(0, 0));
        }

        //Adds to DataBase a communication info 
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string txtQuery = "insert into tbcomm (ConnectionName,Type, IPAddress, Port)values('" + txtName.Text + "','" + cbxType.Text + "','" + txtIP.Text + "','" + txtPort.Text + "')";
            ExecuteQuery(txtQuery);
            LoadData();

            UAVinfo[CounterUAV].N_UAV = txtName.Text;
            //UAVinfo[CounterUAV].CommuName = txtName.Text;
            UAVinfo[CounterUAV].Type = cbxType.Text;
            UAVinfo[CounterUAV].IP = txtIP.Text;
            UAVinfo[CounterUAV].Port = txtPort.Text;

            CounterUAV += 1;

            txtName.Clear();
            txtIP.Clear();
            txtPort.Clear();

            dtvCommunication_CellClick(this, new DataGridViewCellEventArgs(0, dtvCommunication.RowCount - 2));
        }

        //Updates an info in the DataBase
        private void btnEdit_Click(object sender, EventArgs e)
        {
            string txtQuery = "update tbcomm set ConnectionName='" + txtName.Text + "', Type='" + cbxType.Text + "', IPAddress='" + txtIP.Text + "', Port='" + txtPort.Text + "' where ConnectionName='" + CommName + "' and IPAddress='" + IpAddress + "' ";
            ExecuteQuery(txtQuery);
            LoadData();

            int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == CommName);
            UAVinfo[indexx].N_UAV = txtName.Text;
            //UAVinfo[indexx].CommuName = txtName.Text;
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
            int indexUAV = Array.FindIndex(UAVinfo, s => s.N_UAV == CommName);
            if (map.MapControl != null) map.MapControl.RemoveUav(map.MapControl.GetUavById(indexUAV));

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

        public void UpdatingTextBoxCurrentWP(string currentwp)
        {
            if (this.txtWP.InvokeRequired)
            {
                this.txtWP.Invoke(new TextBoxDelegate(UpdatingTextBoxCurrentWP), new object[] { currentwp });
            }
            else
            {
                this.txtWP.Text = currentwp;
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
                string name = UAVinfo[j].N_UAV;

                UAVEstate(con, ip, port, name);
                //Thread.Sleep(200);
            }

            Thread.Sleep(100);
        }

        public void UAVEstate(string t, string i, string p, string name)
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

                    const string delimiter = ", ";
                    string[] StateList = procOutput.Split(delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    try
                    {
                        string latitude = StateList[0];
                        string longitude = StateList[1];
                        string altitude = StateList[2];
                        string groundspeed = StateList[3];
                        string heading = StateList[4];
                        string currentwp = StateList[5];

                        double lat = ParseDouble(latitude);
                        double lng = ParseDouble(longitude);

                        int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == name);
                        UAVinfo[indexx].Lat = latitude;         //Convert to decimal or whatever later
                        UAVinfo[indexx].Lon = longitude;
                        UAVinfo[indexx].Alt = altitude;
                        UAVinfo[indexx].Groundspeed = groundspeed;
                        UAVinfo[indexx].Heading = heading;
                        UAVinfo[indexx].CurrentWP = currentwp;

                        if ((UAVinfo[indexx].Lat == "None") || (UAVinfo[indexx].Lon == "None"))
                        {
                            //Can't do Nothing
                        }
                        else
                        {
                            map.MapControl.GetUavById(indexx).CurrentPosition = new PointLatLng(ParseDouble(UAVinfo[indexx].Lat), ParseDouble(UAVinfo[indexx].Lon));
                        }


                        int ind = Array.FindIndex(UAVinfo, s => s.N_UAV == CommName);
                        string latt = UAVinfo[ind].Lat;
                        string lonn = UAVinfo[ind].Lon;
                        string altt = UAVinfo[ind].Alt;
                        string groundd = UAVinfo[ind].Groundspeed;
                        string currentwpp = UAVinfo[ind].CurrentWP;

                        UpdatingTextBoxLatitude(latt);
                        UpdatingTextBoxLongitude(lonn);
                        UpdatingTextBoxaltitude(altt);
                        UpdatingTextBoxGroundspeed(groundd);
                        UpdatingTextBoxCurrentWP(currentwpp);

                    }
                    catch (FormatException e) { log.WriteLog(e, "Invalid coordinates: " + procOutput); }
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

            ltbDemands.SelectedIndex = CounterMission - 1;
        }

        Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // Creating the listener channel on the server side and implemetation of threads when therer is more than one GCS
        public void Connection_Handler()
        {
            /*
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
            */
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
            Environment.Exit(0);
        }

        private void btnStartMission_Click(object sender, EventArgs e)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                string python = @"C:\Python27\python.exe";
                string myPythonApp = "script-arm-takeoff-and-auto.py";
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

        public List<PointLatLng> GetWpList(string FileName)
        {
            List<PointLatLng> Listawp = new List<PointLatLng>();

            string[] lines = File.ReadAllLines(FileName);    //Creating an array for every line in the text file
            WPLatLon[] info = new WPLatLon[lines.Length];    //Creating an array of structs that will contain the waypoint info(Lat,Lon)
            var array = new string[lines.Length];    //Creating an array that stores information from each column of each row

            for (int i = 1; i < lines.Length; i++)
            {
                array = lines[i].Split(new string[] { "	" }, StringSplitOptions.RemoveEmptyEntries);    //Adding info in each column of each line
            }

            for (int con = 1; con < lines.Length; con++)
            {
                //Writing in each struct variable of each position of the info array the values received by the text file
                array = lines[con].Split(new string[] { "	" }, StringSplitOptions.RemoveEmptyEntries);    
                info[con].Lat = array[8];
                info[con].Lon = array[9];
                
            }

            

            for(int j=1;j<info.Length;j++) //// Fiz alterações aqui. Revise o que eu fiz ;)
            {

                //Debug.WriteLine(info[j].Lat + " " + info[j].Lon);

                Listawp.Add(new PointLatLng(ParseDouble(info[j].Lat), ParseDouble(info[j].Lon)));

            }

            return Listawp;
        }


    }
    
}
