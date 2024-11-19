using UnityEngine;

public class CameraController : MonoBehaviour
{
    // percentage of rotation to target to apply each frame
    private const float CameraRotationRubber = 0.1f;

    private Army army1;
    private Army army2;
    private Vector3 cachedPosition;
    private Transform cachedTransform;

    private void Start()
    {
        cachedTransform = transform;
        cachedPosition = cachedTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 mainCenter = Utils.GetCenter(army1.GetUnits()) + Utils.GetCenter(army2.GetUnits());
        mainCenter *= 0.5f;

        Vector3 forwardTarget = (mainCenter - cachedPosition).normalized;

        // TODO comment on debrief: this is frame rate dependent, and also doesn't use spherical interpolation
        cachedTransform.forward += (forwardTarget - cachedTransform.forward) * CameraRotationRubber;
    }

    public void SetArmies(Army army1, Army army2)
    {
        this.army1 = army1;
        this.army2 = army2;
    }
}