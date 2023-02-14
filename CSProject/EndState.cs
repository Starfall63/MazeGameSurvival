using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSProject
{
    public class EndState : State
    {
        SpriteFont Font;
        Button returnButton;
        Button QuitButton;
        private int _waves;
        private int _seconds;
        string gamestats;


        public EndState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, int waves, int timer) : base(game, graphicsDevice, content)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
            _game = game;
            _waves = waves;
            _seconds = timer;
        }

        public override void Initialize()
        {
          
        }

        public override void LoadContent()
        {
            Font = _content.Load<SpriteFont>("myfont");

            returnButton = new Button(_content.Load<Texture2D>("Button"), Font)
            {
                Position = new Vector2(Game1.screenwidth / 2 - 75, Game1.screenheight / 2),
                Text = "Main Menu",
            };

            returnButton.Click += ReturnButton_Click;

            QuitButton = new Button(_content.Load<Texture2D>("Button"), Font)
            {
                Position = new Vector2(Game1.screenwidth / 2 - 75, Game1.screenheight / 2 + 75),
                Text = "Quit",
            };

            QuitButton.Click += QuitButton_Click;

            gamestats = "You survived " + _waves + " waves in " + Math.Round((double)_seconds/1000) + " seconds.";
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            _game.Exit();
        }

        private void ReturnButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
        }

        public override void Update(GameTime gameTime)
        {
            returnButton.Update(gameTime);
            QuitButton.Update(gameTime);    
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(Font, "You died.", new Vector2(Game1.screenwidth/3+140, Game1.screenheight/4), Color.White);
            spriteBatch.DrawString(Font, gamestats, new Vector2(500, Game1.screenheight/4+50), Color.White);
            returnButton.Draw(gameTime, spriteBatch);
            QuitButton.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        
       
    }
}
