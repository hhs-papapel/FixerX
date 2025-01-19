using UnityEngine;

public class MeshSelector : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer; // SkinnedMeshRenderer를 참조
    public Mesh[] meshes; // 여러 개의 Mesh를 배열로 저장

    int index = Mathf.RoundToInt(GlobalUser.carNumber - 1);
    //int index = 3;
    void Start()
    {
        // 입력값 검증
        if (index < 0 || index >= meshes.Length)
        {
            Debug.LogError($"Invalid mesh index: {index}. It should be between 0 and {meshes.Length - 1}.");
            return;
        }

        // SkinnedMeshRenderer에 선택한 Mesh 설정
        skinnedMeshRenderer.sharedMesh = meshes[index];
        Debug.Log($"Mesh set to index {index}: {meshes[index].name}");
    }

}
