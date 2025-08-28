public interface IZPosHandler
{
    int ZPos { get; set; }
    System.Action OnDeathZPosHolder { get; set; }
}