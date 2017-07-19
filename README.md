# UnityFiniteStateMachine
A Finite State Machine for Unity with States encapsulated in their own classes, few structural requirements of each State and minimal reliance on Unity classes. 

I found 3 State Machines with features I like. I combined the features I like to create this State Machine. All 3 share the notion of _Pure Code_, meaning little to no reliance on Unity code or classes like MonoBehaviour. More info about each of the State Machines I referenced when writing this FSM is below:  
  
1) Jackson Dunstan's FSM  
http://jacksondunstan.com/articles/3137  
— Encapsulates State behavior into dedicated discrete State classes.  
— Structured with anticipation of transitional time during which the primary State activity should be disabled while the State is still the active State and the State's Begin or Exit state is executed.  
— Downside: built around the expectation that States are responsible for triggering the transitions to the next state. I wanted a FSM that is more of a Controller that orchestrates the behavior of the States, knows which is active and which is next, and the FSM itself can be told to exit the current state and then enter the next state. Dunstan's FSM is geared towards the States themselves controlling the initiation of the change to the next State.  
  
2) Jackson Dunstan's Simpler FSM  
http://jacksondunstan.com/articles/3726  
— Removes the constraint of using discrete State objects for separate State actions, however still permits it when appropriate.  
— Demonstrates the use of a single _Execute_ method in which the user can do any Begin/Execute/Exit/Etc. activities through a variety of Iterator approaches that iterate until they are finished (or are told to stop) and then the body of the Execute method resumes.  
  
3) StateKit from Prime31  
https://github.com/prime31/StateKit  
— Also encapsulates State behavior in State classes.  
— Injects data from the State Machine into each of to the States. a) Injects a reference to the State Machine for callback communication from State-to-State Machine, and b) Injects a _context_ object on which the State should operate. Prime31 uses the Generic T so the _context_ could be anything (MonoBehavior, Custom Class, etc.).  
— The FSM manages the States and initiates transitions from a Controller-like perspective (see downside listed in FSM 1 above).  
— Uses the Type of the State classes for saving a dictionary of States that have been added to the FSM.  
— Uses a manually-implemented _update_ method for ticking the FSM. The FSM must have its _update_ method called every frame, and it relays the call to the active State's _update_ method.  
  
Here are the features of this FSM, cobbled together from features in the above FSMs as well as some additional features.  
— Encapsulates State activity in dedicated discrete State classes.  
— Injects data from the State Machine into each of to the States for callbacks to the FSM and reference to a context object to act upon.  
— Anticipates transitional behavior.  
— Uses a single Execute method for all Iterator activity. The BeginExit method allows the FSM to initiate the State's exit or the State itself may trigger the exit when its internal processes are finished.  
— Each State is responsible for its own transitions. A shared transition can be used but is not required. Any IEnumerable method in any class may be called from a State to implement something like a shared Transition class.  
— Use of an Interface so that it's easy to ensure that all States have the necessary properties (like a reference to the FSM) without forcing user to extend a BaseState. StateKit handles similar concerns in a similar way: manual injection of important info into concrete States via a call to a setMachineAndContext method (which is enforced by its Abstract SKState class) as part of the FSM's addState method. I enforce an init method via an Interface which takes a context object on which the State should operate and a reference to the FSM that controls the State's fate.  
— Uses the Type of the State Classes for saving a dictionary of States that have been added to the FSM (like StateKit)  
— No Enter state. A State may implement a transition when it starts as the first part of the Execute method.  
— Manual call to FSM when State finishes Exiting. Although Loose Coupling is worthwhile in many architectural decisions, StateMachines seem like a good place for some tight coupling. Instead of using an Observer pattern to allow States to notify their controlling FSM when they're done exiting, I make the call to FSM.OnStateExitComplete directly from State to FSM.  
— No transitions as separate classes are required. States handle their own transitions as part of the Execute method.  
  
<strong>Q: Can you give me an overview of the flow that happens when moving from State to State?</strong>  
The execute method of a State is where all the action in a State is based. When a State first becomes active Execute will be called as a coroutine and will usually run until it encounters some code structure that causes it to return an IEnumerable. This could be a foreach loop, while loop, etc. (See the example code for details). The code will often _hold_ at this point in the Execute method, waiting for some user input to release it and allow the code to proceed farther down the Execute method. Often, setting the \_stateInternalState to something other than StateInternalStates.Execute is the best way to indicate to the Execute method that it's time to move ahead. A good way to do this is to call SetNextState on the FSM after some user action, which in turn updates the State's \_stateInternalState then calls BeginExit on the State. The active State can recognize that \_stateInternalState is not set to StateInternalStates.Execute anymore and release the Execute method to continue running. BeginExit and the tail end of the Execute method are where you perform _exit_ related cleanup and transition activities on the active State before allowing the next State to become active. After completing all the exiting activities on the active State, allow the FSM to transition to the next State by calling OnStateExitComplete on the FSM.  
  
<strong>Q: What things may I expect to happen when a State is active?</strong>  
— Every frame while the FSM is running the active state's Execute method will be called. 
— When FSM.ExitActiveState is called (directly or by way of SetNextState) \_stateInternalState will be set to StateInternalStates.Exiting so that the State may act accordingly. 
— \_state.OnBeginExit will be called in the State by the FSM when exiting is initiated.  

<strong>Q: Can you give a high-level overview of the requirements and constraints of using the FSM?</strong>  
— FSM must have a reference to a MonoBehaviour so it can call coroutine.  
— States must store a reference to the FSM so they may call methods on it.  
— States must implement IState and have:  
a) public Execute that returns IEnumerable.  
b) public BeginExit that the FSM will call when a State transition has been initiated.  
c) public SetStateInternalState that takes a StateInternalStates enum.  
d) public Init takes a reference to the FSM instance.  
— States must call FSM.OnStateExitComplete to inform the FSM that Exiting is complete.  
  
<strong>Downsides:</strong>     
When doing a similar transition in multiple frames, we may see repetition of similar transition code in each State. This could be avoided by optionally putting the transition content in a separate class. This might look similar to the template Transition class used in Dunstan's sample.    
  
I implemented the FSM in 2 sample projects. One was based on Dunstan's sample and is included in this repository. The second is a Poker game I published here:  https://github.com/ElliotMebane/SevenUpDrawPoker  


 
