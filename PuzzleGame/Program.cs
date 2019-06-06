using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleGame
{
    class Program
    {
        static void Main(string[] args)
        {
            asd.Engine.Initialize("ぱずる", 630, 840, new asd.EngineOption());
            asd.Engine.File.AddRootDirectory("resource/");
            var scene = new TitleScene();
            asd.Engine.ChangeSceneWithTransition(scene, new asd.TransitionFade(0, 1));
            while (asd.Engine.DoEvents())
            {
                if (asd.Engine.Keyboard.GetKeyState(asd.Keys.Escape) == asd.KeyState.Push)
                {
                    break;
                }
                 asd.Engine.Update();
            }
            asd.Engine.Terminate();
        }
    }
}
