namespace Hsenl {
    public static class AdventureFactory {
        public static Adventure Create(int configId, Entity entity = null) {
            var config = Tables.Instance.TbAdventureConfig.GetById(configId);
            entity ??= Entity.Create(config.Alias);

            var adventure = entity.AddComponent<Adventure>();
            adventure.configId = configId;

            var entryNode = BehaviorNodeFactory.CreateNodeLink<Adventure>(config.Nodes);
            adventure.SetEntryNode(entryNode);
            return adventure;
        }
    }
}