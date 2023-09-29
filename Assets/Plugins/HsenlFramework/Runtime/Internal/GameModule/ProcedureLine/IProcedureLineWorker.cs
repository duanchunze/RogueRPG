using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.NoGenerate)]
    public partial interface IProcedureLineWorker {
        ProcedureLineNode ProcedureLineNode { get; }
        void OnAddToNode(ProcedureLineNode node);
        void OnRemoveFromNode(ProcedureLineNode node);
        void OnAddToProcedureLine(ProcedureLine procedureLine);
        void OnRemoveFromProcedureLine(ProcedureLine procedureLine);
    }
}