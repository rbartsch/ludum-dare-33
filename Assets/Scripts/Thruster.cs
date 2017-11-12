using UnityEngine;
using System.Collections;

public enum ThrusterType
{
  LARGE,
  SMALL
}

public enum ThrusterOrientation
{
  BOW,       // NORTH
  STARBOARD, // EAST
  STERN,     // SOUTH
  PORT       // WEST
}

public class Thruster : MonoBehaviour
{
  public ThrusterType thrusterType;
  public ThrusterOrientation orientation;

  Rigidbody2D spaceshipRibo2D;
  Animator animator;
  float thrust = 0f;
  public bool overrideThrust = false;
  public float overriddenThrust = 0f;
  void Awake ()
  {
    spaceshipRibo2D = gameObject.GetComponentInParent<Rigidbody2D>();
    animator = GetComponent<Animator>();
  }

  void Start ()
  {
    if (overrideThrust)
    {
      thrust = overriddenThrust;
    }
    else
    {
      if (thrusterType == ThrusterType.LARGE)
      {
        thrust = 18.0f;
      }
      else if (thrusterType == ThrusterType.SMALL)
      {
        thrust = 27.0f;
      }
    }

    if (transform.rotation.eulerAngles.z >= -1.0f && transform.rotation.eulerAngles.z <= 1.0f)
    {
      orientation = ThrusterOrientation.STERN;
    }
    else if (transform.rotation.eulerAngles.z >= 179.0f && transform.rotation.eulerAngles.z <= 181.0f)
    {
      orientation = ThrusterOrientation.BOW;
    }
    else if (transform.rotation.eulerAngles.z >= 269.0f && transform.rotation.eulerAngles.z <= 271.0f)
    {
      orientation = ThrusterOrientation.PORT;
    }
    else if (transform.rotation.eulerAngles.z >= 89.0f && transform.rotation.eulerAngles.z <= 91.0f)
    {
      orientation = ThrusterOrientation.STARBOARD;
    }
  }

  public void EngageThrust ()
  {
    animator.SetBool( "thruster_active", true );
    animator.SetBool( "thruster_inactive", false );
    spaceshipRibo2D.AddForceAtPosition( transform.up.normalized * thrust, transform.position );
  }

  public void DisengageThrust ()
  {
    animator.SetBool( "thruster_active", false );
    animator.SetBool( "thruster_inactive", true );
  }
}
