using System.Collections.Generic;
using GMap.NET;
using static CoordinatorMap.Utils;
using System;

namespace CoordinatorMap.Grid
{
    internal class GridHandler
    {
        private List<Cell> Cells = new List<Cell>();
        private float[] CellSize;

        public GridHandler(float[] cellSize)
        {
            CellSize = cellSize;
        }



        public void InsertCell(PointLatLng cellLL)
        {
            double lat = cellLL.Lat, lng = cellLL.Lng;

            Cell latNode = Cells.Find(i => i.Coordinate == lat);

            // If there's no latitude node for the cell, it must be created:
            if (latNode == null)
            {
                latNode = new Cell(lat);
                Cells.Add(latNode);
                Cells.Sort((x, y) => x.Coordinate.CompareTo(y.Coordinate));
            }

            latNode.InsertCell(lng);
        }

        private Waypoint AddWaypoint(PointLatLng wpLL, Waypoint prevWp, UAV uav)
        {
            // Find the nearest higher latitude node:
            Cell wpLatNode = null;
            foreach (Cell i in Cells)
            {
                wpLatNode = i;
                if (i.Coordinate > wpLL.Lat) break;
            }

            return wpLatNode.AddWaypoint(prevWp, wpLL.Lng, uav, CellSize);
        }



        public void AddWaypoints(UAV uav)
        {
            List<PointLatLng> wpLLs = uav.WaypointsLL;
            Waypoint wp = null;
            bool isFirstWp = true;

            foreach (PointLatLng i in wpLLs)
            {
                wp = AddWaypoint(i, wp, uav);

                if (isFirstWp)
                {
                    uav.FirstWaypoint = wp;
                    isFirstWp = false;
                }
            }
        }
        
        public void RemoveWaypointsR(Waypoint wp)
        {
            while (wp != null)
            {
                wp.Prev = null;
                wp.Cell.RemoveWaypoint(wp);
                wp = wp.Next;
            }
        }

    }
}

