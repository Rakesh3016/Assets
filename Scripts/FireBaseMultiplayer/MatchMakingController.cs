using Events;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchMakingController : AbstractController
{
  DatabaseReference referenceOfUsersData;
  DatabaseReference referenceOfNewCreators;
  DatabaseReference referenceOfNewJoiners;
  DatabaseReference referenceOfIngamePlayers;
  DatabaseReference CreatorReference;

  string newCreatorsAddedPath = "matchmaking";


  public override void Initialize()
  {
    FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://pipes-ca9d5-default-rtdb.firebaseio.com/");
  }

  public override void RegisterEvents()
  {
    EventManager.Instance.AddListener<StartMatchMaking>(OnStartMatchMaking);
  }

  public override void UnRegisterEvents()
  {
    EventManager.Instance.RemoveListener<StartMatchMaking>(OnStartMatchMaking);
  }

  private void OnStartMatchMaking(StartMatchMaking e)
  {
    //CreateUserData();
    CreateMatchMakers();
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
      }
    });
  }
  public void CreateMatchMakers()
  {
    referenceOfNewCreators = FirebaseDatabase.DefaultInstance.GetReference(newCreatorsAddedPath);
    referenceOfNewCreators.UpdateChildrenAsync(UserData.ToDictionaryForOthers());

    CreatorReference = referenceOfNewCreators.Child("/" + UserData.user.UserId);
    OnDisconnect onDisconnect = CreatorReference.OnDisconnect();
    onDisconnect.RemoveValue();
    CreatorReference.ValueChanged += onGameFound;
  }

  private void onGameFound(object sender, ValueChangedEventArgs args)
  {
    if (args.DatabaseError == null && args.Snapshot.Value != null)
    {
      string s = args.Snapshot.Value.ToString();
      if (s == "placeholder")
        return;
      CreatorReference.RemoveValueAsync();
      //GameManager.Instance.gameId = s;
      PlayerPrefs.SetString("gameId", s);
      CreatorReference.ValueChanged -= onGameFound;

      SceneManager.LoadScene("Main");
    }
  }


}


//public enum State
//{
//  Open = 1,
//  Joined = 2
//}