using System;
using CoordinatorMap.Grid;

namespace CoordinatorMap
{
    public class Waypoint
    {
        public Waypoint Prev;
        public Waypoint Next;
        private UAV Uav;
        public Cell Cell { get; }

        public Waypoint(Waypoint prev, UAV uav, Cell cell)
        {
            Prev = prev;
            Uav = uav;
            Cell = cell;
        }
    }
}
