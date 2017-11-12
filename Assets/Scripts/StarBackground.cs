using UnityEngine;
using System.Collections;

public class StarBackground : MonoBehaviour 
{
  public ParticleSystem[] starBackgroundParticles;

  // TODO: make it pick up from the opposite end instead of center.
  // e.g if going up Y and reach top Y, position moved up by 7500 so that the center is 7500 above but just entering the bottom end of Y
  public Transform player;
  Vector3 lastPosition;
  int sizeOfBoxFromCenter = 7500;
  int minSizeToStartWrap = 7480;

  void Start ()
  {
    player = GameObject.Find( "Player" ).transform;

    foreach (ParticleSystem pS in starBackgroundParticles)
    {
      pS.Pause();
    }

    lastPosition = transform.position;
  }

  void Update ()
  {
    if (( player.position.y >= lastPosition.y + minSizeToStartWrap && player.position.y <= lastPosition.y + sizeOfBoxFromCenter ) ||
      ( player.position.y <= lastPosition.y - minSizeToStartWrap && player.position.y >= lastPosition.y - sizeOfBoxFromCenter ) ||
      ( player.position.x >= lastPosition.x + minSizeToStartWrap && player.position.x <= lastPosition.x + sizeOfBoxFromCenter ) ||
      ( player.position.x <= lastPosition.x - minSizeToStartWrap && player.position.x >= lastPosition.x - sizeOfBoxFromCenter ))
    {
      transform.position = player.position;
      lastPosition = transform.position;
    }
  }
}