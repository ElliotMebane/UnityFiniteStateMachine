using System;
using System.Collections;
using UnityEngine;

public class BaseState : IState 
{
    protected System.Object _contextObject;
    protected StateInternalStates _stateInternalState;
    protected FiniteStateMachine _FSM; 
    
    public BaseState ( System.Object pContext, FiniteStateMachine pFSM )
    {
        _contextObject = pContext;
        _FSM = pFSM;

        _stateInternalState = StateInternalStates.Inactive;
    }

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

public enum StateInternalStates
{
    Inactive,
    Execute,
    Exiting
}
