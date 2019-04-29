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
        public Base[] BasesArray = new Base[10];

        int Nbases;

        private void btnEnterCoordinates_Click(object sender, EventArgs e)
        {
            Nbases = Convert.ToInt32(txtNumber.Text);

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

            int xlat = 12;
            int ylat = 82;
            int xlon = 164;
            int ylon = 82;
            double lat = -35.345168;
            double lon = 149.130011;

            for (int i=0; i<Nbases; i++)
            {
                TextBox textboxlat = new TextBox();
                textboxlat.Name = "txtbaseLat" + i.ToString();
                textboxlat.Text = lat.ToString();
                textboxlat.Location = new Point(xlat,ylat);
                Controls.Add(textboxlat);
                lat = lat + 0.000950;
                ylat = ylat + 25;

                TextBox textboxlon = new TextBox();
                textboxlon.Name = "txtbaseLon" + i.ToString();
                textboxlon.Text = lon.ToString();
                textboxlon.Location = new Point(xlon, ylon);
                Controls.Add(textboxlon);
                lon = lon + 0.001350;
                ylon = ylon + 25;

            }

        }

        private void btnStartCoord_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Nbases; i++)
            {
                BasesArray[i].BaseLat = Convert.ToDouble(((TextBox)Controls["txtbaseLat" + (i).ToString()]).Text);
                BasesArray[i].BaseLon = Convert.ToDouble(((TextBox)Controls["txtbaseLon" + (i).ToString()]).Text);
                BasesArray[i].BaseNumber = i;
            }

            CommunicationLinks comm = new CommunicationLinks();
            comm.Bases_Initializer(BasesArray);

            this.Hide();
            
            comm.Show();
            
        }
    }
}
