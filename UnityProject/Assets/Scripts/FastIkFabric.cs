using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastIkFabric : MonoBehaviour
{

    [SerializeField]
    private int _chainLength = 2;

    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _pole;

    [Header("Solver Parameters")]
    [SerializeField]
    private int _iterations = 10;

    [SerializeField]
    private float _delta = 0.001f;

    [SerializeField]
    [Range(0,1)]
    private float _snapBackStrength = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        var _current = this.transform;
        for(int i = 0; i < _chainLength && _current != null && _current.parent != null; i++)
        {
            _current = _current.parent;
        }
    }
}
