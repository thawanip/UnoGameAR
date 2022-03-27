using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SpawnScript : MonoBehaviour
{
    public Transform player2;
    public GameObject player2deck;
    public Transform CenterDeck;
    public GameObject deck;
    private float tapSpeed = 0.5f; //in seconds
    private float lastTapTime = 0;
    private GamePlay gPlay;
    

    private GameObject[] cards = new GameObject[7];
    private Vector3 pos = new Vector3(0, 0, 0.3f);
    private Vector3 currentCardPos = new Vector3(-0.1f, -0.5f, 0.6f);
    // Start is called before the first frame update
    void Start()
    {
        lastTapTime = 0;
 
        Deck uno = new Deck();
        uno.CreateDeck();

        //Instantiating user cards
        uno.allCards = uno.Shuffle(uno.allCards);
        Vector3[] pos = getPositions(7);
        string[] player1cards = uno.GetNumberOfCards(7, uno.allCards);
        for(int c = 0; c<cards.Length; c++)
        {
            GameObject gObj = Resources.Load<GameObject>("Cards/"+player1cards[c]);
            
            if(gObj != null)
            {
                Instantiate(gObj, pos[c], Quaternion.Euler(new Vector3((float)90,(float)180,(float)0)));
                CurrentCards.AddToUserCards(gObj);
            }
                            
        }        

        CurrentCards.AddToPlayer2Cards(uno.GetNumberOfCards(7, uno.allCards));
        
        //TO-DO : cant have rev/skip/draw2/4/wild/ cards
        //Instantiate current card
        GameObject g = Resources.Load<GameObject>("Cards/"+uno.GetNumberOfCards(1, uno.allCards)[0]);
        g.name += "(Clone)";
        g.tag = "cc";
        Instantiate(g, currentCardPos, Quaternion.LookRotation(Vector3.down));
        CurrentCards.currentCard = g;
        
        
        //Instantiating common deck and Player2 decks
        Instantiate(deck, CenterDeck.position, Quaternion.LookRotation(Vector3.down));
        Instantiate(player2deck, player2.position, Quaternion.LookRotation(Vector3.down));

        gPlay = new GamePlay();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        for (var i = 0; i < Input.touchCount; ++i) 
        //if(Input.touchCount == 1)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began) 
            {
                if((Time.time - lastTapTime) < tapSpeed)
                {
                    // Construct a ray from the current touch coordinates
                    Debug.Log("double tap!"+gPlay.UsersTurnFlag.ToString());
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                    if(Physics.Raycast(ray, out hit) && gPlay.UsersTurnFlag) // check if it is user's turn
                    {
                        Debug.Log(hit.transform.name+"hit");
                        if(!hit.transform.name.Contains("Deck"))
                        {
                            Debug.Log(hit.transform.name+" name");
                            //Transform currCardPos = GameObject.Find(CurrentCards.currentCard.name).transform;
                            Transform currCardPos = GameObject.FindWithTag("cc").transform;
                            if(currCardPos != null)
                            {
                                Debug.Log(hit.transform.position.ToString() + " before destroy");
                                Destroy(GameObject.FindWithTag("cc"));

                                //Destroy(hit.transform.gameObject);
                                GameObject smokePrefab = Resources.Load<GameObject>("Smoke/Smoke");
                                if (smokePrefab != null)
                                {
                                    Instantiate(smokePrefab, currCardPos.position, Quaternion.LookRotation(hit.normal));
                                    hit.transform.gameObject.transform.DOJump(currCardPos.position,
                                    jumpPower: 1,
                                    numJumps: 1,
                                    duration: 2f).SetEase(Ease.InOutBounce);
                                    CurrentCards.currentCard = hit.transform.gameObject;
                                    CurrentCards.currentCard.tag = "cc";
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
    Vector3[] getPositions(int numberOfCards)
    {
        Vector3[] positions = new Vector3[numberOfCards];
        Vector3 refPos = new Vector3((float)-0.1, 0, (float)0.4);
        for(int i = 0; i < numberOfCards; i++)
        {
            positions[i].y = refPos.y;
            positions[i].x = refPos.x + (float)(i*(0.04));
            positions[i].z = refPos.z + (float)(i*(0.01));
        }
        return positions;
    }
}
