using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class InterfaceController : MonoBehaviour
{
    public GameObject item;
    public GameObject[] items = new GameObject[2];

    [Range(0, 1)]
    public float smoothingFactor = 0.5f;
    protected Vector3 smoothedPosition;

    public float moveThreshold = 0.05f; // Umbral de movimiento
    public float depthThreshold = 0.05f; // Umbral de movimiento

    protected Vector3 lastPosition;
    protected float lastDepth;
    protected float lastSize;

    protected GestureInfo gestureInfo;

    protected virtual void Start()
    {
        InitializeItem();
    }

    protected virtual void Update()
    {
        ManomotionManager.Instance.ShouldRunFingerInfo(true);
        ManomotionManager.Instance.ShouldCalculateGestures(true);
        ManomotionManager.Instance.ShouldRunWristInfo(true);

        smoothedPosition = item.transform.position;
        gestureInfo = ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info;

        if (IsGestureActive())
        {
            ShowItem();
        }
        else
        {
            item.transform.position = -Vector3.one;
        }
    }

    protected abstract void InitializeItem();
    protected abstract bool IsGestureActive();
    protected abstract void ShowItem();
    protected abstract void UpdateItemPosition();

    public interface IController
    {
        void Activate();
        void Deactivate();
    }
}
