using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchYourBackLibrary
{
    [Serializable()]
    public enum COMMANDS
    {
        ADD,
        REMOVE,
        MODIFY
    }

    [Serializable()]
    public class NetworkEntityArgs : EventArgs
    {
        private COMMANDS command;
        private ENTITIES type;
        private int id;
        private float xPos;
        private float yPos;
        private int width;
        private int height;
        private float rotation;

        public NetworkEntityArgs(ENTITIES type, COMMANDS command, int id, float xPos, float yPos, int width, int height, float rotation)
        {
            this.type = type;
            this.id = id;
            this.xPos = xPos;
            this.yPos = yPos;
            this.width = width;
            this.height = height;
            this.rotation = rotation;
            this.command = command;
        }

        public COMMANDS Command { get { return command; } }
        public ENTITIES Type { get { return type; } }
        public int ID { get { return id; } }
        public float XPos { get { return xPos; } }
        public float YPos { get { return yPos; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public float Rotation { get { return rotation; } }

    }
}
