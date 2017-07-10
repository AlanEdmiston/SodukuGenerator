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

namespace sudokuGenerator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font1;
        Texture2D sudokuGrid;
        Texture2D square;
        Texture2D largeSquare;
        Rectangle sudokuRect = new Rectangle(20, 20, 450, 450);
        int sizelevel = 4;
        int gridSize;
        int shuffler;
        int scrollx;
        int scrolly;

        Random rand = new Random();

        bool[] largeCellStarted;
        bool[] isPossibleRows;
        bool[] isPossibleColumns;
        int[,] grid;
        int[,] storeGrid;
        Vector2[,] numberGrid;
        Rectangle[,] rectMatrix;
        Rectangle[,] largeRectMatrix;
        string[,] stringGrid;
        bool[] translatorHasBeenUsed;
        int screenHeight;
        int screenWidth;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.graphics.IsFullScreen = false;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here


            gridSize = sizelevel * sizelevel;
            isPossibleRows = new bool[gridSize];
            isPossibleColumns = new bool[gridSize];
            grid = new int[gridSize, gridSize];
            storeGrid = new int[gridSize, gridSize];
            numberGrid = new Vector2[gridSize, gridSize];
            stringGrid = new string[gridSize, gridSize];
            largeCellStarted = new bool[gridSize];
            translatorHasBeenUsed = new bool[gridSize];
            rectMatrix = new Rectangle[gridSize, gridSize];
            largeRectMatrix = new Rectangle[gridSize, gridSize];
            screenHeight = GraphicsDevice.Viewport.Height;
            screenWidth = GraphicsDevice.Viewport.Width;

            KeyboardState keys = Keyboard.GetState();

            if (keys.IsKeyDown (Keys.Left) == true)
            {
                scrollx -= 5;
            }
            if (keys.IsKeyDown(Keys.Right) == true)
            {
                scrollx += 5;
            }
            if (keys.IsKeyDown(Keys.Up) == true)
            {
                scrolly += 5;
            }
            if (keys.IsKeyDown(Keys.Down) == true)
            {
                scrolly -= 5;
            }

            if (scrollx < 0)
            {
                scrollx = 0;
            }
            if (scrolly < 0)
            {
                scrolly = 0;
            }
            if (scrollx > gridSize * 40)
            {
                scrolly = gridSize * 40;
            }
            if (scrolly > gridSize * 40)
            {
                scrolly = gridSize * 40;
            }

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    rectMatrix[i, j] = new Rectangle(scrollx + 25 + i * 40, scrolly + 25 +j * 40, 40, 40);
                }
            }

            for (int i = 0; i < sizelevel; i++)
            {
                for (int j = 0; j < sizelevel; j++)
                {
                    largeRectMatrix[i, j] = new Rectangle(scrollx + 25 + i * 40 * sizelevel, scrolly + 25 + j * 40 * sizelevel, 40 * sizelevel, 40 * sizelevel);
                }
            }

            for (int i = 0; i < gridSize; i++)
            {
                isPossibleRows[i] = true;
                isPossibleColumns[i] = true;
            }

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
            font1 = this.Content.Load<SpriteFont>("SpriteFont1");
            sudokuGrid = this.Content.Load<Texture2D>("SudokuGrid");
            square = this.Content.Load<Texture2D>("Square");
            largeSquare = this.Content.Load <Texture2D>("BigSquare");

            // TODO: use this.Content to load your game content here
            #region

           //decide number systematicaly
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++ )
                {

                    grid[i, j] = (i + sizelevel * j + (1 + j - j % sizelevel) / sizelevel) % gridSize;
                }
            }
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if(grid[i,j] == 0)
                    {
                        grid[i, j] = gridSize;
                    }
                }
            }

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    storeGrid[i, j] = grid[i, j];
                }
            }

            //shuffle numbers randomly
            for (int k = 0; k < sizelevel; k++)
            {
                for (int i = 0; i < sizelevel; )
                {
                    shuffler = rand.Next(0, sizelevel);
                    if (isPossibleColumns[shuffler + sizelevel * k] == true)
                    {
                        isPossibleColumns[shuffler + sizelevel * k] = false;
                        for (int j = 0; j < gridSize; j++)
                        {
                            storeGrid[shuffler + sizelevel * k, j] = grid[i + sizelevel * k, j];
                        }
                        i++;
                    }
                }
            }

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    grid[i, j] = storeGrid[i, j];
                }
            }

            for (int k = 0; k < sizelevel; k++)
            {
                for (int i = 0; i < sizelevel; )
                {
                    shuffler = rand.Next(0, sizelevel);
                    if (isPossibleRows[shuffler + sizelevel * k] == true)
                    {
                        isPossibleRows[shuffler + sizelevel * k] = false;
                        for (int j = 0; j < gridSize; j++)
                        {
                            storeGrid[j, shuffler + sizelevel * k] = grid[j, i + sizelevel * k];
                        }
                        i++;
                    }
                }
            }

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    grid[i, j] = storeGrid[i, j];
                }
            }

            for (int i = 0; i < gridSize; i++)
            {
                isPossibleColumns[i] = true;
                isPossibleRows[i] = true;
            }

            for (int i = 0; i < sizelevel; )
            {
                shuffler = rand.Next(0, sizelevel) * sizelevel;
                if (isPossibleColumns[shuffler] == true)
                {
                    isPossibleColumns[shuffler] = false;
                    for (int j = 0; j < gridSize; j++)
                    {
                        storeGrid[shuffler, j] = grid[i * sizelevel, j];
                        storeGrid[shuffler + 1, j] = grid[i * sizelevel + 1, j];
                        storeGrid[shuffler + 2, j] = grid[i * sizelevel + 2, j];
                    }
                    i++;
                }
            }

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    grid[i, j] = storeGrid[i, j];
                }
            }

            for (int i = 0; i < sizelevel; )
            {
                shuffler = rand.Next(0, sizelevel) * sizelevel;
                if (isPossibleRows[shuffler] == true)
                {
                    isPossibleRows[shuffler] = false;
                    for (int j = 0; j < gridSize; j++)
                    {
                        storeGrid[j, shuffler] = grid[j, i * sizelevel];
                        storeGrid[j, shuffler + 1] = grid[j, i * sizelevel + 1];
                        storeGrid[j, shuffler + 2] = grid[j, i * sizelevel + 2];
                    }
                    i++;
                }
            }
            
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    grid[i, j] = storeGrid[i, j];
                }
            }
                

            #endregion

            //write out the numbers into the grid
                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        if (grid[i, j] == 0)
                        {

                        }
                        else
                        {
                            stringGrid[i, j] = grid[i, j].ToString();
                        }
                    }
                }
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    numberGrid[i, j] = new Vector2(45 + 40 * i, 45 + 40 * j);
                    
                }
            }

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            {
                spriteBatch.Begin();
                
                for(int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        spriteBatch.Draw(square, rectMatrix[i, j], Color.White);
                    }
                }
                for (int i = 0; i < sizelevel; i++)
                {
                    for (int j = 0; j < sizelevel; j++)
                    {
                        //spriteBatch.Draw(largeSquare, largeRectMatrix[i, j], Color.White);
                    }
                }

                
                //spriteBatch.Draw(sudokuGrid, sudokuRect, Color.White);
                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        if (grid[i, j] == 0)
                        {

                        }
                        else
                        {
                            spriteBatch.DrawString(font1, stringGrid[i, j], numberGrid[i, j], Color.Black);
                        }
                    }
                }
                spriteBatch.End();
            }

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
