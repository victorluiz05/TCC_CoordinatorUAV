using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Coordinator
{
    public partial class Bases : Form
    {
        public Bases()
        {
            InitializeComponent();
        }

        public struct Base
        {
            public int BaseNumber;
            public double BaseLat;
            public double BaseLon;
            public int NumberUAV;
        }

        public struct Warehouse
        {
            public int WarehouseNumber;
            public double WarehouseLat;
            public double WarehouseLon;
            public int NumberUAV;
        }
        public Base[] BasesArray = new Base[10];

        public Warehouse[] WarehouseArray = new Warehouse[10];

        int Nwarehouses;

        private void btnEnterCoordinates_Click(object sender, EventArgs e)
        {
            Nwarehouses = Convert.ToInt32(txtNumber.Text);

            Label label = new Label();
            label.Name = "Latitude";
            label.Text = "Latitude";
            label.AutoSize = true;
            label.Location = new Point(12, 54);
            Controls.Add(label);

            Label labell = new Label();
            labell.Name = "Longitude";
            labell.Text = "Longitude";
            labell.AutoSize = true;
            labell.Location = new Point(164, 54);
            Controls.Add(labell);

            WarehouseArray[1].WarehouseLat = -35.342437;
            WarehouseArray[1].WarehouseLon = 149.126881;
            WarehouseArray[2].WarehouseLat = -35.338778;
            WarehouseArray[2].WarehouseLon = 149.128005;
            WarehouseArray[3].WarehouseLat = -35.337535;
            WarehouseArray[3].WarehouseLon = 149.135191;

            int xlat = 12;
            int ylat = 82;
            int xlon = 164;
            int ylon = 82;
            
            for (int i=1; i<=Nwarehouses; i++)
            {
                TextBox textboxlat = new TextBox();
                textboxlat.Name = "txtbaseLat" + i.ToString();
                textboxlat.Text = WarehouseArray[i].WarehouseLat.ToString();
                textboxlat.Location = new Point(xlat,ylat);
                Controls.Add(textboxlat);
                ylat = ylat + 25;

                TextBox textboxlon = new TextBox();
                textboxlon.Name = "txtbaseLon" + i.ToString();
                textboxlon.Text = WarehouseArray[i].WarehouseLon.ToString();
                textboxlon.Location = new Point(xlon, ylon);
                Controls.Add(textboxlon);
                ylon = ylon + 25;
            }

        }

        private void btnStartCoord_Click(object sender, EventArgs e)
        {
           

            for (int i = 1; i <= Nwarehouses; i++)
            {
                
                    WarehouseArray[i].WarehouseLat = Convert.ToDouble(((TextBox)Controls["txtbaseLat" + (i).ToString()]).Text);
                    WarehouseArray[i].WarehouseLon = Convert.ToDouble(((TextBox)Controls["txtbaseLon" + (i).ToString()]).Text);
                    WarehouseArray[i].WarehouseNumber = i;
                
                
            }

            CommunicationLinks comm = new CommunicationLinks();
            comm.Warehouses_Initializer(WarehouseArray);

            this.Hide();
            
            comm.Show();
            
        }
    }
}
