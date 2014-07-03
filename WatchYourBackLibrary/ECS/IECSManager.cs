using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WatchYourBackLibrary
{
    public interface IECSManager
    {
        

        void addSystem(ESystem system);

        void removeSystem(ESystem system);   
     
        void addEntity(Entity entity);

        void removeEntity(Entity entity);

        void addInput(InputSystem input);
        List<Entity> Entities
        {
            get;
        }

        List<Entity> ActiveEntities
        {
            get;
        }

        void clearEntities();
       

        /*
         * Updates the entity lists of the manager, moving active/inactive entities to their proper lists. Any systems that run
         * during the update loop are then updated.
         */
        void update(GameTime gameTime);

        void draw(SpriteBatch spriteBatch);

        Texture2D getTexture(string fileName);

        void addContent(ContentManager content);
       
        InputSystem Input
        {
            get;
        }
        
    }
}
