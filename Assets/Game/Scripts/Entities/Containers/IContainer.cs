public interface IContainer : IEntity, IInteractable, ISearchable
{
	public ContainerData ContainerData { get; }
}