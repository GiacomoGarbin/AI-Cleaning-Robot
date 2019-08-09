using UnityEngine;

class SightedTrashCondition : Condition
{
    private Robot robot;

    public SightedTrashCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        Trash[] TrashList = Object.FindObjectsOfType<Trash>();

        foreach (Trash trash in TrashList)
        {
            if (trash.PickedUp || trash.disposed)
            {
                continue;
            }

            Vector3 direction = trash.transform.position - robot.transform.position;
            float angle = Vector3.Angle(direction, robot.transform.forward);

            if (angle < robot.FieldOfViewAngle && direction.magnitude < robot.FieldOfViewRange)
            {
                robot.CurrentTrash = trash;
                return true;
            }
        }

        return false;
    }
}

class PickedUpTrashCondition : Condition
{
    private Robot robot;

    public PickedUpTrashCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.CurrentTrash.PickedUp == true;
    }
}

class CalledByFellowCondition : Condition
{
    private Robot robot;

    public CalledByFellowCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.FellowRobot != null;
    }
}

class FellowStateCondition : Condition
{
    Robot robot;
    string state;

    public FellowStateCondition(Robot robot, string state)
    {
        this.robot = robot;
        this.state = state;
    }

    public override bool Test()
    {
        return robot.FellowRobot.behaviour.CurrentState.name == state;
    }
}

class CalledByFellowAndWaitForRescueCondition : AllCondition
{
    public CalledByFellowAndWaitForRescueCondition(Robot robot) : base(new Condition[] { new CalledByFellowCondition(robot), new FellowStateCondition(robot, "WaitForRescue") }) { }
}

class CalledByFellowAndWaitForFellowsCondition : AllCondition
{
    public CalledByFellowAndWaitForFellowsCondition(Robot robot) : base(new Condition[] { new CalledByFellowCondition(robot), new FellowStateCondition(robot, "WaitForFellows") }) { }
}

class PickedUpFellowCondition : Condition
{
    private Robot robot;

    public PickedUpFellowCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.FellowRobot.PickedUp == true;
    }
}

class ReachedFellowCondition : Condition
{
    private Robot robot;

    public ReachedFellowCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.ReachedTarget(robot.FellowRobot.gameObject);
    }
}

class NotWaitForFellowsCondition : NotCondition
{
    // public NotWaitForFellowsCondition(Robot robot) : base(new WaitForFellowsCondition(robot)) { }
    public NotWaitForFellowsCondition(Robot robot) : base(new FellowStateCondition(robot, "WaitForFellows")) { }
}

class ReachedFellowAndWaitForFellowsCondition : AllCondition
{
    // public ReachedFellowAndWaitForFellowsCondition(Robot robot) : base(new Condition[] { new ReachedFellowCondition(robot), new WaitForFellowsCondition(robot) }) { }
    public ReachedFellowAndWaitForFellowsCondition(Robot robot) : base(new Condition[] { new ReachedFellowCondition(robot), new FellowStateCondition(robot, "WaitForFellows") }) { }
}

class ReachedTrashCondition : Condition
{
    private Robot robot;

    public ReachedTrashCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.ReachedTarget(robot.CurrentTrash.gameObject);
    }
}

class LightTrashCondition : Condition
{
    private Robot robot;

    public LightTrashCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        if (robot.PickedUpObject != null)
        {
            Trash trash = robot.PickedUpObject.GetComponent<Trash>();
            return trash != null && trash.weight == TrashWeight.Light;
        }
        else
        {
            return robot.CurrentTrash.weight == TrashWeight.Light;
        }
    }
}

class HeavyTrashCondition : Condition
{
    private Robot robot;

    public HeavyTrashCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        if (robot.PickedUpObject != null)
        {
            Trash trash = robot.PickedUpObject.GetComponent<Trash>();
            return trash != null && trash.weight == TrashWeight.Heavy;
        }
        else
        {
            return robot.CurrentTrash.weight == TrashWeight.Heavy;
        }
    }
}

class FellowRobotCondition : Condition
{
    private Robot robot;

    public FellowRobotCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.PickedUpObject.GetComponent<Robot>() != null;
    }
}

