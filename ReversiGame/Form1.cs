using System;
using System.Drawing;
using System.Windows.Forms;

namespace ReversiGame
{
    public partial class Form1 : Form
    {
        private const int CellSize = 60;
        private const int OffsetY = 40;

        private GameBoard gameBoard;
        private AIPlayer aiPlayer;
        private Player currentPlayer = Player.Black;

        private ComboBox difficultyBox;
        private Button restartButton;
        private CheckBox twoPlayerModeBox;

        private bool twoPlayerMode = false;
        private System.Windows.Forms.Timer aiTimer;

        public Form1()
        {
            InitializeComponent();

            this.Text = "Reversi Game - MiniMax AI or Two Player";
            this.DoubleBuffered = true;
            this.ClientSize = new Size(CellSize * 8, CellSize * 8 + OffsetY);

            gameBoard = new GameBoard();
            aiPlayer = new AIPlayer(3); // Default difficulty

            SetupUI();
            SetupAITimer();

            this.MouseClick += Form1_MouseClick;
        }

        private void SetupUI()
        {
            // Difficulty selector
            difficultyBox = new ComboBox
            {
                Location = new Point(10, 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            difficultyBox.Items.AddRange(new string[] { "Beginner", "Standard", "Advanced" });
            difficultyBox.SelectedIndex = 1;
            difficultyBox.SelectedIndexChanged += (s, e) =>
            {
                int depth = difficultyBox.SelectedIndex switch
                {
                    0 => 1,
                    1 => 3,
                    2 => 5,
                    _ => 3
                };
                aiPlayer = new AIPlayer(depth);
            };
            Controls.Add(difficultyBox);

            // Two-player checkbox
            twoPlayerModeBox = new CheckBox
            {
                Text = "Two Player Mode",
                Location = new Point(150, 10),
                AutoSize = true
            };
            twoPlayerModeBox.CheckedChanged += (s, e) =>
            {
                twoPlayerMode = twoPlayerModeBox.Checked;
                aiTimer.Stop(); // Stop AI if switching
                Invalidate();
            };
            Controls.Add(twoPlayerModeBox);

            // Restart button
            restartButton = new Button
            {
                Text = "Restart",
                Location = new Point(300, 8),
                AutoSize = true
            };
            restartButton.Click += (s, e) =>
            {
                gameBoard = new GameBoard();
                currentPlayer = Player.Black;
                aiTimer.Stop();
                Invalidate();
            };
            Controls.Add(restartButton);
        }

        private void SetupAITimer()
        {
            aiTimer = new System.Windows.Forms.Timer();
            aiTimer.Interval = 500;
            aiTimer.Tick += (s, e) =>
            {
                aiTimer.Stop();

                if (currentPlayer == Player.White)
                {
                    var move = aiPlayer.GetMove(gameBoard);
                    if (move != null)
                    {
                        gameBoard.MakeMove(move.Value.Item1, move.Value.Item2, Player.White);
                        currentPlayer = Player.Black;
                        Invalidate();
                    }
                }
            };
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int col = e.X / CellSize;
            int row = (e.Y - OffsetY) / CellSize;
            if (row < 0 || row >= 8 || col < 0 || col >= 8) return;

            if (gameBoard.IsValidMove(row, col, currentPlayer))
            {
                gameBoard.MakeMove(row, col, currentPlayer);
                currentPlayer = currentPlayer.Opponent();
                Invalidate();

                if (!twoPlayerMode && currentPlayer == Player.White)
                    aiTimer.Start();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TranslateTransform(0, OffsetY);

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    g.DrawRectangle(Pens.Black, c * CellSize, r * CellSize, CellSize, CellSize);
                    var cell = gameBoard.Board[r, c];

                    if (cell == Player.Black)
                        g.FillEllipse(Brushes.Black, c * CellSize + 5, r * CellSize + 5, CellSize - 10, CellSize - 10);
                    else if (cell == Player.White)
                        g.FillEllipse(Brushes.White, c * CellSize + 5, r * CellSize + 5, CellSize - 10, CellSize - 10);
                    else if (gameBoard.IsValidMove(r, c, currentPlayer))
                        g.FillEllipse(Brushes.LightGreen, c * CellSize + 20, r * CellSize + 20, CellSize - 40, CellSize - 40);
                }
            }

            g.ResetTransform();

            // Optional: draw turn indicator
            string status = gameBoard.IsGameOver() ? "Game Over" : $"Current Turn: {currentPlayer}";
            g.DrawString(status, Font, Brushes.Black, new PointF(10, 5));
        }
    }
}
