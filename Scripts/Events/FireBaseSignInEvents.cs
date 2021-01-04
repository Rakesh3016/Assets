using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Events
{
  public class GetFirebaseAuth : GameEvent
  {
    public Action<Firebase.Auth.FirebaseAuth> auth;
    public GetFirebaseAuth(Action<FirebaseAuth> auth)
    {
      this.auth = auth;
    }
  }
}