class ReachedTrashAndLightTrashCondition : AllCondition
{
    public ReachedTrashAndLightTrashCondition(Robot robot) : base(new Condition[] { new ReachedTrashCondition(robot), new LightTrashCondition(robot) }) { }
}

class ReachedTrashAndHeavyTrashCondition : AllCondition
{
    public ReachedTrashAndHeavyTrashCondition(Robot robot) : base(new Condition[] { new ReachedTrashCondition(robot), new HeavyTrashCondition(robot) }) { }
}

class ExpiredTimeoutCondition : Condition
{
    private Robot robot;

    public ExpiredTimeoutCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        // return robot.TimeoutCoroutine == null;
        return robot.formation.ExpiredTimeout;
    }
}

class FellowBreakedFormationCondition : Condition
{
    private Robot robot;

    public FellowBreakedFormationCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        // return robot.FellowRobot.formation == null || !robot.FellowRobot.formation.CompletedFormation;
        return robot.FellowRobot.formation == null;
    }
}

class CompletedFormationCondition : Condition
{
    private Robot robot;

    public CompletedFormationCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.formation.CompletedFormation;
    }
}

class FellowCompletedFormationCondition : Condition
{
    private Robot robot;

    public FellowCompletedFormationCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.FellowRobot.formation.CompletedFormation;
    }
}

class ReachedTrashCompactorCondition : Condition
{
    private Robot robot;

    public ReachedTrashCompactorCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.ReachedTarget(robot.compactor.gameObject);
    }
}

class ReachedTrashCompactorAndLightTrashCondition : AllCondition
{
    public ReachedTrashCompactorAndLightTrashCondition(Robot robot) : base(new Condition[] { new ReachedTrashCompactorCondition(robot), new LightTrashCondition(robot) }) { }
}

class ReachedTrashCompactorAndHeavyTrashCondition : AllCondition
{
    public ReachedTrashCompactorAndHeavyTrashCondition(Robot robot) : base(new Condition[] { new ReachedTrashCompactorCondition(robot), new HeavyTrashCondition(robot) }) { }
}

class LowBatteryCondition : Condition
{
    private Robot robot;

    public LowBatteryCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        // return robot.CurrentBattery < robot.FullBattery * robot.LowBatteryLevel;
        return robot.LowBattery;
    }
}

class NotHeavyTrashCondition : NotCondition
{
    public NotHeavyTrashCondition(Robot robot) : base(new HeavyTrashCondition(robot)) { }
}

class LowBatteryAndNotHeavyTrashCondition : AllCondition
{
    public LowBatteryAndNotHeavyTrashCondition(Robot robot) : base(new Condition[] { new LowBatteryCondition(robot), new NotHeavyTrashCondition(robot) }) { }
}

class FellowLowBatteryCondition : Condition
{
    private Robot robot;

    public FellowLowBatteryCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.formation != null && robot.formation.FellowLowBattery;
    }
}

class EmptyBatteryCondition : Condition
{
    private Robot robot;

    public EmptyBatteryCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.CurrentBattery == 0;
    }
}

class ReachedPowerStationCondition : Condition
{
    private Robot robot;

    public ReachedPowerStationCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.ReachedTarget(robot.station.gameObject);
    }
}

class ReachedFellowPowerStationCondition : Condition
{
    private Robot robot;

    public ReachedFellowPowerStationCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.ReachedTarget(robot.FellowRobot.station.gameObject);
    }
}

class StartedRescueCondition : Condition
{
    private Robot robot;

    public StartedRescueCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.PickedUp == true;
    }
}

class StoppedRescueCondition : Condition
{
    private Robot robot;

    public StoppedRescueCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.PickedUp == false;
    }
}

class CompletedRescueCondition : Condition
{
    private Robot robot;

    public CompletedRescueCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.disposed == true;
    }
}

class ChargedBatteryCondition : Condition
{
    private Robot robot;

    public ChargedBatteryCondition(Robot robot)
    {
        this.robot = robot;
    }

    public override bool Test()
    {
        return robot.CurrentBattery == robot.FullBattery;
    }
}