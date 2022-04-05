using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GamePlay : MonoBehaviour
{
    private static bool _usersTurnFlag = true;
    public GameObject PopPanel;
    public string WildCardColour { get; set; }
    public static bool UsersTurnFlag 
    {
        get => _usersTurnFlag;
        set
        {
            _usersTurnFlag = value;
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
    
    private static Deck getDeck()
    {
        return SpawnScript.uno;
    }
    public static void Player2Turn(Deck d)
    {
        if (UsersTurnFlag)
            return;
        string card = Player2CardPick(CurrentCards.GetPlayer2Cards());

        if (card != "")
            Debug.Log("Card selected by player2: " + card);
        if(card == string.Empty)
        {
            AddXCardsToPlayer2(1, d);
            _ShowAndroidToastMessage("Card picked by  player 2");
            string lastAddedCard = CurrentCards.GetPlayer2Cards()[CurrentCards.GetPlayer2Cards().Count - 1];
            
            //CurrentCards.player2Cards.Add(lastAddedCard);
           
            string newcard = Player2CardPick(new List<string>() { lastAddedCard });
            if (newcard == string.Empty)
            {
                _ShowAndroidToastMessage("Your turn!");
                UsersTurnFlag = true;
            }               
            else
                ThrowSelectedCardByPlayer2(newcard);
        }
        else
            ThrowSelectedCardByPlayer2(card);
    }
    public static bool SpecialCardCheck(string name)
    {
        if (name.Contains("Wild") || name.Contains("Skip") || name.Contains("Reverse") || name.Contains("Draw2"))
            return true;
        else
            return false;
    }
    public static void ThrowSelectedCardByPlayer2(string card)
    {
        //Debug.Log("thorw selected entered: ");
        //Debug.Log("cc name: " + CurrentCards.currentCard.gameObject.name);
        //Vector3 currCard = CurrentCards.currentCard.gameObject.transform.position;        
        if (card == "")
            return;
        GameObject selectedCard = Resources.Load<GameObject>("Cards/" + card);
        if(selectedCard != null)
        {

            //TO-DO
            //add Animation for new
            DestroyCC();
            
            selectedCard.tag = "cc";
            Vector3 p = new Vector3(CurrentCards.ccPos.x, CurrentCards.ccPos.y, CurrentCards.ccPos.z);
            p.y = 0.1f;
            Instantiate(selectedCard, CurrentCards.ccPos, Quaternion.Euler(new Vector3(90f, 180f, 0f)));
            selectedCard.transform.DOPunchPosition(Vector3.down, 1f, 6, 0.6f, false);
            //selectedCard.transform.DOJump(CurrentCards.ccPos,
            //                            jumpPower: 0.5f,
            //                            numJumps: 1,
            //                            duration: 0.5f).SetEase(Ease.Linear);

            
            //UsersTurnFlag = true;
            CurrentCards.player2Cards.Remove(card);
            _ShowAndroidToastMessage("Player 2 has " + CurrentCards.player2Cards.Count + " cards left!");
            CheckIfPlayer2Won();
            _ShowAndroidToastMessage("Player2 played: " + selectedCard.name);

            if (SpecialCardCheck(selectedCard.name))
            {
                //Action is needed
                if (selectedCard.name.Contains("Draw4"))
                {
                    //Give 4 cards to user
                    //start coroutine for player turn again
                    UsersTurnFlag = false;
                    CurrentCards.sScript.PickCardsForUser(4);
                    //StartCoroutine(Player2TurnWithoutAction());
                    Player2Turn(SpawnScript.uno);
                }
                else if (selectedCard.name == "Wild")
                {
                    UsersTurnFlag = false;
                    Player2Turn(SpawnScript.uno);
                }
                else if (selectedCard.name.Contains("Draw2"))
                {
                    CurrentCards.sScript.PickCardsForUser(2);
                    _ShowAndroidToastMessage("Your turn!");
                    UsersTurnFlag = true;
                }
                else if (selectedCard.name.Contains("Skip"))
                {
                    UsersTurnFlag = false;
                    Player2Turn(SpawnScript.uno);
                }
                else if (selectedCard.name.Contains("Reverse"))
                {
                    UsersTurnFlag = false;
                    Player2Turn(SpawnScript.uno);
                }
            }
            else
            {
                _ShowAndroidToastMessage("Your turn!");
                UsersTurnFlag = true;
            }
                
        }        
    }

    private static void CheckIfPlayer2Won()
    {
        if (CurrentCards.player2Cards.Count == 0)
        {
            _ShowAndroidToastMessage("Player 2 has WON!!!");
            Time.timeScale = 0;
            SceneManager.UnloadSceneAsync(sceneBuildIndex: 1);
            SceneManager.LoadScene(sceneBuildIndex: 3);
            //GameObject p = GameObject.FindGameObjectWithTag("p2won");

            //p.SetActive(true);
           

        }
            
    }

    private IEnumerator Player2TurnWithoutAction()
    {
        yield return new WaitForSeconds(3);
        //IsActionNeeded(uno);
        GamePlay.Player2Turn(SpawnScript.uno);
    }
    internal static string GetRandomStartCard()
    {
        System.Random r = new System.Random();
        string colour = "RYGB";
        string number = "0123456789";

        return colour[r.Next(colour.Length)]+"_"+ number[r.Next(number.Length)];
    }
    internal static Vector3[] getPositions(int numberOfCards)
    {
        Vector3[] positions = new Vector3[numberOfCards];
        Vector3 refPos = new Vector3(-0.1f, 0f, 0.4f);
        for (int i = 0; i < numberOfCards; i++)
        {
            positions[i].y = refPos.y;
            positions[i].x = refPos.x + (float)(i * (0.03));
            positions[i].z = refPos.z + (float)(i * (0.01));
        }
        return positions;
    }

    private static string Player2CardPick(List<string> p2Cards)
    {
        List<string> possiblePlayableCards = new List<string>();
        System.Random r = new System.Random();
        GameObject gObj = GameObject.FindGameObjectWithTag("cc");
        foreach (string card in p2Cards)
        {
            if (card.Contains("Wild") || gObj.name.Contains("Wild"))
                possiblePlayableCards.Add(card);

            if (card.Contains("_"))
            {
                if(gObj.name.Contains('_'))
                {
                    if (card.Split('_')[0] == gObj.name.Split('_')[0]
                    || card.Split('_')[1] == gObj.name.Split('_')[1])
                        possiblePlayableCards.Add(card);
                }
                else
                {
                    if (card.Split('_')[0] == gObj.name.Split('(')[0]
                    || card.Split('_')[1] == gObj.name.Split('(')[0])
                        possiblePlayableCards.Add(card);
                }                
            }
        }
        if (possiblePlayableCards.Count == 0)
            return string.Empty;
        return possiblePlayableCards[r.Next(possiblePlayableCards.Count)];
    }

    internal static void CheckIfUserWon()
    {
        if (CurrentCards.usercards.Count == 0)
        {
            _ShowAndroidToastMessage("You WON!!!");
            Time.timeScale = 0;
            SceneManager.UnloadSceneAsync(sceneBuildIndex: 1);
            SceneManager.LoadScene(sceneBuildIndex: 2);
            //GameObject p = GameObject.FindGameObjectWithTag("userwon"); ;
            //p.SetActive(true);
            
        }
            
    }

    internal static bool CanUseSelectedCard(string selected, string cc)
    {
        if (selected.Contains("Wild") || cc.Contains("Wild"))
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
        else
        {
            _ShowAndroidToastMessage("Your turn again!");
            UsersTurnFlag = true;
        }
        
    }
    internal static void AddXCardsToPlayer2(int x, Deck d)
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
