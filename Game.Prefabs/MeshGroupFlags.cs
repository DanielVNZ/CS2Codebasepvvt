using System;

namespace Game.Prefabs;

[Flags]
public enum MeshGroupFlags : uint
{
	RequireCold = 1u,
	RequireWarm = 2u,
	RequireHome = 4u,
	RequireHomeless = 8u,
	RequireMotorcycle = 0x10u,
	ForbidMotorcycle = 0x20u
}
