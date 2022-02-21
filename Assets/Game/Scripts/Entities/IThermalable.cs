public interface IThermalable
{
    Modifier[] Modifiers { get; }

    void AddThermalModifer(Modifier modifier);
    void RemoveThermalModifer(Modifier modifier);
}