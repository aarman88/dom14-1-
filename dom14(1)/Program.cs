using System;
using System.Collections.Generic;
using System.Linq;

class Karta
{
    public string Suit { get; }
    public string Rank { get; }

    public Karta(string suit, string rank)
    {
        Suit = suit;
        Rank = rank;
    }
}

class Player
{
    public string Name { get; }
    public List<Karta> Hand { get; }

    public Player(string name)
    {
        Name = name;
        Hand = new List<Karta>();
    }

    public void DisplayHand()
    {
        Console.WriteLine($"{Name}'s hand: {string.Join(", ", Hand.Select(card => $"{card.Rank} of {card.Suit}"))}");
    }
}

class Game
{
    private List<Player> players;
    private List<Karta> deck;

    public Game(int playersCount)
    {
        if (playersCount < 2)
        {
            throw new ArgumentException("Количество игроков должно быть не менее 2.");
        }

        players = Enumerable.Range(1, playersCount).Select(i => new Player($"Игрок {i}")).ToList();
        deck = GenerateDeck();
        ShuffleDeck();
        DealCards();
    }

    private List<Karta> GenerateDeck()
    {
        string[] suits = { "Черви", "Бубны", "Крести", "Пики" };
        string[] ranks = { "6", "7", "8", "9", "10", "Валет", "Дама", "Король", "Туз" };

        return suits.SelectMany(suit => ranks.Select(rank => new Karta(suit, rank))).ToList();
    }

    private void ShuffleDeck()
    {
        Random random = new Random();
        deck = deck.OrderBy(card => random.Next()).ToList();
    }

    private void DealCards()
    {
        int cardsPerPlayer = deck.Count / players.Count;

        foreach (var player in players)
        {
            player.Hand.AddRange(deck.Take(cardsPerPlayer));
            deck = deck.Skip(cardsPerPlayer).ToList();
        }
    }

    private int CardValue(Karta card)
    {
        string[] ranksOrder = { "6", "7", "8", "9", "10", "Валет", "Дама", "Король", "Туз" };
        return Array.IndexOf(ranksOrder, card.Rank);
    }

    public void PlayRound()
    {
        Dictionary<Player, Karta> playedCards = players.ToDictionary(player => player, player => player.Hand.First());
        Player winningPlayer = playedCards.OrderByDescending(pair => CardValue(pair.Value)).First().Key;

        foreach (var player in players)
        {
            player.Hand.Remove(playedCards[player]);
            winningPlayer.Hand.AddRange(playedCards.Values);
        }
    }

    public void PlayGame()
    {
        while (!players.Any(player => player.Hand.Count == deck.Count))
        {
            PlayRound();
        }

        Player winner = players.OrderByDescending(player => player.Hand.Count).First();
        Console.WriteLine($"Победитель: {winner.Name}");
    }
}

class Program
{
    static void Main()
    {
        Game game = new Game(playersCount: 3);
        game.PlayGame();
    }
}
