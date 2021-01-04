using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIController : AbstractController
{
  public GameObject BG;
  public GameObject SettingsPanel;
  public GameObject UserPanel;
  public GameObject QuitPanel;
  public GameObject MatchMakingPanel;

  public Slider mapSizeSlider;
  public Text selectedSliderValue;
  public Toggle randomMapToggle;

  public InputField mapNumber;

  public GameObject MapNumberParent;

  public Text NumberOfMatches;
  public Text NumberOfWins;
  public Text UsersRank;
  public Text DisplayName;

  DatabaseReference referenceOfUsersData;

  public override void Initialize()
  {
    FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://pipes-ca9d5-default-rtdb.firebaseio.com/");
    Button[] AllButtons = GetComponentsInChildren<Button>(true);
    foreach (Button btn in AllButtons)
    {
      btn.onClick.AddListener(() => onClick(btn));
    }

    randomMapToggle.onValueChanged.AddListener((ison) => onValueChanged(randomMapToggle, ison));
    mapSizeSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

    CreateUserData();
  }

  private void onUserCreated()
  {
    int[] matchesInfo = UserData.ParseMatchesInfo();
    NumberOfMatches.text = matchesInfo[0].ToString();
    NumberOfWins.text = matchesInfo[1].ToString();
    UsersRank.text = matchesInfo[2].ToString();
    DisplayName.text = UserData.user.DisplayName;
  }

  private void CreateUserData()
  {
    referenceOfUsersData = FirebaseDatabase.DefaultInstance.RootReference;
    referenceOfUsersData = FirebaseDatabase.DefaultInstance.GetReference("Users");
    referenceOfUsersData.Child(UserData.user.UserId).GetValueAsync().ContinueWith(task =>
    {
      if (task.IsFaulted)
      {
        // Handle the error...
      }
      else if (task.IsCompleted)
      {
        DataSnapshot UserDataSnapshot = task.Result;
        UserData.currentState = State.Open;

        if (UserDataSnapshot.Value == null)
        {
          referenceOfUsersData.UpdateChildrenAsync(UserData.ToDictionaryForUserData());
        }
        else
        {
          UserData.matchesInfo = UserDataSnapshot.Child("MatchesInfo").Value.ToString();
          referenceOfUsersData.UpdateChildrenAsync(UserData.ToDictionaryForUserData());
        }
        //userCreated.Invoke();
      }
    });
  }

  private void ValueChangeCheck()
  {
    selectedSliderValue.text = mapSizeSlider.value.ToString();
  }

  private void onValueChanged(Toggle randomMapToggle, bool ison)
  {
    MapNumberParent.SetActive(!ison);
  }

  public override void RegisterEvents()
  {
  }

  public override void UnRegisterEvents()
  {
  }

  private void onClick(Button button)
  {
    switch (button.name)
    {
      case "Offline":
        SceneManager.LoadScene("Main");
        PlayerPrefs.SetInt("GameMode", (int)GameModes.Offline);
        break;
      case "Online":
        MatchMakingPanel.SetActive(true);
        BG.SetActive(false);
        Utils.EventAsync(new Events.StartMatchMaking());

        PlayerPrefs.SetInt("GameMode", (int)GameModes.Online);
        break;
      case "MenuClose":
        QuitPanel.SetActive(true);
        break;
      case "User":
        onUserCreated();
        UserPanel.SetActive(true);
        break;
      case "UserClose":
        UserPanel.SetActive(false);
        break;
      case "Settings":
        SettingsPanel.SetActive(true);
        break;
      case "Quit":
        Application.Quit();
        break;
      case "QuitCancel":
        QuitPanel.SetActive(false);
        break;
      case "QuitClose":
        QuitPanel.SetActive(false);
        break;
      case "SettingsClose":
        PlayerPrefs.SetInt("Settings_MapSize", int.Parse(mapSizeSlider.value.ToString()));
        if (randomMapToggle.isOn)
        {
          PlayerPrefs.SetInt("Settings_MapNumber", -1);
        }
        else
        {
          if (!string.IsNullOrEmpty(mapNumber.text))
          {
            int mapNumberVal = 7;
            if (int.TryParse(mapNumber.text, out mapNumberVal))
            {
              PlayerPrefs.SetInt("Settings_MapNumber", mapNumberVal);
            }
            else
            {
              PlayerPrefs.SetInt("Settings_MapNumber", mapNumberVal);
            }
          }
          else
          {
            PlayerPrefs.SetInt("Settings_MapNumber", 7);

          }
        }
        PlayerPrefs.SetInt("Settings_MapSize", int.Parse(mapSizeSlider.value.ToString()));
        SettingsPanel.SetActive(false);
        break;
    }

  }

  //private void firebaseAuthCallback(FirebaseAuth auth)
  //{
  //  auth.SignOut();
  //  SignInPanel.SetActive(true);
  //}
}
