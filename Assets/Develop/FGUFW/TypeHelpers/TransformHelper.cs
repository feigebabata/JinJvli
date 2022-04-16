using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Worlds.Battle;

namespace FGUFW.Core
{
    static public class TransformHelper
    {
        static public RectTransform AsRT(this Transform t)
        {
            return t as RectTransform;
        }
        static public RectTransform GetChildRT(this Transform t,int index)
        {
            return t.GetChild(index) as RectTransform;
        }

        static public IEnumerator MoveWorld(this Transform transform,Vector3 endPos,float time)
        {
            float startTime = Time.time;
            while (Time.time-startTime<time)
            {
                float t = (Time.time-startTime)/time;
                transform.position = Vector3.Lerp(transform.position,endPos,t);
                yield return null;
            }
            transform.position = endPos;
        }

        static public IEnumerator MoveLocal(this Transform transform,Vector3 endPos,float time)
        {
            float startTime = Time.time;
            while (Time.time-startTime<time)
            {
                float t = (Time.time-startTime)/time;
                transform.localPosition = Vector3.Lerp(transform.localPosition,endPos,t);
                yield return null;
            }
            transform.localPosition = endPos;
        }

        static public IEnumerator RotateLocal(this Transform transform,Vector3 endAngle,float time)
        {
            float startTime = Time.time;
            Quaternion rotation = Quaternion.Euler(endAngle);
            while (Time.time-startTime<time)
            {
                float t = (Time.time-startTime)/time;
                transform.localRotation = Quaternion.Lerp(transform.localRotation,rotation,t);
                yield return null;
            }
            transform.localRotation = rotation;
        }

        static public IEnumerator RotateWorld(this Transform transform,Vector3 endAngle,float time)
        {
            float startTime = Time.time;
            Quaternion rotation = Quaternion.Euler(endAngle);
            while (Time.time-startTime<time)
            {
                float t = (Time.time-startTime)/time;
                transform.rotation = Quaternion.Lerp(transform.rotation,rotation,t);
                yield return null;
            }
            transform.rotation = rotation;
        }

        static public void Foreach<V>(this Transform transform,IEnumerable list,Action<Transform,V> callback)
        {
            int idx = 0;
            if(list!=null)
            {
                var enumerator = list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Transform item_t = transform.GetOrCreateChild(idx);
                    callback?.Invoke(item_t,(V)enumerator.Current);
                    item_t.gameObject.SetActive(true);
                    idx++;
                }
            }
            for (int i = idx; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        static public void Foreach<T,V>(this Transform transform,IEnumerable list,Action<T,V> callback)
        {
            int idx = 0;
            if(list!=null)
            {
                var enumerator = list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Transform item_t = transform.GetOrCreateChild(idx);
                    callback?.Invoke(item_t.GetComponent<T>(),(V)enumerator.Current);
                    item_t.gameObject.SetActive(true);
                    idx++;
                }
            }
            for (int i = idx; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        static public void Foreach(this Transform transform,int count,Action<int,Transform> callback)
        {
            for (int i = 0; i < count; i++)
            {
                Transform item_t = transform.GetOrCreateChild(i);
                callback?.Invoke(i,item_t);
                item_t.gameObject.SetActive(true);
            }
            for (int i = count; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        static public void Foreach<T>(this Transform transform,int count,Action<int,T> callback)
        {
            for (int i = 0; i < count; i++)
            {
                Transform item_t = transform.GetOrCreateChild(i);
                callback(i,item_t.GetComponent<T>());
                item_t.gameObject.SetActive(true);
            }
            for (int i = count; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        static public string FullPath(this Transform t)
        {
            StringBuilder s = new StringBuilder();
            do
            {
                s.Insert(0,$"\\{t.name}");
                t = t.parent;
            } 
            while (t!=null);
            return s.ToString();
        }

        static public void Sort<T>(this Transform t,Comparison<T> comparison) where T:MonoBehaviour
        {
            List<T> childs = new List<T>(t.childCount);
            foreach (Transform item in t)
            {
                childs.Add(item.GetComponent<T>());
            }
            childs.Sort(comparison);
            for (int i = 0; i < childs.Count; i++)
            {
                childs[i].transform.SetSiblingIndex(i);
            }
        }

        static public void Sort(this Transform t,Comparison<Transform> comparison)
        {
            List<Transform> childs = new List<Transform>(t.childCount);
            //foreach (Transform item in t)
            //{
            //    childs.Add(item);
            //}
            //childs.Sort(comparison);
            //for (int i = 0; i < childs.Count; i++)
            //{
            //    childs[i].SetSiblingIndex(i);
            //}

            foreach (Transform item in t)
            {
                childs.Add(item);
                if (item.GetComponent<Canvas>() == null && item.GetComponent<BattleRenderHeight>() != null)
                {
                    item.gameObject.AddComponent<Canvas>();
                    item.gameObject.AddComponent<GraphicRaycaster>();
                    item.GetComponent<Canvas>().overrideSorting = true;
                }
            }
            childs.Sort(comparison);
            for (int i = 0; i < childs.Count; i++)
            {
                //childs[i].SetSiblingIndex(i);
                if (childs[i].GetComponent<Canvas>() != null)
                {
                    if (childs[i].GetComponent<TowerBase>() != null && childs[i].GetComponent<TowerBase>().TowerType == TowerType.Base)
                        childs[i].GetComponent<Canvas>().sortingOrder = 1;//塔基层级单独处理
                    else
                        childs[i].GetComponent<Canvas>().sortingOrder = i;
                }
            }
        }

        static public Transform RandomChild(this Transform t)
        {
            int idx = UnityEngine.Random.Range(0,t.childCount);
            return t.GetChild(idx);
        }

        static public T RandomChild<T>(this Transform t)
        {
            int idx = UnityEngine.Random.Range(0,t.childCount);
            return t.GetChild(idx).GetComponent<T>();
        }

        static public Transform GetOrCreateChild(this Transform t,int index,string createName=null)
        {
            while (index >= t.childCount)
            {
                var child = GameObject.Instantiate(t.GetChild(0).gameObject,t);
                if(createName!=null)child.name=createName;
            }
            return t.GetChild(index);
        }

        static public T GetOrCreateChild<T>(this Transform t,int index,string createName=null)
        {
            while (index >= t.childCount)
            {
                var child = GameObject.Instantiate(t.GetChild(0).gameObject,t);
                if(createName!=null)child.name=createName;
            }
            return t.GetChild(index).GetComponent<T>();
        }
    }
}