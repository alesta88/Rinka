using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using UniRx;
using DG.Tweening;
using System.Collections;

/// <summary>
/// プレイヤー
/// </summary>
public class Player : MonoBehaviour {
    [Header( "Fly Movement" )]
    [SerializeField] public Vector2 m_flyVector;
    [Header( "Glide Movement" )]
    [SerializeField] public Vector2 m_glideVector;
    [SerializeField] public float m_glideDuration;
    [Header( "Fall Movement" )]
    [SerializeField] public Vector2 m_fallVector;
    [Header( "Left Movement" )]
    [SerializeField] public Vector2 m_leftVector;
    [SerializeField] public float m_LeftDuration;
    [Header( "Right Movement" )]
    [SerializeField] public Vector2 m_rightVector;
    [SerializeField] public float m_rightDuration;
    [Header( "----------------------------------------" )]
    [Header( "Death" )]
    [SerializeField] float m_deathIlluminationRadius;
    [SerializeField] Color m_deathColor;
    [SerializeField] Color m_clearColor;
    [SerializeField] Animator m_deathAnim;
    [SerializeField] SpriteRenderer m_deathSpriteRenderer;
    [Header( "----------------------------------------" )]
    [Header( "Inner Glow Speed" )]
    [SerializeField] [Range( 0f, 2f )] float m_innerGlowOnSpeed = 0f;
    [SerializeField] [Range( 0f, 2f )] float m_innerGlowOffSpeed = 0f;
    [Header( "Illumination Radius" )]
    [SerializeField] [Range( 0f, 1.5f) ] public float m_minIlluminateRadius = 0f;
    [SerializeField] [Range( 0f, 1.5f )] float m_maxIlluminateRadius = 0f;
    [Header( "Illumination Speed" )]
    [SerializeField] [Range( 0f, 2f )] float m_illuminateOnSpeed = 0f;
    [SerializeField] [Range( 0f, 20f )] float m_illuminateOffSpeed = 0f;
    [SerializeField] [Range( 0f, 2f )] float m_illuminateSwitchInterval = 0f;
    [Header( "----------------------------------------" )]
    [Header("Sprites")]
    [SerializeField] public SpriteRenderer m_wispSprite;
    [SerializeField] public SpriteRenderer m_wispInnerGlow;
    [SerializeField] public SpriteRenderer m_wispIllumination;
    [SerializeField] public SpriteRenderer m_skinsprite;
    [SerializeField] public GameObject m_skinObj;
    [SerializeField] public GameObject m_defaultSkinObj;
    [Header( "Other" )]
    [SerializeField] Rigidbody2D m_rb;

    public enum MoveState { Init, Fly, Glide, Fall, Die, Clear, Left, Right }
    public ReactiveProperty<MoveState> Move = new ReactiveProperty<MoveState>( MoveState.Init );
    public bool IsInvulnerable { set; get; }
    public bool CanInteract => Move.Value != MoveState.Die && GameModel.GameState.Value == Define.GameState.Playing;

    bool m_isPaused => ( GameModel.GameState.Value != Define.GameState.Playing );
    Sequence m_illuminateSeq;
    Tweener m_innerGlowTween;
    Action m_movePlayerAction = () => { };
    float m_glideTime;
    float m_leftTime;
    float m_rightTime;
    float? m_startPosX;
    float m_playTime;
    public int skinNumber;

    public bool flytoplayerevent;
    private float mytime;
    Text counterText;

