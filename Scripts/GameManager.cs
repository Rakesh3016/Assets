using Events;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  public List<AbstractController> controllers;
  //public MapGeneratorController mapGenerator;
  //public PlayersController player;
  public Tile[,] tiles;

  [HideInInspector]
  public int noOfPipeGeneratorsLeftForPlayer1 = 1;
  [HideInInspector]
  public int noOfPipeGeneratorsLeftForPlayer2 = 1;

  public Vector2Int player1SelectedTile = new Vector2Int(-1, -1);
  public Vector2Int player2SelectedTile = new Vector2Int(-1, -1);

  public GameObject player1Prefab;
  public GameObject player2Prefab;
  [HideInInspector]
  public Dictionary<GameObject, Vector2Int> player1ActiveGenerators;
  [HideInInspector]
  public Dictionary<GameObject, Vector2Int> player2ActiveGenerators;

  Dictionary<PowerType, int> player1AquiredPowersAndCount;
  Dictionary<PowerType, int> player2AquiredPowersAndCount;

  public Vector2Int mapSize;

  public int seed = 2;
  //public bool shouldGenerateRandomSeed = false;

  public int powerSeed = 0;
  [HideInInspector]
  public string gameId;

  DatabaseReference reference;
  public DataSnapshot dataSnapshotOfMatchMakingData;
  public GameModes CurrentGameMode = GameModes.Offline;

  public int cameraInitialYValue = 6;

  public delegate void timerIncrementEventHandler(int remainingSeconds);
  public event timerIncrementEventHandler incrementPlayer1Timer;
  public event timerIncrementEventHandler incrementPlayer2Timer;

  public TextMeshProUGUI Player1UserName;
  public TextMeshProUGUI Player2UserName;
  public TextMeshProUGUI Player1Rank;
  public TextMeshProUGUI Player2Rank;

  public static GameManager Instance
  {
    get;
    private set;
  }

  //public Vector2Int getMapSize
  //{
  //  get
  //  {
  //    return mapGenerator.mapSize;
  //  }
  //}
  int timer = 0;
  PlayerType myPlayerType = PlayerType.P1;
  void Start()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    timer = maxTimeForTurn;
    CurrentGameMode = (GameModes)PlayerPrefs.GetInt("GameMode");
    if (CurrentGameMode == GameModes.Offline)
    {
      int mapSizeTemp = PlayerPrefs.GetInt("Settings_MapSize", 6);

      mapSize.x = mapSize.y = mapSizeTemp;

      int mapNumberTemp = PlayerPrefs.GetInt("Settings_MapNumber", -1);

      random = new System.Random();
      if (mapNumberTemp == -1)
      {
        seed = random.Next(0, 1000);
      }
      else
      {
        seed = mapNumberTemp;
      }

      //if (shouldGenerateRandomSeed)
      //{
      //}
      powerSeed = random.Next(0, 1000);
      Initialize();
    }
    else
    {
      string gameId = PlayerPrefs.GetString("gameId");
      reference = FirebaseDatabase.DefaultInstance.GetReference("games/" + gameId);
      reference.GetValueAsync().ContinueWith(task =>
      {
        if (task.IsFaulted)
        {
          // Handle the error...
        }
        else if (task.IsCompleted)
        {
          dataSnapshotOfMatchMakingData = task.Result;
          seed = int.Parse(dataSnapshotOfMatchMakingData.Child("seed").Value.ToString());
          powerSeed = int.Parse(dataSnapshotOfMatchMakingData.Child("pSeed").Value.ToString());
          mapSize = new Vector2Int(6, 6);

          if (UserData.user.UserId == dataSnapshotOfMatchMakingData.Child("player1Id").Value.ToString())
          {
            myPlayerType = PlayerType.P1;
            opponentId = dataSnapshotOfMatchMakingData.Child("player2Id").Value.ToString();
            //Player1UserName.text = UserData.user.DisplayName;
            //int[] matchesInfo = UserData.ParseMatchesInfo();
            //Player1Rank.text = matchesInfo[2].ToString();
          }
          else
          {
            myPlayerType = PlayerType.P2;
            opponentId = dataSnapshotOfMatchMakingData.Child("player1Id").Value.ToString();
            //Player2UserName.text = UserData.user.DisplayName;
            //int[] matchesInfo = UserData.ParseMatchesInfo();
            //Player2Rank.text = matchesInfo[2].ToString();
          }

          getOpponentUserData();

        }
      });
    }

  }
  bool shouldInitialize = false;
  public int maxTimeForTurn = 30;
  IEnumerator incrementPlayer1Coroutine;
  IEnumerator incrementPlayer2Coroutine;
  string opponentId = "";
  private void Update()
  {
    if (shouldInitialize)
    {
      shouldInitialize = false;
      Initialize();

      if (CurrentGameMode == GameModes.Online)
      {
        if (myPlayerType == PlayerType.P1)
        {
          Player1UserName.text = "Name : " + UserData.user.DisplayName;
          int[] matchesInfo = UserData.ParseMatchesInfo();
          Player1Rank.text = "Rank : " + matchesInfo[2].ToString();

          Player2UserName.text = "Name : " + OpponentData.user.DisplayName;
          int[] opponentMatchesInfo = OpponentData.ParseMatchesInfo();
          Player2Rank.text = "Rank : " + opponentMatchesInfo[2].ToString();
        }
        else
        {
          Player2UserName.text = "Name : " + UserData.user.DisplayName;
          int[] matchesInfo = UserData.ParseMatchesInfo();
          Player2Rank.text = "Rank : " + matchesInfo[2].ToString();

          Player1UserName.text = "Name : " + OpponentData.user.DisplayName;
          int[] opponentMatchesInfo = OpponentData.ParseMatchesInfo();
          Player1Rank.text = "Rank : " + opponentMatchesInfo[2].ToString();
        }

        incrementPlayer1Coroutine = IncrementTimer(PlayerType.P1);
        incrementPlayer2Coroutine = IncrementTimer(PlayerType.P2);
        StopCoroutine(incrementPlayer1Coroutine);
        StopCoroutine(incrementPlayer2Coroutine);
        timer = maxTimeForTurn;
        StartCoroutine(incrementPlayer1Coroutine);
      }





    }


    //#region CameraResize
    //int width = Screen.width;
    //int height = Screen.height;
    //float screenRatio = Camera.main.aspect;
    //float bestRatio = 0.56f;

    //float cameraYValue = cameraInitialYValue;

    //if (screenRatio <= bestRatio)
    //{
    //  cameraYValue += (1f - screenRatio / bestRatio) / 2f;
    //}
    //else
    //{
    //  cameraYValue += (1f - bestRatio / screenRatio) / 2f;
    //}
    //cameraYValue = cameraYValue + (mapSize.x * 4);


    //Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, cameraYValue, Camera.main.transform.position.z);
    //#endregion
  }
  DatabaseReference referenceOfOpponentData;

  private void getOpponentUserData()
  {
    referenceOfOpponentData = FirebaseDatabase.DefaultInstance.RootReference;
    referenceOfOpponentData = FirebaseDatabase.DefaultInstance.GetReference("Users");
    referenceOfOpponentData.Child(opponentId).GetValueAsync().ContinueWith(task =>
    {
      if (task.IsFaulted)
      {
        // Handle the error...
      }
      else if (task.IsCompleted)
      {
        DataSnapshot UserDataSnapshot = task.Result;
        OpponentData.currentState = State.Open;

        if (UserDataSnapshot.Value == null)
        {
        }
        else
        {
          OpponentData.matchesInfo = UserDataSnapshot.Child("MatchesInfo").Value.ToString();
          OpponentData.user = new DummyFirebaseUser(opponentId, UserDataSnapshot.Child("uname").Value.ToString());
        }

        shouldInitialize = true;

        //userCreated.Invoke();
      }
    });
  }


  private IEnumerator IncrementTimer(PlayerType playerType)
  {
    while (true)
    {
      yield return new WaitForSeconds(1f);
      UpdateTimer(playerType);
    }
  }

  private void UpdateTimer(PlayerType playerType)
  {
    if (timer <= 0)
    {
      Utils.EventAsync(new Events.OutOfTime(onClickedOutOfTimeClose));
    }
    timer--;
    if (playerType == PlayerType.P1)
      incrementPlayer1Timer?.Invoke(timer);
    else
      incrementPlayer2Timer?.Invoke(timer);

  }

  private void onClickedOutOfTimeClose()
  {
    if (currentPlayerTurn == PlayerType.P1)
      Utils.EventAsync(new Events.GameOverEvent(PlayerType.P2));
    else
      Utils.EventAsync(new Events.GameOverEvent(PlayerType.P1));
  }

  void Initialize()
  {
    foreach (IController controller in controllers)
    {
      controller.Initialize();
      controller.RegisterEvents();
    }
    playGameInitialize();
  }

  public void playGameInitialize()
  {
    player1ActiveGenerators = new Dictionary<GameObject, Vector2Int>();
    player2ActiveGenerators = new Dictionary<GameObject, Vector2Int>();

    player1AquiredPowersAndCount = new Dictionary<PowerType, int>();
    player2AquiredPowersAndCount = new Dictionary<PowerType, int>();

    #region TestingValues
    seed = 2;
    powerSeed = 604;
    #endregion

    #region CameraResize
    int width = Screen.width;
    int height = Screen.height;
    float screenRatio = Camera.main.aspect;
    float bestRatio = 0.56f;

    float cameraYValue = cameraInitialYValue;

    if (screenRatio <= bestRatio)
    {
      Debug.Log("Before " + cameraYValue);
      float temp = (1f - screenRatio / bestRatio) * 40f;
      cameraYValue += temp;
      Debug.Log("After " + cameraYValue);

    }
    else
    {
      cameraYValue += (1f - bestRatio / screenRatio);
    }
    cameraYValue = cameraYValue + (mapSize.x * 4);


    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, cameraYValue, Camera.main.transform.position.z);
    #endregion

    Utils.EventAsync(new Events.GenerateMapEvent(mapSize, seed, powerSeed, onGetGeneratedTiles));
    Utils.EventAsync(new Events.GetMapSize(onGetMapSize));

    RegisterEvents();
    if (player1AquiredPowersAndCount.ContainsKey(PowerType.Spawner))
    {
      player1AquiredPowersAndCount[PowerType.Spawner] = 1;
    }
    else
    {
      player1AquiredPowersAndCount.Add(PowerType.Spawner, 1);
    }
    if (player1AquiredPowersAndCount.ContainsKey(PowerType.Spawner))
    {
      player2AquiredPowersAndCount[PowerType.Spawner] = 1;
    }
    else
    {
      player2AquiredPowersAndCount.Add(PowerType.Spawner, 1);
    }
    Utils.EventAsync(new Events.UserAquiredPower(PlayerType.P1, PowerType.Spawner, 1));
    Utils.EventAsync(new Events.UserAquiredPower(PlayerType.P2, PowerType.Spawner, 1));
  }

  private void RegisterEvents()
  {
    EventManager.Instance.RemoveListener<SceneChangingTo>(onSceneChangingTo);

    EventManager.Instance.AddListener<PlayerTurnChangedTo>(playerTurnChanged);
    EventManager.Instance.AddListener<UserUsedSelectedPower>(onUserUsedSelectedPower);
    EventManager.Instance.AddListener<SceneChangingTo>(onSceneChangingTo);
  }
  private void UnregisterEvents()
  {
    EventManager.Instance.RemoveListener<PlayerTurnChangedTo>(playerTurnChanged);
    EventManager.Instance.RemoveListener<UserUsedSelectedPower>(onUserUsedSelectedPower);
    //EventManager.Instance.RemoveListener<SceneChangingTo>(onSceneChangingTo);
  }
  private void onSceneChangingTo(SceneChangingTo e)
  {
    UnregisterEvents();
    foreach (IController controller in controllers)
    {
      controller.UnRegisterEvents();
    }
  }

  private void onUserUsedSelectedPower(UserUsedSelectedPower e)
  {
    if (e.type == PlayerType.P1)
    {
      if (player1AquiredPowersAndCount.ContainsKey(e.usedPower))
      {
        player1AquiredPowersAndCount[e.usedPower] = player1AquiredPowersAndCount[e.usedPower] - 1;
      }
    }
    else
    {
      if (player2AquiredPowersAndCount.ContainsKey(e.usedPower))
      {
        player2AquiredPowersAndCount[e.usedPower] = player2AquiredPowersAndCount[e.usedPower] - 1;
      }
    }
  }
  private void onGetMapSize(Vector2Int obj)
  {
    mapSize = obj;
  }

  private void onGetGeneratedTiles(Tile[,] obj)
  {
    tiles = obj;
  }

  public void AddedNewPipeGenerator(Transform startingPipe, PlayerType playerType)
  {
    StartingPipes startingPipes = startingPipe.GetComponent<StartingPipes>();
    Vector2Int position = startingPipes.GetFrontTileIndex;
    if (playerType == PlayerType.P1 && noOfPipeGeneratorsLeftForPlayer1 > 0)
    {
      GameObject player1Instance = Instantiate(player1Prefab, startingPipe.position, Quaternion.identity);
      Player player = player1Instance.GetComponent<Player>();
      player1ActiveGenerators.Add(player1Instance, position);
      noOfPipeGeneratorsLeftForPlayer1--;

      NextTileOperations(playerType, PlayerSymbol.P1, position, position);

      //player.MoveTowards(new Vector3(tiles[position.x, position.y].transform.position.x, player.transform.position.y, tiles[position.x, position.y].transform.position.z));
      //player1SelectedTile = position;// hit.collider.transform.GetComponent<Tile>().position;
      //tiles[position.x, position.y].setPlayer1Data();
    }
    else if (playerType == PlayerType.P2 && noOfPipeGeneratorsLeftForPlayer2 > 0)
    {
      GameObject player2Instance = Instantiate(player2Prefab, startingPipe.position, Quaternion.identity);
      Player player = player2Instance.GetComponent<Player>();
      player2ActiveGenerators.Add(player2Instance, position);
      noOfPipeGeneratorsLeftForPlayer2--;

      NextTileOperations(playerType, PlayerSymbol.P2, position, position);
      //player.MoveTowards(new Vector3(tiles[position.x, position.y].transform.position.x, player.transform.position.y, tiles[position.x, position.y].transform.position.z));
      //player2SelectedTile = position;// hit.collider.transform.GetComponent<Tile>().position;
      //tiles[position.x, position.y].setPlayer2Data();
    }
    //playerTurnChanged(playerType);

  }
  public void Swipe(SwipeDirection swipeDirection, PlayerType turn)
  {
    //Debug.Log("Button Down Pressed");

    PlayerSymbol currentPlayerSymbol;
    Vector2Int currentPosition;
    if (turn == PlayerType.P1)
    {
      currentPlayerSymbol = PlayerSymbol.P1;
      currentPosition = player1SelectedTile;
    }
    else
    {
      currentPlayerSymbol = PlayerSymbol.P2;
      currentPosition = player2SelectedTile;
    }
    //Tile[,] previousStateTileArray = new Tile[mapGenerator.mapSize.x, mapGenerator.mapSize.y];
    //for (int x = 0; x < mapGenerator.mapSize.x; x++)
    //{
    //  for (int y = 0; y < mapGenerator.mapSize.y; y++)
    //  {
    //    previousStateTileArray[x, y] = (Tile)tiles[x, y].Shallowcopy();
    //  }
    //}
    //Tile[,] previousStateTileArray = Utils.DeepClone<Tile[,]>(tiles);
    //Tile[,] previousStateTileArray = new Tile[mapGenerator.mapSize.x, mapGenerator.mapSize.y];
    //Array.Copy(tiles, previousStateTileArray, tiles.Length);
    //Tile[,] previousStateTileArray = tiles.Clone() as Tile[,];

    //for (int x = 0; x < mapGenerator.mapSize.x; x++)
    //{
    //  for (int y = 0; y < mapGenerator.mapSize.y; y++)
    //  {
    if (tiles[currentPosition.x, currentPosition.y].tileType == currentPlayerSymbol)
    {
      Vector2Int NextTilePosition = Virus.Utils.GetNextTile(new Vector2Int(currentPosition.x, currentPosition.y), swipeDirection, mapSize.x);

      if (NextTilePosition.x != -1 || NextTilePosition.y != -1)
      {
        tiles[currentPosition.x, currentPosition.y].containsP1PipeGenerator = false;
        tiles[currentPosition.x, currentPosition.y].containsP2PipeGenerator = false;
        NextTileOperations(turn, currentPlayerSymbol, currentPosition, NextTilePosition);

        //Check For Game End Condition
        if (GameOverCheck.CheckGameOverConditions(tiles, turn, mapSize.x))
        {
          Utils.EventAsync(new Events.GameOverEvent(turn));

          Debug.LogError("--------------------Dude Game Over------------------------");
          Debug.Log("--------------------Dude Game Over------------------------");
        }
      }
    }

    //  }
    //}
  }

  private void NextTileOperations(PlayerType turn, PlayerSymbol currentPlayerSymbol, Vector2Int currentPosition, Vector2Int NextTilePosition)
  {
    if (tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.Walkable)
    {
      if (turn == PlayerType.P1)
      {
        setPlayersCommonFunctionality(currentPosition, NextTilePosition, turn);
        tiles[NextTilePosition.x, NextTilePosition.y].setPlayer1Data();
      }
      else
      {
        setPlayersCommonFunctionality(currentPosition, NextTilePosition, turn);

        tiles[NextTilePosition.x, NextTilePosition.y].setPlayer2Data();
      }
    }
    else if (tiles[NextTilePosition.x, NextTilePosition.y].tileType == PlayerSymbol.Blocker)
    {
      if (turn == PlayerType.P1)
      {
        if (player1AquiredPowersAndCount.ContainsKey(tiles[NextTilePosition.x, NextTilePosition.y].powerType))
        {
          player1AquiredPowersAndCount[tiles[NextTilePosition.x, NextTilePosition.y].powerType] = player1AquiredPowersAndCount[tiles[NextTilePosition.x, NextTilePosition.y].powerType] + 1;
        }
        else
        {
          player1AquiredPowersAndCount.Add(tiles[NextTilePosition.x, NextTilePosition.y].powerType, 1);
        }
        if (tiles[NextTilePosition.x, NextTilePosition.y].powerType == PowerType.Spawner)
        {
          noOfPipeGeneratorsLeftForPlayer1++;
        }
        Utils.EventAsync(new Events.UserAquiredPower(turn, tiles[NextTilePosition.x, NextTilePosition.y].powerType, player1AquiredPowersAndCount[tiles[NextTilePosition.x, NextTilePosition.y].powerType]));

        tiles[NextTilePosition.x, NextTilePosition.y].setPlayer1Data();
      }
      else
      {
        if (player2AquiredPowersAndCount.ContainsKey(tiles[NextTilePosition.x, NextTilePosition.y].powerType))
        {
          player2AquiredPowersAndCount[tiles[NextTilePosition.x, NextTilePosition.y].powerType] = player2AquiredPowersAndCount[tiles[NextTilePosition.x, NextTilePosition.y].powerType] + 1;
        }
        else
        {
          player2AquiredPowersAndCount.Add(tiles[NextTilePosition.x, NextTilePosition.y].powerType, 1);
        }
        if (tiles[NextTilePosition.x, NextTilePosition.y].powerType == PowerType.Spawner)
        {
          noOfPipeGeneratorsLeftForPlayer2++;
        }
        Utils.EventAsync(new Events.UserAquiredPower(turn, tiles[NextTilePosition.x, NextTilePosition.y].powerType, player2AquiredPowersAndCount[tiles[NextTilePosition.x, NextTilePosition.y].powerType]));

        tiles[NextTilePosition.x, NextTilePosition.y].setPlayer2Data();
      }
      tiles[NextTilePosition.x, NextTilePosition.y].powerType = PowerType.None;
      setPlayersCommonFunctionality(currentPosition, NextTilePosition, turn);
    }
    else if (tiles[NextTilePosition.x, NextTilePosition.y].tileType != currentPlayerSymbol)
    {
      DestroyIfTheTileHasOppositePlayer(turn, NextTilePosition);
      //tiles[NextTilePosition.x, NextTilePosition.y].setWalkableData();
      setPlayersCommonFunctionality(currentPosition, NextTilePosition, turn);
      if (turn == PlayerType.P1)
      {
        tiles[NextTilePosition.x, NextTilePosition.y].setPlayer1Data();
      }
      else
      {
        tiles[NextTilePosition.x, NextTilePosition.y].setPlayer2Data();
      }
    }
    else if (tiles[NextTilePosition.x, NextTilePosition.y].tileType == currentPlayerSymbol)
    {
      setPlayersCommonFunctionality(currentPosition, NextTilePosition, turn);
    }
  }
  PlayerType currentPlayerTurn = PlayerType.P1;
  private void playerTurnChanged(PlayerTurnChangedTo playerTurnChanged)
  {
    currentPlayerTurn = playerTurnChanged.playerType;
    if (CurrentGameMode == GameModes.Online)
    {
      timer = maxTimeForTurn;
      if (playerTurnChanged.playerType == PlayerType.P1)
      {
        StopCoroutine(incrementPlayer2Coroutine);
        StartCoroutine(incrementPlayer1Coroutine);
      }
      else
      {
        StopCoroutine(incrementPlayer1Coroutine);
        StartCoroutine(incrementPlayer2Coroutine);
      }
    }


    foreach (Tile tempTile in tiles)
    {
      if (tempTile.tileType == PlayerSymbol.Walkable)
      {
        tempTile.setPlayerIndicator(playerTurnChanged.playerType);
      }
    }
  }

  private void DestroyIfTheTileHasOppositePlayer(PlayerType turn, Vector2Int nextTilePosition)
  {
    if (turn == PlayerType.P1)
    {
      GameObject seletedPipesGenerator = null;
      foreach (KeyValuePair<GameObject, Vector2Int> b in player2ActiveGenerators)
      {
        if (b.Value == nextTilePosition)
        {
          seletedPipesGenerator = b.Key;
          break;
        }
      }
      if (seletedPipesGenerator != null)
      {
        player2ActiveGenerators.Remove(seletedPipesGenerator);
        Destroy(seletedPipesGenerator);
        int remainingSpawners = 0;
        if (player2AquiredPowersAndCount.ContainsKey(PowerType.Spawner))
        {
          remainingSpawners = player2AquiredPowersAndCount[PowerType.Spawner];
        }
        if (player2ActiveGenerators.Count <= 0 && remainingSpawners <= 0)
        {
          Utils.EventAsync(new Events.GameOverEvent(turn));
        }
      }
    }
    else
    {
      GameObject seletedPipesGenerator = null;
      foreach (KeyValuePair<GameObject, Vector2Int> b in player1ActiveGenerators)
      {
        if (b.Value == nextTilePosition)
        {
          seletedPipesGenerator = b.Key;
          break;
        }
      }
      if (seletedPipesGenerator != null)
      {
        player1ActiveGenerators.Remove(seletedPipesGenerator);
        Destroy(seletedPipesGenerator);
        int remainingSpawners = 0;
        if (player1AquiredPowersAndCount.ContainsKey(PowerType.Spawner))
        {
          remainingSpawners = player1AquiredPowersAndCount[PowerType.Spawner];
        }
        if (player1ActiveGenerators.Count <= 0 && remainingSpawners <= 0)
        {
          Utils.EventAsync(new Events.GameOverEvent(turn));
        }
      }
    }
  }

  public void setPlayersCommonFunctionality(Vector2Int currentPosition, Vector2Int NextTilePosition, PlayerType turn)
  {
    if (turn == PlayerType.P1)
    {
      GameObject seletedPipesGenerator = null;
      foreach (KeyValuePair<GameObject, Vector2Int> b in player1ActiveGenerators)
      {
        if (b.Value == currentPosition)
        {
          seletedPipesGenerator = b.Key;
          break;
        }
      }
      if (seletedPipesGenerator != null)
      {

        Player player = seletedPipesGenerator.GetComponent<Player>();
        player.MoveTowards(new Vector3(tiles[NextTilePosition.x, NextTilePosition.y].transform.position.x, player.transform.position.y, tiles[NextTilePosition.x, NextTilePosition.y].transform.position.z));
        player1ActiveGenerators[seletedPipesGenerator] = NextTilePosition;
      }
      player1SelectedTile = NextTilePosition;
    }
    else
    {
      GameObject seletedPipesGenerator = null;
      foreach (KeyValuePair<GameObject, Vector2Int> b in player2ActiveGenerators)
      {
        if (b.Value == currentPosition)
        {
          seletedPipesGenerator = b.Key;
          break;
        }
      }
      if (seletedPipesGenerator != null)
      {

        Player player = seletedPipesGenerator.GetComponent<Player>();
        player.MoveTowards(new Vector3(tiles[NextTilePosition.x, NextTilePosition.y].transform.position.x, player.transform.position.y, tiles[NextTilePosition.x, NextTilePosition.y].transform.position.z));
        player2ActiveGenerators[seletedPipesGenerator] = NextTilePosition;
      }
      tiles[NextTilePosition.x, NextTilePosition.y].setPlayer2Data();
      player2SelectedTile = NextTilePosition;
    }
  }

  System.Random random = new System.Random();
  public int GetRandomNumber(int min, int max)
  {
    return random.Next(min, max);
  }

}
