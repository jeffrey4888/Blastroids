using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutRebuilderScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject layoutGroupHorizontal;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        layoutGroupHorizontal.GetComponent<GridLayoutGroup>().CalculateLayoutInputHorizontal();
        layoutGroupHorizontal.GetComponent<GridLayoutGroup>().CalculateLayoutInputVertical();
        layoutGroupHorizontal.GetComponent<GridLayoutGroup>().SetLayoutHorizontal();
        layoutGroupHorizontal.GetComponent<GridLayoutGroup>().SetLayoutVertical();
    }
}
