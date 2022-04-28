[System.Serializable]
public class MateSpecs
{
    public enum MateType
    {
        Toad,
        Koopa
    }

    public MateType Type;
    public PlayerSpecs MateSpec;
}
