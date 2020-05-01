using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class SkinChangeScript : MonoBehaviour, ISceneController
{
    [Header("Buttons")]
    [SerializeField] Image m_backIcon;
    [SerializeField] Image m_skipAdsImage;
    [SerializeField] Image m_nextSkinImage;
    [SerializeField] Image m_setSkinImage;

    [Header("*********")]
    public Player MyPlayer;
    public GameObject NowSkin;
  //  public GameObject OldSkin;

    public GameObject[] NewSkins;
    int skinNum;

    
    private void Awake()
    {
        MyPlayer = FindObjectOfType<Player>();
    }
    void Start()
    {
        skinNum = 0;/////////////////
        
        // this.GetComponent<SpriteRenderer>().sprite = MyPlayer.m_skinsprite.sprite;
        NowSkin = MyPlayer.m_skinObj;
        //  OldSkin = MyPlayer.m_skinObj;
        GameObject mySkin = Instantiate(NowSkin, this.transform.position, this.transform.rotation);
        mySkin.transform.parent = this.gameObject.transform;
        //   LeftChoise();
        BindModelToUI();
        BindInput();
    }

    public void LeftChoise()
    {
        // PlayerMgr.Instance.PlayerInstance.SetWispSprite(OrbMgr.Instance.DefaultOrb);//.PlayerInstance.m_wispSprite.sprite = NewSkins[i].GetComponent<SpriteRenderer>().sprite;
        GameObject.Destroy(this.transform.GetChild(0).gameObject);
        skinNum++;
        if(skinNum<=2)
        {
            // PlayerMgr.Instance.PlayerInstance.SetSkinSprite(NewSkins[skinNum]);
            NowSkin = NewSkins[skinNum];
        }
        else
        {
            skinNum = 0;
            NowSkin = NewSkins[skinNum];
            // PlayerMgr.Instance.PlayerInstance.SetSkinSprite(NewSkins[skinNum]);
        }
      //  NowSkin = MyPlayer.m_skinObj;
        GameObject mySkin= Instantiate(NowSkin, this.transform.position, this.transform.rotation);
        mySkin.transform.parent = this.gameObject.transform;
        //  this.GetComponentInChildren<GameObject>(). = NowSkin;

        // this.GetComponent<SpriteRenderer>().sprite = MyPlayer.m_skinsprite.sprite;
    }

    public void SetSkin(GameObject Skin)
    {
        PlayerMgr.Instance.PlayerInstance.SetSkinSprite(Skin); 
    }


    void BindModelToUI()
    {
        GameModel.IsAdSkipPurchased
            .TakeUntilDestroy(this)
            .Subscribe(isAdSkipPurchased => m_skipAdsImage.gameObject.SetActive(!isAdSkipPurchased));
        m_skipAdsImage.gameObject.SetActive(!GameModel.IsAdSkipPurchased.Value);
    }

    void BindInput()
    {
        // 戻るボタンが押された
        int i=0;
       
        m_backIcon.ObservableButton(this).Subscribe(_ => GameModel.GameState.Value = Define.GameState.StageSelection);
        m_nextSkinImage.ObservableButton(this).Subscribe(_ => LeftChoise());
        m_setSkinImage.ObservableButton(this).Subscribe(_ => SetSkin(NowSkin));
        m_skipAdsImage.ObservableButton(this).Subscribe(_ => RinkaPurchaseManager.Instance.Buy(ProductModel.Instance.ProductInfos[0]));
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSceneActive()
    {
    }

    public void OnSceneInactive()
    {
    }
}
