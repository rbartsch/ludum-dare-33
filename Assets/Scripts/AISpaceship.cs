using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

enum Quadrant
{
  UPPER_LEFT,
  UPPER_RIGHT,
  BOTTOM_LEFT,
  BOTTOM_RIGHT,
  CENTER
}

public class AISpaceship : MonoBehaviour
{
  Rigidbody2D ribo2D;
  List<Thruster> thrusters = new List<Thruster>();
  List<Hardpoint> hardpoints = new List<Hardpoint>();

  bool mvForward = false;
  bool mvBackward = false;
  bool mvRight = false;
  bool mvLeft = false;
  bool engageHardpoints = false;

  public bool horizontalFlightAssist = false;
  bool lastMvRight = false;
  bool lastMvLeft = false;

  public Transform target;
  Vector3 directionToTarget = new Vector3( 0, 0, 0 );
  Vector3 thisSpaceshipDirection = new Vector3( 0, 0, 0 );
  float angleToReach = 5;

  Quadrant aiCurrentQuadrant;
  Quadrant targetCurrentQuadrant;

  public float healthCapacity = 100.0f;
  public float currHealth = 100.0f;
  public float weaponTimer = 1.0f;
  public float currWeaponTime = 0.0f;

  Text aiInfoTxt;
  Text healthTxt;
  public Text targetedTxt;

  public bool targeted = false;

  public LayerMask mask;

  AudioSource hardpointsEngagedSound;

  // Initialisation
  void Awake ()
  {
    ribo2D = GetComponent<Rigidbody2D>();
    //aiInfoTxt = GameObject.Find( "AIInfo" ).GetComponent<Text>();
    healthTxt = transform.FindChild( "WorldCanvas" ).FindChild( "Health" ).GetComponent<Text>();
    targetedTxt = transform.FindChild( "WorldCanvas" ).FindChild( "Target" ).GetComponent<Text>();
    target = GameObject.Find( "Player" ).transform;
    hardpointsEngagedSound = GetComponent<AudioSource>();
  }

  // Setup from initialisation
  void Start ()
  {
    ribo2D.centerOfMass = new Vector2( 0, 0 );

    foreach (Transform t in transform)
    {
      if (t.GetComponent<Thruster>() != null)
      {
        thrusters.Add( t.GetComponent<Thruster>() );
      }
    }

    foreach (Transform t in transform)
    {
      if (t.GetComponent<Hardpoint>() != null)
      {
        hardpoints.Add( t.GetComponent<Hardpoint>() );
      }
    }
  }

  void Update ()
  {
    float angle = Vector3.Angle( thisSpaceshipDirection, directionToTarget );
    HandleInput();

    if (currHealth <= 0.0f)
    {
      Destroy( this.gameObject );
    }

    //aiInfoTxt.text = string.Format( "AI Info\n-\nVelocity: {0}\nAngular Velocity: {1}\nSpaceship Facing Direction: {2}\nDirection To Target: {3}\nAI Current Quadrant: {4}\nTarget Current Quadrant: {5}\nAngle: {6}",
    //  ribo2D.velocity, ribo2D.angularVelocity, thisSpaceshipDirection, directionToTarget, aiCurrentQuadrant, targetCurrentQuadrant, angle );

    healthTxt.text = string.Format( "{0}%", currHealth / healthCapacity * 100.0f );

    if (targeted)
    {
      targetedTxt.text = "[T]";
      targeted = false;
    }
    else
    {
      targetedTxt.text = "";
    }
  }

  void FixedUpdate ()
  {
    ApplyInput();
  }

  #region "OnDrawGizmos"
#if UNITY_EDITOR
  void OnDrawGizmos ()
  {
    if (Application.isPlaying)
    {
      Gizmos.color = Color.cyan;
      Gizmos.DrawWireCube( new Vector3( transform.position.x - ribo2D.centerOfMass.x, transform.position.y - ribo2D.centerOfMass.y, 0 ), new Vector2( 5, 5 ) );
    }
  }
#endif
  #endregion

