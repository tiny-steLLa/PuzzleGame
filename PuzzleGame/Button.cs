using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleGame
{
    class Button : asd.TextureObject2D
    {
        private asd.GeometryObject2D Back;
        public GeneralText ChildText;
        private static asd.Color FrameColor = new asd.Color(0, 200, 200, 200);
        public static Button ChosenButton { get; private set; }
        private static int MaxRow, MaxCol;
        public static Button[,] Buttons { get; private set; }
        public static int CurrentRow { get; private set; }
        public static int CurrentCol { get; private set; }
        int Count, MovingSpeed;//押しっぱなしでボタンが進む
        private int FontSize;
        /// <summary>
        /// キーボードとコントローラーで反応するボタン的なのをつくる、選択時の処理は使用側に
        /// </summary>
        /// <param name="text">ボタンのテキスト</param>
        /// <param name="pos">位置</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        private Button(string text, asd.Vector2DF pos, int width, int height, int fontSize)
        {
            FontSize = fontSize;
            MovingSpeed = 12;
            Position = pos;
            ChildText = new GeneralText(new asd.Vector2DF(0, 0), text, FontSize, new asd.Color(2, 20, 20, 200), 2, new asd.Color(222, 222, 222, 200));
            var rect = new asd.RectangleShape();
            rect.DrawingArea = new asd.RectF(-width / 2, -height / 2, width, height);
            Back = new asd.GeometryObject2D();
            Back.Shape = rect;
            AddChild(Back, asd.ChildManagementMode.RegistrationToLayer, asd.ChildTransformingMode.All);
            AddChild(ChildText, asd.ChildManagementMode.RegistrationToLayer, asd.ChildTransformingMode.All);
        }

        protected override void OnUpdate()
        {
            if (this == Buttons[MaxRow, MaxCol])//一番最後にアップデートされる物体(1回だけ呼びたい)
            {
                void SoundSE() => Sounds.PlaySound(Sounds.SE1);
                if ((Controller.JoystickHoldCheck(14) || asd.Engine.Keyboard.GetKeyState(asd.Keys.Up) == asd.KeyState.Hold) && CurrentRow != 0)
                {
                    if (Count % MovingSpeed == 0)
                    {
                        ChangeCurrentButton(CurrentRow - 1, CurrentCol);
                        SoundSE();
                    }
                    Count++;
                }
                else if ((Controller.JoystickHoldCheck(15) || asd.Engine.Keyboard.GetKeyState(asd.Keys.Right) == asd.KeyState.Hold) && CurrentCol != MaxCol)
                {
                    if (Count % MovingSpeed == 0)
                    {
                        ChangeCurrentButton(CurrentRow, CurrentCol + 1);
                        SoundSE();
                    }
                    Count++;
                }
                else if ((Controller.JoystickHoldCheck(16) || asd.Engine.Keyboard.GetKeyState(asd.Keys.Down) == asd.KeyState.Hold) && CurrentRow != MaxRow)
                {
                    if (Count % MovingSpeed == 0)
                    {
                        ChangeCurrentButton(CurrentRow + 1, CurrentCol);
                        SoundSE();
                    }
                    Count++;
                }
                else if ((Controller.JoystickHoldCheck(17) || asd.Engine.Keyboard.GetKeyState(asd.Keys.Left) == asd.KeyState.Hold) && CurrentCol != 0)
                {
                    if (Count % MovingSpeed == 0)
                    {
                        ChangeCurrentButton(CurrentRow, CurrentCol - 1);
                        SoundSE();
                    }
                    Count++;
                }
                else
                    Count = 0;
            }
        }

        //選ばれているボタンを変化させる
        private static void ChangeCurrentButton(int row, int col)
        {
            Buttons[CurrentRow, CurrentCol].Back.Color = new asd.Color(255, 255, 255, 255);
            CurrentRow = row;
            CurrentCol = col;
            Buttons[row, col].Back.Color = FrameColor;
            ChosenButton = Buttons[row, col];
        }

        /// <summary>
        /// text配列からたくさんボタンを作る、使用側で間隔調節
        /// </summary>
        /// <param name="layer">追加レイヤー</param>
        /// <param name="text">テキストのArray</param>
        /// <param name="UpSpace">上の間隔</param>
        /// <param name="DownSpace">下の間隔</param>
        /// <param name="leftSpace">左の間隔</param>
        /// <param name="rightSpace">右の間隔</param>
        /// <param name="buttonWideSpace">ボタンの間の横間隔</param>
        /// <param name="buttonHeightSpace">ボタンの間の縦間隔</param>
        /// <returns></returns>
        public static void MakeButtonArray(asd.Layer2D layer, string[,] texts, int UpSpace, int DownSpace, int leftSpace, int rightSpace, int buttonWideSpace, int buttonHeightSpace, int fontSize)
        {
            MaxRow = texts.GetLength(0);//縦の個数
            MaxCol = texts.GetLength(1);
            var width = asd.Engine.WindowSize.X - leftSpace - rightSpace;//この物体のX幅
            var height = asd.Engine.WindowSize.Y - UpSpace - DownSpace;
            var buttonWidth = (width - (MaxCol - 1) * buttonWideSpace) / MaxCol;//1つのボタン幅
            var buttonHeight = (height - (MaxRow - 1) * buttonHeightSpace) / MaxRow;
            Buttons = new Button[MaxRow, MaxCol];
            for (int row = 0; row < MaxRow; row++)
                for (int column = 0; column < MaxCol; column++)
                    Buttons[row, column] = new Button(texts[row, column], new asd.Vector2DF(column * (buttonWidth + buttonWideSpace) + buttonWidth / 2 + leftSpace, row * (buttonHeight + buttonHeightSpace) + buttonHeight / 2 + UpSpace), buttonWidth, buttonHeight, fontSize);
            MaxRow--;//個数だと使いずらいから0からのカウントに調節
            MaxCol--;//同様
            CurrentRow = 0;
            CurrentCol = 0;
            ChangeCurrentButton(0, 0);
            foreach (var b in Buttons)
                layer.AddObject(b);
        }
    }
}
