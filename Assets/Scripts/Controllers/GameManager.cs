using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<eStateGame> StateChangedAction = delegate { };

    // public enum eLevelMode
    // {
    //     TIMER,
    //     MOVES
    // }

    public enum eStateGame
    {
        SETUP,
        MAIN_MENU,
        GAME_STARTED,
        PAUSE,
        GAME_OVER,
        GAME_WIN,
    }

    public enum eGameMode
    {
        Normal,
        TimeAttack
    }

    private eStateGame m_state;
    public eStateGame State
    {
        get { return m_state; }
        private set
        {
            m_state = value;

            StateChangedAction(m_state);
        }
    }
    
    private eGameMode m_currentGameMode = eGameMode.Normal;
    public eGameMode CurrentGameMode => m_currentGameMode;

    public bool GameWon { get; private set; }

    private float m_timeRemaining;
    private bool m_isTimerRunning;

    public float GetTimeRemaining() => m_timeRemaining;

    public event Action<float> OnTimeUpdate;

    private GameSettings m_gameSettings;


    private BoardController m_boardController;

    private UIMainManager m_uiMenu;

    // private LevelCondition m_levelCondition;

    private void Awake()
    {
        State = eStateGame.SETUP;

        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);

        m_uiMenu = FindObjectOfType<UIMainManager>();
        m_uiMenu.Setup(this);
    }

    void Start()
    {
        State = eStateGame.MAIN_MENU;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_boardController != null) m_boardController.Update();
    }


    internal void SetState(eStateGame state)
    {
        if (state == eStateGame.GAME_WIN)
        {
            GameWon = true;
            State = eStateGame.GAME_OVER; 
        }
        else if (state == eStateGame.GAME_OVER)
        {
            GameWon = false; 
            State = eStateGame.GAME_OVER;
        }
        else
        {
            State = state;
        }

        if(State == eStateGame.PAUSE)
        {
            DOTween.PauseAll();
        }
        else
        {
            DOTween.PlayAll();
        }
    }

    public void SetGameMode(eGameMode mode)
    {
        m_currentGameMode = mode;
    }

    public void LoadLevel()
    {
        GameWon = false;
        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.StartGame(this, m_gameSettings);

        SetState(eStateGame.GAME_STARTED);

        if (m_currentGameMode == eGameMode.TimeAttack)
        {
            m_timeRemaining = m_gameSettings.TimeAttackDuration;
            m_isTimerRunning = true;
            StartCoroutine(TimeAttackTimerCoroutine());
        }
    }

    private IEnumerator TimeAttackTimerCoroutine()
    {
        while (m_isTimerRunning && m_timeRemaining > 0 && State == eStateGame.GAME_STARTED)
        {
            yield return new WaitForSeconds(1f);
            
            if (!m_isTimerRunning || State != eStateGame.GAME_STARTED)
                yield break;
            
            m_timeRemaining -= 1f;
            OnTimeUpdate?.Invoke(m_timeRemaining);
            
            if (m_timeRemaining <= 0)
            {
                if (m_boardController != null && m_boardController.IsBoardEmpty())
                {
                    SetState(eStateGame.GAME_WIN);
                }
                else
                {
                    SetState(eStateGame.GAME_OVER);
                }
                m_isTimerRunning = false;
                yield break;
            }
        }
    }

    public void StopTimer()
    {
        m_isTimerRunning = false;
    }

    // public void GameOver()
    // {
    //     StartCoroutine(WaitBoardController());
    // }

    internal void ClearLevel()
    {
        StopTimer();
        if (m_boardController)
        {
            m_boardController.Clear();
            Destroy(m_boardController.gameObject);
            m_boardController = null;
        }
    }
    
    public void StartAutoplay(bool aimToWin)
    {
        GameWon = false;
        LoadLevel(); 
        StartCoroutine(WaitForBoardControllerAndStartAutoplay(aimToWin));
    }
    
    private IEnumerator WaitForBoardControllerAndStartAutoplay(bool isWin)
    {
        yield return new WaitForEndOfFrame();
    
        if (m_boardController != null)
        {
            if (isWin)
            {
                m_boardController.StartAutoplayWin();
            }
            else
            {
                m_boardController.StartAutoplayLose();
            }
        }
    }

    // private IEnumerator WaitBoardController()
    // {
    //     while (m_boardController.IsBusy)
    //     {
    //         yield return new WaitForEndOfFrame();
    //     }
    //
    //     yield return new WaitForSeconds(1f);
    //
    //     State = eStateGame.GAME_OVER;
    //
    //     if (m_levelCondition != null)
    //     {
    //         m_levelCondition.ConditionCompleteEvent -= GameOver;
    //
    //         Destroy(m_levelCondition);
    //         m_levelCondition = null;
    //     }
    // }
}
