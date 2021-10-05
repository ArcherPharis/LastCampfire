using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTransition : MonoBehaviour
{

    [SerializeField] CinemachineVirtualCamera destinationCam;
    [SerializeField] float TransitionTime = 2.0f;
    [SerializeField] CinemachineBrain brain;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            brain.m_DefaultBlend.m_Time = TransitionTime;
            destinationCam.Priority = 1000;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            brain.m_DefaultBlend.m_Time = TransitionTime - 1;
            destinationCam.Priority = 1;
        }
    }
}
