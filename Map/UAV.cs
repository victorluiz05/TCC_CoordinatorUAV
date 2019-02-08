using System;
using System.Collections.Generic;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using static UAVCoordinator.Utils;

namespace UAVCoordinator
{
    public class UAV
    {
        private Bitmap _uavMarkerBitmap;
        private float _currentAngle = 0;
        public int Sysid { get; }
        private Color _color;
        public Waypoint FirstWaypoint;
        private PointLatLng _currentPosition;
        public float Altitude;
        private GMapMarker _uavMarker;
        private List<PointLatLng> _waypointsLL;
        private List<GMapPolygon> _polygons = new List<GMapPolygon>();
        private List<GMapPolygon> _trailPolygons = new List<GMapPolygon>();
        private UAVCoordinator _coordinator;
        private List<GMapMarker> _waypointMarkers = new List<GMapMarker>();
        private int _currentWaypoint = 0;

        public PointLatLng CurrentPosition
        {
            get
            {
                return _currentPosition;
            }

            set
            {
                PointLatLng oldPos = _currentPosition;

                _currentPosition = value;

                if (_uavMarker == null)
                {
                    DrawUAVMarker();
                }
                else
                {
                    if (!_uavMarker.IsVisible)
                        _uavMarker.IsVisible = true;
                    double deltaTheta = DeltaTheta(oldPos, _currentPosition, -_currentAngle);
                    RotateUAVMarker((float)deltaTheta);
                    _uavMarker.Position = _currentPosition;
                }

                if (Selected) DrawTrail();
            }
        }

        private void DrawUAVMarker()
        {
            Bitmap bitmap = new Bitmap(46, 50);
            Graphics flagGraphics = Graphics.FromImage(bitmap);
            flagGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            Point p1 = new Point(0, 3), p2 = new Point(46, 25), p3 = new Point(5, 21);
            Point p4 = new Point(p1.X, 50 - p1.Y), p5 = new Point(p3.X, 50 - p3.Y);
            Point p6 = new Point(3, 25);

            flagGraphics.FillPolygon(Brushes.White, new Point[] { p1, p2, p3 });
            flagGraphics.FillPolygon(Brushes.White, new Point[] { p4, p2, p5 });
            flagGraphics.FillPolygon(new SolidBrush(Color.FromArgb(230, 230, 230)), new Point[] { p6, p3, p2, p5 });
            flagGraphics.DrawLine(new Pen(Color.FromArgb(220, 220, 220)), p3, p2);
            flagGraphics.DrawLine(new Pen(Color.FromArgb(220, 220, 220)), p5, p2);
            flagGraphics.DrawLine(new Pen(Color.FromArgb(210, 210, 210)), p6, p2);

            flagGraphics.DrawPolygon(new Pen(_color), new Point[] { p1, p3, p6, p5, p4, p2 });

            _uavMarkerBitmap = bitmap;


            bitmap = new Bitmap(66, 66);
            flagGraphics = Graphics.FromImage(bitmap);
            flagGraphics.DrawImage(_uavMarkerBitmap, new Point(10, 8));

            _uavMarker = new GMarkerGoogle(_currentPosition, bitmap);
            _uavMarker.Offset = new Point(-33, -33);
            _uavMarker.IsVisible = false;
            _coordinator.Overlay.Markers.Add(_uavMarker);
        }

        private void RotateUAVMarker(float angle)
        {
            _currentAngle = (_currentAngle - angle) % 360;

            Bitmap newBitmap = new Bitmap(66, 66);
            Graphics flagGraphics = Graphics.FromImage(newBitmap);

            flagGraphics.TranslateTransform(33, 33);
            flagGraphics.RotateTransform(_currentAngle);
            flagGraphics.TranslateTransform(-33, -33);
            flagGraphics.DrawImage(_uavMarkerBitmap, new Point(10, 8));

            _coordinator.Overlay.Markers.Remove(_uavMarker);
            _uavMarker = new GMarkerGoogle(_currentPosition, newBitmap);
            _uavMarker.Offset = new Point(-33, -33);
            _coordinator.Overlay.Markers.Add(_uavMarker);
        }

