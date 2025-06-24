using System;

namespace Game.Prefabs;

[Flags]
public enum BuildingFlags : uint
{
	RequireRoad = 1u,
	NoRoadConnection = 2u,
	LeftAccess = 4u,
	RightAccess = 8u,
	BackAccess = 0x10u,
	RestrictedPedestrian = 0x20u,
	RestrictedCar = 0x40u,
	ColorizeLot = 0x80u,
	HasLowVoltageNode = 0x100u,
	HasWaterNode = 0x200u,
	HasSewageNode = 0x400u,
	HasInsideRoom = 0x800u,
	RestrictedParking = 0x1000u,
	RestrictedTrack = 0x2000u,
	CanBeOnRoad = 0x4000u
}
