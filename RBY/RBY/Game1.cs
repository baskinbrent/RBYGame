using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace RBY
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //1=RED 2=BLUE 3=YELLOW
        private int CURRENTCOLOR = 0;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SoundEffect dotHitSound1;
        private SoundEffect dotHitSound2;
        private SoundEffect dotHitSound3;
        private SoundEffect dotHitSound4;
        private SoundEffect dotHitSound5;
        private SoundEffect dotHitSound6;
        private MouseState oldMouseState;
        private MouseState newMouseState;
        private KeyboardState oldKeyState;
        private KeyboardState newKeyState;
        private Player player = new Player();
        #region Init Player and Enemy textures
        private Texture2D blue;
        private Texture2D yellow;
        private Texture2D red;
        private Texture2D enemyRed;
        private Texture2D enemyBlue;
        private Texture2D enemyGreen;
        private Texture2D enemyOrange;
        private Texture2D enemyPurple;
        private Texture2D enemyYellow;
        #endregion
        private static int waveNumber = 1;
        private float waveSpawnTimer = 2000;
        private float newSpawnWait = (1f / waveNumber) * 2000f;
        private static int numEnemyDotsToSpawn = 10;
        private Random r = new Random();
        private IncomingDots[] enemyDotList = new IncomingDots[numEnemyDotsToSpawn];
        private int enemyRandom = 1;
        

        
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 960;
            graphics.PreferredBackBufferWidth = 640;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            enemyDotList = PopulateEnemyDotList(enemyDotList);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Sound loads
            dotHitSound1 = Content.Load<SoundEffect>("DotSounds\\redhit");
            dotHitSound2 = Content.Load<SoundEffect>("DotSounds\\bluehit");
            dotHitSound3 = Content.Load<SoundEffect>("DotSounds\\yellowhit");
            dotHitSound4 = Content.Load<SoundEffect>("DotSounds\\purplehit");
            dotHitSound5 = Content.Load<SoundEffect>("DotSounds\\greenhit");
            dotHitSound6 = Content.Load<SoundEffect>("DotSounds\\orangehit");
            #endregion

            #region Sprite loads
            blue = Content.Load<Texture2D>("PlayerDots\\blue");
            yellow = Content.Load<Texture2D>("PlayerDots\\orange");
            red = Content.Load<Texture2D>("PlayerDots\\red");
            enemyRed = Content.Load<Texture2D>("EnemyDots\\redenemydot");
            enemyBlue = Content.Load<Texture2D>("EnemyDots\\blueenemydot");
            enemyGreen = Content.Load<Texture2D>("EnemyDots\\greenenemydot");
            enemyOrange = Content.Load<Texture2D>("EnemyDots\\orangeenemydot");
            enemyPurple = Content.Load<Texture2D>("EnemyDots\\purpleenemydot");
            enemyYellow = Content.Load<Texture2D>("EnemyDots\\yellowenemydot");
            #endregion
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (numEnemyDotsToSpawn <= 0)
            {
                if (CheckIfListDead(enemyDotList))
                    enemyDotList = NewWave();

            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            Viewport view = graphics.GraphicsDevice.Viewport;
            Vector2 screenCenter = new Vector2(view.Width / 2f, view.Height / 2f);

            #region Mouse and Keyboard Controls
            oldMouseState = newMouseState;
            newMouseState = Mouse.GetState();
            oldKeyState = newKeyState;
            newKeyState = Keyboard.GetState();
            newSpawnWait += gameTime.ElapsedGameTime.Milliseconds;

            if(((oldMouseState.LeftButton == ButtonState.Released && newMouseState.LeftButton == ButtonState.Pressed)
                || oldKeyState.IsKeyUp(Keys.Space) && newKeyState.IsKeyDown(Keys.Space)
                || oldKeyState.IsKeyUp(Keys.Right) && newKeyState.IsKeyDown(Keys.Right))
                && !(newKeyState.IsKeyDown(Keys.LeftControl) || newKeyState.IsKeyDown(Keys.RightControl)))
            {
                player.switchColorsForwards();
            }

            if ((((oldMouseState.LeftButton == ButtonState.Released && newMouseState.LeftButton == ButtonState.Pressed)
                || oldKeyState.IsKeyUp(Keys.Space) && newKeyState.IsKeyDown(Keys.Space))
                && (newKeyState.IsKeyDown(Keys.LeftControl)||newKeyState.IsKeyDown(Keys.RightControl)))
                || oldKeyState.IsKeyUp(Keys.Left) && newKeyState.IsKeyDown(Keys.Left))
            {
                player.switchColorsBackwards();
            }
            #endregion



            for (int i = 0; i < enemyDotList.Length; i++)
            {
                if (newSpawnWait >= waveSpawnTimer)
                {                    
                    if (!enemyDotList[i].hasBeenPlaced && !enemyDotList[i].isAlive)
                    {
                        newSpawnWait = 0;
                        enemyDotList[i].hasBeenPlaced = true;
                        enemyDotList[i].isAlive = true;
                        numEnemyDotsToSpawn--;
                    }
                }
            }

            CheckCollision(enemyDotList, timeDelta, screenCenter);

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Viewport view = graphics.GraphicsDevice.Viewport;
            Vector2 screenCenter = new Vector2(view.Width / 2f, view.Height);
            Vector2 playerCenter = new Vector2(red.Width / 2f, red.Height / 2f);
            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            //Draw logic for random enemy colors
            #region Enemy Draw Logic
            
            for (int i = 0; i < enemyDotList.Length; i++)
            {
                if (enemyDotList[i].hasBeenPlaced && enemyDotList[i].isAlive)
                {
                    switch (enemyDotList[i].color)
                    {
                        case IncomingDots.possibleColors.red:
                            spriteBatch.Draw(enemyRed, enemyDotList[i].spawnPosition, Color.White);
                            break;
                        case IncomingDots.possibleColors.blue:
                            spriteBatch.Draw(enemyBlue, enemyDotList[i].spawnPosition, Color.White);
                            break;
                        case IncomingDots.possibleColors.yellow:
                            spriteBatch.Draw(enemyYellow, enemyDotList[i].spawnPosition, Color.White);
                            break;
                        case IncomingDots.possibleColors.purple:
                            spriteBatch.Draw(enemyPurple, enemyDotList[i].spawnPosition, Color.White);
                            break;
                        case IncomingDots.possibleColors.orange:
                            spriteBatch.Draw(enemyOrange, enemyDotList[i].spawnPosition, Color.White);
                            break;
                        case IncomingDots.possibleColors.green:
                            spriteBatch.Draw(enemyGreen, enemyDotList[i].spawnPosition, Color.White);
                            break;
                        default:
                            break;
                    }
                }
            }
            
            #endregion

            //Draw logic for Player switching colors
            #region Draw Logic for Player color switching
            if (player.isAlive)
            {
                if (player.currentColor == Player.Colors.red)
                {
                    if (CURRENTCOLOR != 1)
                        spriteBatch.Draw(red, screenCenter, null, Color.White, 0f, playerCenter, 1f, SpriteEffects.None, 0f);
                }
                else if (player.currentColor == Player.Colors.blue)
                {
                    if (CURRENTCOLOR != 2)
                        spriteBatch.Draw(blue, screenCenter, null, Color.White, 0f, playerCenter, 1f, SpriteEffects.None, 0f);
                }
                else if (player.currentColor == Player.Colors.yellow)
                {
                    if (CURRENTCOLOR != 3)
                        spriteBatch.Draw(yellow, screenCenter, null, Color.White, 0f, playerCenter, 1f, SpriteEffects.None, 0f);
                }
            }
            #endregion


            spriteBatch.End();

                // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private IncomingDots[] PopulateEnemyDotList(IncomingDots[] enemyDotList)
        {
            Viewport view = graphics.GraphicsDevice.Viewport;
            Vector2 screenCenter = new Vector2(view.Width/2f, view.Height);

            for (int i = 0; i < numEnemyDotsToSpawn; i++)
            {
                enemyDotList[i].hasBeenPlaced = false;
                enemyDotList[i].isAlive = false;
                enemyDotList[i].velocity = 200+(waveNumber*3);

                #region assign random enemy dot color
                
                if (waveNumber <= 3)
                    enemyRandom = r.Next(9) + 1;
                else
                    enemyRandom = r.Next(15)+1;
        
                switch (enemyRandom)
                {
                    case 1:
                    case 2:
                    case 3:
                        enemyDotList[i].color = IncomingDots.possibleColors.red;
                        break;
                    case 4:
                    case 5:
                    case 6:
                        enemyDotList[i].color = IncomingDots.possibleColors.blue;
                        break;
                    case 7:
                    case 8:
                    case 9:
                        enemyDotList[i].color = IncomingDots.possibleColors.yellow;
                        break;
                    case 10:
                    case 11:
                        enemyDotList[i].color = IncomingDots.possibleColors.purple;
                        break;
                    case 12:
                    case 13:
                        enemyDotList[i].color = IncomingDots.possibleColors.orange;
                        break;
                    case 14:
                    case 15:
                        enemyDotList[i].color = IncomingDots.possibleColors.green;
                        break;
                    default:
                        break;
                }
                #endregion

                #region assign random X starting position on top of screen
                
                enemyDotList[i].spawnCoordX = r.Next(640)+1;

                enemyDotList[i].spawnCoordY = -(view.Height);
                enemyDotList[i].spawnPosition.X = enemyDotList[i].spawnCoordX;
                enemyDotList[i].spawnPosition.Y = enemyDotList[i].spawnCoordY;

                enemyDotList[i].direction = screenCenter - enemyDotList[i].spawnPosition;

                #endregion

            }
            return enemyDotList;
        }

        private IncomingDots[] NewWave()
        {
            waveNumber++;

            if (waveSpawnTimer <= 500)
            {
                waveSpawnTimer = 500 - (waveNumber);

                if (waveSpawnTimer <= 300)
                    waveSpawnTimer = 300;
            }
            else
                waveSpawnTimer -= (30 * waveNumber);

            numEnemyDotsToSpawn = (int)(Math.Round
                (Math.Sqrt(waveNumber + 1) * 10, 0, MidpointRounding.AwayFromZero));
            IncomingDots[] enemyDotList = new IncomingDots[numEnemyDotsToSpawn];
            

            enemyDotList = PopulateEnemyDotList(enemyDotList);
            return enemyDotList;
        }

        private bool CheckIfListDead(IncomingDots[] enemyDotList)
        {
            bool listDead = true;

            foreach(IncomingDots incomingDot in enemyDotList)
            {
                if (incomingDot.hasBeenPlaced && incomingDot.isAlive)
                    listDead = false;
            }

            return listDead;
        }

        private void CheckCollision(IncomingDots[] enemyDotList, float timeDelta, Vector2 screenCenter)
        {
            for (int i = 0; i < enemyDotList.Length; i++)
            {
                if (enemyDotList[i].hasBeenPlaced && enemyDotList[i].isAlive)
                {
                    enemyDotList[i].Update(timeDelta);
                }

                if (enemyDotList[i].isAlive)
                {
                    if (enemyDotList[i].spawnPosition.Y >= 950)
                    {
                        enemyRandom = r.Next(6) + 1;
                        switch (enemyRandom)
                        {
                            case 1:
                                dotHitSound1.Play();
                                break;
                            case 2:
                                dotHitSound2.Play();
                                break;
                            case 3:
                                dotHitSound3.Play();
                                break;
                            case 4:
                                dotHitSound4.Play();
                                break;
                            case 5:
                                dotHitSound5.Play();
                                break;
                            case 6:
                                dotHitSound6.Play();
                                break;
                        }

                        enemyDotList[i].isAlive = false;

                        if (player.currentColor == Player.Colors.red && (enemyDotList[i].color == IncomingDots.possibleColors.red
                            || enemyDotList[i].color == IncomingDots.possibleColors.orange
                            || enemyDotList[i].color == IncomingDots.possibleColors.purple))
                            enemyDotList[i].isAlive = false;
                        else if (player.currentColor == Player.Colors.blue && (enemyDotList[i].color == IncomingDots.possibleColors.blue
                            || enemyDotList[i].color == IncomingDots.possibleColors.green
                            || enemyDotList[i].color == IncomingDots.possibleColors.purple))
                            enemyDotList[i].isAlive = false;
                        else if (player.currentColor == Player.Colors.yellow && (enemyDotList[i].color == IncomingDots.possibleColors.yellow
                            || enemyDotList[i].color == IncomingDots.possibleColors.orange
                            || enemyDotList[i].color == IncomingDots.possibleColors.green))
                            enemyDotList[i].isAlive = false;
                        else { player.isAlive = false; }
                    }
                }
            }
        }
    }
}
