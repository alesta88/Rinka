using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinColor : MonoBehaviour
{
    public Color skinColor;
    Transform[] allChildren=null;
    List<GameObject> tag_targets;
    // Start is called before the first frame update
    void Start()
    {
        allChildren = GetComponentsInChildren<Transform>();
        
        var i = AllChilds(this.gameObject).Count;
        Debug.Log("tagtag " + i);
    }

    // Update is called once per frame 
    void Update()
    {
        //ChangeColor();
        //var i = AllChilds(this.gameObject).Count;
        //Debug.Log("tagtag " + i);
    }

    void ChangeColor()
    {
        foreach (Transform child in allChildren)
        {
           
          //  Debug.Log("childs " + child); 
            if (child.GetComponent<ParticleSystem>() != null)
            {
                Debug.Log("child01");
                ParticleSystem.MainModule settings = child.GetComponent<ParticleSystem>().main;
                settings.startColor = new ParticleSystem.MinMaxGradient(skinColor);
            }
            //child.gameObject.SetActive(false);
            

            //var newstartcolor = child.GetComponent<ParticleSystem>().main.startColor;
            //child.GetComponent<ParticleSystem>().main.startColor = skinColor;
        }
    }


    private void AddDescendantsWithTag(Transform parent, string tag, List<GameObject> list)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.tag == tag)
            {
                list.Add(child.gameObject);
            }
            AddDescendantsWithTag(child, tag, list);
        }
    }



    private List<GameObject> AllChilds(GameObject root)
    {
        List<GameObject> result = new List<GameObject>();
        if (root.transform.childCount > 0)
        {
            foreach (Transform VARIABLE in root.transform)
            {
                Searcher(result, VARIABLE.gameObject);
            }
        }
        return result;
    }

    private void Searcher(List<GameObject> list, GameObject root)
    {
        list.Add(root);
        if (root.transform.childCount > 0)
        {
            foreach (Transform VARIABLE in root.transform)
            {
                Searcher(list, VARIABLE.gameObject);
            }
        }
    }
}
