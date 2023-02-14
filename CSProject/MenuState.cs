using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSProject
{
    public class MenuState : State
    {

        SpriteFont Font;
        SpriteFont TitleFont;
        Button startButton;
        Button HowtoPlayButton;
        Button QuitButton;


        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
            _game = game;
        }

        public override void Initialize()
        {
            
        }

        public override void LoadContent()
        {
            Font = _content.Load<SpriteFont>("myfont");
            TitleFont = _content.Load<SpriteFont>("TitleFont");

            startButton = new Button(_content.Load<Texture2D>("Button"), Font)
            {
                Position = new Vector2(Game1.screenwidth/2 - 75, Game1.screenheight/2),
                Text = "Start",
            };

            startButton.Click += StartButton_Click;

            HowtoPlayButton = new Button(_content.Load<Texture2D>("Button"), Font)
            {
                Position = new Vector2(Game1.screenwidth / 2 - 75, Game1.screenheight / 2 + 75),
                Text = "Help",
            };

            HowtoPlayButton.Click += HowtoPlayButton_Click;

            QuitButton = new Button(_content.Load<Texture2D>("Button"), Font)
            {
                Position = new Vector2(Game1.screenwidth / 2 - 75, Game1.screenheight / 2 + 150),
                Text = "Quit",
            };

            QuitButton.Click += QuitButton_Click;
        }

        private void HowtoPlayButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new HowToPlayState(_game, _graphicsDevice, _content));
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            _game.Exit();
        }



        public override void Update(GameTime gameTime)
        {
            startButton.Update(gameTime);
            HowtoPlayButton.Update(gameTime);
            QuitButton.Update(gameTime);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            startButton.Draw(gameTime, spriteBatch);
            HowtoPlayButton.Draw(gameTime, spriteBatch);
            QuitButton.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(TitleFont, "SURVIVE THE MAZE", new Vector2(Game1.screenwidth / 5, Game1.screenheight / 4), Color.Red);
            spriteBatch.End();
        }
        
    }
}
