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

        /// <summary>
        /// Constructor for the EndState.
        /// Loads all of the statistics that will be displayed to the plaer after they die.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="content"></param>
        /// <param name="waves"></param>
        /// <param name="timer"></param>
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

        /// <summary>
        /// Loads all the fonts and textures for buttons and initialises the buttons.
        /// </summary>
        public override void LoadContent()
        {
            Font = _content.Load<SpriteFont>("myfont");

            //Initializes the return button with its postion, text and font.
            returnButton = new Button(_content.Load<Texture2D>("Button"), Font)
            {
                Position = new Vector2(Game1.screenwidth / 2 - 75, Game1.screenheight / 2),
                Text = "Main Menu",
            };

            //Calls the event that will happen when the return button is clicked.
            returnButton.Click += ReturnButton_Click;

            //Initializes the quit button with its postion, text and font.
            QuitButton = new Button(_content.Load<Texture2D>("Button"), Font)
            {
                Position = new Vector2(Game1.screenwidth / 2 - 75, Game1.screenheight / 2 + 75),
                Text = "Quit",
            };

            //Calls the event that will happen when the quit game button is clicked.
            QuitButton.Click += QuitButton_Click;

            gamestats = "You survived " + _waves + " waves in " + Math.Round((double)_seconds/1000) + " seconds.";
        }

        /// <summary>
        /// Closes the game when the quit button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitButton_Click(object sender, EventArgs e)
        {
            _game.Exit();
        }

        /// <summary>
        /// Returns to the starting menu screen when the return button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
