using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
  public Transform target;
  public bool slowly = false;

  void Start ()
  {
    target = GameObject.Find( "Player" ).transform;
  }

  void LateUpdate ()
  {
    if (slowly)
    {
      transform.position = Vector3.Lerp( transform.position, target.position, 1 * Time.deltaTime / 60);
      transform.eulerAngles = new Vector3( 0, 0, 0 );
    }
    else
    {
      transform.position = new Vector3( target.position.x, target.position.y, -10 );
      transform.eulerAngles = new Vector3( 0, 0, 0 );
    }
  }
}