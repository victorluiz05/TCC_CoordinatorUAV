using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CoordinatorMap.Grid;
using static CoordinatorMap.Utils;

namespace CoordinatorMap
{
    public partial class CoordinatorMap : Form
    {
        public CoordinatorMap(PointLatLng position, float[] cellSize)
        {
            CellSize = cellSize;
            GridCellsHandler = new GridHandler(CellSize);

            MapPosition = position;

            InitializeComponent();
            
            Map.DragButton = MouseButtons.Left;
            Map.ShowCenter = false; // Remove the center "plus" sign


            // Define the possible UAV colors:
            UavColors.Add(Color.FromArgb(0, 230, 153));
            UavColors.Add(Color.FromArgb(89, 0, 179));
            UavColors.Add(Color.FromArgb(230, 0, 172));
            UavColors.Add(Color.FromArgb(204, 204, 0));
            UavColors.Add(Color.FromArgb(255, 128, 0));

            UavColors = UavColors.OrderBy(a => Guid.NewGuid()).ToList(); // Shuffle the list of colors


            UAV.StartAnimationThread(Uavs, UavsListMutex);
        }



        // Set up the Map when it is completely loaded:
        private void Map_Load(object sender, EventArgs e)
        {
            Map.Overlays.Add(Overlay);
            Map.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            Map.Position = MapPosition;


            // Set the proportion between meters and pixels:
            Map.MouseWheelZoomEnabled = false;

            _meterPixelProportion = new double[5];

            PointLatLng[] points = new PointLatLng[] { MapPosition, new PointLatLng(MapPosition.Lat + 0.001, MapPosition.Lng) };
            GMapRoute route = new GMapRoute(points, "test route");
            double distance = Distance(points[0], points[1]);

            for (int i = 0; i < 5; i++)
            {
                Map.Zoom = 17 + i;
                Overlay.Routes.Add(route);
                _meterPixelProportion[i] = distance / Math.Abs(route.LocalPoints[1].Y - route.LocalPoints[0].Y);
                Overlay.Routes.Remove(route);
            }

            Map.Zoom = 19;

            Map.MouseWheelZoomEnabled = true;


            // Initialize the Grid:
            double dx = ClientSize.Width / 2 * MeterPixelProportion,
                    dy = ClientSize.Height / 2 * MeterPixelProportion;

            PointLatLng temp1 = NewLLPoint(MapPosition, -dx, -dy, MapPosition.Lat);
            PointLatLng temp2 = NewLLPoint(MapPosition, dx, dy, MapPosition.Lat);
            GridCoordinates = new double[] { temp1.Lat, temp2.Lat, temp1.Lng, temp2.Lng };
            InitGrid();


            MapHasLoaded = true;
        }

        private double[] _meterPixelProportion;
        private double MeterPixelProportion // Distance in meters divided by distance in pixels
        {
            get
            {
                return _meterPixelProportion[(int)Map.Zoom - 17];
            }
        }

        private void Map_MouseMove(object sender, MouseEventArgs e) { CorrectPosition(); }
        private void Map_MouseLeave(object sender, EventArgs e) { CorrectPosition(); }

        private void CorrectPosition()
        {
            double k = MeterPixelProportion;

            double  x = ClientSize.Width / 2 * k,
                    y = ClientSize.Height / 2 * k;

            PointLatLng currPos = Map.Position;

            bool flag = false;

            PointLatLng minXPoint = NewLLPoint(new PointLatLng(currPos.Lat, GridCoordinates[2]), x, 0),
                        maxXPoint = NewLLPoint(new PointLatLng(currPos.Lat, GridCoordinates[3]), -x, 0),
                        minYPoint = NewLLPoint(new PointLatLng(GridCoordinates[0], currPos.Lng), 0, y),
                        maxYPoint = NewLLPoint(new PointLatLng(GridCoordinates[1], currPos.Lng), 0, -y);

            if (currPos.Lat > maxYPoint.Lat)
            {
                currPos.Lat = maxYPoint.Lat;
                flag = true;
            }

            if (currPos.Lat < minYPoint.Lat)
            {
                currPos.Lat = minYPoint.Lat;
                flag = true;
            }

            if (currPos.Lng > maxXPoint.Lng)
            {
                currPos.Lng = maxXPoint.Lng;
                flag = true;
            }

            if (currPos.Lng < minXPoint.Lng)
            {
                currPos.Lng = minXPoint.Lng;
                flag = true;
            }

            if (flag) Map.Position = currPos;
        }

