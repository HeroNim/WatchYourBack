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
        Circle closeRangeVision;
        Polygon visionPolygon;

        public VisionComponent(float fieldOfView, Vector2 center, float radius)
        {
            this.fieldOfView = MathHelper.ToRadians(fieldOfView);
            closeRangeVision = new Circle(center, radius);
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

        public Circle CloseRangeVision
        {
            get { return closeRangeVision; }
        }
    }
}
