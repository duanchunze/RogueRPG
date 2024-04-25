/*MIT License

Copyright (c) 2018 tanghai

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [Serializable]
    public class TaskLockManager : Singleton<TaskLockManager> {
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly List<TaskLockQueueType> _list = new();

        private readonly Queue<(int, long, int)> _nextFrameRun = new();

        public TaskLockManager() {
            for (var i = 0; i < TaskLockType.Max; ++i) {
                var queueType = new TaskLockQueueType(i);
                this._list.Add(queueType);
            }
        }

        protected override void OnUnregister() {
            this._list.Clear();
            this._nextFrameRun.Clear();
        }

        public void Update() {
            // 循环过程中会有对象继续加入队列
            while (this._nextFrameRun.Count > 0) {
                var (coroutineLockType, key, count) = this._nextFrameRun.Dequeue();
                this.Notify(coroutineLockType, key, count);
            }
        }

        internal void RunNext(int taskLockType, long key, int level) {
            // 一个协程队列一帧处理超过100个,说明比较多了,打个warning,检查一下是否够正常
            if (level == 100) {
                Log.Warning($"too much task lock level: {taskLockType} {key}");
            }

            this._nextFrameRun.Enqueue((taskLockType, key, level));
        }

        public async HTask<TaskLocker> Wait(int taskLockType, long key, int time = 60000) {
            var queueType = this._list[taskLockType];
            return await queueType.Wait(key, time, this);
        }

        private void Notify(int taskLockType, long key, int level) {
            var queueType = this._list[taskLockType];
            queueType.Notify(key, level);
        }
    }
}