namespace ReversiGame
{
    public enum Player
    {
        None = 0,
        Black = 1,
        White = 2
    }

    public static class PlayerExtensions
    {
        public static Player Opponent(this Player p)
        {
            return p == Player.Black ? Player.White : Player.Black;
        }
    }
}
