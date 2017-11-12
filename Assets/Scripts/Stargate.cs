using UnityEngine;
using System.Collections;

public class Stargate : MonoBehaviour 
{
  void FixedUpdate ()
  {
    transform.eulerAngles += new Vector3( 0, 0, 1000 * Time.fixedDeltaTime );
  }

  void OnTriggerEnter2D ( Collider2D collider )
  {
    if (collider.tag == "Player")
    {
      if (Application.loadedLevelName == "Luhman16")
      {
        Application.LoadLevel( "BarnardsStar" );
      }
      if (Application.loadedLevelName == "BarnardsStar")
      {
        Application.LoadLevel( "AlphaCentuari" );
      }
      if (Application.loadedLevelName == "AlphaCentuari")
      {
        Application.LoadLevel( "Solar" );
      }
    }
  }
}