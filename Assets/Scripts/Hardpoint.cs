using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hardpoint : MonoBehaviour 
{
  public Transform target;
  public GameObject projectile;

  float trackingStrength     = 1f;
  public float strength      = 2f;
  public float minRotation   = 212f;
  public float maxRotation   = 327f;
  public bool lockRotation   = true;
  public bool horizontalAxis = false;

  Spaceship player;
  AISpaceship ai;

  float ClampAngle ( float angle, float min, float max )
  {
    if (angle < 90 || angle > 270)
    {       // if angle in the critic region...
      if (angle > 180)
      {
        angle -= 360;  // convert all angles to -180..+180
      }
      if (max > 180)
      {
        max -= 360;
      }
      if (min > 180)
      {
        min -= 360;
      }
    }
    angle = Mathf.Clamp( angle, min, max );
    if (angle < 0)
    {
      angle += 360;  // if angle negative, convert to 0..360
    }
    return angle;
  }

  void RotationMovement ()
  {
    if (target != null)
    {
      Vector3 diff = target.transform.position - transform.position;
      diff.Normalize();

      float rot_z = Mathf.Atan2( diff.y, diff.x ) * Mathf.Rad2Deg;
      Quaternion tempQuaternion = Quaternion.Euler( 0f, 0f, rot_z - 90 );

      float str = Mathf.Min( strength * Time.deltaTime * trackingStrength, trackingStrength );
      transform.rotation = Quaternion.Lerp( transform.rotation, tempQuaternion, str );

      if (lockRotation)
      {
        //float lockedRotation = Mathf.Clamp( transform.localEulerAngles.z, minRotation, maxRotation );
        //Debug.Log( lockedRotation );
        //Debug.Log( transform.localEulerAngles.z );
        //transform.localEulerAngles = new Vector3( transform.localEulerAngles.x, transform.localEulerAngles.y, lockedRotation );
        transform.localEulerAngles = new Vector3( transform.localEulerAngles.x, transform.localEulerAngles.y, ClampAngle( transform.localEulerAngles.z, minRotation, maxRotation ) );
      }
    }
    else
    {
      transform.localEulerAngles = new Vector3( 0, 0, ClampAngle( transform.localEulerAngles.z, minRotation, maxRotation ) );
      //Debug.Log( "No target" );
    }
  }

  public void EngageHardpoint ()
  {
    GameObject proj = (GameObject)Instantiate( projectile, transform.position, Quaternion.identity );
    Projectile _projectile = proj.GetComponent<Projectile>();

    if (transform.parent.tag == "Player")
    {
      _projectile.origin = RelationshipOrigin.PLAYER;
    }
    else if (transform.parent.tag == "Enemy")
    {
      _projectile.origin = RelationshipOrigin.ENEMY;
    }

    proj.transform.Rotate( 0, 0, transform.rotation.eulerAngles.z );
    proj.GetComponent<Rigidbody2D>().AddForce( transform.up * 30f, ForceMode2D.Impulse );
  }

  void Start ()
  {
    if (transform.parent.tag == "Player")
    {
      player = transform.parent.GetComponent<Spaceship>();
      target = player.target;
    }
    else if (transform.parent.tag == "Enemy")
    {
      ai = transform.parent.GetComponent<AISpaceship>();
      target = ai.target;
    }
  }

  void Update ()
  {
    if (transform.parent.tag == "Player")
    {
      target = player.target;
    }
    else if (transform.parent.tag == "Enemy")
    {
      target = ai.target;
    }

    RotationMovement();
  }
}