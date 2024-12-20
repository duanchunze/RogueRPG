﻿using System;
using TMPro;
using UnityEngine;

namespace Hsenl.View {
    public class UIHeadFollow : UI<UIHeadFollow> {
        public RectTransform location;
        public VolumeSlider healthSlider;
        public VolumeSlider energySlider;
        public VolumeSlider manaSlider;
        public TextMeshProUGUI tempNameText;
        public UnityEngine.Transform statusHolder;
        public UIStatusSlot statusSlotTemplate;
        public UnityEngine.Transform followTarget;
        public Vector3 followOffset;

        private RectTransform _rectTransform;

        public StatusBar StatusBar { get; private set; }

        private void Awake() {
            this._rectTransform = (RectTransform)this.transform;
        }

        private void Update() {
            if (this.followTarget) {
                if (UIManager.WorldToUIPosition(this._rectTransform, this.followTarget.position + (UnityEngine.Vector3)this.followOffset, out var uiWorldPos)) {
                    this._rectTransform.position = uiWorldPos;
                }

                if (this.tempNameText != null) {
                    if (UIManager.WorldToUIPosition(this._rectTransform, this.followTarget.position, out uiWorldPos)) {
                        this.tempNameText.transform.position = uiWorldPos;
                    }
                }
            }
        }

        public void FillInStatusBar(StatusBar statusBar) {
            this.StatusBar = statusBar;

            var statuses = statusBar.GetAllActiveStatuses();
            this.statusHolder.MakeSureChildrenCount(this.statusSlotTemplate.transform, statuses.Count);
            for (int i = 0; i < statuses.Count; i++) {
                var status = statuses[i];
                var uiSlot = this.statusHolder.GetChild(i).GetComponent<UIStatusSlot>();
                uiSlot.FillIn(status);
            }
        }
    }
}