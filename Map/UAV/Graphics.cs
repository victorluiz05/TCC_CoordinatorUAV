using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using static CoordinatorMap.Utils;
using FrameStateTuple = System.Tuple<int, double, double, GMap.NET.PointLatLng, GMap.NET.PointLatLng, long>;

namespace CoordinatorMap
{
    public partial class UAV
    {
        private GMapMarker UavMarker;
        private Bitmap UavMarkerBitmap;
        private float _currentAngle = 0;
        public Color UavColor { get; }
        private List<GMapPolygon> WpLines = new List<GMapPolygon>(); // Lines connecting waypoints
        private List<GMapPolygon> TrailLines = new List<GMapPolygon>();
        private List<GMapMarker> WpMarkers = new List<GMapMarker>();



        private float CurrentAngle
        {
            get
            {
                Coordinator.UavAngleMutex.WaitOne();
                float angle = _currentAngle;
                Coordinator.UavAngleMutex.ReleaseMutex();
                return angle;
            }
        }

        private void DrawUAVMarker()
        {
            Bitmap bitmap = new Bitmap(46, 50);
            Graphics flagGraphics = Graphics.FromImage(bitmap);
            flagGraphics.SmoothingMode = SmoothingMode.AntiAlias;


            Point p1 = new Point(0, 3), p2 = new Point(46, 25), p3 = new Point(5, 21);
            Point p4 = new Point(p1.X, 50 - p1.Y), p5 = new Point(p3.X, 50 - p3.Y);
            Point p6 = new Point(3, 25);

            flagGraphics.FillPolygon(Brushes.White, new Point[] { p1, p2, p3 });
            flagGraphics.FillPolygon(Brushes.White, new Point[] { p4, p2, p5 });
            flagGraphics.FillPolygon(new SolidBrush(Color.FromArgb(230, 230, 230)), new Point[] { p6, p3, p2, p5 });
            flagGraphics.DrawLine(new Pen(Color.FromArgb(220, 220, 220)), p3, p2);
            flagGraphics.DrawLine(new Pen(Color.FromArgb(220, 220, 220)), p5, p2);
            flagGraphics.DrawLine(new Pen(Color.FromArgb(210, 210, 210)), p6, p2);

            flagGraphics.DrawPolygon(new Pen(UavColor), new Point[] { p1, p3, p6, p5, p4, p2 });

            UavMarkerBitmap = bitmap;


            bitmap = new Bitmap(66, 66);
            flagGraphics = Graphics.FromImage(bitmap);
            flagGraphics.DrawImage(UavMarkerBitmap, new Point(10, 8));

            UavMarker = new GMarkerGoogle(_currentPosition, bitmap);
            UavMarker.Offset = new Point(-33, -33);
            Coordinator.Overlay.Markers.Add(UavMarker);
        }

        private void RotateUavMarker(float angle)
        {
            Coordinator.UavAngleMutex.WaitOne();

            _currentAngle = (_currentAngle - angle) % 360;

            Bitmap newBitmap = new Bitmap(66, 66);
            Graphics flagGraphics = Graphics.FromImage(newBitmap);

            flagGraphics.TranslateTransform(33, 33);
            flagGraphics.RotateTransform(_currentAngle);
            flagGraphics.TranslateTransform(-33, -33);
            flagGraphics.DrawImage(UavMarkerBitmap, new Point(10, 8));

            Coordinator.Overlay.Markers.Remove(UavMarker);
            UavMarker = new GMarkerGoogle(UavMarker.Position, newBitmap);
            UavMarker.Offset = new Point(-33, -33);
            Coordinator.Overlay.Markers.Add(UavMarker);

            Coordinator.UavAngleMutex.ReleaseMutex();
        }

        private void DrawWaypointMarker(PointLatLng wpLL, int wpNum)
        {
            Bitmap bmp = new Bitmap(28, 28);
            Graphics flagGraphics = Graphics.FromImage(bmp);

            flagGraphics.SmoothingMode = SmoothingMode.AntiAlias;

            flagGraphics.FillEllipse(new SolidBrush(UavColor), 1, 1, 26, 26); // Draw a circle


            // Draw the waypoint number:

            StringFormat strFormat = new StringFormat();
            strFormat.LineAlignment = StringAlignment.Center;
            strFormat.Alignment = StringAlignment.Center;
            
            flagGraphics.DrawString(wpNum == 0 ? "H" : wpNum + "",
                new Font("Calibri", 13, FontStyle.Bold),
                new SolidBrush(Color.White), 14 + 1, 14 + 1 /* +1 is due to Calibri */, strFormat);


            GMapMarker marker = new GMarkerGoogle(wpLL, bmp);
            marker.Offset = new Point(-14, -14);

            WpMarkers.Add(marker);
            Coordinator.Overlay.Markers.Add(marker);
        }

