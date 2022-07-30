using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AVL;
using System;

public class AVLTest2 : MonoBehaviour
{
    struct MyStruct
    {
        //-134217728 ~ 134217727

        /*
        public int s0;
        public int s1;
        public int lid;
        public int rid;
        public uint datahlb;


        public void SetData(int keyx, int keyy, int lid_, int rid_, int h, int lb)
        {
            s0 = keyx;
            s1 = keyy;
            lb += 2;
            datahlb = (uint)lb * 64 + (uint)h;
            lid = lid_;
            rid = rid_;
        }

        public (Vector2Int, int, int, int, int) GetData()
        {
            int lb;
            int h;
            lb = (int)(datahlb / 64);
            h = (int)(datahlb % 64);
            lb -= 2;
            return (new Vector2Int(s0, s1), lid, rid, h, lb);
        }
        */

        public uint s0;
        public uint s1;
        public int lid;
        public int rid;

        public void SetData(int keyx, int keyy, int lid_, int rid_, int h, int lb)
        {
            lb %= 2;//-1～1に
            lb += 2;//1～3に
            h %= 64;//6bit以内に
            uint data = (uint)lb * 64 + (uint)h;

            lid = lid_;
            rid = rid_;
            keyx += 134217728;
            keyy += 134217728;//これで28bitに入った
            s0 = (uint)keyx % 268435456;
            s1 = (uint)keyy % 268435456;

            uint data1 = data % 16;
            uint data2 = data / 16;
            s0 += 268435456 * data1;
            s1 += 268435456 * data2;
        }

        public (Vector2Int, int, int, int, int) GetData()
        {
            int lb = 0;
            int h = 0;
            uint data;

            uint x, y;
            x = s0 % 268435456;
            y = s1 % 268435456;
            int keyx = ((int)x) - 134217728;
            int keyy = ((int)y) - 134217728;

            x = s0 / 268435456;
            y = s1 / 268435456;

            data = y * 16 + x;
            lb = (int)data / 64;
            h = (int)data % 64;
            lb -= 2;

            return (new Vector2Int(keyx, keyy), lid, rid, h, lb);
        }
        
    };





    // Start is called before the first frame update
    MyAVL avl;
    ComputeBuffer KeyBuffer, ValBuffer, IntData;
    [SerializeField] ComputeShader computeShader = default;
    void Start()
    {
        KeyBuffer = new ComputeBuffer(1024 * 1024 * 4, 4 * 4);
        ValBuffer = new ComputeBuffer(1024 * 1024 * 4, 2 * 4);
        IntData = new ComputeBuffer(2, 4);
        int k = computeShader.FindKernel("AVLInsert");
        computeShader.SetBuffer(k, "KeyBuffer", KeyBuffer);
        computeShader.SetBuffer(k, "ValBuffer", ValBuffer);
        computeShader.SetBuffer(k, "IntData", IntData);
        computeShader.SetInt("rootid", 0);

        int[] keyto = new int[8];
        int lpnm = 270000;


        Debug.Log("開始");


        List<Vector2Int> rnd = new List<Vector2Int>();
        for (int i = 0; i < lpnm; i++) 
        {
            rnd.Add(new Vector2Int(UnityEngine.Random.Range(-2100000000 / 16, 2100000000 / 16), UnityEngine.Random.Range(-2100000000 / 16, 2100000000 / 16)));
        }


        var stm_c = Gettime();
        avl = new MyAVL();
        Dictionary<Vector2Int, int> alldata = new Dictionary<Vector2Int, int>();

        for (int i = 0; i < lpnm; i++)
        {
            alldata[rnd[i]] = 1;
            avl.Insert(rnd[i], rnd[i].x * rnd[i].y);
        }
        var etm_c = Gettime();
        Debug.Log("CPU" + (etm_c - stm_c) + "ms");

        avl.DBG_SORT(alldata);
        Debug.Log("" + lpnm + "" + avl.GetRoot() + "");
        //CPU計算おわり

        //GPU開始
        var stm = Gettime();
        for (int i = 0; i < lpnm; i++)
        {
            keyto[0] = rnd[i].x;
            keyto[4] = rnd[i].y;
            computeShader.SetInts("key_", keyto);
            computeShader.SetInts("val_", keyto);
            computeShader.Dispatch(k, 1, 1, 1);
        }
        

        MyStruct[] kb_host = new MyStruct[1];
        KeyBuffer.GetData(kb_host, 0, 0, 1);
        var etm = Gettime();
        //時間計測ここまで
        Debug.Log("GPU" + (etm - stm) + "ms");



        kb_host = new MyStruct[lpnm];
        KeyBuffer.GetData(kb_host, 0, 0, lpnm);

        
        for (int i = 0; i < lpnm; i++) 
        {
            var (v2i, li, ri, h, lb) = avl.key[i];
            var (v2i2, li2, ri2, h2, lb2) = kb_host[i].GetData();
            
            if ((v2i, li, ri, h, lb) != (v2i2, li2, ri2, h2, lb2)) 
            {
                Debug.Log((v2i, li, ri, h, lb));
                Debug.Log((v2i2, li2, ri2, h2, lb2));
            }
        }
        
        Debug.Log("終了");
    }



    private void OnDestroy()
    {
        KeyBuffer.Release();
        ValBuffer.Release();
        IntData.Release();
    }

    //現在の時刻をms単位で取得
    int Gettime()
    {
        return DateTime.Now.Millisecond + DateTime.Now.Second * 1000
         + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Hour * 60 * 60 * 1000;
    }


}
