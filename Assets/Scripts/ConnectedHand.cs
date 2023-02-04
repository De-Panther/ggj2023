using UnityEngine;

public class ConnectedHand : MonoBehaviour
{
  public FixedJoint joint;
  private Rigidbody lastConnectedBody;

  void Update()
  {
    if (lastConnectedBody != joint.connectedBody)
    {
      lastConnectedBody = joint.connectedBody;
      ConnectedCube connectedCube = lastConnectedBody?.GetComponent<ConnectedCube>();
      if (connectedCube != null)
      {
        connectedCube.TryRequestAuthority();
      }
    }
  }
}
