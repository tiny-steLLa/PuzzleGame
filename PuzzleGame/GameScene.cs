using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleGame
{
    //PieceのPositionはPieceWidthの倍数で取る
    class GameScene : asd.Scene
    {
        protected asd.Layer2D GameLayer, BackgroundLayer;
        protected Piece[,] Pieces;
        protected Piece TransparentPiece;//空きマスの位置を記憶しておく
        protected static int PieceNum;//N×Nのピース数
        protected int PieceWidth;//１ピースの大きさ
        protected const int ShuffleNum = 10000;//混ぜる回数
        private Random Rnd = new Random();
        private String TextureName;
        private List<Piece> PieceList;
        private asd.Vector2DI WindowSize = asd.Engine.WindowSize;
        private int MoveCount, MoveSpeed, Minutes;//MoveSpeed:Holdで進む速さを調整する
        private bool IsClear, IsTimeCountMax, IsReverse;//クリアフラグ、59m:59sでtrue、操作の向き反転か？
        private float Seconds;
        private GeneralText TimeText;

        public GameScene(int pieceNum, int textureNum, bool isReverse)
        {
            Seconds = 0;
            Minutes = 0;
            MoveCount = 0;
            MoveSpeed = 8;
            GameLayer = new asd.Layer2D();
            BackgroundLayer = new asd.Layer2D();
            AddLayer(BackgroundLayer);
            AddLayer(GameLayer);
            IsTimeCountMax = false;
            IsClear = false;
            IsReverse = isReverse;
            PieceNum = pieceNum;
            switch (textureNum)
            {
                case 0:
                    TextureName = "00476.png";
                    break;
                case 1:
                    TextureName = "00477.png";
                    break;
                case 2:
                    TextureName = "puzzle.png";
                    break;
            }
        }

        protected override void OnRegistered()
        {
            BackgroundLayer.AddObject(new Background());
            Pieces = new Piece[PieceNum, PieceNum];
            PieceList = new List<Piece>();
            MakePuzzle(Pieces);
            TimeText = new GeneralText(new asd.Vector2DF(WindowSize.X / 4, PieceWidth * PieceNum + 50), Seconds.ToString(), 30, new asd.Color(0, 0, 0, 255), 4, new asd.Color(255, 255, 255, 255));
            GameLayer.AddObject(TimeText);
            Shuffle(Pieces, ShuffleNum);
        }

        protected override void OnUpdated()
        {
            if (!IsClear && !IsTimeCountMax)
            {

                Seconds += asd.Engine.DeltaTime;
                if (Seconds >= 60)
                {
                    Seconds -= 60;
                    Minutes++;
                }
                TimeText.Text = Minutes.ToString() + ":" + Seconds.ToString();
            }
            if (Minutes == 60)
            {
                IsTimeCountMax = true;
                TimeText.Text = "59:59.9999";
            }
            MoveCheck();
            var joystick = asd.Engine.JoystickContainer.GetJoystickAt(0);
            bool joystickCheck(int t) => asd.Engine.JoystickContainer.GetIsPresentAt(0) && joystick.GetButtonState(t) == asd.JoystickButtonState.Push;
            if (asd.Engine.Keyboard.GetKeyState(asd.Keys.Space) == asd.KeyState.Push || joystickCheck(1))
                asd.Engine.ChangeSceneWithTransition(new TitleScene(), new asd.TransitionFade(0, 1));
        }

        protected void MakePuzzle(Piece[,] pieces)
        {
            var texture = asd.Engine.Graphics.CreateTexture2D(TextureName);//大きさ計算用
            if (texture == null)
            {
                //puzzle.pngがない時のデフォを設定
                TextureName = "00476.png";
                texture = asd.Engine.Graphics.CreateTexture2D(TextureName);
            }
            var scale = new asd.Vector2DF((float)WindowSize.X / texture.Size.X, (float)WindowSize.X / texture.Size.Y);//元画像の大きさ調整値
            PieceWidth = WindowSize.X / PieceNum;
            for (int row = 0; row < PieceNum; row++)
            {
                for (int col = 0; col < PieceNum; col++)
                {
                    pieces[row, col] = new Piece(new asd.Vector2DF(row * PieceWidth, col * PieceWidth));
                    pieces[row, col].Texture = asd.Engine.Graphics.CreateTexture2D(TextureName);
                    pieces[row, col].Src = new asd.RectF(row * texture.Size.X / PieceNum, col * texture.Size.Y / PieceNum, texture.Size.X / PieceNum, texture.Size.Y / PieceNum);
                    pieces[row, col].Scale = scale * new asd.Vector2DF(Piece.AdjustWidth, Piece.AdjustWidth);//ピースの間隔をとるための調整
                    GameLayer.AddObject(pieces[row, col]);
                    PieceList.Add(pieces[row, col]);
                }
            }
            TransparentPiece = new Piece(new asd.Vector2DF((PieceNum - 1) * PieceWidth, (PieceNum - 1) * PieceWidth));
            GameLayer.AddObject(TransparentPiece);
            PieceList.Add(TransparentPiece);
        }

        /// <summary>
        /// 第二引数は偶数でないとクリア不可能な盤面になる
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="repeatNum">混ぜ回数</param>
        protected void Shuffle(Piece[,] piece, int repeatNum)
        {
            for (int i = 0; i < repeatNum; i++)
            {
                Piece tempPiece = piece[Rnd.Next(PieceNum), Rnd.Next(PieceNum)];
                Piece tempPiece2 = piece[Rnd.Next(PieceNum), Rnd.Next(PieceNum)];
                //混ぜ回数調整、一番右下のピースと交換させない、同じ位置だと混ざらないのでもう一回
                if (tempPiece == piece[PieceNum - 1, PieceNum - 1] || tempPiece2 == piece[PieceNum - 1, PieceNum - 1] || tempPiece == tempPiece2)
                {
                    i--;
                    continue;
                }
                tempPiece.SwapPos(tempPiece2);
            }
            //初手クリア防止
            if (ClearCheck())
                Shuffle(Pieces, ShuffleNum);
            piece[PieceNum - 1, PieceNum - 1].Position = new asd.Vector2DF((PieceNum - 1) * PieceWidth, PieceNum * PieceWidth);
        }

        //動けるならtrue、クリック操作で使用
        protected bool CanMove(Piece piece)
        {
            if (piece.Position == TransparentPiece.Position - new asd.Vector2DF(PieceWidth, 0)
                || piece.Position == TransparentPiece.Position + new asd.Vector2DF(PieceWidth, 0)
                || piece.Position == TransparentPiece.Position - new asd.Vector2DF(0, PieceWidth)
                || piece.Position == TransparentPiece.Position + new asd.Vector2DF(0, PieceWidth))
                return true;
            else
                return false;
        }

        //ボタン押しorクリックチェックとその動作
        protected void MoveCheck()
        {
            bool isMoved = false;
            //keyとjoystickと反転操作の状態をチェックする
            bool AllStateCheck(asd.Keys key, int t, bool isReverse) => (asd.Engine.Keyboard.GetKeyState(key) == asd.KeyState.Hold || Controller.JoystickHoldCheck(t)) && IsReverse == isReverse;
            if (asd.Engine.Mouse.LeftButton.ButtonState == asd.MouseButtonState.Push)
            {
                int pointX = (int)asd.Engine.Mouse.Position.X / PieceWidth;
                int pointY = (int)asd.Engine.Mouse.Position.Y / PieceWidth;
                if (pointY > PieceNum - 1 && pointX != PieceNum - 1)//右下に配慮した条件
                    return;
                var selectedPiece = PieceList.Find(p => p.Position == new asd.Vector2DF(pointX * PieceWidth, pointY * PieceWidth));
                if (CanMove(selectedPiece))

                    selectedPiece.SwapPos(TransparentPiece);
                isMoved = true;
            }
            else if (AllStateCheck(asd.Keys.Down, 14, true) || AllStateCheck(asd.Keys.Up, 16, false))
            {
                KeyControl(0, ref isMoved);
            }
            else if (AllStateCheck(asd.Keys.Right, 17, true) || AllStateCheck(asd.Keys.Left, 15, false))
            {
                KeyControl(1, ref isMoved);
            }
            else if (AllStateCheck(asd.Keys.Up, 16, true) || AllStateCheck(asd.Keys.Down, 14, false))
            {
                KeyControl(2, ref isMoved);
            }
            else if (AllStateCheck(asd.Keys.Left, 15, true) || AllStateCheck(asd.Keys.Right, 17, false))
            {
                KeyControl(3, ref isMoved);
            }
            else
            {
                MoveCount = 0;
            }
            if (isMoved && !IsClear)
            {
                Sounds.PlaySound(Sounds.SelectSE3);
            }
            //クリア判断
            if (isMoved && TransparentPiece.Position == new asd.Vector2DF((PieceNum - 1) * PieceWidth, PieceNum * PieceWidth))
            {
                if (ClearCheck())
                {
                    IsClear = true;
                    foreach (var piece in Pieces)
                        piece.Dispose();
                    var texture = asd.Engine.Graphics.CreateTexture2D(TextureName);//大きさ計算用
                    var scale = new asd.Vector2DF((float)WindowSize.X / texture.Size.X, (float)WindowSize.X / texture.Size.Y);//元画像の大きさ調整値
                    var clearTexture = new Piece(new asd.Vector2DF(0, 0))
                    {
                        Texture = asd.Engine.Graphics.CreateTexture2D(TextureName),
                        Scale = new asd.Vector2DF((float)WindowSize.X / texture.Size.X, (float)WindowSize.X / texture.Size.Y)
                    };
                    GameLayer.AddObject(clearTexture);
                    GameLayer.AddObject(new GeneralText(new asd.Vector2DF(WindowSize.X / 2, PieceWidth * PieceNum + 140), "    " + PieceNum + "×" + PieceNum + " " + "Clear!!\n" + "スペースキーでタイトルへ", 40, new asd.Color(0, 0, 0, 255), 4, new asd.Color(255, 255, 255, 255)));
                }
            }
        }
        /// <summary>
        /// Count++される
        /// </summary>
        /// <param name="moveNum">周囲マス基準で0が上、１が右、2が下、3が左移動</param>
        /// <param name="isMoved"></param>
        protected void KeyControl(int moveNum, ref bool isMoved)
        {
            if (MoveCount % MoveSpeed == 0)
            {
                var table = new Dictionary<int, asd.Vector2DF>
                {
                    {0,TransparentPiece.Position + new asd.Vector2DF(0,PieceWidth)},
                    {1,TransparentPiece.Position + new asd.Vector2DF(PieceWidth,0)},
                    {2,TransparentPiece.Position + new asd.Vector2DF(0,-PieceWidth)},
                    {3,TransparentPiece.Position + new asd.Vector2DF(-PieceWidth,0)}
                };
                var tempPiece = PieceList.Find(p => p.Position == table[moveNum]);
                if (tempPiece != null)
                {
                    TransparentPiece.SwapPos(tempPiece);
                    isMoved = true;
                }
            }
            MoveCount++;
        }

        //位置入れ替え
        protected void SwapPos(Piece p1, Piece p2)
        {
            var tempPos = p1.Position;
            p1.Position = p2.Position;
            p2.Position = tempPos;
        }

        //クリアしているならtrueを返す
        protected bool ClearCheck()
        {
            for (int row = 0; row < PieceNum; row++)
                for (int col = 0; col < PieceNum; col++)
                    if (Pieces[row, col].Position != new asd.Vector2DF(row * PieceWidth, col * PieceWidth))
                        return false;
            return true;
        }
    }
}