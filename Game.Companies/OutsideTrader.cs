using System.Runtime.InteropServices;
using Unity.Entities;

namespace Game.Companies;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct OutsideTrader : IComponentData, IQueryTypeParameter
{
}
