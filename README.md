# Blackjack

Det er et konsolbaseret Blackjack-spil skrevet i C# med OOP-principper.
Koden er struktureret i flere klasser med hver deres ansvar:

Card: Repræsenterer et enkelt kort med kulør, rang og værdi.

Deck: Håndterer en kortbunke – fylder, blander og udleverer kort.

Player: Repræsenterer en spiller med en hånd, kan trække kort, tjekke værdi, bust og blackjack.

Dealer: Arver fra Player, men har sin egen spillogik (trækker indtil mindst 17).

Game: Styrer hele spillet – opretter spillere, uddeler kort, håndterer spiller- og dealer-ture, afgør vinderen og giver mulighed for flere runder.

Spillet understøtter 1–4 spillere, interaktiv input via konsollen, og håndterer regler som blackjack, bust, es-værdi (1 eller 11), dealerens logik og uafgjort (push)
