public interface IPlayer : IEntity, IKillable
{
	public PlayerStatus Status { get; }
}