        private void InitGrid()
        {
            PointLatLng first = new PointLatLng(GridCoordinates[1], GridCoordinates[2]);

            while (first.Lat > GridCoordinates[0])
            {
                while (first.Lng < GridCoordinates[3])
                {
                    DrawCell(first);
                    first = NewLLPoint(first, CellSize[0], 0, MapPosition.Lat);
                }

                GridCoordinates[3] = first.Lng;

                first.Lng = GridCoordinates[2];
                first = NewLLPoint(first, 0, -CellSize[1], MapPosition.Lat);
            }
            GridCoordinates[0] = first.Lat;
        }

        private UAV AddUav(int id)
        {
            UAV uav = new UAV(id, this, _nextUavColor);

            UavsListMutex.WaitOne();
            Uavs.Add(uav);
            UavsListMutex.ReleaseMutex();


            ToolStripMenuItem uavMenuItem = new ToolStripMenuItem();
            uavMenuItem.Text = "UAV " + id;


            return uav;
        }

        public UAV GetUavById(int id) {
            UAV uav = Uavs.Find(x => x.Id == id);

            return uav != null ? uav : AddUav(id);
        }

        // Everytime a mission changes, it must pass through this:
        public void MissionChanged(UAV uav, List<PointLatLng> waypointsLL)
        {
            // Expand the Grid if necessary:
            double[] newGridCoordinates = new double[] { GridCoordinates[0], GridCoordinates[1], GridCoordinates[2], GridCoordinates[3] };
            foreach (PointLatLng i in waypointsLL)
            {
                if (i.Lat < newGridCoordinates[0])
                    newGridCoordinates[0] = NewLLPoint(i, 0, -CellSize[1] / 2, MapPosition.Lat).Lat;
                else if (i.Lat > newGridCoordinates[1])
                    newGridCoordinates[1] = NewLLPoint(i, 0, CellSize[1] / 2, MapPosition.Lat).Lat;
                if (i.Lng < newGridCoordinates[2])
                    newGridCoordinates[2] = NewLLPoint(i, -CellSize[0] / 2, 0, MapPosition.Lat).Lng;
                else if (i.Lng > newGridCoordinates[3])
                    newGridCoordinates[3] = NewLLPoint(i, CellSize[0] / 2, 0, MapPosition.Lat).Lng;
            }
            RefreshGrid(newGridCoordinates);

            uav.WaypointsLL = waypointsLL;

            GridCellsHandler.AddWaypoints(uav); // Link the uav's waypoints to Grid cells
        }

        // Draw a cell from left to right:
        private void DrawCell(PointLatLng first)
        {
            List<PointLatLng> points = new List<PointLatLng>();

            points.Add(first);
            points.Add(NewLLPoint(points[0], CellSize[0], 0, MapPosition.Lat));
            points.Add(NewLLPoint(points[0], CellSize[0], -CellSize[1], MapPosition.Lat));
            points.Add(NewLLPoint(points[0], 0, -CellSize[1], MapPosition.Lat));

            GridCellsHandler.InsertCell(points[0]);

            DrawCellPolygon(new GMapPolygon(points, "a grid cell"));
        }

        // Draw a cell from right to left:
        private void DrawCell1(PointLatLng first)
        {
            List<PointLatLng> points = new List<PointLatLng>();

            points.Add(first);
            points.Add(NewLLPoint(points[0], -CellSize[0], 0, MapPosition.Lat));
            points.Add(NewLLPoint(points[0], -CellSize[0], -CellSize[1], MapPosition.Lat));
            points.Add(NewLLPoint(points[0], 0, -CellSize[1], MapPosition.Lat));

            GridCellsHandler.InsertCell(points[1]);

            DrawCellPolygon(new GMapPolygon(points, "a grid cell"));
        }

