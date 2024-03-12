using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedurePreloadAssets : AProcedureState {
        [ShadowFunction]
        protected override async void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            await HTask.Completed;
            await this.OnEnterShadow(manager, prev);
            manager.ChangeState<ProcedurePreprocessing>();
        }

        [ShadowFunction]
        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            this.OnLeaveShadow(manager, next);
        }

        private async Hsenl.HTask OnEnterShadow2(Hsenl.ProcedureManager manager, Hsenl.FsmState<Hsenl.ProcedureManager> prev) {
            if (!Hsenl.ShadowFunction.GetFunctions(3830295823, out var dels))
                return;

            HTask? first0PriorityDel = null;
            ListHTask<HTask> parallels = null;
            foreach (var kv in dels) {
                if (kv.Key < 0) {
                    if (Convert2Task(kv.Value, out var task)) {
                        try {
                            await task;
                        }
                        catch (Exception e) {
                            ShadowDebug.LogError(e);
                        }
                    }
                }
                else if (kv.Key == 0) {
                    if (first0PriorityDel == null && parallels == null) {
                        if (!Convert2Task(kv.Value, out var task1)) {
                            continue;
                        }

                        first0PriorityDel = task1;
                        continue;
                    }

                    parallels ??= ListHTask<HTask>.Create();
                    if (first0PriorityDel != null) {
                        parallels.Add(first0PriorityDel.Value);
                        first0PriorityDel = null;
                    }

                    if (Convert2Task(kv.Value, out var task2))
                        parallels.Add(task2);
                }
                else {
                    if (first0PriorityDel != null || parallels != null) {
                        try {
                            await GetParallelTasks();
                        }
                        catch (Exception e) {
                            ShadowDebug.LogError(e);
                        }
                        finally {
                            parallels?.Dispose();
                            first0PriorityDel = null;
                            parallels = null;
                        }
                    }

                    if (Convert2Task(kv.Value, out var task)) {
                        try {
                            await task;
                        }
                        catch (Exception e) {
                            ShadowDebug.LogError(e);
                        }
                    }
                }
            }

            if (first0PriorityDel != null || parallels != null) {
                try {
                    await GetParallelTasks();
                }
                catch (Exception e) {
                    ShadowDebug.LogError(e);
                }
                finally {
                    parallels?.Dispose();
                }
            }

            return;

            Hsenl.HTask GetParallelTasks() {
                if (parallels != null)
                    return Hsenl.HTask.WaitAll(parallels);

                if (first0PriorityDel == null)
                    throw new NullReferenceException(nameof(first0PriorityDel));

                return first0PriorityDel.Value;
            }

            bool Convert2Task(Delegate del, out Hsenl.HTask task) {
                if (del is global::System.Func<Hsenl.ProcedurePreloadAssets, Hsenl.ProcedureManager, Hsenl.FsmState<Hsenl.ProcedureManager>, Hsenl.HTask>
                    func) {
                    try {
                        task = func.Invoke(this, manager, prev);
                    }
                    catch (Exception e) {
                        ShadowDebug.LogError(e);
                        return false;
                    }

                    return true;
                }
                else if (del is global::System.Func<Hsenl.ProcedureManager, Hsenl.FsmState<Hsenl.ProcedureManager>, Hsenl.HTask> func1) {
                    try {
                        task = func1.Invoke(manager, prev);
                    }
                    catch (Exception e) {
                        ShadowDebug.LogError(e);
                        return false;
                    }

                    return true;
                }

                return false;
            }
        }
    }
}