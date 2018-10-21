using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapController : MonoBehaviour {

    public Transform playerSpawn;
    public Transform level;
    public Room startRoom;
    public Room middleRoom;
    public Room endRoom;
    public Room corridor;
    public OVRCameraRig playerPrefab;
    public ArcTeleporter teleporterPrefab;
    private Map mapData;
    public GameObject paintingPrefab;
    public GameObject textMeshPro;

    private readonly string mapJsonFile = "map.json";

    // Use this for initialization
    void Start() {
        string jsonPath = Path.Combine(Application.streamingAssetsPath, mapJsonFile);
        print(jsonPath);
        if (File.Exists(jsonPath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(jsonPath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            mapData = JsonUtility.FromJson<Map>(dataAsJson);
        } else
        {
            print("error on reading json file");
        }

        Room prevRoom = Instantiate(startRoom, level);
        // instantiate player
        OVRCameraRig player = Instantiate(playerPrefab, playerSpawn.position, playerSpawn.rotation);
        ArcTeleporter teleporter = Instantiate(teleporterPrefab);
        teleporter.objectToMove = player.transform;
        teleporter.GetComponent<TallRaycaster>().trackingSpace = player.GetComponentInChildren<Transform>();

        // generate rooms
        for (int i = 0; i < mapData.rooms.Length; i++)
        {
            // room type is middle room unless this is last room
            Room roomType = (i == mapData.rooms.Length - 1) ? endRoom : middleRoom;

            RoomContent rc = mapData.rooms[i];

            Room currentRoom = Instantiate(roomType, level);

            // get the previous exit and this entrance
            Transform prevExit = prevRoom.exits[prevRoom.exits.Length - 1];
            Transform roomEntrance = currentRoom.exits[0];

            // calculate world offset based on previous exit

            currentRoom.transform.rotation = new Quaternion(roomEntrance.rotation.x - prevExit.rotation.x,
                roomEntrance.rotation.y - (prevExit.rotation.y + 180),
                roomEntrance.rotation.z - prevExit.rotation.z,
                roomEntrance.rotation.w - prevExit.rotation.w);
            currentRoom.transform.position = prevExit.position - currentRoom.transform.rotation * roomEntrance.localPosition;

            //room is created, fill with content
            for (int j = 0; j < rc.imagePaths.Length && j < currentRoom.paintingSpawns.Length; j++)
            {
                Texture2D texture = loadImage(new Vector2(100, 100), Path.Combine(Application.streamingAssetsPath, rc.imagePaths[j]));
                GameObject prefab = Instantiate(paintingPrefab, currentRoom.paintingSpawns[j].position, currentRoom.paintingSpawns[j].rotation);
                prefab.GetComponent<Renderer>().material.mainTexture = texture;
            }

            GameObject tmp = Instantiate(textMeshPro, currentRoom.textSpawn.position, currentRoom.textSpawn.rotation);
            tmp.GetComponent<TMPro.TextMeshPro>().text = rc.text;

            // instanstiate room
            prevRoom = currentRoom;
        }
    }

	// Update is called once per frame
	void Update () {
		
	}

    private static Texture2D loadImage(Vector2 size, string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D((int)size.x, (int)size.y, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Trilinear;
        texture.LoadImage(bytes);

        return texture;
    }
}
