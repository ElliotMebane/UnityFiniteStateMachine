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
— Uses the Type of the State Classes for saving a dictionary of States that have been added to the FSM.  
— No Enter state. A State may implement a transition when it starts as the first part of the Execute method.  
— Manual call to FSM when State finishes Exiting. Although Loose Coupling is worthwhile in many architectural decisions, StateMachines seem like a good place for some tight coupling.  
— No transitions as separate classes are required. States handle their own transitions as part of the Execute method.  
  
Downsides:  
When doing a similar transition in multiple frames, we may see repetition of similar transition code in each State. This could be avoided by optionally putting the transition content in a separate class. This might look similar to the template Transition class used in Dunstan's sample.  
  
I implemented the FSM in 2 sample projects. One was based on Dunstan's sample and is included in this repository. The second is a Poker game I published here:   https://github.com/ElliotMebane/SevenUpDrawPoker  


 
