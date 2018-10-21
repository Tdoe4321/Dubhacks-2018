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
    public OVRPlayerController player;
    private Map mapData;

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
        Instantiate(player, playerSpawn.position, playerSpawn.rotation);

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
            print(currentRoom.transform.position);

            // instanstiate room
            prevRoom = currentRoom;
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
