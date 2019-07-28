using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using MSUtil;

public interface IRecycleSlotData
{
}

public abstract class RecycleSlotBase : MonoBehaviour, IPoolObjectBase
{
    public abstract float GetWidth();
    public abstract float GetHeight();
    public abstract void Init(Transform parent, List<IRecycleSlotData> dataList);
    public abstract void Refresh(int idx);

    public void PushAction()
    {
        gameObject.SetActive(false);
    }

    public void PopAction()
    {
        gameObject.SetActive(true);
    }
}

public class RecycleScroll : ScrollRect
{
    public delegate RecycleSlotBase InstantiateItemDelegate();

    #region Inspector
    public LayoutGroup LayoutGroup;
    #endregion
    private List<IRecycleSlotData> m_DataList;
    private LinkedList<RecycleSlotBase> m_SlotList = new LinkedList<RecycleSlotBase>();
    private Vector2 m_ViewportSize;
    private Vector2 m_PaddingSize;
    private Vector2 m_ItemSize;

    private float m_UsedItemSize, m_ItemSpace;
    private float m_UsedViewportSize;
    private int m_MaxVisibleCount;
    private int m_StartIdx, m_LastIdx;
    private int m_TotalCount;
    private int m_ConstraintCount;
    private float m_Threshold = 0.1f;

    private bool m_IsReverse;
    private bool m_IsFirstUpdate;
    private InstantiateItemDelegate m_InstantDelegate;

    protected override void Awake()
    {
        onValueChanged.AddListener(OnValueChange);
    }

    public void Init(List<IRecycleSlotData> dataList, InstantiateItemDelegate instantDelegate)
    {
        m_IsFirstUpdate = true;
        m_DataList = dataList;
        var corners = new Vector3[4];
        viewport.GetWorldCorners(corners);
        m_ViewportSize = Func.GetSizeByCorner(corners);
        m_UsedViewportSize = vertical ? m_ViewportSize.y : m_ViewportSize.x;

        m_InstantDelegate = instantDelegate;
        var firstSlot = m_InstantDelegate();
        firstSlot.Init(content, m_DataList);
        m_ItemSize = new Vector2(firstSlot.GetWidth(), firstSlot.GetHeight());
        m_PaddingSize = new Vector2(LayoutGroup.padding.horizontal, LayoutGroup.padding.vertical);
        m_UsedItemSize = vertical ? m_ItemSize.y : m_ItemSize.x;

        if (vertical)
            m_IsReverse = content.pivot.y == 0 ? true : false;
        else
            m_IsReverse = content.pivot.x == 1 ? true : false;

        if (LayoutGroup is HorizontalOrVerticalLayoutGroup)
        {
            var group = LayoutGroup as HorizontalOrVerticalLayoutGroup;
            m_ConstraintCount = 1;
            m_MaxVisibleCount = Mathf.CeilToInt(m_UsedViewportSize / (m_UsedItemSize + group.spacing)) + 2;
            m_ItemSpace = group.spacing;
        }
        else
        {
            var gridGroup = LayoutGroup as GridLayoutGroup;
            m_ItemSize = gridGroup.cellSize;
            m_ConstraintCount = gridGroup.constraintCount;
            if (vertical)
            {
                var wCount = Mathf.CeilToInt(m_ViewportSize.x / (m_ItemSize.x + gridGroup.spacing.x));
                var hCount = Mathf.CeilToInt(m_ViewportSize.y / (m_ItemSize.y + gridGroup.spacing.y)) + 1;
                m_MaxVisibleCount = wCount * hCount;
                m_ItemSpace = gridGroup.spacing.y;
                m_UsedItemSize = m_ItemSize.y;
            }
            else
            {
                var wCount = Mathf.CeilToInt(m_ViewportSize.x / (m_ItemSize.x + gridGroup.spacing.x)) + 1;
                var hCount = Mathf.CeilToInt(m_ViewportSize.y / (m_ItemSize.y + gridGroup.spacing.y));
                m_MaxVisibleCount = wCount * hCount;
                m_ItemSpace = gridGroup.spacing.x;
                m_UsedItemSize = m_ItemSize.x;
            }
        }

        m_TotalCount = m_DataList.Count;
        int count = Math.Min(m_MaxVisibleCount, m_TotalCount);
        if (count > 0)
        {
            m_SlotList.AddLast(firstSlot);
            firstSlot.Refresh(m_StartIdx);
            for (int i = 1; i < count; ++i)
            {
                var item = m_InstantDelegate();
                item.Init(content, m_DataList);
                item.Refresh(m_StartIdx + i);
                m_SlotList.AddLast(item);
                if(m_IsReverse)
                    item.transform.SetAsFirstSibling();
            }
        }
        m_LastIdx = count - 1;
    }

    private void OnValueChange(Vector2 position)
    {
        ScrollUpdate(position);
    }