        private void DrawCellPolygon(GMapPolygon cell)
        {
            cell.Stroke = new Pen(Color.FromArgb(32, 255, 255, 255), 2);
            cell.Fill = new SolidBrush(Color.Empty);
            Overlay.Polygons.Add(cell);
        }

        // Expand the Grid:
        private void RefreshGrid(double[] newGridCoordinates)
        {
            PointLatLng first = new PointLatLng(GridCoordinates[1], GridCoordinates[2]);
            while (first.Lat < newGridCoordinates[1])
            {
                first = NewLLPoint(first, 0, CellSize[1], MapPosition.Lat);
                while (first.Lng < GridCoordinates[3])
                {
                    DrawCell(first);
                    first = NewLLPoint(first, CellSize[0], 0, MapPosition.Lat);
                }
                first.Lng = GridCoordinates[2];
            }
            GridCoordinates[1] = first.Lat;


            first = new PointLatLng(GridCoordinates[0], GridCoordinates[2]);
            while (first.Lat > newGridCoordinates[0])
            {
                while (first.Lng < GridCoordinates[3])
                {
                    DrawCell(first);
                    first = NewLLPoint(first, CellSize[0], 0, MapPosition.Lat);
                }
                first.Lng = GridCoordinates[2];
                first = NewLLPoint(first, 0, -CellSize[1], MapPosition.Lat);
            }
            GridCoordinates[0] = first.Lat;


            first = new PointLatLng(GridCoordinates[1], GridCoordinates[2]);
            while (first.Lat > GridCoordinates[0])
            {
                while (first.Lng > newGridCoordinates[2])
                {
                    DrawCell1(first);
                    first = NewLLPoint(first, -CellSize[0], 0, MapPosition.Lat);
                }
                newGridCoordinates[2] = first.Lng;
                first.Lng = GridCoordinates[2];
                first = NewLLPoint(first, 0, -CellSize[1], MapPosition.Lat);
            }
            GridCoordinates[2] = newGridCoordinates[2];


            first = new PointLatLng(GridCoordinates[1], GridCoordinates[3]);
            while (first.Lat > GridCoordinates[0])
            {
                while (first.Lng < newGridCoordinates[3])
                {
                    DrawCell(first);
                    first = NewLLPoint(first, CellSize[0], 0, MapPosition.Lat);
                }
                newGridCoordinates[3] = first.Lng;
                first.Lng = GridCoordinates[3];
                first = NewLLPoint(first, 0, -CellSize[1], MapPosition.Lat);
            }
            GridCoordinates[3] = newGridCoordinates[3];
        }

        // Leave the application when this form is closed:
        private void UAVCoordinator_FormClosed(object sender, FormClosedEventArgs e) { Environment.Exit(0); }



        private bool MapHasLoaded = false;
        internal GMapOverlay Overlay { get; } = new GMapOverlay("overlay");
        internal GridHandler GridCellsHandler { get; }
        private double[] GridCoordinates;
        private PointLatLng MapPosition;
        public float[] CellSize { get; }
        private List<UAV> Uavs = new List<UAV>();

        private Mutex UavsListMutex = new Mutex();
        internal Mutex UavPositionMutex { get; } = new Mutex();
        internal Mutex UavAngleMutex { get; } = new Mutex();


        private List<Color> UavColors = new List<Color>();

        private int LastUavColor = -1;
        private Color _nextUavColor
        {
            get
            {
                if (++LastUavColor >= UavColors.Count) LastUavColor = 0;
                return UavColors[LastUavColor];
            }
        }


        private int _selectedUavId = -1; // Index of the selected uav. -1 means none
        public int SelectedUavId
        {
            get
            {
                return _selectedUavId;
            }

            set
            {
                if (_selectedUavId != -1) GetUavById(_selectedUavId).Selected = false;

                _selectedUavId = value;

                if (_selectedUavId != -1) GetUavById(_selectedUavId).Selected = true;
            }
        }
    }
}