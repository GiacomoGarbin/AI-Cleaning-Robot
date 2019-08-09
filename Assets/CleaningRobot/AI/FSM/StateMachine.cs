using System;
using System.Collections.Generic;

public class StateMachine
{
    public State CurrentState;
    public Transition LastTransition;

    public StateMachine(State InitialState)
    {
        CurrentState = InitialState;
        LastTransition = null;
    }

    public void Start()
    {
        PlayActions(CurrentState.EntryActions);
    }

    public void Update()
    {
        Transition TriggeredTransition = null;

        foreach (Transition transition in CurrentState.transitions)
        {
            if (transition.IsTriggered())
            {
                TriggeredTransition = transition;
                break;
            }
        }

        if (TriggeredTransition != null)
        {
            State TargetState = TriggeredTransition.target;

            List<Action> actions = new List<Action>();
            actions.AddRange(CurrentState.ExitActions);
            actions.AddRange(TriggeredTransition.actions);
            actions.AddRange(TargetState.EntryActions);

            PlayActions(actions);

            CurrentState = TargetState;
            LastTransition = TriggeredTransition;
        }
        else
        {
            PlayActions(CurrentState.StayActions);
        }
    }

    private void PlayActions(List<Action> actions)
    {
        foreach (Action action in actions)
        {
            action();
        }
    }
}