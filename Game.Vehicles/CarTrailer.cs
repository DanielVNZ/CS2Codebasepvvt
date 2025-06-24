using System.Runtime.InteropServices;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Vehicles;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct CarTrailer : IComponentData, IQueryTypeParameter, IEmptySerializable
{
}
