using System;
using System.Collections.Generic;

namespace ReversiGame
{
    public class GameBoard
    {
        public Player[,] Board { get; private set; }
        private readonly int[][] directions = new int[][]
        {
            new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 },
            new[] { -1, -1 }, new[] { -1, 1 }, new[] { 1, -1 }, new[] { 1, 1 }
        };

        public GameBoard()
        {
            Board = new Player[8, 8];
            Board[3, 3] = Player.White;
            Board[3, 4] = Player.Black;
            Board[4, 3] = Player.Black;
            Board[4, 4] = Player.White;
        }

        public bool IsValidMove(int row, int col, Player player)
        {
            if (Board[row, col] != Player.None) return false;

            foreach (var dir in directions)
            {
                int r = row + dir[0], c = col + dir[1];
                bool hasOpponent = false;

                while (IsInBounds(r, c) && Board[r, c] == player.Opponent())
                {
                    r += dir[0]; c += dir[1]; hasOpponent = true;
                }

                if (hasOpponent && IsInBounds(r, c) && Board[r, c] == player)
                    return true;
            }

            return false;
        }

        public bool MakeMove(int row, int col, Player player)
        {
            if (!IsValidMove(row, col, player)) return false;

            Board[row, col] = player;

            foreach (var dir in directions)
            {
                int r = row + dir[0], c = col + dir[1];
                var toFlip = new List<(int, int)>();

                while (IsInBounds(r, c) && Board[r, c] == player.Opponent())
                {
                    toFlip.Add((r, c));
                    r += dir[0]; c += dir[1];
                }

                if (IsInBounds(r, c) && Board[r, c] == player)
                    foreach (var (fr, fc) in toFlip)
                        Board[fr, fc] = player;
            }

            return true;
        }

        public List<(int, int)> GetValidMoves(Player player)
        {
            var moves = new List<(int, int)>();
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                    if (IsValidMove(r, c, player))
                        moves.Add((r, c));
            return moves;
        }

        public bool IsGameOver()
        {
            return GetValidMoves(Player.Black).Count == 0 && GetValidMoves(Player.White).Count == 0;
        }

        public int Evaluate()
        {
            int score = 0;
            foreach (var cell in Board)
                score += cell == Player.White ? 1 : cell == Player.Black ? -1 : 0;
            return score;
        }

        public GameBoard Clone()
        {
            var clone = new GameBoard();
            Array.Copy(this.Board, clone.Board, this.Board.Length);
            return clone;
        }

        private bool IsInBounds(int r, int c) => r >= 0 && r < 8 && c >= 0 && c < 8;
    }
}
