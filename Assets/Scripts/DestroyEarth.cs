using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DestroyEarth : MonoBehaviour 
{
  Animator animator;
  GameObject earthObject;
  public GameObject destroyedEarthObject;
  Button destroyEarthButton;
  Text buttonText;
  GameObject youWinPanel;

  void Start ()
  {
    destroyEarthButton = GameObject.Find( "DestroyEarthButton" ).GetComponent<Button>();
    buttonText = destroyEarthButton.transform.FindChild( "Text" ).GetComponent<Text>();
    youWinPanel = GameObject.Find( "YouWinPanel" );
    youWinPanel.SetActive( false );
    earthObject = gameObject;
  }

  void OnTriggerEnter2D (Collider2D collider)
  {
    if (collider.tag == "Player")
    {
      destroyEarthButton.interactable = true;
      buttonText.text = "Destroy Earth (In Range)";
    }
  }

  void OnTriggerExit2D ( Collider2D collider )
  {
    if (collider.tag == "Player")
    {
      destroyEarthButton.interactable = false;
      buttonText.text = "Destroy Earth (Out Of Range)";
    }
  }

  IEnumerator DestroyAnim (GameObject _clone)
  {
    yield return new WaitForSeconds( 1 );
    Destroy( _clone );
    youWinPanel.SetActive( true );
    Time.timeScale = 0.2f;
  }

  public void DoDestroyEarth ()
  {
    GameObject clone = (GameObject)Instantiate( destroyedEarthObject, new Vector3( transform.position.x, transform.position.y, transform.position.z ), Quaternion.identity );
    earthObject.transform.position = new Vector3( 15000, 15000, 15000 );
    StartCoroutine( DestroyAnim( clone ) );
  }
}