    //***********************************************************
    // 初期化
    //***********************************************************
    void Awake() {
        var onMoveStateChanged = Move.TakeUntilDisable( this ).DistinctUntilChanged();
        onMoveStateChanged.Where( state => state == MoveState.Init ).Subscribe( _ => OnInit() );
        onMoveStateChanged.Where( state => state == MoveState.Fly ).Subscribe( _ => OnFly() );
        onMoveStateChanged.Where( state => state == MoveState.Glide ).Subscribe( _ => OnGlide() );
        onMoveStateChanged.Where( state => state == MoveState.Fall ).Subscribe( _ => OnFall() );
        onMoveStateChanged.Where( state => state == MoveState.Die ).Subscribe( _ => OnDie() );
        onMoveStateChanged.Where( state => state == MoveState.Clear ).Subscribe( _ => OnClear() );
        onMoveStateChanged.Where( state => state == MoveState.Left ).Subscribe( _ => OnLeft() );
        onMoveStateChanged.Where( state => state == MoveState.Right ).Subscribe( _ => OnRight() );

        GameModel.GameState.TakeUntilDisable( this ).Where( state => state != Define.GameState.Playing ).Subscribe( _ => OnPaused() );
        GameModel.GameState.TakeUntilDisable( this ).Where( state => state == Define.GameState.Playing ).Subscribe( _ => OnUnpause() );

        MessageBroker.Default.Receive<PlayerDeathEvent>().TakeUntilDisable( this ).Subscribe( _ => Move.Value = MoveState.Die );
        MessageBroker.Default.Receive<ClearStageEvent>().TakeUntilDisable( this ).Subscribe( _ => Move.Value = MoveState.Clear );
        MessageBroker.Default.Receive<ConsumeOrbEvent>().TakeUntilDisable( this ).Subscribe( orbEvent => OnPlayerConsumedOrb( orbEvent.Orb ) );
        m_skinsprite = m_wispSprite;
        m_skinObj = m_defaultSkinObj;
    }

    void OnPlayerConsumedOrb( OrbView orb ) {
        Illuminate();
        Debug.Log("ORBDATA " + orb.Data);
        StageMgr.Instance.ConsumedOrbsInStage++;
        SetWispSprite( orb.Data );
        
    }

    public void SetWispSprite( OrbData orbData ) {
        
        m_wispSprite.sprite = orbData.WispSprite;
        m_wispIllumination.sprite = orbData.WispIlluminationSprite;
    }

    public void SetSkinSprite(GameObject skinData) 
    {
        m_skinObj = skinData;
        m_skinsprite.sprite = skinData.GetComponent<SpriteRenderer>().sprite;
        Transform mySkin = this.gameObject.transform.Find("Skin");
        GameObject.Destroy(mySkin.gameObject);
        GameObject myNewSkin = Instantiate(skinData, this.transform.position, this.transform.rotation);
        myNewSkin.transform.parent = this.gameObject.transform;// mySkin;// 
        myNewSkin.name = "Skin";
         
    }

    void Illuminate() {
        if( m_illuminateSeq != null && m_illuminateSeq.IsPlaying() ) {
            m_illuminateSeq.Kill();
        }

        m_illuminateSeq = DOTween.Sequence();
        m_illuminateSeq.Append( m_wispIllumination.transform.DOScaleAtSpeed( m_maxIlluminateRadius, m_illuminateOnSpeed ).SetUpdate( true ) )
                       .AppendInterval( m_illuminateSwitchInterval )
                       .Append( m_wispIllumination.transform.DOScale( m_minIlluminateRadius, m_illuminateOffSpeed ).SetUpdate( true ) )
                       .SetUpdate( true )
                       .Play();
    }

    //***********************************************************
    // ポーズ関係
    //***********************************************************
    void OnPaused() {
        m_rb.velocity = Vector2.zero;
        m_rb.simulated = false;
        m_illuminateSeq.Pause();
        StopAllCoroutines();
    }

    void OnUnpause() {
        m_rb.simulated = true;
        m_illuminateSeq.Play();

        if(flytoplayerevent==true)
        {
            StartCoroutine(FlytoPL());
        }
    }

    //***********************************************************
    // 飛ぶ行動
    //***********************************************************
    void FixedUpdate() {
        if( m_isPaused || Move.Value == MoveState.Init )
            return;

        m_movePlayerAction();
    }

    void Update() {
        if( m_isPaused || Move.Value == MoveState.Init )
            return;

        //UpdateFlyState();
        UpdateFlyLeftState();
        UpdateDistance(); 
    }

