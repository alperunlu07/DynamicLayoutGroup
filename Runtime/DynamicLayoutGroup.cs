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
        public Vector2 m_CellSize;
        public Vector2 m_Spacing;

        public int preferredColumns, minColumns;

        public List<RectTransform> rectChildren;
        public float width, height;
        public float maxWidth;
        public RectTransform rectT;

        private List<RectTransform> childs; 
        public List<LayoutRow> layoutRows = new List<LayoutRow>();
        public LayoutRow row = new LayoutRow();
        private void Update()
        {
            if (rectT == null)
                rectT = GetComponent<RectTransform>();
            CalculateLayout();
            SetPositions();
        }

        public void CalculateLayout()
        {
            GetChildren();
            width = rectT.sizeDelta.x - m_Padding.left - m_Padding.right;
            height = rectT.sizeDelta.y - m_Padding.top - m_Padding.bottom;
        }
        public void SetPositions()
        { 
            SetGroupItems();
            AllignRows();
        }
        void SetGroupItems()
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
                        row_.items[j].anchoredPosition = new Vector2(row_.items[j - 1].anchoredPosition.x + row_.items[j - 1].sizeDelta.x + m_Spacing.x, (prevHeight + m_Padding.top) * -1f);
                    else
                        row_.items[j].anchoredPosition = new Vector2(m_Padding.left, (prevHeight + m_Padding.top) * -1f);
                }
                prevHeight += row_.height + m_Padding.top;
            }
        } 
        void GetChildren()
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