using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizedList<T> {
    public List<T> OriginalData;
    public List<int> IndexList;
    public int Count
    {
        get
        {
            return OriginalData.Count;
        }
    }


    private bool _isShuffled = false;
    public bool IsShuffled
    {
        get
        {
            return _isShuffled;
        }
    }

    public RandomizedList(List<T> originalData, int seed)
    {
        OriginalData = originalData;
        Shuffle(seed);
    }



    /// <summary>
    /// Randomize the list. Should only be called after list is constructed fully, since
    /// adding elements will change the shuffled order.
    /// 
    /// Apparently this method is called Fisher-Yates.
    /// </summary>
    /// <param name="seed"></param>
    public void Shuffle(int seed)
    {
        if (OriginalData.Count == 0)
            return;

        IndexList = new List<int>();
        for (int i = 0; i < OriginalData.Count; i++)
            IndexList.Add(i);

        Random.InitState(seed);
        int temp = 0;
        for (int i = IndexList.Count - 1; i > 0; i--)
        {
            temp = IndexList[i];
            int rand = Random.Range(0, IndexList.Count - 1);
            IndexList[i] = IndexList[rand];
            IndexList[rand] = temp;
        }
        _isShuffled = true;
    }

    /// <summary>
    /// Get or set the value at position i after shuffling.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public T this[int i]
    {
        get
        {
            return OriginalData[IndexList[i]];
        }
        set
        {
            OriginalData[IndexList[i]] = value;
        }
    }


    public string GetIndicesAsString()
    {
        string s = "";
        if (IndexList.Count == 0)
            return s;
        for (int i = 0; i < IndexList.Count - 1; i++)
            s += IndexList[i] + ", ";
        return s + IndexList[IndexList.Count - 1]; 
    }

}
