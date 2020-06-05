using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyToPlayer : MonoBehaviour {

    public GameObject myPlayer;
    Player njhf;
    float changeSpeed;
    Collider2D[] m_colOverlaps = new Collider2D[5];
    int m_wallLayer;
    int touch=0;
    public int t;
    int colCnt1 = 0;
    public GameObject ps_child;
    public Color myColor;
    public Sprite wispSprite;
    public GameObject plusPointsText;
    [SerializeField] Animator m_flyhAnim;

    void Awake()
    {
        m_wallLayer = 1 << LayerMask.NameToLayer("Orb");

    }
    private void Start()
    {
        myPlayer = FindObjectOfType<Player>().gameObject;
        njhf = FindObjectOfType<Player>();
        t = Random.Range(5, 8);
        ps_child.SetActive(false);
        changeSpeed = 0.1f;
       
    }
    private void Update()
    {
        // Random.Range(0f, 1.5f);

        if(PlayerMgr.Instance.PlayerInstance.flytoplayerevent==true)
        {
            if (njhf.transform.localScale.x >= 1f)
            {
                njhf.transform.localScale = new Vector3(njhf.transform.localScale.x - 0.01f, njhf.transform.localScale.y - 0.01f, njhf.transform.localScale.z - 0.01f);
            }

            colCnt1 = Physics2D.OverlapCircleNonAlloc(myPlayer.transform.position, 3f, m_colOverlaps, m_wallLayer);
            if (touch >= 1)
            {

                this.transform.position = Vector3.Lerp(transform.position
                , myPlayer.transform.position
                , changeSpeed);
                //this.GetComponent<SpriteRenderer>().enabled = false;
                // PlayerMgr.Instance.PlayerInstance.max
                //     changeSpeed += 0.1f;
                StartCoroutine(ExecuteAfterTime2(1f));
                StartCoroutine(ExecuteAfterTime(0.5f));
                // changeSpeed += 0.05f;
                njhf.m_minIlluminateRadius = 1f;
                //  njhf.m_wispIllumination.transform.localScale = Vector3.one * 2f;

            }
        }
        
       

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PlayerMgr.Instance.PlayerInstance.flytoplayerevent == true)
        {
            Debug.Log("asasasasasas" + this.myColor);// collision.gameObject.GetComponent<SpriteRenderer>().color);
            touch = 1;
            m_flyhAnim.SetTrigger("play_fly");
            // var a= Instantiate(plusPointsText);
            StartCoroutine(ExecutPointsPlus(0.5f));
            Destroy(this.gameObject, 1f);
        //    Destroy(a, 1f);
            //Color.cyan; //new Color(0f, 0f, 0f, 1f); ;

            //  ps_child.SetActive(true);
        }
    }



    IEnumerator ExecutPointsPlus(float time)
    { 
        yield return new WaitForSeconds(time);

        var a = Instantiate(plusPointsText);
        a.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "+ "+(GameModel.Stage.Value.PointsPerOrb * 2).ToString();// "AAAAAA";
        Destroy(a, 1f);
    }




        IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        this.transform.position = Vector3.Lerp(transform.position
            , myPlayer.transform.position
            , changeSpeed);
        this.GetComponent<SpriteRenderer>().enabled = false;
        njhf.m_wispSprite.GetComponent<SpriteRenderer>().color = this.myColor;
        /////////////
        //var skinskin=njhf.transform.Find("Skin");
        //Debug.LogError(skinskin.childCount);
        //ParticleSystem.MainModule settings = skinskin.GetComponentInChildren<ParticleSystem>().main; 
        //settings.startColor = new ParticleSystem.MinMaxGradient(myColor);
            ///////////// 
        if (njhf.transform.localScale.x <= 1.5f)
        {
            njhf.transform.localScale = new Vector3(njhf.transform.localScale.x + 0.1f, njhf.transform.localScale.y + 0.1f, njhf.transform.localScale.z + 0.1f);//new Vector3(1.15f,1.15f,1.15f);
        }
        ps_child.SetActive(true);
       // StartCoroutine(ExecuteAfterTime2(0.5f));
    }
    IEnumerator ExecuteAfterTime2(float time)
    {
        yield return new WaitForSeconds(time);

        this.GetComponent<SpriteRenderer>().enabled = false;
        
    }

    //void findChildrens(Transform obj, Color newcolor)
    //{
    //    ParticleSystem.MainModule settings = obj.GetComponentInChildren<ParticleSystem>().main;
    //    settings.startColor = new ParticleSystem.MinMaxGradient(newcolor);
    //    var newobj = obj.GetComponentsInChildren<ParticleSystem>();
    //}

}
