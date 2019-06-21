# VideoPokerAssistant
Helps you pick cards to replace in your typical video poker games

Started: June 18th, 2019

First working version: June 21st, 2019

Last updated: June 21st, 2019

Creator: Ben Koo

Running poker() in the main function will run this program the way that it is intended by the creator.
This program is a video game poker assistant.

The game rules are as follows:
You will be dealt 5 random cards from a standard 52-card deck.
You may discard (no replacement into deck) any number of these cards and replace them with random cards drawn from the deck.
Your new set of cards are evaluated as standard poker hands, with the following payout table:
* 1P = 1, 2P = 2, 3oK = 3, S = 4, F = 6, FH = 9, 4oK = 25, SF = 50, RF = 250

For most video game poker machines, there is an additional rule that 1P must be Jacks or better to count.
This additional has been implemented into the program.

For a given 5-card hand (randomly or manually selected), the program will output what combinations of
card replacements will yield the highest expected payout.

In the works:
 * Determining expected payout using probabilities
 * Add Deuces Wild variant
 * Connecting this program to a playing-card-recognizing camera
 * Unicode shapes console output
 * Putting this on GitHub
 * Use ML approach instead of these if statements

