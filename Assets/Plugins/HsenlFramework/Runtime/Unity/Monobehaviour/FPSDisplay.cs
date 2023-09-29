using UnityEngine;

namespace Hsenl {
    public class FPSDisplay : MonoBehaviour {
        private float totalDeltaTime = 0.0f;
        private int frameCount = 0;
        private GUIStyle style;
        private float timer = 0.0f;

        private float _fps;

        [SerializeField]
        private float updateInterval = 0.1f; // 更新间隔（秒）

        private void Start() {
            // 创建样式（style）用于显示帧数
            this.style = new GUIStyle {
                fontSize = 20,
                normal = {
                    textColor = Color.white
                }
            };
        }

        private void Update() {
            // 累加帧时间和帧数
            this.totalDeltaTime += Time.unscaledDeltaTime;
            this.frameCount++;

            // 计时器累加每一帧的时间
            this.timer += Time.unscaledDeltaTime;

            // 当计时器超过更新间隔时，更新帧数显示
            if (this.timer >= this.updateInterval) {
                this.UpdateFPSDisplay();
                this.timer -= this.updateInterval;
            }
        }

        private void UpdateFPSDisplay() {
            // 计算过去5秒内的帧数平均值
            this._fps = this.frameCount / this.totalDeltaTime;
            
            // 重置计数器和累计时间
            this.frameCount = 0;
            this.totalDeltaTime = 0.0f;
        }

        private void OnGUI() {
            // 在屏幕左上角显示帧数平均值
            GUI.Label(new Rect(10, 10, 100, 20), $"FPS: {this._fps:0.}", this.style);
        }
    }
}