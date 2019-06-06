using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleGame
{
    static class Sounds
    {
        public static asd.SoundSource SE1 = asd.Engine.Sound.CreateSoundSource("16bit_0001_select_1.wav", true);
        public static asd.SoundSource SE2 = asd.Engine.Sound.CreateSoundSource("16bit_0002_select_2.wav", true);
        public static asd.SoundSource SE3 = asd.Engine.Sound.CreateSoundSource("16bit_0003_select_3.wav", true);
        public static asd.SoundSource SE4 = asd.Engine.Sound.CreateSoundSource("16bit_0004_select_4.wav", true);
        public static asd.SoundSource SE5 = asd.Engine.Sound.CreateSoundSource("16bit_0006_select_5.wav", true);
        public static asd.SoundSource SE6 = asd.Engine.Sound.CreateSoundSource("16bit_0007_select_6.wav", true);
        public static asd.SoundSource SE7 = asd.Engine.Sound.CreateSoundSource("16bit_0009_select_7.wav", true);
        public static asd.SoundSource SE8 = asd.Engine.Sound.CreateSoundSource("16bit_0010_select_8.wav", true);
        public static asd.SoundSource SE9 = asd.Engine.Sound.CreateSoundSource("16bit_0012_select_9.wav", true);
        public static asd.SoundSource SE10 = asd.Engine.Sound.CreateSoundSource("16bit_0013_select_10.wav", true);
        public static asd.SoundSource SelectSE1 = asd.Engine.Sound.CreateSoundSource("16bit_0019_panel_1.wav", true);
        public static asd.SoundSource SelectSE2 = asd.Engine.Sound.CreateSoundSource("16bit_0020_panel_2.wav", true);
        public static asd.SoundSource SelectSE3 = asd.Engine.Sound.CreateSoundSource("16bit_0022_panel_3_syuko.wav", true);
        public static bool IsMute = false;
        /// <summary>
        /// asd.Engine.Sound.Playの代わりにこっちを使う
        /// </summary>
        /// <param name="se"></param>
        public static void PlaySound(asd.SoundSource se)
        {
            if(!IsMute)
            asd.Engine.Sound.Play(se);
        }
    }
}
