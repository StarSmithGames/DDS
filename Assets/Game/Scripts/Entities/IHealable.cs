public interface IHealable<T> where T : struct
{
    void Heal(T value);
}