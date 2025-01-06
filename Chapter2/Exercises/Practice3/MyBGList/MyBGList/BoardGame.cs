namespace MyBGList
{
    public class BoardGame
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Year { get; set; }
        public int MinPlayers { get; set; } = 1;
        public int MaxPlayers { get; set; } = 1;
    }
}
