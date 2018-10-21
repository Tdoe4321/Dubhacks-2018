using System;

[Serializable]
public class RoomContent {
    public String text;
    public float sentiment;
    public String[] imagePaths;
    public String audioPath;
    public String[] associatedTags;
}
