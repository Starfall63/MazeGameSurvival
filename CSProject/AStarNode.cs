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


        public void HeuristicDistance(int TargetX, int TargetY)
        {
            this.Distance = Math.Abs(TargetX- X) + Math.Abs(TargetY- Y);
        }


    }
}
