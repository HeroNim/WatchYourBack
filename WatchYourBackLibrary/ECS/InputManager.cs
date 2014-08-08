using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WatchYourBackLibrary
{
    ///<summary>
    ///An interface which input systems can use to identify themselves
    ///</summary>
    public static class InputManager
    {
        private static Dictionary<World, List<ESystem>> inputs = new Dictionary<World, List<ESystem>>();
        private static World activeWorld;

        public static void addInput(World world, ESystem input)
        {
            if (!inputs.ContainsKey(world))
                inputs.Add(world, new List<ESystem>());
            inputs[world].Add(input);
        }

        public static void removeWorld(World world)
        {
            if (inputs.ContainsKey(world))
            {
                inputs.Remove(world);
            }
        }

        public static void setActiveWorld(World world)
        {
            activeWorld = world;
        }

        public static bool checkIfActive(ESystem system)
        {
            if (inputs[activeWorld].Contains(system))
                return true;
            return false;
        }
    }
}
