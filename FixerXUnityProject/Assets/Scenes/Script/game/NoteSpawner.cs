using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefabA; // 'A' ��Ʈ ������
    public GameObject notePrefabS; // 'S' ��Ʈ ������
    public GameObject notePrefabD; // 'D' ��Ʈ ������
    public GameObject notePrefabF; // 'F' ��Ʈ ������
    public Transform leftSpawnPoint; // ���� ���� ��ġ
    public Transform rightSpawnPoint; // ������ ���� ��ġ
    public float spawnInterval = 1.5f; // ��Ʈ ���� ����
    public Transform parentObject; // 생성된 Note의 부모가 될 Transform

    private float timer = 0f;

    void Update()
    {
        if (!GameManager.ManagerIns.isUseGMFunc){
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnNote();
                timer = 0f;
            }
        }
    }

    void SpawnNote()
    {
        // �������� ��Ʈ ����
        int randomKey = Random.Range(0, 4);
        GameObject notePrefab = null;

        switch (randomKey)
        {
            case 0: notePrefab = notePrefabA; break; // 'A' ��Ʈ
            case 1: notePrefab = notePrefabS; break; // 'S' ��Ʈ
            case 2: notePrefab = notePrefabD; break; // 'D' ��Ʈ
            case 3: notePrefab = notePrefabF; break; // 'F' ��Ʈ
        }

        // �������� ���� �Ǵ� �����ʿ��� ����
        int randomSide = Random.Range(0, 2);
        Transform spawnPoint = randomSide == 0 ? leftSpawnPoint : rightSpawnPoint;

        // ��Ʈ ����, 특정 부모 객체 아래로 생성
        GameObject note = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity, parentObject);

        
    }
}
