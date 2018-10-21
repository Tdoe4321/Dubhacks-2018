using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Room : MonoBehaviour {

    public Transform[] exits;
    public Transform[] objectSpawns;
    public Transform[] paintingSpawns;
    public Transform textSpawn;
    public BoxCollider roomCollider;
    public AudioSource audioSource;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        audioSource.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        audioSource.Stop();
    }

    public void SetAudio(string path)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, path);
        string url = "file:///" + path;
        WWW weblink = new WWW(url);
        audioSource.clip = weblink.GetAudioClip();
    }

    
}
