using System;
using System.Collections;
using UnityEngine;

public interface IState 
{
    IEnumerable Execute ();
    void BeginExit();
    void SetStateInternalState ( StateInternalStates pStateControl );
    void init( System.Object pContext, FiniteStateMachine pFSM );
}
