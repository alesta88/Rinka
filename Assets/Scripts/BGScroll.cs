using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroll : MonoBehaviour
{
    private float startpos, height;
    private GameObject cam;

    public Sprite[] BGSprites;

    private void Start()
    {
        cam = CameraMgr.Instance.m_cinemachineCam.gameObject;
        startpos = transform.position.y;
        // startpos_x = transform.position.x;
        var i = GameModel.Stage.Value.StageNumber;
        Debug.Log("AAA" + GameModel.Stage.Value.StageNumber+" "+ BGSprites.Length);

        if(i<=BGSprites.Length)
        {
            GetComponent<SpriteRenderer>().sprite = BGSprites[i-1];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = BGSprites[BGSprites.Length-1];
        }
        
        height = GetComponent<SpriteRenderer>().bounds.size.y;
      //  length = GetComponent<SpriteRenderer>().bounds.size.x;

    }

    private void FixedUpdate()
    {
        float dist = cam.transform.position.y;  
       // float dist_x = cam.transform.position.x-17f;

        transform.position = new Vector3(transform.position.x, startpos + dist,transform.position.z);

        //if (dist > startpos + height) startpos += height;
        //else if (dist < startpos - height) startpos -= height;

      //  if (dist_x > startpos_x + length) startpos_x += length;
        //else if (dist_x < startpos_x - length) startpos_x -= length;
    }
}
