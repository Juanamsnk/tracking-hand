using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerUI : MonoBehaviour
{
    public RingController ringController;
    public WatchController watchController;

    public Button ringButton;
    public Button watchButton;

    void Start()
    {
        ringButton.onClick.AddListener(ActivateRingController);
        watchButton.onClick.AddListener(ActivateWatchController);

        ringController.Deactivate();
        watchController.Deactivate();
    }

    void ActivateRingController()
    {
        ringController.Activate();
        watchController.Deactivate();
    }

    void ActivateWatchController()
    {
        watchController.Activate();
        ringController.Deactivate();
    }
}