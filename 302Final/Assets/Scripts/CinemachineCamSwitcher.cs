using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCamSwitcher : MonoBehaviour
{
    public CinemachineCamera CAM1;
    public CinemachineCamera CAM2;
    public CinemachineCamera IsoCam;

    public void SwitchCamera(int camNum)
    {
        if (camNum == 1)
        {
            CAM1.Priority = 10;
            CAM2.Priority = 5;
            IsoCam.Priority = 5;
        }
        else if (camNum == 2)
        {
            CAM2.Priority = 10;
            IsoCam.Priority = 5;
            CAM1.Priority = 5;
        }
        else if (camNum == 3)
        {
            IsoCam.Priority = 10;
            CAM1.Priority = 5;
            CAM2.Priority = 5;
        }
    }
}
