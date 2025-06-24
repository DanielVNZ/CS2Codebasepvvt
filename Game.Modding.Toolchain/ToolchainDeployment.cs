using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Colossal;
using Game.Modding.Toolchain.Dependencies;
using Game.SceneFlow;
using Game.UI;
using Game.UI.Localization;

namespace Game.Modding.Toolchain;

public static class ToolchainDeployment
{
	public static ToolchainDependencyManager dependencyManager { get; }

	static ToolchainDeployment()
	{
		dependencyManager = new ToolchainDependencyManager();
		dependencyManager.Register<UnityDependency>();
		dependencyManager.Register<UnityLicenseDependency>();
		dependencyManager.Register<UnityModProjectDependency>();
		dependencyManager.Register<DotNetDependency>();
		dependencyManager.Register<ProjectTemplateDependency>();
		dependencyManager.Register<NodeJSDependency>();
		dependencyManager.Register<NpxModProjectDependency>();
		dependencyManager.Register<IDEDependency>();
	}

	public static async void RunWithUI(DeploymentAction action, List<IToolchainDependency> dependencies = null, Action<bool> callback = null)
	{
		if (dependencyManager.isInProgress)
		{
			return;
		}
		(List<IToolchainDependency> accepted, List<IToolchainDependency> discarded) filtered = ToolchainDependencyManager.DependencyFilter.Process(action, dependencies);
		if (filtered.accepted.Count == 0)
		{
			callback?.Invoke(dependencies == null);
			await dependencyManager.GetCurrentState();
			return;
		}
		filtered.accepted.Sort((action < DeploymentAction.Uninstall) ? new Comparison<IToolchainDependency>(IToolchainDependency.InstallSorting) : new Comparison<IToolchainDependency>(IToolchainDependency.UninstallSorting));
		TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
		Dictionary<string, ILocElement> dictionary = new Dictionary<string, ILocElement>();
		foreach (IToolchainDependency item in filtered.accepted)
		{
			dictionary.Add($"Item{dictionary.Count}", (action < DeploymentAction.Uninstall) ? item.installDescr : item.uninstallDescr);
		}
		LocalizedString message = LocalizedString.Id((action < DeploymentAction.Uninstall) ? "Options.WARN_TOOLCHAIN_INSTALL_NEW" : "Options.WARN_TOOLCHAIN_UNINSTALL_NEW");
		ConfirmationDialog dialog = new ConfirmationDialog(details: new LocalizedString(null, string.Join("\n\n", dictionary.Keys.Select((string key) => "- {" + key + "}")), dictionary), title: "Common.DIALOG_TITLE[Warning]", message: message, copyButton: false, confirmAction: "Common.DIALOG_ACTION[Yes]", cancelAction: "Common.DIALOG_ACTION[No]", otherActions: Array.Empty<LocalizedString>());
		GameManager.instance.userInterface.appBindings.ShowConfirmationDialog(dialog, delegate(int msg)
		{
			tcs.SetResult(msg == 0);
		});
		bool goAhead = await tcs.Task;
		if (goAhead)
		{
			await RunImpl(action, filtered.accepted);
		}
		callback?.Invoke(goAhead);
	}

	public static async Task Run(DeploymentAction action, List<IToolchainDependency> dependencies = null)
	{
		if (!dependencyManager.isInProgress)
		{
			(List<IToolchainDependency>, List<IToolchainDependency>) tuple = ToolchainDependencyManager.DependencyFilter.Process(action, dependencies);
			if (tuple.Item1.Count != 0)
			{
				await RunImpl(action, tuple.Item1);
			}
		}
	}

	private static async Task RunImpl(DeploymentAction action, List<IToolchainDependency> dependencies)
	{
		if (action < DeploymentAction.Uninstall)
		{
			await TaskManager.instance.SharedTask<List<IToolchainDependency>>("InstallToolchain", (Func<List<IToolchainDependency>, CancellationToken, Task>)dependencyManager.Install, dependencies, GameManager.instance.terminationToken);
		}
		else
		{
			await TaskManager.instance.SharedTask<List<IToolchainDependency>>("UninstallToolchain", (Func<List<IToolchainDependency>, CancellationToken, Task>)dependencyManager.Uninstall, dependencies, GameManager.instance.terminationToken);
		}
	}
}
