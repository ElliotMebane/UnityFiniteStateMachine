using UnityEngine;

public class TestScript : MonoBehaviour
{
    private FiniteStateMachine _FSM;
    
    void Start()
    {
        // Create the initial state
        // var mainMenuState = new MainMenuState();
        // Create the state machine
        // var stateMachine = new FiniteStateMachine( mainMenuState );
        // Run the state machine
        // StartCoroutine( stateMachine.Execute().GetEnumerator() );

        _FSM = new FiniteStateMachine( this );
        _FSM.SetNextState( typeof( StateMainMenu ), false );
        _FSM.OnStateExitComplete(); 
        _FSM.Start();
    }
}