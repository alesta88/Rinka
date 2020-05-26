using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWalls : MonoBehaviour {

    public GameObject[] SpawnObject;
    public GameObject[] SpawnTopObject;
    public GameObject[] SpawnBottomObject;
    public GameObject[] SpawnBorders;
    public int SpawnCount;
    private int MinSpawnCount;
    private int randomINT;
    int StCount;

    [Header("----------------------------------------")]
    [Header("Particles")]
    //public ParticleSystem[] SpawnParticles;
    public GameObject[] SpawnParticles;
    public int ParticleCount;
    StageFlowData m_stageFlow;

    float SpriteWidthScale;
    float SpriteHeightScale;
    float DifficultyRatio;
    int m_wallLayer;
    Collider2D[] m_colOverlaps = new Collider2D[5];
    int[] randomMiddleCount;// = new int[5];

    bool IsPointTouchingCollider(Vector3 point, int layer, float WScale)
    {
        int colCnt = Physics2D.OverlapCircleNonAlloc(point, WScale, m_colOverlaps, layer);
        return colCnt >= 1;
    }
    void Awake()
    {
        m_wallLayer = 1 << LayerMask.NameToLayer("GenerateWall");

    }

    private void Start()
    {
        GameObject BorderTopObject = Instantiate(SpawnBorders[0], new Vector3(this.transform.position.x, +0.85f, 0), this.transform.rotation);
        GameObject BorderBottomObject = Instantiate(SpawnBorders[1], new Vector3(this.transform.position.x, -0.85f, 0), Quaternion.Euler(0f, 0f, 180f));
        BorderTopObject.transform.parent = gameObject.transform;
        BorderBottomObject.transform.parent = gameObject.transform;



        if (GameModel.StageWhenClear.Value == null)//(GameModel.Stage.Value.Difficulty!=9)
        {
            Spawner();
        }
        else
        { 
            ParticleSpawner();
        }
    }


    void Spawner()
    {

     //   FindRandomTopObj();

        Vector3 MyPosition = this.transform.position;
        MyPosition.x = this.transform.position.x - 2f;
        //    Debug.Log("POSITION " + this.transform.position);
       
        for (int i = 0; i <= SpawnCount; i++)
        {
            FindRandomTopObj();

            MyPosition.y = (0.47f - SpriteHeightScale / 2000f);
            int colCnt1 = Physics2D.OverlapCircleNonAlloc(MyPosition, 0.1f+SpriteHeightScale / 2000f, m_colOverlaps, m_wallLayer); 
            //int colCnt2 = Physics2D.OverlapCircleNonAlloc(MyPosition, 0.1f+SpriteWidthScale / 2000f, m_colOverlaps, m_wallLayer); 
            while (colCnt1 >= 1)
            {
                MyPosition.x += SpriteWidthScale / 2000f;
                colCnt1 = Physics2D.OverlapCircleNonAlloc(MyPosition, 0.1f + SpriteHeightScale / 2000f, m_colOverlaps, m_wallLayer);
             //   Debug.Log("SpriteHeight " + SpriteHeightScale / 2000f + " my pos " + MyPosition.x + " " + MyPosition.y + " cnt " + colCnt1);
            }
            if (MyPosition.x <= transform.position.x + 3.2f)
            {
                Debug.Log("Spawn top OBJ " + SpawnTopObject[randomINT] + " " + randomINT);
                GameObject spawnObject = Instantiate(SpawnTopObject[randomINT], MyPosition, Quaternion.identity);
                spawnObject.transform.parent = gameObject.transform;
            }


            if(i%3==0)
            {
                FindRandomMiddleObj();
                MyPosition.y = 0f;
                colCnt1 = Physics2D.OverlapCircleNonAlloc(MyPosition, 0.1f + SpriteHeightScale / 2000f, m_colOverlaps, m_wallLayer);
                while (colCnt1 >= 1)
                {
                    MyPosition.x += SpriteWidthScale / 2000f;
                    colCnt1 = Physics2D.OverlapCircleNonAlloc(MyPosition, 0.1f + SpriteHeightScale / 2000f, m_colOverlaps, m_wallLayer);
                }
                if (MyPosition.x <= transform.position.x + 3.2f)
                {
                    GameObject spawnObject2 = Instantiate(SpawnObject[randomINT], MyPosition, Quaternion.Euler(0f, 0f, 0f));
                    spawnObject2.transform.parent = gameObject.transform;
                }
            }
           





            FindRandomBottomObj();
            MyPosition.y = (-0.47f + SpriteHeightScale / 2000f);
            colCnt1 = Physics2D.OverlapCircleNonAlloc(MyPosition, 0.1f + SpriteHeightScale / 2000f, m_colOverlaps, m_wallLayer);
           // colCnt2 = Physics2D.OverlapCircleNonAlloc(MyPosition, 0.1f + SpriteWidthScale / 2000f, m_colOverlaps, m_wallLayer);
            while (colCnt1 >= 1)
            {
                MyPosition.x += SpriteWidthScale / 2000f;
                colCnt1 = Physics2D.OverlapCircleNonAlloc(MyPosition, 0.1f + SpriteHeightScale / 2000f, m_colOverlaps, m_wallLayer);
            //    Debug.Log("SpriteHeight " + SpriteHeightScale / 2000f + " my pos " + MyPosition.x + " " + MyPosition.y + " cnt " + colCnt1);
            }
            if (MyPosition.x <= transform.position.x + 3.2f)
            {
                Debug.Log("Spawn OBJ " + SpawnBottomObject[randomINT] + " " + randomINT);
                GameObject spawnObject2 = Instantiate(SpawnBottomObject[randomINT], MyPosition, Quaternion.Euler(0f, 0f, 180f));
                spawnObject2.transform.parent = gameObject.transform;
            }
        }
            
    }

    void FindRandomTopObj()
    {
        int min; 
        int max;
        StCount = StageMgr.Instance.StagesCounter;

        if (SpawnTopObject.Length>=StCount+SpawnTopObject.Length/2)
        {
            min = StCount-1;
        }
        else
        {
            min = SpawnTopObject.Length - SpawnTopObject.Length / 2-1;
        }

        if(StCount< SpawnTopObject.Length)
        {
            max = SpawnTopObject.Length/2 + StCount-1;
        }
        else
        {
            max = SpawnTopObject.Length-1;
        }
    //    Debug.Log("min " + min + " max " + max+" size "+ SpawnTopObject.Length+" size/2 "+ SpawnTopObject.Length/2+" stages counter "+ StCount);
        randomINT = Random.Range(min, max);//RandomExcept(min, max, randomINT);//
     //   Debug.Log("random int " + randomINT);
        SpriteWidthScale = SpawnTopObject[randomINT].GetComponent<SpriteRenderer>().sprite.texture.width;
        SpriteHeightScale = SpawnTopObject[randomINT].GetComponent<SpriteRenderer>().sprite.texture.height;
        //Debug.Log("W " + SpriteWidthScale + " H " + SpriteHeightScale);
    }
    void FindRandomBottomObj()
    {
        int min;
        int max;
        StCount = StageMgr.Instance.StagesCounter;
        if (SpawnBottomObject.Length >= StCount + SpawnBottomObject.Length / 2)
        {
            min = StCount-1;
        }
        else
        {
            min = SpawnBottomObject.Length - SpawnBottomObject.Length / 2-1;
        }

        if (StCount < SpawnBottomObject.Length)
        {
            max = SpawnBottomObject.Length / 2 + StCount-1;
        }
        else
        {
            max = SpawnBottomObject.Length-1;
        }
        randomINT = Random.Range(min, max);//RandomExcept(min, max, randomINT);
                                           // randomINT = Random.Range(0, SpawnBottomObject.Length);
        SpriteWidthScale = SpawnBottomObject[randomINT].GetComponent<SpriteRenderer>().sprite.texture.width;
        SpriteHeightScale = SpawnBottomObject[randomINT].GetComponent<SpriteRenderer>().sprite.texture.height;
        //Debug.Log("W " + SpriteWidthScale + " H " + SpriteHeightScale);
    }

    void FindRandomMiddleObj()
    {
        randomINT = Random.Range(0, SpawnObject.Length);
        SpriteWidthScale = SpawnObject[randomINT].GetComponent<SpriteRenderer>().sprite.texture.width;
        SpriteHeightScale = SpawnObject[randomINT].GetComponent<SpriteRenderer>().sprite.texture.height;
        //Debug.Log("W " + SpriteWidthScale + " H " + SpriteHeightScale);
    }

    public int RandomExcept(int min, int max, int except)
    {
        int random = Random.Range(min, max);
        if (random == except) random = min;
        return random;
    }/// <summary>
    /// list?
    /// </summary>

    void ParticleSpawner()
    {
        float dataWidth = GetComponentInParent<StageChunkView>().transform.position.x;
        float dataHeight = GetComponentInParent<StageChunkView>().transform.position.y;
       // const float MYSTERY_NUMBER = 0.027f;
      //  float spawnAreaWidth = dataWidth * MYSTERY_NUMBER;
       // float spawnAreaHeight = dataHeight * MYSTERY_NUMBER;
       if(ParticleCount!=0)
        for (int i = 0; i <= ParticleCount; i++)
        {
            float spawnX = Random.Range(dataWidth-0.7f, dataWidth+3f);
            float spawnY = Random.Range(dataHeight-0.4f, dataHeight+0.4f);
            GameObject spawnParticle = Instantiate(SpawnParticles[Random.Range(0,4)], new Vector3(spawnX,spawnY,0f), Quaternion.identity);
            spawnParticle.transform.parent = gameObject.transform;
        }
      //  GameModel.StageC.Value = null;
    }


    public void Update()
    {
        
    }















    //public GameObject[] SpawnObject_top;
    //public GameObject[] SpawnBorders;
    //public int SpawnCount;
    //private int randomINT;
    //Quaternion q;
    //Vector3 Pos;
    //Vector3 ObjectNullPOS;
    //int SpriteWidthScale;
    //int SpriteHeightScale;
    //float test=0f;
    //void Awake()
    //{
    //    m_wallLayer = 1 << LayerMask.NameToLayer("StageWall");

    //}


    //void Start () {

    //    ObjectNullPOS = this.transform.position;
    //     q = this.transform.rotation;
    //     Pos = this.transform.position;
    //    Pos.x = this.transform.position .x- 3.2f;
    //    Pos.y = 0.4f;
    //    SpawnRandom(0.4f,-1f);
    //    Pos.x = this.transform.position.x - 3.2f;
    //   q.z += 180f;
    //  //s  SpawnRandomBottom(-0.4f, 1f);
    //    GameObject BorderTopObject = Instantiate(SpawnBorders[0], new Vector3(this.transform.position.x, +0.4f, 0), this.transform.rotation);
    //    GameObject BorderBottomObject = Instantiate(SpawnBorders[1], new Vector3(this.transform.position.x, -0.4f, 0), Quaternion.Euler(0f, 0f, 180f));
    //    BorderTopObject.transform.parent = gameObject.transform;
    //    BorderBottomObject.transform.parent = gameObject.transform;
    //}

    //void SpawnRandom(float ypos,float ypos2)
    //{

    //    for (int i = 0; i < SpawnCount; i++)
    //    {
    //        randomINT = Random.Range(0, SpawnObject_top.Length);
    //        SpriteWidthScale = SpawnObject_top[randomINT].GetComponent<SpriteRenderer>().sprite.texture.width;
    //        SpriteHeightScale = SpawnObject_top[randomINT].GetComponent<SpriteRenderer>().sprite.texture.height;
    //        //ypos2 = SpriteHeightScale / 1000f;
    //        Pos.y = 0.4f;

    //       if (!IsPointTouchingCollider(Pos, m_wallLayer))
    //        {
    //            Debug.Log("pos " + Pos);
    //            Debug.Log("scale " + SpriteWidthScale / 3000f);
    //            Debug.Log("scale2 " + SpriteWidthScale / 1000f);
    //            Debug.Log("scale3 " + SpriteHeightScale / 1000f);
    //            Pos.y -= (-0.05f+SpriteHeightScale / 2000f);
    //            GameObject spawnObject = Instantiate(SpawnObject_top[randomINT], Pos, q);
    //            spawnObject.transform.parent = gameObject.transform;
    //            Debug.Log("bool " + IsPointTouchingCollider(Pos, m_wallLayer));
    //        }
    //      if (Pos.x <= this.transform.position.x + 3.2f)
    //        {
    //            Debug.Log("pos12358 " + this.transform.position);
    //            Pos.x += (0.02f+ SpriteWidthScale / 1000f);
    //              // 0.5f;
    //        }
    //    }
    //}

    //void SpawnRandomBottom(float ypos, float ypos2)
    //{

    //    for (int i = 0; i < SpawnCount; i++)
    //    {
    //        randomINT = Random.Range(0, SpawnObject_top.Length);
    //        SpriteWidthScale = SpawnObject_top[randomINT].GetComponent<SpriteRenderer>().sprite.texture.width;
    //        SpriteHeightScale = SpawnObject_top[randomINT].GetComponent<SpriteRenderer>().sprite.texture.height;
    //        ypos2 = SpriteHeightScale / 1000f;
    //        Pos.y = ypos + ypos2;

    //         if (!IsPointTouchingCollider(Pos, m_wallLayer))
    //        {
    //            Debug.Log("pos " + Pos);
    //            Debug.Log("scale " + SpriteWidthScale / 3000f);
    //            Debug.Log("scale2 " + SpriteWidthScale / 1000f);
    //            Debug.Log("scale3 " + SpriteHeightScale / 1000f);
    //            Pos.y -= (-0.05f + SpriteHeightScale / 2000f);
    //            GameObject spawnObject = Instantiate(SpawnObject_top[randomINT], Pos, q);
    //            spawnObject.transform.parent = gameObject.transform;
    //            Debug.Log("bool " + IsPointTouchingCollider(Pos, m_wallLayer));
    //        }
    //        if (Pos.x <= this.transform.position.x + 3.2f)
    //        {
    //            Debug.Log("pos12358 " + this.transform.position);
    //            Pos.x += (0.02f + SpriteWidthScale / 1000f);
    //            // 0.5f;
    //        }
    //    }
    //}

    //int m_wallLayer;
    //Collider2D[] m_colOverlaps = new Collider2D[5];
    //bool IsPointTouchingCollider(Vector3 point, int layer)
    //{
    //    int colCnt = Physics2D.OverlapCircleNonAlloc(point, /*SpriteWidthScale / 3000f*/0.15f, m_colOverlaps, layer);
    //    return colCnt >= 1;
    //}

}
