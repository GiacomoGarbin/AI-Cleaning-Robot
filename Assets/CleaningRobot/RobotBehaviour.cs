using System;
using System.Collections;
using UnityEngine;

public class RobotBehaviour : StateMachine
{
    MonoBehaviour MB;

    /* STATES */

    State SearchTrash = new State("SearchTrash");
    State HeadForTrash = new State("HeadForTrash");
    State WaitForFellows = new State("WaitForFellows");
    State HeadForTrashCompactor = new State("HeadForTrashCompactor");
    State HeadForPowerStation = new State("HeadForPowerStation");
    State ChargeBattery = new State("ChargeBattery");
    State WaitForRescue = new State("WaitForRescue");
    State RescueByFellow = new State("RescueByFellow");
    State HeadForStoppedRobot = new State("HeadForStoppedRobot");
    State HeadForLeaderRobot = new State("HeadForLeaderRobot");
    State HeadForFellowPowerStation = new State("HeadForFellowPowerStation");
    State HelpFellow = new State("HelpFellow");

    /* TRANSITIONS */

    Transition SightedTrash = new Transition("SightedTrash");
    Transition PickedUpTrash = new Transition("PickedUpTrash");
    Transition ReachedTrashAndLightTrash = new Transition("ReachedTrash AND LightTrash");
    Transition ReachedTrashAndHeavyTrash = new Transition("ReachedTrash AND HeavyTrash");
    Transition ReachedTrashCompactorAndLightTrash = new Transition("ReachedTrashCompactor AND LightTrash");
    Transition ReachedTrashCompactorAndHeavyTrash = new Transition("ReachedTrashCompactor AND HeavyTrash");
    Transition CalledByFellowAndWaitForRescue = new Transition("CalledByFellow AND WaitForRescue");
    Transition CalledByFellowAndWaitForFellows = new Transition("CalledByFellow AND WaitForFellows");
    Transition PickedUpFellow = new Transition("PickedUpFellow");
    Transition NotWaitForFellows = new Transition("NOT WaitForFellows");
    Transition FellowCompletedFormation = new Transition("FellowCompletedFormation");
    Transition ExpiredTimeout = new Transition("ExpiredTimeout");
    Transition CompletedFormation = new Transition("CompletedFormation");
    Transition ReachedStoppedRobot = new Transition("ReachedStoppedRobot");
    Transition ReachedLeaderRobot = new Transition("ReachedLeaderRobot");
    Transition FellowBreakedFormation = new Transition("FellowBreakedFormation");
    Transition LowBattery = new Transition("LowBattery");
    Transition LowBatteryAndNotHeavyTrash = new Transition("LowBattery AND (NOT HeavyTrash)");
    Transition FellowLowBattery = new Transition("FellowLowBattery");
    Transition EmptyBattery = new Transition("EmptyBattery");
    Transition ReachedPowerStation = new Transition("ReachedPowerStation");
    Transition ChargedBattery = new Transition("ChargedBattery");
    Transition StartedRescue = new Transition("StartedRescue");
    Transition StoppedRescue = new Transition("StoppedRescue");
    Transition CompletedRescue = new Transition("CompletedRescue");
    Transition ReachedFellowPowerStation = new Transition("ReachedFellowPowerStation");

