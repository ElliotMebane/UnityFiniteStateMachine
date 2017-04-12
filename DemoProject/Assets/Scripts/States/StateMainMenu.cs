using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StateMainMenu : BaseState, IState {

    private Canvas canvas;
    private Canvas canvasCover;
    private Image cover;
    private Text frameCount;
    private int initialFrame;

    public StateMainMenu( System.Object pContext, FiniteStateMachine pFSM ) : base( pContext, pFSM )
    {
        // empty
    }

    public override IEnumerable Execute()
    {
        // Create the main menu UI
        var canvasPrefab = Resources.Load<Canvas>( "MainMenu" );
        canvas = UnityEngine.Object.Instantiate( canvasPrefab );
        var playButtonGO = canvas.transform.Find( "PlayButton" );
        var playButton = playButtonGO.GetComponent<Button>();
        playButton.onClick.AddListener( HandlePlayButton );
        var frameCountGO = canvas.transform.Find( "FrameCount" );
        frameCount = frameCountGO.GetComponent<Text>();
        initialFrame = Time.frameCount;

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

        // Update the frame count until we transition
        while ( _stateInternalState == StateInternalStates.Execute )
        {
            var numFrames = Time.frameCount - initialFrame;
            frameCount.text = "Frames spent on menu: " + numFrames;
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
        // Clean up the UI
        UnityEngine.Object.Destroy( canvas.gameObject );
        UnityEngine.Object.Destroy( canvasCover.gameObject );
    }

    private void HandlePlayButton()
    {
        // Transition to the play state using a screen fade
        // var nextState = new PlayState();
        _FSM.SetNextState( typeof( StatePlay ), true );
        // var transition = new ScreenFadeTransition( 2 );
        // var eventArgs = new StateBeginExitEventArgs( nextState, transition );
        // OnBeginExit( this, eventArgs );
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
