using System;
using System.Collections.Generic;

namespace Hsenl.Network {
    public interface IPluggableRealized {
        protected internal Dictionary<Type, Dictionary<Type, IPlug>> Plugs { get; }

        public Tg[] GetPlugsOfGroup<Tg>() where Tg : IPlugGroup {
            var groupType = typeof(Tg);
            Tg[] array;
            if (this.Plugs.TryGetValue(groupType, out var dict)) {
                array = new Tg[dict.Values.Count];
                int index = 0;
                foreach (var value in dict.Values) {
                    array[index++] = (Tg)value;
                }
            }

            array = Array.Empty<Tg>();
            return array;
        }

        public bool GetPlugsOfGroup<Tg>(List<Tg> plugs) where Tg : IPlugGroup {
            if (plugs == null)
                throw new ArgumentNullException(nameof(plugs));

            var groupType = typeof(Tg);
            if (this.Plugs.TryGetValue(groupType, out var dict)) {
                foreach (var kv in dict) {
                    plugs.Add((Tg)kv.Value);
                }

                return true;
            }

            return false;
        }

        public T GetPlug<Tg, T>() where Tg : IPlugGroup where T : IPlug, IPlugGroup {
            var groupType = typeof(Tg);
            var plugType = typeof(T);
            if (this.Plugs.TryGetValue(groupType, out var dict)) {
                if (dict.TryGetValue(plugType, out var plug))
                    return (T)plug;
            }

            return default;
        }

        public void Foreach<Tg>(Action<Tg> action) where Tg : IPlugGroup {
            var groupType = typeof(Tg);
            if (this.Plugs.TryGetValue(groupType, out var dict)) {
                foreach (var kv in dict) {
                    action.Invoke((Tg)kv.Value);
                }
            }
        }

        public void AddPlug<Tg, T>(T plug) where Tg : IPlugGroup where T : IPlug, IPlugGroup {
            var groupType = typeof(Tg);
            if (!this.Plugs.TryGetValue(groupType, out var dict)) {
                dict = new Dictionary<Type, IPlug>();
                this.Plugs[groupType] = dict;
            }

            dict[plug.GetType()] = plug;
        }

        public bool RemovePlug<Tg, T>() where Tg : IPlugGroup where T : IPlug, IPlugGroup {
            var groupType = typeof(Tg);
            var plugType = typeof(T);
            if (this.Plugs.TryGetValue(groupType, out var dict)) {
                var ret = dict.Remove(plugType);
                if (dict.Count == 0)
                    this.Plugs.Remove(groupType);
                return ret;
            }

            return false;
        }

        public bool RemovePlug<Tg>() where Tg : IPlugGroup {
            var ret = this.Plugs.Remove(typeof(Tg));
            return ret;
        }

        public void Dispose() {
            this.Plugs.Clear();
        }
    }
}