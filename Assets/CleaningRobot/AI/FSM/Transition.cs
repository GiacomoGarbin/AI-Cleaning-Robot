using System;
using System.Collections.Generic;

public class Transition
{
    public string name;

    public State target;
    public List<Action> actions;
    public Condition condition;

    public Transition(string name = null)
    {
        this.name = name;
        actions = new List<Action>();
    }

    public bool IsTriggered()
    {
        return condition.Test();
    }
}