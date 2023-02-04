using UnityEngine;
using Coherence;
using Coherence.Toolkit;
using WebXR.Interactions;

public class ConnectedCube : MonoBehaviour
{
  public CoherenceSync coherenceSync;
  public MouseDragObject mouseDragObject;

  void Start()
  {
    if (!coherenceSync.HasStateAuthority)
    {
      mouseDragObject.enabled = false;
    }
  }

  private void OnEnable()
  {
    coherenceSync.OnStateAuthority.AddListener(OnStateAuthority);
    coherenceSync.OnStateRemote.AddListener(OnStateRemote);
  }

  private void OnDisable()
  {
    coherenceSync.OnStateAuthority.RemoveListener(OnStateAuthority);
    coherenceSync.OnStateRemote.RemoveListener(OnStateRemote);
  }

  private void OnStateAuthority()
  {
    mouseDragObject.enabled = true;
  }

  private void OnStateRemote()
  {
    mouseDragObject.enabled = false;
  }

  void OnMouseDown()
  {
    TryRequestAuthority();
  }

  public void TryRequestAuthority()
  {
    if (!coherenceSync.HasStateAuthority)
    {
      mouseDragObject.enabled = true;
      if (coherenceSync.isOrphaned)
      {
        coherenceSync.Adopt();
      }
      else
      {
        coherenceSync.RequestAuthority(AuthorityType.Full);
      }
    }
  }
}
