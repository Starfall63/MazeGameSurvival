using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CSProject
{
    class monster : GameObject
    {
        #region fields
        public enum Direction
        {
            down,
            left,
            right,
            up,
        }
       
        protected  int _speed = 4;
        protected int _damage = 5;
        protected int health = 50;
        protected bool _alive;
        protected bool _playerSeen = false;
        protected bool _moving = false;
        protected bool _needtobreakwall = false;
        Vector2 newLocation;



        protected Stack<AStarNode> path = new Stack<AStarNode>();

        #endregion

        #region methods
        public int GetDamage()
        {
            return _damage;
        }

        public bool GetAlive()
        {
            return _alive;
        }

        public bool GetMoveStatus()
        {
            return _moving;
        }

        public bool GetSeen()
        {
            return _playerSeen;
        }

        public bool NeedToBreakWall()
        {
            return _needtobreakwall;
        }

        /// <summary>
        /// Constructor for the monster.
        /// Spawns the monster in a random location in the maze making sure that it does not spawn in the walls.
        /// </summary>
        /// <param name="maze"></param>

        public monster(int[,] maze)
        {
            
            Random RNG = new Random();
            bool correctSpawn = false;
            int xspawn = 1;
            int yspawn = 1;
            while (!correctSpawn)
            {
                xspawn = RNG.Next(1, GameState.mapwidth - 2);
                yspawn = RNG.Next(1, GameState.mapheight - 2);
                if (maze[yspawn, xspawn] != 1) correctSpawn = true;
            }
            _alive = true;
            currentcolour = Color.White;
            Location = new Vector2(xspawn * GameState.pixelsize, yspawn * GameState.pixelsize);

        }
        /// <summary>
        /// Loads the sprite texture of the monster.
        /// </summary>
        /// <param name="Content"></param>
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            Texture = Content.Load<Texture2D>(@"ghost64");
        }


        public override void Update(GameTime gameTime)
        {

            Edge = new Rectangle((int)Location.X, (int)Location.Y, 32, 32);

            //Will move the monster if it has not reached the new location.
            if (_moving)
                Move();
            
            //Gets the next location to be moved to if there is a path that is available and it has finished its previous movement.
            if (path.Any() && !_moving)
            {
                setDestination();
                _moving = true;
            }
            
            //Checks whether the monster has been killed.
            if(health <= 0)_alive = false;
            currentcolour = Color.White;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Location, currentcolour);
        }

        
        /// <summary>
        /// Moves the monster when there is a newlocation to be moved to.
        /// </summary>
        private void Move()
        {
            if (Location.X < newLocation.X) Location.X += _speed;
            else if (Location.X > newLocation.X) Location.X -= _speed;
            else if (Location.Y < newLocation.Y) Location.Y += _speed;
            else if (Location.Y > newLocation.Y) Location.Y -= _speed;
            else _moving = false;
        }

        /// <summary>
        /// Gets the next location that the monster needs from the stack created from the A star algorithm.
        /// </summary>
        private void setDestination()
        {

            newLocation.X = path.Peek().X * 64;
            newLocation.Y = path.Pop().Y * 64;
        }

        /// <summary>
        /// Makes the monster take damage and makes it go red for a visual cue to the player to show they are doing damage.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            health -= damage;
            currentcolour = Color.Red;
        }

        /// <summary>
        /// The A star algorithm that gets the shortest path to get to the player from the monster's current location.
        /// Gets the current location of the monster and the heuristic distance from the player.
        /// Adds all valid nodes around the monster into a list.
        /// Adds the start node (the monster's location) to a visitednodes list.
        /// The next node to be checked will be the node that is the shortest distance away from the play in the validnodes list.
        /// Repeats this process of checking nodes until the player is found.
        /// Then will add the path to get to the player into a stack.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="maze"></param>
        public virtual void AStar(Player player, int[,] maze)
        {
            //Will only run if the player has been seen.
            if (!_playerSeen) return;
            
            //Clears any existing paths to allow for rerouting.
            path.Clear();
            
            //Gets the Locations of the monster and the player and assigns them to the start node and target node to be found.
            var start = new AStarNode();
            start.X = (int)Location.X / 64;
            start.Y = (int)Location.Y / 64;

            var target = new AStarNode();
            target.X = (int)player.Location.X / 64;
            target.Y = (int)player.Location.Y / 64;

            //Gets the heuristic distance from the monster to the player.
            start.HeuristicDistance(target.X, target.Y);

            //Adds the start node into activenodes list which are nodes waiting to be checked.
            var activenodes = new List<AStarNode>();
            activenodes.Add(start);
            //Visited nodes are a list of nodes that have been checked.
            var visitednodes = new List<AStarNode>();

            while (activenodes.Any())
            {
                //Gets the node that is the shortest distance away from the player.
                var checkNode = activenodes.OrderBy(x => x.CostDistance).First();
                
                //If the node selected is the target then it will add the nodes to the path stack by going back through the parent nodes until the start node has been reached.
                //Will stop running the algorithm after the path has been all added to the stack.
                if(checkNode.X == target.X && checkNode.Y == target.Y)
                {
                    var node = checkNode;

                    while (true)
                    {
                        if (maze[node.Y,node.X] == 0)
                        {
                            path.Push(node);
                        }
                        node = node.Parent;
                        if (node == null)
                            return;
                    }
                }

                
                visitednodes.Add(checkNode);
                activenodes.Remove(checkNode);

                //Gets a list of all the valid nodes that can be travelled to around the checkNode.
                var validnodes = GetValidNodes(maze, checkNode, target);

                foreach (var validnode in validnodes)
                {
                    //Checks all the validnodes in the list to see if it is already in the activenodes list.
                    //If it isn't already in the list then it will be added to the list.
                    //If it is then it will compare the distance and see whether going through the checknode is quicker than a path previously found.
                    if(activenodes.Any(x => x.X == validnode.X && x.Y == validnode.Y))
                    {
                        var existingnode = activenodes.First(x => x.X == validnode.X && x.Y == validnode.Y);
                        if(existingnode.CostDistance > checkNode.CostDistance)
                        {
                            activenodes.Remove(existingnode);
                            activenodes.Add(validnode);
                        }
                    }
                    else
                    {
                        activenodes.Add(validnode);
                    }
                }
            }

        }
        /// <summary>
        /// Helper method for the A star algorithm.
        /// Takes in an A star node and the map.
        /// Checks every node around to make sure it is a valid node to be travelled to.
        /// Checks by checking whether the nodes around it are walls or are outside the maze.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="currentnode"></param>
        /// <param name="targetNode"></param>
        /// <returns></returns>
        private static List<AStarNode> GetValidNodes(int[,]map, AStarNode currentnode, AStarNode targetNode)
        {
            var possiblenodes = new List<AStarNode>()
            {
                new AStarNode{X = currentnode.X, Y = currentnode.Y-1, Parent = currentnode, Cost = currentnode.Cost+1},
                new AStarNode{X = currentnode.X, Y = currentnode.Y+1, Parent = currentnode, Cost = currentnode.Cost+1},
                new AStarNode{X = currentnode.X-1, Y = currentnode.Y, Parent = currentnode, Cost = currentnode.Cost+1},
                new AStarNode{X = currentnode.X+1, Y = currentnode.Y, Parent = currentnode, Cost = currentnode.Cost+1},
            };

            var maxX = map.GetLength(1)-1;
            var maxY = map.GetLength(0)-1;

            return possiblenodes
                .Where(node => node.X >= 0 && node.X <= maxX)
                .Where(node => node.Y >= 0 && node.Y <= maxY)
                .Where(node => map[node.Y, node.X] == 0)
                .ToList();

        }

        /// <summary>
        /// Checks 10 tiles up down left and right of the monster to see if the player is within range to be attacked.
        /// Each check will stop after checking 10 tiles or if it has reached a wall.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="maze"></param>
        public virtual void sensePlayer(Player player, int[,] maze)
        {
            if (_playerSeen) return;
            int playerx = (int)player.Location.X / 64;
            int playery = (int)player.Location.Y / 64;

            int currentx = (int)Location.X / 64;
            int currenty = (int)Location.Y / 64;

            for (int i = currentx-10; i < currentx+10; i++)
            {
                if (i <= 0) continue;
                if (maze[currenty, i] == 1) break;
                if (i == playerx && currenty == playery)
                {
                    _playerSeen = true;
                    break;
                }
            }
            for (int i = currentx; i > currentx - 10; i--)
            {
                if (maze[currenty, i] == 1) break;
                if (i == playerx && currenty == playery)
                {
                    _playerSeen = true;
                    break;
                }
            }
            for (int i = currenty; i < currenty + 10; i++)
            {
                if (maze[i, currentx] == 1) break;
                if (currentx == playerx && i == playery)
                {
                    _playerSeen = true;
                    break;
                }
            }
            for (int i = currenty; i > currenty - 10; i--)
            {
                if (maze[i, currentx] == 1) break;
                if (currentx == playerx && i == playery)
                {
                    _playerSeen = true;
                    break;
                }
            }


        }

        #endregion

    }
}
