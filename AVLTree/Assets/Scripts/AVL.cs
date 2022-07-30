using System.Collections.Generic;
using UnityEngine;

namespace AVL 
{

    public class MyAVL
    {
        //key,ç∂,âE,çÇÇ≥,ÉâÉxÉã
        List<(Vector2Int, int, int, int, int)> key = new List<(Vector2Int, int, int, int, int)>();
        List<int> val = new List<int>();
        int rootid = 0;

        public void Clear() 
        {
            key.Clear();
            val.Clear();
            rootid = 0;
        }

        public int Count() 
        {
            return key.Count;
        }

        //a<bÇ»ÇÁ-1,a==bÇ»ÇÁ0,a>bÇ»ÇÁ1
        int LER(Vector2Int a, Vector2Int b) 
        {
            if (a.y < b.y) return -1;
            if (a.y > b.y) return 1;
            if (a.x < b.x) return -1;
            if (a.x > b.x) return 1;
            return 0;
        }


        //Ç»ÇµÇÕ-1
        public int KeyToIndex(Vector2Int key_)
        {
            if (key.Count == 0) return -1;
            int id = rootid;
            Vector2Int v2i;
            int li, ri;
            for (int k = 0; k < 32; k++)
            {
                (v2i, li, ri, _, _) = key[id];
                int ler = LER(v2i, key_);
                if (ler == 0) return id;
                id = (ler == -1) ? ri : li;
                if (id == -1) break;
            }
            return -1;
        }

        public bool FindKey(Vector2Int key_) 
        {
            if (KeyToIndex(key_) == -1)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }

        //Ç»ÇµÇÕ-1
        public int KeyToVal(Vector2Int key_)
        {
            int id = KeyToIndex(key_);
            if (id == -1)
            {
                return -1;
            }
            else
            {
                return val[id];
            }
        }



        //nums[key]=valÇÃêVãKìoò^Ç∆ìØÇ∂égÇ¢ï˚
        //êVãKìoò^ÇÃÇ›ÅBÇ∑Ç≈Ç…ìoò^Ç≥ÇÍÇ¢ÇÈèÍçáÇ»ÇÒÇ©ÇÃÉGÉâÅ[Ç…Ç»ÇÈ(èàóùçÇë¨âªÇÃÇΩÇﬂè»Ç¢ÇƒÇ¢ÇÈ)
        public void Insert(Vector2Int key_, int val_)
        {
            int cnt = val.Count;
            val.Add(val_);
            key.Add((key_, -1, -1, 1, 0));

            if (key.Count == 1) 
            {
                rootid = 0;
                return;
            }
            
            int id = rootid;
            Vector2Int v2i;
            int li, ri, h, lb;
            int[] idlog = new int[32];
            int k;

            for (k = 0; k < 32; k++)
            {
                idlog[k] = id;
                (v2i, li, ri, h, lb) = key[id];
                int ler = LER(v2i, key_);
                if (ler == 0) 
                {
                    //àÍâûîOÇÃÇΩÇﬂë„ì¸
                    val[id] = val_;
                    val.RemoveAt(cnt);
                    key.RemoveAt(cnt);
                    return;
                }


                if (ler == -1) 
                {
                    if (ri == -1) 
                    {
                        key[id] = (v2i, li, cnt, h, lb);
                        break;
                    }
                    id = ri;
                }
                if (ler == 1)
                {
                    if (li == -1) 
                    {
                        key[id] = (v2i, cnt, ri, h, lb);
                        break;
                    }
                    id = li;
                }
            }




            for (int i = k; i >= 0; i--)
            {
                id = idlog[i];
                //Ç±ÇÃidÇÃçÇÇ≥ÇämîFÇµÇΩÇ¢
                (v2i, li, ri, h, lb) = key[id];
                int h1 = 0, h2 = 0, lb1 = 0, lb2 = 0, ll = -1, lr = -1, rl = -1, rr = -1, u = -1, w = -1;
                Vector2Int v2ili = new Vector2Int(0, 0), v2iri = new Vector2Int(0, 0);
                Vector2Int p1;
                int p2, p3, p4, p5;
                if (li != -1)
                    (v2ili, ll, lr, h1, lb1) = key[li];
                if (ri != -1)
                    (v2iri, rl, rr, h2, lb2) = key[ri];


                h = Mathf.Max(h1, h2) + 1;
                lb = h2 - h1;

                int radint = 0;//-2Å`+2 ëoç∂âÒì]ÅAíPç∂âÒì]ÅAâΩÇ‡ÇµÇ»Ç¢ÅAíPâEâÒì]ÅAëoâEâÒì]
                //ç∂âÒì]Ç™ïKóv
                if (lb >= 2)
                {
                    if (lb2 > 0)
                    {
                        radint = -1;
                    }
                    else 
                    {
                        if (lb2 < 0) radint = 2;
                    }
                }
                else 
                {
                    //âEâÒì]Ç™ïKóv
                    if (lb <= -2)
                    {

                        if (lb1 < 0)
                        {
                            radint = 1;
                        }
                        else
                        {
                            if (lb1 > 0) radint = -2;
                        }
                    }
                }


                
                if (radint == 0) 
                {
                    key[id] = (v2i, li, ri, h, lb);
                    continue;
                }


                //âÒì]ëÄçÏÇÃå„ è„Ç©ÇÁÇÃï”ÇÇ¬Ç»ÇÆÇÃÇñYÇÍÇ∏Ç…
                int upperid = -1;
                if (i > 0) upperid = idlog[i - 1];

                //(radint == -1)íPç∂âÒì] (radint == 1)íPâEâÒì]
                if ((radint == -1) | (radint == 1))
                {
                    u = (radint == -1) ? ri : li;
                    int newul, newur;
                    int newvl, newvr;
                    newul = (radint == -1) ? id : ll;
                    newur = (radint == -1) ? rr : id;
                    newvl = (radint == -1) ? li : lr;
                    newvr = (radint == -1) ? rl : ri;

                    //u
                    p1 = (radint == -1) ? v2iri : v2ili;
                    p4 = (radint == -1) ? h2 : h1;
                    p5 = (radint == -1) ? lb2 : lb1;
                    key[u] = (p1, newul, newur, p4, p5);
                    idlog[i] = u; i++;

                    //v
                    key[id] = (v2i, newvl, newvr, h, lb);
                    idlog[i] = id; i++;

                    //è„Ç©ÇÁÇÃï”ÇÇ¬Ç»ÇÆ
                    if (upperid == -1)
                    {
                        rootid = u;
                    }
                    else
                    {
                        (p1, p2, p3, p4, p5) = key[upperid];
                        if (p2 == id) p2 = u;
                        if (p3 == id) p3 = u;
                        key[upperid] = (p1, p2, p3, p4, p5);
                    }
                }


                //(radint == -2)ëoç∂âÒì] (radint == 2)ëoâEâÒì]
                if ((radint == -2) | (radint == 2)) 
                {
                    u = (radint == 2) ? ri : li;
                    w = (radint == 2) ? rl : lr;
                    int newul, newur;
                    int newvl, newvr;
                    int newwl, newwr;
                    var (w2i, xl, xr, xh, xlb) = key[w];

                    newwl = (radint == 2) ? id : u;
                    newwr = (radint == 2) ? u : id;
                    newul = (radint == 2) ? xr : ll;
                    newur = (radint == 2) ? rr : xl;
                    newvl = (radint == 2) ? li : xr;
                    newvr = (radint == 2) ? xl : ri;

                    //w
                    key[w] = (w2i, newwl, newwr, xh, xlb);
                    idlog[i] = w; i++;

                    //u
                    p1 = (radint == 2) ? v2iri : v2ili;
                    p4 = (radint == 2) ? h2 : h1;
                    p5 = (radint == 2) ? lb2 : lb1;
                    key[u] = (p1, newul, newur, p4, p5);
                    idlog[i] = u; i++;

                    //v
                    key[id] = (v2i, newvl, newvr, h, lb);
                    idlog[i] = id; i++;

                    //è„Ç©ÇÁÇÃï”ÇÇ¬Ç»ÇÆ
                    if (upperid == -1)
                    {
                        rootid = w;
                    }
                    else
                    {
                        (p1, p2, p3, p4, p5) = key[upperid];
                        if (p2 == id) p2 = w;
                        if (p3 == id) p3 = w;
                        key[upperid] = (p1, p2, p3, p4, p5);
                    }
                }


            }
            return;
        }








