using System;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    public RobotController controller;

    enum BatteryState
    {
        Full,
        ThreeQuarters,
        Half,
        OneQuarter,
        Empty
    }

    public Texture2D[] BatteryListYellow = new Texture2D[5];
    public Texture2D[] BatteryListMagenta = new Texture2D[5];
    public Texture2D[] BatteryListCyan = new Texture2D[5];
    public Texture2D TrashBin;
    public Texture2D LabelBackground;

    [Range(0, 1)] public float ScaleFactor = 0.1f;

    private int CameraX;
    private int CameraY;
    private float BatteryWidth;
    private float BatteryHeight;
    private float TrashBinWidth;
    private float TrashBinHeight;
    public float LabelWidth = 230;
    public float LabelHeight = 80;

    private int PaddingNorth = 10;
    private int PaddingSouth = 10;
    private int PaddingWest = 10;
    private int PaddingEast = 10;

    private GUIStyle TextStyleWhite;
    private GUIStyle TextStyleYellow;
    private GUIStyle TextStyleMagenta;
    private GUIStyle TextStyleCyan;

    void Start()
    {
        CameraX = GetComponent<Camera>().pixelWidth;
        CameraY = GetComponent<Camera>().pixelHeight;

        BatteryWidth = BatteryListYellow[0].width * ScaleFactor;
        BatteryHeight = BatteryListYellow[0].height * ScaleFactor;
        TrashBinWidth = TrashBin.width * ScaleFactor;
        TrashBinHeight = TrashBin.height * ScaleFactor;

        Font font = Font.CreateDynamicFontFromOSFont("Consolas", 10);
        TextStyleWhite = new GUIStyle { alignment = TextAnchor.MiddleLeft, font = font, normal = new GUIStyleState { background = LabelBackground }, padding = new RectOffset(5, 5, 0, 0) };
        TextStyleWhite.normal.textColor = Color.white;
        TextStyleYellow = new GUIStyle(TextStyleWhite);
        TextStyleYellow.normal.textColor = Color.yellow;
        TextStyleMagenta = new GUIStyle(TextStyleWhite);
        TextStyleMagenta.normal.textColor = Color.magenta;
        TextStyleCyan = new GUIStyle(TextStyleWhite);
        TextStyleCyan.normal.textColor = Color.cyan;
    }

    Func<Robot, string> RobotData = robot => string.Format("CHARGE:{0,6} %\nTRASH:{1,7}\nCURRENT STATE:\n{2}\nLAST TRANSITION:\n{3}", (int)((robot.CurrentBattery / robot.FullBattery) * 100), robot.DisposedTrash, robot.behaviour.CurrentState.name, robot.behaviour.LastTransition == null ? "" : robot.behaviour.LastTransition.name);
    Func<TrashCompactor, string> CompactorData = compactor => string.Format("TOTAL TRASH:{0,7}\n{3}\nLIGHT TRASH:{1,7}\nHEAVY TRASH:{2,7}", compactor.DisposedLightTrash + compactor.DisposedHeavyTrash, compactor.DisposedLightTrash, compactor.DisposedHeavyTrash, new String('-', 19));

    void OnGUI()
    {
        if (!controller.enabled)
        {
            return;
        }

        float x, y;

        // north west
        x = PaddingWest;
        y = PaddingNorth;
        GUI.Label(new Rect(x, y, LabelWidth, LabelHeight), CompactorData(controller.compactor), TextStyleWhite);
        x = PaddingWest + LabelWidth - TrashBinWidth - PaddingEast;
        y = PaddingNorth * 2;
        GUI.Label(new Rect(x, y, TrashBinWidth, TrashBinHeight), TrashBin);

        // north east
        Texture2D BatteryYellow = GetBattery(controller.RobotYellow);

        if (BatteryYellow != TrashBin)
        {
            x = CameraX - LabelWidth - PaddingEast;
            y = PaddingNorth;
            GUI.Label(new Rect(x, y, LabelWidth, LabelHeight), RobotData(controller.RobotYellow), TextStyleYellow);
            x = CameraX - BatteryWidth - PaddingEast * 2;
            y = PaddingNorth;
            GUI.Label(new Rect(x, y, BatteryWidth, BatteryHeight), BatteryYellow);
        }

        // south east
        Texture2D BatteryMagenta = GetBattery(controller.RobotMagenta);

        if (BatteryMagenta != TrashBin)
        {
            x = CameraX - LabelWidth - PaddingEast;
            y = CameraY - LabelHeight - PaddingSouth;
            GUI.Label(new Rect(x, y, LabelWidth, LabelHeight), RobotData(controller.RobotMagenta), TextStyleMagenta);
            x = CameraX - BatteryWidth - PaddingEast * 2;
            y = CameraY - LabelHeight - PaddingSouth;
            GUI.Label(new Rect(x, y, BatteryWidth, BatteryHeight), BatteryMagenta);
        }

        // south west
        Texture2D BatteryCyan = GetBattery(controller.RobotCyan);

        if (BatteryCyan != TrashBin)
        {
            x = PaddingWest;
            y = CameraY - LabelHeight - PaddingSouth;
            GUI.Label(new Rect(x, y, LabelWidth, LabelHeight), RobotData(controller.RobotCyan), TextStyleCyan);
            x = PaddingWest + LabelWidth - BatteryWidth - PaddingEast;
            y = CameraY - LabelHeight - PaddingSouth;
            GUI.Label(new Rect(x, y, BatteryWidth, BatteryHeight), BatteryCyan);
        }
    }

    Texture2D GetBattery(Robot robot)
    {
        if (robot == null)
        {
            return TrashBin;
        }

        Texture2D[] BatteryList;

        switch (robot.name)
        {
            case "RobotYellow":
                BatteryList = BatteryListYellow;
                break;
            case "RobotMagenta":
                BatteryList = BatteryListMagenta;
                break;
            case "RobotCyan":
                BatteryList = BatteryListCyan;
                break;
            default:
                BatteryList = null;
                break;
        }

        if (robot.CurrentBattery > robot.FullBattery * 7 / 8)
        {
            return BatteryList[(int)BatteryState.Full];
        }
        else if (robot.FullBattery * 5 / 8 < robot.CurrentBattery && robot.CurrentBattery <= robot.FullBattery * 7 / 8)
        {
            return BatteryList[(int)BatteryState.ThreeQuarters];
        }
        else if (robot.FullBattery * 3 / 8 < robot.CurrentBattery && robot.CurrentBattery <= robot.FullBattery * 5 / 8)
        {
            return BatteryList[(int)BatteryState.Half];
        }
        else if (robot.FullBattery * 1 / 8 < robot.CurrentBattery && robot.CurrentBattery <= robot.FullBattery * 3 / 8)
        {
            return BatteryList[(int)BatteryState.OneQuarter];
        }
        else if (robot.CurrentBattery < robot.FullBattery * 1 / 8)
        {
            return BatteryList[(int)BatteryState.Empty];
        }
        else
        {
            return BatteryList[(int)BatteryState.Empty];
        }
    }
}