  // Used in Update for responsive handling
  void HandleInput ()
  {
    directionToTarget = target.position - transform.position;
    directionToTarget.Normalize();
    thisSpaceshipDirection = transform.up;
    thisSpaceshipDirection.Normalize();

    // Check where the AI is pointing
    if (( ( Mathf.Approximately( thisSpaceshipDirection.x, 1.0f ) || Mathf.Approximately( thisSpaceshipDirection.x, -1.0f ) ) && Mathf.Approximately( thisSpaceshipDirection.y, 0.0f ) ) ||
      ( ( Mathf.Approximately( thisSpaceshipDirection.y, 1.0f ) || Mathf.Approximately( thisSpaceshipDirection.y, -1.0f ) ) && Mathf.Approximately( thisSpaceshipDirection.x, 0.0f ) ))
    {
      //aiCurrentQuadrant = Quadrant.CENTER;
    }
    if (thisSpaceshipDirection.x < 0.0f && thisSpaceshipDirection.y > 0.0f)
    {
      aiCurrentQuadrant = Quadrant.UPPER_LEFT;
    }
    if (thisSpaceshipDirection.x > 0.0f && thisSpaceshipDirection.y > 0.0f)
    {
      aiCurrentQuadrant = Quadrant.UPPER_RIGHT;
    }
    if (thisSpaceshipDirection.x < 0.0f && thisSpaceshipDirection.y < 0.0f)
    {
      aiCurrentQuadrant = Quadrant.BOTTOM_LEFT;
    }
    if (thisSpaceshipDirection.x > 0.0f && thisSpaceshipDirection.y < 0.0f)
    {
      aiCurrentQuadrant = Quadrant.BOTTOM_RIGHT;
    }

    // Check where the target is pointing in terms of direction, this is relative to the ai and not literally the absolute direction of the target at the time.
    if (( ( Mathf.Approximately( directionToTarget.x, 1.0f ) || Mathf.Approximately( directionToTarget.x, -1.0f ) ) && Mathf.Approximately( directionToTarget.y, 0.0f ) ) ||
      ( ( Mathf.Approximately( directionToTarget.y, 1.0f ) || Mathf.Approximately( directionToTarget.y, -1.0f ) ) && Mathf.Approximately( directionToTarget.x, 0.0f ) ))
    {
      //targetCurrentQuadrant = Quadrant.CENTER;
    }
    if (directionToTarget.x < 0.0f && directionToTarget.y > 0.0f)
    {
      targetCurrentQuadrant = Quadrant.UPPER_LEFT;
    }
    if (directionToTarget.x > 0.0f && directionToTarget.y > 0.0f)
    {
      targetCurrentQuadrant = Quadrant.UPPER_RIGHT;
    }
    if (directionToTarget.x < 0.0f && directionToTarget.y < 0.0f)
    {
      targetCurrentQuadrant = Quadrant.BOTTOM_LEFT;
    }
    if (directionToTarget.x > 0.0f && directionToTarget.y < 0.0f)
    {
      targetCurrentQuadrant = Quadrant.BOTTOM_RIGHT;
    }

    /************************/

    mvForward = true;

    // TARGET -> UPPER LEFT
    if (( targetCurrentQuadrant == Quadrant.UPPER_LEFT && aiCurrentQuadrant == Quadrant.UPPER_RIGHT ) && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvLeft = true;
      lastMvLeft = true;
    }
    if (( targetCurrentQuadrant == Quadrant.UPPER_LEFT && aiCurrentQuadrant == Quadrant.BOTTOM_RIGHT ) && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvLeft = true;
      lastMvLeft = true;
    }
    if (( targetCurrentQuadrant == Quadrant.UPPER_LEFT && aiCurrentQuadrant == Quadrant.BOTTOM_LEFT ) && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvRight = true;
      lastMvRight = true;
    }

    // TARGET -> UPPER RIGHT
    if (( targetCurrentQuadrant == Quadrant.UPPER_RIGHT && aiCurrentQuadrant == Quadrant.UPPER_LEFT ) && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvRight = true;
      lastMvRight = true;
    }
    if (( targetCurrentQuadrant == Quadrant.UPPER_RIGHT && aiCurrentQuadrant == Quadrant.BOTTOM_LEFT ) && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvRight = true;
      lastMvRight = true;
    }
    if (( targetCurrentQuadrant == Quadrant.UPPER_RIGHT && aiCurrentQuadrant == Quadrant.BOTTOM_RIGHT ) && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvLeft = true;
      lastMvLeft = true;
    }

    // TARGET -> BOTTOM LEFT
    if (( targetCurrentQuadrant == Quadrant.BOTTOM_LEFT && aiCurrentQuadrant == Quadrant.UPPER_LEFT ) && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvLeft = true;
      lastMvLeft = true;
    }
    if (( targetCurrentQuadrant == Quadrant.BOTTOM_LEFT && aiCurrentQuadrant == Quadrant.UPPER_RIGHT ) && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvLeft = true;
      lastMvLeft = true;
    }
    if (( targetCurrentQuadrant == Quadrant.BOTTOM_LEFT && aiCurrentQuadrant == Quadrant.BOTTOM_RIGHT ) && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvRight = true;
      lastMvRight = true;
    }

    // TARGET -> BOTTOM RIGHT
    if (( targetCurrentQuadrant == Quadrant.BOTTOM_RIGHT && aiCurrentQuadrant == Quadrant.UPPER_LEFT ) && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvLeft = true;
      lastMvLeft = true;
    }
    if (( targetCurrentQuadrant == Quadrant.BOTTOM_RIGHT && aiCurrentQuadrant == Quadrant.UPPER_RIGHT ) && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvRight = true;
      lastMvRight = true;
    }
    if (( targetCurrentQuadrant == Quadrant.BOTTOM_RIGHT && aiCurrentQuadrant == Quadrant.BOTTOM_LEFT ) && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvLeft = true;
      lastMvLeft = true;
    }

    // if for whatever reason it tries to move right and left at the same time (and cause it to go in that direction infinitely)
    // we check what the last attempt was and unset the opposite.
    // e.g if we moved left and run at about the same time but right was the very last, then we unset left
    if (mvRight == true && mvLeft == true)
    {
      //Debug.Log( "GOT STUCK, UNWIGGLING!" );
      if (lastMvRight)
      {
        mvLeft = false;
      }
      else if (lastMvLeft)
      {
        mvRight = false;
      }
    }

    if (mvLeft && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvLeft = true;
    }
    else
    {
      mvLeft = false;
    }

    if (mvRight && Vector3.Angle( thisSpaceshipDirection, directionToTarget ) > angleToReach)
    {
      mvRight = true;
    }
    else
    {
      mvRight = false;
    }

    RaycastHit2D hit = Physics2D.Raycast( transform.position, transform.up, 600.0f, mask );
    RaycastHit2D hit2 = Physics2D.Raycast( new Vector3(transform.position.x + 16f, transform.position.y, transform.position.z), transform.up, 600.0f, mask );
    RaycastHit2D hit3 = Physics2D.Raycast( new Vector3( transform.position.x - 16f, transform.position.y, transform.position.z ), transform.up, 600.0f, mask );

    if (( hit.collider != null && hit.collider.tag == "Player" ) || ( hit2.collider != null && hit2.collider.tag == "Player" ) || ( hit3.collider != null && hit3.collider.tag == "Player" ))
    {
      engageHardpoints = true;
    }
    else
    {
      engageHardpoints = false;
    }
    //Debug.DrawRay( transform.position, transform.up * 600.0f, Color.red );
    //Debug.DrawRay( new Vector3( transform.position.x + 16f, transform.position.y, transform.position.z ), transform.up * 600.0f, Color.red );
    //Debug.DrawRay( new Vector3( transform.position.x - 16f, transform.position.y, transform.position.z ), transform.up * 600.0f, Color.red );
  }

