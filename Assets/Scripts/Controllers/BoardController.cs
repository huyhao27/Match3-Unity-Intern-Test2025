using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnMoveEvent = delegate { };

    public bool IsBusy { get; private set; }

    private Board m_board;
    private BottomArea m_bottomArea;
    private GameManager m_gameManager;
    private Camera m_cam;
    private GameSettings m_gameSettings;
    private bool m_gameOver;
    private bool m_isAutoplayWin;
    private bool m_isAutoplayLose;

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;
        m_gameSettings = gameSettings;
        m_gameManager.StateChangedAction += OnGameStateChange;
        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);
        
        Func<bool> shouldCheckLose = () => 
            m_gameManager.CurrentGameMode != GameManager.eGameMode.TimeAttack;
        
        m_bottomArea = new BottomArea(this.transform, gameSettings.BottomCellCount, 
            OnMatchCleared, OnBottomAreaFull, shouldCheckLose);
        
        Fill();
    }

    private void Fill()
    {
        m_board.Fill();
        IsBusy = false;
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
            case GameManager.eStateGame.GAME_WIN:
                m_gameOver = true;
                IsBusy = true;
                break;
        }
    }

    public void Update()
    {
        if (m_gameOver) return;
        if (IsBusy && !m_isAutoplayWin && !m_isAutoplayLose) return;

        if (m_isAutoplayWin || m_isAutoplayLose)
        {
            return; 
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleTap();
        }
    }

    private void HandleTap()
    {
        if (IsBusy) return;

        var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            Cell cell = hit.collider.GetComponent<Cell>();
            if (cell != null)
            {
                if (cell.IsBottomCell && !cell.IsEmpty && 
                    m_gameManager.CurrentGameMode == GameManager.eGameMode.TimeAttack)
                {
                    ReturnItemToBoard(cell);
                }
                else if (!cell.IsBottomCell && !cell.IsEmpty)
                {
                    MoveItemToBottom(cell);
                }
            }
        }
    }

    private void MoveItemToBottom(Cell boardCell)
    {
        if (IsBusy) return;
        if (boardCell.IsEmpty) return;

        if (m_bottomArea.IsFull() && m_gameManager.CurrentGameMode == GameManager.eGameMode.Normal)
        {
            OnBottomAreaFull();
            return;
        }

        IsBusy = true;
        Item item = boardCell.Item;
        item.InitialBoardPosition = new Vector2Int(boardCell.BoardX, boardCell.BoardY);
        item.IsInBottomArea = false;
        
        boardCell.Free();

        if (m_bottomArea.TryAddItem(item))
        {
            item.IsInBottomArea = true;
            OnMoveEvent();
            StartCoroutine(WaitForMoveAnimationAndCheckWin());
        }
        else
        {
            boardCell.Assign(item);
            IsBusy = false;
        }
    }

    private IEnumerator WaitForMoveAnimationAndCheckWin()
    {
        yield return new WaitForSeconds(0.3f);
        IsBusy = false;
        CheckWinCondition();
    }

    private void ReturnItemToBoard(Cell bottomCell)
    {
        if (IsBusy) return;
        if (bottomCell.IsEmpty) return;
        
        IsBusy = true;
        Item item = bottomCell.Item;
        Vector2Int originalPos = item.InitialBoardPosition;
        
        Cell targetBoardCell = m_board.GetCellAt(originalPos.x, originalPos.y);
        
        if (targetBoardCell != null && targetBoardCell.IsEmpty)
        {
            m_bottomArea.RemoveItemAtCell(bottomCell);
            item.IsInBottomArea = false;
            
            targetBoardCell.Assign(item);
            item.SetViewRoot(m_board.GetRoot());
            
            item.View.DOMove(targetBoardCell.transform.position, 0.3f)
                .SetEase(DG.Tweening.Ease.OutQuad)
                .OnComplete(() =>
                {
                    IsBusy = false;
                });
        }
        else
        {
            IsBusy = false;
        }
    }

    private void CheckWinCondition()
    {
        if (m_board.IsBoardEmpty())
        {
            StartCoroutine(WaitAndWin());
        }
    }

    private IEnumerator WaitAndWin()
    {
        yield return new WaitForSeconds(0.5f);
        m_gameManager.SetState(GameManager.eStateGame.GAME_WIN);
    }

    private void OnMatchCleared()
    {
        CheckWinCondition();
    }

    private void OnBottomAreaFull()
    {
        if (!m_board.IsBoardEmpty())
        {
            StartCoroutine(WaitAndLose());
        }
    }

    private IEnumerator WaitAndLose()
    {
        yield return new WaitForSeconds(0.5f);
        m_gameManager.SetState(GameManager.eStateGame.GAME_OVER);
    }

    public void StartAutoplayWin()
    {
        m_isAutoplayWin = true;
        m_isAutoplayLose = false;
        StartCoroutine(AutoplayWinCoroutine());
    }

    public void StartAutoplayLose()
    {
        m_isAutoplayLose = true;
        m_isAutoplayWin = false;
        StartCoroutine(AutoplayLoseCoroutine());
    }

    private IEnumerator AutoplayWinCoroutine()
    {
        while (!m_gameOver && !m_board.IsBoardEmpty())
        {
            if (m_bottomArea.IsFull())
            {
                yield break;
            }

            Cell cellToTap = m_bottomArea.FindBestCellToTapOnBoard(m_board.GetAllCellsWithItems());

            if (cellToTap == null)
            {
                List<Cell> cellsWithItems = m_board.GetAllCellsWithItems();
                if (cellsWithItems.Count > 0)
                {
                    cellToTap = cellsWithItems[UnityEngine.Random.Range(0, cellsWithItems.Count)];
                }
                else
                {
                    yield break; 
                }
            }
            
            MoveItemToBottom(cellToTap);

            yield return new WaitForSeconds(m_gameSettings.AutoplayDelay);
        }
    }

    private IEnumerator AutoplayLoseCoroutine()
    {
        while (!m_gameOver && !m_bottomArea.IsFull())
        {
            if (m_board.IsBoardEmpty())
            {
                yield break;
            }

            Cell cellToTap = m_bottomArea.FindWorstCellToTapOnBoard(m_board.GetAllCellsWithItems());

            if (cellToTap == null)
            {
                List<Cell> cellsWithItems = m_board.GetAllCellsWithItems();
                if (cellsWithItems.Count > 0)
                {
                    cellToTap = cellsWithItems[UnityEngine.Random.Range(0, cellsWithItems.Count)];
                }
                else
                {
                    yield break; 
                }
            }

            MoveItemToBottom(cellToTap);

            yield return new WaitForSeconds(m_gameSettings.AutoplayDelay);
        }
    }

    public bool IsBoardEmpty()
    {
        return m_board.IsBoardEmpty();
    }

    internal void Clear()
    {
        m_board?.Clear();
        m_bottomArea?.Clear();
    }
}