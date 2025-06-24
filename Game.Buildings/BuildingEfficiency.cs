using System.Runtime.InteropServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct BuildingEfficiency : IComponentData, IQueryTypeParameter, ISerializable
{
	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write((byte)0);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		byte b = default(byte);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref b);
	}
}
