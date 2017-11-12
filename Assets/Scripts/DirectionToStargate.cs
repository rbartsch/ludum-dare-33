using UnityEngine;
using System.Collections;

public class DirectionToStargate : MonoBehaviour
{
  public Transform player;
  public Transform target;

  void Start ()
  {
    player = GameObject.Find( "Player" ).transform;

    if (Application.loadedLevelName == "Solar")
    {
      target = GameObject.Find( "Earth" ).transform;
    }
    else
    {
      target = GameObject.Find( "Stargate" ).transform;
    }
  }

  void Update ()
  {
    Vector3 diff = target.transform.position - player.position;
    diff.Normalize();

    float rot_z = Mathf.Atan2( diff.y, diff.x ) * Mathf.Rad2Deg;
    Quaternion tempQuaternion = Quaternion.Euler( 0f, 0f, rot_z - 90 );

    float str = Mathf.Min( 55 * Time.deltaTime * 55, 55 );
    transform.rotation = Quaternion.Lerp( transform.rotation, tempQuaternion, str );
  }
}