using System;
using System.Collections.Generic;

namespace Hsenl.Network {
    public interface IPluggable {
        public Tg[] GetPlugsOfGroup<Tg>() where Tg : IPlugGroup;
        public bool GetPlugsOfGroup<Tg>(List<Tg> plugs) where Tg : IPlugGroup;
        public T GetPlugOfType<T>() where T : IPlug;
        public T GetPlug<Tg, T>() where Tg : IPlugGroup where T : IPlug, IPlugGroup;
        public void AddPlug<Tg, T>(T plug) where Tg : IPlugGroup where T : IPlug, IPlugGroup;
        public bool RemovePlug<Tg, T>() where Tg : IPlugGroup where T : IPlug, IPlugGroup;
        public bool RemovePlug<Tg>() where Tg : IPlugGroup;
    }
}