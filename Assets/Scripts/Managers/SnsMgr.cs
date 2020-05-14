using UnityEngine;
using SA.Common.Models;
using SA.IOSNative.Social;

/// <summary>
/// SNS管理クラス
/// </summary>
public class SnsMgr : MonoSingleton<SnsMgr> {

//alestasns    public void DisplayRanking() {
//#if UNITY_ANDROID
//        GooglePlayMgr.Instance.DisplayRankingUI();
//#elif UNITY_IOS
//        GameCenterMgr.Instance.ShowLeaderboard();
//#endif
//    }

//    public void ReportScore() {
//#if UNITY_ANDROID
//        GooglePlayMgr.Instance.ReportScore();
//#elif UNITY_IOS
//        GameCenterMgr.Instance.ReportScore();
//#endif
//    }

//    /// <summary>
//    /// Twitterにポストする
//    /// </summary>
//    public void PostTwitterMessage() {
//        string snsMsg = System.Uri.EscapeDataString( DialogMgr.Instance.GetTwitterRankingMsg() );
//        snsMsg = Define.SocialPlatform.TWITTER + "post?message=" + snsMsg;
//        Debug.Log( snsMsg );
//#if UNITY_IOS
//        if( SA.IOSNative.System.SharedApplication.CheckUrl( Define.SocialPlatform.TWITTER ) ) {
//            SA.IOSNative.System.SharedApplication.OpenUrl( snsMsg );
//#else
//                    if( NativeMgr.IsAppInstalled( Define.SocialPlatform.TWITTER ) ) {

//                        Application.OpenURL( snsMsg );
//#endif
//        }
//        else
//        {
//            DialogMgr.Instance.DisplayNativeUnavailableApp();
//        }
//    }

//    Texture2D GetScreenTexture() {
//        Texture2D mTexture = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, mipChain: false );
//        mTexture.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0, recalculateMipMaps: false );
//        mTexture.Apply();
//        return mTexture;
//    }

//    public void PostFacebookMessage() {
//#if UNITY_EDITOR
//        Debug.Log( "DEBUG: Facebook msg " );
//#else
//#if UNITY_IOS
//        if( NativeMgr.IsAppInstalled( Define.SocialPlatform.FACEBOOK ) ) {
//            if( GameModel.GameState.Value == Define.GameState.Settings ) {
//                SA.IOSNative.Social.Facebook.Post( DialogMgr.Instance.GetSnsRankingMsg(), null );
//            } else {
//                SA.IOSNative.Social.Facebook.Post( DialogMgr.Instance.GetSnsRankingMsg(), GetScreenTexture() );
//            }
//        }
//#else
//        if( NativeMgr.IsAppInstalled( Define.SocialPlatform.FACEBOOK ) ) {
//            // TODO: Facebook統合
//            //FacebookMgr.Instance.PostTimeline( DialogMgr.Instance.GetSnsRankingMsg() );
//        }
//#endif
//        else {
//            DialogMgr.Instance.DisplayNativeUnavailableApp();
//        }
//#endif
//    }

//    public void PostLineMessage() {
//#if UNITY_EDITOR
//        Debug.Log( "DEBUG: LINE msg " );
//#else
//        if( NativeMgr.IsAppInstalled(  Define.SocialPlatform.LINE ) ) {
//            Application.OpenURL( Define.SocialPlatform.LINE + "msg/text/" + WWW.EscapeURL( DialogMgr.Instance.GetSnsRankingMsg() ) );
//        } else {
//            DialogMgr.Instance.DisplayNativeUnavailableApp();
//        }
//#endif
//    }

//    public void RequestAppReview() {
//#if UNITY_EDITOR
//        Debug.Log( "DEBUG: Review App" );
//#else
//#if UNITY_IOS
//        var ratePopup = IOSRateUsPopUp.Create( DialogMgr.Text.AppReviewRequestTitle, DialogMgr.Text.AppReviewRequestMsg );
//        ratePopup.OnComplete += RatePopup_OnComplete;
//#else
//        Application.OpenURL( "https://play.google.com/store/apps/details?id=com.moonisland.rinka2&reviewId=0" );
//#endif
//#endif
//    }

//#if UNITY_IOS
//    void RatePopup_OnComplete( IOSDialogResult result ) {
//        switch( result ) {
//            case IOSDialogResult.RATED : 
//                PlayerPrefs.SetInt( Define.PlayerPref.APP_REVIEW, 1 );
//                GameModel.IsAppReviewed.Value = true;
//                break;
//        }
//    }
//#endif
} 