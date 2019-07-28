using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UI/UIList")]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UIList : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Action<int,RectTransform> m_ItemShow;
    public Action<int,RectTransform> m_ItemHide;
    public Func<int,int> m_ItemProvider;

    [SerializeField]
    RectTransform[] m_itemProviders;
    [SerializeField]
    [Range(1,byte.MaxValue)]
    byte m_h_Line,m_v_Line;
    public byte H_Line
    {
        get{return m_h_Line;}
        set
        {
            if(value==m_h_Line || value<1)
            {
                return;
            }
            m_h_Line=value;
            ResetUIList();
        }
    }
    public byte V_Line
    {
        get{return m_v_Line;}
        set
        {
            if(value==m_v_Line || value<1)
            {
                return;
            }
            m_v_Line=value;
            ResetUIList();
        }
    }
    Vector2 m_itemSize,m_listSize;

    public RectTransform.Axis m_Axis;

    [SerializeField]
    uint m_itemNum;
    public uint ItemNum
    {
        get { return m_itemNum;}
        set
        {
            m_itemNum = value;
            ResetUIList();
        }
    }
    float m_drag_H,m_drag_V;
    Dictionary<int,RectTransform> m_itemList = new Dictionary<int,RectTransform>();
    Dictionary<int,List<RectTransform>> m_itemCache = new Dictionary<int, List<RectTransform>>();
    float m_list_left,m_list_right;
    List<int> removeToCache = new List<int>();

    protected override void Reset()
    {
        if(m_h_Line==0)
        {
            m_h_Line=1;
        }
        if(m_v_Line==0)
        {
            m_v_Line=1;
        }
        ResetUIList();
    }

    protected override void Awake()
    {
        ResetUIList();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(m_Axis==RectTransform.Axis.Horizontal)
        {
            m_drag_H-=eventData.delta.x;
            resetUIList_H();
        }
        else if(m_Axis==RectTransform.Axis.Vertical)
        {
            m_drag_V-=eventData.delta.y;
            resetUIList_V();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        
    }

    public void ResetUIList()
    {
        if(m_itemProviders==null || m_itemProviders.Length==0)
        {
            Debug.LogWarning("UIList.Item Providers不能为空!");
            return;
        }
        m_listSize = GetComponent<RectTransform>().sizeDelta;
        m_list_left = transform.localPosition.x-m_listSize.x/2;
        m_list_right = transform.localPosition.x+m_listSize.x/2;
        m_itemSize = new Vector2(m_listSize.x/H_Line,m_listSize.y/V_Line);
        if(m_Axis==RectTransform.Axis.Horizontal)
        {
            resetUIList_H();
        }
        else if(m_Axis==RectTransform.Axis.Vertical)
        {
            resetUIList_V();
        }
    }

    void resetUIList_H()
    {
        int firstIndex = Mathf.FloorToInt(Mathf.Abs(m_drag_H)/m_itemSize.x)*V_Line;
        int endIndex = Mathf.Min(Mathf.CeilToInt((Mathf.Abs(m_drag_H)+m_listSize.x)/m_itemSize.x)*V_Line,(int)ItemNum);
        for (int index = firstIndex; index < endIndex; index++)
        {
            int providerIndex = 0;
            if(m_ItemProvider!= null)
            {
                providerIndex = m_ItemProvider(index);
            }
            if(m_itemList.ContainsKey(index))
            {
                ItemName itemName = new ItemName(m_itemList[index].name);
                if(providerIndex!=itemName.ProviderIndex)
                {
                    var item = m_itemList[index];
                    m_itemList[index]=getProvider(providerIndex);
                    addToCache(providerIndex,item);
                    itemRenderer(index,m_itemList[index]);
                }
            }
            else
            {
                m_itemList[index]=(getProvider(providerIndex));
                itemRenderer(index,m_itemList[index]);
            }
            
            {
                int item_x,item_y;
                item_x = index/V_Line;
                item_y = index%V_Line;
                RectTransform item = m_itemList[index];
                ItemName itemName = new ItemName(){Index = index,ProviderIndex = providerIndex};
                item.name = itemName.ToString();

                float end_x = Mathf.CeilToInt((float)ItemNum/V_Line)*m_itemSize.x-m_listSize.x;
                if(m_drag_H>end_x)
                {
                    m_drag_H=end_x;
                }
                if(m_drag_H<0)
                {
                    m_drag_H=0;
                }
                item.localPosition = new Vector3(item_x*m_itemSize.x-m_listSize.x/2-m_drag_H,item_y*-m_itemSize.y+m_listSize.y/2,0);
                
            }
        }

        var itemList = m_itemList.GetEnumerator();
        removeToCache.Clear();
        while(itemList.MoveNext())
        {
            if(itemList.Current.Key<firstIndex || itemList.Current.Key>=endIndex)
            {
                removeToCache.Add(itemList.Current.Key);
            }
        }
        for (int i = 0; i < removeToCache.Count; i++)
        {
            RectTransform item = m_itemList[removeToCache[i]];
            if(m_ItemHide != null)
            {
                m_ItemHide(removeToCache[i],item);
            }
            m_itemList.Remove(removeToCache[i]);
            addToCache(new ItemName(item.name).ProviderIndex,item);
        }
        
    }

    void resetUIList_V()
    {
        int firstIndex = Mathf.FloorToInt(Mathf.Abs(m_drag_V)/m_itemSize.y)*H_Line;
        int endIndex = Mathf.Min(Mathf.CeilToInt((Mathf.Abs(m_drag_V)+m_listSize.y)/m_itemSize.y)*H_Line,(int)ItemNum);
        for (int index = firstIndex; index < endIndex; index++)
        {
            int providerIndex = 0;
            if(m_ItemProvider!= null)
            {
                providerIndex = m_ItemProvider(index);
            }
            if(m_itemList.ContainsKey(index))
            {
                ItemName itemName = new ItemName(m_itemList[index].name);
                if(providerIndex!=itemName.ProviderIndex)
                {
                    var item = m_itemList[index];
                    m_itemList[index]=getProvider(providerIndex);
                    addToCache(providerIndex,item);
                    itemRenderer(index,m_itemList[index]);
                }
            }
            else
            {
                m_itemList[index]=(getProvider(providerIndex));
                itemRenderer(index,m_itemList[index]);
            }
            
            {
                int item_x,item_y;
                item_x = index%H_Line;
                item_y = index/H_Line;
                RectTransform item = m_itemList[index];
                ItemName itemName = new ItemName(){Index = index,ProviderIndex = providerIndex};
                item.name = itemName.ToString();

                float end_y = Mathf.CeilToInt((float)ItemNum/H_Line)*m_itemSize.y-m_listSize.y;
                if(m_drag_V<-end_y)
                {
                    m_drag_V=-end_y;
                }
                if(m_drag_V>0)
                {
                    m_drag_V=0;
                }
                item.localPosition = new Vector3(item_x*m_itemSize.x-m_listSize.x/2,item_y*-m_itemSize.y-m_drag_V+m_listSize.y/2,0);
                
            }
        }

        var itemList = m_itemList.GetEnumerator();
        removeToCache.Clear();
        while(itemList.MoveNext())
        {
            if(itemList.Current.Key<firstIndex || itemList.Current.Key>=endIndex)
            {
                removeToCache.Add(itemList.Current.Key);
            }
        }
        for (int i = 0; i < removeToCache.Count; i++)
        {
            RectTransform item = m_itemList[removeToCache[i]];
            if(m_ItemHide != null)
            {
                m_ItemHide(removeToCache[i],item);
            }
            m_itemList.Remove(removeToCache[i]);
            addToCache(new ItemName(item.name).ProviderIndex,item);
        }
    }

    void itemRenderer(int _index,RectTransform _item)
    {
        if(m_ItemShow!=null)
        {
            m_ItemShow(_index,_item);
        }
    }

    void addToCache(int _providerIndex,RectTransform _item)
    {
        _item.gameObject.SetActive(false);
        if(m_itemCache.ContainsKey(_providerIndex))
        {
            m_itemCache[_providerIndex].Add(_item);
        }
        else
        {
            m_itemCache.Add(_providerIndex,new List<RectTransform>(){_item});
        }
    }

    RectTransform getProvider(int _providerIndex)
    {
        RectTransform item;
        if(m_itemCache.ContainsKey(_providerIndex) && m_itemCache[_providerIndex].Count>0)
        {
            item = m_itemCache[_providerIndex][0];
            m_itemCache[_providerIndex].Remove(item);
        }
        else
        {
            item = Instantiate(m_itemProviders[_providerIndex],transform);
        }
        item.sizeDelta = m_itemSize;
        item.pivot = Vector2.up;
        item.anchorMin = Vector2.up;
        item.anchorMax = Vector2.up;
        item.gameObject.SetActive(true);
        return item;
    }

    public void Clear()
    {
        for (int i = 0; i < m_itemList.Count; i++)
        {
            Destroy(m_itemList[i]);
        }
        m_itemList.Clear();
    }

    struct ItemName
    {
        public int Index;
        public int ProviderIndex;

        public ItemName(string _name)
        {
            Index = int.Parse(_name.Split('_')[0]);
            ProviderIndex = int.Parse(_name.Split('_')[1]);
        }

        public override string ToString()
        {
            return $"{Index}_{ProviderIndex}";
        }
    }
}
