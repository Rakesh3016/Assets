using Events;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : AbstractController
{
  public Transform Player1PowerParent;
  public Transform Player2PowerParent;

  public GameObject player1PowerBlocker;
  public GameObject player2PowerBlocker;

  public TextMeshProUGUI turnText;
  public Image turnImage;

  //public PowerToggleWidget PowerUpToggle;

  [SerializeField]
  PowerPrefabDictionary player1AquiredPowersAndCountSerilized;


  public GameObject gameOver;
  public GameObject disConnected;
  public Button disConnectedCloseButton;
  public Button MenuButton;
  public Text interuptionText;
  public Text playerWonText;

  public IDictionary<PowerType, PowerToggleWidget> Player1PowersAndCount
  {
    get { return player1AquiredPowersAndCountSerilized; }
    set { player1AquiredPowersAndCountSerilized.CopyFrom(value); }
  }

  [SerializeField]
  PowerPrefabDictionary player2AquiredPowersAndCountSerilized;

  public IDictionary<PowerType, PowerToggleWidget> Player2PowersAndCount
  {
    get { return player2AquiredPowersAndCountSerilized; }
    set { player2AquiredPowersAndCountSerilized.CopyFrom(value); }
  }
  //Dictionary<PowerType, PowerToggleWidget> player1AquiredPowersAndCount;
  //Dictionary<PowerType, PowerToggleWidget> player2AquiredPowersAndCount;

  public override void Initialize()
  {
    //this.gameObject.InvokeMethodInChildrenAndSelf<Button>(x => x.onClick.AddListener(() => onClick(x.name)));


    //Button[] AllButtons = GetComponentsInChildren<Button>();
    //foreach (Button btn in AllButtons)
    //{
    //  btn.onClick.AddListener(() => );
    //}
    foreach (KeyValuePair<PowerType, PowerToggleWidget> Player1Power in player1AquiredPowersAndCountSerilized)
    {
      Player1Power.Value.toggle.onValueChanged.AddListener(delegate
          {
            onChangePowerToggle(Player1Power.Value, Player1Power.Value.toggle.isOn, PlayerType.P1);
          });
    }
    foreach (KeyValuePair<PowerType, PowerToggleWidget> Player2Power in player2AquiredPowersAndCountSerilized)
    {
      Player2Power.Value.toggle.onValueChanged.AddListener(delegate
      {
        onChangePowerToggle(Player2Power.Value, Player2Power.Value.toggle.isOn, PlayerType.P2);
      });
    }
    //player1AquiredPowersAndCount = new Dictionary<PowerType, PowerToggleWidget>();
    //player2AquiredPowersAndCount = new Dictionary<PowerType, PowerToggleWidget>();
  }


  private void OnIncrementTimer(int remainingSeconds)
  {
    float value = map(remainingSeconds, 0, GameManager.Instance.maxTimeForTurn, 0, 1);
    turnImage.fillAmount = value;
  }

  private float map(float n, float start1, float stop1, float start2, float stop2)
  {
    return ((n - start1) / (stop1 - start1)) * (stop2 - start2) + start2;
  }

  Action onCLoseClicked = null;
  private void onClick(string btn)
  {
    switch (btn)
    {
      case "Close_Button":
        disConnected.SetActive(false);
        onCLoseClicked.Invoke();
        break;
      case "Menu":
        Utils.EventAsync(new SceneChangingTo("Menu"));
        SceneManager.LoadScene("Menu");
        break;
    }
  }

  public override void RegisterEvents()
  {
    EventManager.Instance.AddListener<UserAquiredPower>(onUserGotNewPower);
    EventManager.Instance.AddListener<PlayerTurnChangedTo>(onPlayerTurnChanged);
    EventManager.Instance.AddListener<UserUsedSelectedPower>(onUserUsedSelectedPower);
    EventManager.Instance.AddListener<GameOverEvent>(onGameOverEvent);
    EventManager.Instance.AddListener<OpponentDisconnected>(onOpponentDisconnected);
    EventManager.Instance.AddListener<OutOfTime>(onOutOfTime);

    disConnectedCloseButton.onClick.AddListener(() => onClick(disConnectedCloseButton.name));
    MenuButton.onClick.AddListener(() => onClick(MenuButton.name));
    GameManager.Instance.incrementPlayer1Timer += OnIncrementTimer;
    GameManager.Instance.incrementPlayer2Timer += OnIncrementTimer;
  }


  public override void UnRegisterEvents()
  {
    EventManager.Instance.RemoveListener<UserAquiredPower>(onUserGotNewPower);
    EventManager.Instance.RemoveListener<PlayerTurnChangedTo>(onPlayerTurnChanged);
    EventManager.Instance.RemoveListener<UserUsedSelectedPower>(onUserUsedSelectedPower);
    EventManager.Instance.RemoveListener<GameOverEvent>(onGameOverEvent);
    EventManager.Instance.RemoveListener<OpponentDisconnected>(onOpponentDisconnected);
    EventManager.Instance.RemoveListener<OutOfTime>(onOutOfTime);
    disConnectedCloseButton.onClick.RemoveAllListeners();
    MenuButton.onClick.RemoveAllListeners();
    GameManager.Instance.incrementPlayer1Timer -= OnIncrementTimer;
    GameManager.Instance.incrementPlayer2Timer -= OnIncrementTimer;
  }

  private void onOutOfTime(OutOfTime e)
  {
    interuptionText.text = "Time Out";
    onCLoseClicked = e.onClose;
    disConnected.SetActive(true);
  }

  private void onOpponentDisconnected(OpponentDisconnected e)
  {
    interuptionText.text = "Opponent Disconnected";
    onCLoseClicked = e.onClose;
    disConnected.SetActive(true);
  }

  private void onGameOverEvent(GameOverEvent e)
  {
    gameOver.SetActive(true);
    if (e.playerWon == PlayerType.P1)
      playerWonText.text = "Player 1 Won";
    else
      playerWonText.text = "Player 2 Won";

  }

  private void onPlayerTurnChanged(PlayerTurnChangedTo e)
  {
    if (e.playerType == PlayerType.P1)
    {
      player1PowerBlocker.SetActive(false);
      player2PowerBlocker.SetActive(true);
      turnText.text = "P1";
      turnImage.color = new Color(1, 0.4f, 0.27f, 1);
    }
    else
    {
      player1PowerBlocker.SetActive(true);
      player2PowerBlocker.SetActive(false);
      turnText.text = "P2";
      turnImage.color = new Color(0, 0.58f, 1, 1);


    }
  }

  private void onUserUsedSelectedPower(UserUsedSelectedPower e)
  {
    if (e.type == PlayerType.P1)
    {
      PowerToggleWidget powerToggleWidget = Player1PowersAndCount[e.usedPower];
      powerToggleWidget.UserUsedThePower();
    }
    else
    {
      PowerToggleWidget powerToggleWidget = Player2PowersAndCount[e.usedPower];
      powerToggleWidget.UserUsedThePower();
    }
  }


  private void onUserGotNewPower(UserAquiredPower e)
  {
    if (e.playerWhoAquiredPower == PlayerType.P1)
    {
      if (Player1PowersAndCount.ContainsKey(e.powerType))
      {
        Player1PowersAndCount[e.powerType].SetPowerToggleWidgetData(e.powerType, e.countOfPowerType);
      }
      else
      {
        //  PowerToggleWidget powerToggle = Instantiate(PowerUpToggle, Player1PowerParent);
        //  powerToggle.SetPowerToggleWidgetData(e.powerType, e.countOfPowerType);
        //  powerToggle.toggle.onValueChanged.AddListener(delegate
        //  {
        //    onChangePowerToggle(powerToggle, powerToggle.toggle.isOn);
        //  });
        //  player1AquiredPowersAndCount.Add(e.powerType, powerToggle);
      }
    }
    else
    {
      if (Player2PowersAndCount.ContainsKey(e.powerType))
      {
        Player2PowersAndCount[e.powerType].SetPowerToggleWidgetData(e.powerType, e.countOfPowerType);
      }
      else
      {
        //PowerToggleWidget powerToggle = Instantiate(PowerUpToggle, Player2PowerParent);
        //powerToggle.SetPowerToggleWidgetData(e.powerType, e.countOfPowerType);
        //player2AquiredPowersAndCount.Add(e.powerType, powerToggle);
      }
    }
    //Instantiate()
  }

  private void onChangePowerToggle(PowerToggleWidget powerToggle, bool isOn, PlayerType playerType)
  {
    Utils.EventAsync(new Events.UserSelectedPower(powerToggle.powerType, isOn, playerType));
  }
}
