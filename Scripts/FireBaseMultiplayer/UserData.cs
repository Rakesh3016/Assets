using Firebase.Auth;
using System.Collections.Generic;

public static class UserData
{
  public static FirebaseUser user;
  //public static string UserName;
  //public static string UserID;
  //public static UserData(string UserName, string UserID)
  //{
  //    this.UserID = UserID;
  //    this.UserName = UserName;
  //}
  public static string matchesInfo;
  public static State currentState;

  static UserData()
  {
    user = null;
    //TotalMatchesPlayed, TotalWon, WinPrecent
    matchesInfo = "0,0,0";
    currentState = State.Open;
  }

  public static Dictionary<string, object> ToDictionaryForUserData()
  {
    Dictionary<string, object> result = new Dictionary<string, object>();
    result[user.UserId] = ToDictionaryOfKey2();

    return result;
  }

  public static Dictionary<string, object> ToDictionaryForOthers()
  {
    Dictionary<string, object> result = new Dictionary<string, object>();
    result[user.UserId] = "placeholder";
    return result;
  }
  public static Dictionary<string, object> ToDictionaryOfKey2()
  {
    Dictionary<string, object> result = new Dictionary<string, object>();
    result["uname"] = user.DisplayName;
    result["state"] = (int)currentState;
    result["MatchesInfo"] = matchesInfo;
    return result;
  }

  public static int[] ParseMatchesInfo()
  {
    string[] eachInfo = matchesInfo.Split(',');
    int[] integrs = new int[eachInfo.Length];
    for (int i = 0; i < eachInfo.Length; i++)
    {
      integrs[i] = int.Parse(eachInfo[i]);
    }
    return integrs;
  }
}

public static class OpponentData
{
  public static DummyFirebaseUser user;
  public static string matchesInfo;
  public static State currentState;

  static OpponentData()
  {
    user = null;
    //TotalMatchesPlayed, TotalWon, WinPrecent
    matchesInfo = "0,0,0";
    currentState = State.Open;
  }

  public static int[] ParseMatchesInfo()
  {
    string[] eachInfo = matchesInfo.Split(',');
    int[] integrs = new int[eachInfo.Length];
    for (int i = 0; i < eachInfo.Length; i++)
    {
      integrs[i] = int.Parse(eachInfo[i]);
    }
    return integrs;
  }

}

public class DummyFirebaseUser
{
  public string UserId;
  public string DisplayName;

  public DummyFirebaseUser(string userId, string displayName)
  {
    UserId = userId;
    DisplayName = displayName;
  }
}