        public List<PointLatLng> WaypointsLL
        {
            get
            {
                return _waypointsLL;
            }

            set
            {
                ClearTheStage();
                _coordinator.GridCellsHandler.RemoveWaypointsR(FirstWaypoint);

                _waypointsLL = value;

                for(int i = 0; i < _waypointsLL.Count - 1; i++)
                {
                    GMapPolygon polygon = new GMapPolygon(_waypointsLL.GetRange(i, 2), "_");
                    polygon.Stroke = new Pen(_color, 3);

                    _coordinator.Overlay.Polygons.Add(polygon);
                    _polygons.Add(polygon);
                }

                int count = 1;
                foreach(PointLatLng i in _waypointsLL)
                {
                    Bitmap markerBitmap = new Bitmap(28, 28);
                    DrawWaypointMarker(markerBitmap, count++);

                    GMapMarker temp = new GMarkerGoogle(i, markerBitmap);
                    temp.Offset = new Point(-14, -14);

                    _waypointMarkers.Add(temp);
                    _coordinator.Overlay.Markers.Add(temp);
                }
            }
        }

        private bool _selected = false;
        public bool Selected
        {
            get
            {
                return _selected;
            }

            set
            {
                _selected = value;

                int count = 0;
                bool flag = false;
                foreach (GMapPolygon i in _polygons)
                {
                    if (count++ >= _currentWaypoint) flag = true;
                    Color newColor = _color;
                    if (_selected && flag) newColor = Color.FromArgb(100, _color);
                    i.Stroke = new Pen(newColor, 3);
                    _coordinator.Overlay.Polygons.Remove(i);
                    _coordinator.Overlay.Polygons.Add(i);
                }

                if (_selected) DrawTrail();
                foreach (GMapPolygon i in _trailPolygons) _coordinator.Overlay.Polygons.Remove(i);
            }
        }

        private void DrawTrail()
        {
            if (_currentPosition == null) return;

            PointLatLng wp1 = _waypointsLL[_currentWaypoint], wp2 = _waypointsLL[_currentWaypoint + 1];

            List<PointLatLng> polygonPoints = new List<PointLatLng>();
            PointLatLng projectedPoint = LinearProjection(wp1, _currentPosition, wp2);
            polygonPoints.Add(wp1);
            polygonPoints.Add(projectedPoint);

            GMapPolygon newPolygon = new GMapPolygon(polygonPoints, "_");
            Pen pen = new Pen(_color, 2);
            newPolygon.Stroke = pen;
            _coordinator.Overlay.Polygons.Add(newPolygon);

            if (_trailPolygons.Count > 0 && newPolygon.Distance < _trailPolygons[_trailPolygons.Count - 1].Distance)
            {
                foreach (GMapPolygon i in _trailPolygons)
                    _coordinator.Overlay.Polygons.Remove(i);
                _trailPolygons = new List<GMapPolygon>();
                _trailPolygons.Add(newPolygon);

                for (int i = 1; i < 6; i++)
                {
                    polygonPoints = new List<PointLatLng>();
                    polygonPoints.Add(wp1);
                    projectedPoint = LinearProjection(wp1, NewLLPoint(_currentPosition, 0.5 * i, 0), wp2);
                    polygonPoints.Add(projectedPoint);
                    newPolygon = new GMapPolygon(polygonPoints, "_");
                    newPolygon.Stroke = pen;
                    _coordinator.Overlay.Polygons.Add(newPolygon);
                    _trailPolygons.Add(newPolygon);
                }
            }
            else
            {
                _trailPolygons.Add(newPolygon);

                int count = _trailPolygons.Count - 20;
                if (count > 0)
                    _trailPolygons.RemoveRange(0, count);
            }
        }

        private void DrawWaypointMarker(Bitmap bitmap, int wpNumber)
        {
            Graphics flagGraphics = Graphics.FromImage(bitmap);
            flagGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            flagGraphics.FillEllipse(new SolidBrush(_color), 1, 1, 26, 26);
            

            StringFormat strFormat = new StringFormat();
            strFormat.LineAlignment = StringAlignment.Center;
            strFormat.Alignment = StringAlignment.Center;

            flagGraphics.DrawString(wpNumber + "", new Font("Arial", 10, FontStyle.Bold), new SolidBrush(Color.White), 14, 14, strFormat);
        }


        public UAV(int sysid, UAVCoordinator coordinator, Color color)
        {
            Sysid = sysid;
            _coordinator = coordinator;
            _color = color;
        }

        private void ClearTheStage()
        {
            foreach (GMapPolygon i in _polygons)
                _coordinator.Overlay.Polygons.Remove(i);
            _polygons = new List<GMapPolygon>();

            foreach (GMapMarker i in _waypointMarkers)
                _coordinator.Overlay.Markers.Remove(i);
            _waypointMarkers = new List<GMapMarker>();
        }

    }
}
