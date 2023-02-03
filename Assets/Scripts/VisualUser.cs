using UnityEngine;
using WebXR;

public class VisualUser : MonoBehaviour
{
  public Transform controllerLeft;
  public Transform controllerRight;
  public Transform targetHead;
  public WebXRController targetControllerLeft;
  public WebXRController targetControllerRight;

  public bool controllerLeftEnable;
  public bool controllerRightEnable;

  private Transform targetTransformLeft;
  private Transform targetTransformRight;

  private void Start()
  {
    targetTransformLeft = targetControllerLeft?.transform;
    targetTransformRight = targetControllerRight?.transform;
  }

  private void Update()
  {
    transform.SetPositionAndRotation(targetHead.position, targetHead.rotation);
    controllerLeftEnable = targetControllerLeft.isControllerActive || targetControllerLeft.isHandActive;
    if (controllerLeftEnable)
    {
      controllerLeft.SetPositionAndRotation(targetTransformLeft.position, targetTransformLeft.rotation);
    }
    controllerRightEnable = targetControllerRight.isControllerActive || targetControllerRight.isHandActive;
    if (controllerRightEnable)
    {
      controllerRight.SetPositionAndRotation(targetTransformRight.position, targetTransformRight.rotation);
    }
  }
}
