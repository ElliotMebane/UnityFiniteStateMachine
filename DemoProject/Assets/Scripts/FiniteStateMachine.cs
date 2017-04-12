using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// A Finite State Machine influenced by the work of Jackson Dunstan:
/// http://jacksondunstan.com/articles/3726
/// http://jacksondunstan.com/articles/3137
/// By Roguish.com
/// </summary>
public class FiniteStateMachine     
{
    private Dictionary<Type, BaseState> _states;
    private MonoBehaviour _monoBehaviourContext; // needed so we can call StartCoroutine
    private BaseState _activeState;
    private BaseState _nextState;
    private IEnumerable _activeStateIEnumerable;

	public FiniteStateMachine ( MonoBehaviour pMonoBehaviourContext, List<Type> pStateTypes = null )
	{
        _monoBehaviourContext = pMonoBehaviourContext;
        _states = new Dictionary<Type, BaseState>();

        if (pStateTypes != null && pStateTypes.Count > 0)
        {
            foreach ( Type tStateType in pStateTypes )
            {
                AddState( tStateType );
            }
        }    
    }

    public BaseState AddState ( Type pStateType )
    {
        BaseState tReturnState = null;

        if (!_states.ContainsKey( pStateType ))
        {
            object[] tParams = new object[2] { _monoBehaviourContext, this };
            _states[ pStateType ] = Activator.CreateInstance( pStateType, tParams ) as BaseState;
            tReturnState = _states[ pStateType ];
        }
        
        return tReturnState;
    }

    public BaseState GetState ( Type pStateType )
    {
        if ( _states.ContainsKey( pStateType ) )
        {
            return _states[ pStateType ];
        }
        else
        {
            return AddState( pStateType );
        }
    }
    
    public void Start()
    {
        _monoBehaviourContext.StartCoroutine( ExecuteState() );
    }

    /// <summary>
    /// Initializes the next state and queues it up so it can begin once the activeState is finished exiting.
    /// </summary>
    /// <param name="pStateType">The Type of the next state</param>
    /// <param name="pOverride">There may be conflicts when setting _nextState. 
    /// The activeState may try to set it via this method but during the time it takes
    /// to complete the exit another attempt to set _nextState may be attempted from outside the state. 
    /// Use pOverride when calling SetNextState if you want to be sure 
    /// the next state takes priority over any _nextState that may already be set. 
    /// Subsequent calls to SetNextState may override the setting. </param>
    /// <param name="pBeginExitActiveState">Immediately begin exiting active state after setting the next State.
    /// </param>
    public void SetNextState( Type pStateType, bool pBeginExitActiveState = true, bool pOverride = false )
    {
        if( _nextState == null || pOverride )
        {
            _nextState = GetState( pStateType );
        }
        else
        {
            // dont set _nextState. 
        }

        if( pBeginExitActiveState )
        {
            ExitActiveState();
        }
    }

    public void ExitActiveState()
    {
        if ( _activeState != null )
        {
            _activeState.SetStateInternalState( StateInternalStates.Exiting );
            _activeState.BeginExit();
        }
    }

    /// <summary>
    /// Called by every state when Exit is complete (usually when Execute Iterator is finished).
    /// Responsible for calling SwitchState
    /// </summary>
    public void OnStateExitComplete()
    {
        SwitchState();
    }

    /// <summary>
    /// Switches _activeState to _nextState. 
    /// Set _nextState prior to calling this. 
    /// Made private to encourage avoiding calling directly from State. Instead, State
    /// Should call OnStateExitComplete() which calls this. Make public if need to call directly. 
    /// </summary>
    private void SwitchState ()
    {
        // Will be null on first state 
        if ( _activeState != null )
        {
            _activeState.SetStateInternalState( StateInternalStates.Inactive );
        }

        if ( _nextState != null )
        {
            _activeState = _nextState;
            _nextState = null;
        
            _activeState.SetStateInternalState( StateInternalStates.Execute );
            _activeStateIEnumerable = _activeState.Execute();
        }
    }

    /// <summary>
    /// Called as a coroutine. Change _activeStateIEnumberable to _activeState.Execute() to switch the State that
    /// is actively being processed. 
    /// </summary>
    /// <returns></returns>
    public IEnumerator ExecuteState ()
    {
        // _activeState.Execute() returns the IEnumerable. 
        // foreach calls MoveNext on each IEnumerator in the IEnumerable and current reflects the Iterator. 
        while ( _activeStateIEnumerable != null )
        {
            foreach ( var cur in _activeStateIEnumerable )
            {
                yield return cur;
            }
        }
    }

    public Type GetActiveStateType()
    {
        if ( _activeState != null )
        {
            return _activeState.GetType();
        }
        else
        {
            return null;
        }
    }
    
    public Type GetNextStateType()
    {
        if ( _nextState != null )
        {

            return _nextState.GetType();
        }
        else
        {
            return null;
        }
    }
}
