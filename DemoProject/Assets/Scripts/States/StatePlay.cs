using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// A state for the game play portion of the example
/// </summary>
public class StatePlay : BaseState, IState
{
    private GameObject targetsContainer;
    private List<GameObject> targets;
    private Canvas canvasCover;
    private Image cover;

    public StatePlay()
    {
        // empty
    }

    public void EndEnter()
    {
        // Turn the targets green to indicate that they're ready to be clicked
        foreach ( var target in targets )
        {
            SetTargetColor( target, Color.green );
        }
    }

    public override IEnumerable Execute()
    {
        // Set up the targets
        targetsContainer = new GameObject( "TargetsContainer" );
        var targetPrefab = Resources.Load<GameObject>( "Target" );
        targets = new List<GameObject>( 3 );
        for ( var i = 0; i < targets.Capacity; ++i )
        {
            var target = UnityEngine.Object.Instantiate( targetPrefab );
            target.transform.parent = targetsContainer.transform;
            target.transform.position += new Vector3( i * 2, 0, 0 );
            targets.Add( target );
        }

        // Set up the fade cover
        var screenFadePrefab = Resources.Load<Canvas>( "ScreenFade" );
        canvasCover = UnityEngine.Object.Instantiate( screenFadePrefab );
        var coverGO = canvasCover.transform.Find( "Cover" );
        cover = coverGO.GetComponent<Image>();

        // Fade in transition (fade out cover)
        float fadeTime = 2;
        foreach ( var e in TweenAlpha( 1, 0, fadeTime / 2 ) )
        {
            yield return e;
        }
        canvasCover.enabled = false;

        EndEnter();
        
        // Handle clicks
        while ( _stateInternalState == StateInternalStates.Execute )
        {
            if ( Input.GetMouseButtonDown( 0 ) )
            {
                HandleClick();
            }
            yield return null;
        }

        // Fade out transition (fade in cover)
        canvasCover.enabled = true;
        foreach ( var e in TweenAlpha( 0, 1, fadeTime / 2 ) )
        {
            yield return e;
        }

        EndExit();

        // Notify the FSM that this State has finished Exiting
        _FSM.OnStateExitComplete();
    }
    
    public void EndExit()
    {
        // Clean up the targets
        UnityEngine.Object.Destroy( targetsContainer );

        // Clean up the UI
        UnityEngine.Object.Destroy( canvasCover.gameObject );
    }

    private void HandleClick()
    {
        // Cast a ray where the user clicked
        var ray = Camera.main.ScreenPointToRay( Input.mousePosition );
        RaycastHit hitInfo;
        if ( Physics.Raycast( ray, out hitInfo ) == false )
        {
            return;
        }

        // Check if the user clicked any targets
        foreach ( var target in targets )
        {
            if ( target != null && hitInfo.transform == target.transform )
            {
                HitTarget( target );
                break;
            }
        }
    }

    private void HitTarget( GameObject target )
    {
        // Turn the target red so the user knows they hit it
        SetTargetColor( target, Color.red );

        // If the user hit all the targets
        targets.Remove( target );
        if ( targets.Count == 0 )
        {
            // Transition to the main menu using the screen fade transition
            //var nextState = new MainMenuState();
            //var transition = new ScreenFadeTransition( 2 );
            //var eventArgs = new StateBeginExitEventArgs( nextState, transition );
            //OnBeginExit( this, eventArgs );

            _FSM.SetNextState( typeof( StateMainMenu ), true );
        }
    }

    private void SetTargetColor( GameObject target, Color color )
    {
        var renderer = target.GetComponent<Renderer>();
        renderer.material.color = color;
    }

    // Tween the alpha of the fade cover
    private IEnumerable TweenAlpha(
        float fromAlpha,
        float toAlpha,
        float duration
    )
    {
        var startTime = Time.time;
        var endTime = startTime + duration;
        while ( Time.time < endTime )
        {
            var sinceStart = Time.time - startTime;
            var percent = sinceStart / duration;
            var color = cover.color;
            color.a = Mathf.Lerp( fromAlpha, toAlpha, percent );
            cover.color = color;
            yield return null;
        }
    }
}