    void UpdateFlyState() {
        if(Move.Value == MoveState.Glide) {
            m_glideTime += Time.deltaTime;
            if(m_glideTime > m_glideDuration) {
                Move.Value = MoveState.Fall;
            }
        } else {
            m_glideTime = 0f;
        }
        m_playTime += Time.deltaTime;
    }


    void UpdateFlyLeftState() {
        if (Move.Value == MoveState.Left) {
            m_leftTime += Time.deltaTime;
            if(m_leftTime > m_LeftDuration) {
                Move.Value = MoveState.Fall;
            }
        } else {
            m_leftTime = 0f;
        }
        m_playTime += Time.deltaTime;
    }
    
    void UpdateFlyRightState() {
        if (Move.Value == MoveState.Right) {
            m_rightTime += Time.deltaTime;
            if(m_rightTime > m_rightDuration) {
                Move.Value = MoveState.Fall;
            }
        } else { 
            m_rightTime = 0f;
        }
        m_playTime += Time.deltaTime;
    }

    void UpdateDistance() {
        if( !m_startPosX.HasValue ) {
            m_startPosX = transform.position.x;
        }

        float dis = ( transform.position.x - m_startPosX.Value ) * Define.DISTANCE_MULTIPLIER;
        if( (int)dis != GameModel.CurrentLifeDistance.Value ) {
            GameModel.CurrentLifeDistance.Value = (int)dis;
        }
    }

    //***********************************************************
    // ステート変更
    //***********************************************************
    public void OnInit() {
        m_playTime = 0f;
        m_illuminateSeq?.Kill();
        m_rb.velocity = Vector2.zero;
        EnableInteraction( false );
        m_wispSprite.enabled = true;
        m_wispInnerGlow.enabled = true;
        m_wispIllumination.transform.localScale = Vector3.one * m_minIlluminateRadius;
        m_wispIllumination.color = Color.white;
        m_movePlayerAction = () => { };
    }

    void OnFly() {
        EnableInteraction( true );
        PlayInnerGlowTween( isOn: true );

        m_movePlayerAction = () => m_rb.AddForce( m_flyVector * Time.fixedDeltaTime, ForceMode2D.Force );
    }

    void OnGlide() {
        EnableInteraction( true );
        PlayInnerGlowTween( isOn: false );
        m_movePlayerAction = () => m_rb.AddForce( m_glideVector * Time.fixedDeltaTime, ForceMode2D.Force );
    }

    void OnFall() {
        EnableInteraction( true );
        m_movePlayerAction = () => m_rb.velocity = m_fallVector * Time.fixedDeltaTime;
    }
    void OnLeft() {
        EnableInteraction( true );
        m_movePlayerAction = () => m_rb.AddForce(m_leftVector * Time.fixedDeltaTime, ForceMode2D.Force); 
    }
    void OnRight() {
        EnableInteraction( true );
        m_movePlayerAction = () => m_rb.AddForce(m_rightVector * Time.fixedDeltaTime, ForceMode2D.Force); 
    }

    void OnDie() {
        m_rb.simulated = false;
     //   m_deathAnim.SetTrigger("play_death");
        m_deathAnim.SetTrigger("play_clear");
        m_wispSprite.enabled = false;
        m_wispInnerGlow.enabled = false;
        m_wispIllumination.transform.localScale = Vector3.one * m_deathIlluminationRadius;
        m_wispIllumination.color = m_deathColor;
        m_movePlayerAction = () => { };

        Analytics.CustomEvent(Define.AnalyticsEvent.STAGE_SELECTION, new Dictionary<string, object>()
        {
            ["time"] = m_playTime
        });
        m_playTime = 0f;
    }

