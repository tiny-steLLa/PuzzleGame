using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleGame
{
    class TitleScene : asd.Scene
    {
        protected asd.Layer2D ButtonLayer;
        protected asd.Layer2D BackgroundLayer;
        private static int WhichPicture = 0;//選択画像、シーンが変わっても状態を記憶させる
        private static bool IsReverse = false;//操作反転、上同様
        int PictureNum;//画像総数（自分で用意したのも含む）
        private int FontSize;
        private Dictionary<int, string> PictureTable;
        private Dictionary<bool, string> ReverseTable, MuteTable;
        protected string[,] ButtonText;
        public TitleScene()
        {
            PictureTable = new Dictionary<int, string>()
            {
                // TODO: enum利用
                {0,"画像選択\n=またたび" },
                {1, "画像選択\n=ししゃも"},
                {2, "画像選択\n=puzzle" },
            };
            ReverseTable = new Dictionary<bool, string>()
            {
                { false,"操作基準\n周囲マス"},
                { true,"操作基準\n空きマス"}
            };
            MuteTable = new Dictionary<bool, string>()
            {
                {false, "音有り" },
                {true,"音無し" }
            };
            ButtonText = new string[,]
            {
                { "3×3" ,"4×4","5×5"},
                { "6×6","7×7", "8×8" },
                { MuteTable[Sounds.IsMute],ReverseTable[IsReverse], PictureTable[WhichPicture]},
            };
            FontSize = 20;
        }
        protected override void OnRegistered()
        {
            PictureNum = 3;
            BackgroundLayer = new asd.Layer2D();
            ButtonLayer = new asd.Layer2D();
            AddLayer(BackgroundLayer);
            AddLayer(ButtonLayer);
            BackgroundLayer.AddObject(new Background());
            BackgroundLayer.AddObject(new GeneralText(new asd.Vector2DF(asd.Engine.WindowSize.X / 2, 100), "ぱずる", 50, new asd.Color(0, 0, 0, 255), 4, new asd.Color(255, 255, 255, 255)));
            Button.MakeButtonArray(ButtonLayer, ButtonText, 200, 200, 50, 50, 20, 20, FontSize);
        }
        protected override void OnUpdated()
        {
            if (asd.Engine.Keyboard.GetKeyState(asd.Keys.Space) == asd.KeyState.Push || Controller.JoystickPushCheck(2))
                PushedEnter();
            base.OnUpdated();
        }
        protected void PushedEnter()
        {
            Sounds.PlaySound(Sounds.SE6);
            int t = 3 * Button.CurrentRow + Button.CurrentCol + 3;
            //音
            if (t == 9)
            {
                Sounds.IsMute = !Sounds.IsMute;
                Button.Buttons[2, 0].ChildText.Text = MuteTable[Sounds.IsMute];
            }
            //操作反転
            else if (t == 10)
            {
                IsReverse = !IsReverse;
                Button.Buttons[2, 1].ChildText.Text = ReverseTable[IsReverse];
            }
            //画像変更
            else if (t == 11)
            {
                WhichPicture++;
                if (WhichPicture >= PictureNum)
                    WhichPicture = 0;
                Button.Buttons[2, 2].ChildText.Text = PictureTable[WhichPicture];
            }
            else
                asd.Engine.ChangeScene(new GameScene(t, WhichPicture, IsReverse));
        }

    }
}
