using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    public class StatusBar : Bodied {
        private readonly MultiList<string, Status> _statuses = new();

        public Action<Status> onStatusEnter;
        public Action<Status> onStatusLeave;

        public Status GetStatus(string name) {
            if (this._statuses.TryGetValue(name, out var list)) {
                return list[0];
            }

            return null;
        }

        public void AddStatus(Status status) {
            status.SetParent(this.Entity);
        }

        public void RemoveStatus(Status status) {
            status.SetParent(null);
        }

        protected override void OnChildScopeAdd(Scope child) {
            if (child is not Status status)
                return;

            status.transform.NormalTransfrom();
            status.Reactivation();
            this._statuses.Add(status.Name, status);
        }

        protected override void OnChildScopeRemove(Scope child) {
            if (child is not Status status)
                return;

            this._statuses.Remove(status.Name);
        }

        public void OnStatusEnter(Status status) {
            try {
                this.onStatusEnter?.Invoke(status);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void OnStatusLeave(Status status, StatusFinishDetails details) {
            try {
                this.onStatusLeave?.Invoke(status);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}