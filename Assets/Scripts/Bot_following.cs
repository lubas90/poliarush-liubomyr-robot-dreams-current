using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionsTest : MonoBehaviour
{
    [SerializeField] private int[] _integerArray;
    [SerializeField] private List<float> _floatList;

    [SerializeField] private Transform _curent;
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;
    [SerializeField] private int _targetNumber;
    [SerializeField] private Transform[] _targets; // Array to hold current and target transforms


    [ContextMenu("Animation")]
    private void Animation()
    {
        _ = StartCoroutine(AnimationRoutine());
    }

    private IEnumerator AnimationRoutine()
    {
        //yield return null;
        _targetNumber = 1;
        float interval = _speed * Time.deltaTime;
        float distance = (_curent.position - _target.position).magnitude;
        while (_targetNumber < _targets.Length)
        {
            if (distance > interval)
            {
                _curent.position = Vector3.MoveTowards(_curent.position, _target.position, interval);
                yield return null;
                distance = (_curent.position - _target.position).magnitude;
                interval = _speed * Time.deltaTime;
            }
            else
            {
                _curent.position = _target.position;
                _targetNumber++;
                if (_targetNumber >= _targets.Length)
                {
                    _targetNumber = 0;
                    _target = _targets[_targetNumber];
                }
                else
                {
                    _target = _targets[_targetNumber];
                }

                distance = (_curent.position - _target.position).magnitude;
                interval = _speed * Time.deltaTime;
            }
        }

    }

}