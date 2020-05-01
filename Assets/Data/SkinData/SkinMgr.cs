using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Analytics;

public class SkinMgr :  MonoSingleton<SkinMgr>
{
    [SerializeField] SkinData[] m_skinData;

    public SkinData GetSkinData()
    {
        int skinnum = PlayerMgr.Instance.PlayerInstance.skinNumber;
        var skinTypeList = new List<Define.SkinType>();
        Debug.Log("skinnum" + skinnum);
       if(skinnum==1)
        {
            skinTypeList.Add(Define.SkinType.one);
        }
       if(skinnum==2)
        {
            skinTypeList.Add(Define.SkinType.two);
        }
       if(skinnum==2)
        {
            skinTypeList.Add(Define.SkinType.three);
        }
        skinTypeList.Add(Define.SkinType.three);
        Define.SkinType randSkin = skinTypeList.Random(); Debug.Log("skinnum" + m_skinData);
        return m_skinData.First();
    }
   
    public void ChangeSkin(SkinData skinData)
    {
        //PlayerMgr.Instance.PlayerInstance.SetSkinSprite(OrbMgr.Instance.DefaultOrb);

        skinData.SkinSprite=GetSkinData().SkinSprite;  
    }




}
