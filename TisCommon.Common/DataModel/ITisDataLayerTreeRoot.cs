
namespace TiS.Core.TisCommon.DataModel
{
    [EmbedAttribute("Guid(\"94CF9A8D-E563-4B1A-B059-7A38B6FEA022\")")]
    public interface ITisDataLayerTreeRoot : ITisDataLayerTreeNode
    {
        ITisEntityReflection TreeQuery { get; }
    }
}
