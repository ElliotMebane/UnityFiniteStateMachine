using System;
using System.Collections;
using UnityEngine;

public interface IState 
{
    IEnumerable Execute ();
    void BeginExit();
    void SetStateInternalState ( StateInternalStates pStateControl );
    void Init( FiniteStateMachine pFSM );
}
