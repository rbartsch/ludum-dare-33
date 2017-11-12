//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class Spaceship : MonoBehaviour 
//{
//  public List<GameObject> thrusters;
//  Rigidbody2D ribo2D;

//  void Awake ()
//  {
//    ribo2D = GetComponent<Rigidbody2D>();
//    ribo2D.centerOfMass = new Vector2( 0, 0 );
//  }

//  bool mvLeft;
//  bool mvRight;
//  bool mvFwd;
//  bool mvBckwd;

//  void Update ()
//  {
//    if (Input.GetKey( KeyCode.W ))
//    {
//      mvFwd = true;
//    }
//    if (Input.GetKey( KeyCode.S ))
//    {
//      mvBckwd = true;
//    }
//    if (Input.GetKey( KeyCode.A ))
//    {
//      mvLeft = true;
//    }
//    if (Input.GetKey( KeyCode.D ))
//    {
//      mvRight = true;
//    }

//    Debug.Log( "World center of mass " + ribo2D.worldCenterOfMass );
//    Debug.Log( "Center of mass " + ribo2D.centerOfMass );
//  }

//  void FixedUpdate ()
//  {
//    if(mvLeft)
//    {
//      // use thruster on right to move left
//      ribo2D.AddForceAtPosition( thrusters[ 3 ].transform.up.normalized *24f, thrusters[ 3 ].transform.position );
//      //Vector2 dir = transform.position - thrusters[ 3 ].transform.position;
//      //ribo2D.AddForceAtPosition( dir.normalized * 6f, thrusters[ 3 ].transform.position );
//      mvLeft = false;
//    }
//    if(mvRight)
//    {
//      // use thruster on left to move right
//      ribo2D.AddForceAtPosition( thrusters[ 2 ].transform.up.normalized * 24f, thrusters[ 2 ].transform.position );
//      //Vector2 dir = transform.position - thrusters[ 2 ].transform.position;
//      //ribo2D.AddForceAtPosition( dir.normalized * 6f, thrusters[ 2 ].transform.position );
//      mvRight = false;
//    }
//    if(mvFwd)
//    {
//      ribo2D.AddForceAtPosition( thrusters[ 1 ].transform.up.normalized * 24f, thrusters[ 1 ].transform.position );
//      //Vector2 dir = transform.position - thrusters[ 1 ].transform.position;
//      //ribo2D.AddForceAtPosition( dir.normalized * 2f, thrusters[ 1 ].transform.position );
//      mvFwd = false;
//    }

//  }

//#if UNITY_EDITOR
//  void OnDrawGizmos ()
//  {

//    if (Application.isPlaying)
//    {
//      Gizmos.color = Color.cyan;
//      Gizmos.DrawWireCube( new Vector3( transform.position.x - ribo2D.centerOfMass.x, transform.position.y - ribo2D.centerOfMass.y, 0 ), new Vector2( 5, 5 ) );
//    }
//  }
//#endif
//}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Spaceship : MonoBehaviour
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

  Vector3 spaceshipDir = new Vector3( 0, 0, 0 );

  public float healthCapacity = 500.0f;
  public float currHealth = 500.0f;

  public string mutationName = "";

  bool takeInput = true;
  GameObject gameOverPanel;

  public Transform target;
  AISpaceship aiTarget;

  Interactor interactor;
  Text playerInfoTxt;

  AudioSource hardpointsEngagedSound;

  // Initialisation
  void Awake ()
  {
    Time.timeScale = 1.0f;
    ribo2D = GetComponent<Rigidbody2D>();
    playerInfoTxt = GameObject.Find( "PlayerInfo" ).GetComponent<Text>();
    interactor = GameObject.Find( "Camera" ).GetComponent<Interactor>();
    hardpointsEngagedSound = GetComponent<AudioSource>();
    gameOverPanel = GameObject.Find( "GameOverPanel" );
    gameOverPanel.SetActive( false );
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
    spaceshipDir = transform.up;
    spaceshipDir.Normalize();

    if (takeInput)
    {
      HandleInput();
    }

    if (currHealth <= 0.0f)
    {
      Time.timeScale = 0.2f;
      takeInput = false;
      gameOverPanel.SetActive( true );
    }

    if (interactor.returnedObject != null && interactor.returnedObject.tag == "Enemy")
    {
      target = interactor.returnedObject.transform;
      aiTarget = target.GetComponent<AISpaceship>();
      aiTarget.targeted = true;
    }

    //playerInfoTxt.text = string.Format( "Player Info\n-\nVelocity: {0}\nAngular Velocity: {1}\nSpaceship Facing Direction: {2}\n", ribo2D.velocity, ribo2D.angularVelocity, spaceshipDir );
    playerInfoTxt.text = string.Format( "Integrity: {1}%\nShip Mutation: {0}", mutationName, currHealth / healthCapacity * 100.0f );
  }

  void FixedUpdate ()
  {
    ApplyInput();
  }

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

  // Used in Update for responsive handling
  void HandleInput ()
  {
    if (Input.GetKey( KeyCode.W ))
    {
      mvForward = true;
    }
    else
    {
      mvForward = false;
    }

    if (Input.GetKey( KeyCode.S ))
    {
      mvBackward = true;
    }
    else
    {
      mvBackward = false;
    }

    if (Input.GetKey( KeyCode.D ))
    {
      mvRight = true;
      lastMvRight = true;
      lastMvLeft = false;
    }
    else
    {
      mvRight = false;
    }

    if (Input.GetKey( KeyCode.A ))
    {
      mvLeft = true;
      lastMvRight = false;
      lastMvLeft = true;
    }
    else
    {
      mvLeft = false;
    }

    if (Input.GetKeyDown(KeyCode.Space))
    {
      engageHardpoints = true;
      hardpointsEngagedSound.Play();
    }
    else
    {
      engageHardpoints = false;
    }
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

    if (mvBackward)
    {
      Thrust( ThrusterOrientation.BOW, true );
    }
    else
    {
      Thrust( ThrusterOrientation.BOW, false );
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
      EngageHardpoints();
      engageHardpoints = false; // we have to set immediately to false because physics FixedUpdate is set to 0.01 instead of 0.02 so it is called twice because the Update() sets it to false slower
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
            Thrust( ThrusterOrientation.PORT, true );
          }
          else
          {
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
            Thrust( ThrusterOrientation.STARBOARD, true );
          }
          else
          {
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

  // Engage all the hardpoints/weapons
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

  public void LoadMainMenu ()
  {
    Application.LoadLevel( "MainMenu" );
  }

  public void RestartGame ()
  {
    Application.LoadLevel( "Luhman16" );
  }
}
