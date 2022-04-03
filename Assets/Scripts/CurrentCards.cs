using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentCards : MonoBehaviour
{
    // Start is called before the first frame update
    public static List<GameObject> usercards = new List<GameObject>();
    public static  List<string> player2Cards = new List<string>();
    public static Vector3 ccPos;
 
    //private static string currentCard = "";
    void Start()
    {
        //usercards = new List<GameObject>();
        //player2Cards 
    }
    void Update()
    {
       
    }
    public static GameObject currentCard {get; set;}
    public static List<GameObject> GetUserCards()
    {
        return usercards;
    }
    public static List<string> GetPlayer2Cards()
    {
        return player2Cards;
    }
    public static void AddToUserCards(GameObject card)
    {
        usercards.Add(card);
    }
    public static void RemoveFromUserCards(GameObject card)
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
