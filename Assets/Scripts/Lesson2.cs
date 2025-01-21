using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Lesson2 : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] private string _number1;
    // Start is called before the first frame update
    [ContextMenu("Hello World")] private void HelloWorld()
    {
        Debug.Log($"Hello World {_number1}");
    }
}
