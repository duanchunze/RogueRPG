using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Hsenl {
    public class UIPatch : MonoBehaviour {
        public RectTransform messageBox;
        public Text messageText;
        public Button confirmButton;
        public Button cancelButton;
        public Slider progressSlider;
        public Text sliderTipsText;
    }
}