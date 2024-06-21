using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingController : InterfaceController
{
    private FingerInfo _fingerInfo;

    protected override void InitializeItem()
    {
        ManomotionManager.Instance.ToggleFingerInfoFinger(4);

        Vector3 invertScale = new Vector3(-items[1].transform.localScale.x, -items[1].transform.localScale.y, items[1].transform.localScale.z);
        items[1].transform.localScale = invertScale;
        lastDepth = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.depth_estimation;
        lastSize = Vector3.Distance(_fingerInfo.left_point, _fingerInfo.right_point);
    }

    protected override bool IsGestureActive()
    {
        return gestureInfo.mano_class == ManoClass.GRAB_GESTURE;
    }

    protected override void ShowItem()
    {
        ManomotionManager.Instance.ShouldCalculateSkeleton3D(true);
        _fingerInfo = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.fingerInfo;

        UpdateItemPosition();

        if (gestureInfo.hand_side == HandSide.Palmside)
        {
            ShowItemPart(false);
        }
        else
        {
            ShowItemPart(true);
        }
    }

    protected override void UpdateItemPosition()
    {
        Vector3 leftPoint = ManoUtils.Instance.CalculateNewPositionDepth(new Vector3(_fingerInfo.left_point.x, _fingerInfo.left_point.y, 0), lastDepth);
        Vector3 rightPoint = ManoUtils.Instance.CalculateNewPositionDepth(new Vector3(_fingerInfo.right_point.x, _fingerInfo.right_point.y, 0), lastDepth);

        float distanceBetweenPoints = Vector3.Distance(_fingerInfo.left_point, _fingerInfo.right_point);
        item.transform.localScale = new Vector3(lastSize, lastSize, lastSize);

        var depthEstimation = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.depth_estimation;
        if (Mathf.Abs(depthEstimation - lastDepth) > depthThreshold)
        {
            leftPoint = ManoUtils.Instance.CalculateNewPositionDepth(new Vector3(_fingerInfo.left_point.x, _fingerInfo.left_point.y, 0), depthEstimation);
            rightPoint = ManoUtils.Instance.CalculateNewPositionDepth(new Vector3(_fingerInfo.right_point.x, _fingerInfo.right_point.y, 0), depthEstimation);

            lastDepth = depthEstimation;
            item.transform.localScale = new Vector3(distanceBetweenPoints, distanceBetweenPoints, distanceBetweenPoints);
            lastSize = distanceBetweenPoints;
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

    private void ShowItemPart(bool isFront)
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