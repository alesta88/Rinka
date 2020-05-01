using UnityEngine;

[CreateAssetMenu( fileName = "language_text_data", menuName = "Create Language Text" )]
public class TextData : ScriptableObject {
    public SystemLanguage Language;
    [Header( "Native Text" )]
    public string NativeConfirmDiagText = "";
    [Header("Review Request")]
    public string AppReviewRequestTitle = "";
    public string AppReviewRequestMsg = "";
    [Header( "Ranking/Leaderboard" )]
    [TextArea( 1, 4 )] public string HighScoreSocialMsg;
    [TextArea( 1, 4 )] public string LastScoreSocialMsg;
    [TextArea( 1, 4 )] public string PromoteSocialMsg;
    [Header( "Ranking/Leaderboard with HashTag" )]
    [TextArea( 1, 4 )] public string HighScoreSocialMsgWithHashtag;
    [TextArea( 1, 4 )] public string LastScoreSocialMsgWithHashtag;
    [TextArea( 1, 4 )] public string PromoteSocialMsgWithHashtag;
    [Header( "No App Installed" )]
    public string NoAppInstalledTitle = "";
    [TextArea( 1, 4 )] public string NoAppInstalledMsg;
    [Header( "Success" )]
    public string RestoreSuccessTitle;
    [TextArea( 1, 4 )] public string RestoreSuccessMsg;
    [Header( "Errors" )]
    public string NoConnectionErrorTitle;
    [TextArea( 1, 4 )] public string NoConnectionErrorMsg;
    [Space( 10 )]
    public string RestoreErrorTitle;
    [TextArea( 1, 4 )] public string RestoreErrorMsg;
    [Space(10)]
    public string InitErrorTitle;
    [TextArea( 1, 4 )] public string InitErrorMsg;
    [Space( 10 )]
    public string PurchaseSettingsErrorTitle;
    [TextArea( 1, 4 )] public string PurchaseSettingsErrorMsg;
    [Space( 10 )]
    public string UnknownPurchaseErrorTitle;
    [TextArea( 1, 4 )] public string UnknownPurchaseErrorMsg;
    [Space( 10 )]
    public string AdvertisementDownloadingErrorTitle;
    [TextArea( 1, 4 )] public string AdvertisementDownloadingErrorMsg;
}
