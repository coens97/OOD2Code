using System.Drawing;
using FlowSystem.Common;

namespace FlowSystem.Presentation.Drawable
{
    public abstract class Drawable
    {
        public PointEntity Position { get; set; }
        public int W { get; set; }
        public int H { get; set; }

        public abstract void Draw(Graphics g);


    }
}