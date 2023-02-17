using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSProject
{
    internal class CollisionTiles:Tiles
    {
        public CollisionTiles(int i, Rectangle newRectangle)
        {
            texture = Content.Load<Texture2D>("Tile" + i);
            this.Rectangle = newRectangle;
            health = maxhealth;
        }

        /// <summary>
        /// Returns the current health of the tile when called.
        /// </summary>
        /// <returns></returns>
        public int GetHealth()
        {
            return health;
        }

        /// <summary>
        /// Reduces the health of the tile when it takes damage.
        /// </summary>
        /// <param name="damage"></param>
        public void takeDamage(int damage)
        {
            health -= damage;
        }

    }
}
