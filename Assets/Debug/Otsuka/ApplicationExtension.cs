

/// <summary>
/// 不要な可能性が高く、後々削除を検討中
/// </summary>
public class ApplicationExtension : MonoSingleton<ApplicationExtension> {
    public enum OperatingSystem {
        Windows,
        iOS,
        Android,
    }

    public OperatingSystem operatingSystem { get; private set; }

    void Awake() {
#if UNITY_EDITOR
        operatingSystem = OperatingSystem.Windows;
#elif UNITY_ANDROID
        operatingSystem = OperatingSystem.Android;
#elif UNITY_IPHONE
        operatingSystem = OperatingSystem.iOS;
#endif
    }
}