using UnityEngine;
using System.Collections;

public enum RelationshipOrigin
{
  PLAYER,
  ENEMY
}

public class Projectile : MonoBehaviour 
{
  public float damage = 20.0f;
  public RelationshipOrigin origin;

  float timeToLive = 5.0f;
  float currentTime = 0.0f;

  void Start ()
  {
    currentTime = timeToLive;
  }

  void FixedUpdate ()
  {
    currentTime -= Time.fixedDeltaTime;

    if (currentTime <= 0.0f)
    {
      Destroy( this.gameObject );
    }
  }

  void OnTriggerEnter2D ( Collider2D collider )
  {
    if (collider.tag == "Enemy" && origin == RelationshipOrigin.PLAYER)
    {
      AISpaceship ai = collider.GetComponent<AISpaceship>();
      ai.ApplyDamage( damage );
      Destroy( this.gameObject );
    }
    else if (collider.tag == "Player" && origin == RelationshipOrigin.ENEMY)
    {
      Spaceship player = collider.GetComponent<Spaceship>();
      player.ApplyDamage( damage );
      Destroy( this.gameObject );
    }
  }
}