using UnityEngine;
using WebXR;
using Coherence.Toolkit;

public class VisualUser : MonoBehaviour
{
  public CoherenceSync coherenceSync;
  public GameObject head;
  public Transform controllerLeft;
  public Transform controllerRight;
  public Transform targetHead;
  public WebXRController targetControllerLeft;
  public WebXRController targetControllerRight;

  public bool controllerLeftEnable;
  public bool controllerRightEnable;

  public Vector3 controllerLeftPosition;
  public Quaternion controllerLeftRotation;
  public Vector3 controllerRightPosition;
  public Quaternion controllerRightRotation;

  private Transform targetTransformLeft;
  private Transform targetTransformRight;
  private GameObject controllerLeftGameObject;
  private GameObject controllerRightGameObject;

  private void Start()
  {
    controllerLeftGameObject = controllerLeft.gameObject;
    controllerRightGameObject = controllerRight.gameObject;
    if (coherenceSync.HasStateAuthority)
    {
      targetTransformLeft = targetControllerLeft?.transform;
      targetTransformRight = targetControllerRight?.transform;
      head.SetActive(false);
    }
    else
    {
      head.SetActive(true);
    }
  }

  private void Update()
  {
    if (coherenceSync.HasStateAuthority)
    {
      UpdateSelf();
    }
    else
    {
      UpdateFromNetwork();
    }
  }

  private void UpdateSelf()
  {
    transform.SetPositionAndRotation(targetHead.position, targetHead.rotation);
    controllerLeftEnable = targetControllerLeft.isControllerActive || targetControllerLeft.isHandActive;
    if (controllerLeftEnable)
    {
      if (!controllerLeftGameObject.activeSelf)
      {
        controllerLeftGameObject.SetActive(true);
      }
      controllerLeftPosition = targetTransformLeft.position;
      controllerLeftRotation = targetTransformLeft.rotation;
      controllerLeft.SetPositionAndRotation(controllerLeftPosition, controllerLeftRotation);
    }
    else if (controllerLeftGameObject.activeSelf)
    {
      controllerLeftGameObject.SetActive(false);
    }
    controllerRightEnable = targetControllerRight.isControllerActive || targetControllerRight.isHandActive;
    if (controllerRightEnable)
    {
      if (!controllerRightGameObject.activeSelf)
      {
        controllerRightGameObject.SetActive(true);
      }
      controllerRightPosition = targetTransformRight.position;
      controllerRightRotation = targetTransformRight.rotation;
      controllerRight.SetPositionAndRotation(controllerRightPosition, controllerRightRotation);
    }
    else if (controllerRightGameObject.activeSelf)
    {
      controllerRightGameObject.SetActive(false);
    }
  }

  private void UpdateFromNetwork()
  {
    if (controllerLeftEnable)
    {
      if (!controllerLeftGameObject.activeSelf)
      {
        controllerLeftGameObject.SetActive(true);
      }
      controllerLeft.SetPositionAndRotation(controllerLeftPosition, controllerLeftRotation);
    }
    else if (controllerLeftGameObject.activeSelf)
    {
      controllerLeftGameObject.SetActive(false);
    }
    if (controllerRightEnable)
    {
      if (!controllerRightGameObject.activeSelf)
      {
        controllerRightGameObject.SetActive(true);
      }
      controllerRight.SetPositionAndRotation(controllerRightPosition, controllerRightRotation);
    }
    else if (controllerRightGameObject.activeSelf)
    {
      controllerRightGameObject.SetActive(false);
    }
  }
}
