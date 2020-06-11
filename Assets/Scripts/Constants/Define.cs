using UnityEngine;

public static class Define {

    public static class SocialPlatform {
#if UNITY_IOS
        public const string FACEBOOK = "fb://";
#else
        public const string FACEBOOK = "facebook://";
#endif
        public const string TWITTER = "twitter://";
        public const string LINE = "line://";
    }

    public static class Ranking {
        public const string RANKING_ID = "CgkIiab0_s8MEAIQAQ";
        public enum MsgType { LastScore, HighScore }
        public static string WEBSITE_URL => Application.systemLanguage == SystemLanguage.Japanese ?
            "http://rinka.moonisland.jp" :
            "http://rinka.moonisland.jp/en";
        public static string FACEBOOK_POST_TITLE => Application.systemLanguage == SystemLanguage.Japanese ?
            "Rinkaベストスコア" :
            "Rinka High Score";
    }

    public static class PlayerPref {
        public const string GOOGLE_PLAY_USER_ID = "GooglePlayUserId";
        public const string TUTORIAL = "Tutorial";
        public const string AUDIO_VOLUME = "AudioVolume";
        public const string APP_REVIEW = "AppReview";
        public const string IS_AD_SKIP_PURCHASED = "HasPurchased";
        public const string HIGH_SCORE = "HighScore";
    }

    public static class Scene {
        public const string PERMANENT = "PermanentScene";
        public const string TITLE = "TitleScene";
        public const string STAGESELECTION = "StageSelectionScene";
        public const string PLAY = "PlayScene";
        public const string CONTINUE = "ContinueScene";
        public const string CONTINUE1 = "ContinueScene1";
        public const string GOTOBONUS = "GoToBonusScene";
        public const string GAMEOVER = "GameOverScene";
        public const string SKIN = "SkinChangeScene";
    }

    public static class Tag {
        public const string PLAYER = "player";
        public const string WALL = "wall";
        public const string ORB = "orb";
        public const string DISTANCE_MARK = "distance_mark";
        public const string CLEAR = "clear";
    }

    public static class AnalyticsEvent {
        public const string STAGE_SELECTION = "stage_selection";
        public const string PLAY_TIME = "total_play_time";
        public const string STAGE_OF_DEATH = "stage_of_death";
        public const string SKIPPABLE_AD_START = "skippable_ad_start";
        public const string SKIPPABLE_AD_FINISH = "skippable_ad_finish";
        public const string FULL_AD_START = "skippable_ad_start";
        public const string FULL_AD_FINISH = "skippable_ad_finish";
    }

    public const float INIT_TIME_SCALE = 1.1f;
    public static Vector2 Origin => Vector3.zero;
    public const float DISTANCE_MULTIPLIER = 4.5f;
    public const float MAX_GAME_SPEED = 30f;
    public const int PLAYER_LIVES_PER_GAME = 2;
    public const int IGNORE_ADS_AFTER_PLAY_SEC = 10;
    public const int DISTANCE_NOTIFICATION = 10; // meters

    public enum Platform {
        Editor,
        Android,
        iOS,
    }

    public enum Audio {
        TitleBgm,
        PlayBgm,
        GetOrb,
        Death,
        NewHighScore,
        TapButton,
    }

    public enum OrbType {
        White,
        Blue,
        Green,
        Orange,
        Red,
    }

    public enum SkinType
    {
        one,
        two,
        three, 
    }

    public enum GameState {
        Tutorial,
        Title,
        Settings,
        StageSelection,
        Playing,
        Paused,
        UnpauseCountdown,
        Continue,
        GoToBonus,
        PlayStageSelectionAd,
        PlayContinueAd,
        GameOver,
        Clear,
        Skin
    }

    public enum GameSubState
    {
        BonusTime,
        BackTo,
        NoBonusTime
    }

}