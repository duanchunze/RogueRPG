﻿using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl.View {
    public class UIJumpMessage : UI<UIJumpMessage> {
        public TextMeshProUGUI text;
        public RectTransform textRect;

        [DisableInEditorMode]
        public UnityEngine.Transform followTarget;

        [DisableInEditorMode]
        public bool hasAnchor;

        [DisableInEditorMode]
        public float3 anchorPosition;

        [DisableInEditorMode]
        public float3 followOffset;

        private void Update() {
            if (this.hasAnchor) {
                if (UIManager.WorldToUIPosition(this.textRect, this.anchorPosition + this.followOffset, out var uiWorldPos)) {
                    this.transform.position = uiWorldPos;
                }
            }
            else {
                if (this.followTarget) {
                    if (UIManager.WorldToUIPosition(this.textRect, this.followTarget.position + (Vector3)this.followOffset, out var uiWorldPos)) {
                        this.transform.position = uiWorldPos;
                    }
                }
            }
        }

        protected override void OnClose() {
            base.OnClose();

            this.followTarget = null;
            this.hasAnchor = false;
            this.anchorPosition = float3.zero;
            this.followOffset = float3.zero;
        }

        public void WriteText(string message, UnityEngine.Transform target, float3 offset, float3 jumpOffset) {
            this.followTarget = target;
            this.hasAnchor = false;
            this.WriteText(message, offset, jumpOffset);
        }

        public void WriteText(string message, float3 anchor, float3 offset, float3 jumpOffset) {
            this.hasAnchor = true;
            this.anchorPosition = anchor;
            this.WriteText(message, offset, jumpOffset);
        }

        public async void WriteText(string message, float3 offset, float3 jumpOffset) {
            this.followOffset = offset;
            this.text.text = message;
            UIManager.WorldToUILocalPosition(this.textRect, Vector3.zero, out var staLocalPos);
            UIManager.WorldToUILocalPosition(this.textRect, jumpOffset, out var endLocalPos);
            this.text.transform.localPosition = Vector3.zero;
            this.text.transform.DOScale(new Vector3(0, 0, 0), 0.3f).From().SetEase(Ease.OutBack);
            // 通过这种世界位置转换，求得的移动距离，可以确保即便在不同的分辨率下，移动的距离都是相同的
            var translate = endLocalPos - staLocalPos;
            this.text.transform.DOLocalMove(translate, 0.3f);
            await Timer.WaitTime(700);
            this.Close();
        }
    }
}