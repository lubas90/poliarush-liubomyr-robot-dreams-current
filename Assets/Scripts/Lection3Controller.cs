using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListSample : MonoBehaviour
{
    [SerializeField] private string _new_Value;
    
    [SerializeField] private List<string> _list;

    [ContextMenu("Print the list")]
    private void Print()
    {
        string msg = "List: ";
        if (_list.Count > 0)
        {
            int i = 0;
            while (i < _list.Count - 1)
            {
                msg += $"{_list[i]}, ";
                i++;
            }
            msg += $"{_list[i]}.";  
        }
        else
        {
            msg = "Empty list";
        }
        Debug.Log(msg);
    }

    [ContextMenu("Add to list")]
    private void Addtolist()
    {
        if (_new_Value == "")
        {
            Debug.Log("No value given. Please enter a new value.");
        }
        else
        {
            _list.Add(_new_Value);
        }
    }
    [ContextMenu("Remove the last element of the list")]
    private void Removefromlist()
    {
        if (_list.Count == 0)
        {
            Debug.Log("List is empty. Cannot remove the last element.");
        }
        else
        {
            _list.Remove(_list[_list.Count - 1]);
        }
    }
    [ContextMenu("Clear entire list")]
    private void ClearList()
    {
        if (_list.Count == 0)
        {
            Debug.Log("List is already empty.");
        }
        else
        {
            _list.Clear();
        }
    }
    [ContextMenu("Sort the list ascending")]
    private void SortList()
    {
        if (_list.Count == 0)
        {
            Debug.Log("List is empty. Cannot sort the list.");
        }
        else
        {
            _list.Sort();
        }
    }
}
