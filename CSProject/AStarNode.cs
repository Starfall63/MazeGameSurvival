using System;
using System.Collections.Generic;
using System.Text;

namespace CSProject
{
    class AStarNode
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Cost { get;set; }
        public int Distance { get;set; }
        public int CostDistance => Cost + Distance;

        public AStarNode Parent;

        public bool IsWall { get; set; }

        /// <summary>
        /// Gets the heuristic distance by adding the x distance and y distance.
        /// </summary>
        /// <param name="TargetX"></param>
        /// <param name="TargetY"></param>
        public void HeuristicDistance(int TargetX, int TargetY)
        {
            this.Distance = Math.Abs(TargetX- X) + Math.Abs(TargetY- Y);
        }


    }
}
