using System;
using System.Collections;
using UnityEngine;

public class BaseState : IState 
{
    protected System.Object _FSMContext;
    protected StateInternalStates _stateInternalState;
    protected FiniteStateMachine _FSM; 
    
    public BaseState ()
    {
        _stateInternalState = StateInternalStates.Inactive;
    }

    public virtual void Init( FiniteStateMachine pFSM )
    {
        _FSM = pFSM;
        _FSMContext = _FSM.FSMContext;
    }

    /// <summary>
    /// Called once per frame from FSM while this is the active state</summary>
    public virtual IEnumerable Execute ()
    {
        yield return null;
    }

    public virtual void SetStateInternalState( StateInternalStates pStateControl )
    {
        _stateInternalState = pStateControl;
    }

    /// <summary>
    /// Called from FiniteStateMachine when ExitActiveState is called in the FSM.  
    /// FiniteStateMachine sets the _stateInternalState to Exiting prior to calling BeginExit.
    /// Avoid calling BeginExit directly from within a state. Rather, call _FSM.SetNextState().
    /// </summary>
    public virtual void BeginExit()
    {
        // empty
    }
 
}