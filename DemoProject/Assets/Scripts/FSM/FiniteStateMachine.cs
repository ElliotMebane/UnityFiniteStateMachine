using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// A low-requirements Finite State Machine 
/// https://github.com/ElliotMebane/UnityFiniteStateMachine
/// By Roguish.com
/// </summary>
public class FiniteStateMachine     
{
    private Dictionary<Type, IState> _states;
    private MonoBehaviour _monoBehaviourContext;
    // The contexts referenced in the states themselves are simple Objects so the states don't have any reliance on MonoBehaviour.
    private System.Object _FSMContext;
    private IState _activeState;
    private IState _nextState;
    private IEnumerable _activeStateIEnumerable;

    ///<summary>Initialize the FSM</summary>
    ///<param name="pMonobehaviourContext">Any Monobehaviour. Primarily for its ability to run a coroutine.</param>
    ///<param name="pContext">The context object which all States reference. May be the same as pMonoBehaviourContext.</param>
    ///<param name="pInitialState">Optional first state to run when the FSM is started.</param>
    ///<param name="pStateTypes">Optional List of state types that will be automatically instantiated and added.</param>
    public FiniteStateMachine ( MonoBehaviour pMonobehaviourContext, System.Object pContext, Type pInitialState = null, List<Type> pStateTypes = null )
	{
        _monoBehaviourContext = pMonobehaviourContext;
        _FSMContext = pContext;
        _states = new Dictionary<Type, IState>();

        if (pStateTypes != null && pStateTypes.Count > 0)
        {
            foreach ( Type tStateType in pStateTypes )
            {
                AddState( tStateType );
            }
        }

        // prepare initial state
        if ( pInitialState != null )
        {
            SetNextState( pInitialState, false );
            SwitchState();
        }
    }

    public IState AddState ( Type pStateType )
    { 
        if ( !_states.ContainsKey( pStateType ) )
        {
            _states[ pStateType ] = (IState)Activator.CreateInstance( pStateType );
            _states[ pStateType ].Init( this );
        } 

        return _states[ pStateType ];
    }

    public IState GetState ( Type pStateType )
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
    /// <param name="pForce">There may be conflicts when setting _nextState. 
    /// The activeState may try to set it via this method but during the time it takes
    /// to complete the exit another attempt to set _nextState may be attempted from outside the state. 
    /// Use pOverride when calling SetNextState if you want to be sure 
    /// pStateType takes priority over any _nextState that may already be set. 
    /// Subsequent calls to SetNextState may override the setting. </param>
    /// <param name="pBeginExitActiveState">Immediately begin exiting active state after setting the next State.</param>
    public void SetNextState( Type pStateType, bool pBeginExitActiveState = true, bool pForce = false )
    {
        if( _nextState == null || pForce )
        {
            _nextState = GetState( pStateType );
        }
        else
        {
            // dont set _nextState if it was already set. If you want to set it even when _nextState has already been set
            // use pForce.
        }

        if( pBeginExitActiveState )
        {
            ExitActiveState();
        }
    }

    /// <summary>When ready to switch states this sets the _activeState's internal state to Exiting and calls its BeginExit method.</summary>
    public void ExitActiveState()
    {
        if ( _activeState != null )
        {
            _activeState.SetStateInternalState( StateInternalStates.Exiting );
            _activeState.BeginExit();
        }
    }

    /// <summary>
    /// Called by states when Exit is complete (usually when Execute Iterator is finished).
    /// Responsible for calling SwitchState
    /// </summary>
    public void OnStateExitComplete()
    {
        SwitchState();
    }

    /// <summary>
    /// Switches _activeState to _nextState. Following this method call, the _nextState will be the active state. 
    /// Set _nextState prior to calling this. 
    /// Made private to discourage calling directly from State. Instead, State
    /// should call OnStateExitComplete() which calls this. Make public if you prefer to call directly. 
    /// </summary>
    private void SwitchState ()
    {
        // Avoid problem when _activeState is null on first pass 
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

    public object FSMContext
    {
        get
        {
            return _FSMContext;
        }
    }
}

public enum StateInternalStates
{
    Inactive,
    Execute,
    Exiting
}
