using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected State m_currentState;

    public virtual void SwitchState(State newState) 
    {
        m_currentState.OnStateExit(this);
        m_currentState = newState;
        m_currentState.OnStateEnter(this);
    }
}
