using UnityEngine;
using UniRx;

/// <summary>
/// AudioMgrクラス
/// </summary>
public class AudioMgr : MonoSingleton<AudioMgr> {
    [SerializeField] AudioListener m_audioListener;
    [Header( "Audio Files" )]
    [SerializeField] AudioSource m_titleBgm;
    [SerializeField] AudioSource m_playBgm;
    [SerializeField] AudioSource m_getOrb;
    [SerializeField] AudioSource m_death;
    [SerializeField] AudioSource m_newHighScore;
    [SerializeField] AudioSource m_tapButton;


    /// <summary>
    /// 音を有効化
    /// </summary>
    public void PlayDeath() => PlaySeClip( m_death );
    public void PlayGetOrb() => PlaySeClip( m_getOrb );
    public void PlayNewHighScore() => PlaySeClip( m_newHighScore );
    public void PlayTapButton() => PlaySeClip( m_tapButton );

    public void PlayTitleBgm() {
        if( m_titleBgm.isPlaying )
            return;

        PlaySeClip( m_titleBgm );
        m_playBgm.Stop();
    }

    public void PlayGameBgm() {
        if( m_playBgm.isPlaying )
            return;

        PlaySeClip( m_playBgm );
        m_titleBgm.Stop();
    }

    public void StopBgm() {
        m_titleBgm.Stop();
        m_playBgm.Stop();
    }

    void PlaySeClip( AudioSource source ) {
        if( !GameModel.IsAudioOn.Value )
            return;

        source.Play();
    }
}