using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlackBoard : MonoBehaviour
{
    Dictionary<string, List<Robot>> board;

    string[] StatesPriority;
    Dictionary<string, Action<Robot>> StatesAction;

    // public BlackBoard()
    void Start()
    {
        board = new Dictionary<string, List<Robot>>();

        StatesPriority = new string[] { "WaitForRescue", "WaitForFellows" };

        StatesAction = new Dictionary<string, Action<Robot>>
        {
            { "WaitForRescue", ChooseRescuer },
            { "WaitForFellows", GatherFellows }
        };
    }

    void Update()
    {
        UpdateBoard();
    }

    public void AddRobot(Robot robot)
    {
        lock (this)
        {
            string state = robot.behaviour.CurrentState.name;

            if (board.ContainsKey(state))
            {
                if (!board[state].Contains(robot))
                {
                    board[state].Add(robot);
                }
                // otherwise the robot is already on the board for this state
            }
            else
            {
                // we never seen this state so far, so we create a new record
                board.Add(state, new List<Robot> { robot });
            }
        }
    }

    Robot RemoveRobot(string state)
    {
        Robot robot = null;

        lock (this)
        {
            if (board.ContainsKey(state))
            {
                if (board[state].Count > 0)
                {
                    robot = board[state][0];
                    board[state].RemoveAt(0);
                }
            }
        }

        return robot;
    }

    void UpdateBoard()
    {
        Robot robot;

        foreach (string state in StatesPriority)
        {
            if ((robot = RemoveRobot(state)) != null)
            {
                // check if we are still in the state
                if (robot.behaviour.CurrentState.name == state)
                {
                    StatesAction[state](robot);
                    break;
                }
            }
        }
    }

    void ChooseRescuer(Robot StoppedRobot)
    {
        Robot[] RobotList = UnityEngine.Object.FindObjectsOfType<Robot>();

        // 14 ring-shaped regions: [0, 10), [10, 20), ..., [130, 140)
        List<Robot>[] AnnulusList = Enumerable.Repeat(new List<Robot>(), 14).ToArray();

        foreach (Robot robot in RobotList)
        {
            if (ReferenceEquals(robot, StoppedRobot))
            {
                // skip ourselves
                continue;
            }

            if (robot.LowBattery)
            {
                // skip robots with low battery
                continue;
            }

            string[] states = new string[] { "SearchTrash", "HeadForTrash", "HeadForLeaderFellow" };

            string RobotState = robot.behaviour.CurrentState.name;

            if (!states.Contains(RobotState))
            {
                // skip robots which current state is not in states
                continue;
            }

            float distance = (robot.transform.position - StoppedRobot.transform.position).magnitude;

            int i = (int)(distance / 10);

            AnnulusList[i].Add(robot);
        }

        Robot RescueRobot = null;

        foreach (List<Robot> annulus in AnnulusList)
        {
            if (annulus.Count > 0)
            {
                // robot with max current battery in this annulus
                RescueRobot = annulus.Aggregate((Robot1, Robot2) => Robot1.CurrentBattery > Robot2.CurrentBattery ? Robot1 : Robot2);
                break;
            }
        }

        if (RescueRobot != null)
        {
            RescueRobot.FellowRobot = StoppedRobot;
        }
    }

    void GatherFellows(Robot LeaderRobot)
    {
        Robot[] RobotList = UnityEngine.Object.FindObjectsOfType<Robot>();

        foreach (Robot robot in RobotList)
        {
            if (ReferenceEquals(robot, LeaderRobot))
            {
                // skip ourselves
                continue;
            }

            if (robot.LowBattery)
            {
                // skip robots with low battery
                continue;
            }

            string[] states = new string[] { "SearchTrash", "HeadForTrash" };

            if (!states.Contains(robot.behaviour.CurrentState.name))
            {
                // skip robots which current state is not in states
                continue;
            }

            if (robot.FellowRobot == null)
            {
                robot.FellowRobot = LeaderRobot;
            }
        }
    }
}