  // Used for FixedUpdate for physics
  void ApplyInput ()
  {
    if (mvForward)
    {
      Thrust( ThrusterOrientation.STERN, true );
    }
    else
    {
      Thrust( ThrusterOrientation.STERN, false );
    }

    if (mvRight)
    {
      Thrust( ThrusterOrientation.PORT, true );
    }
    else
    {
      Thrust( ThrusterOrientation.PORT, false );
    }

    if (mvLeft)
    {
      Thrust( ThrusterOrientation.STARBOARD, true );
    }
    else
    {
      Thrust( ThrusterOrientation.STARBOARD, false );
    }

    if (engageHardpoints)
    {
      currWeaponTime -= Time.fixedDeltaTime;

      if (currWeaponTime <= 0.0f)
      {
        hardpointsEngagedSound.Play();
        EngageHardpoints();
        engageHardpoints = false;
        currWeaponTime = weaponTimer;
      }
    }

    // Apply opposite force for right/left. Flight assist.
    // Perhaps do these checks in Update() and apply the thrust here in FixedUpdate so that we don't have to 
    // set Fixed Timestep to 0.01 from 0.02 to check if the angular velocity is or isn't 0
    #region "Horizontal Flight Assist"
    if (horizontalFlightAssist)
    {
      if (Mathf.Round( ribo2D.angularVelocity ) != 0 && !mvRight && !mvLeft)
      {
        if (lastMvRight && !lastMvLeft)
        {
          if (ribo2D.angularVelocity > 0)
          {
            //ribo2D.angularVelocity = 0;
            Thrust( ThrusterOrientation.PORT, true );
          }
          else
          {
            //ribo2D.angularVelocity = 0;
            Thrust( ThrusterOrientation.STARBOARD, true );
          }
        }
        else if (!lastMvRight && lastMvLeft)
        {
          // check if < 0 angular velocity and attempted to turn in opposite direction in order to ease to 0 velocity and not cause an infinite spin when trying to do
          // manual flight + assist. If you would turn right and angular velocity would be < 0 and then try turn left the assistance would think you turned left last and would
          // constantly turn the ship right/opposite direction till it was 0 angular velocity but since this happened with < 0 angular velocity you'd go in an infinite spin because you'd
          // never reach >= 0. This way we apply thrust to the same direction direction as last instruction and ignore the direction that would've been the opposite of last instruction in order to reach 0.
          // e.g
          // at 0 angular velocity -> turning right -> angular velocity < 0 -> turn left -> since angular velocity < 0 we don't then do opposite of left, we continue with left.
          // Same goes for above but opposite
          if (ribo2D.angularVelocity < 0)
          {
            //ribo2D.angularVelocity = 0;
            Thrust( ThrusterOrientation.STARBOARD, true );
          }
          else
          {
            //ribo2D.angularVelocity = 0;
            Thrust( ThrusterOrientation.PORT, true );
          }
        }
      }
      else if (Mathf.Round( ribo2D.angularVelocity ) == 0 && ( lastMvRight || lastMvLeft ))
      {
        ribo2D.angularVelocity = 0;
        lastMvRight = false;
        lastMvLeft = false;
        Thrust( ThrusterOrientation.STARBOARD, false );
        Thrust( ThrusterOrientation.PORT, false );
      }
    }
    #endregion
  }

