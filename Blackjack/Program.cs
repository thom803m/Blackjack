using System;
using System.Collections.Generic;

namespace BlackjackOOP
{
    class Program
    {
        static void Main(string[] args)
        {
            // Spillet startes ved at lave en instans af Game og kalde på Start metoden.
            Game game = new Game();
            game.Start();
        }
    }

    // Kort klassen
    // Repræsenterer et enkelt kort med kulør, rang (værdi) og pointværdi.
    public class Card
    {
        public string Suit { get; private set; }   // Kulør (hjerte, spar, klør, ruder)
        public string Rank { get; private set; }   // Rang (2, 3, ..., J, Q, K, A)
        public int Value { get; private set; }     // Den numeriske værdi af kortet

        public Card(string suit, string rank, int value)
        {
            Suit = suit;
            Rank = rank;
            Value = value;
        }

        public int GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            // Når et kort skrives ud, fx "Hjerte A"
            return $"{Suit} {Rank}";
        }
    }

    // Kortbunke klassen
    // Indeholder en liste af kort, som kan blandes og trækkes fra.
    public class Deck
    {
        private List<Card> Cards = new List<Card>();   // Kortlisten
        private Random random = new Random();             // Til blanding

        public Deck()
        {
            Reset(); // Bunken samles (eller retter når man laver et nyt deck) - bunken bliver det fyldt op igen og blandet
        }

        // Fylder bunken med 52 kort, hvor der er 4 kulører x 13 rang
        public void Reset()
        {
            Cards.Clear();
            string[] suits = { "Hjerte", "Rude", "Klør", "Spar" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

            foreach (string suit in suits)
            {
                foreach (string rank in ranks)
                {
                    int value;
                    if (int.TryParse(rank, out value))   // Hvis det er et tal (2–10)
                        value = int.Parse(rank);
                    else if (rank == "A")                // Es starter som 11
                        value = 11;
                    else                                 // Billedkort (J, Q, K) = 10
                        value = 10;

                    Cards.Add(new Card(suit, rank, value));
                }
            }
            Shuffle();
        }

        // Her blandes kortene
        public void Shuffle()
        {
            for (int i = Cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
            }
        }

        // Her trækker der det øverste kort
        public Card DrawCard()
        {
            Card card = Cards[0];
            Cards.RemoveAt(0);
            return card;
        }
    }

    // Spiller klassen
    // Repræsenterer en spiller (eller dealer) med navn og hånd.
    public class Player
    {
        public string Name { get; private set; }
        public List<Card> Hand = new List<Card>(); // Kortene spilleren har på hånden

        public Player(string name)
        {
            Name = name;
        }

        public void AddCard(Card card)
        {
            Hand.Add(card);
        }

        // Beregner den samlede værdi af hånden
        public int GetHandValue()
        {
            int value = 0;
            foreach (Card card in Hand)
            {
                value += card.GetValue();
            }
            return value;
        }

        // Tjekker om spilleren er (Bust), hvilket er over 21
        public bool IsBust()
        {
            return GetHandValue() > 21;
        }

        // Rydder hånden - til ny runde
        public void ClearHand()
        {
            Hand.Clear();
        }

        // Viser spillerens hånd. Hvis hideFirstCard = true, skjules det første kort (brugt til dealer).
        public void ShowHand(bool hideFirstCard = false)
        {
            Console.WriteLine($"{Name}'s hånd:");
            for (int i = 0; i < Hand.Count; i++)
            {
                if (i == 0 && hideFirstCard)
                    Console.WriteLine("[Skjult kort]");
                else
                    Console.WriteLine(Hand[i]);
            }

            if (!hideFirstCard)
                Console.WriteLine($"Samlet værdi på: {GetHandValue()}");
            Console.WriteLine();
        }

        // Tjekker om spilleren har blackjack (2 kort der giver 21)
        public bool HasBlackjack()
        {
            return Hand.Count == 2 && GetHandValue() == 21;
        }
    }

    // Dealer klassen
    // Dealeren er en slags spiller, som arves fra spiller, men med sin egen logik for hvordan den spiller.
    public class Dealer : Player
    {
        public Dealer() : base("Dealeren") { }

        // Dealerens trækker kort indtil hånden er på mindst 17.
        public void PlayTurn(Deck deck)
        {
            ShowHand();
            while (GetHandValue() < 17)
            {
                Console.WriteLine("Dealeren trækker et kort...");
                AddCard(deck.DrawCard());
                ShowHand();
            }
        }
    }

    // Spil klassen
    // Her styres hele spillet, fra setup til runder, spillernes- og dealerens tur, og vinderen.
    public class Game
    {
        private Deck Deck;
        private List<Player> Players = new List<Player>();
        private Dealer Dealer;

        public void Start()
        {
            Dealer = new Dealer();

            // Spørger hvor mange spillere (1–4)
            int playerCount;
            while (true)
            {
                Console.Write("Hvor mange spillere (1-4)? ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out playerCount) && playerCount >= 1 && playerCount <= 4)
                    break;
                Console.WriteLine("Ugyldigt antal spillere. Indtast et tal mellem 1 og 4.");
            }

            // Opretter spillere med navne
            for (int i = 1; i <= playerCount; i++)
            {
                string name;
                while (true)
                {
                    Console.Write($"Spiller {i}, hvad er dit navn? ");
                    name = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(name))
                        break;
                    Console.WriteLine("Navnet kan ikke være tomt.");
                }
                Players.Add(new Player(name));
            }

            bool continuePlaying = true;
            while (continuePlaying)
            {
                Deck = new Deck();
                DealInitialCards(); // Deler kortene ud

                // Spillernes tur
                foreach (var player in Players)
                {
                    PlayerTurn(player);
                }

                // Dealerens tur
                DealerTurn();

                // Afgør hvem der vinder
                DetermineWinner();

                // Spørger om ny runde
                string answer;
                while (true)
                {
                    Console.WriteLine("\nVil du spille en ny runde? ");
                    Console.WriteLine("Tryk 'j' for ja eller 'n' for nej: ");
                    answer = Console.ReadLine().ToLower();
                    if (answer == "j" || answer == "n")
                        break;
                    Console.WriteLine("Ugyldigt valg, prøv igen.");
                }

                if (answer == "n")
                {
                    Console.WriteLine("\nTak for spillet!");
                    continuePlaying = false;
                }
                else
                {
                    // Rydder hænder men bevarer spillere og dealer
                    foreach (var player in Players)
                    {
                        player.ClearHand();
                    }
                    Dealer.ClearHand();
                    Deck = new Deck();
                    Console.Clear();
                }
            }
        }

        // Deler starthænder, hvor der vil være 2 kort til hver spiller og dealeren
        public void DealInitialCards()
        {
            foreach (var player in Players)
            {
                Card firstCard = AceChoice(player, isStartHand: true);
                player.AddCard(firstCard);

                Card secondCard = AceChoice(player, isStartHand: true);
                player.AddCard(secondCard);
            }

            Dealer.AddCard(AceChoice(Dealer, isDealer: true));
            Dealer.AddCard(AceChoice(Dealer, isDealer: true));
        }

        // Håndtering af es (A) som er enten 1 eller 11 afhængig af spiller/dealer
        private Card AceChoice(Player player, bool isDealer = false, bool isStartHand = false)
        {
            Card card = Deck.DrawCard();

            if (card.Rank == "A")
            {
                int value;

                if (isDealer)
                {
                    // Dealer vælger automatisk så hånden ikke går (Bust)
                    value = (player.GetHandValue() + 11 > 21) ? 1 : 11;
                }
                else
                {
                    // Ens es (A) starter som 11
                    value = 11;
                }

                card = new Card(card.Suit, card.Rank, value);
            }

            return card;
        }

        // Spilleren vælger selv værdien af et es (stadig 11 eller 1)
        private int ChooseAceValue(Player player)
        {
            while (true)
            {
                Console.WriteLine($"\n{player.Name}, du har trukket et Es som nu giver 11!");
                Console.WriteLine($"Vil du stadig beholde værdien på 11 eller 1?");
                string input = Console.ReadLine();
                if (input == "1") return 1;
                if (input == "11") return 11;
                Console.WriteLine("Ugyldigt valg, prøv igen.");
            }
        }

        // Styrer en spillers tur
        public void PlayerTurn(Player player)
        {
            Console.WriteLine($"\nDet er {player.Name}'s tur");

            player.ShowHand();
            Dealer.ShowHand(hideFirstCard: true);

            // Hvis spilleren starter med Blackjack
            if (player.HasBlackjack())
            {
                Console.WriteLine($"\nWOW! {player.Name} har BLACKJACK!");
                player.ShowHand();
                Dealer.ShowHand(hideFirstCard: true);
                return;
            }

            // Spørger spilleren om es (A) i spillerens start hånd
            for (int i = 0; i < player.Hand.Count; i++)
            {
                Card card = player.Hand[i];
                if (card.Rank == "A")
                {
                    int chosenValue = ChooseAceValue(player);
                    player.Hand[i] = new Card(card.Suit, card.Rank, chosenValue);
                }
            }

            // Hit/Stand loop
            while (true)
            {
                Console.WriteLine("\nVil du vælge Hit eller Stand?");
                Console.WriteLine("Tryk 's' for stand eller 'h' for hit: ");
                string input = Console.ReadLine().ToLower();

                if (input == "h")
                {
                    Card newCard = AceChoice(player); // Håndtering af es (A), når man trækker et kort
                    player.AddCard(newCard);

                    if (newCard.Rank == "A")
                    {
                        int chosenValue = ChooseAceValue(player);
                        player.Hand[player.Hand.Count - 1] = new Card(newCard.Suit, newCard.Rank, chosenValue);
                    }

                    player.ShowHand();

                    if (player.IsBust())
                    {
                        Console.WriteLine($"Bust! {player.Name} taber.");
                        break;
                    }
                }
                else if (input == "s")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Ugyldigt valg, prøv igen.");
                }
            }
        }

        // Dealerens tur
        public void DealerTurn()
        {
            Console.WriteLine("\nDet er Dealerens tur");
            Dealer.PlayTurn(Deck);
        }

        // Finder vinderen af runden
        public void DetermineWinner()
        {
            int dealerValue = Dealer.GetHandValue();
            Console.WriteLine("\nResultat:");

            if (Dealer.IsBust())
            {
                Console.WriteLine($"Dealeren (Bust) med {dealerValue}! Alle spillere vinder!\n");
                foreach (var player in Players)
                {
                    if (player.HasBlackjack())
                        Console.WriteLine($"{player.Name} vinder med BLACKJACK!");
                    else
                        Console.WriteLine($"{player.Name} vinder over dealeren!");
                }
                return;
            }

            foreach (var player in Players)
            {
                int playerValue = player.GetHandValue();

                if (player.IsBust())
                    Console.WriteLine($"{player.Name} (Bust) med {playerValue} og taber til dealeren.");
                else if (player.HasBlackjack() && !Dealer.HasBlackjack())
                    Console.WriteLine($"{player.Name} vinder med BLACKJACK! ({playerValue} vs {dealerValue})");
                else if (!player.HasBlackjack() && Dealer.HasBlackjack())
                    Console.WriteLine($"{player.Name} taber – dealeren har BLACKJACK! ({playerValue} vs {dealerValue})");
                else if (playerValue > dealerValue)
                    Console.WriteLine($"{player.Name} vinder over dealeren ({playerValue} vs {dealerValue}).");
                else if (playerValue < dealerValue)
                    Console.WriteLine($"{player.Name} taber til dealeren ({playerValue} vs {dealerValue}).");
                else
                    Console.WriteLine($"{player.Name} (Push) spiller uafgjort med dealeren.");
            }
        }
    }
}