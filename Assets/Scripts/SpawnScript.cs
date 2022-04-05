using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class SpawnScript : MonoBehaviour
{
    public Transform player2;
    public GameObject player2deck;
    public Transform CenterDeck;
    public GameObject deck;
    public GameObject cCard;
    public List<GameObject> usercards = new List<GameObject>();
    public List<string> player2Cards = new List<string>();

    private float tapSpeed = 0.5f; //in seconds
    private float lastTapTime = 0;
    //private GamePlay gPlay;
    
    public GameObject Getcc()
    {
        return cCard;
    }
    private GameObject[] cards = new GameObject[7];
    private Vector3 pos = new Vector3(0, 0, 0.3f);
    private Vector3 currentCardPos = new Vector3(-0.1f, -0.3f, 0.5f);
    //private Vector3 currentCardPos = new Vector3(-0.2f, 0f, 0.5f);
    public static Deck uno;
    private bool userPlayedCard = false;
    // Start is called before the first frame update
    void Start()
    {
        lastTapTime = 0;
       // gPlay = new GamePlay();
        uno = new Deck();
        uno.CreateDeck();
        CurrentCards.sScript = this;

        //Instantiating user cards
        uno.allCards = uno.Shuffle(uno.allCards);
        Vector3[] pos = GamePlay.getPositions(7);
        string[] player1cards = uno.GetNumberOfCards(7, uno.allCards);
        for(int c = 0; c<cards.Length; c++)
        {
            GameObject gObj = Resources.Load<GameObject>("Cards/"+player1cards[c]);
            
            if(gObj != null)
            {
                gObj.tag = "userCard";
                Instantiate(gObj, pos[c], Quaternion.Euler(new Vector3(90f, 180f, 0f)));
                CurrentCards.AddToUserCards(gObj);
                usercards.Add(gObj);
            }
                            
        }
        string[] p2 = (uno.GetNumberOfCards(7, uno.allCards));
        foreach (string c in p2)
            player2Cards.Add(c);
        CurrentCards.AddToPlayer2Cards(p2);

        //TO-DO : cant have rev/skip/draw2/4/wild/ cards
        //Instantiate current card
        //string cardInPlay = uno.GetNumberOfCards(1, uno.allCards)[0];
        string cardInPlay = GamePlay.GetRandomStartCard();
        
        GameObject g = Resources.Load<GameObject>("Cards/"+cardInPlay);
        g.name += "(Clone)";
        g.tag = "cc";
        Instantiate(g, currentCardPos, Quaternion.Euler(new Vector3(90f, 180f, 0f)));
        CurrentCards.currentCard = g;
        cCard = g;
        GamePlay._ShowAndroidToastMessage("Current card: " + g.name);
        
        
        //Instantiating common deck and Player2 decks
        Instantiate(deck, CenterDeck.position, Quaternion.LookRotation(Vector3.down));
        Instantiate(player2deck, player2.position, Quaternion.LookRotation(Vector3.down));

        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        CurrentCards.sScript = this;
        for (var i = 0; i < Input.touchCount; ++i) 
        //if(Input.touchCount == 1)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began) 
            {
                if((Time.time - lastTapTime) < tapSpeed)
                {
                    // Construct a ray from the current touch coordinates
                    //Debug.Log("double tap!");
                   
                    //Debug.Log("double tap!"+GamePlay.UsersTurnFlag.ToString());
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                    if(Physics.Raycast(ray, out hit) && GamePlay.UsersTurnFlag) // check if it is user's turn
                    {
                        Debug.Log(hit.transform.name+"hit");
                        if(!hit.transform.name.Contains("Deck"))
                        {
                            GameObject sCard = hit.transform.gameObject;
                            if (cCard == null)
                                Debug.Log("c Card in Spawnscript is null");
                            if (GameObject.FindWithTag("cc").gameObject == null)
                                Debug.Log("cc is null");
                            

                            if (GamePlay.CanUseSelectedCard(sCard.name, GameObject.FindWithTag("cc").gameObject.name))
                            {
                                //Debug.Log(sCard.name + " name");
                                
                                Vector3 currCardPos = GameObject.FindWithTag("cc").transform.position;
                                if (currCardPos != null)
                                {
                                    //Debug.Log(hit.transform.position.ToString() + " before destroy");
                                    GamePlay.DestroyCC();
                                    
                                    GameObject smokePrefab = Resources.Load<GameObject>("Smoke/Smoke");
                                    if (smokePrefab != null)
                                    {
                                        Instantiate(smokePrefab, CurrentCards.ccPos, Quaternion.LookRotation(hit.normal));
                                        sCard.transform.DOJump(currCardPos,
                                        jumpPower: 0.5f,
                                        numJumps: 1,
                                        duration: 2f).SetEase(Ease.InBounce);
                                        sCard.tag = "cc";
                                        cCard = sCard;
                                        CurrentCards.currentCard = sCard.gameObject;

                                        //updating list of user cards
                                        CurrentCards.usercards.Remove(CurrentCards.usercards.First(x => x.name == sCard.name.Split('(')[0]));

                                        GamePlay.CheckIfUserWon();
                                        
                                        StartCoroutine(Call());                                        
                                    }
                                }
                            }                          

                        }
                    }
                }
                else
                lastTapTime = Time.time;
                
            }
        }
    }

    
    public void PickCardsForUser(int numOfNewCards)
    {
        //if(GamePlay.UsersTurnFlag)
        {
            GameObject[] uCards = GameObject.FindGameObjectsWithTag("userCard");
            List<string> newCardNames = new List<string>();
            Vector3[] positions;
            
            List<string> uCardNames = new List<string>();
            

            foreach(GameObject g in uCards)
            {
                uCardNames.Add(g.name.Split('(')[0]);                
            }
             
            if(uCards.Length != 0)
            {

                for (int j = 0; j < numOfNewCards; j++)
                    newCardNames.Add(uno.allCards.Pop());
                //card = uno.allCards.Pop();
                
                //uCardNames.Concat(newCardNames);
                newCardNames.ForEach(x => uCardNames.Add(x));

                if(uCardNames.Count != 0)
                {
                    int numberOfCards = uCards.Length;

                    foreach (GameObject obj in uCards)
                        DestroyImmediate(obj);

                    //GameObject newCard = Resources.Load<GameObject>("Cards/" + card);
                    //newCard.tag = "userCard";
                    positions = GamePlay.getPositions(uCardNames.Count );
                    CurrentCards.usercards.Clear();
                    for(int i = 0; i < uCardNames.Count; i++)
                    {
                        GameObject gCard = Resources.Load<GameObject>("Cards/" + uCardNames[i]);
                        if (gCard != null)
                        {
                            gCard.tag = "userCard";
                            Instantiate(gCard, positions[i], Quaternion.Euler(new Vector3(90f, 180f, 0f)));

                            CurrentCards.usercards.Add(gCard);
                        }
                    }

                    
                    GamePlay.UsersTurnFlag = false;
                    StartCoroutine(CallWithoutAction());
                    
                    GamePlay._ShowAndroidToastMessage("You have "+ numOfNewCards+" new card(s)!");
                }                
            }
        }
    }
   private IEnumerator Call()
    {
        yield return new WaitForSeconds(3);
        IsActionNeeded(uno);
        GamePlay.Player2Turn(uno);
    }
    private IEnumerator CallWithoutAction()
    {
        yield return new WaitForSeconds(3);
        //IsActionNeeded(uno);
        GamePlay.Player2Turn(uno);
    }
    private void IsActionNeeded( Deck d)
    {
        GameObject gObj = GameObject.FindGameObjectWithTag("cc");
        if (GamePlay.SpecialCardCheck(gObj.name))
        {
            if (gObj.name.Contains("Wild"))
                GamePlay.WildThrownByUser(gObj.name, d);
            else if (gObj.name.Contains("Skip")
                    || gObj.name.Contains("Reverse"))
            {
                GamePlay.UsersTurnFlag = true;
                GamePlay._ShowAndroidToastMessage("Your turn again!");
            }
                
            else if (gObj.name.Contains("Draw2"))
            {
                GamePlay.AddXCardsToPlayer2(2, d);
                GamePlay._ShowAndroidToastMessage("2 cards to  player 2");
                GamePlay.UsersTurnFlag = false;
            }
        }
        else
        {
            GamePlay.UsersTurnFlag = false;
            //gPlay.Player2Turn();
        }
    }
    public static void MakeGameWait()
    {
        //yield return new WaitForSeconds(4);
    }

    
}
