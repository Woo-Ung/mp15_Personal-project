using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float waitTime = 0.3f;
    [SerializeField] private float turnSpeed = 90f;
    [SerializeField] private Transform _pathHolder;

    private bool _canMove;
    private float _remainingWaitTime;
   

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _canMove = false;
    }

    public void SetCanMove(bool can)
    {
        _canMove = can;
    }


    public void Start()
    {
        Vector3[] waypoints = new Vector3[_pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = _pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(FollowPath(waypoints));
    }

    private IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while(true)
        {
            if(!_canMove)
            {
                yield return null;
                continue;
            }

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