    void OnClear()
    {
        Debug.Log("ClearScene!"); 
      //  m_rb.simulated = false;
        //m_deathAnim.SetTrigger("play_clear");
        ////m_wispSprite.enabled = false;
        ////m_wispInnerGlow.enabled = false;
        //m_wispIllumination.transform.localScale = Vector3.one * m_deathIlluminationRadius;
        //m_wispIllumination.color = m_clearColor;
        //m_movePlayerAction = () => { };

       // Analytics.CustomEvent(Define.AnalyticsEvent.FULL_AD_START);
       // m_playTime = 0f;
    }

    public void PlayInnerGlowTween( bool isOn ) {
        if(m_innerGlowTween != null && m_innerGlowTween.IsPlaying()) {
            m_innerGlowTween.Kill();
        }
        m_innerGlowTween = isOn ?
            m_wispInnerGlow.DOFadeAtSpeed( 1f, m_innerGlowOnSpeed ).SetUpdate( true ) :
            m_wispInnerGlow.DOFadeAtSpeed( 0f, m_innerGlowOffSpeed ).SetUpdate( true );
    }

    void EnableInteraction( bool isEnabled ) {
        bool isPlaying = GameModel.GameState.Value == Define.GameState.Playing;
        m_rb.simulated = isPlaying && isEnabled;
        m_rb.gravityScale = isEnabled ? 0.5f : 0f;
    }

    //***********************************************************
    // Collisions
    //***********************************************************
    void OnTriggerEnter2D( Collider2D other ) {
        if( IsInvulnerable || Move.Value == MoveState.Init )
            return;

        if( other.transform.tag == Define.Tag.WALL ) {
            // 壁にぶつけた
            MessageBroker.Default.Publish( new PlayerDeathEvent() );
        } else if( other.tag == Define.Tag.ORB ) {
            // Orbの取得した
            Debug.Log("orb");
            MessageBroker.Default.Publish( new ConsumeOrbEvent( other.GetComponent<OrbView>() ) );
            //ParticleSystem PT01 = m_skinObj.GetComponentInChildren<ParticleSystem>();
            //ParticleSystem PT02 = PT01.GetComponentInChildren<ParticleSystem>();
            //ParticleSystem PT03 = PT01.GetComponentInChildren<ParticleSystem>();
            //PT02 = PT01.transform.GetChild(0).GetComponentInChildren<ParticleSystem>();
            //PT03 = PT01.transform.GetChild(1).GetComponentInChildren<ParticleSystem>();
            //Debug.Log("Particle           " + PT02 + " " + PT03);
            //var main = PT01.main;
            //main.startColor = new Color(1, 0, 1, .5f);
            //main = PT02.main;
            //main.startColor = new Color(1, 0, 1, .5f);
            //main = PT03.main;
            //main.startColor = new Color(1, 0, 1, .5f);
        }
        else if (other.tag == "Finish")
        {
            // Destroy(other.gameObject);

            m_illuminateSeq?.Kill();
            m_minIlluminateRadius = 1f;
            m_wispIllumination.transform.localScale = new Vector3(1f,1f,1f);
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAA");
          //  GameModel.Score.Value += 10;
            GameModel.Score.Value += GameModel.Stage.Value.PointsPerOrb*2;
            AudioMgr.Instance.PlayGetOrb();
            other.GetComponent<Animator>().SetTrigger("play_fly");
            m_wispIllumination.sprite = other.GetComponent<FlyToPlayer>().wispSprite;// orbData.WispIlluminationSprite;
          //  MessageBroker.Default.Publish(new ConsumeOrbEvent(other.GetComponent<OrbView>()));
            //StartCoroutine(ExecuteAfterTime2(0.5f));
        }
        //else
        //if (other.tag == "reload")
        //{
        //    this.m_minIlluminateRadius = 0.2f;
        //    m_illuminateSeq.Play();
        //    m_minIlluminateRadius = 0.2f;
        //    m_wispSprite.color = Color.white;
        //    Debug.Log("reload " + m_minIlluminateRadius);
        //}
        //else

        //if (other.tag == "Respawn")
        //{
        //    this.m_minIlluminateRadius = 0.2f;
        //    m_illuminateSeq.Play();
        //    m_minIlluminateRadius = 0.2f;
        //    m_wispSprite.color = Color.white;
        //    // MessageBroker.Default.Publish(new ClearStageEvent());
        //    GameModel.GameState.Value = Define.GameState.Clear;
        //    GameModel.StageWhenClear.Value = null;
        //    StageMgr.Instance.StagesCount++;
        //    StageMgr.Instance.ConsumedOrbsInStage = 0;
        //    StageMgr.Instance.isStageClear = true;
        //    Debug.Log("BBBBBBBBBBBBBBBBBBB "+m_minIlluminateRadius);
        //}
        /////////////////////////////////////////////newworld
        if (other.tag == "NewWorld")
        {
            StageMgr.Instance.StagesCount=0;
        }
        if (other.tag == "LockZ")
        {
            CameraMgr.Instance.CameraDeadZone(0.25f);
            CameraMgr.Instance.m_cinemachineCam.GetComponent<LockCameraZ>().enabled = false;
        }

        if (other.tag == Define.Tag.CLEAR)
        {
            MessageBroker.Default.Publish(new ClearStageEvent());
        }
        if (other.tag == "BonusText")
        {
            StageMgr.Instance.currentchunk_bonustext.BonusText.gameObject.SetActive(true);
            StageMgr.Instance.currentchunk_bonustext.CountdownBonusText.gameObject.SetActive(true);

            counterText = StageMgr.Instance.currentchunk_bonustext.CountdownBonusText.GetComponentInChildren<Text>() as Text;


            flytoplayerevent = true;
            mytime = 15f;
            StartCoroutine(FlytoPL());
            
            //StartCoroutine(ClearEvent(28f));
        }
        if (other.tag == "noText")
        {
            StageMgr.Instance.currentchunk_bonustext.BonusText.gameObject.SetActive(false);
            
        }
    }