        private void DrawTrail(PointLatLng pos)
        {
            if (!_selected /*|| pos == null*/ || _waypointsLL == null || _waypointsLL.Count <= _currentWpNum + 1) return;

            PointLatLng wp1 = _waypointsLL[_currentWpNum], wp2 = _waypointsLL[_currentWpNum + 1];
            
            PointLatLng projPoint = DistanceProjection(wp1, pos, wp2);
            if (projPoint == wp1)
            {
                foreach (GMapPolygon i in TrailLines) Coordinator.Overlay.Polygons.Remove(i);
                return;
            }

            List<PointLatLng> points = new List<PointLatLng>();
            points.Add(wp1);
            points.Add(projPoint);

            GMapPolygon newLine = new GMapPolygon(points, "a trail polygon");
            Pen trailPen = new Pen(UavColor, 2);
            newLine.Stroke = trailPen;
            Coordinator.Overlay.Polygons.Add(newLine);


            if (TrailLines.Count > 0 && newLine.Distance < TrailLines[TrailLines.Count - 1].Distance)
            {
                foreach (GMapPolygon i in TrailLines) Coordinator.Overlay.Polygons.Remove(i);
                TrailLines = new List<GMapPolygon>();
                TrailLines.Add(newLine);

                double D = Math.Sqrt(Math.Pow(wp2.Lng - wp1.Lng, 2) + Math.Pow(wp2.Lat - wp1.Lat, 2));
                double d = Math.Sqrt(Math.Pow(wp2.Lng - pos.Lng, 2) + Math.Pow(wp2.Lat - pos.Lat, 2));
                double d_wp1 = D - d;
                double delta = 0.001 * d_wp1;

                for (int i = 0; i < 15; i++)
                {
                    d_wp1 -= delta;
                    points = new List<PointLatLng>();
                    points.Add(wp1);
                    points.Add(DistanceProjection(wp1, d_wp1, wp2));
                    newLine = new GMapPolygon(points, "_");
                    newLine.Stroke = trailPen;
                    Coordinator.Overlay.Polygons.Add(newLine);
                    TrailLines.Add(newLine);
                }
            }
            else
            {
                TrailLines.Add(newLine);

                int count = TrailLines.Count - 20;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++) Coordinator.Overlay.Polygons.Remove(TrailLines[i]);
                    TrailLines.RemoveRange(0, count);
                }
            }
        }

        // Remove all drawings relating to the uav, except for its marker:
        private void ClearStage()
        {
            foreach (GMapPolygon i in TrailLines) Coordinator.Overlay.Polygons.Remove(i);

            foreach (GMapPolygon i in WpLines)
                Coordinator.Overlay.Polygons.Remove(i);
            WpLines = new List<GMapPolygon>();

            foreach (GMapMarker i in WpMarkers)
                Coordinator.Overlay.Markers.Remove(i);
            WpMarkers = new List<GMapMarker>();
        }



        private static void Animate(List<UAV> uavs, Mutex uavsListMutex)
        {
            const int FrameDelay = 20;
            const int FramesNum = 20;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Dictionary<UAV, FrameStateTuple> states = new Dictionary<UAV, FrameStateTuple>();


            while (true)
            {
                uavsListMutex.WaitOne();

                foreach (UAV i in uavs)
                {
                    if (!states.ContainsKey(i))
                    {
                        PointLatLng pos = i.CurrentPosition;
                        states.Add(i, new FrameStateTuple(0, 0, 0, pos, pos, 0));
                    }

                    FrameStateTuple state = states[i];

                    if (watch.ElapsedMilliseconds < state.Item6) continue;

                    if (state.Item1 == 0)
                    {
                        PointLatLng newPos = i.CurrentPosition;

                        if (newPos.Equals(state.Item4)) continue;

                        state = new FrameStateTuple(0,
                            Math.Sqrt(Math.Pow(newPos.Lat - state.Item4.Lat, 2) + Math.Pow(newPos.Lng - state.Item4.Lng, 2)) / FramesNum,
                            MovementAngularVariation(state.Item4, newPos, -i.CurrentAngle) / FramesNum,
                            state.Item4,
                            newPos,
                            0);
                    }

                    PointLatLng projPoint = DistanceProjection(state.Item4, state.Item2, state.Item5);

                    i.Coordinator.Map.Invoke(new Action(() =>
                    {
                        i.UavMarker.Position = projPoint;
                        i.RotateUavMarker((float)state.Item3);
                        i.DrawTrail(projPoint);
                    }));

                    states[i] = state.Item1 == FramesNum - 1
                        ? new FrameStateTuple(0, 0, 0, state.Item5, state.Item5, 0)
                        : new FrameStateTuple(state.Item1 + 1, state.Item2, state.Item3, projPoint, state.Item5, watch.ElapsedMilliseconds + FrameDelay);
                }

                uavsListMutex.ReleaseMutex();
            }
        }
    }
}