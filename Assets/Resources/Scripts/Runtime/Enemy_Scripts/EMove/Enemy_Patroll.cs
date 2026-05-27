using UnityEngine;
using UnityEngine.AI;

#region

#endregion


public class Enemy_Patroll : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private float _patrollRadius = 10.0f; // 순찰 반경
    #endregion

    #region 외부 호출 함수
    public Vector3 GetRandomPatrollPos()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _patrollRadius;
        randomDirection += transform.position;

        int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _patrollRadius, walkableMask))
        {
            return hit.position;
        }

        return transform.position;
    }
    #endregion
}
