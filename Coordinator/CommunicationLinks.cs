﻿using System;
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
using CoordinateSharp;
using System.Drawing.Drawing2D;

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
        string Basee = "";
        string missionn = "";
        private Logger log = new Logger("CommunicationLinks");

        public struct DistanceCalculus
        {
            public double Distance;
            public int WarehouseNumber;
            public int BaseNumber;
            public string NameUAVAssigned;
            public Coordinate Coord1;
        }

        public DistanceCalculus[] DistanceArray = new DistanceCalculus[10];


        public struct UAVStatus
        {
            public string Type;
            public string IP;
            public string Port;
            public string N_UAV;
            public string N_Warehouse;
            public string Lat;
            public string Lon;
            public string Alt;
            public string Groundspeed;
            public string Heading;
            public string BatteryLevel;
            public string UAVAutomataEstate; //Estates: 'IN FLIGHT' or 'IDLE'
            public string Estate;
            public string CurrentWP;
            public string NumberWpMission;
        }

        public struct Demands
        {
            public double DemandLatitude;
            public double DemandLongitude;
            public int NumberoftheBase;
        }

        public Demands[] DemandsArray = new Demands[163];

        public struct MissionInfo
        {
            public string MissionName;
            public string MissionPath;
        }

        public struct DeliveryBases
        {
            public string DeliveryBaseName;
            public double DeliveyBaseLat;
            public double DeliveyBaseLon;
        }

        public DeliveryBases[] DeliveryBasesArray = new DeliveryBases[4];

        public struct WPLatLon
        {
            public string Lat;
            public string Lon;
        }

        public Bases.Warehouse[] WarehouseArrayComm = new Bases.Warehouse[10];

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

            map.button1.Click += OpenMap;
        }

        //Drawing the locations of the bases on the map
        private void OpenMap(object sender, EventArgs e)
        {
            foreach (Bases.Warehouse i in WarehouseArrayComm)
            {
                Bitmap bmp = new Bitmap(80, 80);
                Graphics g = Graphics.FromImage(bmp);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Pen p = new Pen(Color.Black);
                p.Width = 2;
                g.DrawEllipse(p, 2, 2, bmp.Width - 4, bmp.Height - 4);
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString("W " + i.WarehouseNumber.ToString(), new Font("Arial", 10), Brushes.Black, 0, 0);

                map.MapControl.DrawMarker(new PointLatLng(i.WarehouseLat, i.WarehouseLon), bmp);
            }

            foreach (DeliveryBases i in DeliveryBasesArray)
            {
                Bitmap bmp = new Bitmap(50, 50);
                Graphics g = Graphics.FromImage(bmp);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Pen p = new Pen(Color.Blue);
                p.Width = 1;
                g.DrawEllipse(p, 2, 2, bmp.Width - 4, bmp.Height - 4);
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(i.DeliveryBaseName.ToString(), new Font("Arial", 9), Brushes.Yellow, 0, 0);

                map.MapControl.DrawMarker(new PointLatLng(i.DeliveyBaseLat, i.DeliveyBaseLon), bmp);
            }

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
        int TRAVA = 0;
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

            if (TRAVA == 0)
            {
                foreach (DataRow row in DT.Rows)
                {

                    UAVinfo[CounterUAV].N_UAV = row["ConnectionName"].ToString();
                    UAVinfo[CounterUAV].Type = row["Type"].ToString();
                    UAVinfo[CounterUAV].IP = row["IPAddress"].ToString();
                    UAVinfo[CounterUAV].Port = row["Port"].ToString();
                    UAVinfo[CounterUAV].N_Warehouse = row["Base"].ToString();
                    CounterUAV += 1;
                }
                TRAVA = 1;
            }
            else
            {
                foreach (DataRow row in DT.Rows)
                {

                    UAVinfo[CounterUAV].N_UAV = row["ConnectionName"].ToString();
                    UAVinfo[CounterUAV].Type = row["Type"].ToString();
                    UAVinfo[CounterUAV].IP = row["IPAddress"].ToString();
                    UAVinfo[CounterUAV].Port = row["Port"].ToString();
                    UAVinfo[CounterUAV].N_Warehouse = row["Base"].ToString();

                }
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
            txtBase.Text = dtvCommunication.Rows[Index].Cells[4].Value.ToString();

            DataGridViewRow selectedRow = dtvCommunication.Rows[Index];

            //Once you select a row in the table, the global variables are filled with the respective information, and from this, the values are passed to the thread that will connect with DroneKit
            CommName = selectedRow.Cells[0].Value.ToString();
            typee = selectedRow.Cells[1].Value.ToString();
            IpAddress = selectedRow.Cells[2].Value.ToString();
            Portt = selectedRow.Cells[3].Value.ToString();
            Basee = selectedRow.Cells[4].Value.ToString();

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
            string myPythonApp = "upload-mision-for-allocation.py";
            UploadMission(missionn, typee, IpAddress, Portt, CommName, myPythonApp);

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

            dtvWarehouse.ColumnCount = 3;
            dtvWarehouse.Columns[0].Name = "Name";
            dtvWarehouse.Columns[1].Name = "Latitude";
            dtvWarehouse.Columns[2].Name = "Longitude";

            for (int i = 0; i < WarehouseArrayComm.Length; i++)
            {
                if (WarehouseArrayComm[i].WarehouseLat != 0)
                {
                    dtvWarehouse.Rows.Add("Warehouse " + WarehouseArrayComm[i].WarehouseNumber, WarehouseArrayComm[i].WarehouseLat, WarehouseArrayComm[i].WarehouseLon);
                }

            }

            for (int i = 0; i < DeliveryBasesArray.Length; i++)
            {
                if (DeliveryBasesArray[i].DeliveyBaseLat != 0)
                {
                    dtvWarehouse.Rows.Add(DeliveryBasesArray[i].DeliveryBaseName, DeliveryBasesArray[i].DeliveyBaseLat, DeliveryBasesArray[i].DeliveyBaseLon);
                }

            }


        }

        //Adds to DataBase a communication info 
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string txtQuery = "insert into tbcomm (ConnectionName,Type, IPAddress, Port, Base)values('" + txtName.Text + "','" + cbxType.Text + "','" + txtIP.Text + "','" + txtPort.Text + "','" + txtBase.Text + "')";
            ExecuteQuery(txtQuery);
            LoadData();

            UAVinfo[CounterUAV].N_UAV = txtName.Text;
            UAVinfo[CounterUAV].Type = cbxType.Text;
            UAVinfo[CounterUAV].IP = txtIP.Text;
            UAVinfo[CounterUAV].Port = txtPort.Text;
            UAVinfo[CounterUAV].N_Warehouse = txtBase.Text;
            UAVinfo[CounterUAV].UAVAutomataEstate = "IDLE";

            CounterUAV += 1;

            txtName.Clear();
            txtIP.Clear();
            txtPort.Clear();
            txtBase.Clear();

            dtvCommunication_CellClick(this, new DataGridViewCellEventArgs(0, dtvCommunication.RowCount - 2));
        }

        //Updates an info in the DataBase
        private void btnEdit_Click(object sender, EventArgs e)
        {
            string txtQuery = "update tbcomm set ConnectionName='" + txtName.Text + "', Type='" + cbxType.Text + "', IPAddress='" + txtIP.Text + "', Port='" + txtPort.Text + "', Base='" + txtBase.Text + "' where ConnectionName='" + CommName + "' and IPAddress='" + IpAddress + "' ";
            ExecuteQuery(txtQuery);
            LoadData();

            int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == CommName);
            UAVinfo[indexx].N_UAV = txtName.Text;
            //UAVinfo[indexx].CommuName = txtName.Text;
            UAVinfo[indexx].Type = cbxType.Text;
            UAVinfo[indexx].IP = txtIP.Text;
            UAVinfo[indexx].Port = txtPort.Text;
            UAVinfo[indexx].N_Warehouse = txtBase.Text;


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

            CounterUAV = CounterUAV - 1;
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
            /*
            if (this.txtWP.InvokeRequired)
            {
                this.txtWP.Invoke(new TextBoxDelegate(UpdatingTextBoxCurrentWP), new object[] { currentwp });
            }
            else
            {
                this.txtWP.Text = currentwp;
            }
            */
        }

        //Update the estate of the UAV in the respective textbox
        public void UpdatingTextBoxUAVAutoamata(string estate)
        {
            txtUAVAutomataEstate.BeginInvoke(new MethodInvoker(() =>
            {
                if (estate == "IN FLIGHT")
                {
                    this.txtUAVAutomataEstate.Text = "\tIN FLIGHT";
                    this.txtUAVAutomataEstate.BackColor = Color.Orange;
                }
                else if(estate == "PAUSED")
                {
                    this.txtUAVAutomataEstate.Text = "\tPAUSED";
                    this.txtUAVAutomataEstate.BackColor = Color.Red;
                }else
                {
                    this.txtUAVAutomataEstate.Text = "\tIDLE";
                    this.txtUAVAutomataEstate.BackColor = Color.Green;
                }
            }));

        }

        public void UpdatingTextBoxBatteryLevel(string batterylevel)
        {
            if (this.txtBattery.InvokeRequired)
            {
                this.txtBattery.Invoke(new TextBoxDelegate(UpdatingTextBoxBatteryLevel), new object[] { batterylevel });
            }
            else
            {
                this.txtBattery.Text = batterylevel;
            }
        }

        //Launches the UAV and gets the current state of it (latitude, longitude etc)
        private void btnLaunch_Click_1(object sender, EventArgs e)
        {
            aTimer.Elapsed += new ElapsedEventHandler(TimerCall);
            aTimer.Enabled = true;
            aTimer.AutoReset = true;

            aTimer2.Elapsed += new ElapsedEventHandler(TimerCall2);
            aTimer2.Enabled = true;
            aTimer2.AutoReset = true;
            
            aTimer3.Elapsed += new ElapsedEventHandler(TimerCall3);
            aTimer3.Enabled = true;
            aTimer3.AutoReset = true;
            
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
            ReturnToHome(typee, IpAddress, Portt, CommName);
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear_Mission(typee, IpAddress, Portt, CommName);
        }

        //Queue used for the demands
        Queue DemandsQueueWarehouse = new Queue();
        public Demands DemandInfoWarehouse;

        private void btnGenerateDemand_Click(object sender, EventArgs e)
        {
            if (cbxDemand.Text == "Delivery Base 1")
            {
                DemandInfoWarehouse.DemandLatitude = DeliveryBasesArray[0].DeliveyBaseLat;
                DemandInfoWarehouse.DemandLongitude = DeliveryBasesArray[0].DeliveyBaseLon;
                DemandInfoWarehouse.NumberoftheBase = 1;
                
            }
            else if (cbxDemand.Text == "Delivery Base 2")
            {
                DemandInfoWarehouse.DemandLatitude = DeliveryBasesArray[1].DeliveyBaseLat;
                DemandInfoWarehouse.DemandLongitude = DeliveryBasesArray[1].DeliveyBaseLon;
                DemandInfoWarehouse.NumberoftheBase = 2;
                
            }
            else if (cbxDemand.Text == "Delivery Base 3")
            {
                DemandInfoWarehouse.DemandLatitude = DeliveryBasesArray[2].DeliveyBaseLat;
                DemandInfoWarehouse.DemandLongitude = DeliveryBasesArray[2].DeliveyBaseLon;
                DemandInfoWarehouse.NumberoftheBase = 3;
                
            }
            else if (cbxDemand.Text == "Delivery Base 4")
            {
                DemandInfoWarehouse.DemandLatitude = DeliveryBasesArray[3].DeliveyBaseLat;
                DemandInfoWarehouse.DemandLongitude = DeliveryBasesArray[3].DeliveyBaseLon;
                DemandInfoWarehouse.NumberoftheBase = 4;
                
            }
            DemandsQueueWarehouse.Enqueue(DemandInfoWarehouse);
            txtQueue.Text = DemandsQueueWarehouse.Count.ToString();
        }

        /*---------------------------------------------------------------END of Form Methods and Settings-----------------------------------------------------------------------------------------*/

        /*---------------------------------------------------------------General Methods and Settings-----------------------------------------------------------------------------------------*/

        public void Warehouses_Initializer(Bases.Warehouse[] WarehouseArray)
        {
            WarehouseArrayComm = WarehouseArray;

            //Counting how many UAVs are in each base
            for (int i = 0; i < WarehouseArrayComm.Length; i++)
            {
                for (int j = 0; j < CounterUAV; j++)
                {
                    if (WarehouseArrayComm[i].WarehouseNumber == Convert.ToInt32(UAVinfo[j].N_Warehouse))
                    {
                        WarehouseArrayComm[i].NumberUAV = WarehouseArrayComm[i].NumberUAV + 1;
                    }
                }
            }

            //Adding the coordinates of the delivery bases to the array of delivery bases
            DeliveryBasesArray[0].DeliveyBaseLat = -35.341583;
            DeliveryBasesArray[0].DeliveyBaseLon = 149.135051;
            DeliveryBasesArray[0].DeliveryBaseName = "DB 1";
            DeliveryBasesArray[1].DeliveyBaseLat = -35.339708;
            DeliveryBasesArray[1].DeliveyBaseLon = 149.123068;
            DeliveryBasesArray[1].DeliveryBaseName = "DB 2";
            DeliveryBasesArray[2].DeliveyBaseLat = -35.335846;
            DeliveryBasesArray[2].DeliveyBaseLon = 149.131441;
            DeliveryBasesArray[2].DeliveryBaseName = "DB 3";
            DeliveryBasesArray[3].DeliveyBaseLat = -35.342576;
            DeliveryBasesArray[3].DeliveyBaseLon = 149.131159;
            DeliveryBasesArray[3].DeliveryBaseName = "DB 4";
            //DeliveryBasesArray[4].DeliveyBaseLat = -35.336118;
            //DeliveryBasesArray[4].DeliveyBaseLon = 149.124383;
            //DeliveryBasesArray[4].DeliveryBaseName = "DB 5";

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

        System.Timers.Timer aTimer = new System.Timers.Timer(3000);   //Creation of the timer

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
                        string battery = StateList[6];

                        double lat = ParseDouble(latitude);
                        double lng = ParseDouble(longitude);

                        int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == name);
                        UAVinfo[indexx].Lat = latitude;
                        UAVinfo[indexx].Lon = longitude;
                        UAVinfo[indexx].Alt = altitude;
                        UAVinfo[indexx].Groundspeed = groundspeed;
                        UAVinfo[indexx].Heading = heading;
                        UAVinfo[indexx].CurrentWP = currentwp;
                        UAVinfo[indexx].BatteryLevel = battery;

                        /*
                        if ((UAVinfo[indexx].Lat == "None") || (UAVinfo[indexx].Lon == "None"))
                        {
                            //Can't do Nothing
                        }
                        else
                        {
                            map.MapControl.GetUavById(indexx).CurrentPosition = new PointLatLng(ParseDouble(UAVinfo[indexx].Lat), ParseDouble(UAVinfo[indexx].Lon));
                        }
                        */
                        BeginInvoke(new MethodInvoker(() => 
                        {
                            MapRefresh(indexx);
                        }));
                        if (ParseDouble(UAVinfo[indexx].Alt) > 2.0)
                        {
                            UAVinfo[indexx].UAVAutomataEstate = "IN FLIGHT";
                        }

                        if ((UAVinfo[indexx].UAVAutomataEstate == "IN FLIGHT") && ((ParseDouble(UAVinfo[indexx].Alt) <= 2.0)))
                        {
                            
                            if (Convert.ToInt32(UAVinfo[indexx].CurrentWP) == Convert.ToInt32(UAVinfo[indexx].NumberWpMission))
                            {
                                UAVinfo[indexx].UAVAutomataEstate = "IDLE";
                                UAV_arrived(UAVinfo[indexx].N_Warehouse);
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
                        string batteryy = UAVinfo[ind].BatteryLevel;

                        UpdatingTextBoxLatitude(latt);
                        UpdatingTextBoxLongitude(lonn);
                        UpdatingTextBoxaltitude(altt);
                        UpdatingTextBoxGroundspeed(groundd);
                        UpdatingTextBoxCurrentWP(currentwpp);
                        UpdatingTextBoxUAVAutoamata(estate);
                        UpdatingTextBoxBatteryLevel(batteryy);
                        
                    }
                    catch (FormatException e) { log.WriteLog(e, "Invalid coordinates: " + procOutput); }

                } Thread.Sleep(150);
            }
        }
        //Refreshing the UAVs on the map
        public void MapRefresh(int j)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                if ((UAVinfo[j].Lat == "None") || (UAVinfo[j].Lon == "None"))
            {
                //Can't do Nothing
            }
            else
            {
                map.MapControl.GetUavById(j).CurrentPosition = new PointLatLng(ParseDouble(UAVinfo[j].Lat), ParseDouble(UAVinfo[j].Lon));
            }
            }));

            thread.Start();
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
            int indexUAV = Array.FindIndex(UAVinfo, s => s.Port == Portt);
            UAVinfo[indexUAV].UAVAutomataEstate = "PAUSED";
        }

        public void Loiter(string typee, string IpAddress, string Portt)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                string python = @"C:\Python27\python.exe";
                string myPythonApp = "Loiter.py";
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
            int indexUAV = Array.FindIndex(UAVinfo, s => s.Port == Portt);
            UAVinfo[indexUAV].UAVAutomataEstate = "IDLE";
        }

        //Method that sends the UAV to the base
        public void ReturnToHome(string typee, string IpAddress, string Portt, string Namee)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                string python = @"C:\Python27\python.exe";
                string myPythonApp = "ReturnToHome.py";
                string con = typee;
                string ip = IpAddress;
                string port = Portt;
                string name = Namee;
                string arg = "";

                arg = myPythonApp + " " + con + " " + ip + " " + port + " " + name;   //Final String that will passed to Dronekit

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

        System.Timers.Timer aTimer2 = new System.Timers.Timer(2500);   //Creation of the timer
        public void TimerCall2(object sender, ElapsedEventArgs e)
        {
            CalculatingDistances();
        }

        Demands DemandInfoExtract;
        int TRAVA2 = 0;
        public void CalculatingDistances()
        {
            List<DistanceCalculus> ListDistance = new List<DistanceCalculus>();
            DistanceCalculus distances = new DistanceCalculus();

            if (DemandsQueueWarehouse.Count != 0)
            {
                for (int i = 0; i < CounterUAV; i++)
                {
                    if (UAVinfo[i].UAVAutomataEstate == "IDLE")
                    {
                        if(TRAVA2 == 0)//Dequeue only on the first iteration, in the next ones, the value are already storage at the struct
                        {
                            DemandInfoExtract = (Demands)DemandsQueueWarehouse.Dequeue();
                            TRAVA2 = 1;
                        }
                        
                        Coordinate coord2 = new Coordinate(DemandInfoExtract.DemandLatitude, DemandInfoExtract.DemandLongitude);
                        Coordinate coord1 = new Coordinate(Double.Parse(UAVinfo[i].Lat, CultureInfo.InvariantCulture), Double.Parse(UAVinfo[i].Lon, CultureInfo.InvariantCulture));
                        Distance d = new Distance(coord1, coord2);
                        
                        distances.Distance = d.Meters;
                        distances.BaseNumber = DemandInfoExtract.NumberoftheBase;
                        distances.WarehouseNumber = Convert.ToInt16(UAVinfo[i].N_Warehouse);
                        distances.NameUAVAssigned = UAVinfo[i].N_UAV;
                        ListDistance.Add(distances);
                        
                    }
                }
                
                if (ListDistance.Count > 0)
                {
                    ListDistance.Sort((x, y) => x.Distance.CompareTo(y.Distance));
                    int warehousenumber = ListDistance[0].WarehouseNumber;
                    string path = @"Missions\" + "CD" + warehousenumber.ToString() + "B" + ListDistance[0].BaseNumber.ToString() + ".txt";
                    int j = Array.FindIndex(UAVinfo, s => s.N_UAV == ListDistance[0].NameUAVAssigned);
                    
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        txtDemandFrom.Text = "Base " + DemandInfoExtract.NumberoftheBase.ToString();
                        txtAssignedTo.Text = UAVinfo[j].N_UAV;
                        txtQueue.Text = DemandsQueueWarehouse.Count.ToString();
                    }));
                    ListDistance.Clear();
                    TRAVA2 = 0;

                    UAVinfo[j].UAVAutomataEstate = "IN FLIGHT";
                    UAV_left(UAVinfo[j].N_Warehouse);
                    initiate_delivery_Ui(path, j);  
                } 
            }
        }

        public void initiate_delivery_Ui(string path, int i)
        {
            string myPythonApp = "upload-mission.py";
            UploadMission(path, UAVinfo[i].Type, UAVinfo[i].IP, UAVinfo[i].Port, UAVinfo[i].N_UAV, myPythonApp);
            Thread.Sleep(1000);
            Fly_UAV(UAVinfo[i].Type, UAVinfo[i].IP, UAVinfo[i].Port, UAVinfo[i].N_UAV);

        }

        public void go_Ui_to_Wj(string path, int i, string numberwarehouse)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                UpdatingDB(i, numberwarehouse);
            }));
            string myPythonApp = "upload-mission.py";
            Thread.Sleep(150);
            UploadMission(path, UAVinfo[i].Type, UAVinfo[i].IP, UAVinfo[i].Port, UAVinfo[i].N_UAV, myPythonApp);
            Thread.Sleep(800);
            Fly_UAV(UAVinfo[i].Type, UAVinfo[i].IP, UAVinfo[i].Port, UAVinfo[i].N_UAV);

        }

        public void UAV_left(string numberwarehouseofuav)
        {
            for (int i = 0; i < WarehouseArrayComm.Length; i++)
            {
                if (WarehouseArrayComm[i].WarehouseNumber == Convert.ToInt32(numberwarehouseofuav))
                {
                    if (WarehouseArrayComm[i].NumberUAV > 2)
                    {
                        WarehouseArrayComm[i].NumberUAV = WarehouseArrayComm[i].NumberUAV - 1;
                    } else
                    {
                        lastbutoneUAV_left(i, numberwarehouseofuav);
                    }

                }
            }
        }

        public void lastbutoneUAV_left(int i, string numberwarehouseofuav)
        {

            if (WarehouseArrayComm[i].NumberUAV > 1)
            {
                WarehouseArrayComm[i].NumberUAV = WarehouseArrayComm[i].NumberUAV - 1;
            } else
            {
                lastUAV_left(i, numberwarehouseofuav);
            }

        }

        public void lastUAV_left(int i, string numberwarehouseofuav)
        {
            WarehouseArrayComm[i].NumberUAV = WarehouseArrayComm[i].NumberUAV - 1;
            request_UAV(numberwarehouseofuav);
        }

        public void UAV_arrived(string numberwarehouseofuav)
        {
            for (int i = 0; i < WarehouseArrayComm.Length; i++)
            {
                if (WarehouseArrayComm[i].WarehouseNumber == Convert.ToInt32(numberwarehouseofuav))
                {
                    WarehouseArrayComm[i].NumberUAV = WarehouseArrayComm[i].NumberUAV + 1;
                }
            }
        }

        Queue RequestsQueue = new Queue();
        Demands RequestInfo = new Demands();
        public void request_UAV(string numberwarehouseofuav)
        {
            int indexWare = Array.FindIndex(WarehouseArrayComm, s => s.WarehouseNumber == Convert.ToInt32(numberwarehouseofuav));
            RequestInfo.DemandLatitude = WarehouseArrayComm[indexWare].WarehouseLat;
            RequestInfo.DemandLongitude = WarehouseArrayComm[indexWare].WarehouseLon;
            RequestInfo.NumberoftheBase = Convert.ToInt32(numberwarehouseofuav);
            RequestsQueue.Enqueue(RequestInfo);

            BeginInvoke(new MethodInvoker(() =>
            {
                txtWareQueue.Text = RequestsQueue.Count.ToString();
            }));
        }

        //UAV allocation between warehouses
        System.Timers.Timer aTimer3 = new System.Timers.Timer(3000);
        public void TimerCall3(object sender, ElapsedEventArgs e)
        {
            AllocatingUAV();
        }

        Demands RequestInfoExtract = new Demands();
        int TRAVA3 = 0;
        public void AllocatingUAV()
        {
            List<DistanceCalculus> ListDistanceWarehouse = new List<DistanceCalculus>();
            DistanceCalculus distancesWarehouse = new DistanceCalculus();

            if (RequestsQueue.Count != 0)
            {
                
                for (int i = 0; i < CounterUAV; i++)
                {
                    if ((UAVinfo[i].UAVAutomataEstate == "IDLE"))
                    {
                       string possiblewareahouse =  UAVinfo[i].N_Warehouse;
                       int indexWare = Array.FindIndex(WarehouseArrayComm, s => s.WarehouseNumber == Convert.ToInt32(possiblewareahouse));
                       if (WarehouseArrayComm[indexWare].NumberUAV >= 3)
                       {
                            if(TRAVA3 == 0)
                            {
                                RequestInfoExtract = (Demands)RequestsQueue.Dequeue();
                                TRAVA3 = 1;
                            }
                            Coordinate coord2 = new Coordinate(RequestInfoExtract.DemandLatitude, RequestInfoExtract.DemandLongitude);
                            Coordinate coord1 = new Coordinate(Double.Parse(UAVinfo[i].Lat, CultureInfo.InvariantCulture), Double.Parse(UAVinfo[i].Lon, CultureInfo.InvariantCulture));
                            Distance d = new Distance(coord1, coord2);

                            distancesWarehouse.Distance = d.Meters;
                            distancesWarehouse.BaseNumber = RequestInfoExtract.NumberoftheBase;
                            distancesWarehouse.WarehouseNumber = Convert.ToInt16(UAVinfo[i].N_Warehouse);
                            distancesWarehouse.NameUAVAssigned = UAVinfo[i].N_UAV;
                            ListDistanceWarehouse.Add(distancesWarehouse);
                       }
                    }
                }

                if (ListDistanceWarehouse.Count > 0)
                {
                    ListDistanceWarehouse.Sort((x, y) => x.Distance.CompareTo(y.Distance));
                    int warehousenumber = ListDistanceWarehouse[0].WarehouseNumber;
                    string path = @"Missions\" + "CD" + warehousenumber.ToString() + "CD" + ListDistanceWarehouse[0].BaseNumber.ToString() + ".txt";
                    int j = Array.FindIndex(UAVinfo, s => s.N_UAV == ListDistanceWarehouse[0].NameUAVAssigned);

                    BeginInvoke(new MethodInvoker(() =>
                    {
                        txtRequest.Text = "Warehouse " + RequestInfoExtract.NumberoftheBase.ToString();
                        txtServed.Text = UAVinfo[j].N_UAV;
                        txtWareQueue.Text = DemandsQueueWarehouse.Count.ToString();
                    }));
                    UAV_left(UAVinfo[j].N_Warehouse);
                    go_Ui_to_Wj(path, j, ListDistanceWarehouse[0].BaseNumber.ToString());
                    ListDistanceWarehouse.Clear();
                    TRAVA3 = 0;
                }
            }
            
        }
        
        public void UpdatingDB(int j, string numberwarehouse)
        {

                int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == UAVinfo[j].N_UAV);
                txtName.Text = UAVinfo[indexx].N_UAV;
                txtIP.Text = UAVinfo[indexx].IP;
                txtBase.Text = numberwarehouse;

                string txtQuery = "update tbcomm set Base='" + txtBase.Text + "' where ConnectionName='" + txtName.Text + "' and IPAddress='" + txtIP.Text + "' ";
                ExecuteQuery(txtQuery);
                LoadData();

                UAVinfo[indexx].N_Warehouse = numberwarehouse;

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

                for (int j = 1; j < info.Length; j++) //// Fiz alterações aqui. Revise o que eu fiz ;)
                {

                    //Debug.WriteLine(info[j].Lat + " " + info[j].Lon);

                    Listawp.Add(new PointLatLng(ParseDouble(info[j].Lat), ParseDouble(info[j].Lon)));

                }

                return Listawp;
            }

        //Method that uploads the mission on the UAV
        public void UploadMission(string mission, string con, string ip, string port, string namee, string myPythonApp)
            {
                var thread = new Thread(new ThreadStart(() =>
                {
                    string python = @"C:\Python27\python.exe";
                    myPythonApp = "upload-mission.py";
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
                    int indexx = Array.FindIndex(UAVinfo, s => s.N_UAV == namee);
                    UAVinfo[indexx].UAVAutomataEstate = "IN FLIGHT";

                    string python = @"C:\Python27\python.exe";
                    string myPythonApp = "StartMission.py";
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

        public void Land(string con, string ip, string port, string namee)
            {
                var thread = new Thread(new ThreadStart(() =>
                {
                    string python = @"C:\Python27\python.exe";
                    string myPythonApp = "Land.py";
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

        //Method that changes the automata estate 
        public void Automata_Estate_Changer(int indexx, string Automata)
            {
                /*
                switch (Automata)
                {
                    case "UAV":

                        if (UAVinfo[indexx].UAVAutomataEstate == "IDLE")
                        {
                            UAVinfo[indexx].UAVAutomataEstate = "IN FLIGHT";
                            UpdatingTextBoxUAVAutoamata(UAVinfo[indexx].UAVAutomataEstate);

                        }
                        else if (UAVinfo[indexx].UAVAutomataEstate == "IN FLIGHT")
                        {
                            UAVinfo[indexx].UAVAutomataEstate = "IDLE";
                            UpdatingTextBoxUAVAutoamata(UAVinfo[indexx].UAVAutomataEstate);

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
                */
            }

            /*---------------------------------------------------------------END of General Methods and Settings-----------------------------------------------------------------------------------------*/

        }
    } 

