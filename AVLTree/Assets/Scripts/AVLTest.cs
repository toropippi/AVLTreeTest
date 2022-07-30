using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AVL;

public class AVLTest : MonoBehaviour
{
    // Start is called before the first frame update
    MyAVL avl;
    void Start()
    {
        Debug.Log("開始");


        for (int j = 0; j < 123; j++)
        {
            avl = new MyAVL();
            int lpnm = 2 + Random.Range(0, 
                3398 + Random.Range(0, 
                13398 + Random.Range(0,
                13398 + Random.Range(0,
                13398 + Random.Range(0,
                933398)))));
            Dictionary<Vector2Int, int> alldata = new Dictionary<Vector2Int, int>();
            for (int i = 0; i < lpnm; i++)
            {
                var r1 = Random.Range(-2100000000, 2100000000);
                var r2 = Random.Range(-2100000000, 2100000000);

                alldata[new Vector2Int(r1, r2)] = 1;
                avl.Insert(new Vector2Int(r1, r2), r1 * r2);
            }

            //エラーチェック
            avl.DBG_SORT(alldata);
            //Debug.Log(avl.GetRoot());
        }


        Debug.Log("終了");
    }

}
