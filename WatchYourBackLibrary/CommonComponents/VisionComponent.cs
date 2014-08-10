using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WatchYourBackLibrary
{
    public class VisionComponent : EComponent
    {
        public override int BitMask { get { return (int)Masks.Vision; } }
        public override Masks Mask { get { return Masks.Vision; } }

        float fieldOfView;
        Polygon visionPolygon;

        public VisionComponent(float fieldOfView)
        {
            this.fieldOfView = MathHelper.ToRadians(fieldOfView);
        }

        public float FOV
        {
            get { return fieldOfView; }
            set { fieldOfView = value; }
        }

        public Polygon VisionField
        {
            get { return visionPolygon; }
            set { visionPolygon = value; }
        }
    }
}
