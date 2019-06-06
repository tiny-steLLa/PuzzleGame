using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleGame
{
    class Controller
    {
        private static asd.Joystick joystick = asd.Engine.JoystickContainer.GetJoystickAt(0);
        public static bool JoystickHoldCheck(int t) => asd.Engine.JoystickContainer.GetIsPresentAt(0) && joystick.GetButtonState(t) == asd.JoystickButtonState.Hold;
        public static bool JoystickPushCheck(int t) => asd.Engine.JoystickContainer.GetIsPresentAt(0) && joystick.GetButtonState(t) == asd.JoystickButtonState.Push;
    }
}
