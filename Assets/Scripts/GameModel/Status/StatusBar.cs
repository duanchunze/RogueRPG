using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hsenl {
    public class StatusBar : Bodied {
        private readonly Dictionary<string, Status> _statuses = new();
        private readonly List<Status> _activeStatuses = new();

        public Action<Status> onStatusEnter;
        public Action<Status> onStatusLeave;

        public Status GetStatus(string name) {
            if (this._statuses.TryGetValue(name, out var status)) {
                return status;
            }

            return null;
        }

        public IReadOnlyList<Status> GetAllActiveStatuses() {
            return this._activeStatuses;
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
            this.Changed();
        }

        protected override void OnChildScopeRemove(Scope child) {
            if (child is not Status status)
                return;

            this._statuses.Remove(status.Name);
            this.Changed();
        }

        public void OnStatusEnter(Status status) {
            if (!this._activeStatuses.Contains(status))
                this._activeStatuses.Add(status);

            try {
                this.onStatusEnter?.Invoke(status);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            this.Changed();
        }

        public void OnStatusLeave(Status status, StatusFinishDetails details) {
            this._activeStatuses.Remove(status);

            try {
                this.onStatusLeave?.Invoke(status);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            this.Changed();
        }

        public void Changed() {
            EventStation.OnStatusBarChanged(this);
        }
    }
}