using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentCards : MonoBehaviour
{
    // Start is called before the first frame update
    private static List<string> usercards = new List<string>();
    private static List<string> player2Cards = new List<string>();
    //private static string currentCard = "";
    public static string currentCard {get; set;}
    public static List<string> GetUserCards()
    {
        return usercards;
    }
    public static List<string> GetPlayer2Cards()
    {
        return player2Cards;
    }
    public static void AddToUserCards(string[] cards)
    {
        foreach(string s in cards)
             usercards.Add(s);
    }
    public static void RemoveFromUserCards(string card)
    {
        usercards.Remove(card);
    }
    public static void AddToPlayer2Cards(string[] cards)
    {
        foreach(string s in cards)
             player2Cards.Add(s);
    }
    public static void RemoveFromPlayer2Cards(string card)
    {
        player2Cards.Remove(card);
    }

}
