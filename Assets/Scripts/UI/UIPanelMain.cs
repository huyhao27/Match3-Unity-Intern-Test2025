using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelMain : MonoBehaviour, IMenu
{

    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnAutoplayWin; 
    [SerializeField] private Button btnAutoLose;
    [SerializeField] private Button btnTimeAttack;

    private UIMainManager m_mngr;

    private void Awake()
    {
        btnAutoplayWin?.onClick.AddListener(OnClickAutoplayWin);
        btnAutoLose?.onClick.AddListener(OnClickAutoLose);
        btnPlay?.onClick.AddListener(OnClickPlay);
        btnTimeAttack?.onClick.AddListener(OnClickTimeAttack);
    }

    private void OnDestroy()
    {
        if (btnAutoplayWin) btnAutoplayWin.onClick.RemoveAllListeners(); 
        if (btnAutoLose) btnAutoLose.onClick.RemoveAllListeners();
        if (btnPlay) btnPlay.onClick.RemoveAllListeners();
        if (btnTimeAttack) btnTimeAttack.onClick.RemoveAllListeners();
    }

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }

    private void OnClickAutoplayWin()
    {
        m_mngr.StartAutoplayWin(); 
    }

    private void OnClickAutoLose()
    {
        m_mngr.StartAutoplayLose(); 
    }
   
    private void OnClickPlay()  
    {
        m_mngr.StartGame();
    }

    private void OnClickTimeAttack()
    {
        m_mngr.LoadLevelTimeAttack();
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
