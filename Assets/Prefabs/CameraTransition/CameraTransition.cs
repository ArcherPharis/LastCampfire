using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTransition : MonoBehaviour
{

    [SerializeField] CinemachineVirtualCamera destinationCam;
    [SerializeField] float TransitionTime = 2.0f;
    [SerializeField] CinemachineBrain brain;

    CameraTriggering ct;

    private void Start()
    {
        ct = GetComponent<CameraTriggering>();
    }

    void MoveCam()
    {
        if(ct.triggered == true)
        {
            brain.m_DefaultBlend.m_Time = TransitionTime;
            destinationCam.Priority = 1000;
        }
        else
        {
            destinationCam.Priority = 1;
        }
    }

    private void Update()
    {
        MoveCam();
    }



}
