using System;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class AdventureManager : SingletonComponent<AdventureManager> {
        public Adventure Adventure { get; private set; }

        public Adventure NewAdventure(int configId) {
            if (this.Adventure.IsValid())
                Object.Destroy(this.Adventure.Entity);

            this.Adventure = AdventureFactory.Create(configId);
            this.Adventure.Entity.DontDestroyOnLoadWithUnity();
            this.Adventure.SetRecordPath(Application.dataPath, this.Adventure.Name);
            // if (!this.Adventure.Load()) {
            //     this.Adventure.SetRecord(new RcdAdventure() {
            //         currentCheckpoint = 1,
            //         totalCheckpoint = 2,
            //     });
            //
            //     this.Adventure.Save();
            // }
            // else {
            //     var record = (RcdAdventure)this.Adventure.Record;
            // }

            return this.Adventure;
        }
    }
}