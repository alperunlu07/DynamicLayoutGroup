using System; 
using System.Collections.Generic;
using UnityEngine;

namespace Alperunlu.Utils
{
    [ExecuteAlways]
    [AddComponentMenu("Layout/Dynamic Layout Group", 153)]
    public class DynamicLayoutGroup : MonoBehaviour
    {
        [SerializeField]
        protected RectOffset m_Padding = new RectOffset(); 
        public Vector2 m_Spacing; 

        private List<RectTransform> rectChildren;
        public float width, height; 
        private RectTransform rectT;
        
        private List<LayoutRow> layoutRows = new List<LayoutRow>();
        private LayoutRow row = new LayoutRow();
        public bool setScrollViewContentHeight;
        public float scrollViewOffsetHeight = 100f;
        void Start()
        {
            UpdatePos();
        }

#if UNITY_EDITOR
        private void Update()
        {
            UpdatePos();
        }
#endif 
        public void UpdatePos()
        { 
            if (rectT == null)
                rectT = GetComponent<RectTransform>();
            CalculateLayout();
            SetPositions();
            if(setScrollViewContentHeight)
            {
                var sizeDelta = rectT.sizeDelta;
                rectT.sizeDelta = new Vector2(sizeDelta.x, height + scrollViewOffsetHeight);
            }
                
        }
        private void CalculateLayout()
        {
            GetChildren();
            //width = rectT.sizeDelta.x - m_Padding.left - m_Padding.right;
            width = rectT.rect.width - m_Padding.left - m_Padding.right;
            //height = rectT.sizeDelta.y - m_Padding.top - m_Padding.bottom;
            //height = rectT.rect.height - m_Padding.top - m_Padding.bottom;
        }
        private void SetPositions()
        { 
            SetGroupItems();
            AllignRows();
        }
        private void SetGroupItems()
        {
            layoutRows = new List<LayoutRow>();
            row = new LayoutRow();
            row.items = new List<RectTransform>();



            float width_ = 0f;
            float height_ = 0f;
             
            int i = 0;
            while (i < rectChildren.Count)
            {
                float currentWidth = rectChildren[i].sizeDelta.x;
                if (row.items.Count == 0)
                {
                    row.items.Add(rectChildren[i]);
                    width_ = currentWidth + m_Spacing.x;
                    height_ = rectChildren[i].sizeDelta.y;
                    i++;
                }
                else if ((width_ + currentWidth) < width)
                {
                    row.items.Add(rectChildren[i]);
                    width_ += currentWidth + m_Spacing.x;
                    float currentHeight = rectChildren[i].sizeDelta.y;
                    if (currentHeight > height_)
                    {
                        height_ = currentHeight;
                    }
                    i++;
                }
                else
                {
                    row.width = width_;
                    row.height = height_ + m_Spacing.y;


                    layoutRows.Add(row);
                    row = new LayoutRow();
                    row.items = new List<RectTransform>();
                    width_ = 0;
                    height_ = 0;
                }
            }
            row.width = width_;
            row.height = height_ + m_Spacing.y;
            layoutRows.Add(row); 
        }
        private void AllignRows()
        {
            float prevHeight = 0;
            for (int i = 0; i < layoutRows.Count; i++)
            {
                var row_ = layoutRows[i];
                for (int j = 0; j < row_.items.Count; j++)
                {
                    if (j > 0)
                        row_.items[j].anchoredPosition = new Vector2(row_.items[j - 1].anchoredPosition.x + row_.items[j - 1].sizeDelta.x + m_Spacing.x, (prevHeight) * -1f);
                    else
                    {
                        if(i == 0)
                            row_.items[j].anchoredPosition = new Vector2(m_Padding.left, (prevHeight + m_Padding.top) * -1f);
                        else
                            row_.items[j].anchoredPosition = new Vector2(m_Padding.left, (prevHeight) * -1f); 
                    } 
                }
                prevHeight += row_.height + m_Padding.top;
            }
            height = prevHeight;
        }
        private void GetChildren()
        {
            rectChildren = new List<RectTransform>();
            foreach (RectTransform child in rectT)
            {
                rectChildren.Add(child);
                child.anchorMax = Vector2.up;
                child.anchorMin = Vector2.up;
                child.pivot = Vector2.up;
                //Destroy(child.gameObject);
            }

        }
    }
     

    [Serializable]
    public class LayoutRow
    {
        public int index;
        public float width, height;
        public List<RectTransform> items;
    }
}