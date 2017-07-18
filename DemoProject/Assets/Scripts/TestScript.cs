using UnityEngine;

public class TestScript : MonoBehaviour
{
    private FiniteStateMachine _FSM;
    
    void Start()
    {
        _FSM = new FiniteStateMachine( this, this, typeof( StateMainMenu ) );
        _FSM.Start();
    }
}