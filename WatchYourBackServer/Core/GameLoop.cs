using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace WatchYourBackServer
{
    class GameLoop
    {
        GameTime gameTime;
        Stack<World> worldStack;
        World activeWorld;

        World mainMenu;
        World inGame;
        World pauseMenu;

        InputListener inputListener;

        Dictionary<LevelName, LevelTemplate> levels;

        public GameLoop()
        {
            gameTime = new GameTime();
            Initialize();
            LoadContent();
            while (true)
                Update(gameTime);
        }

        private void Initialize()
        {
            // TODO: Add your initialization logic here
            worldStack = new Stack<World>();
            inputListener = new InputListener(this);


            levels = new Dictionary<LevelName, LevelTemplate>();

            //FIXFIXFIXFIXFIXFIXFIX

            //levels.Add(LevelName.TEST_LEVEL, new LevelTemplate(testLevelLayout));
            //levels.Add(LevelName.FIRST_LEVEL, new LevelTemplate(levelOne));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        private void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            createMainMenu();
            createGame();
            createPauseMenu();

            worldStack.Push(mainMenu);
            activeWorld = worldStack.Peek();

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void Update(GameTime gameTime)
        {

            // TODO: Add your update logic here

            activeWorld.Manager.update(gameTime);
            activeWorld = worldStack.Peek();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        private void createMainMenu()
        {
            mainMenu = new World(Worlds.MAIN_MENU);
            inputListener.addWorld(mainMenu, true);
            mainMenu.Manager.addEntity(EFactory.createButton(50, 50, 50, 50, Inputs.START));
            mainMenu.Manager.addEntity(EFactory.createButton(50, 200, 50, 50, Inputs.EXIT));
        }

        private void createGame()
        {
            inGame = new World(Worlds.IN_GAME);
            inputListener.addWorld(inGame, false);
            GameInputSystem input = new GameInputSystem();

            inGame.Manager.addSystem(new AvatarInputSystem());
            inGame.Manager.addSystem(new GameCollisionSystem());
            inGame.Manager.addSystem(new MovementSystem());
            inGame.Manager.addSystem(new LevelSystem(levels));
            inGame.Manager.addSystem(new AttackSystem());


        }

        private void createPauseMenu()
        {
            pauseMenu = new World(Worlds.PAUSE_MENU);
            inputListener.addWorld(pauseMenu, true);
            pauseMenu.Manager.addEntity(EFactory.createButton(50, 50, 50, 50, Inputs.START));
            pauseMenu.Manager.addEntity(EFactory.createButton(50, 200, 50, 50, Inputs.EXIT));
        }

        private void reset(World world)
        {
            if (world.MenuType == Worlds.IN_GAME)
                createGame();
            else if (world.MenuType == Worlds.MAIN_MENU)
                createMainMenu();
            else if (world.MenuType == Worlds.PAUSE_MENU)
                createPauseMenu();
        }

        private void Exit()
        {
            return;
        }

        private class InputListener
        {
            private Dictionary<World, InputSystem> inputs;
            GameLoop game;

            public InputListener(GameLoop game)
            {
                this.game = game;
                inputs = new Dictionary<World, InputSystem>();
            }



            public void addWorld(World world, bool isMenu)
            {
                InputSystem toAdd;
                if (inputs.ContainsKey(world))
                    return;
                if (isMenu)
                    toAdd = new MenuInputSystem();
                else
                    toAdd = new GameInputSystem();
                world.Manager.addInput(toAdd);
                world.Manager.addSystem((ESystem)toAdd);
                inputs.Add(world, toAdd);
                toAdd.inputFired += new EventHandler(inputFired);
            }

            private void inputFired(object sender, EventArgs e)
            {
                InputArgs args = (InputArgs)e;
                Console.WriteLine(game.worldStack.Peek().MenuType + "=> " + args.InputType);

                if (game.activeWorld.MenuType == Worlds.MAIN_MENU)
                {
                    if (args.InputType == Inputs.START)
                        game.worldStack.Push(game.inGame);
                    if (args.InputType == Inputs.EXIT)
                        game.Exit();
                }

                else if (game.activeWorld.MenuType == Worlds.IN_GAME)
                {
                    if (args.InputType == Inputs.PAUSE)
                    {
                        game.worldStack.Push(game.pauseMenu);
                    }
                }

                else if (game.activeWorld.MenuType == Worlds.PAUSE_MENU)
                {
                    if (args.InputType == Inputs.START)
                        game.worldStack.Pop();
                    if (args.InputType == Inputs.EXIT)
                    {
                        game.worldStack.Pop();
                        game.worldStack.Pop();
                        game.reset(game.inGame);
                    }
                }
                game.activeWorld = game.worldStack.Peek();
            }
        }
    }
}
