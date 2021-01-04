using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FireBaseSignIn : MonoBehaviour
{
  Firebase.Auth.FirebaseAuth auth;
  [Header("Sign in Settings")]
  public InputField UserEmailInputField;
  public InputField UserDisplayInputField;
  public InputField UserPasswordInputField;

  [Header("Login Settings")]
  public InputField UserEmailInputFieldLogin;
  public InputField UserPasswordInputFieldLogin;

  public GameObject signUpPanel;

  private void Awake()
  {
    RegisterEvents();
  }

  private void RegisterEvents()
  {
    EventManager.Instance.AddListener<Events.GetFirebaseAuth>(onGetFirebaseAuth);
  }

  private void onGetFirebaseAuth(GetFirebaseAuth e)
  {
    e.auth(auth);
  }

  private void Start()
  {
    auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    auth.StateChanged += AuthStateChanged;
    AuthStateChanged(this, null);
    Button[] btn = GetComponentsInChildren<Button>();
    foreach (Button button in btn)
    {
      button.onClick.AddListener(() => OnClick(button));
    }
    if (auth.CurrentUser != null)
    {
      Debug.Log("Current User " + auth.CurrentUser.UserId);

      if (!String.IsNullOrEmpty(auth.CurrentUser.UserId))
      {
        //this.gameObject.SetActive(false);
        SceneManager.LoadScene("Menu");
      }
    }
  }

  void AuthStateChanged(object sender, System.EventArgs eventArgs)
  {
    if (auth.CurrentUser != UserData.user)
    {
      bool signedIn = UserData.user != auth.CurrentUser && auth.CurrentUser != null;
      if (!signedIn && UserData.user != null)
      {
        Debug.Log("Signed out " + UserData.user.UserId);
      }
      UserData.user = auth.CurrentUser;
      if (signedIn)
      {
        Debug.Log("Signed in " + UserData.user.UserId + " username " + UserData.user.DisplayName);
        //displayName = user.DisplayName ?? "";
        //emailAddress = user.Email ?? "";
        //photoUrl = user.PhotoUrl ?? "";
      }
    }
  }

  private void OnClick(Button button)
  {
    switch (button.name)
    {
      case "Login":
        Login(UserEmailInputFieldLogin.text, UserPasswordInputFieldLogin.text);
        break;
      case "Signup":
        CreateAccount(UserEmailInputField.text, UserPasswordInputField.text);
        break;
      case "SignupPanel":
        signUpPanel.SetActive(true);
        break;
      case "SignupPanelClose":
        signUpPanel.SetActive(false);
        break;
    }
  }

  void CreateAccount(string UserEmail, string password)
  {
    auth.CreateUserWithEmailAndPasswordAsync(UserEmail, password).ContinueWith(task =>
    {
      if (task.IsCanceled)
      {
        Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
        return;
      }
      if (task.IsFaulted)
      {
        Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
        return;
      }

      // Firebase user has been created.
      UserData.user = task.Result;
      Debug.LogFormat("Firebase user created successfully: {0} ({1})",
          UserData.user.DisplayName, UserData.user.UserId);

      AdditionalOperations();

    });
    //this.gameObject.SetActive(false);
    SceneManager.LoadScene("Menu");

  }

  void AdditionalOperations()
  {
    Firebase.Auth.FirebaseUser user = auth.CurrentUser;
    if (user != null)
    {
      Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
      {
        DisplayName = UserDisplayInputField.text,
        //PhotoUrl = new System.Uri("https://example.com/jane-q-user/profile.jpg"),
      };
      user.UpdateUserProfileAsync(profile).ContinueWith(task =>
      {
        if (task.IsCanceled)
        {
          Debug.LogError("UpdateUserProfileAsync was canceled.");
          return;
        }
        if (task.IsFaulted)
        {
          Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
          return;
        }

        Debug.Log("User profile updated successfully.");
      });
    }



  }
  void Login(string UserEmail, string password)
  {
    auth.SignInWithEmailAndPasswordAsync(UserEmail, password).ContinueWith(task =>
    {
      if (task.IsCanceled)
      {
        Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
        return;
      }
      if (task.IsFaulted)
      {
        Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
        return;
      }
      PlayerPrefs.SetString("UserEmail", UserEmail);
      PlayerPrefs.SetString("UserPassword", password);
      UserData.user = task.Result;
      Debug.LogFormat("User signed in successfully: {0} ({1})",
          UserData.user.DisplayName, UserData.user.UserId);

    });
    //this.gameObject.SetActive(false);
    SceneManager.LoadScene("Menu");

  }
}
