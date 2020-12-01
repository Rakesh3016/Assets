using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Tile : MonoBehaviour
{
  public Vector2Int position;
  public PlayerSymbol tileType;

  //public Material walkableMaterial;
  //public Material blockerMaterial;
  //public Material player1Material;
  //public Material player2Material;

  public GameObject blockerPrefab;
  public GameObject player1PipePrefab;
  public GameObject player2PipePrefab;
  public GameObject WalkerPrefab;
  public Material WalkableMaterialPlayer1;
  public Material WalkableMaterialPlayer2;
  public bool containsPipeGenerator;

  private Renderer renderer;

  public PowerType powerType;

  public void Awake()
  {
    renderer = WalkerPrefab.transform.GetComponent<Renderer>();
  }

  public void setPlayerIndicator(PlayerType turn)
  {
    if (turn == PlayerType.P1)
      renderer.material = WalkableMaterialPlayer1;
    else
      renderer.material = WalkableMaterialPlayer2;

  }

  public void setInitialData(PlayerSymbol playerSymbol, Vector2Int position)
  {
    //if (playerSymbol == PlayerSymbol.Walkable)
    //{
    //  //this.transform.tag = "Walkable";
    //  renderer.material = walkableMaterial;
    //}
    //else if (playerSymbol == PlayerSymbol.Blocker)
    //  renderer.material = blockerMaterial;

    tileType = playerSymbol;
    activateValidPrefab(tileType);

    this.position = position;
  }

  public void setPlayer1Data()
  {
    //renderer.material = player1Material;
    tileType = PlayerSymbol.P1;
    containsPipeGenerator = true;
    activateValidPrefab(tileType);

  }
  public void setWalkableData()
  {
    //renderer.material = walkableMaterial;
    tileType = PlayerSymbol.Walkable;
    containsPipeGenerator = false;
    activateValidPrefab(tileType);
  }

  public void setPlayer2Data()
  {
    //renderer.material = player2Material;
    tileType = PlayerSymbol.P2;
    containsPipeGenerator = true;
    activateValidPrefab(tileType);
  }

  private PowerType GetRandomPower()
  {
    int randomNumber = Virus.Utils.GetRandomNumber(1, Enum.GetValues(typeof(PowerType)).Length);
    Debug.Log("rand 1,3 " + randomNumber);
    return (PowerType)randomNumber;
  }

  private void activateValidPrefab(PlayerSymbol tileType)
  {
    blockerPrefab.SetActive(false);
    player1PipePrefab.SetActive(false);
    player2PipePrefab.SetActive(false);
    WalkerPrefab.SetActive(false);

    switch (tileType)
    {
      case PlayerSymbol.Blocker:
        powerType = GetRandomPower();
        blockerPrefab.SetActive(true);
        break;
      case PlayerSymbol.P1:
        player1PipePrefab.SetActive(true);
        break;
      case PlayerSymbol.P2:
        player2PipePrefab.SetActive(true);
        break;
      case PlayerSymbol.Walkable:
        WalkerPrefab.SetActive(true);
        break;
    }
  }

  public object Shallowcopy()
  {
    return this.MemberwiseClone();
  }

}

