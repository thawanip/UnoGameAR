using System;
using System.Collections.Generic;
using System.Linq;

public class Deck {
    string[] colors = {"R", "G", "B", "Y"};
    string[] values = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Skip", "Reverse", "Draw2"};
    public Stack<string> allCards = new Stack<string>();
    string card;
    int zeroInDeck = 0;

    public void CreateDeck() {
        for(int times = 0; times < 2; times++) {
            for(int col = 0; col < 4; col++) {
                for(int row = 0; row < 13; row++) {
                    if(row == 0 && zeroInDeck >= 4){
                        continue;
                    }
                    if(row == 0) {
                        zeroInDeck++;
                    }
                    card = colors[col] + "_" + values[row];
                    allCards.Push(card);
                }
                if(times < 1) {
                    card = "Wild";
                    allCards.Push(card);
                    card = "Draw4";
                    allCards.Push(card);
                }
            }
        }
    }
    
    public void PrintDeck() {
        foreach(string card in allCards) {
            //Console.WriteLine(card);
        }
    }
  
    public Stack<string> Shuffle(Stack<string> stack) {
        System.Random rnd=new System.Random();
        return new Stack<string>(stack.OrderBy(x => rnd.Next()));
    }

    public string[] GetNumberOfCards(int num, Stack<string> stack)
    {
        string[] cards = new string[num];
        for(int j = 0; j < num; j++ )
        {
            cards[j] = stack.Pop();
        }
        return cards;
    }
    public void PutCardBackInDeck(string card)
    {
        allCards.Push(card);
        allCards = Shuffle(allCards);
    }
    // static void Main(string[] args) {
    //     Deck deck = new Deck();
    //     deck.CreateDeck();
    //     //deck.PrintDeck();
    //     //deck.allCards = Shuffle(deck.allCards);
    //     //deck.PrintDeck();
    // }
}