using GMap.NET;
using System;
using System.Drawing;
using System.Windows.Forms;
using static CoordinatorMap.Utils;

namespace CoordinatorMap
{
    public partial class MapSetup : Form
    {
        CoordinatorMap map;

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

            map = new CoordinatorMap(position, cellSize);
            map.TopLevel = false;
            map.Visible = true;
            Controls.Add(map);
            map.Location = new Point(0, 0);
            map.ClientSize = ClientSize;
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
                if (map != null) map.ClientSize = value;
            }
        }
    }
}
