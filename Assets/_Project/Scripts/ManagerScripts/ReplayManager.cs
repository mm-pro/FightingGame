using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReplayManager : MonoBehaviour
{
    [SerializeField] private PauseMenu _replayPauseMenu = default;
    [SerializeField] private TextAsset _versionTextAsset = default;
    [SerializeField] private GameObject _replayPrompts = default;
    [SerializeField] private GameObject _replayInput = default;
    [SerializeField] private Animator _replayNotificationAnimator = default;
    [SerializeField] private InputHistory _playerOneInputHistory = default;
    [SerializeField] private InputHistory _playerTwoInputHistory = default;
    [Header("Debug")]
    [SerializeField] private bool _isReplayMode;
    [Range(0, 99)]
    [SerializeField] private int _replayIndex;
    private BrainController _playerOneController;
    private BrainController _playerTwoController;
    private InputBuffer _playerOneInputBuffer;
    private InputBuffer _playerTwoInputBuffer;
    private readonly int _replaysLimit = 100;
    private string[] _replayFiles;
    private readonly string _versionSplit = "Version:";
    private readonly string _patchNotesSplit = "Patch Notes:";
    private readonly string _playerOneSplit = "Player One:";
    private readonly string _playerTwoSplit = "Player Two:";
    private readonly string _stageSplit = "Stage:";
    private readonly string _skipSplit = "Skip:";
    private readonly string _playerOneInputsSplit = "Player One Inputs:";
    private readonly string _playerTwoInputsSplit = "Player Two Inputs:";

    public string VersionNumber { get; private set; }
    public int Skip { get; set; }
    public int ReplayFilesAmount { get { return _replayFiles.Length; } private set { } }
    public static ReplayManager Instance { get; private set; }



    void Awake()
    {
        if (!SceneSettings.SceneSettingsDecide)
        {
            SceneSettings.ReplayMode = _isReplayMode;
            SceneSettings.ReplayIndex = _replayIndex;
        }
        _replayFiles = Directory.GetFiles(Application.persistentDataPath, "*.txt", SearchOption.AllDirectories);
        if (SceneSettings.ReplayMode)
        {
            SetReplay();
        }
        CheckInstance();
    }

    public void SetReplay()
    {
        ReplayCardData replayCardData = GetReplayData(SceneSettings.ReplayIndex);
        SceneSettings.SceneSettingsDecide = true;
        SceneSettings.PlayerOne = replayCardData.characterOne;
        SceneSettings.ColorOne = replayCardData.colorOne;
        SceneSettings.AssistOne = replayCardData.assistOne;
        SceneSettings.PlayerTwo = replayCardData.characterTwo;
        SceneSettings.ColorTwo = replayCardData.colorTwo;
        SceneSettings.AssistTwo = replayCardData.assistTwo;
        SceneSettings.StageIndex = replayCardData.stage;
        SceneSettings.MusicName = replayCardData.musicName;
        SceneSettings.Bit1 = replayCardData.bit1;
        SceneSettings.ControllerOne = InputSystem.devices[0];
        SceneSettings.ControllerTwo = InputSystem.devices[0];
        SceneSettings.ReplayMode = true;
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

    void Start()
    {
        Setup();
        if (SceneSettings.ReplayMode)
        {
            LoadReplay();
        }
    }

    private void Setup()
    {
        string versionText = _versionTextAsset.text;
        int versionTextPosition = versionText.IndexOf(_versionSplit) + _versionSplit.Length;
        VersionNumber = versionText[versionTextPosition..versionText.LastIndexOf(_patchNotesSplit)].Trim();
        if (GameManager.Instance != null)
        {
            _playerOneInputBuffer = GameManager.Instance.PlayerOne.GetComponent<InputBuffer>();
            _playerTwoInputBuffer = GameManager.Instance.PlayerTwo.GetComponent<InputBuffer>();
        }
    }

    public void SaveReplay()
    {
        if (!SceneSettings.IsTrainingMode && !SceneSettings.ReplayMode && _replayNotificationAnimator != null)
        {
            int filesAmount = _replayFiles.Length;
            if (filesAmount == _replaysLimit)
            {
                DeleteReplay();
            }

            string fileName = Application.persistentDataPath + $@"/{filesAmount + 1}_{GameManager.Instance.PlayerOne.PlayerStats.name}_{GameManager.Instance.PlayerTwo.PlayerStats.name}.txt";
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                using FileStream fileStream = File.Create(fileName);
                byte[] version = new UTF8Encoding(true).GetBytes(
                    $"Version:\n{VersionNumber}");
                fileStream.Write(version, 0, version.Length);
                byte[] playerOne = new UTF8Encoding(true).GetBytes(
                    $"\nPlayer One:\n{SceneSettings.PlayerOne}, {SceneSettings.ColorOne}, {SceneSettings.AssistOne}");
                fileStream.Write(playerOne, 0, playerOne.Length);
                byte[] playerTwo = new UTF8Encoding(true).GetBytes(
                    $"\nPlayer Two:\n{SceneSettings.PlayerTwo}, {SceneSettings.ColorTwo}, {SceneSettings.AssistTwo}");
                fileStream.Write(playerTwo, 0, playerTwo.Length);
                byte[] stage = new UTF8Encoding(true).GetBytes(
                    $"\nStage:\n{SceneSettings.StageIndex}, {GameManager.Instance.CurrentMusic.name}, {SceneSettings.Bit1}");
                fileStream.Write(stage, 0, stage.Length);
                string playerOneInputsHistory = "";
                for (int i = 0; i < _playerOneInputHistory.Inputs.Count; i++)
                {
                    playerOneInputsHistory += $"{_playerOneInputHistory.Inputs[i]},{_playerOneInputHistory.Directions[i]},{_playerOneInputHistory.InputTimes[i]}";
                    if (i != _playerOneInputHistory.Inputs.Count - 1)
                    {
                        playerOneInputsHistory += "|";
                    }
                }
                byte[] playerOneInputs = new UTF8Encoding(true).GetBytes(
                    $"\nPlayer One Inputs:\n{playerOneInputsHistory}");
                fileStream.Write(playerOneInputs, 0, playerOneInputs.Length);
                string playerTwoInputsHistory = "";
                for (int i = 0; i < _playerTwoInputHistory.Inputs.Count; i++)
                {
                    playerTwoInputsHistory += $"{_playerTwoInputHistory.Inputs[i]},{_playerTwoInputHistory.Directions[i]},{_playerTwoInputHistory.InputTimes[i]}";
                    if (i != _playerTwoInputHistory.Inputs.Count - 1)
                    {
                        playerTwoInputsHistory += "|";
                    }
                }
                byte[] playerTwoInputs = new UTF8Encoding(true).GetBytes(
                    $"\nPlayer Two Inputs:\n{playerTwoInputsHistory}");
                fileStream.Write(playerTwoInputs, 0, playerTwoInputs.Length);
                byte[] skip = new UTF8Encoding(true).GetBytes(
                    $"\nSkip:\n{Skip}");
                fileStream.Write(skip, 0, skip.Length);
                _replayNotificationAnimator.SetTrigger("Save");
            }
            catch (Exception e)
            {
                Debug.LogError("Error saving replay: " + e);
            }
        }
    }

    public void LoadReplay()
    {
        SceneSettings.ReplayMode = true;
        _playerOneController = GameManager.Instance.PlayerOne.GetComponent<BrainController>();
        _playerTwoController = GameManager.Instance.PlayerTwo.GetComponent<BrainController>();
        GameManager.Instance.PlayerOne.GetComponent<PlayerInput>().enabled = false;
        GameManager.Instance.PlayerTwo.GetComponent<PlayerInput>().enabled = false;
        replayCardData = GetReplayData(SceneSettings.ReplayIndex);
        s = true;
    }

    private ReplayCardData replayCardData;
    bool t;
    bool s;
    private int i;

    public void StartLoadReplay()
    {
        replayCardData = GetReplayData(SceneSettings.ReplayIndex);
        t = true;
    }
    private void FixedUpdate()
    {
        if (s)
        {
            if (DemonicsPhysics.Frame == replayCardData.skip)
            {
                GameManager.Instance.SkipIntro();
                s = false;
                t = true;
            }
        }
        if (t)
        {
            NextReplayAction();
        }
    }
    private void NextReplayAction()
    {
        if (i < replayCardData.playerOneInputs.Length)
        {
            if (DemonicsPhysics.Frame >= replayCardData.playerOneInputs[i].time)
            {
                _playerOneInputBuffer.AddInputBufferItem(replayCardData.playerOneInputs[i].input, replayCardData.playerOneInputs[i].direction);
                if (replayCardData.playerOneInputs[i].input == InputEnum.Direction)
                {
                    switch (replayCardData.playerOneInputs[i].direction)
                    {
                        case InputDirectionEnum.None:
                            _playerOneController.ActiveController.InputDirection = Vector2Int.zero;
                            break;
                        case InputDirectionEnum.Up:
                            _playerOneController.ActiveController.InputDirection = new Vector2Int(_playerOneController.ActiveController.InputDirection.x, 1);
                            break;
                        case InputDirectionEnum.Down:
                            _playerOneController.ActiveController.InputDirection = new Vector2Int(_playerOneController.ActiveController.InputDirection.x, -1);
                            break;
                        case InputDirectionEnum.Left:
                            _playerOneController.ActiveController.InputDirection = new Vector2Int(-1, _playerOneController.ActiveController.InputDirection.y);
                            break;
                        case InputDirectionEnum.Right:
                            _playerOneController.ActiveController.InputDirection = new Vector2Int(1, _playerOneController.ActiveController.InputDirection.y);
                            break;
                    }
                }
                i++;
                NextReplayAction();
            }
        }
    }

    public ReplayCardData GetReplayData(int index)
    {
        string replayText = File.ReadAllText(_replayFiles[index]);

        int versionTextPosition = replayText.IndexOf(_versionSplit) + _versionSplit.Length;
        string versionNumber = replayText[versionTextPosition..replayText.LastIndexOf(_playerOneSplit)].Trim();

        int playerOneTextPosition = replayText.IndexOf(_playerOneSplit) + _playerOneSplit.Length;
        string playerOneTextWhole = replayText[playerOneTextPosition..replayText.LastIndexOf(_playerTwoSplit)].Trim();
        string[] playerOneInfo = playerOneTextWhole.Split(',');

        int playerTwoTextPosition = replayText.IndexOf(_playerTwoSplit) + _playerTwoSplit.Length;
        string playerTwoTextWhole = replayText[playerTwoTextPosition..replayText.LastIndexOf(_stageSplit)].Trim();
        string[] playerTwoInfo = playerTwoTextWhole.Split(',');

        int stageTextPosition = replayText.IndexOf(_stageSplit) + _stageSplit.Length;
        string stageTextWhole = replayText[stageTextPosition..replayText.LastIndexOf(_playerOneInputsSplit)].Trim();
        string[] stageInfo = stageTextWhole.Split(',');

        int playerOneInputTextPosition = replayText.IndexOf(_playerOneInputsSplit) + _playerOneInputsSplit.Length;
        string playerOneInputTextWhole = replayText[playerOneInputTextPosition..replayText.LastIndexOf(_playerTwoInputsSplit)].Trim();
        string[] playerOneInputInfo = playerOneInputTextWhole.Split('|');


        int playerTwoInputTextPosition = replayText.IndexOf(_playerTwoInputsSplit) + _playerTwoInputsSplit.Length;
        string playerTwoInputTextWhole = replayText[playerTwoInputTextPosition..replayText.LastIndexOf(_skipSplit)].Trim();
        string[] playerTwoInputInfo = playerTwoInputTextWhole.Split('|');

        string skipTextWhole = replayText[(replayText.IndexOf(_skipSplit) + _skipSplit.Length)..].Trim();

        List<ReplayInput> replayOneInputs = new();
        if (playerOneInputInfo[0] != "")
        {
            for (int i = 0; i < playerOneInputInfo.Length; i++)
            {
                string[] playerInput = playerOneInputInfo[i].Split(',');
                replayOneInputs.Add(new ReplayInput() { input = Enum.Parse<InputEnum>(playerInput[0]), direction = Enum.Parse<InputDirectionEnum>(playerInput[1]), time = int.Parse(playerInput[2]) });
            }
        }
        List<ReplayInput> replayTwoInputs = new();
        if (playerTwoInputInfo[0] != "")
        {
            for (int i = 0; i < playerTwoInputInfo.Length; i++)
            {
                string[] playerInput = playerTwoInputInfo[i].Split(',');
                replayTwoInputs.Add(new ReplayInput() { input = Enum.Parse<InputEnum>(playerInput[0]), direction = Enum.Parse<InputDirectionEnum>(playerInput[1]), time = int.Parse(playerInput[2]) });
            }

        }

        ReplayCardData replayData = new()
        {
            versionNumber = versionNumber,
            characterOne = int.Parse(playerOneInfo[0]),
            colorOne = int.Parse(playerOneInfo[1]),
            assistOne = int.Parse(playerOneInfo[2]),
            characterTwo = int.Parse(playerTwoInfo[0]),
            colorTwo = int.Parse(playerTwoInfo[1]),
            assistTwo = int.Parse(playerTwoInfo[2]),
            stage = int.Parse(stageInfo[0]),
            musicName = stageInfo[1].Trim(),
            bit1 = bool.Parse(stageInfo[2]),
            playerOneInputs = replayOneInputs.ToArray(),
            playerTwoInputs = replayTwoInputs.ToArray(),
            skip = float.Parse(skipTextWhole)
        };
        return replayData;
    }

    private void DeleteReplay()
    {
        File.Delete(_replayFiles[0]);
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;
        GameManager.Instance.DisableAllInput();
        GameManager.Instance.PauseMusic();
        _replayPauseMenu.Show();
    }

    public void ShowReplayPrompts()
    {
        _replayInput.SetActive(true);
        _replayPrompts.SetActive(true);
    }

    public void ToggleReplayInputHistory()
    {
        GameObject playerOneInputHistory = _playerOneInputHistory.transform.GetChild(0).gameObject;
        GameObject playerTwoInputHistory = _playerTwoInputHistory.transform.GetChild(0).gameObject;

        if (playerOneInputHistory.activeSelf)
        {
            playerOneInputHistory.SetActive(false);
            playerTwoInputHistory.SetActive(false);
        }
        else
        {
            playerOneInputHistory.SetActive(true);
            playerTwoInputHistory.SetActive(true);
        }
    }


#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock) && !SceneSettings.IsTrainingMode)
        {
            SaveReplay();
        }
    }
#endif
}

