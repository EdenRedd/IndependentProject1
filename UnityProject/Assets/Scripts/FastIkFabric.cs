using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    protected float[] bonesLength;
    protected float completeLength;
    protected Transform[] bones;
    protected Vector3[] positions;
    protected Vector3[] startDirectionSucc;
    protected Quaternion[] startRotationBone;
    protected Quaternion startRotationTarget;
    protected Quaternion startRotationRoot;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        //Initial array
        bones = new Transform[_chainLength + 1];
        positions = new Vector3[_chainLength + 1];
        bonesLength = new float[_chainLength];
        startDirectionSucc = new Vector3[_chainLength + 1];
        startRotationBone = new Quaternion[_chainLength + 1];

        if(_target == null)
        {
            _target = new GameObject(gameObject.name + " Target").transform;
            _target.position = transform.position;
        }
        startRotationTarget = _target.rotation;
        completeLength = 0;

        //init data
        var current = transform;
        for(var i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;
            startRotationBone[i] = current.rotation;
            if(i == bones.Length - 1)
            {
                //leaf
                startDirectionSucc[i] = _target.position - current.position;
            }
            else
            {//mid bone
                startDirectionSucc[i] = bones[i + 1].position - current.position;
                bonesLength[i] = (bones[i + 1].position - current.position).magnitude;
                completeLength += bonesLength[i];
            }
            current = current.parent;
        }

        if (bones[0] == null)
            throw new UnityException("The chain value is longer than the ancestor chain!");
    }

    private void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if (_target == null) return;

        if (bonesLength.Length != _chainLength) Init();

        //get position
        for(int i = 0; i < bones.Length; i++)
        {
            positions[i] = bones[i].position;
        }

        var RootRot = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
        var RootRotDiff = RootRot * Quaternion.Inverse(startRotationRoot);

        //calculations: 1st is it possible to reach?
        if((_target.position - bones[0].position).sqrMagnitude >= completeLength * completeLength)
        {
            //just stretch it
            var direction = (_target.position - positions[0]).normalized;
            //set everything after root
            for(int i = 1; i < positions.Length; i++)
            {
                positions[i] = positions[i - 1] + direction * bonesLength[i - 1];
            }
        }
        else
        {
            for(int i = 0; i < positions.Length - 1; i++)
            {
                positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + RootRotDiff * startDirectionSucc[i], _snapBackStrength);
            }

            for(int iteration = 0; iteration < _iterations; iteration++)
            {
                //back
                for(int i = positions.Length - 1; i > 0; i--)
                {
                    if (i == positions.Length - 1)
                        positions[i] = _target.position; //set it to target
                    else
                        positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * bonesLength[i]; //set in line on distance
                }

                //forward
                for(int i = 1; i < positions.Length; i++)
                {
                    positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * bonesLength[i - 1];
                }

                //When close enough
                if ((positions[positions.Length - 1] - _target.position).sqrMagnitude < _delta * _delta) break;
            }
        }

        //move towards pole
        if(_pole != null)
        {
            for(int i = 1; i < positions.Length - 1; i++)
            {
                var plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(_pole.position);
                var projectedBone = plane.ClosestPointOnPlane(positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }
        //set position & rotation
        for(int i = 0; i < positions.Length; i++)
        {
            if (i == positions.Length - 1)
                bones[i].rotation = _target.rotation * Quaternion.Inverse(startRotationTarget) * startRotationBone[i];
            else
                bones[i].rotation = Quaternion.FromToRotation(startDirectionSucc[i], positions[i + 1] - positions[i]) * startRotationBone[i];
            bones[i].position = positions[i];
        }
    }


    private void OnDrawGizmos()
    {
        var _current = this.transform;
        for(int i = 0; i < _chainLength && _current != null && _current.parent != null; i++)
        {
            var scale = Vector3.Distance(_current.position, _current.parent.position) * 0.1f;
            //forms the actual matrix that connects the balls
            Handles.matrix = Matrix4x4.TRS(_current.position, Quaternion.FromToRotation(Vector3.up, _current.parent.position - _current.position), new Vector3(scale, Vector3.Distance(_current.parent.position, _current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            _current = _current.parent;
        }
    }
}
