using System.Linq;

public abstract class Condition
{
    public abstract bool Test();
}

class NotCondition : Condition
{
    private Condition condition;

    public NotCondition(Condition condition)
    {
        this.condition = condition;
    }

    public override bool Test()
    {
        return !condition.Test();
    }
}

class AllCondition : Condition
{
    private Condition[] conditions;

    public AllCondition(Condition[] conditions)
    {
        this.conditions = conditions;
    }

    public override bool Test()
    {
        return conditions.All(condition => condition.Test());
    }
}

class AnyCondition : Condition
{
    private Condition[] conditions;

    public AnyCondition(Condition[] conditions)
    {
        this.conditions = conditions;
    }

    public override bool Test()
    {
        return conditions.Any(condition => condition.Test());
    }
}