  // Handle all the thrust movement
  void Thrust ( ThrusterOrientation orientation, bool engage )
  {
    switch (orientation)
    {
      // Thrusters placed at the bottom end of the ship to move forward
      case ThrusterOrientation.STERN:
        {
          for (int i = 0; i < thrusters.Count; i++)
          {
            if (thrusters[ i ].orientation == ThrusterOrientation.STERN)
            {
              if (engage)
              {
                thrusters[ i ].EngageThrust();
              }
              else
              {
                thrusters[ i ].DisengageThrust();
              }
            }
          }
        } break;

      // Thrusters placed on the top end of the ship to move backward
      case ThrusterOrientation.BOW:
        {
          for (int i = 0; i < thrusters.Count; i++)
          {
            if (thrusters[ i ].orientation == ThrusterOrientation.BOW)
            {
              if (engage)
              {
                thrusters[ i ].EngageThrust();
              }
              else
              {
                thrusters[ i ].DisengageThrust();
              }
            }
          }
        } break;

      // Thrusters placed on the left side of the ship to move right
      case ThrusterOrientation.PORT:
        {
          for (int i = 0; i < thrusters.Count; i++)
          {
            if (thrusters[ i ].orientation == ThrusterOrientation.PORT)
            {
              if (engage)
              {
                thrusters[ i ].EngageThrust();
              }
              else
              {
                thrusters[ i ].DisengageThrust();
              }
            }
          }
        } break;

      // Thrusters placed on the right side of the ship to move left;
      case ThrusterOrientation.STARBOARD:
        {
          for (int i = 0; i < thrusters.Count; i++)
          {
            if (thrusters[ i ].orientation == ThrusterOrientation.STARBOARD)
            {
              if (engage)
              {
                thrusters[ i ].EngageThrust();
              }
              else
              {
                thrusters[ i ].DisengageThrust();
              }
            }
          }
        } break;

      default:
        {
          Debug.Log( "Don't know which direction to thrust!" );
        } break;
    }
  }

  void EngageHardpoints ()
  {
    for (int i = 0; i < hardpoints.Count; i++)
    {
      hardpoints[ i ].EngageHardpoint();
    }
  }

  public void ApplyDamage ( float damage )
  {
    currHealth -= damage;
  }
}