    //***********************************************************
    // Destroy
    //***********************************************************
    void OnDestroy() {
        m_illuminateSeq.Kill();
        m_innerGlowTween.Kill();
    }

    IEnumerator FlytoPL()
    {
        StartCoroutine(StartCountdown());
        yield return new WaitForSeconds(mytime);
        flytoplayerevent = false;
        StartCoroutine(ClearEvent(1f));
        Debug.Log("UNPAUSE "+mytime);
       

    }

    public IEnumerator StartCountdown()
    {
        
        while (mytime > 0)
        {
            var intmytime = (int)mytime;
            counterText.text = intmytime.ToString() + " to clear";
            Debug.Log("Countdown: " + counterText.text);
            yield return new WaitForSeconds(2.0f);
            mytime--;
        }
    }
    IEnumerator ClearEvent(float time)
    {
        yield return new WaitForSeconds(time);
        this.m_minIlluminateRadius = 0.2f;
        this.m_illuminateSeq.Play();
        this.m_minIlluminateRadius = 0.2f;
        this.m_wispSprite.color = Color.white;
        this.transform.localScale = new Vector3(1f,1f,1f);
        Debug.Log("reloadevent " + this.transform.localScale);
        //StageMgr.Instance.currentchunk_bonustext.gameObject.SetActive(false);
        //   counterText.gameObject.SetActive(false);
        counterText.text = " ";
      //  StageMgr.Instance.onedead = true;
        CameraMgr.Instance.CameraDeadZone(1f);
        CameraMgr.Instance.m_cinemachineCam.GetComponent<LockCameraZ>().enabled = true;
        GameModel.StageWhenClear.Value = null;
        StageMgr.Instance.StagesCount++;
        StageMgr.Instance.ConsumedOrbsInStage = 0;
        StageMgr.Instance.isStageClear = true;
        StageMgr.Instance.oneclear = false;
        StageMgr.Instance.oneclearlast = true;
        Debug.Log("reloadevent2 " + m_minIlluminateRadius);
        GameModel.GameState.Value = Define.GameState.Clear;
        
    }


}
