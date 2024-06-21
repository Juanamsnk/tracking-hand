using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WatchController : InterfaceController
{
    private WristInfo _wristInfo;

    protected override void InitializeItem()
    {
        Vector3 invertScale = new Vector3(-items[1].transform.localScale.x, -items[1].transform.localScale.y, -items[1].transform.localScale.z);
        items[1].transform.localScale = invertScale;

        lastPosition = item.transform.position;
        lastDepth = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.depth_estimation;
    }

    protected override bool IsGestureActive()
    {
        return gestureInfo.mano_class != ManoClass.NO_HAND;
    }

    protected override void ShowItem()
    {
        _wristInfo = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.wristInfo;

        UpdateItemPosition();

        if (gestureInfo.hand_side == HandSide.Palmside)
        {
            ShowItemSide(false);
        }
        else
        {
            ShowItemSide(true);
        }
    }

    protected override void UpdateItemPosition()
    {
        Vector3 leftPoint = ManoUtils.Instance.CalculateNewPositionDepth(new Vector3(_wristInfo.left_point.x, _wristInfo.left_point.y, 0), lastDepth);
        Vector3 rightPoint = ManoUtils.Instance.CalculateNewPositionDepth(new Vector3(_wristInfo.right_point.x, _wristInfo.right_point.y, 0), lastDepth);

        var depthEstimation = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.depth_estimation;

        if (Mathf.Abs(depthEstimation - lastDepth) > depthThreshold)
        {
            leftPoint = ManoUtils.Instance.CalculateNewPositionDepth(new Vector3(_wristInfo.left_point.x, _wristInfo.left_point.y, 0), depthEstimation);
            rightPoint = ManoUtils.Instance.CalculateNewPositionDepth(new Vector3(_wristInfo.right_point.x, _wristInfo.right_point.y, 0), depthEstimation);

            lastDepth = depthEstimation;
        }

        Vector3 itemPosition = Vector3.Lerp(leftPoint, rightPoint, 0.5f);

        if (Vector3.Distance(itemPosition, lastPosition) > moveThreshold)
        {
            smoothedPosition = Vector3.Lerp(smoothedPosition, itemPosition, smoothingFactor);
            item.transform.position = smoothedPosition;
            item.transform.LookAt(leftPoint);

            lastPosition = item.transform.position;
        }
    }

    private void ShowItemSide(bool isFront)
    {
        items[0].SetActive(isFront);
        items[1].SetActive(!isFront);
    }

    public void Activate()
    {
        enabled = true;
        item.SetActive(true);
    }

    public void Deactivate()
    {
        enabled = false;
        item.SetActive(false);
    }
}

