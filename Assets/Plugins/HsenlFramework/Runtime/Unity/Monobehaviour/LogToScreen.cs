using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    public class LogToScreen : MonoBehaviour {
        public int maxLines = 50;
        public int maxLineLength = 120;
        public int fontSize = 15;

        private string _logStr = "";
        private readonly List<string> _lines = new List<string>();

        private void OnEnable() {
            Application.logMessageReceived += this.Log;
        }

        private void OnDisable() {
            Application.logMessageReceived -= this.Log;
        }

        public void Log(string logString, string stackTrace, UnityEngine.LogType type) {
            foreach (var line in logString.Split('\n')) {
                if (line.Length <= maxLineLength) {
                    this._lines.Add(line);
                    continue;
                }

                var lineCount = line.Length / maxLineLength + 1;
                for (int i = 0; i < lineCount; i++) {
                    if ((i + 1) * maxLineLength <= line.Length) {
                        this._lines.Add(line.Substring(i * maxLineLength, maxLineLength));
                    }
                    else {
                        this._lines.Add(line.Substring(i * maxLineLength, line.Length - i * maxLineLength));
                    }
                }
            }

            if (this._lines.Count > maxLines) {
                this._lines.RemoveRange(0, this._lines.Count - maxLines);
            }

            this._logStr = string.Join("\n", this._lines);
        }

        private void OnGUI() {
            GUI.matrix = UnityEngine.Matrix4x4.TRS(UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, new UnityEngine.Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));
            GUI.Label(new Rect(10, 10, 800, 370), this._logStr, new GUIStyle() { fontSize = System.Math.Max(10, this.fontSize) });
        }
    }
}