using System;
using System.Collections.Generic;

namespace ReversiGame
{
    public class AIPlayer
    {
        private readonly int depth;

        public AIPlayer(int depth)
        {
            this.depth = depth;
        }

        public (int, int)? GetMove(GameBoard board)
        {
            var moves = board.GetValidMoves(Player.White);
            if (moves.Count == 0) return null;

            int bestScore = int.MinValue;
            (int, int)? bestMove = null;

            foreach (var move in moves)
            {
                var clone = board.Clone();
                clone.MakeMove(move.Item1, move.Item2, Player.White);
                int score = MiniMax(clone, depth - 1, false);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private int MiniMax(GameBoard board, int depth, bool isMax)
        {
            if (depth == 0 || board.IsGameOver())
                return board.Evaluate();

            Player current = isMax ? Player.White : Player.Black;
            var moves = board.GetValidMoves(current);
            if (moves.Count == 0) return board.Evaluate();

            int best = isMax ? int.MinValue : int.MaxValue;

            foreach (var move in moves)
            {
                var clone = board.Clone();
                clone.MakeMove(move.Item1, move.Item2, current);
                int score = MiniMax(clone, depth - 1, !isMax);
                best = isMax ? Math.Max(best, score) : Math.Min(best, score);
            }

            return best;
        }
    }
}
