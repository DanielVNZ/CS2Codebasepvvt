using Colossal.PSI.Steamworks;
using UnityEngine.Scripting;

namespace Game.Dlc;

[Preserve]
public class SteamworksDlcsMapping : SteamworksDlcMapper
{
	private const uint kProjectWashingtonId = 2427731u;

	private const uint kProjectCaliforniaId = 2427730u;

	private const uint kExpansionPass = 2472660u;

	private const uint kProjectFloridaId = 2427740u;

	private const uint kProjectNewJerseyId = 2427743u;

	public SteamworksDlcsMapping()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		((SteamworksDlcMapper)this).Map(Dlc.LandmarkBuildings, 2427731u);
		((SteamworksDlcMapper)this).Map(Dlc.SanFranciscoSet, 2427730u);
		((SteamworksDlcMapper)this).Map(Dlc.BridgesAndPorts, 2427743u);
	}
}
