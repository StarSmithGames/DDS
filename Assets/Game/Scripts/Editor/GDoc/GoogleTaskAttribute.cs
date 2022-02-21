using System;

namespace Game.Editor.GDocs
{
    /// <summary>
    /// �������, �������� ����� ������ ������.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class GoogleTaskAttribute : System.Attribute
    {
        public string name;

        public GoogleTaskAttribute(string name)
        {
            this.name = name;
        }
	}
}