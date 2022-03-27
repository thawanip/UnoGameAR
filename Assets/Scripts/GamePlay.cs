using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class GamePlay : MonoBehaviour
{
    private bool _usersTurnFlag = true;
    public bool UsersTurnFlag 
    {
        get => _usersTurnFlag;
        set
        {
            _usersTurnFlag = value;
            if(!_usersTurnFlag)
            {
                //Thread.Sleep(3000);
                Player2Turn();
            }
            //else
            //    UsersTurn();
        }
    }

    private void Player2Turn()
    {
        if(CurrentCards.currentCard.Contains("Reverse"))
            UsersTurn();
        //if()
    }
    private void UsersTurn()
    {

    }

    
    
}
