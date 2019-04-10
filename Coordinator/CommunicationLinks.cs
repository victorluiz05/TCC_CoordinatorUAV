using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Data.SQLite;
using System.Timers;
using System.Net.Sockets;
using System.Text;
using System.Net;
using GMap.NET;
using static CoordinatorMap.Utils;
using System.Drawing;
using System.Collections;
using System.Linq;


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
            public string Type;
            public string IP;
            public string Port;
            public string N_UAV;
            public string Lat;
            public string Lon;
            public string Alt;
            public string Groundspeed;
            public string Heading;
            public string UAVAutomataEstate; //Estates: 'IN FLIGHT' or 'IDLE'
            public string Estate;
            public string CurrentWP;
            public string NumberWpMission;
        }

        public struct Demands
        {
            public double DemandLatitude;
            public double DemandLongitude;
            public string DemandAutomataEstate; //Estates: 'UNSIGNED', 'ASSIGNED' or 'DELIVERED'
        }

        public Demands[] DemandsArray = new Demands[163];

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

        public MissionInfo[] MissionList = new MissionInfo[163];       //Still used in the upload button
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

        /*---------------------------------------------------------------Database Methods and Settings----------------------------------------------------------------------------------*/

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
                UAVinfo[CounterUAV].Type = row["Type"].ToString();
                UAVinfo[CounterUAV].IP = row["IPAddress"].ToString();
                UAVinfo[CounterUAV].Port = row["Port"].ToString();

                CounterUAV += 1;
            }

            sql_con.Close();

        }

        /*---------------------------------------------------------------END of Database Methods and Settings---------------------------------------------------------------------------*/

        /*---------------------------------------------------------------Form Methods and Settings--------------------------------------------------------------------------------------*/

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
            //Run_Script("upload-mission.py");
            UploadMission(missionn, typee, IpAddress, Portt, CommName);

            /*
            map.MapControl.MissionChanged(
                map.MapControl.GetUavById(indexUAV),
                GetWpList(MissionList[indexx].MissionPath)
            );

            map.MapControl.SelectedUavId = indexUAV;
            */
        }

        private void CommunicationLinks_Load(object sender, EventArgs e)
        {
            //Connection_Handler();


            //Iniciating the coordinator with all UAVs IDLE
            for (int j = 0; j <= CounterUAV; j++)
            {
                UAVinfo[j].UAVAutomataEstate = "IDLE";

            }

            if (CounterUAV > 0) dtvCommunication_CellClick(this, new DataGridViewCellEventArgs(0, 0));

        }

        //Adds to DataBase a communication info 
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string txtQuery = "insert into tbcomm (ConnectionName,Type, IPAddress, Port)values('" + txtName.Text + "','" + cbxType.Text + "','" + txtIP.Text + "','" + txtPort.Text + "')";
            ExecuteQuery(txtQuery);
            LoadData();

            UAVinfo[CounterUAV].N_UAV = txtName.Text;
            UAVinfo[CounterUAV].Type = cbxType.Text;
            UAVinfo[CounterUAV].IP = txtIP.Text;
            UAVinfo[CounterUAV].Port = txtPort.Text;
            UAVinfo[CounterUAV].UAVAutomataEstate = "IDLE";

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
        //Update the value of the latitude of an UAV in the respective textbox
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

        //Update the value of the longitude of an UAV in the respective textbox
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

        //Update the value of the altitude of an UAV in the respective textbox
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

        //Update the value of the groundspeed of an UAV in the respective textbox
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

        //Update the value of the Waypoint that the UAV is going towards in the respective textbox
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

        public void UpdatingTextBoxUAVAutoamata(string estate)
        {
            txtUAVAutomataEstate.BeginInvoke(new MethodInvoker(() =>
            {
                if (estate == "IN FLIGHT")
                {
                    this.txtUAVAutomataEstate.Text = "\t IN FLIGHT";
                    this.txtUAVAutomataEstate.BackColor = Color.Red;
                }
                else
                {
                    this.txtUAVAutomataEstate.Text = "\t IDLE";
                    this.txtUAVAutomataEstate.BackColor = Color.Green;
                }
            }));

        }

        //Launches the UAV and gets the current state of it (latitude, longitude etc)
        private void btnLaunch_Click(object sender, EventArgs e)
        {

            aTimer.Elapsed += new ElapsedEventHandler(TimerCall);
            aTimer.Enabled = true;
            aTimer.AutoReset = true;

            aTimer2.Elapsed += new ElapsedEventHandler(TimerCall2);
            aTimer2.Enabled = true;
            aTimer2.AutoReset = true;

        }

        //Button that pauses the mission of a selected UAV
        private void btnPause_Click(object sender, EventArgs e)
        {
            PauseMission(typee, IpAddress, Portt);
        }

        //Button that sends the UAV back to the mission paused
        private void btnResume_Click(object sender, EventArgs e)
        {
            ResumeMission(typee, IpAddress, Portt);
        }

        //Button that sends the UAV to the Base
        private void btnReturn_Click(object sender, EventArgs e)
        {
            ReturnToHome(typee, IpAddress, Portt);
        }

        //Button that loads a mission from a diretory to coordinator
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

        //Update the listbox of mission
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

        //Ends the socket when the applications is closed
        private void CommunicationLinks_FormClosed(object sender, FormClosedEventArgs e)
        {
            //listenerSocket.Close();
            Environment.Exit(0);
        }

        //Button that send the start command to the UAV to start the mission
        private void btnStartMission_Click(object sender, EventArgs e)
        {

            Fly_UAV(typee, IpAddress, Portt, CommName);

        }

        //Does nothing, probably I double clicked on some element from the design and now if a delete this code, my design gives some error
        public void rtbScript_TextChanged(object sender, EventArgs e)
        {
        }

        private void btntest_Click(object sender, EventArgs e)
        {
            demand_automatic_test(txttestelat.Text, txttestelon.Text);
            txtQueue.Text = DemandsQueue.Count.ToString();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear_Mission(typee,IpAddress,Portt,CommName);
        }

        /*---------------------------------------------------------------END of Form Methods and Settings-----------------------------------------------------------------------------------------*/

        /*---------------------------------------------------------------General Methods and Settings-----------------------------------------------------------------------------------------*/

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

        System.Timers.Timer aTimer = new System.Timers.Timer(2000);   //Creation of the timer

        //Timer that runs the method to get the actual estate of the UAVs
        public void TimerCall(object sender, ElapsedEventArgs e)
        {
            for (int j = 0; j <= CounterUAV; j++)
            {
                string con = UAVinfo[j].Type;
                string ip = UAVinfo[j].IP;
                string port = UAVinfo[j].Port;
                string name = UAVinfo[j].N_UAV;

                UAVEstate(con, ip, port, name);
            }

        }

        //One of the most important method, runs inside a timer e gets the actual estate os each uav listed in the coordinator
        public void UAVEstate(string t, string i, string p, string name)
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
                          UAVinfo[indexx].Lat = latitude;
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

                            //Verifying when the mission is over to set free the UAV
                            if ((UAVinfo[indexx].UAVAutomataEstate == "IN FLIGHT") && ((Convert.ToInt32(UAVinfo[indexx].CurrentWP) == Convert.ToInt32(UAVinfo[indexx].NumberWpMission))))
                            {
                                if (ParseDouble(UAVinfo[indexx].Alt)  <= 2.0)
                                {
                                    Mission_Has_Ended(indexx, "UAV");
                                }

                            }

                            //It must changes when a select a UAV on dtv
                            int ind = Array.FindIndex(UAVinfo, s => s.N_UAV == CommName);
                            string latt = UAVinfo[ind].Lat;
                            string lonn = UAVinfo[ind].Lon;
                            string altt = UAVinfo[ind].Alt;
                            string groundd = UAVinfo[ind].Groundspeed;
                            string currentwpp = UAVinfo[ind].CurrentWP;
                            string estate = UAVinfo[ind].UAVAutomataEstate;

                            UpdatingTextBoxLatitude(latt);
                            UpdatingTextBoxLongitude(lonn);
                            UpdatingTextBoxaltitude(altt);
                            UpdatingTextBoxGroundspeed(groundd);
                            UpdatingTextBoxCurrentWP(currentwpp);
                            UpdatingTextBoxUAVAutoamata(estate);

                      }
                      catch (FormatException e) { log.WriteLog(e, "Invalid coordinates: " + procOutput); }
                   
                }Thread.Sleep(525);
            }           
        }

        //Method for pausing the mission of a select UAV
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

        //Method that sends the UAV back to the mission paused
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

        //Method that sends the UAV to the base
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

        /*
        // Creating the listener channel on the server side and implemetation of threads when there is more than one GCS
        public void Connection_Handler()
        {
            Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var thread = new Thread(new ThreadStart(() =>
            {
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8890);
                listenerSocket.Bind(ipEnd);
                listenerSocket.Listen(0);
                
                while (true)
                {
                    Socket clientSocket = listenerSocket.Accept();
                    //THREADS
                    Thread MPthread;
                    //MPthread = new Thread(() => Request_Received(clientSocket));
                    MPthread = new Thread(() => Demand_Received(clientSocket));
                    MPthread.Start();
                }

            }));

            thread.Start();
            
        }
        
        public void Demand_Received(Socket clientSocket)
        {
            byte[] clientData = new byte[1024 * 5000];

            int receivedBytesLen = clientSocket.Receive(clientData);

            if(receivedBytesLen != 0)
            {
                int fileNameLen = BitConverter.ToInt32(clientData, 0);
                string fileName = Encoding.ASCII.GetString(clientData);
                                
                const string delimiter = ";";
                string[] StateList = fileName.Split(delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                DemandInfo.DemandLatitude = double.Parse(StateList[0], CultureInfo.InvariantCulture);
                DemandInfo.DemandLongitude = double.Parse(StateList[1], CultureInfo.InvariantCulture);
                DemandInfo.DemandAutomataEstate = "UNSIGNED";

                DemandsQueue.Enqueue(DemandInfo);
            }

            PlanningPath();
        }
        */

        //Queue used for the demands
        Queue DemandsQueue = new Queue();
        public Demands DemandInfo;

        public void demand_automatic_test(string lat, string lon)
        {
            DemandInfo.DemandLatitude = double.Parse(lat, CultureInfo.InvariantCulture);
            DemandInfo.DemandLongitude = double.Parse(lon, CultureInfo.InvariantCulture);
            DemandInfo.DemandAutomataEstate = "UNSIGNED";

            DemandsQueue.Enqueue(DemandInfo);


        }

        System.Timers.Timer aTimer2 = new System.Timers.Timer(1000);   //Creation of the timer

        public void TimerCall2(object sender, ElapsedEventArgs e)
        {
            for (int j = 0; j <= CounterUAV; j++)
            {
                string automataestate = UAVinfo[j].UAVAutomataEstate;
                PlanningPath(j, automataestate);
            }

        }

        public void PlanningPath(int j, string automataestate)
        {
            if (DemandsQueue.Count != 0)
            {
                if ((automataestate == "IDLE") && (UAVinfo[j].UAVAutomataEstate == "IDLE"))
                {
                    if (UAVinfo[j].UAVAutomataEstate == "IDLE")
                    {
                        DemandInfo = (Demands)DemandsQueue.Dequeue();
                        DemandsArray[j].DemandLatitude = DemandInfo.DemandLatitude;
                        DemandsArray[j].DemandLongitude = DemandInfo.DemandLongitude;
                        DemandsArray[j].DemandAutomataEstate = DemandInfo.DemandAutomataEstate;


                        Automata_Estate_Changer(j, "DEMAND");
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            txtQueue.Text = DemandsQueue.Count.ToString();
                            //Update_DemandsDTV();
                        }));

                        if (UAVinfo[j].UAVAutomataEstate == "IDLE")
                        {
                            PathPlannigAlgorithm(DemandInfo.DemandLatitude, DemandInfo.DemandLongitude, j);
                        }
                    }

                }

            }
            
        }

        public void DecisionalAlgorithm(string path, int i)
        {
            //Clear_Mission(UAVinfo[i].Type, UAVinfo[i].IP, UAVinfo[i].Port, UAVinfo[i].N_UAV);
            Thread.Sleep(1000);
            UploadMission(path, UAVinfo[i].Type, UAVinfo[i].IP, UAVinfo[i].Port, UAVinfo[i].N_UAV);
            
            Thread.Sleep(1500);
            Fly_UAV(UAVinfo[i].Type, UAVinfo[i].IP, UAVinfo[i].Port, UAVinfo[i].N_UAV);

        }

        //creates a list of waypoints of the mission to draw in the map
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
                
        //Method that uploads the mission on the UAV
        public void UploadMission(string mission, string con, string ip, string port, string namee)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                string python = @"C:\Python27\python.exe";
                string myPythonApp = "upload-mission.py";
                string arg = "";
                
                arg = myPythonApp + " " + con + " " + ip + " " + port + " " + mission;   //Final String that will passed to Dronekit
                
                int indexUAV = Array.FindIndex(UAVinfo, s => s.N_UAV == namee);
                if (indexUAV == -1)
                {
                    MessageBox.Show("You must select an UAV before adding a mission", "No UAV selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Thread.Sleep(200);
                BeginInvoke(new MethodInvoker(() =>
                {
                     map.MapControl.MissionChanged(map.MapControl.GetUavById(indexUAV), GetWpList(mission));
                     map.MapControl.SelectedUavId = indexUAV;
                }));
                
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
                        int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == namee);
                        UAVinfo[indexx].NumberWpMission = procOutput;

                    }
                    else Thread.Sleep(20);
                }
            }));

            thread.Start();
        }

        public void ResetUAV(string con, string ip, string port, string namee)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                string python = @"C:\Python27\python.exe";
                string myPythonApp = "ResetUAV.py";
                string arg = "";

                arg = myPythonApp + " " + con + " " + ip + " " + port;   //Final String that will passed to Dronekit

                int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == namee);

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

        //Event from UAV Automata/Method that sends the command for the UAV start a mission
        public void Fly_UAV(string con, string ip, string port, string namee)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                string python = @"C:\Python27\python.exe";
                string myPythonApp = "StartMission.py";
                string arg = "";

                arg = myPythonApp + " " + con + " " + ip + " " + port;   //Final String that will passed to Dronekit

                int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == namee);

                Automata_Estate_Changer(indexx, "UAV");

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

        public void Clear_Mission(string con, string ip, string port, string namee)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                string python = @"C:\Python27\python.exe";
                string myPythonApp = "ClearMission.py";
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
        
        //Event from UAV Automata
        public void Mission_Has_Ended(int indexx, string Automata)
        {
            //ResetUAV(UAVinfo[indexx].Type, UAVinfo[indexx].IP, UAVinfo[indexx].Port, UAVinfo[indexx].N_UAV);
            Automata_Estate_Changer(indexx, Automata);
        }

        //Event from Demand Automata
        public void Demand_Answered(int indexx, string Automata)
        {
            Automata_Estate_Changer(indexx, Automata);
        }
        
        public void Update_DemandsDTV()
        {
           foreach(Demands k in DemandsArray)
           {
                if ((k.DemandAutomataEstate !="") && (k.DemandLatitude != 0) && (k.DemandLongitude != 0))
                {
                    dtvDemands.Rows.Add(k.DemandLatitude, k.DemandLongitude, k.DemandAutomataEstate);
                }
           }
                 
        }

        //Method that changes the automata estate 
        public void Automata_Estate_Changer(int indexx, string Automata)
        {
            switch (Automata)
            {
                case "UAV":

                    if (UAVinfo[indexx].UAVAutomataEstate == "IDLE")
                    {
                        UAVinfo[indexx].UAVAutomataEstate = "IN FLIGHT";
                        
                    }
                    else if (UAVinfo[indexx].UAVAutomataEstate == "IN FLIGHT")
                    {
                        UAVinfo[indexx].UAVAutomataEstate = "IDLE";
                        
                    }
                    break;

                case "DEMAND":

                    if(DemandsArray[indexx].DemandAutomataEstate == "UNSIGNED")
                    {
                        DemandsArray[indexx].DemandAutomataEstate = "ASSIGNED";
                    }
                    else if(DemandsArray[indexx].DemandAutomataEstate == "ASSIGNED")
                    {
                        DemandsArray[indexx].DemandAutomataEstate = "ANSWERED";
                    }

                    break;
            }

        }

        /*---------------------------------------------------------------END of General Methods and Settings-----------------------------------------------------------------------------------------*/

        double BaseLat = -35.363261;
        double BaseLon = 149.165236;
        double BaseAlt = 584.000000;
        int cont = 1;
        int a = 0;

        public void PathPlannigAlgorithm(double lat, double lon, int i)
        {
            string missioname = "m" + cont.ToString();
            string path = @"Missions\" + missioname + ".txt";

            //if (!File.Exists(path))
            //{
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("QGC WPL 110");
                try
                {
                    sw.WriteLine("0\t1\t0\t16\t0\t0\t0\t0\t" +
                                 double.Parse(BaseLat.ToString()).ToString("0.000000", new CultureInfo("en-US")) +
                                 "\t" +
                                 double.Parse(BaseLon.ToString()).ToString("0.000000", new CultureInfo("en-US")) +
                                 "\t" +
                                 double.Parse(BaseAlt.ToString()).ToString("0.000000", new CultureInfo("en-US")) +
                                 "\t1");
                }
                catch { }

                sw.Write((a + 1)); // seq
                sw.Write("\t" + 0); // current
                sw.Write("\t" + "3"); //frame 
                sw.Write("\t" + "16");
                sw.Write("\t" +
                         double.Parse("0")
                                     .ToString("0.000000", new CultureInfo("en-US")));
                sw.Write("\t" +
                         double.Parse("0")
                             .ToString("0.000000", new CultureInfo("en-US")));
                sw.Write("\t" +
                         double.Parse("0")
                             .ToString("0.000000", new CultureInfo("en-US")));
                sw.Write("\t" +
                         double.Parse("0")
                             .ToString("0.000000", new CultureInfo("en-US")));
                sw.Write("\t" +
                         double.Parse(lat.ToString())
                             .ToString("0.000000", new CultureInfo("en-US")));
                sw.Write("\t" +
                         double.Parse(lon.ToString())
                             .ToString("0.000000", new CultureInfo("en-US")));
                sw.Write("\t" +
                         (double.Parse("50")).ToString("0.000000", new CultureInfo("en-US")));
                sw.Write("\t" + 1);
                sw.WriteLine("");

                cont = cont + 1;
                sw.Close();
                cont = cont + 1;
            }
            cont = cont + 1;
            //}

            
            DecisionalAlgorithm(path, i);


        }

        
    }

}
