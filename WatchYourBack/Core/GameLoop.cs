#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace WatchYourBack
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameLoop : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ECSManager systemManager;
        EFactory factory;
        LevelManager levelManager;

        Rectangle body;
        Rectangle wall;
        Texture2D bodyTexture;
        Texture2D wallTexture;
        WallTemplate wallTemplate;

        public GameLoop()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
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

            factory = new EFactory();
            systemManager = new ECSManager(new List<Entity>(), factory);
            levelManager = new LevelManager(systemManager, factory);

            systemManager.addSystem(new InputSystem());
            systemManager.addSystem(new PlayerInputSystem());
            systemManager.addSystem(new CollisionSystem());
            systemManager.addSystem(new MovementSystem());

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Texture2D testLevelLayout = Content.Load<Texture2D>("TestLevel");
            levelManager.addLevel(LevelName.firstLevel, new Level(testLevelLayout));

            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            bodyTexture = new Texture2D(GraphicsDevice, 1, 1);
            bodyTexture.SetData(new[] { Color.White });
            wallTexture = new Texture2D(GraphicsDevice, 1, 1);
            wallTexture.SetData(new[] { Color.Black });
            body = new Rectangle(100, 100, GraphicsDevice.Viewport.Width / 40, GraphicsDevice.Viewport.Width / 40);
            wall = new Rectangle(0, 0, GraphicsDevice.Viewport.Width / 32, GraphicsDevice.Viewport.Height / 18);
            
            wallTemplate = new WallTemplate(wallTexture, wall);
            factory.setWallTemplate(wallTemplate);

            systemManager.addEntity(factory.createAvatar(body, bodyTexture, Color.White));
            levelManager.buildLevel(LevelName.firstLevel);
            
            // TODO: use this.Content to load your game content here
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Add your update logic here

            systemManager.update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            systemManager.draw(spriteBatch);
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
