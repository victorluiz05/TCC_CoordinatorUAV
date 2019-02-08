using GMap.NET;
using System;
using System.Drawing;
using System.Windows.Forms;
using static CoordinatorMap.Utils;

namespace CoordinatorMap
{
    public partial class MapSetup : Form
    {
        public CoordinatorMap MapControl;

        public MapSetup()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PointLatLng position = new PointLatLng();
            float[] cellSize = new float[2];

            position.Lat = ParseDouble(textBox1.Text);
            position.Lng = ParseDouble(textBox2.Text);

            cellSize[0] = ParseFloat(textBox4.Text);
            cellSize[1] = ParseFloat(textBox3.Text);

            panel1.Visible = false;

            MapControl = new CoordinatorMap(position, cellSize);
            MapControl.TopLevel = false;
            MapControl.Visible = true;
            Controls.Add(MapControl);
            MapControl.Location = new Point(0, 0);
            MapControl.ClientSize = ClientSize;
        }

        public new Size ClientSize
        {
            get
            {
                return base.ClientSize;
            }

            set
            {
                base.ClientSize = value;
                if (MapControl != null) MapControl.ClientSize = value;
            }
        }
    }
}
