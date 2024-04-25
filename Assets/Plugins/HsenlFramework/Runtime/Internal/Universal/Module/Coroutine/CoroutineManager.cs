using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [Serializable]
    public sealed class CoroutineManager : Singleton<CoroutineManager> {
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, HideLabel]
#endif
        private readonly Manager _manager = new();

        public void Update() {
            this._manager.Update();
        }

        protected override void OnUnregister() {
            this._manager.Destroy();
        }

        public int Count => this._manager.RoutineCount;
        public int Start(IEnumerator enumerator) => this._manager.Start(enumerator);
        public int GlobalSingleStart(IEnumerator enumerator) => this._manager.GlobalSingleStart(enumerator);
        public bool Stop(int key) => this._manager.Stop(key);
        public void StopAll() => this._manager.StopAll();

        private enum Status {
            NormalHang, // 正常挂起
            NormalStop, // 正常结束
            Accident, // 意外
        }

        private class Routine {
            public int Key { get; private set; }

#if UNITY_EDITOR
            [ShowInInspector, ReadOnly]
#endif
            private IEnumerator Core { get; set; }

            private Routine Custodian { get; set; }
            private Manager Manager { get; set; }

            private IWait _wait;
            private Routine _custodyTarget;
            private WhenBreak _whenBreak;
            private HTask? _task;

            private bool IsDestoryed => this.Core == null;
            private bool IsManaged => this.Manager != null;

            public static Routine Create(int key, IEnumerator enumerator, Manager manager, Routine custodian) {
                var routine = ObjectPool.Rent<Routine>();
                routine.Key = key;
                routine.Core = enumerator;
                routine.Custodian = custodian;
                routine.Manager = manager;

                routine._wait = null;
                routine._custodyTarget = null;
                routine._whenBreak = null;
                routine._task = null;
                return routine;
            }

            public static void Recycle(Routine routine) {
                ObjectPool.Return(routine);
            }

            public Status MoveNext() {
                // 先判断自己是否有托管人，且托管人是否安好
                if (this.Custodian is { IsDestoryed: true }) return Status.Accident;
                // 判断自己是不是已经被销毁了
                if (this.IsDestoryed) return Status.Accident;

                // 挂起
                JUMP_WAIT:
                if (this._wait != null) {
                    if (this._wait.Tick()) {
                        this._wait = null;
                        goto JUMP_MOVE_NEXT;
                    }

                    return Status.NormalHang;
                }

                JUMP_CUSTODY_TARGET:
                if (this._custodyTarget != null) {
                    if (this._custodyTarget.IsDestoryed) {
                        if (this._custodyTarget.Custodian == this) {
                            this._custodyTarget.Custodian = null;
                        }

                        this._custodyTarget = null;
                        goto JUMP_MOVE_NEXT;
                    }

                    return Status.NormalHang;
                }

                JUMP_TASK:
                if (this._task != null) {
                    if (this._task.Value.IsCompleted) {
                        this._task = null;
                        goto JUMP_MOVE_NEXT;
                    }

                    return Status.NormalHang;
                }

                // get current
                switch (this.Core.Current) {
                    case IWait wait:
                        this._wait = wait;
                        goto JUMP_WAIT;

                    case IEnumerator custodyTarget:
                        // 这里本需进行嵌套与否的判断，但因为开启协程都需要做Contains判断，所以，这里可以不做嵌套检测了
                        this._custodyTarget = this.Manager.InternalStart(custodyTarget, this);
                        if (this._custodyTarget != null)
                            goto JUMP_CUSTODY_TARGET;
                        else
                            goto JUMP_MOVE_NEXT;

                    case HTask task:
                        this._task = task;
                        goto JUMP_TASK;
                }

                JUMP_MOVE_NEXT:
                bool next;
                try {
                    next = this.Core.MoveNext(); // move next 会接着上一个yield挂起的地方开始执行，并寻找下一个 yield，找不到则返回 false
                }
                catch (Exception e) {
                    Log.Error($"<协程内容体执行错误> {this.Core} + {e}");
                    return Status.Accident;
                }

                // WhenBreak 的意义是，不挂起，只是保存一个回调，所以，连续执行 MoveNext
                // 再做一次 IsDisposed 检测是因为，有可能这次 MoveNext 的过程中，把协程给关闭了
                if (!this.IsDestoryed && next && this.Core.Current is WhenBreak when) {
                    if (this._whenBreak != null) throw new Exception("already yield return when break");
                    this._whenBreak = when;
                    goto JUMP_MOVE_NEXT;
                }

                return next ? Status.NormalHang : Status.NormalStop;
            }

            public void Stop(Status status) {
                if (this.IsDestoryed) return;
                // 此举保证了是子协程先关闭，父协程再关的。其实因为协程本身并不依赖结束回调，所以其实谁先关都无所谓，但因为加入了 WhenBreaks 后就不一样了，必须要确保是子协程先关闭
                if (this._custodyTarget != null) {
                    var target = this._custodyTarget;
                    this._custodyTarget = null;
                    target.Stop(status);
                }

                if (this._whenBreak != null) {
                    var when = this._whenBreak;
                    this._whenBreak = null;
                    if (when.ActingOnEnd || status != Status.Accident) {
                        InvokeWhen(when);
                    }
                }

                // this.Key = 0;
                this.Core = null;
                this.Custodian = null;
                this.Manager = null;
                this._wait = null;
                this._custodyTarget = null;
                this._whenBreak = null;
                this._task = null;
            }

            public void Destory() {
                this._whenBreak = null;
                this.Core = null;
                this.Custodian = null;
                this.Manager = null;
                this._wait = null;
                this._custodyTarget = null;
                this._whenBreak = null;
                this._task = null;
            }

            private static void InvokeWhen(WhenBreak whenBreak) {
                try {
                    whenBreak.Action.Invoke();
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            public override string ToString() {
                return this.Core?.ToString();
            }
        }

        /// <summary>
        /// 1、yield 后可以识别 IWait、IEnumerator 以及 IETTask
        /// 2、可以通过一个小灶里面直接 yield return new IEnumerator 的方式来开启一个子协程，实现小灶的嵌套
        /// 3、协程添加时，会立即执行一次MoveNext
        /// 4、Stop的时候，会立即执行，并触发事件，但对于协程从Manager中删除，却会延迟一帧
        /// </summary>
        private class Manager {
#if UNITY_EDITOR
            [ShowInInspector, ReadOnly, LabelText("Coroutines")]
#endif
            private readonly Dictionary<int, Routine> _kv = new();

            private Queue<Routine> _queue1 = new();
            private Queue<Routine> _queue2 = new();

            public int RoutineCount => this._kv.Count;

            public void Update() {
                while (this._queue1.Count > 0) {
                    var routine = this._queue1.Dequeue();
                    var status = Status.NormalHang;
                    try {
                        status = routine.MoveNext();
                    }
                    catch (Exception e) {
                        Log.Error(e);
                    }

                    if (status != Status.NormalHang) {
                        routine.Stop(status);
                        this.RemoveRoutine(routine);
                        continue;
                    }

                    this._queue2.Enqueue(routine);
                }

                ObjectHelper.Swap(ref this._queue1, ref this._queue2);
            }

            public int Start(IEnumerator enumerator) {
                var key = enumerator.GetHashCode();
                if (this._kv.ContainsKey(key)) throw new Exception("try repeat start coroutine by a same enumerator");
                var routine = Routine.Create(key, enumerator, this, null);
                return this.AddRoutine(key, routine) ? key : 0;
            }

            // 一般来说, 除了对于某些全局计时, 公共检测之类的情况, 你都没有理由需要使用单例开始
            public int GlobalSingleStart(IEnumerator enumerator) {
                // 单例模式下，使用enumerator的字符串作为key，因为该字符串对于一个协程函数来说，始终是唯一的
                var key = enumerator.ToString().GetHashCode();
                if (this._kv.TryGetValue(key, out var routine)) routine.Stop(Status.Accident);
                routine = Routine.Create(key, enumerator, this, null);
                return this.AddRoutine(key, routine) ? key : 0;
            }

            public Routine InternalStart(IEnumerator enumerator, Routine custodian) {
                var key = enumerator.GetHashCode();
                if (this._kv.ContainsKey(key)) throw new Exception("同一个Enumerator无法反复用来开启新协程，请等待上一个协程结束再次使用，建议通过 new 的方式来创建托管的协程");
                var routine = Routine.Create(key, enumerator, this, custodian);
                return this.AddRoutine(key, routine) ? routine : null;
            }

            public bool Stop(int key) {
                if (!this._kv.TryGetValue(key, out var routine)) {
                    return false;
                }

                routine.Stop(Status.Accident);
                return true;
            }

            public void StopAll() {
                foreach (var routine in this._kv.Values) {
                    routine.Stop(Status.Accident);
                }
            }

            public void Destroy() {
                foreach (var routine in this._kv.Values) {
                    routine.Destory();
                    Routine.Recycle(routine);
                }

                this._kv.Clear();
                this._queue1.Clear();
                this._queue2.Clear();
            }

            private bool AddRoutine(int key, Routine routine) {
                var success = true;
                try {
                    var status = routine.MoveNext();
                    if (status != Status.NormalHang) {
                        routine.Stop(status);
                        return true;
                    }

                    this._kv[key] = routine;
                    this._queue1.Enqueue(routine);
                }
                catch (Exception e) {
                    success = false;
                    Log.Error(e);
                }

                return success;
            }

            private bool RemoveRoutine(Routine routine) {
                Routine.Recycle(routine);
                if (!this._kv.TryGetValue(routine.Key, out var result))
                    throw new Exception($"Remove routine fail '{routine}'");
                if (result == routine)
                    this._kv.Remove(routine.Key);
                else
                    throw new Exception($"Remove routine fail '{routine}'");

                return true;
            }
        }
    }
}