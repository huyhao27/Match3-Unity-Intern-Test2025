using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{

    private int boardSizeX;
    private int boardSizeY;
    private Cell[,] m_cells;
    private Transform m_root;


    public Board(Transform transform, GameSettings gameSettings)
    {
        m_root = transform;
        this.boardSizeX = gameSettings.BoardSizeX;
        this.boardSizeY = gameSettings.BoardSizeY;
        m_cells = new Cell[boardSizeX, boardSizeY];
        CreateBoard();
    }

    private void CreateBoard()
    {
        Vector3 origin = new Vector3(-boardSizeX * 0.5f + 0.5f, -boardSizeY * 0.5f + 0.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                GameObject go = GameObject.Instantiate(prefabBG);
                go.transform.position = origin + new Vector3(x, y, 0f);
                go.transform.SetParent(m_root);

                Cell cell = go.GetComponent<Cell>();
                cell.Setup(x, y);
                m_cells[x, y] = cell;
            }
        }

    }

    internal void Fill()
    {
        int totalCells = boardSizeX * boardSizeY;
        int itemTypesCount = Enum.GetValues(typeof(NormalItem.eNormalType)).Length;
        
        List<NormalItem.eNormalType> itemTypesList = new List<NormalItem.eNormalType>();
        
        foreach (NormalItem.eNormalType type in Enum.GetValues(typeof(NormalItem.eNormalType)))
        {
            for (int i = 0; i < 3; i++)
            {
                itemTypesList.Add(type);
            }
        }
        
        int remainingCells = totalCells - itemTypesList.Count;
        
        int itemsPerTypeBase = (remainingCells / itemTypesCount) / 3 * 3;
        int totalBaseItems = itemsPerTypeBase * itemTypesCount;
        int finalRemaining = remainingCells - totalBaseItems;
        
        foreach (NormalItem.eNormalType type in Enum.GetValues(typeof(NormalItem.eNormalType)))
        {
            for (int i = 0; i < itemsPerTypeBase; i++)
            {
                itemTypesList.Add(type);
            }
        }
        
        int remainingGroups = finalRemaining / 3;
        for (int i = 0; i < remainingGroups; i++)
        {
            NormalItem.eNormalType type = (NormalItem.eNormalType)UnityEngine.Random.Range(0, itemTypesCount);
            for (int j = 0; j < 3; j++)
            {
                itemTypesList.Add(type);
            }
        }
        
        for (int i = 0; i < itemTypesList.Count; i++)
        {
            NormalItem.eNormalType temp = itemTypesList[i];
            int randomIndex = UnityEngine.Random.Range(i, itemTypesList.Count);
            itemTypesList[i] = itemTypesList[randomIndex];
            itemTypesList[randomIndex] = temp;
        }
        
        int index = 0;
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (index >= itemTypesList.Count) break;
                
                Cell cell = m_cells[x, y];
                NormalItem item = new NormalItem();
                item.SetType(itemTypesList[index]);
                item.SetView();
                item.SetViewRoot(m_root);
                
                cell.Assign(item);
                cell.ApplyItemPosition(false);
                index++;
            }
        }
    }

    public bool IsBoardEmpty()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (m_cells[x,y] != null && !m_cells[x, y].IsEmpty)
                    return false;
            }
        }
        return true;
    }

    public List<Cell> GetAllCellsWithItems()
    {
        List<Cell> cells = new List<Cell>();
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (m_cells[x,y] != null && !m_cells[x, y].IsEmpty)
                {
                    cells.Add(m_cells[x, y]);
                }
            }
        }
        return cells;
    }
    
    // Shuffle(), FillGapsWithNewItems(), ExplodeAllItems(), Swap(),
    // GetHorizontalMatches(), GetVerticalMatches(), ConvertNormalToBonus(),
    // GetMatchDirection(), FindFirstMatch(), CheckBonusIfCompatible(),
    // GetPotentialMatches(), GetPotentialMatch(), LookForTheSecondCellHorizontal(),
    // LookForTheSecondCellVertical(), LookForTheThirdCell(), CheckThirdCell(), ShiftDownItems()

    public Cell GetCellAt(int x, int y)
    {
        if (x < 0 || x >= boardSizeX || y < 0 || y >= boardSizeY) return null;
        return m_cells[x, y];
    }

    public Transform GetRoot()
    {
        return m_root;
    }

    public void Clear()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (m_cells[x, y] == null) continue;
                Cell cell = m_cells[x, y];
                cell.Clear();

                GameObject.Destroy(cell.gameObject);
                m_cells[x, y] = null;
            }
        }
    }
}