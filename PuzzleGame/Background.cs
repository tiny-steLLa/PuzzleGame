using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleGame
{
    class Background:asd.TextureObject2D
    {
        public Background()
        {
            Texture = asd.Engine.Graphics.CreateTexture2D("Background.png");
        }
    }
}
