using System.Collections;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public GameObject BoxObject;
    [SerializeField]public GameObject ItemBoxPrefab;

    bool isSetBoj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isSetBoj = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (BoxObject == null)
        {

            if (!isSetBoj)
            {
                StartCoroutine(SetItemBox());
                isSetBoj = true;
            }
        }
        
    }

    public IEnumerator SetItemBox()
    {
        yield return new WaitForSeconds(3);

        GameObject obj = Instantiate(ItemBoxPrefab,
            transform.position,
            Quaternion.identity,
            transform.parent);

        BoxObject = obj;
        isSetBoj = false;   
    }


}
