using UnityEngine;

public class Settings : MonoBehaviour
{
    public int FrameRate = 20;
    void Start()
    {
        Application.targetFrameRate = FrameRate;
    }
}
