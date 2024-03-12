using Hsenl.ai;
using Hsenl.numeric;

namespace Hsenl.actor {
    public partial class ActorConfig {
        public NumericActorConfig NumericActorConfig => Tables.Instance.TbNumericActorConfig.GetByAlias(this.NumericAlias);
        public AIConfig AIConfig => Tables.Instance.TbAIConfig.GetByAlias(this.AiAlias);
    }
}