        public void Debug2() 
        {
            Debug.Log(rootid);
            foreach (var (v2i, l, r, h, _) in key)
            {
                Debug.Log((v2i, l, r, h));
            }
            
        }




        //íÜä‘ÇÃílÇÇ›ÇÈ
        (int, int) DFS(int id, List<Vector2Int> sortdata)
        {
            int h1 = 0, h2 = 0, lb1 = 0, lb2 = 0;
            var (v2i, li, ri, h, lb) = key[id];
            if (li != -1) (h1, lb1) = DFS(li, sortdata);

            sortdata.Add(v2i);
            if (ri != -1) (h2, lb2) = DFS(ri, sortdata);


            if (h != Mathf.Max(h1, h2) + 1) 
            {
                Debug.Log("hïsàÍív");
                Debug.Log(h);
                Debug.Log(h1);
                Debug.Log(h2);
            }
            if (lb != h2 - h1)
            {
                Debug.Log("lbïsàÍív");
                Debug.Log(lb);
                Debug.Log(h1);
                Debug.Log(h2);
            }

            if (v2i.x * v2i.y != val[id]) 
            {
                Debug.Log("valïsàÍív" + id);
                Debug.Log("val=" + val[id] + "");
                Debug.Log("x*y=" + (v2i.x * v2i.y) + "");
            }
            return (h, lb);
        }

        //É\Å[ÉgÇ≥ÇÍÇƒÇÈÇ©ämîF
        public void DBG_SORT(Dictionary<Vector2Int, int> alldata) 
        {
            if (key.Count == 0) return;
            List<Vector2Int> sortdata = new List<Vector2Int>();
            DFS(rootid, sortdata);

            for (int i = 1; i < sortdata.Count; i++) 
            {
                if (LER(sortdata[i - 1], sortdata[i]) != -1)
                {
                    Debug.Log("É\Å[ÉgÉGÉâÅ[");
                    Debug.Log(sortdata[i]);
                    Debug.Log(sortdata[i - 1]);
                }
            }

            for (int i = 0; i < sortdata.Count; i++) 
            {
                alldata[sortdata[i]]--;
                if (alldata[sortdata[i]] < 0)
                {
                    Debug.Log("ÉLÅ[Ç®Ç©ÇµÇ¢");
                    Debug.Log(sortdata[i]);
                }
            }

            return;
        }

        public (int, int) GetRoot() 
        {
            var (_, _, _, h, _) = key[rootid];
            return (rootid, h);
        }


    }





}

