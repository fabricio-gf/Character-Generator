[System.Serializable]
public class CharacterProfile
{
    public string ProfileName;
    public int[] ProfileValues;

    public CharacterProfile(string name, int[]values)
    {
        ProfileName = name;
        ProfileValues = values;
    }
}
