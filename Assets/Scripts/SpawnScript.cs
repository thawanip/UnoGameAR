using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    public Transform player2;
    public GameObject player2deck;
    public Transform CenterDeck;
    public GameObject deck;
    public float tapSpeed = 1f; //in seconds
 
    private float lastTapTime = 0;

    private GameObject[] cards = new GameObject[7];
    private Vector3 pos = new Vector3(0, 0, (float)0.3);
    private Vector3 currentCardPos = new Vector3((float)-0.1, (float)-0.5, (float)0.6);
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
            //gObj.GetComponents<MeshCollider>()
            if(gObj != null)
                Instantiate(gObj, pos[c], Quaternion.Euler(new Vector3((float)90,(float)180,(float)0)));            
        }
        CurrentCards.AddToUserCards(player1cards);

        CurrentCards.AddToPlayer2Cards(uno.GetNumberOfCards(7, uno.allCards));
        //Instantiate current card
        GameObject g = Resources.Load<GameObject>("Cards/"+uno.GetNumberOfCards(1, uno.allCards)[0]);
        Instantiate(g, currentCardPos, Quaternion.LookRotation(Vector3.down));
        
        
        //Instantiating common deck and Player2 decks
        Instantiate(deck, CenterDeck.position, Quaternion.LookRotation(Vector3.down));
        Instantiate(player2deck, player2.position, Quaternion.LookRotation(Vector3.down));
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
                    Debug.Log("double tap!");
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                    if(Physics.Raycast(ray, out hit))
                    {
                        Debug.Log(hit.transform.name+"hit");
                        if(!hit.transform.name.Contains("Deck"))
                        {
                            Destroy(hit.transform.gameObject);
                            //Instantiate(smoke, hit.transform.position, Quaternion.LookRotation(hit.normal));
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
