using UnityEngine;
using System.Collections;

public class Interactor : MonoBehaviour
{
  Vector3 mousePos;
  Vector3 rayPos;

  GameObject ReturnObjectFromRay (Vector3 _rayPos)
  {
    if (Physics2D.OverlapPoint( _rayPos ) != null)
    {
      return Physics2D.OverlapPoint( _rayPos ).gameObject;
    }
    else
    {
      Debug.Log( "No GameObject selected. Returned null" );
      return null;
    }
  }

  [System.NonSerialized]
  public GameObject returnedObject;

  Camera cam;

  void Awake ()
  {
    returnedObject = new GameObject();
    returnedObject.name = null;
    cam = GetComponent<Camera>();
  }

  void Update ()
  {
    mousePos = cam.ScreenToWorldPoint( Input.mousePosition );
    rayPos = new Vector3( mousePos.x, mousePos.y, mousePos.z );

    if (Input.GetMouseButtonDown( 0 ))
    {
      returnedObject = ReturnObjectFromRay( rayPos );
    }
  }
}