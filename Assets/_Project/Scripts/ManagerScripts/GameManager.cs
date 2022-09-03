using Cinemachine;
using Demonics.Manager;
using Demonics.Sounds;
using Demonics.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[Header("Debug")]
	[SerializeField] private StageTypeEnum _stage = default;
	[SerializeField] private CharacterTypeEnum _characterOne = default;
	[SerializeField] private CharacterTypeEnum _characterTwo = default;
	[SerializeField] private AssistTypeEnum _assistOne = default;
	[SerializeField] private AssistTypeEnum _assistTwo = default;
	[Range(-1, 3)]
	[SerializeField] private int _controllerOne = default;
	[SerializeField] private ControllerTypeEnum _controllerOneType = default;
	[Range(-1, 3)]
	[SerializeField] private int _controllerTwo = default;
	[SerializeField] private ControllerTypeEnum _controllerTwoType = default;
	[SerializeField] private MusicTypeEnum _music = default;
	[Range(0, 10)]
	[SerializeField] private int _playerOneSkin = default;
	[Range(0, 10)]
	[SerializeField] private int _playerTwoSkin = default;
	[SerializeField] private bool _isTrainingMode = default;
	[SerializeField] private bool _isOnlineMode = default;
	[SerializeField] private bool _1BitOn = default;
	[Range(1, 10)]
	[SerializeField] private int _gameSpeed = 1;
	[Range(10, 300)]
	[SerializeField] private float _countdownTime = 99.0f;
	[Header("Data")]
	[SerializeField] private Transform[] _spawnPositions = default;
	[SerializeField] private IntroUI _introUI = default;
	[SerializeField] private FadeHandler _fadeHandler = default;
	[SerializeField] protected PlayerUI _playerOneUI = default;
	[SerializeField] protected PlayerUI _playerTwoUI = default;
	[SerializeField] private PlayerDialogue _playerOneDialogue = default;
	[SerializeField] private PlayerDialogue _playerTwoDialogue = default;
	[SerializeField] private Animator _timerAnimator = default;
	[SerializeField] private Animator _timerMainAnimator = default;
	[SerializeField] private Animator _introAnimator = default;
	[SerializeField] protected TextMeshProUGUI _countdownText = default;
	[SerializeField] protected TextMeshProUGUI _readyText = default;
	[SerializeField] protected TextMeshProUGUI _winnerNameText = default;
	[SerializeField] protected TextMeshProUGUI _winsText = default;
	[SerializeField] protected GameObject _keyboardPrompts = default;
	[SerializeField] protected GameObject _controllerPrompts = default;
	[SerializeField] protected GameObject _xboxPrompts = default;
	[SerializeField] protected GameObject[] _readyObjects = default;
	[SerializeField] protected GameObject[] _arcanaObjects = default;
	[SerializeField] protected GameObject _playerLocal = default;
	[SerializeField] protected GameObject _playerNetcode = default;
	[SerializeField] protected GameObject _infiniteTime = default;
	[SerializeField] protected GameObject _winsImage = default;
	[SerializeField] private GameObject[] _hearts = default;
	[SerializeField] private GameObject _trainingPrompts = default;
	[SerializeField] private MusicMenu _musicMenu = default;
	[SerializeField] private InputHistory[] _inputHistories = default;
	[SerializeField] private PlayerStatsSO[] _playerStats = default;
	[SerializeField] private TrainingMenu _trainingMenu = default;
	[SerializeField] private GameObject[] _stages = default;
	[SerializeField] private AssistStatsSO[] _assists = default;
	[SerializeField] private BaseMenu _matchOverMenu = default;
	[SerializeField] private BaseMenu _matchOverReplayMenu = default;
	[SerializeField] private Animator _readyAnimator = default;
	[SerializeField] private CinemachineTargetGroup _cinemachineTargetGroup = default;
	[SerializeField] private Audio _musicAudio = default;
	[SerializeField] private Audio _uiAudio = default;
	protected BrainController _playerOneController;
	protected BrainController _playerTwoController;
	private PlayerAnimator _playerOneAnimator;
	private PlayerAnimator _playerTwoAnimator;
	private PlayerInput _playerOneInput;
	private PlayerInput _playerTwoInput;
	private Coroutine _roundOverTrainingCoroutine;
	private Coroutine _hitStopCoroutine;
	private Sound _currentMusic;
	private GameObject _currentStage;
	private List<IHitstop> _hitstopList = new();
	private Vector2 _cachedOneResetPosition;
	private Vector2 _cachedTwoResetPosition;
	private float _countdown;
	private float _startSkipTime;
	private int _currentRound = 1;
	private bool _reverseReset;
	private bool _hasSwitchedCharacters;
	private bool _canCallSwitchCharacter = true;
	private bool _finalRound;
	private int _playerOneWins;
	private int _playerTwoWins;

	public bool IsDialogueRunning { get; set; }
	public bool HasGameStarted { get; set; }
	public bool IsTrainingMode { get { return _isTrainingMode; } set { } }
	public bool InfiniteHealth { get; set; }
	public bool InfiniteArcana { get; set; }
	public bool InfiniteAssist { get; set; }
	public Player PlayerOne { get; private set; }
	public Player PlayerTwo { get; private set; }
	public PauseMenu PauseMenu { get; set; }
	public static GameManager Instance { get; private set; }
	public BaseController PausedController { get; set; }
	public float GameSpeed { get; set; }


	void Awake()
	{
		_startSkipTime = Time.time;
		HasGameStarted = false;
		GameSpeed = _gameSpeed;
		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 1;
		CheckInstance();
		if (!SceneSettings.SceneSettingsDecide)
		{
			if (_controllerOne >= 0)
			{
				SceneSettings.ControllerOne = InputSystem.devices[_controllerOne];
			}
			if (_controllerTwo >= 0)
			{
				SceneSettings.ControllerTwo = InputSystem.devices[_controllerTwo];
			}
			SceneSettings.ControllerOneScheme = _controllerOneType.ToString();
			SceneSettings.ControllerTwoScheme = _controllerTwoType.ToString();
			SceneSettings.PlayerOne = (int)_characterOne;
			SceneSettings.PlayerTwo = (int)_characterTwo;
			SceneSettings.AssistOne = (int)_assistOne;
			SceneSettings.AssistTwo = (int)_assistTwo;
			SceneSettings.StageIndex = (int)_stage;
			SceneSettings.ColorOne = _playerOneSkin;
			SceneSettings.ColorTwo = _playerTwoSkin;
			SceneSettings.IsTrainingMode = _isTrainingMode;
			SceneSettings.Bit1 = _1BitOn;
			SceneSettings.MusicName = _music.ToString();
		}
		else
		{
			_isTrainingMode = SceneSettings.IsTrainingMode;
		}
		CheckSceneSettings();

		GameObject playerOneObject = Instantiate(_playerLocal);
		playerOneObject.GetComponent<PlayerStats>().PlayerStatsSO = _playerStats[SceneSettings.PlayerOne];
		GameObject playerTwoObject = Instantiate(_playerLocal);
		playerTwoObject.GetComponent<PlayerStats>().PlayerStatsSO = _playerStats[SceneSettings.PlayerTwo];
		InitializePlayers(playerOneObject, playerTwoObject);
	}


	private void InitializePlayers(GameObject playerOneObject, GameObject playerTwoObject)
	{
		_playerOneController = playerOneObject.GetComponent<BrainController>();
		_playerTwoController = playerTwoObject.GetComponent<BrainController>();
		PlayerOne = playerOneObject.GetComponent<Player>();
		PlayerTwo = playerTwoObject.GetComponent<Player>();
		playerOneObject.GetComponent<CpuController>().SetOtherPlayer(playerTwoObject.transform);
		playerTwoObject.GetComponent<CpuController>().SetOtherPlayer(playerOneObject.transform);
		playerOneObject.SetActive(true);
		playerTwoObject.SetActive(true);
		PlayerOne.transform.position = _spawnPositions[0].position;
		PlayerTwo.transform.position = _spawnPositions[1].position;
		if (SceneSettings.ControllerOne != null && _controllerOneType != ControllerTypeEnum.Cpu)
		{
			_playerOneController.SetControllerToPlayer();
		}
		else
		{
			_playerOneController.SetControllerToCpu();
		}
		if (SceneSettings.ControllerTwo != null && _controllerTwoType != ControllerTypeEnum.Cpu)
		{
			_playerTwoController.SetControllerToPlayer();
		}
		else
		{
			_playerTwoController.SetControllerToCpu();
		}
		PlayerOne.SetController();
		PlayerTwo.SetController();
		_playerOneAnimator = PlayerOne.transform.GetChild(1).GetComponent<PlayerAnimator>();
		_playerTwoAnimator = PlayerTwo.transform.GetChild(1).GetComponent<PlayerAnimator>();
		PlayerOne.transform.GetChild(4).GetComponent<PlayerStateManager>().Initialize(_playerOneUI, _trainingMenu);
		PlayerTwo.transform.GetChild(4).GetComponent<PlayerStateManager>().Initialize(_playerTwoUI, _trainingMenu);
		PlayerOne.transform.GetChild(1).GetComponent<PlayerAnimationEvents>().SetTrainingMenu(_trainingMenu);
		PlayerTwo.transform.GetChild(1).GetComponent<PlayerAnimationEvents>().SetTrainingMenu(_trainingMenu);
		_playerOneAnimator.SetSpriteLibraryAsset(SceneSettings.ColorOne);
		if (SceneSettings.ColorTwo == SceneSettings.ColorOne && PlayerOne.PlayerStats.characterName == PlayerTwo.PlayerStats.characterName)
		{
			if (SceneSettings.ColorOne == 3)
			{
				SceneSettings.ColorTwo = 0;
			}
			else
			{
				SceneSettings.ColorTwo++;
			}
		}
		_playerTwoAnimator.SetSpriteLibraryAsset(SceneSettings.ColorTwo);
		_playerOneController.IsPlayerOne = true;
		PlayerOne.SetPlayerUI(_playerOneUI);
		PlayerTwo.SetPlayerUI(_playerTwoUI);
		PlayerOne.SetAssist(_assists[SceneSettings.AssistOne]);
		PlayerTwo.SetAssist(_assists[SceneSettings.AssistTwo]);
		PlayerOne.SetOtherPlayer(PlayerTwo);
		PlayerOne.IsPlayerOne = true;
		if (SceneSettings.ControllerOne != null && _controllerOneType != ControllerTypeEnum.Cpu)
		{
			_playerOneController.ControllerInputName = SceneSettings.ControllerOne.displayName;
			_playerOneController.InputDevice = SceneSettings.ControllerOne;
		}
		else
		{
			_playerOneController.ControllerInputName = "Cpu";
		}
		PlayerTwo.SetOtherPlayer(PlayerOne);
		PlayerTwo.IsPlayerOne = false;
		if (SceneSettings.ControllerTwo != null && _controllerTwoType != ControllerTypeEnum.Cpu)
		{
			_playerTwoController.ControllerInputName = SceneSettings.ControllerTwo.displayName;
			_playerTwoController.InputDevice = SceneSettings.ControllerTwo;
		}
		else
		{
			_playerTwoController.ControllerInputName = "Cpu";
		}
		PlayerOne.name = $"{_playerStats[SceneSettings.PlayerOne].name}({SceneSettings.ControllerOne})_player";
		PlayerTwo.name = $"{_playerStats[SceneSettings.PlayerTwo].name}({SceneSettings.ControllerTwo})_player";
		PlayerOne.GetComponent<InputBuffer>().Initialize(_inputHistories[0]);
		PlayerTwo.GetComponent<InputBuffer>().Initialize(_inputHistories[1]);
		string inputSchemeOne = "";
		string inputSchemeTwo = "";
		if (SceneSettings.ControllerOne != null && SceneSettings.ControllerOneScheme != ControllerTypeEnum.Cpu.ToString())
		{
			inputSchemeOne = _playerOneController.InputDevice.displayName;
		}
		if (SceneSettings.ControllerTwo != null && SceneSettings.ControllerTwoScheme != ControllerTypeEnum.Cpu.ToString())
		{
			inputSchemeTwo = _playerTwoController.InputDevice.displayName;
		}
		if (inputSchemeOne.Contains("Keyboard"))
		{
			_keyboardPrompts.SetActive(true);
			_xboxPrompts.SetActive(false);
			_controllerPrompts.SetActive(false);
		}
		else if (inputSchemeOne.Contains("Xbox"))
		{
			_keyboardPrompts.SetActive(false);
			_xboxPrompts.SetActive(true);
			_controllerPrompts.SetActive(false);
		}
		else
		{
			_keyboardPrompts.SetActive(false);
			_xboxPrompts.SetActive(false);
			_controllerPrompts.SetActive(true);
		}
		_playerOneInput = PlayerOne.GetComponent<PlayerInput>();
		_playerTwoInput = PlayerTwo.GetComponent<PlayerInput>();
		if (inputSchemeOne.Contains("Keyboard") && inputSchemeTwo.Contains("Keyboard"))
		{
			SceneSettings.ControllerOneScheme = "Keyboard";
			SceneSettings.ControllerTwoScheme = "KeyboardTwo";
			_playerOneInput.SwitchCurrentControlScheme(SceneSettings.ControllerOneScheme, _playerOneController.InputDevice);
			_playerTwoInput.SwitchCurrentControlScheme(SceneSettings.ControllerTwoScheme, _playerTwoController.InputDevice);
		}
		else
		{
			if (_playerOneController.InputDevice != null && _controllerOneType != ControllerTypeEnum.Cpu)
			{
				_playerOneInput.SwitchCurrentControlScheme(SceneSettings.ControllerOneScheme, _playerOneController.InputDevice);
			}
			if (_playerTwoController.InputDevice != null && _controllerTwoType != ControllerTypeEnum.Cpu)
			{
				_playerTwoInput.SwitchCurrentControlScheme(SceneSettings.ControllerTwoScheme, _playerTwoController.InputDevice);
			}
		}
		_inputHistories[0].PlayerController = PlayerOne.GetComponent<PlayerController>();
		_inputHistories[1].PlayerController = PlayerTwo.GetComponent<PlayerController>();
		_cinemachineTargetGroup.AddMember(PlayerOne.CameraPoint, 0.5f, 0.5f);
		_cinemachineTargetGroup.AddMember(PlayerTwo.CameraPoint, 0.5f, 0.5f);
	}

	private void CheckInstance()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	public void CheckSceneSettings()
	{
		for (int i = 0; i < _stages.Length; i++)
		{
			_stages[i].SetActive(false);
		}
		_currentStage = _stages[SceneSettings.StageIndex];
		foreach (Transform stageColor in _currentStage.transform)
		{
			stageColor.gameObject.SetActive(false);
		}
		_currentStage.SetActive(true);
		int stageColorIndex = SceneSettings.Bit1 ? 1 : 0;
		if (SceneSettings.Bit1)
		{
			_playerOneUI.Turn1BitVisuals();
			_playerTwoUI.Turn1BitVisuals();
		}
		_currentStage.transform.GetChild(stageColorIndex).gameObject.SetActive(true);
	}

	public void ActivateCpus()
	{
		_playerOneController.ActivateCpu();
		_playerTwoController.ActivateCpu();
	}

	public void DeactivateCpus()
	{
		if (IsTrainingMode)
		{
			_playerOneController.DeactivateCpu();
			_playerTwoController.DeactivateCpu();
		}
	}

	public void MaxHealths()
	{
		if (IsTrainingMode)
		{
			PlayerOne.MaxHealthStats();
			PlayerTwo.MaxHealthStats();
		}
	}

	void Start()
	{
		if (SceneSettings.MusicName == "Random")
		{
			_currentMusic = _musicAudio.SoundGroup("Music").PlayInRandom();
		}
		else
		{
			_currentMusic = _musicAudio.SoundGroup("Music").Sound(SceneSettings.MusicName);
			_currentMusic.Play();
		}
		if (_isTrainingMode)
		{
			_cachedOneResetPosition = PlayerOne.transform.position;
			_cachedTwoResetPosition = PlayerTwo.transform.position;
			_countdownText.gameObject.SetActive(false);
			_hearts[0].gameObject.SetActive(false);
			_hearts[1].gameObject.SetActive(false);
			_winsImage.SetActive(false);
			_trainingPrompts.gameObject.SetActive(true);
			HasGameStarted = true;
			if (!_isOnlineMode)
			{
				StartTrainingRound();
			}
		}
		else
		{
			_musicMenu.ShowMusicMenu(_currentMusic.name);
			_inputHistories[0].transform.GetChild(0).gameObject.SetActive(false);
			_inputHistories[1].transform.GetChild(0).gameObject.SetActive(false);
			_trainingPrompts.gameObject.SetActive(false);
			StartIntro();
		}
	}

	void Update()
	{
		if (HasGameStarted && !_isTrainingMode)
		{
			_countdown -= Time.deltaTime;
			_countdownText.text = Mathf.Round(_countdown).ToString();

			if (_countdown <= 0.0f)
			{
				_timerMainAnimator.Rebind();
				RoundOver(true);
			}
			else if (_countdown <= 10.5f)
			{
				_timerMainAnimator.SetTrigger("TimerLow");
			}

		}
		if (IsDialogueRunning && !SceneSettings.ReplayMode)
		{
			if (Input.anyKeyDown)
			{
				ReplayManager.Instance.Skip = Time.time - _startSkipTime;
				SkipIntro();
			}
		}
	}

	public void SkipIntro()
	{
		_playerOneDialogue.StopDialogue();
		_playerTwoDialogue.StopDialogue();
		StartRound();
		_introAnimator.SetBool("IsIntroRunning", false);
		IsDialogueRunning = false;
	}

	public void StartIntro()
	{
		for (int i = 0; i < _arcanaObjects.Length; i++)
		{
			_arcanaObjects[i].SetActive(false);
		}
		_introUI.SetPlayerNames(_playerStats[SceneSettings.PlayerOne].characterName.ToString(), _playerStats[SceneSettings.PlayerTwo].characterName.ToString());
			_playerOneDialogue.Initialize(true, _playerStats[SceneSettings.PlayerOne]._dialogue, _playerStats[SceneSettings.PlayerTwo].characterName);
		_playerTwoDialogue.Initialize(false, _playerStats[SceneSettings.PlayerTwo]._dialogue, _playerStats[SceneSettings.PlayerOne].characterName);
		_introAnimator.SetBool("IsIntroRunning", true);
	}

	IEnumerator RoundOverCoroutine(bool timeout)
	{
		HasGameStarted = false;
		_uiAudio.Sound("Round").Play();
		yield return new WaitForSecondsRealtime(0.5f);
		string roundOverCause;
		bool playerOneWon = false;
		bool playerTwoWon = false;
		if (PlayerOne.Health == PlayerTwo.Health)
		{
			roundOverCause = "DOUBLE KO";
			if (PlayerOne.Lives > 1 && PlayerTwo.Lives > 1)
			{
				PlayerOne.LoseLife();
				PlayerTwo.LoseLife();
			}
			else if (PlayerOne.Lives == 1 && PlayerTwo.Lives > 1)
			{
				playerTwoWon = true;
				PlayerOne.LoseLife();
			}
			else if (PlayerTwo.Lives == 1 && PlayerOne.Lives > 1)
			{
				playerOneWon = true;
				PlayerTwo.LoseLife();
			}
			else if (PlayerOne.Lives == 1 && PlayerTwo.Lives == 1 && _finalRound)
			{
				PlayerOne.LoseLife();
				PlayerTwo.LoseLife();
			}
			else
			{
				_finalRound = true;
			}
		}
		else
		{
			if (PlayerOne.PlayerStats.maxHealth == PlayerOne.Health
				|| PlayerTwo.PlayerStats.maxHealth == PlayerTwo.Health)
			{
				roundOverCause = "PERFECT KO";
			}
			else
			{
				roundOverCause = "KO";
			}
			if (PlayerOne.Health > PlayerTwo.Health)
			{
				playerOneWon = true;
				PlayerTwo.LoseLife();

			}
			else if (PlayerTwo.Health > PlayerOne.Health)
			{
				playerTwoWon = true;
				PlayerOne.LoseLife();
			}
		}
		if (timeout)
		{
			_readyText.text = "TIME UP";
		}
		else
		{
			_readyText.text = roundOverCause;
		}
		for (int i = 0; i < _readyObjects.Length; i++)
		{
			_readyObjects[i].SetActive(true);
		}
		_uiAudio.Sound("TextSound").Play();
		_readyAnimator.SetTrigger("Show");
		yield return new WaitForSecondsRealtime(1.0f);

		_uiAudio.Sound("TextSound").Play();
		_readyAnimator.SetTrigger("Show");
		_currentRound++;
		if (playerOneWon)
		{
			_readyText.text = "WINNER";
			_winnerNameText.text = $"{_playerOneUI.PlayerName}";
		}
		else if (playerTwoWon)
		{
			_readyText.text = "WINNER";
			_winnerNameText.text = $"{_playerTwoUI.PlayerName}";
		}
		else
		{
			_readyText.text = "DRAW";
		}
		yield return new WaitForSecondsRealtime(2.0f);

		if (playerOneWon)
		{
			PlayerOne.PlayerStateManager.TryToTauntState();
			PlayerTwo.PlayerStateManager.TryToGiveUpState();
		}
		else if (playerTwoWon)
		{
			PlayerOne.PlayerStateManager.TryToGiveUpState();
			PlayerTwo.PlayerStateManager.TryToTauntState();
		}
		else
		{
			PlayerOne.PlayerStateManager.TryToGiveUpState();
			PlayerTwo.PlayerStateManager.TryToGiveUpState();
		}
		yield return new WaitForSecondsRealtime(2.0f);

		for (int i = 0; i < _readyObjects.Length; i++)
		{
			_readyObjects[i].SetActive(false);
		}
		_winnerNameText.text = "";
		_readyText.text = "";
		if (playerOneWon)
		{
			if (PlayerTwo.Lives == 0)
			{
				_playerOneWins++;
				_winsText.text = $"{_playerOneWins}-{_playerTwoWins}";
				MatchOver();
			}
			else
			{
				_fadeHandler.StartFadeTransition(true);
				_fadeHandler.onFadeEnd.AddListener(() => StartRound());
			}
		}
		else if (playerTwoWon)
		{
			if (PlayerOne.Lives == 0)
			{
				_playerTwoWins++;
				_winsText.text = $"{_playerOneWins}-{_playerTwoWins}";
				MatchOver();
			}
			else
			{
				_fadeHandler.StartFadeTransition(true);
				_fadeHandler.onFadeEnd.AddListener(() => StartRound());
			}
		}
		else
		{
			if (PlayerTwo.Lives == 0 || PlayerOne.Lives == 0)
			{
				MatchOver();
			}
			else
			{
				_fadeHandler.StartFadeTransition(true);
				_fadeHandler.onFadeEnd.AddListener(() => StartRound());
			}
		}
	}

	private void MatchOver()
	{
		_finalRound = false;
		_winnerNameText.text = "";
		_readyText.text = "";
		_currentRound = 1;
		if (SceneSettings.ReplayMode)
		{
			_matchOverReplayMenu.Show();
		}
		else
		{
			_matchOverMenu.Show();
		}
		ReplayManager.Instance.SaveReplay();
		Time.timeScale = 0.0f;
	}

	public virtual void StartRound()
	{
		_fadeHandler.StartFadeTransition(false);
		if (SceneSettings.ReplayMode)
		{
			ReplayManager.Instance.ShowReplayPrompts();
		}
		_timerMainAnimator.Rebind();
		IsDialogueRunning = false;
		for (int i = 0; i < _arcanaObjects.Length; i++)
		{
			_arcanaObjects[i].SetActive(true);
		}
		_countdown = _countdownTime;
		_countdownText.text = Mathf.Round(_countdown).ToString();
		PlayerOne.ResetPlayer();
		PlayerTwo.ResetPlayer();
		PlayerOne.transform.position = _spawnPositions[0].position;
		PlayerTwo.transform.position = _spawnPositions[1].position;
		PlayerOne.StopComboTimer();
		PlayerTwo.StopComboTimer();
		PlayerOne.PlayerStateManager.TryToTauntState();
		PlayerTwo.PlayerStateManager.TryToTauntState();
		StartCoroutine(ReadyCoroutine());
	}

	private void StartTrainingRound()
	{
		PlayerOne.ResetPlayer();
		PlayerTwo.ResetPlayer();
		PlayerOne.ResetLives();
		PlayerTwo.ResetLives();
		_playerOneUI.FadeIn();
		_playerTwoUI.FadeIn();
		_timerAnimator.SetTrigger("FadeIn");
		_infiniteTime.SetActive(true);
		PlayerOne.transform.position = _spawnPositions[0].position;
		PlayerTwo.transform.position = _spawnPositions[1].position;
		HasGameStarted = true;
	}

	IEnumerator ReadyCoroutine()
	{
		Time.timeScale = GameSpeed;
		yield return new WaitForSeconds(0.5f);
		_uiAudio.Sound("TextSound").Play();
		_readyAnimator.SetTrigger("Show");
		if (_currentRound == 4)
		{
			_finalRound = true;
		}
		for (int i = 0; i < _readyObjects.Length; i++)
		{
			_readyObjects[i].SetActive(true);
		}
		if (_finalRound)
		{
			_readyText.text = $"Final Round";
		}
		else
		{
			_readyText.text = $"Round {_currentRound}";
		}
		yield return new WaitForSeconds(1.0f);
		_readyAnimator.SetTrigger("Show");
		_uiAudio.Sound("TextSound").Play();
		_readyText.text = "Fight!";
		_countdownText.gameObject.SetActive(true);
		_playerOneUI.FadeIn();
		_playerTwoUI.FadeIn();
		_timerAnimator.SetTrigger("FadeIn");
		yield return new WaitForSeconds(1.0f);
		for (int i = 0; i < _readyObjects.Length; i++)
		{
			_readyObjects[i].SetActive(false);
		}
		_readyText.text = "";
		_playerOneController.ActivateInput();
		_playerTwoController.ActivateInput();
		HasGameStarted = true;
		if (_currentRound == 1)
		{
			_inputHistories[0].StartInputTime = Time.time;
			_inputHistories[1].StartInputTime = Time.time;
			if (SceneSettings.ReplayMode)
			{
				ReplayManager.Instance.StartLoadReplay();
			}
		}
	}

	public virtual void RoundOver(bool timeout)
	{
		if (HasGameStarted)
		{
			if (_isTrainingMode)
			{
				_roundOverTrainingCoroutine = StartCoroutine(RoundOverTrainingCoroutine());
			}
			else
			{
				StartCoroutine(RoundOverCoroutine(timeout));
			}
		}
	}

	IEnumerator RoundOverTrainingCoroutine()
	{
		HasGameStarted = false;
		Time.timeScale = 0.25f;
		yield return new WaitForSeconds(1.5f);
		StartTrainingRound();
	}

	public void SwitchCharacters()
	{
		if (IsTrainingMode && _canCallSwitchCharacter)
		{
			StartCoroutine(SwitchCharactersCoroutine());
		}
	}

	IEnumerator SwitchCharactersCoroutine()
	{
		PlayerStatsSO playerStatsOneTemp = PlayerOne.PlayerStats;
		PlayerOne.PlayerStats = PlayerTwo.PlayerStats;
		PlayerTwo.PlayerStats = playerStatsOneTemp;
		_playerTwoController.IsPlayerOne = !_playerTwoController.IsPlayerOne;
		_playerOneController.IsPlayerOne = !_playerOneController.IsPlayerOne;
		_playerOneUI.ShowPlayerIcon();
		_playerTwoUI.ShowPlayerIcon();
		_canCallSwitchCharacter = false;
		if (_hasSwitchedCharacters)
		{
			if (_playerOneController.ControllerInputName != ControllerTypeEnum.Cpu.ToString() && _playerTwoController.ControllerInputName == ControllerTypeEnum.Cpu.ToString())
			{
				_playerOneController.SetControllerToCpu();
				_playerTwoController.SetControllerToPlayer();
				_playerOneInput.enabled = false;
				_playerTwoInput.enabled = true;
			}
			else if (_playerTwoController.ControllerInputName != ControllerTypeEnum.Cpu.ToString() && _playerOneController.ControllerInputName == ControllerTypeEnum.Cpu.ToString())
			{
				_playerOneController.SetControllerToPlayer();
				_playerTwoController.SetControllerToCpu();
				_playerOneInput.enabled = true;
				_playerTwoInput.enabled = false;
			}
			string a = _playerOneController.ControllerInputName;
			string b = _playerTwoController.ControllerInputName;
			_playerOneController.ControllerInputName = b;
			_playerTwoController.ControllerInputName = a;
			if (SceneSettings.ControllerOne != null && _playerOneInput.enabled)
			{
				_playerOneInput.SwitchCurrentControlScheme(SceneSettings.ControllerOneScheme, _playerOneController.InputDevice);
			}
			if (SceneSettings.ControllerTwo != null && _playerTwoInput.enabled)
			{
				_playerTwoInput.SwitchCurrentControlScheme(SceneSettings.ControllerTwoScheme, _playerTwoController.InputDevice);
			}
		}
		else
		{
			if (_playerOneController.ControllerInputName != ControllerTypeEnum.Cpu.ToString() && _playerTwoController.ControllerInputName == ControllerTypeEnum.Cpu.ToString())
			{
				_playerOneController.SetControllerToCpu();
				_playerTwoController.SetControllerToPlayer();
				_playerOneInput.enabled = false;
				_playerTwoInput.enabled = true;
			}
			else if (_playerTwoController.ControllerInputName != ControllerTypeEnum.Cpu.ToString() && _playerOneController.ControllerInputName == ControllerTypeEnum.Cpu.ToString())
			{
				_playerOneController.SetControllerToPlayer();
				_playerTwoController.SetControllerToCpu();
				_playerOneInput.enabled = true;
				_playerTwoInput.enabled = false;
			}
			string a = _playerOneController.ControllerInputName;
			string b = _playerTwoController.ControllerInputName;
			_playerOneController.ControllerInputName = b;
			_playerTwoController.ControllerInputName = a;
			if (SceneSettings.ControllerTwo != null && _playerOneInput.enabled)
			{
				_playerOneInput.SwitchCurrentControlScheme(SceneSettings.ControllerTwoScheme, _playerTwoController.InputDevice);
			}
			if (SceneSettings.ControllerOne != null && _playerTwoInput.enabled)
			{
				_playerTwoInput.SwitchCurrentControlScheme(SceneSettings.ControllerOneScheme, _playerOneController.InputDevice);
			}
		}

		_hasSwitchedCharacters = !_hasSwitchedCharacters;
		yield return new WaitForSecondsRealtime(0.1f);
		_canCallSwitchCharacter = true;
	}

	public virtual void ResetRound(Vector2 movementInput)
	{
		if (_isTrainingMode)
		{
			_fadeHandler.StartFadeTransition(true);
			_fadeHandler.onFadeEnd.AddListener(() =>
			{
				HasGameStarted = true;
				Time.timeScale = GameSpeed;
				if (_roundOverTrainingCoroutine != null)
				{
					StopCoroutine(_roundOverTrainingCoroutine);
				}
				PlayerOne.ResetPlayer();
				PlayerTwo.ResetPlayer();
				PlayerOne.ResetLives();
				PlayerTwo.ResetLives();
				ObjectPoolingManager.Instance.DisableAllObjects();
				if (movementInput.y == 1.0f)
				{
					_reverseReset = !_reverseReset;
					if (!_reverseReset)
					{
						PlayerOne.transform.position = _cachedOneResetPosition;
						PlayerTwo.transform.position = _cachedTwoResetPosition;
					}
					else
					{
						PlayerTwo.transform.position = _cachedOneResetPosition;
						PlayerOne.transform.position = _cachedTwoResetPosition;
					}
				}

				if (movementInput == Vector2.zero)
				{
					if (!_reverseReset)
					{
						PlayerOne.transform.position = _cachedOneResetPosition;
						PlayerTwo.transform.position = _cachedTwoResetPosition;
					}
					else
					{
						PlayerOne.transform.position = _cachedTwoResetPosition;
						PlayerTwo.transform.position = _cachedOneResetPosition;
					}
				}
				else if (movementInput.y == -1.0f)
				{
					if (!_reverseReset)
					{
						PlayerOne.transform.position = _spawnPositions[0].position;
						PlayerTwo.transform.position = _spawnPositions[1].position;
					}
					else
					{
						PlayerTwo.transform.position = _spawnPositions[0].position;
						PlayerOne.transform.position = _spawnPositions[1].position;
					}
					_cachedOneResetPosition = PlayerOne.transform.position;
					_cachedTwoResetPosition = PlayerTwo.transform.position;
				}
				else if (movementInput.x == 1.0f)
				{
					if (!_reverseReset)
					{
						PlayerOne.transform.position = new Vector2(_spawnPositions[0].position.x + 9, _spawnPositions[0].position.y);
						PlayerTwo.transform.position = new Vector2(_spawnPositions[1].position.x + 6, _spawnPositions[1].position.y);
					}
					else
					{
						PlayerTwo.transform.position = new Vector2(_spawnPositions[0].position.x + 9, _spawnPositions[0].position.y);
						PlayerOne.transform.position = new Vector2(_spawnPositions[1].position.x + 6, _spawnPositions[1].position.y);
					}
					_cachedOneResetPosition = PlayerOne.transform.position;
					_cachedTwoResetPosition = PlayerTwo.transform.position;
				}
				else if (movementInput.x == -1.0f)
				{
					if (!_reverseReset)
					{
						PlayerOne.transform.position = new Vector2(_spawnPositions[0].position.x - 6, _spawnPositions[0].position.y);
						PlayerTwo.transform.position = new Vector2(_spawnPositions[1].position.x - 9, _spawnPositions[1].position.y);
					}
					else
					{
						PlayerTwo.transform.position = new Vector2(_spawnPositions[0].position.x - 6, _spawnPositions[0].position.y);
						PlayerOne.transform.position = new Vector2(_spawnPositions[1].position.x - 9, _spawnPositions[1].position.y);
					}
					_cachedOneResetPosition = PlayerOne.transform.position;
					_cachedTwoResetPosition = PlayerTwo.transform.position;
				}
				_fadeHandler.StartFadeTransition(false);
			});
		}
	}

	public void StartMatch()
	{
		if (SceneSettings.RandomStage)
		{
			_currentStage.SetActive(false);
			int randomStageIndex = Random.Range(0, _stages.Length);
			_currentStage = _stages[randomStageIndex];
			_currentStage.SetActive(true);
		}
		_matchOverMenu.Hide();
		PlayerOne.ResetLives();
		PlayerTwo.ResetLives();
		_currentMusic.Stop();
		if (SceneSettings.MusicName == "Random")
		{
			_currentMusic = _musicAudio.SoundGroup("Music").PlayInRandom();
		}
		else
		{
			_currentMusic = _musicAudio.SoundGroup("Music").Sound(SceneSettings.MusicName);
			_currentMusic.Play();
		}
		StartRound();
	}

	public void PauseMusic()
	{
		_currentMusic.Pause();
	}

	public void PlayMusic()
	{
		_currentMusic.Play();
	}

	public void LoadScene(int index)
	{
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(index);
	}

	public void DisableAllInput()
	{
		if (_playerOneInput.enabled)
		{
			if (_playerOneController.ControllerInputName != ControllerTypeEnum.Cpu.ToString())
			{
				_playerOneInput.SwitchCurrentActionMap("UI");
			}
		}

		if (_playerTwoInput.enabled)
		{
			if (_playerTwoController.ControllerInputName != ControllerTypeEnum.Cpu.ToString())
			{
				_playerTwoInput.SwitchCurrentActionMap("UI");
			}
		}
	}

	public void EnableAllInput()
	{
		if (_playerOneInput.enabled)
		{
			if (_playerOneController.ControllerInputName != ControllerTypeEnum.Cpu.ToString())
			{
				_playerOneInput.SwitchCurrentActionMap("Gameplay");
			}
		}

		if (_playerTwoInput.enabled)
		{
			if (_playerTwoController.ControllerInputName != ControllerTypeEnum.Cpu.ToString())
			{
				_playerTwoInput.SwitchCurrentActionMap("Gameplay");
			}
		}
	}

	public void AddHitstop(IHitstop hitstop)
	{
		_hitstopList.Add(hitstop);
	}

	public void SuperFreeze()
	{
		StartCoroutine(SuperFreezeCoroutine());
	}
	IEnumerator SuperFreezeCoroutine()
	{
		Time.timeScale = 0.0f;
		yield return new WaitForSecondsRealtime(0.3f);
		Time.timeScale = 0.5f;
		yield return new WaitForSecondsRealtime(1.0f);
		Time.timeScale = 1f;
	}
	public void HitStop(float hitstop)
	{
		if (hitstop > 0.0f)
		{
			if (_hitStopCoroutine != null)
			{
				StopCoroutine(_hitStopCoroutine);
			}
			_hitStopCoroutine = StartCoroutine(HitStopCoroutine(hitstop));
		}
	}

	IEnumerator HitStopCoroutine(float hitstop)
	{
		for (int i = 0; i < _hitstopList.Count; i++)
		{
			_hitstopList[i].EnterHitstop();
		}
		yield return new WaitForSecondsRealtime(hitstop);
		for (int i = 0; i < _hitstopList.Count; i++)
		{
			_hitstopList[i].ExitHitstop();
		}
		_hitstopList.Clear();
	}
}
