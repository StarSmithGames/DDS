public interface IItem : IEntity, IInteractable
{
    public ItemData ItemData { get; }
}