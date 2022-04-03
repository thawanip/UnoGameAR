using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class GamePlay : MonoBehaviour
{
    private static bool _usersTurnFlag = true;
    public string WildCardColour { get; set; }
    public static bool UsersTurnFlag 
    {
        get => _usersTurnFlag;
        set
        {
            _usersTurnFlag = value;
            if(!_usersTurnFlag)
            {
                //t();
               
            }
        }
    }
    public static void DestroyCC()
    {
        try
        {
            GameObject cc = GameObject.FindWithTag("cc").gameObject;
            CurrentCards.ccPos = cc.transform.position;
            CurrentCards.currentCard = cc;
            DestroyImmediate(cc);
        }
        catch(Exception e)
        {
            Debug.Log("Exception in DestroyCC " + e.Message);
        }

    }
    private async void t()
    {
        //await System.Threading.Tasks.Task.Run(() => WaitForSecondsRealtime(4));
        await System.Threading.Tasks.Task.Run(() => Player2Turn(getDeck()));
    }
    private static Deck getDeck()
    {
        return SpawnScript.uno;
    }
    public static void Player2Turn(Deck d)
    {
        string card = Player2CardPick(CurrentCards.GetPlayer2Cards());

        if (card != "")
            Debug.Log("Card selected by player2: " + card);
        if(card == string.Empty)
        {
            AddXCardsToPlayer2(1, d);
            _ShowAndroidToastMessage("1 card picked by  player 2");
            string lastAddedCard = CurrentCards.GetPlayer2Cards()[CurrentCards.GetPlayer2Cards().Count - 1];

            string newcard = Player2CardPick(new List<string>() { lastAddedCard });
            if (newcard == string.Empty)
                UsersTurnFlag = true;
            else
                ThrowSelectedCardByPlayer2(newcard);
        }
        ThrowSelectedCardByPlayer2(card);
    }
    public static void ThrowSelectedCardByPlayer2(string card)
    {
        Debug.Log("thorw selected entered: ");
        Debug.Log("cc name: " + CurrentCards.currentCard.gameObject.name);
        Vector3 currCard = CurrentCards.currentCard.gameObject.transform.position;
        //Destroy(GameObject.FindWithTag("cc"));

        // Thread.Sleep(2000);
       // Invoke(nameof(SpawnScript.MakeGameWait), 2f);
        

        GameObject selectedCard = Resources.Load<GameObject>("Cards/" + card);
        if(selectedCard != null)
        {

            //TO-DO
            //add Animation for new
            //DestroyImmediate(GameObject.FindWithTag("cc").gameObject);
            DestroyCC();
            //Thread.Sleep(2000);
            //Invoke(nameof(SpawnScript.MakeGameWait), 0f);
            selectedCard.tag = "cc";
            Instantiate(selectedCard, CurrentCards.ccPos, Quaternion.Euler(new Vector3(90f, 180f, 0f)));
            
            
            UsersTurnFlag = true;
            _ShowAndroidToastMessage("Card picked: " + selectedCard.name);
        }        
    }
    private static string Player2CardPick(List<string> p2Cards)
    {
        List<string> possiblePlayableCards = new List<string>();
        System.Random r = new System.Random();
        foreach (string card in p2Cards)
        {
            if (card.Contains("Wild"))
                possiblePlayableCards.Add(card);
            if (card.Contains("_"))
            {
                if (card.Split('_')[0] == CurrentCards.currentCard.name.Split('_')[0]
                    || card.Split('_')[1] == CurrentCards.currentCard.name.Split('_')[1])
                    possiblePlayableCards.Add(card);
            }
        }
        if (possiblePlayableCards.Count == 0)
            return string.Empty;
        return possiblePlayableCards[r.Next(possiblePlayableCards.Count)];
    }

    internal static bool CanUseSelectedCard(string selected, string cc)
    {
        if (selected.Contains("Wild"))
                return true;
        if (cc.Contains("_"))
        {
            if ((cc.Split('_')[0] == selected.Split('_')[0]) || (cc.Split('_')[1].Split('(')[0] == selected.Split('_')[1].Split('(')[0]))
                return true;
            else
                return false;
        }
        return false;

    }

    internal static void WildThrownByUser(string name, Deck d)
    {
        if(name.Contains("Draw4"))
        {
            AddXCardsToPlayer2(4, d);
            _ShowAndroidToastMessage("4 cards to  player 2");
            UsersTurnFlag = true;
        }
        if(name.Contains("Draw2"))
        {
            AddXCardsToPlayer2(2, d);
            _ShowAndroidToastMessage("2 cards to  player 2");
            Player2Turn(d);
        }
    }
    private static void AddXCardsToPlayer2(int x, Deck d)
    {
        string[] drawCards = new string[x];
        for (int i = 0; i < x; i++)
            drawCards[i] = d.allCards.Pop();

        CurrentCards.AddToPlayer2Cards(drawCards);
    }
    public static void _ShowAndroidToastMessage(string message)
    {
        try
        {
            Debug.Log(message);
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
               
            }
        }
        catch(Exception)
        {

        }
        
    }
}
