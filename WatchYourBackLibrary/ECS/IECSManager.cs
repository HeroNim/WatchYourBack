using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WatchYourBackLibrary
{

    /* An interface for the class which manages the systems in the game. Is responsible for initializing, updating, and removing systems as needed. Also contains a list of
     * all the entities which has changed during the last update cycle; this allows for the server to send data to the client on what needs
     * to be removed, added, or modified.
     */


    public interface IECSManager
    {
        

        void addSystem(ESystem system);

        void removeSystem(ESystem system);   
     
        void addEntity(Entity entity);

        void removeEntity(Entity entity);

        void addInput(InputSystem input);

        Dictionary<int, Entity> Entities
        {
            get;
        }

        Dictionary<int, Entity> ActiveEntities
        {
            get;
        }

        Dictionary<int, COMMANDS> ChangedEntities
        {
            get;
        }

        void addChangedEntities(Entity e, COMMANDS c);

        void clearEntities();

        void RemoveAll();

        
        double[] Accumulator { get; set; }
       

        /*
         * Updates the entity lists of the manager, moving active/inactive entities to their proper lists. Any systems that run
         * during the update loop are then updated.
         */
        void update(TimeSpan gameTime);

        void draw(SpriteBatch spriteBatch);

        bool hasGraphics();

        Texture2D getTexture(string fileName);

        void addContent(ContentManager content);
       
        InputSystem Input
        {
            get;
        }
        
    }
}
