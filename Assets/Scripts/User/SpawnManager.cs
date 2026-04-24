using System.Collections;
using UnityEngine;
using Unity.XR.CoreUtils;

public class PlayerSpawner : MonoBehaviour
{
    public XROrigin xrOrigin;
    public Transform spawnPoint;

    void Start()
    {
        StartCoroutine(SpawnPlayerRoutine());
    }

    IEnumerator SpawnPlayerRoutine()
    {
        // Tăng thời gian lên 0.5s để kính Quest kịp đồng bộ tracking khi vừa ấn Play
        yield return new WaitForSeconds(0.1f); 

        if (xrOrigin != null && spawnPoint != null)
        {
            // Gọi thẳng lệnh dịch chuyển, KHÔNG can thiệp vào CharacterController nữa
            // Việc này giúp hệ thống XRBodyTransformer không bị gián đoạn và báo lỗi
            xrOrigin.MoveCameraToWorldLocation(spawnPoint.position);
            xrOrigin.MatchOriginUpCameraForward(spawnPoint.up, spawnPoint.forward);
        }
    }
}