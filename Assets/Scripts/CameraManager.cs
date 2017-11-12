using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
  Camera cam;
  public bool reduceBy50Percx2Scale = false;

  void Awake ()
  {
    cam = GetComponent<Camera>();
  }

  void Start ()
  {
    cam.orthographicSize = Screen.height / 2f;

    if (reduceBy50Percx2Scale)
    {
      cam.orthographicSize = cam.orthographicSize - ( cam.orthographicSize * 50.0f / 100.0f );
    }
  }

  //void Update ()
  //{
  //  if(Screen.width >= 1366)
  //  {
  //    if(Input.GetKeyDown(KeyCode.S))
  //    {
  //      cam.orthographicSize = ( Screen.height / 2f ) - ( ( 25f / 100f ) * Screen.height / 2f );
  //    }
  //  }
  //}
}
