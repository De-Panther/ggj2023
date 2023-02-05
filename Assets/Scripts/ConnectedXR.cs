using UnityEngine;
using WebXR;

public class ConnectedXR : MonoBehaviour
{
  public void OnDisconnect()
  {
    switch (WebXRManager.Instance.XRState)
    {
      case WebXRState.AR:
        WebXRManager.Instance.ToggleAR();
        break;
      case WebXRState.VR:
        WebXRManager.Instance.ToggleVR();
        break;
    }
  }
}
