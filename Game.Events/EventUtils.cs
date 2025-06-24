using Game.Prefabs;
using Unity.Mathematics;

namespace Game.Events;

public static class EventUtils
{
	public const uint MIN_IN_DANGER_TIME = 64u;

	public const float FLOOD_DEPTH_TOLERANCE = 0.5f;

	public static float GetSeverity(float3 position, WeatherPhenomenon weatherPhenomenon, WeatherPhenomenonData weatherPhenomenonData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		float num = math.distance(((float3)(ref position)).xz, ((float3)(ref weatherPhenomenon.m_HotspotPosition)).xz) / weatherPhenomenon.m_HotspotRadius;
		float num2 = weatherPhenomenon.m_Intensity * weatherPhenomenonData.m_DamageSeverity * (1f - num);
		return math.select(num2, 0f, num2 < 0.001f);
	}

	public static bool IsWorse(DangerFlags flags, DangerFlags other)
	{
		DangerFlags dangerFlags = flags ^ other;
		if ((dangerFlags & DangerFlags.Evacuate) != 0)
		{
			return (flags & DangerFlags.Evacuate) != 0;
		}
		if ((dangerFlags & DangerFlags.StayIndoors) != 0)
		{
			return (flags & DangerFlags.StayIndoors) != 0;
		}
		return false;
	}
}
