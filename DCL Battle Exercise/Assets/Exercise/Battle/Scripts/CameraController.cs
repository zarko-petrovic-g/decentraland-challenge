using UnityEngine;

public class CameraController : MonoBehaviour
{
    // percentage of rotation to target to apply each frame
    [SerializeField]
    private float rotationRubber = 0.1f;

    [SerializeField]
    private Battle battle;

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
        Vector3 mainCenter = (battle.Army1.Center + battle.Army2.Center) * 0.5f;

        Vector3 forwardTarget = (mainCenter - cachedPosition).normalized;

        // TODO this is frame rate dependent, and also doesn't use spherical interpolation
        cachedTransform.forward += (forwardTarget - cachedTransform.forward) * rotationRubber;
    }
}