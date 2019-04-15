using CoordinatorMap;

namespace Coordinator
{
    partial class CommunicationLinks
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommunicationLinks));
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtBase = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.cbxType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dtvCommunication = new System.Windows.Forms.DataGridView();
            this.btnConnect = new System.Windows.Forms.Button();
            this.rtbScript = new System.Windows.Forms.RichTextBox();
            this.cbxScript = new System.Windows.Forms.ComboBox();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnLaunch = new System.Windows.Forms.Button();
            this.txtLat = new System.Windows.Forms.TextBox();
            this.txtLon = new System.Windows.Forms.TextBox();
            this.txtAlt = new System.Windows.Forms.TextBox();
            this.txtGs = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.txtQueue = new System.Windows.Forms.TextBox();
            this.dtvDemands = new System.Windows.Forms.DataGridView();
            this.Latitude = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Longitude = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Estate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label11 = new System.Windows.Forms.Label();
            this.btnLoadMission = new System.Windows.Forms.Button();
            this.ltbDemands = new System.Windows.Forms.ListBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.btnReturn = new System.Windows.Forms.Button();
            this.btnStartMission = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.txtWP = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtUAVAutomataEstate = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txttestelon = new System.Windows.Forms.TextBox();
            this.txttestelat = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.btntest = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtvCommunication)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtvDemands)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtBase);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnEdit);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.cbxType);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtName);
            this.panel1.Controls.Add(this.txtPort);
            this.panel1.Controls.Add(this.txtIP);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(763, 624);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(528, 122);
            this.panel1.TabIndex = 19;
            // 
            // txtBase
            // 
            this.txtBase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtBase.Location = new System.Drawing.Point(359, 37);
            this.txtBase.Name = "txtBase";
            this.txtBase.Size = new System.Drawing.Size(136, 20);
            this.txtBase.TabIndex = 28;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(261, 40);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(34, 13);
            this.label16.TabIndex = 29;
            this.label16.Text = "Base:";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(434, 70);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(88, 23);
            this.btnDelete.TabIndex = 27;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(345, 70);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(79, 23);
            this.btnEdit.TabIndex = 26;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(264, 70);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 25;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // cbxType
            // 
            this.cbxType.FormattingEnabled = true;
            this.cbxType.Items.AddRange(new object[] {
            "udp",
            "tcp"});
            this.cbxType.Location = new System.Drawing.Point(114, 44);
            this.cbxType.Name = "cbxType";
            this.cbxType.Size = new System.Drawing.Size(135, 21);
            this.cbxType.TabIndex = 21;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Type:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Connection Name:";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtName.Location = new System.Drawing.Point(114, 15);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(135, 20);
            this.txtName.TabIndex = 1;
            // 
            // txtPort
            // 
            this.txtPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtPort.Location = new System.Drawing.Point(360, 11);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(135, 20);
            this.txtPort.TabIndex = 2;
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(114, 76);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(135, 20);
            this.txtIP.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "IP Address:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(262, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Port:";
            // 
            // dtvCommunication
            // 
            this.dtvCommunication.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dtvCommunication.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtvCommunication.Location = new System.Drawing.Point(763, 432);
            this.dtvCommunication.MultiSelect = false;
            this.dtvCommunication.Name = "dtvCommunication";
            this.dtvCommunication.ReadOnly = true;
            this.dtvCommunication.Size = new System.Drawing.Size(528, 160);
            this.dtvCommunication.TabIndex = 20;
            this.dtvCommunication.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtvCommunication_CellClick);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(135, 216);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(233, 23);
            this.btnConnect.TabIndex = 21;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // rtbScript
            // 
            this.rtbScript.Location = new System.Drawing.Point(432, 441);
            this.rtbScript.Name = "rtbScript";
            this.rtbScript.Size = new System.Drawing.Size(325, 305);
            this.rtbScript.TabIndex = 22;
            this.rtbScript.Text = "";
            this.rtbScript.TextChanged += new System.EventHandler(this.rtbScript_TextChanged);
            // 
            // cbxScript
            // 
            this.cbxScript.FormattingEnabled = true;
            this.cbxScript.Items.AddRange(new object[] {
            "script-arm-takeoff-and-auto.py",
            "vehicle_stateC.py",
            "UAV_Current_State.py"});
            this.cbxScript.Location = new System.Drawing.Point(135, 189);
            this.cbxScript.Name = "cbxScript";
            this.cbxScript.Size = new System.Drawing.Size(233, 21);
            this.cbxScript.TabIndex = 23;
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(297, 3);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(122, 23);
            this.btnUpload.TabIndex = 24;
            this.btnUpload.Text = "Upload Mission";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnLaunch
            // 
            this.btnLaunch.Location = new System.Drawing.Point(31, 12);
            this.btnLaunch.Name = "btnLaunch";
            this.btnLaunch.Size = new System.Drawing.Size(244, 23);
            this.btnLaunch.TabIndex = 25;
            this.btnLaunch.Text = "Start Telemetry";
            this.btnLaunch.UseVisualStyleBackColor = true;
            this.btnLaunch.Click += new System.EventHandler(this.btnLaunch_Click);
            // 
            // txtLat
            // 
            this.txtLat.Location = new System.Drawing.Point(82, 58);
            this.txtLat.Name = "txtLat";
            this.txtLat.Size = new System.Drawing.Size(144, 20);
            this.txtLat.TabIndex = 26;
            // 
            // txtLon
            // 
            this.txtLon.Location = new System.Drawing.Point(341, 61);
            this.txtLon.Name = "txtLon";
            this.txtLon.Size = new System.Drawing.Size(144, 20);
            this.txtLon.TabIndex = 27;
            // 
            // txtAlt
            // 
            this.txtAlt.Location = new System.Drawing.Point(82, 87);
            this.txtAlt.Name = "txtAlt";
            this.txtAlt.Size = new System.Drawing.Size(144, 20);
            this.txtAlt.TabIndex = 28;
            // 
            // txtGs
            // 
            this.txtGs.Location = new System.Drawing.Point(341, 90);
            this.txtGs.Name = "txtGs";
            this.txtGs.Size = new System.Drawing.Size(144, 20);
            this.txtGs.TabIndex = 29;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "Latitude:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(246, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Longitude:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 94);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "Altitude (m):";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(232, 97);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(106, 13);
            this.label8.TabIndex = 33;
            this.label8.Text = "Ground Speed (m/s):";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.txtQueue);
            this.panel2.Controls.Add(this.dtvDemands);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.btnLoadMission);
            this.panel2.Controls.Add(this.ltbDemands);
            this.panel2.Controls.Add(this.btnUpload);
            this.panel2.Location = new System.Drawing.Point(3, 289);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(419, 292);
            this.panel2.TabIndex = 37;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(9, 51);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(102, 13);
            this.label15.TabIndex = 60;
            this.label15.Text = "Queue of Demands:";
            // 
            // txtQueue
            // 
            this.txtQueue.Location = new System.Drawing.Point(117, 48);
            this.txtQueue.Name = "txtQueue";
            this.txtQueue.Size = new System.Drawing.Size(144, 20);
            this.txtQueue.TabIndex = 59;
            // 
            // dtvDemands
            // 
            this.dtvDemands.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dtvDemands.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtvDemands.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Latitude,
            this.Longitude,
            this.Estate});
            this.dtvDemands.Location = new System.Drawing.Point(11, 145);
            this.dtvDemands.MultiSelect = false;
            this.dtvDemands.Name = "dtvDemands";
            this.dtvDemands.ReadOnly = true;
            this.dtvDemands.Size = new System.Drawing.Size(387, 120);
            this.dtvDemands.TabIndex = 48;
            // 
            // Latitude
            // 
            this.Latitude.HeaderText = "Latitude";
            this.Latitude.Name = "Latitude";
            this.Latitude.ReadOnly = true;
            // 
            // Longitude
            // 
            this.Longitude.HeaderText = "Longitude";
            this.Longitude.Name = "Longitude";
            this.Longitude.ReadOnly = true;
            // 
            // Estate
            // 
            this.Estate.HeaderText = "Estate";
            this.Estate.Name = "Estate";
            this.Estate.ReadOnly = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 8);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 13);
            this.label11.TabIndex = 47;
            this.label11.Text = "Demands List";
            // 
            // btnLoadMission
            // 
            this.btnLoadMission.Location = new System.Drawing.Point(169, 3);
            this.btnLoadMission.Name = "btnLoadMission";
            this.btnLoadMission.Size = new System.Drawing.Size(122, 23);
            this.btnLoadMission.TabIndex = 45;
            this.btnLoadMission.Text = "Load Mission";
            this.btnLoadMission.UseVisualStyleBackColor = true;
            this.btnLoadMission.Click += new System.EventHandler(this.btnLoadMission_Click);
            // 
            // ltbDemands
            // 
            this.ltbDemands.FormattingEnabled = true;
            this.ltbDemands.Location = new System.Drawing.Point(297, 32);
            this.ltbDemands.Name = "ltbDemands";
            this.ltbDemands.Size = new System.Drawing.Size(119, 43);
            this.ltbDemands.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Location = new System.Drawing.Point(588, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(703, 422);
            this.panel3.TabIndex = 38;
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(135, 163);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(122, 23);
            this.btnPause.TabIndex = 42;
            this.btnPause.Text = "Pause Mission";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnResume
            // 
            this.btnResume.Location = new System.Drawing.Point(263, 163);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(122, 23);
            this.btnResume.TabIndex = 43;
            this.btnResume.Text = "Resume Mission";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            // 
            // btnReturn
            // 
            this.btnReturn.Location = new System.Drawing.Point(391, 163);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(122, 23);
            this.btnReturn.TabIndex = 44;
            this.btnReturn.Text = "Return to Home";
            this.btnReturn.UseVisualStyleBackColor = true;
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // btnStartMission
            // 
            this.btnStartMission.Location = new System.Drawing.Point(7, 163);
            this.btnStartMission.Name = "btnStartMission";
            this.btnStartMission.Size = new System.Drawing.Size(122, 23);
            this.btnStartMission.TabIndex = 45;
            this.btnStartMission.Text = "Start Mission";
            this.btnStartMission.UseVisualStyleBackColor = true;
            this.btnStartMission.Click += new System.EventHandler(this.btnStartMission_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(232, 133);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(92, 13);
            this.label10.TabIndex = 47;
            this.label10.Text = "Current Waypoint:";
            // 
            // txtWP
            // 
            this.txtWP.Location = new System.Drawing.Point(341, 126);
            this.txtWP.Name = "txtWP";
            this.txtWP.Size = new System.Drawing.Size(144, 20);
            this.txtWP.TabIndex = 46;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 130);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(40, 13);
            this.label9.TabIndex = 49;
            this.label9.Text = "Status:";
            // 
            // txtUAVAutomataEstate
            // 
            this.txtUAVAutomataEstate.Location = new System.Drawing.Point(82, 123);
            this.txtUAVAutomataEstate.Name = "txtUAVAutomataEstate";
            this.txtUAVAutomataEstate.Size = new System.Drawing.Size(144, 20);
            this.txtUAVAutomataEstate.TabIndex = 50;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 634);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 13);
            this.label12.TabIndex = 51;
            this.label12.Text = "Test Area";
            // 
            // txttestelon
            // 
            this.txttestelon.Location = new System.Drawing.Point(70, 698);
            this.txttestelon.Name = "txttestelon";
            this.txttestelon.Size = new System.Drawing.Size(144, 20);
            this.txttestelon.TabIndex = 52;
            this.txttestelon.Text = "149.166808";
            // 
            // txttestelat
            // 
            this.txttestelat.Location = new System.Drawing.Point(70, 660);
            this.txttestelat.Name = "txttestelat";
            this.txttestelat.Size = new System.Drawing.Size(144, 20);
            this.txttestelat.TabIndex = 53;
            this.txttestelat.Text = "-35.362701";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 663);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(27, 13);
            this.label13.TabIndex = 54;
            this.label13.Text = "LAT";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(9, 701);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(29, 13);
            this.label14.TabIndex = 55;
            this.label14.Text = "LON";
            // 
            // btntest
            // 
            this.btntest.Location = new System.Drawing.Point(231, 673);
            this.btntest.Name = "btntest";
            this.btntest.Size = new System.Drawing.Size(122, 23);
            this.btntest.TabIndex = 56;
            this.btntest.Text = "Test";
            this.btntest.UseVisualStyleBackColor = true;
            this.btntest.Click += new System.EventHandler(this.btntest_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(7, 192);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(122, 23);
            this.btnClear.TabIndex = 57;
            this.btnClear.Text = "Clear Mission";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // CommunicationLinks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1303, 755);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btntest);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txttestelat);
            this.Controls.Add(this.txttestelon);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtUAVAutomataEstate);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtWP);
            this.Controls.Add(this.btnStartMission);
            this.Controls.Add(this.btnReturn);
            this.Controls.Add(this.btnResume);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtGs);
            this.Controls.Add(this.txtAlt);
            this.Controls.Add(this.txtLon);
            this.Controls.Add(this.txtLat);
            this.Controls.Add(this.btnLaunch);
            this.Controls.Add(this.cbxScript);
            this.Controls.Add(this.rtbScript);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.dtvCommunication);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CommunicationLinks";
            this.Text = "Multi UAV Coordinator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CommunicationLinks_FormClosed);
            this.Load += new System.EventHandler(this.CommunicationLinks_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtvCommunication)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtvDemands)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dtvCommunication;
        private System.Windows.Forms.ComboBox cbxType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox cbxScript;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnLaunch;
        private System.Windows.Forms.TextBox txtLat;
        private System.Windows.Forms.TextBox txtLon;
        private System.Windows.Forms.TextBox txtAlt;
        private System.Windows.Forms.TextBox txtGs;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Button btnReturn;
        private System.Windows.Forms.ListBox ltbDemands;
        private System.Windows.Forms.Button btnLoadMission;
        private System.Windows.Forms.Button btnStartMission;
        private CoordinatorMap.MapSetup map;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtWP;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtUAVAutomataEstate;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txttestelon;
        private System.Windows.Forms.TextBox txttestelat;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btntest;
        public System.Windows.Forms.RichTextBox rtbScript;
        private System.Windows.Forms.DataGridView dtvDemands;
        private System.Windows.Forms.DataGridViewTextBoxColumn Latitude;
        private System.Windows.Forms.DataGridViewTextBoxColumn Longitude;
        private System.Windows.Forms.DataGridViewTextBoxColumn Estate;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtQueue;
        private System.Windows.Forms.TextBox txtBase;
        private System.Windows.Forms.Label label16;
    }
}

