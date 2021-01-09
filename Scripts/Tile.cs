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
  public GameObject player1Pipe4Prefab;
  public GameObject player2Pipe4Prefab;
  public GameObject player1Pipe3Prefab;
  public GameObject player2Pipe3Prefab;
  public GameObject player1Pipe2Prefab;
  public GameObject player2Pipe2Prefab;
  public GameObject player1Pipe1Prefab;
  public GameObject player2Pipe1Prefab;
  public GameObject WalkerPrefab;
  public Material WalkableMaterialPlayer1;
  public Material WalkableMaterialPlayer2;
  public bool containsP1PipeGenerator;
  public bool containsP2PipeGenerator;
  private Renderer renderer;

  public PowerType powerType;

  public bool isTilePlayer1EndGoal = false;
  public bool isTilePlayer2EndGoal = false;

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

  public void setInitialData(PlayerSymbol playerSymbol, PowerType powerType, Vector2Int position)
  {
    //if (playerSymbol == PlayerSymbol.Walkable)
    //{
    //  //this.transform.tag = "Walkable";
    //  renderer.material = walkableMaterial;
    //}
    //else if (playerSymbol == PlayerSymbol.Blocker)
    //  renderer.material = blockerMaterial;

    tileType = playerSymbol;
    this.powerType = powerType;
    activateValidPrefab(tileType);

    this.position = position;
  }

  public void setPlayer1Data()
  {
    //renderer.material = player1Material;
    tileType = PlayerSymbol.P1;
    containsP1PipeGenerator = true;
    containsP2PipeGenerator = false;
    activateValidPrefab(tileType);

  }
  public void setWalkableData()
  {
    //renderer.material = walkableMaterial;
    tileType = PlayerSymbol.Walkable;
    containsP1PipeGenerator = false;
    containsP2PipeGenerator = false;
    activateValidPrefab(tileType);
  }

  public void setPlayer2Data()
  {
    //renderer.material = player2Material;
    tileType = PlayerSymbol.P2;
    containsP1PipeGenerator = false;
    containsP2PipeGenerator = true;
    activateValidPrefab(tileType);
  }

  private PowerType GetRandomPower()
  {
    int randomNumber = GameManager.Instance.GetRandomNumber(1, Enum.GetValues(typeof(PowerType)).Length);
    Debug.Log("rand 1,3 " + randomNumber);
    return (PowerType)randomNumber;
  }

  private void activateValidPrefab(PlayerSymbol tileType)
  {
    blockerPrefab.SetActive(false);
    player1Pipe4Prefab.SetActive(false);
    player2Pipe4Prefab.SetActive(false);
    WalkerPrefab.SetActive(false);

    switch (tileType)
    {
      case PlayerSymbol.Blocker:
        //powerType = GetRandomPower();
        blockerPrefab.SetActive(true);
        break;
      case PlayerSymbol.P1:
        //player1Pipe4Prefab.SetActive(true);
        break;
      case PlayerSymbol.P2:
        // player2Pipe4Prefab.SetActive(true);
        break;
      case PlayerSymbol.Walkable:
        WalkerPrefab.SetActive(true);
        break;
    }
  }

  public void SetAppropriatePipeGenerator()
  {
    bool isRightTileMine = isTileMine(SwipeDirection.Right);
    bool isLeftTileMine = isTileMine(SwipeDirection.Left);
    bool isFrontTileMine = isTileMine(SwipeDirection.Up);
    bool isBackTileMine = isTileMine(SwipeDirection.Down);

    if (isLeftTileMine || isRightTileMine || isFrontTileMine || isBackTileMine)
      ResetAllPipes();

    if (isLeftTileMine && isRightTileMine && isFrontTileMine && isBackTileMine)
    {
      Set4WayPipe();
    }
    else if (isLeftTileMine && isRightTileMine && isFrontTileMine)
    {
      Set3WayPipe(0);
    }
    else if (isLeftTileMine && isBackTileMine && isFrontTileMine)
    {
      Set3WayPipe(-90);
    }
    else if (isRightTileMine && isBackTileMine && isFrontTileMine)
    {
      Set3WayPipe(90);
    }
    else if (isRightTileMine && isBackTileMine && isLeftTileMine)
    {
      Set3WayPipe(180);
    }
    else if (isRightTileMine && isFrontTileMine)
    {
      // FlipHorizontal flipVertical assuming it is in L Shape
      Set2WayPipe(0);
    }
    else if (isFrontTileMine && isLeftTileMine)
    {
      //Flip Horizontal
      Set2WayPipe(270);
    }
    else if (isBackTileMine && isLeftTileMine)
    {
      Set2WayPipe(180);
    }
    else if (isBackTileMine && isRightTileMine)
    {
      //FlipVertical
      Set2WayPipe(90);
    }
    else if (isLeftTileMine && isRightTileMine)
    {
      Set1WayPipe(0);
    }
    else if (isBackTileMine && isFrontTileMine)
    {
      Set1WayPipe(90);
    }
    else if (isBackTileMine)
    {
      Set1WayPipe(90);
    }
    else if (isLeftTileMine)
    {
      Set1WayPipe(0);
    }
    else if (isRightTileMine)
    {
      Set1WayPipe(0);
    }
    else if (isFrontTileMine)
    {
      Set1WayPipe(90);
    }

  }



  private void ResetAllPipes()
  {
    player1Pipe4Prefab.SetActive(false);
    player2Pipe4Prefab.SetActive(false);
    player1Pipe3Prefab.SetActive(false);
    player2Pipe3Prefab.SetActive(false);
    player1Pipe2Prefab.SetActive(false);
    player2Pipe2Prefab.SetActive(false);
    player1Pipe1Prefab.SetActive(false);
    player2Pipe1Prefab.SetActive(false);
  }

  private void Set1WayPipe(int Rotation)
  {
    if (tileType == PlayerSymbol.P1)
    {
      player1Pipe1Prefab.SetActive(true);
      player1Pipe1Prefab.transform.localRotation =Quaternion.Euler( new Vector3(0, Rotation, 0));
    }
    else if (tileType == PlayerSymbol.P2)
    {
      player2Pipe1Prefab.SetActive(true);
      player2Pipe1Prefab.transform.localRotation = Quaternion.Euler(new Vector3(0, Rotation, 0));
    }
  }
  private void Set2WayPipe(int Rotation)
  {
    if (tileType == PlayerSymbol.P1)
    {
      player1Pipe2Prefab.SetActive(true);
      player1Pipe2Prefab.transform.localRotation = Quaternion.Euler(new Vector3(0, Rotation, 0));
    }
    else if (tileType == PlayerSymbol.P2)
    {
      player2Pipe2Prefab.SetActive(true);
      player2Pipe2Prefab.transform.localRotation = Quaternion.Euler(new Vector3(0, Rotation, 0));
    }
  }

  private void Set3WayPipe(int Rotation)
  {
    if (tileType == PlayerSymbol.P1)
    {
      player1Pipe3Prefab.SetActive(true);
      player1Pipe3Prefab.transform.localRotation = Quaternion.Euler(new Vector3(0, Rotation, 0));
    }
    else if (tileType == PlayerSymbol.P2)
    {
      player2Pipe3Prefab.SetActive(true);
      player2Pipe3Prefab.transform.localRotation = Quaternion.Euler(new Vector3(0, Rotation, 0));
    }
  }
  private void Set4WayPipe()
  {
    if (tileType == PlayerSymbol.P1)
    {
      player1Pipe4Prefab.SetActive(true);
    }
    else if (tileType == PlayerSymbol.P2)
    {
      player2Pipe4Prefab.SetActive(true);
    }
  }
  bool isTileMine(SwipeDirection swipeDirection)
  {
    Vector2Int NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(position.x, position.y), swipeDirection, GameManager.Instance.mapSize.x);
    if (NextTilePosition.x != -1 || NextTilePosition.y != -1)
    {
      if (tileType == PlayerSymbol.P1)
      {
        if (GameManager.Instance.tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.P1)
        {
          return true;
        }
      }
      else if (tileType == PlayerSymbol.P2)
      {
        if (GameManager.Instance.tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.P2)
        {
          return true;
        }
      }

    }
    else
    {
      //ItIsStartingPipe
      if (tileType == PlayerSymbol.P1)
      {
        //this if is for cornors
        if (position.x == 0 || position.x == GameManager.Instance.mapSize.x - 1)
        {
          if (swipeDirection == SwipeDirection.Left || swipeDirection == SwipeDirection.Right)
            return false;
          else
            return true;
        }
        else
          return true;
      }
      else if (tileType == PlayerSymbol.P2)
      {
        if (position.x == 0 || position.x == GameManager.Instance.mapSize.x - 1)
        {
          if (swipeDirection == SwipeDirection.Down || swipeDirection == SwipeDirection.Up)
            return false;
          else
            return true;
        }
        else
          return true;
      }
    }
    return false;
  }

  public object Shallowcopy()
  {
    return this.MemberwiseClone();
  }

}

