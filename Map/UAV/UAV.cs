using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace CoordinatorMap
{
    public partial class UAV
    {
        public int Id { get; }
        private PointLatLng _currentPosition;
        private int _currentWpNum = 0;
        private CoordinatorMap Coordinator;
        private List<PointLatLng> _waypointsLL;
        internal Waypoint FirstWaypoint;

        internal UAV(int sysid, CoordinatorMap coordinator, Color color)
        {
            Id = sysid;
            Coordinator = coordinator;
            UavColor = color;
        }



        internal static void StartAnimationThread(List<UAV> uavs, Mutex uavsListMutex) { new Thread(() => Animate(uavs, uavsListMutex)).Start(); }

        public PointLatLng CurrentPosition
        {
            get
            {
                Coordinator.UavPositionMutex.WaitOne();

                PointLatLng pos = _currentPosition;

                Coordinator.UavPositionMutex.ReleaseMutex();

                return pos;
            }

            set
            {
                Coordinator.UavPositionMutex.WaitOne();

                _currentPosition = value;

                if (UavMarker == null) DrawUAVMarker();

                Coordinator.UavPositionMutex.ReleaseMutex();
            }
        }

        internal List<PointLatLng> WaypointsLL
        {
            get
            {
                return _waypointsLL;
            }

            set
            {
                // Remove old information:
                ClearStage();
                Coordinator.GridCellsHandler.RemoveWaypointsR(FirstWaypoint);

                _waypointsLL = value;

                // Draw the lines connecting the waypoints:
                for (int i = 0; i < _waypointsLL.Count - 1; i++)
                {
                    GMapPolygon polygon = new GMapPolygon(_waypointsLL.GetRange(i, 2), "_");
                    polygon.Stroke = new Pen(UavColor, 3);
                    Coordinator.Overlay.Polygons.Add(polygon);
                    WpLines.Add(polygon);
                }

                int count = 0;
                foreach (PointLatLng i in _waypointsLL) DrawWaypointMarker(i, count++);
            }
        }

        public int CurrentWpNum
        {
            get
            {
                return _currentWpNum;
            }

            set
            {
                int oldWpNum = _currentWpNum;
                _currentWpNum = value;

                if (Selected)
                {
                    foreach (GMapPolygon i in TrailLines) Coordinator.Overlay.Polygons.Remove(i);

                    // Remove last current waypoint line's transparency
                    Coordinator.Overlay.Polygons.Remove(WpLines[oldWpNum]);
                    WpLines[oldWpNum].Stroke = new Pen(UavColor, 3);
                    Coordinator.Overlay.Polygons.Add(WpLines[oldWpNum]);

                    // Add transparency to the new current waypoint line
                    Coordinator.Overlay.Polygons.Remove(WpLines[_currentWpNum]);
                    WpLines[_currentWpNum].Stroke = new Pen(Color.FromArgb(100, UavColor), 3);
                    Coordinator.Overlay.Polygons.Add(WpLines[_currentWpNum]);
                }
            }
        }
        
        private bool _selected = false;
        internal bool Selected
        {
            get
            {
                return _selected;
            }

            set
            {
                _selected = value;

                int count = 0;
                foreach (GMapPolygon i in WpLines)
                    if (count++ >= _currentWpNum)
                    {
                        i.Stroke = new Pen(_selected ? Color.FromArgb(100, UavColor) : UavColor, 3);
                        Coordinator.Overlay.Polygons.Remove(i);
                        Coordinator.Overlay.Polygons.Add(i);
                    }

                foreach (GMapPolygon i in TrailLines) Coordinator.Overlay.Polygons.Remove(i);

                DrawTrail(_currentPosition);
            }
        }
    }
}
