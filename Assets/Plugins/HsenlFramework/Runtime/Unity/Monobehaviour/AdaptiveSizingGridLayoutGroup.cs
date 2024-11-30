using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hsenl {
    [UnityEngine.RequireComponent(typeof(RectTransform))]
    [UnityEngine.RequireComponent(typeof(GridLayoutGroup))]
    public class AdaptiveSizingGridLayoutGroup : MonoBehaviour {
        private GridLayoutGroup gridLayoutGroup;
        private RectTransform rectTransform;

        private void Awake() {
            this.rectTransform = this.GetComponent<RectTransform>();
            this.gridLayoutGroup = this.GetComponent<GridLayoutGroup>();
        }

        private void Update() {
            var contentSize = this.CalculateContentSize();

            switch (this.gridLayoutGroup.startAxis) {
                case GridLayoutGroup.Axis.Horizontal:
                    this.rectTransform.sizeDelta = new UnityEngine.Vector2(this.rectTransform.sizeDelta.x, contentSize.y);
                    break;
                case GridLayoutGroup.Axis.Vertical:
                    this.rectTransform.sizeDelta = new UnityEngine.Vector2(contentSize.x, this.rectTransform.sizeDelta.y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private UnityEngine.Vector2 CalculateContentSize() {
            // 获取GridLayoutGroup的子元素
            var layoutGroup = this.gridLayoutGroup;
            var childCount = layoutGroup.transform.childCount;

            // 如果没有子元素，返回零向量
            if (childCount == 0) {
                return UnityEngine.Vector2.zero;
            }

            // 计算GridLayoutGroup的总尺寸
            var totalWidth = 0f;
            var totalHeight = 0f;

            // 遍历子元素，计算总尺寸
            for (var i = 0; i < childCount; i++) {
                var childRect = layoutGroup.transform.GetChild(i).transform as RectTransform;
                if (childRect != null) {
                    // 增加子元素的尺寸
                    totalWidth += childRect.sizeDelta.x;
                    if (i % layoutGroup.constraintCount == layoutGroup.constraintCount - 1 || i == childCount - 1) {
                        // 如果是最后一列，增加到总高度
                        totalHeight += childRect.sizeDelta.y;
                    }
                }
            }

            // 根据GridLayoutGroup的间距和约束调整总尺寸
            totalWidth += layoutGroup.spacing.x * (layoutGroup.constraintCount - 1);
            totalHeight += layoutGroup.spacing.y * (childCount / layoutGroup.constraintCount - 1);

            return new UnityEngine.Vector2(totalWidth, totalHeight);
        }
    }
}