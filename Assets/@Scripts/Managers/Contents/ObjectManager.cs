using Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ObjectManager
{
    const float START_DEGREE = DELTA_DEGREE / 2;
    const float DELTA_DEGREE = (2 * Mathf.PI / 24);
    const int SLICE_COUNT = 24;
    const int LAYER_COUNT = 6;

    // 셀 좌표 미리 계산해둔다
    public Vector3[] Pos { get; private set; } = new Vector3[SLICE_COUNT * LAYER_COUNT];

    public MonsterController[] Monsters { get; private set; } = new MonsterController[SLICE_COUNT * LAYER_COUNT];
    public PlayerController Player { get; set; }
    public BossController Boss { get; private set; }

    List<int> FreeIndex = new List<int>();
    List<RespawnData> _respawnData;
    int[] _point;

    float interval = 0f;

    int _remainRespawnTurn = 0;

    #region Roots
    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }

    public Transform HeroRoot { get { return GetRootTransform("@Heroes"); } }
    public Transform MonsterRoot { get { return GetRootTransform("@Monsters"); } }
    public Transform ProjectileRoot { get { return GetRootTransform("@Projectiles"); } }
    public Transform EnvRoot { get { return GetRootTransform("@Envs"); } }
    public Transform EffectRoot { get { return GetRootTransform("@Effects"); } }
    public Transform NpcRoot { get { return GetRootTransform("@Npc"); } }
    public Transform ItemHolderRoot { get { return GetRootTransform("@ItemHolders"); } }
    #endregion


    public ObjectManager()
    {
    }

    public void Init()
    {

    }

    public void LoadStageData(StageData stageData)
    {

    }

    public List<MonsterController> GetMonsters()
    {
        return Monsters.Where(m => { return m != null; }).OrderBy(m => m.GetDistance(Vector2.zero)).ToList();
    }

    public List<MonsterController> GetLowestHpSelectedMonsters()
    {
        return Monsters.Where(m => { return m != null && m.Selected; }).OrderBy(x => x.Hp).ToList();       //무기에 따라 정렬 규칙이 달라질 경우 수정 필요
    }

    public void Clear()
    {

    }

    public GameObject SpawnGameObject(Vector3 position, string prefabName)
    {
        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        go.transform.position = position;

        return go;
    }

    public void ResetStageObjects()
    {
        // TODO ILHAK
    }

    public void ClearFreeIndex(int layer)
    {
        FreeIndex.Clear();

        int startIndex = layer * SLICE_COUNT;

        for (int slice = 0; slice < SLICE_COUNT; slice++)
        {
            int index = startIndex + slice;
            if (Monsters[index] != null)
                continue;

            FreeIndex.Add(index);
        }
    }

    public void DespawnGameObject<T>(T obj) where T : BaseController
    {

    }

    public void DespawnMonster(MonsterController mc)
    {

    }

    public void DropCoin(Vector2 dropPos, int amount = 1)
    {
        // TODO ILHAK Coin
    }

    public void SpawnPlayer(string key, Vector2 pos)
    {
        var go = Managers.Resource.Instantiate(key);
        var pc = go.GetOrAddComponent<PlayerController>();
        
        // TODO ILHAK
    }

    public IEnumerator SpawnMonster(int layer)
    {
        // TODO ILHAK
        yield return null;
    }
}