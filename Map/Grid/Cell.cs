using System.Collections.Generic;

namespace CoordinatorMap.Grid
{
    internal class Cell
    {
        public double Coordinate { get; } // Latitude or longitude

        public Cell(double val)
        {
            Coordinate = val;
        }



        private List<Cell> Children; // If Cell object represents latitude, it has child objects representing longitude values

        public void InsertCell(double lng)
        {
            if (Children == null) Children = new List<Cell>();

            Children.Add(new Cell(lng));
            Children.Sort((x, y) => x.Coordinate.CompareTo(y.Coordinate));
        }

        public Waypoint AddWaypoint(Waypoint prev, double lng, UAV uav, float[] cellSize)
        {
            // Find the nearest lower longitude cell:
            Cell wpCell = null;
            foreach (Cell i in Children)
            {
                if (i.Coordinate > lng) break;
                wpCell = i;
            }

            Waypoint wp = new Waypoint(prev, uav, wpCell);
            if (prev != null) prev.Next = wp;

            wpCell.AddWaypoint(wp);

            return wp;
        }


        private List<Waypoint> Waypoints; // The waypoints within the cell

        public void AddWaypoint(Waypoint wp)
        {
            if (Waypoints == null) Waypoints = new List<Waypoint>();

            Waypoints.Add(wp);
        }

        public void RemoveWaypoint(Waypoint wp)
        {
            Waypoints.Remove(wp);
        }
    }
}