    public void ScrollUpdate(Vector2 scrollPos)
    {
        if (m_DataList == null || m_DataList.Count == 0 || m_DataList.Count <= m_MaxVisibleCount || m_IsFirstUpdate)
        {
            m_IsFirstUpdate = false;
            return;
        }
        var offset = m_UsedItemSize + m_ItemSpace;
        m_Threshold = Mathf.Max(m_Threshold, offset);
        var viewBound = new Bounds(viewport.rect.center, viewport.rect.size);

        if (vertical)
        {
            var offsetPos = new Vector2(0, offset);
            if (viewBound.min.y <= m_ContentBounds.min.y + LayoutGroup.padding.bottom && m_IsReverse)
            {
                if (m_StartIdx <= 0)
                    return;
                content.anchoredPosition -= offsetPos;
                m_ContentStartPosition -= offsetPos;

                SetSlot(false);
                m_Threshold = Mathf.Max(m_Threshold, offset);
            }
            else if (viewBound.min.y > m_ContentBounds.min.y + m_Threshold + LayoutGroup.padding.bottom && m_IsReverse)
            {
                if (m_LastIdx >= m_TotalCount - 1)
                    return;
                content.anchoredPosition += offsetPos;
                m_ContentStartPosition += offsetPos;

                SetSlot(true);
            }

            if (viewBound.max.y >= m_ContentBounds.max.y - LayoutGroup.padding.top && !m_IsReverse)
            {
                if (m_StartIdx <= 0)
                    return;

                content.anchoredPosition += offsetPos;
                m_ContentStartPosition += offsetPos;

                SetSlot(false);
                m_Threshold = Mathf.Max(m_Threshold, offset);
            }
            else if (viewBound.max.y < m_ContentBounds.max.y - m_Threshold - LayoutGroup.padding.top && !m_IsReverse)
            {
                if (m_LastIdx >= m_TotalCount - 1)
                    return;

                content.anchoredPosition -= offsetPos;
                m_ContentStartPosition -= offsetPos;

                SetSlot(true);
            }
        }
        else
        {
            var offsetPos = new Vector2(offset, 0);
            if (viewBound.min.x <= m_ContentBounds.min.x + LayoutGroup.padding.left && !m_IsReverse)
            {
                if (m_StartIdx <= 0)
                        return;

                content.anchoredPosition -= offsetPos;
                m_ContentStartPosition -= offsetPos;

                SetSlot(false);
                m_Threshold = Mathf.Max(m_Threshold, offset);
            }
            else if (viewBound.min.x > m_ContentBounds.min.x + m_Threshold + LayoutGroup.padding.left && !m_IsReverse)
            {
                if (m_LastIdx >= m_TotalCount - 1)
                    return;

                content.anchoredPosition += offsetPos;
                m_ContentStartPosition += offsetPos;

                SetSlot(true);
            }

            if (viewBound.max.x >= m_ContentBounds.max.x - LayoutGroup.padding.right && m_IsReverse)
            {
                if (m_StartIdx <= 0)
                    return;

                content.anchoredPosition += offsetPos;
                m_ContentStartPosition += offsetPos;

                SetSlot(false);
                m_Threshold = Mathf.Max(m_Threshold, offset);
            }
            else if (viewBound.max.x < m_ContentBounds.max.x - m_Threshold - LayoutGroup.padding.right && m_IsReverse)
            {
                if (m_LastIdx >= m_TotalCount - 1)
                    return;

                content.anchoredPosition -= offsetPos;
                m_ContentStartPosition -= offsetPos;

                SetSlot(true);
            }
        }
    }

    private void SetSlot(bool setLast)
    {
        if (setLast)
        {
            for (int i = 0; i < m_ConstraintCount; ++i)
            {
                var removeSlot = m_SlotList.First.Value;
                m_SlotList.RemoveFirst();
                ObjectFactory.Instance.DeactivateObject(removeSlot, true);
                ++m_StartIdx;

                if (m_LastIdx >= m_TotalCount - 1)
                    continue;

                ++m_LastIdx;
                var addSlot = m_InstantDelegate();
                addSlot.Init(content, m_DataList);
                addSlot.Refresh(m_LastIdx);
                m_SlotList.AddLast(addSlot);

                if (m_IsReverse)
                    addSlot.transform.SetAsFirstSibling();
                else
                    addSlot.transform.SetAsLastSibling();
            }
        }
        else
        {
            var rest = m_SlotList.Count % m_ConstraintCount;
            var removeCount = rest > 0 ? rest : m_ConstraintCount;

            for (int i = 0; i < m_ConstraintCount; ++i)
            {
                if (removeCount > i)
                {
                    var removeSlot = m_SlotList.Last.Value;
                    m_SlotList.RemoveLast();
                    ObjectFactory.Instance.DeactivateObject(removeSlot, true);
                    --m_LastIdx;
                }

                if (m_StartIdx <= 0)
                    continue;

                --m_StartIdx;
                var addSlot = m_InstantDelegate();
                addSlot.Init(content, m_DataList);
                addSlot.Refresh(m_StartIdx);
                m_SlotList.AddFirst(addSlot);

                if (m_IsReverse)
                    addSlot.transform.SetAsLastSibling();
                else
                    addSlot.transform.SetAsFirstSibling();
            }
        }
    }

    private float GetContentSize()
    {
        float result = 0;

        var padding = vertical ? m_PaddingSize.y : m_PaddingSize.x;

        if (LayoutGroup is GridLayoutGroup)
        {
            var grid = LayoutGroup as GridLayoutGroup;
            var gridSpace = vertical ? grid.spacing.y : grid.spacing.x;

            var count = Mathf.CeilToInt(m_UsedViewportSize / (m_UsedItemSize + gridSpace));
            result = padding + m_UsedItemSize * count + (m_ItemSpace * (count - 1));
        }
        else
            result = padding + m_UsedItemSize * m_MaxVisibleCount + (m_ItemSpace * (m_MaxVisibleCount - 1));
        return result;
    }

    public void Release()
    {
        if (m_SlotList != null && m_SlotList.Count > 0)
        {
            var iter = m_SlotList.GetEnumerator();
            while (iter.MoveNext())
                ObjectFactory.Instance.DeactivateObject(iter.Current, true);
        }
        m_DataList.Clear();
    }
}
