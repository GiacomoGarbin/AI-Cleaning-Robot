using System;
using System.Collections.Generic;

public class State
{
    public string name; 

    public List<Action> EntryActions;
    public List<Action> StayActions;
    public List<Action> ExitActions;
    public List<Transition> transitions;

    public State(string name = null)
    {
        this.name = name;
        EntryActions = new List<Action>();
        StayActions = new List<Action>();
        ExitActions = new List<Action>();
        transitions = new List<Transition>();
    }
}