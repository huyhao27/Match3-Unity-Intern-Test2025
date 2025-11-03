using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class BottomArea
{
    private Cell[] m_bottomCells;
    private Transform m_root;
    private int m_cellCount;
    private Action m_onMatchClear;
    private Action m_onFull;

    public BottomArea(Transform root, int cellCount, Action onMatchClear, Action onFull)
    {
        m_root = root;
        m_cellCount = cellCount;
        m_onMatchClear = onMatchClear;
        m_onFull = onFull;
        m_bottomCells = new Cell[m_cellCount];
        
        CreateBottomCells();
    }

    private void CreateBottomCells()
    {
        Vector3 origin = new Vector3(-m_cellCount * 0.5f + 0.5f, -3.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        
        for (int i = 0; i < m_cellCount; i++)
        {
            GameObject go = GameObject.Instantiate(prefabBG);
            go.transform.position = origin + new Vector3(i, 0f, 0f);
            go.transform.SetParent(m_root);
            
            Cell cell = go.GetComponent<Cell>();
            cell.Setup(i, -1);
            cell.SetIsBottomCell(true);
            
            m_bottomCells[i] = cell;
        }
    }

    public bool TryAddItem(Item item)
    {
        for (int i = 0; i < m_cellCount; i++)
        {
            if (m_bottomCells[i].IsEmpty)
            {
                m_bottomCells[i].Assign(item);
                item.SetViewRoot(m_root);
                item.View.DOMove(m_bottomCells[i].transform.position, 0.3f).OnComplete(() =>
                {
                    CheckForMatches();
                });
                return true;
            }
        }
        
        if (IsFull())
        {
            m_onFull?.Invoke();
        }
        return false;
    }

    private void CheckForMatches()
    {
        Dictionary<NormalItem.eNormalType, List<Cell>> typeGroups = new Dictionary<NormalItem.eNormalType, List<Cell>>();
        
        for (int i = 0; i < m_cellCount; i++)
        {
            if (!m_bottomCells[i].IsEmpty && m_bottomCells[i].Item is NormalItem normalItem)
            {
                if (!typeGroups.ContainsKey(normalItem.ItemType))
                {
                    typeGroups[normalItem.ItemType] = new List<Cell>();
                }
                typeGroups[normalItem.ItemType].Add(m_bottomCells[i]);
            }
        }

        bool matchFound = false;  
        foreach (var group in typeGroups)
        {
            if (group.Value.Count == 3)
            {
                matchFound = true; 
                ClearMatches(group.Value);
                m_onMatchClear?.Invoke();
                break;
            }
        }

        if (!matchFound && IsFull())
        {
            m_onFull?.Invoke(); 
        }
    }

    private void ClearMatches(List<Cell> cellsToClear)
    {
        foreach (var cell in cellsToClear)
        {
            cell.ExplodeItem();
        }
    }

    public bool IsFull()
    {
        for (int i = 0; i < m_cellCount; i++)
        {
            if (m_bottomCells[i].IsEmpty)
                return false;
        }
        return true;
    }

    public void Clear()
    {
        for (int i = 0; i < m_cellCount; i++)
        {
            if (m_bottomCells[i] != null)
            {
                m_bottomCells[i].Clear();
                GameObject.Destroy(m_bottomCells[i].gameObject);
            }
        }
    }
    
    public Cell FindBestCellToTapOnBoard(List<Cell> boardCells)
    {
        Dictionary<NormalItem.eNormalType, int> counts = new Dictionary<NormalItem.eNormalType, int>();
        foreach (Cell cell in m_bottomCells)
        {
            if (!cell.IsEmpty && cell.Item is NormalItem normalItem)
            {
                if (!counts.ContainsKey(normalItem.ItemType))
                    counts[normalItem.ItemType] = 0;
                counts[normalItem.ItemType]++;
            }
        }

        foreach (Cell boardCell in boardCells)
        {
            if (boardCell.Item is NormalItem boardItem)
            {
                if (counts.ContainsKey(boardItem.ItemType) && counts[boardItem.ItemType] == 2)
                {
                    return boardCell;
                }
            }
        }

        foreach (Cell boardCell in boardCells)
        {
            if (boardCell.Item is NormalItem boardItem)
            {
                if (counts.ContainsKey(boardItem.ItemType) && counts[boardItem.ItemType] == 1)
                {
                    return boardCell;
                }
            }
        }

        return null;
    }

    public Cell FindWorstCellToTapOnBoard(List<Cell> boardCells)
    {
        Dictionary<NormalItem.eNormalType, int> counts = new Dictionary<NormalItem.eNormalType, int>();
        foreach (Cell cell in m_bottomCells)
        {
            if (!cell.IsEmpty && cell.Item is NormalItem normalItem)
            {
                if (!counts.ContainsKey(normalItem.ItemType))
                    counts[normalItem.ItemType] = 0;
                counts[normalItem.ItemType]++;
            }
        }

        foreach (Cell boardCell in boardCells)
        {
            if (boardCell.Item is NormalItem boardItem)
            {
                if (!counts.ContainsKey(boardItem.ItemType))
                {
                    return boardCell; 
                }
            }
        }

        foreach (Cell boardCell in boardCells)
        {
            if (boardCell.Item is NormalItem boardItem)
            {
                if (counts.ContainsKey(boardItem.ItemType) && counts[boardItem.ItemType] == 1)
                {
                    return boardCell; 
                }
            }
        }
        
        return null; 
    }
}