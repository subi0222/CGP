using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using Unity.AI.Navigation;

[RequireComponent(typeof(NavMeshSurface))]
public class MapGenerator : MonoBehaviour
{
    [Header("Map Size")]
    public float X_Axis = 50f;
    public float Z_Axis = 50f;

    [Header("Floor")]
    public Material floorMaterial;
    public float floor_Y_Axis = -0.1f;

    [Header("Wall Material")]
    public Material wallMaterial;

    [Header("Wall Settings")]
    public float wallThickness = 0.2f;
    public float wallHeight = 3f;

    [Header("Room Prefabs")]
    public List<GameObject> roomPrefabs;
    public GameObject examRoomPrefab;

    [Header("Placement Settings")]
    public float innerMargin = 2f;
    public float outerMargin = 3f;
    public int maxTry = 200;

    [Header("Exit")]
    public GameObject exitPrefab;
    public float minExitDistance = 15f;

    [Header("Player")]
    public GameObject playerPrefab;

    Transform map;
    List<RoomInfo> infos = new List<RoomInfo>();

    struct RoomInfo
    {
        public GameObject prefab;
        public Vector3 size;
        public Vector3 pivotOffset;
    }
    NavMeshSurface surface;
    void Awake()
    {
        surface = GetComponent<NavMeshSurface>();
        surface.collectObjects = CollectObjects.Children;
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        surface.layerMask = LayerMask.GetMask("Navigation");
    }
    void Start()
    {
        map = new GameObject("Map").transform;
        map.SetParent(transform, true);

        BuildFloor();
        MeasurePrefabs();

        GameObject examRoomObj = null;
        bool valid = false;

        while (!valid)
        {

            valid = PlaceRooms(out examRoomObj);
            if (!valid)
            {
                for (int i = map.childCount - 1; i >= 0; i--)
                {
                    var obj = map.GetChild(i);
                    if (obj.name != "MapFloor")
                        DestroyImmediate(obj.gameObject);
                }
            }
        }
        surface.BuildNavMesh();

        if (playerPrefab != null && examRoomObj != null)
        {
            var bounds = CalcBounds(examRoomObj);
            float y = bounds.min.y + 0.1f;
            Vector3 spawnPoint = new Vector3(bounds.center.x, y, bounds.center.z);

            GameObject player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
            player.name = "Player";
        }
    }

    // 바닥 생성
    void BuildFloor()
    {
        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "MapFloor";
        floor.transform.SetParent(map, true);
        floor.transform.localScale = new Vector3(X_Axis / 10f, 1, Z_Axis / 10f);
        floor.transform.position = new Vector3(0, floor_Y_Axis, 0);

        if (floorMaterial != null)
            floor.GetComponent<Renderer>().sharedMaterial = floorMaterial;
        floor.isStatic = true;
    }

    //방 prefab 크기 측정
    void MeasurePrefabs()
    {
        foreach (var prefab in roomPrefabs)
        {
            var temp = Instantiate(prefab, Vector3.up * 1000f, Quaternion.identity);
            temp.transform.localScale = prefab.transform.localScale;
            var rends = temp.GetComponentsInChildren<Renderer>(true);
            var b = rends[0].bounds;
            foreach (var r in rends) b.Encapsulate(r.bounds);

            infos.Add(new RoomInfo
            {
                prefab = prefab,
                size = b.size,
                pivotOffset = b.center - temp.transform.position
            });
            DestroyImmediate(temp);
        }
    }

    //방 배치 - 모든 prefab 한번씩 사용 후 재사용 -> maxTry 횟수만큼 반복
    //prefab에 'Door_Frame'부분엔 벽 배치 안함 (출입구)
    bool PlaceRooms(out GameObject examRoomObj)
    {
        var placedBounds = new List<Bounds>();
        var placedRooms = new List<(GameObject obj, Transform[] doors)>();

        var unused = infos.ToList();
        System.Random rnd = new System.Random();
        unused = unused.OrderBy(_ => rnd.Next()).ToList();

        var reuse = infos
                    .Where(info => info.prefab != examRoomPrefab)
                    .ToList();

        int emptyStreak = 0;
        int maxEmptyStreak = maxTry;

        bool hasExamRoom = false;
        examRoomObj = null;

        while (emptyStreak < maxEmptyStreak)
        {
            RoomInfo info;
            if (unused.Count > 0)
            {
                info = unused[0];
                unused.RemoveAt(0);
            }
            else
            {
                info = reuse[Random.Range(0, reuse.Count)];
            }

            bool placed = false;
            for (int i = 0; i < maxTry; i++)
            {
                float x = Random.Range(
                    -X_Axis * .5f + info.size.x * .5f + outerMargin,
                     X_Axis * .5f - info.size.x * .5f - outerMargin);
                float z = Random.Range(
                    -Z_Axis * .5f + info.size.z * .5f + outerMargin,
                     Z_Axis * .5f - info.size.z * .5f - outerMargin);

                var testB = new Bounds(
                    new Vector3(x, 0, z),
                    new Vector3(info.size.x + innerMargin, info.size.y, info.size.z + innerMargin)
                );

                if (!placedBounds.Any(b => b.Intersects(testB)))
                {
                    var obj = Instantiate(info.prefab, map);
                    obj.transform.localScale = info.prefab.transform.localScale;
                    obj.transform.position = new Vector3(x, 0, z) - info.pivotOffset;

                    var floorR = obj.GetComponentsInChildren<Renderer>(true)
                                   .Where(r => r.gameObject.name.ToLower().Contains("floor"))
                                   .ToArray();
                    if (floorR.Length == 0)
                        floorR = obj.GetComponentsInChildren<Renderer>(true);
                    float minY = floorR.Min(r => r.bounds.min.y);
                    obj.transform.position += Vector3.up * (-minY + floor_Y_Axis);
                    obj.isStatic = true;

                    bool isExam = (examRoomPrefab != null && info.prefab == examRoomPrefab) || obj.name.ToLower().Contains("examroom");

                    if (isExam) examRoomObj = obj;

                    placedBounds.Add(CalcBounds(obj));

                    var doors = obj.GetComponentsInChildren<Transform>(true)
                                  .Where(t => t.name == "Door_Frame")
                                  .ToArray();
                    placedRooms.Add((obj, doors));

                    placed = true;
                    break;
                }
            }

            if (placed)
            {
                emptyStreak = 0;
            }
            else
            {
                emptyStreak++;
            }
        }

        hasExamRoom = placedRooms.Any(pair => pair.obj.name.StartsWith("ExamRoom"));

        if (!hasExamRoom)
            return false;

        foreach (var (go, doors) in placedRooms)
            CreateRoomWalls(go, doors);

        BuildOuterWalls();

        MakeExit(examRoomObj, placedRooms);

        return true;
    }



