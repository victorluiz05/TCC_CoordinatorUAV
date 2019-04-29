using GMap.NET;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using static CoordinatorMap.Utils;

namespace CoordinatorMap
{
    public partial class MapSetup : Form
    {
        public CoordinatorMap MapControl;

        public MapSetup()
        {
            InitializeComponent();

            try
            {
                string[] cache = File.ReadAllText(@"Cache\mapsetup").Split(' ');
                textBox1.Text = cache[0];
                textBox2.Text = cache[1];
                textBox3.Text = cache[2];
                textBox4.Text = cache[3];
            }
            catch (FileNotFoundException) { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PointLatLng position = new PointLatLng();
            float[] cellSize = new float[2];
            
            position.Lat = ParseDouble(textBox1.Text);
            position.Lng = ParseDouble(textBox2.Text);

            cellSize[0] = ParseFloat(textBox4.Text);
            cellSize[1] = ParseFloat(textBox3.Text);

            Directory.CreateDirectory(@"Cache");
            File.WriteAllText(@"Cache\mapsetup",
                textBox1.Text + " " + textBox2.Text + " " + textBox3.Text + " " + textBox4.Text);

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

        public void DrawMarker(PointLatLng pointLatLng, Bitmap bmp)
        {
            throw new NotImplementedException();
        }
    }
}