    public RobotBehaviour(Robot robot) : base(null)
    {
        MB = UnityEngine.Object.FindObjectsOfType<RobotController>()[0].GetComponent<MonoBehaviour>();

        /* STATES */

        /* ----- SearchTrash ----- */

        // ENTRY ACTIONS
        // SearchTrash.EntryActions.Add(robot.SetDestinationForNothing);
        SearchTrash.EntryActions.Add(robot.wander.StartWandering);
        // STAY ACTIONS
        // ...
        // EXIT ACTIONS
        SearchTrash.ExitActions.Add(robot.wander.StopWandering);
        // TRANSITIONS
        SearchTrash.transitions.Add(LowBattery);
        SearchTrash.transitions.Add(CalledByFellowAndWaitForRescue);
        SearchTrash.transitions.Add(CalledByFellowAndWaitForFellows);
        SearchTrash.transitions.Add(SightedTrash);

        /* ----- HeadForTrash ----- */

        // ENTRY ACTIONS
        HeadForTrash.EntryActions.Add(robot.SetDestinationForTrash);
        // STAY ACTIONS
        // ...
        // EXIT ACTIONS
        HeadForTrash.ExitActions.Add(robot.SetDestinationForNothing);
        // TRANSITIONS
        HeadForTrash.transitions.Add(LowBattery);
        HeadForTrash.transitions.Add(PickedUpTrash);
        HeadForTrash.transitions.Add(CalledByFellowAndWaitForRescue);
        HeadForTrash.transitions.Add(CalledByFellowAndWaitForFellows);
        HeadForTrash.transitions.Add(ReachedTrashAndLightTrash);
        HeadForTrash.transitions.Add(ReachedTrashAndHeavyTrash);

        /* ----- HeadForTrashCompactor ----- */

        // ENTRY ACTIONS
        HeadForTrashCompactor.EntryActions.Add(robot.SetDestinationForTrashCompactor);
        // STAY ACTIONS
        // ...
        // EXIT ACTIONS
        HeadForTrashCompactor.ExitActions.Add(robot.SetDestinationForNothing);
        // TRANSITIONS
        HeadForTrashCompactor.transitions.Add(LowBatteryAndNotHeavyTrash);
        HeadForTrashCompactor.transitions.Add(FellowLowBattery);
        HeadForTrashCompactor.transitions.Add(ReachedTrashCompactorAndLightTrash);
        HeadForTrashCompactor.transitions.Add(ReachedTrashCompactorAndHeavyTrash);

        /* ----- HeadForStoppedRobot ----- */

        // ENTRY ACTIONS
        HeadForStoppedRobot.EntryActions.Add(robot.SetDestinationForFellow);
        // STAY ACTIONS
        // ...
        // EXIT ACTIONS
        HeadForStoppedRobot.ExitActions.Add(robot.SetDestinationForNothing);
        // TRANSITIONS
        HeadForStoppedRobot.transitions.Add(LowBattery);
        HeadForStoppedRobot.transitions.Add(PickedUpFellow);
        HeadForStoppedRobot.transitions.Add(ReachedStoppedRobot);

        /* ----- HeadForFellowPowerStation ----- */

        // ENTRY ACTIONS
        HeadForFellowPowerStation.EntryActions.Add(robot.SetDestinationForFellowPowerStation);
        // STAY ACTIONS
        // ...
        // EXIT ACTIONS
        HeadForFellowPowerStation.ExitActions.Add(robot.SetDestinationForNothing);
        // TRANSITIONS
        HeadForFellowPowerStation.transitions.Add(LowBattery);
        HeadForFellowPowerStation.transitions.Add(ReachedFellowPowerStation);

        /* ----- HeadForLeaderRobot ----- */

        // ENTRY ACTIONS
        HeadForLeaderRobot.EntryActions.Add(robot.SetDestinationForFellow);
        // STAY ACTIONS
        // ...
        // EXIT ACTIONS
        HeadForLeaderRobot.ExitActions.Add(robot.SetDestinationForNothing);
        // TRANSITIONS
        HeadForLeaderRobot.transitions.Add(LowBattery);
        HeadForLeaderRobot.transitions.Add(CalledByFellowAndWaitForRescue);
        HeadForLeaderRobot.transitions.Add(NotWaitForFellows);
        HeadForLeaderRobot.transitions.Add(FellowCompletedFormation);
        HeadForLeaderRobot.transitions.Add(ReachedLeaderRobot);

        /* ----- HelpFellow ----- */

        // ENTRY ACTIONS
        HelpFellow.EntryActions.Add(robot.StartMovement);
        // STAY ACTIONS
        // ...
        // EXIT ACTIONS
        HelpFellow.ExitActions.Add(robot.SetDestinationForNothing);
        // TRANSITIONS
        HelpFellow.transitions.Add(FellowBreakedFormation);

        /* ----- WaitForFellows ----- */

        // ENTRY ACTIONS
        WaitForFellows.EntryActions.Add(robot.StopMovement);
        // STAY ACTIONS
        WaitForFellows.StayActions.Add(WaitSecondsToRepeatAction(robot.AddRobotOnBoard, 1));
        // EXIT ACTIONS
        WaitForFellows.ExitActions.Add(robot.StartMovement);
        // TRANSITIONS
        WaitForFellows.transitions.Add(ExpiredTimeout);
        WaitForFellows.transitions.Add(CompletedFormation);

        /* ----- WaitForRescue ----- */

        // ENTRY ACTIONS
        // WaitForRescue.EntryActions.Add(robot.SetDestinationForNothing);
        // STAY ACTIONS
        WaitForRescue.StayActions.Add(WaitSecondsToRepeatAction(robot.AddRobotOnBoard, 5));
        // EXIT ACTIONS
        // ...
        // TRANSITIONS
        WaitForRescue.transitions.Add(StartedRescue);

        /* ----- RescueByFellow ----- */

        // ENTRY ACTIONS
        // ...
        // STAY ACTIONS
        // ...
        // EXIT ACTIONS
        // ...
        // TRANSITIONS
        RescueByFellow.transitions.Add(StoppedRescue);
        RescueByFellow.transitions.Add(CompletedRescue);

        /* ----- HeadForPowerStation ----- */

        // ENTRY ACTIONS
        HeadForPowerStation.EntryActions.Add(robot.SetDestinationForPowerStation);
        // STAY ACTIONS
        // ...
        // EXIT ACTIONS
        HeadForPowerStation.ExitActions.Add(robot.SetDestinationForNothing);
        // TRANSITIONS
        HeadForPowerStation.transitions.Add(EmptyBattery);
        HeadForPowerStation.transitions.Add(ReachedPowerStation);

        /* ----- ChargeBattery ----- */

        // ENTRY ACTIONS
        ChargeBattery.EntryActions.Add(robot.StopMovement);
        // STAY ACTIONS
        ChargeBattery.StayActions.Add(robot.station.ChargeBattery);
        // EXIT ACTIONS
        ChargeBattery.ExitActions.Add(robot.StartMovement);
        // TRANSITIONS
        ChargeBattery.transitions.Add(ChargedBattery);

        /* TRANSITIONS */

        /* ----- SightedTrash ----- */

        SightedTrash.target = HeadForTrash;
        // ACTIONS
        SightedTrash.condition = new SightedTrashCondition(robot);

        /* ----- PickedUpTrash ----- */

        PickedUpTrash.target = SearchTrash;
        PickedUpTrash.actions.Add(robot.ClearCurrentTrash);
        PickedUpTrash.condition = new PickedUpTrashCondition(robot);

        /* ----- CalledByFellowAndWaitForRescue ----- */

        CalledByFellowAndWaitForRescue.target = HeadForStoppedRobot;
        CalledByFellowAndWaitForRescue.actions.Add(robot.ClearCurrentTrash);
        CalledByFellowAndWaitForRescue.condition = new CalledByFellowAndWaitForRescueCondition(robot);

        /* ----- CalledByFellowAndWaitForFellows ----- */

        CalledByFellowAndWaitForFellows.target = HeadForLeaderRobot;
        CalledByFellowAndWaitForFellows.actions.Add(robot.ClearCurrentTrash);
        CalledByFellowAndWaitForFellows.condition = new CalledByFellowAndWaitForFellowsCondition(robot);

        /* ----- ReachedTrashAndLightTrash ----- */

        ReachedTrashAndLightTrash.target = HeadForTrashCompactor;
        ReachedTrashAndLightTrash.actions.Add(robot.PickUpLightTrash);
        ReachedTrashAndLightTrash.condition = new ReachedTrashAndLightTrashCondition(robot);

        /* ----- ReachedTrashAndHeavyTrash ----- */

        ReachedTrashAndHeavyTrash.target = WaitForFellows;
        ReachedTrashAndHeavyTrash.actions.Add(robot.PickUpHeavyTrash);
        ReachedTrashAndHeavyTrash.condition = new ReachedTrashAndHeavyTrashCondition(robot);

        /* ----- ReachedTrashCompactorAndiLghtTrash ----- */

        ReachedTrashCompactorAndLightTrash.target = SearchTrash;
        ReachedTrashCompactorAndLightTrash.actions.Add(robot.DisposeLightTrash);
        ReachedTrashCompactorAndLightTrash.condition = new ReachedTrashCompactorAndLightTrashCondition(robot);

        /* ----- ReachedTrashCompactorAndHeavyTrash ----- */

        ReachedTrashCompactorAndHeavyTrash.target = SearchTrash;
        ReachedTrashCompactorAndHeavyTrash.actions.Add(robot.DisposeHeavyTrash);
        ReachedTrashCompactorAndHeavyTrash.actions.Add(robot.BreakFormation);
        ReachedTrashCompactorAndHeavyTrash.actions.Add(robot.ClearFormation);
        ReachedTrashCompactorAndHeavyTrash.condition = new ReachedTrashCompactorAndHeavyTrashCondition(robot);

        /* ----- ExpiredTimeout ----- */

        ExpiredTimeout.target = SearchTrash;
        ExpiredTimeout.actions.Add(robot.BreakFormation);
        ExpiredTimeout.actions.Add(robot.ClearFormation);
        ExpiredTimeout.actions.Add(robot.PutDownPickedUpObject);
        // ExpiredTimeout.actions.Add(robot.ClearCurrentTrash);
        ExpiredTimeout.condition = new ExpiredTimeoutCondition(robot);

        /* ----- ReachedStoppedRobot ----- */

        ReachedStoppedRobot.target = HeadForFellowPowerStation;
        ReachedStoppedRobot.actions.Add(robot.PickUpFellow);
        ReachedStoppedRobot.condition = new ReachedFellowCondition(robot);

        /* ----- ReachedLeaderRobot ----- */

        ReachedLeaderRobot.target = HelpFellow;
        ReachedLeaderRobot.actions.Add(robot.StopMovement);
        ReachedLeaderRobot.actions.Add(robot.TakePosition);
        ReachedLeaderRobot.condition = new ReachedFellowCondition(robot);

        /* ----- ReachedFellowPowerStation ----- */

        ReachedFellowPowerStation.target = SearchTrash;
        ReachedFellowPowerStation.actions.Add(robot.DisposeFellow);
        ReachedFellowPowerStation.condition = new ReachedFellowPowerStationCondition(robot);

        /* ----- NotWaitForFellows ----- */

        NotWaitForFellows.target = SearchTrash;
        NotWaitForFellows.actions.Add(robot.ClearFellowRobot);
        NotWaitForFellows.condition = new NotWaitForFellowsCondition(robot);

        /* ----- FellowCompletedFormation ----- */

        FellowCompletedFormation.target = SearchTrash;
        FellowCompletedFormation.actions.Add(robot.ClearFellowRobot);
        FellowCompletedFormation.condition = new FellowCompletedFormationCondition(robot);

        /* ----- CompletedFormation ----- */

        CompletedFormation.target = HeadForTrashCompactor;
        // ACTIONS
        CompletedFormation.condition = new CompletedFormationCondition(robot);

        /* ----- PickedUpFellow ----- */

        PickedUpFellow.target = SearchTrash;
        PickedUpFellow.actions.Add(robot.ClearFellowRobot);
        PickedUpFellow.condition = new PickedUpFellowCondition(robot);

        /* ----- LowBattery ----- */

        LowBattery.target = HeadForPowerStation;
        LowBattery.actions.Add(robot.PutDownPickedUpObject);
        LowBattery.actions.Add(robot.ClearCurrentTrash);
        LowBattery.actions.Add(robot.ClearFellowRobot);
        LowBattery.condition = new LowBatteryCondition(robot);

        /* ----- ReachedPowerStation ----- */

        ReachedPowerStation.target = ChargeBattery;
        ReachedPowerStation.actions.Add(robot.station.StartChargeBattery);
        ReachedPowerStation.condition = new ReachedPowerStationCondition(robot);

        /* ----- ChargedBattery ----- */

        ChargedBattery.target = SearchTrash;
        ChargedBattery.actions.Add(robot.station.StopChargeBattery);
        ChargedBattery.condition = new ChargedBatteryCondition(robot);

        /* ----- EmptyBattery ----- */

        EmptyBattery.target = WaitForRescue;
        EmptyBattery.actions.Add(StartActionAfterSeconds(robot.StopMovement, 1));
        EmptyBattery.condition = new EmptyBatteryCondition(robot);

        /* ----- StartedRescue ----- */

        StartedRescue.target = RescueByFellow;
        // ACTIONS
        StartedRescue.condition = new StartedRescueCondition(robot);

        /* ----- StoppedRescue ----- */

        StoppedRescue.target = WaitForRescue;
        // ACTIONS
        StoppedRescue.condition = new StoppedRescueCondition(robot);

        /* ----- CompletedRescue ----- */

        CompletedRescue.target = ChargeBattery;
        CompletedRescue.actions.Add(() => robot.PickedUp = false);
        CompletedRescue.actions.Add(() => robot.disposed = false);
        CompletedRescue.condition = new CompletedRescueCondition(robot);

        /* ----- FellowBreakedFormation ----- */

        FellowBreakedFormation.target = SearchTrash;
        FellowBreakedFormation.actions.Add(robot.ClearFellowRobot);
        FellowBreakedFormation.condition = new FellowBreakedFormationCondition(robot);

        /* ----- FellowLowBattery ----- */

        FellowLowBattery.target = SearchTrash;
        FellowLowBattery.actions.Add(robot.BreakFormation);
        FellowLowBattery.actions.Add(robot.ClearFormation);
        FellowLowBattery.actions.Add(robot.PutDownPickedUpObject);
        FellowLowBattery.actions.Add(robot.ClearCurrentTrash);
        FellowLowBattery.condition = new FellowLowBatteryCondition(robot);

        /* ----- LowBatteryAndNotHeavyTrash ----- */

        LowBatteryAndNotHeavyTrash.target = HeadForPowerStation;
        LowBatteryAndNotHeavyTrash.actions.Add(robot.PutDownPickedUpObject);
        LowBatteryAndNotHeavyTrash.actions.Add(robot.ClearCurrentTrash);
        LowBatteryAndNotHeavyTrash.condition = new LowBatteryAndNotHeavyTrashCondition(robot);

        /* ... */

        CurrentState = SearchTrash;
    }

    public void UpdateBeviour()
    {
        Update();
    }

    public Action StartActionAfterSeconds(Action action, float seconds)
    {
        return () => MB.StartCoroutine(AddDelayBeforeAction(action, seconds));
    }

    IEnumerator AddDelayBeforeAction(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }

    Coroutine coroutine;

    public Action WaitSecondsToRepeatAction(Action action, float seconds)
    {
        return () => coroutine = coroutine ?? MB.StartCoroutine(AddDelayAfterAction(action, seconds));
    }

    IEnumerator AddDelayAfterAction(Action action, float seconds)
    {
        action();
        yield return new WaitForSeconds(seconds);
        coroutine = null;
    }
}