using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;
    [SerializeField] private float turnSpeed;
    [SerializeField] private Transform _pathHolder;
    [SerializeField] private int _startNum = 0;
    [SerializeField] private Animator _animator;

    [SerializeField]private bool _canMove = true;
    private float _remainingWaitTime;
   

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _canMove = true;
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        _canMove = true;

        Vector3[] waypoints = new Vector3[_pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = _pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(FollowPath(waypoints, _startNum));
    }

    public void SetCanMove(bool can)
    {
        _canMove = can;

        if(!can)
        {
            _animator.SetBool("IsWalking",false);
        }
    }
        
    private IEnumerator FollowPath(Vector3[] waypoints, int startNum)
    {
        transform.position = waypoints[startNum];
        int targetWaypointIndex = (startNum + 1) % waypoints.Length;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);
      
        while (true)
        {
            if (!_canMove)
            {
                yield return null;
                continue;
            }

            _animator.SetBool("IsWalking", true);
            Debug.Log(
    $"{name} 현재 위치: {transform.position} / " +
    $"목표 위치: {targetWaypoint} / " +
    $"거리: {Vector3.Distance(transform.position, targetWaypoint)}");
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if(transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
               
                _remainingWaitTime = waitTime;
                while(_remainingWaitTime > 0f)
                {
                    if(_canMove)
                    {
                        _remainingWaitTime -= Time.deltaTime;
                    }
                    yield return null;
                }
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }
    }

    private IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            if (!_canMove)
            {
                yield return null;
                continue;
            }

            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = _pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in _pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);
    }

}