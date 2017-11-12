using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
  // this is just for main menu

  void Start ()
  {
    Time.timeScale = 0.2f;
  }

  public void Play ()
  {
    Application.LoadLevel( "Luhman16" );
  }

  public void HowToPlay ()
  {
    Application.LoadLevel( "HowToPlay" );
  }

  public void AboutThisEntry ()
  {
    Application.LoadLevel( "AboutThisEntry" );
  }

  public void BackToMainMenu ()
  {
    Application.LoadLevel( "MainMenu" );
  }

  public void Exit ()
  {
    Application.Quit();
  }
}
