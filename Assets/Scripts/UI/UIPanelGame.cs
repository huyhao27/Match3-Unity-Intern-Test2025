using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelGame : MonoBehaviour,IMenu
{

    [SerializeField] private Button btnPause;
    [SerializeField] private Text timerText;
    [SerializeField] private Image backGroundTimerText;

    private UIMainManager m_mngr;

    private void Awake()
    {
        btnPause.onClick.AddListener(OnClickPause);
    }
    
    private void OnDestroy()
    {
        if (btnPause) btnPause.onClick.RemoveAllListeners();
    }

    private void OnClickPause()
    {
        m_mngr.ShowPauseMenu();
    }

    private void Update()
    {
        if (m_mngr != null && m_mngr.GetGameManager() != null)
        {
            if (m_mngr.GetGameManager().CurrentGameMode == GameManager.eGameMode.TimeAttack)
            {
                float timeRemaining = m_mngr.GetGameManager().GetTimeRemaining();
                if (timerText != null)
                {
                    backGroundTimerText.gameObject.SetActive(true);
                    timerText.gameObject.SetActive(true);
                    int minutes = Mathf.FloorToInt(timeRemaining / 60);
                    int seconds = Mathf.FloorToInt(timeRemaining % 60);
                    timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                }
            }
            else
            {
                if (timerText != null)
                {
                    timerText.gameObject.SetActive(false);
                    backGroundTimerText.gameObject.SetActive(false);
                }
            }
        }
    }

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}