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

        public int GetHealth()
        {
            return health;
        }

        public void takeDamage(int damage)
        {
            health -= damage;
        }

    }
}
