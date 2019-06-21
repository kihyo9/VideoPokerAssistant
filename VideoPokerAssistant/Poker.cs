/* Started: June 18th, 2019
 * First working version: June 21st, 2019
 * Last updated: June 21st, 2019
 * Creator: Ben Koo
 * 
 * Running poker() in the main function will run this program the way that it is intended by the creator.
 * This program is a video game poker assistant.
 * 
 * The game rules are as follows:
 * You will be dealt 5 random cards from a standard 52-card deck.
 * You may discard (no replacement into deck) any number of these cards and replace them with random cards drawn from the deck.
 * Your new set of cards are evaluated as standard poker hands, with the following payout table:
 * 1P = 1, 2P = 2, 3oK = 3, S = 4, F = 6, FH = 9, 4oK = 25, SF = 50, RF = 250
 * For most video game poker machines, there is an additional rule that 1P must be Jacks or better to count.
 * This additional has been implemented into the program.
 * 
 * For a given 5-card hand (randomly or manually selected), the program will output what combinations of
 * card replacements will yield the highest expected payout.
 */

/* In the works:
 * Determining expected payout using probabilities
 * Add Deuces Wild variant
 * Connecting this program to a playing-card-recognizing camera
 * Unicode shapes console output
 * Putting this on GitHub
 * Use ML approach instead of these if statements
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;


namespace VideoPokerAssistant
{
    public class Poker
    {
        //for things to be referenced globally
        static int handSize = 5;
        static int replaceCombinations = (int)Math.Pow(2, handSize);
        static int[] iterations = new int[replaceCombinations];
        static int[] deucesArray = { 1, 14, 27, 40 };
        Dictionary<int, string> rank = new Dictionary<int, string>()
            {
                {0,"A" },{1,"2" },{2,"3" },{3,"4"},{4,"5"},{5,"6"},{6,"7"},
                {7,"8" },{8,"9" },{9,"T" },{10,"J"},{11,"Q"},{12,"K"},{-1,"~Replaced"}
            };
        Dictionary<int, string> suit = new Dictionary<int, string>()
            {
                {0,"~Clubs"}, {1,"~Diamonds"},{2,"~Hearts"},{3,"~Spades"},{-1,""}
            };
        Dictionary<string, int> result = new Dictionary<string, int>()
            {
                {"HC",0},{"1P",1},{"2P",2},{"3oK",3},{"S",4},{"F",6},{"FH",9},{"4oK",25},{"SF",50},{"RF",250}
            };
        //for deuces wild
        Dictionary<string, double> DeucesWild = new Dictionary<string, double>()
            {
                {"HC",0},{"1P",0},{"2P",0},{"3oK",1},{"S",2},{"F",2},{"FH",3},{"4oK",5},{"SF",9},{"5oK",15 },{"WRF",25 },{"4D",200 },{"NRF",800}
            };
        static List<int> freshDeck = new List<int>(new int[] {0,2,3,4,5,6,7,8,9,10,11,12,
                                                       13,15,16,17,18,19,20,21,22,23,24,25,
                                                       26,28,29,30,31,32,33,34,35,36,37,38,
                                                       39,41,42,43,45,46,47,48,49,50,51});
        static List<int> freshDeck2 = new List<int>(freshDeck);
        bool isDeucesWild = true;


        //for performance
        int[] rankFreq = new int[13];
        int[] rankFreqCopy = new int[13];
        int[] rankFreq2 = new int[5];
        int[] highCard = { 1, 1, 1, 1, 1 };
        int[] onePair = { 0, 1, 1, 1, 2 };
        int[] twoPair = { 0, 0, 1, 2, 2 };
        int[] threeoK = { 0, 0, 1, 1, 3 };
        int[] fh = { 0, 0, 0, 2, 3 };
        int[] fouroK = { 0, 0, 0, 1, 4 };

        //Test Cases
        //hand = new List<int>(new int[] { 0, 1, 2, 3, 4}); //straight flush
        //hand = new List<int>(new int[] { 9, 10, 11, 12, 0}); //royal flush
        //hand = new List<int>(new int[] { 2, 15, 28, 41, 50}); //4oK
        //hand = new List<int>(new int[] { 3, 16, 29, 12, 25}); //FH
        //hand = new List<int>(new int[] { 40, 41, 42, 43, 44}); //flush
        //hand = new List<int>(new int[] { 4, 31, 19, 46, 34}); //straight
        //hand = new List<int>(new int[] { 7, 20, 33, 34, 50}); //3oK
        //hand = new List<int>(new int[] { 24, 37, 22, 48, 1}); //2P
        //hand = new List<int>(new int[] { 12, 25, 11, 7, 30}); //1P
        //hand = new List<int>(new int[] { 1, 3, 5, 17, 26}); //HC

        //RANKS----A  2  3  4  5  6  7  8  9  T  J  Q  K
        //clubs:   0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,11,12
        //diamonds:13,14,15,16,17,18,19,20,21,22,23,24,25
        //hearts:  26,27,28,29,30,31,32,33,34,35,36,37,38
        //spades:  39,40,41,42,43,44,45,46,47,48,49,50,51

        public void start()
        {
            //Start
            try
            {
                BestHand();
                exit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                exit();
            }
        }

        public void BestHand()
        {
            //set-up needed collections and stuff
            List<int> deck = new List<int>();
            List<int> hand = new List<int>();
            double[] expectedReturnValues = new double[replaceCombinations];
            Random rng = new Random();
            Stopwatch sw = new Stopwatch();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //CHANGE THIS TO CHANGE INPUT OF 5-CARD HAND
            bool customHand = true;
            bool consoleInput = true;

            //make deck
            for (int i = 0; i < 52; i++)
            {
                deck.Add(i);
            }

            //make random hand
            if (!customHand)
            {
                //make random hand and remove those cards from deck
                for (int j = 0; j < handSize; j++)
                {
                    int randCardIndex = rng.Next(52 - j);
                    int randCardValue = deck.ElementAt(randCardIndex);
                    hand.Add(randCardValue);
                    deck.Remove(randCardValue);
                }
            }
            //make custom hand
            else if (consoleInput)
            {
                Console.WriteLine("Choosing custom hand!");
                Console.WriteLine("\n   RANKS:[A][2][3][4][5][6][7][8][9][T][J][Q][K]\n" +
                                    "   clubs| 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,11,12\n" +
                                    "diamonds| 13,14,15,16,17,18,19,20,21,22,23,24,25\n" +
                                    "  hearts| 26,27,28,29,30,31,32,33,34,35,36,37,38\n" +
                                    "  spades| 39,40,41,42,43,44,45,46,47,48,49,50,51\n");
                do
                {
                    string input;
                    int inputInt = -1;
                    bool goodInput = true;

                    do
                    {
                        Console.Write("Pick a card #{0} (0-51): ", hand.Count + 1);
                        goodInput = true;
                        input = Console.ReadLine();
                        try
                        {
                            inputInt = Convert.ToInt32(input);
                            if (inputInt < 0 || inputInt > 51)
                            {
                                Console.WriteLine("Must pick from 0 to 51!");
                                goodInput = false;
                            }
                            foreach (int card in hand)
                            {
                                if (inputInt == card)
                                {
                                    Console.WriteLine("You've already chose this card...");
                                    goodInput = false;
                                    break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Input is not a number!");
                            goodInput = false;
                        }
                    } while (!goodInput);

                    hand.Add(inputInt);
                    deck.Remove(inputInt);
                } while (hand.Count < 5);
            }

            //custom hand in code
            //hand = new List<int>(new int[] {1,6,30,48,51});
            //foreach(int card in hand)
            //{
            //    deck.Remove(card);
            //}

            //Write hand to console
            Console.Write("Initial Hand:");
            for (int j = 0; j < handSize; j++)
            {
                int suitNum = hand[j] / 13;
                int rankNum = hand[j] % 13;
                Console.Write(" [{0}{1}]", rank[rankNum], suit[suitNum]);
            }
            Console.WriteLine("\n\n");





            /***************************************************************************************************/
            //This is where the calculations happen
            //Each iteration is a unique way to replace cards
            sw.Start(); //go!!!!!!
            bool replace = false;
            for (byte replacedCards = 0; replacedCards < replaceCombinations; replacedCards++)
            {
                /* Use this for debugging purposes
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\kihyo\Desktop\pokerDebug2.txt", true))
                {
                    file.WriteLine("___________________Iteration: " + replacedCards + "___________________");
                }
                */

                Console.WriteLine("___________________Iteration: " + replacedCards + "___________________");
                List<int> handCopy = new List<int>(hand);

                //this loop removes cards
                for (int cardNum = handSize - 1; cardNum >= 0; cardNum--)
                {
                    replace = (replacedCards & (1 << cardNum)) != 0;
                    if (replace)
                    {
                        handCopy.Remove(hand.ElementAt(cardNum));
                    }
                }

                //Brute force: copy deck list to an array for easier iterating
                int[] deckCopyArray = new int[52 - handSize];
                deck.CopyTo(deckCopyArray);
                expectedReturnValues[replacedCards] = BruteForce(handCopy, deckCopyArray, replacedCards, isDeucesWild);

                //Probability method: coming soon!
            }
            Console.WriteLine();
            sw.Stop(); //stop!!!!
            /***************************************************************************************************/





            //Console output
            double[] expectedReturnValues2 = new double[replaceCombinations];
            Array.Copy(expectedReturnValues, expectedReturnValues2, replaceCombinations);
            Console.WriteLine("___________________TOP RESULTS___________________");
            for (int i = 0; i < replaceCombinations; i++)
            {
                //get the highest return value to display each loop
                double maxValue = expectedReturnValues2.Max();
                int maxIndex = expectedReturnValues2.ToList().IndexOf(maxValue);
                expectedReturnValues2[maxIndex] = -1;
                List<int> handCopy2 = new List<int>(hand);

                //determine which hand cards are replaced
                for (int cardNum = handSize - 1; cardNum >= 0; cardNum--)
                {
                    replace = (maxIndex & (1 << cardNum)) != 0;
                    if (replace)
                    {
                        handCopy2[cardNum] = -14;
                    }
                }

                //write to the console
                Console.Write("Cards:");
                foreach (int card in handCopy2)
                {
                    int suitNum = card / 13;
                    int rankNum = card % 13;
                    Console.Write(" [{0}{1}]", rank[rankNum], suit[suitNum]);
                }
                Console.WriteLine();
                Console.WriteLine(iterations[maxIndex] + " combinations calculated.\n" + maxValue.ToString("G6") + " is the expected return." + "\n");
            }

            //performance
            Console.WriteLine("\n\nElapsed: {0}", sw.Elapsed);
            Console.WriteLine("Total number of evaluations: " + iterations.Sum());
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\kihyo\Desktop\pokerPerf.txt", true))
            {
                DateTime time = DateTime.Now;
                file.WriteLine(time + " - Elapsed: {0}", sw.Elapsed);
            }
        }

        public void exit()
        {
            Console.Write("\n\nPress Enter to close the console app...");
            Console.ReadLine();
            //end
        }

        public double BruteForce(List<int> handCopy, int[] deckCopy, int forLoopNumber, bool isDeucesWild)
        {
            int initialHandCount = handCopy.Count;
            int deckCount = 52 - initialHandCount;
            int combinations = NchooseK(deckCount, handCopy.Count);
            int counter = 0;
            double sum = 0;
            int currentCard = 0;
            double hold = 0;


            /* To debug, delete the target write-file on the desktop
             * Next, wrap this 'using' around the -.-'s
             * Then, put the foreach loop below every sum += hold statement
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\kihyo\Desktop\pokerDebug2.txt", true))
            {
            }            
            foreach (int card in handCopy)
            {
                int suitNum = card / 13;
                int rankNum = card % 13;
                file.Write(" [{0}{1}]", rank[rankNum], suit[suitNum]);
            }
            file.Write(" - " + hold + "\n");
            */


            /*-.-*/
            //first card
            currentCard++;
            foreach (int card1 in deckCopy)
            {
                if (initialHandCount > 4)
                {
                    break;
                }
                handCopy.Add(card1);
                //second card
                currentCard++;
                foreach (int card2 in deckCopy)
                {
                    if (initialHandCount > 3)
                    {
                        break;
                    }
                    if (card1 >= card2)
                    {
                        continue;
                    }
                    handCopy.Add(card2);
                    //third card
                    currentCard++;
                    foreach (int card3 in deckCopy)
                    {
                        if (initialHandCount > 2)
                        {
                            break;
                        }
                        if (card1 >= card3 || card2 >= card3)
                        {
                            continue;
                        }
                        handCopy.Add(card3);
                        //fourth card
                        currentCard++;
                        foreach (int card4 in deckCopy)
                        {
                            if (initialHandCount > 1)
                            {
                                break;
                            }
                            if (card1 >= card4 || card2 >= card4 || card3 >= card4)
                            {
                                continue;
                            }
                            handCopy.Add(card4);
                            //fifth card
                            currentCard++;
                            foreach (int card5 in deckCopy)
                            {
                                if (initialHandCount > 0)
                                {
                                    break;
                                }
                                if (card1 >= card5 || card2 >= card5 || card3 >= card5 || card4 >= card5)
                                {
                                    continue;
                                }
                                handCopy.Add(card5);
                                if (currentCard == 5 - initialHandCount)
                                {
                                    counter++;
                                    if (isDeucesWild)
                                    {
                                        hold = evaluatorDeucesWild(handCopy);
                                    }
                                    else
                                    {
                                        hold = evaluator(handCopy);
                                    }                                    
                                    sum += hold;
                                }
                                handCopy.RemoveAt(handCopy.Count - 1);
                            }
                            currentCard--;
                            //end fifth
                            if (currentCard == 5 - initialHandCount)
                            {
                                counter++;
                                if (isDeucesWild)
                                {
                                    hold = evaluatorDeucesWild(handCopy);
                                }
                                else
                                {
                                    hold = evaluator(handCopy);
                                }
                                sum += hold;
                            }
                            handCopy.RemoveAt(handCopy.Count - 1);
                        }
                        currentCard--;
                        //end fourth
                        if (currentCard == 5 - initialHandCount)
                        {
                            counter++;
                            if (isDeucesWild)
                            {
                                hold = evaluatorDeucesWild(handCopy);
                            }
                            else
                            {
                                hold = evaluator(handCopy);
                            }
                            sum += hold;
                        }
                        handCopy.RemoveAt(handCopy.Count - 1);
                    }
                    currentCard--;
                    //end third
                    if (currentCard == 5 - initialHandCount)
                    {
                        counter++;
                        if (isDeucesWild)
                        {
                            hold = evaluatorDeucesWild(handCopy);
                        }
                        else
                        {
                            hold = evaluator(handCopy);
                        }
                        sum += hold;
                    }
                    handCopy.RemoveAt(handCopy.Count - 1);
                }
                currentCard--;
                //end second
                if (currentCard == 5 - initialHandCount)
                {
                    counter++;
                    if (isDeucesWild)
                    {
                        hold = evaluatorDeucesWild(handCopy);
                    }
                    else
                    {
                        hold = evaluator(handCopy);
                    }
                    sum += hold;

                }
                handCopy.RemoveAt(handCopy.Count - 1);
            }
            currentCard--;
            //end first
            if (currentCard == 5 - initialHandCount)
            {
                counter++;
                if (isDeucesWild)
                {
                    hold = evaluatorDeucesWild(handCopy);
                }
                else
                {
                    hold = evaluator(handCopy);
                }
                sum += hold;
            }
            /*-.-*/

            iterations[forLoopNumber] = counter;
            return (double)sum / (double)counter;
        }

        public int evaluator(List<int> handCopy)
        {
            //1P = 1, 2P = 2, 3oK = 3, S = 4, F = 6, FH = 9, 4oK = 25, SF = 50, RF = 250
            //unique grouping values: 11111 -> HC/S/F/SF/RF, 1112 -> 1P, 113 -> 3oK, 122 -> 2P, 23 -> FH, 14 -> 4oK

            //Test Cases            
            //handCopy.Clear();
            //handCopy = new List<int>(testCase);


            //initialize
            bool straight = true;
            bool flush = true;
            List<int> handCopyUnsuited = new List<int>(new int[] { });
            foreach (int card in handCopy)
            {
                handCopyUnsuited.Add(card % 13);
            }
            handCopyUnsuited.Sort();

            //clear reusable arrays and copy relevant elements
            Array.Clear(rankFreq, 0, 13);
            Array.Clear(rankFreqCopy, 0, 13);
            Array.Clear(rankFreq2, 0, 5);
            foreach (int card in handCopyUnsuited)
            {
                rankFreq[card]++;
            }
            Array.Copy(rankFreq, 0, rankFreqCopy, 0, 13);
            Array.Sort(rankFreq);//sorted freq array
            Array.Copy(rankFreq, 8, rankFreq2, 0, 5);

            //evaluates the hand
            if (rankFreq2.SequenceEqual(highCard))
            {
                //is straight?
                for (int i = 4; i > 0; i--)
                {
                    if (handCopyUnsuited[i] - handCopyUnsuited[i - 1] != 1)
                    {
                        if (!(handCopyUnsuited[i] == 9 && handCopyUnsuited[i - 1] == 0))
                        {
                            //the cards must be in ascending in order by one.
                            //the edge case is when the high card is an ace, and its rank
                            //value is zero when it "should" be 13
                            straight = false;
                            break;
                        }
                    }
                }

                //is flush?
                int firstCardSuit = handCopy[0] / 13;
                foreach (int card in handCopy)
                {
                    if (card / 13 != firstCardSuit)
                    {
                        flush = false;
                        break;
                    }
                }

                //return straights/flushes
                if (flush && straight)
                {
                    if (handCopyUnsuited[0] == 0 && handCopyUnsuited[4] == 12)
                    {
                        return result["RF"];
                    }
                    else
                    {
                        return result["SF"];
                    }
                }
                else if (flush && !straight)
                {
                    return result["F"];
                }
                else if (!flush && straight)
                {
                    return result["S"];
                }

                //actually high card
                return result["HC"];
            }
            else if (rankFreq2.SequenceEqual(onePair))
            {
                //Jacks or better rule
                if (rankFreqCopy.ToList().IndexOf(2) >= 10 || rankFreqCopy.ToList().IndexOf(2) == 0)
                {
                    return result["1P"];
                }
                else
                {
                    //basically HC
                    return result["HC"];
                }
            }
            else if (rankFreq2.SequenceEqual(twoPair))
            {
                return result["2P"];
            }
            else if (rankFreq2.SequenceEqual(threeoK))
            {
                return result["3oK"];
            }
            else if (rankFreq2.SequenceEqual(fh))
            {
                return result["FH"];
            }
            else if (rankFreq2.SequenceEqual(fouroK))
            {
                return result["4oK"];
            }



            //if this is reached, something went wrong
            return 0;
        }

        public double BruteForce2(List<int> handCopy, List<int> deckCopy)
        {
            int initialHandCount = handCopy.Count;
            int deckCount = 52 - initialHandCount;
            int combinations = NchooseK(deckCount, handCopy.Count);
            int counter = 0;
            double sum = 0;
            int currentCard = 0;
            double hold = 0;


            /* To debug, delete the target write-file on the desktop
             * Next, wrap this 'using' around the -.-'s
             * Then, put the foreach loop below every sum += hold statement
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\kihyo\Desktop\pokerDebug2.txt", true))
            {
            }            
            foreach (int card in handCopy)
            {
                int suitNum = card / 13;
                int rankNum = card % 13;
                file.Write(" [{0}{1}]", rank[rankNum], suit[suitNum]);
            }
            file.Write(" - " + hold + "\n");
            */


            /*-.-*/
            //first card
            currentCard++;
            foreach (int card1 in deckCopy)
            {
                if (initialHandCount > 4)
                {
                    break;
                }
                handCopy.Add(card1);
                //second card
                currentCard++;
                foreach (int card2 in deckCopy)
                {
                    if (initialHandCount > 3)
                    {
                        break;
                    }
                    if (card1 >= card2)
                    {
                        continue;
                    }
                    handCopy.Add(card2);
                    //third card
                    currentCard++;
                    foreach (int card3 in deckCopy)
                    {
                        if (initialHandCount > 2)
                        {
                            break;
                        }
                        if (card1 >= card3 || card2 >= card3)
                        {
                            continue;
                        }
                        handCopy.Add(card3);
                        //fourth card
                        currentCard++;
                        foreach (int card4 in deckCopy)
                        {
                            if (initialHandCount > 1)
                            {
                                break;
                            }
                            if (card1 >= card4 || card2 >= card4 || card3 >= card4)
                            {
                                continue;
                            }
                            handCopy.Add(card4);
                            //fifth card
                            currentCard++;
                            foreach (int card5 in deckCopy)
                            {
                                if (initialHandCount > 0)
                                {
                                    break;
                                }
                                if (card1 >= card5 || card2 >= card5 || card3 >= card5 || card4 >= card5)
                                {
                                    continue;
                                }
                                handCopy.Add(card5);
                                if (currentCard == 5 - initialHandCount)
                                {
                                    counter++;
                                    hold = evaluatorDeucesWild(handCopy);
                                    sum += hold;
                                }
                                handCopy.RemoveAt(handCopy.Count - 1);
                            }
                            currentCard--;
                            //end fifth
                            if (currentCard == 5 - initialHandCount)
                            {
                                counter++;
                                hold = evaluatorDeucesWild(handCopy);
                                sum += hold;
                            }
                            handCopy.RemoveAt(handCopy.Count - 1);
                        }
                        currentCard--;
                        //end fourth
                        if (currentCard == 5 - initialHandCount)
                        {
                            counter++;
                            hold = evaluatorDeucesWild(handCopy);
                            sum += hold;
                        }
                        handCopy.RemoveAt(handCopy.Count - 1);
                    }
                    currentCard--;
                    //end third
                    if (currentCard == 5 - initialHandCount)
                    {
                        counter++;
                        hold = evaluatorDeucesWild(handCopy);
                        sum += hold;
                    }
                    handCopy.RemoveAt(handCopy.Count - 1);
                }
                currentCard--;
                //end second
                if (currentCard == 5 - initialHandCount)
                {
                    counter++;
                    hold = evaluatorDeucesWild(handCopy);
                    sum += hold;

                }
                handCopy.RemoveAt(handCopy.Count - 1);
            }
            currentCard--;
            //end first
            if (currentCard == 5 - initialHandCount)
            {
                counter++;
                hold = evaluatorDeucesWild(handCopy);
                sum += hold;
            }
            /*-.-*/
            
            return (double)sum / (double)counter;
        }

        public double evaluatorDeucesWild(List<int> handCopy)
        {
            //1P = 1, 2P = 2, 3oK = 3, S = 4, F = 6, FH = 9, 4oK = 25, SF = 50, RF = 250
            //unique grouping values: 11111 -> HC/S/F/SF/RF, 1112 -> 1P, 113 -> 3oK, 122 -> 2P, 23 -> FH, 14 -> 4oK

            //Test Cases            
            //handCopy.Clear();
            //handCopy = new List<int>(testCase);

            //Deal with deuces (1,14,27,40)
            int countDeuces = 0;
            foreach(int card in handCopy)
            {
                foreach(int deuce in deucesArray)
                {
                    if (card == deuce)
                    {
                        countDeuces++;
                        handCopy.Remove(card);
                    }
                }
            }

            if(countDeuces == 4)
            {                
                return DeucesWild["4D"];
            }
            else if(countDeuces >= 1)
            {
                freshDeck2 = new List<int>(freshDeck);
                foreach (int card in handCopy)
                {
                    freshDeck2.Remove(card);
                }
                return BruteForce2(handCopy, freshDeck2);
            }
            
            //initialize
            bool straight = true;
            bool flush = true;
            List<int> handCopyUnsuited = new List<int>(new int[] { });
            foreach (int card in handCopy)
            {
                handCopyUnsuited.Add(card % 13);
            }
            handCopyUnsuited.Sort();
            


            //clear reusable arrays and copy relevant elements
            Array.Clear(rankFreq, 0, 13);
            Array.Clear(rankFreqCopy, 0, 13);
            Array.Clear(rankFreq2, 0, 5);
            foreach (int card in handCopyUnsuited)
            {
                rankFreq[card]++;
            }
            Array.Copy(rankFreq, 0, rankFreqCopy, 0, 13);
            Array.Sort(rankFreq);//sorted freq array
            Array.Copy(rankFreq, 8, rankFreq2, 0, 5);
            //evaluates the hand
            if (rankFreq2.SequenceEqual(highCard))
            {
                //is straight?
                for (int i = 4; i > 0; i--)
                {
                    if (handCopyUnsuited[i] - handCopyUnsuited[i - 1] != 1)
                    {
                        if (!(handCopyUnsuited[i] == 9 && handCopyUnsuited[i - 1] == 0))
                        {
                            //the cards must be in ascending in order by one.
                            //the edge case is when the high card is an ace, and its rank
                            //value is zero when it "should" be 13
                            straight = false;
                            break;
                        }
                    }
                }

                //is flush?
                int firstCardSuit = handCopy[0] / 13;
                foreach (int card in handCopy)
                {
                    if (card / 13 != firstCardSuit)
                    {
                        flush = false;
                        break;
                    }
                }

                //return straights/flushes
                if (flush && straight)
                {
                    if (handCopyUnsuited[0] == 0 && handCopyUnsuited[4] == 12)
                    {
                        return result["RF"];
                    }
                    else
                    {
                        return result["SF"];
                    }
                }
                else if (flush && !straight)
                {
                    return result["F"];
                }
                else if (!flush && straight)
                {
                    return result["S"];
                }

                //actually high card
                return result["HC"];
            }
            else if (rankFreq2.SequenceEqual(onePair))
            {
                //Jacks or better rule
                if (rankFreqCopy.ToList().IndexOf(2) >= 10 || rankFreqCopy.ToList().IndexOf(2) == 0)
                {
                    return result["1P"];
                }
                else
                {
                    //basically HC
                    return result["HC"];
                }
            }
            else if (rankFreq2.SequenceEqual(twoPair))
            {
                return result["2P"];
            }
            else if (rankFreq2.SequenceEqual(threeoK))
            {
                return result["3oK"];
            }
            else if (rankFreq2.SequenceEqual(fh))
            {
                return result["FH"];
            }
            else if (rankFreq2.SequenceEqual(fouroK))
            {
                return result["4oK"];
            }



            //if this is reached, something went wrong
            return 0;
        }

        public int NchooseK(int N, int K)
        {
            int num = 1;
            int den = 1;
            for (int i = 0; i < K; i++)
            {
                num = num * (N - i);
                den = den * (i + 1);
            }
            return num / den;
        }
    }
}