    // 방에 벽생성
    void CreateRoomWalls(GameObject room, Transform[] doors)
    {
        var b = CalcBounds(room);
        float xMin = b.min.x, xMax = b.max.x;
        float zMin = b.min.z, zMax = b.max.z;

        BuildWallSegments(xMin, xMax, zMax, true, doors);
        BuildWallSegments(xMin, xMax, zMin, true, doors);

        BuildWallSegments(zMin, zMax, xMax, false, doors);
        BuildWallSegments(zMin, zMax, xMin, false, doors);
    }

    void BuildWallSegments(
        float start, float end, float fixedC,
        bool horizontal, Transform[] doors)
    {
        var gaps = new List<float>();
        foreach (var d in doors)
        {
            float dist = horizontal
                ? Mathf.Abs(d.position.z - fixedC)
                : Mathf.Abs(d.position.x - fixedC);
            if (dist < 0.1f)
                gaps.Add(horizontal ? d.position.x : d.position.z);
        }

        if (gaps.Count == 0)
        {
            SpawnWall((start + end) * .5f, fixedC, end - start, horizontal);
            return;
        }

        gaps.Sort();
        float cur = start;
        foreach (var g in gaps)
        {
            float halfDoor = 1f * .5f;
            float left = g - halfDoor;
            float right = g + halfDoor;
            if (left > cur)
                SpawnWall((cur + left) * .5f, fixedC, left - cur, horizontal);
            cur = Mathf.Max(cur, right);
        }
        if (cur < end)
            SpawnWall((cur + end) * .5f, fixedC, end - cur, horizontal);
    }

    void SpawnWall(float mid, float fixedC, float length, bool horizontal)
    {
        Vector3 pos, scale;

        if (horizontal)
        {
            pos = new Vector3(mid, wallHeight * .5f, fixedC);
            scale = new Vector3(length, wallHeight, wallThickness);
        }
        else
        {
            pos = new Vector3(fixedC, wallHeight * .5f, mid);
            scale = new Vector3(wallThickness, wallHeight, length);
        }

        var w = GameObject.CreatePrimitive(PrimitiveType.Cube);
        w.name = "InnerWall";
        w.transform.SetParent(map, true);
        w.transform.position = pos;
        w.transform.localScale = scale;
        if (wallMaterial != null)
            w.GetComponent<Renderer>().sharedMaterial = wallMaterial;
    }

    // 외벽 생성
    void BuildOuterWalls()
    {
        float hx = X_Axis * .5f;
        float hz = Z_Axis * .5f;
        float t = wallThickness;

        SpawnWall(0, hz + t * .5f, X_Axis, true);
        SpawnWall(0, -hz - t * .5f, X_Axis, true);
        SpawnWall(0, hx + t * .5f, Z_Axis, false);
        SpawnWall(0, -hx - t * .5f, Z_Axis, false);
    }

    //출구 생성 - Exam Room을 기준으로 일정 거리 이상에서 생성
    void MakeExit(GameObject examRoom, List<(GameObject obj, Transform[] doors)> rooms)
    {
        Vector3 examCenter = CalcBounds(examRoom).center;

        var sel = rooms
            .Where(room => room.obj != examRoom)
            .Select(room => (room.obj, center: CalcBounds(room.obj).center))
            .Where(tar => Vector3.Distance(tar.center, examCenter) >= minExitDistance)
            .ToList();

        if (sel.Count == 0)
        {
            sel = rooms
                .Where(room => room.obj != examRoom)
                .Select(room => (room.obj, center: CalcBounds(room.obj).center))
                .OrderByDescending(tar => Vector3.Distance(tar.center, examCenter))
                .Take(1)
                .ToList();
        }

        var target = sel[Random.Range(0, sel.Count)];

        var targetBounds = CalcBounds(target.obj);
        Vector3 spawnPos = new Vector3(target.center.x, floor_Y_Axis, target.center.z);

        var exit = Instantiate(exitPrefab, spawnPos, Quaternion.identity, map);
        exit.name = "Exit";
        exit.isStatic = true;
    }

    // 경계 계산
    Bounds CalcBounds(GameObject go)
    {
        var rends = go.GetComponentsInChildren<Renderer>(true);
        Bounds b = rends[0].bounds;
        foreach (var r in rends) b.Encapsulate(r.bounds);
        return b;
    }
}