using UnityEngine;

public class PowerStation : MonoBehaviour
{
    [HideInInspector] public Robot robot;
    
    public void StartChargeBattery()
    {
        robot.transform.parent = transform;
        robot.transform.localPosition = new Vector3(0, 1.25f, 0);
        robot.transform.localRotation = Quaternion.identity;
    }

    public float ChargeRate = 100;

    public void ChargeBattery()
    {
        robot.CurrentBattery += ChargeRate * Time.deltaTime;
        robot.CurrentBattery = Mathf.Min(robot.CurrentBattery, robot.FullBattery);
    }

    public void StopChargeBattery()
    {
        // check if we hit something before put down the robot

        robot.transform.localPosition = new Vector3(0, 0, 1);
        robot.transform.parent = null;
    }
}
