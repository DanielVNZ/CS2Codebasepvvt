using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Game.SceneFlow;
using UnityEngine;
using UnityEngine.Networking;

namespace Game;

public static class UnityWebRequestExtensionMethods
{
	public class UnityWebRequestAwaiter : INotifyCompletion
	{
		private UnityWebRequestAsyncOperation asyncOp;

		private Action continuation;

		public bool IsCompleted => ((AsyncOperation)asyncOp).isDone;

		public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
		{
			this.asyncOp = asyncOp;
			((AsyncOperation)this.asyncOp).completed += OnRequestCompleted;
		}

		public Result GetResult()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return asyncOp.webRequest.result;
		}

		public void OnCompleted(Action continuation)
		{
			this.continuation = continuation;
			if (IsCompleted)
			{
				OnRequestCompleted((AsyncOperation)(object)asyncOp);
			}
		}

		public void OnRequestCompleted(AsyncOperation obj)
		{
			continuation?.Invoke();
		}

		public UnityWebRequestAwaiter GetAwaiter()
		{
			return this;
		}
	}

	public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
	{
		return new UnityWebRequestAwaiter(asyncOp);
	}

	public static UnityWebRequestAwaiter ConfigureAwait(this UnityWebRequestAsyncOperation asyncOperation, Func<UnityWebRequestAsyncOperation, bool> updaterMethod, CancellationToken token, float connectionTimeout = 0f)
	{
		float progress = 0f;
		float time = Time.realtimeSinceStartup;
		GameManager.instance.RegisterUpdater(delegate
		{
			if (token.IsCancellationRequested)
			{
				asyncOperation.webRequest.Abort();
				return true;
			}
			if (connectionTimeout > 0f)
			{
				if (((AsyncOperation)asyncOperation).progress > progress)
				{
					progress = ((AsyncOperation)asyncOperation).progress;
					time = Time.realtimeSinceStartup;
				}
				else if (Time.realtimeSinceStartup - time > connectionTimeout)
				{
					asyncOperation.webRequest.Abort();
					return true;
				}
			}
			return updaterMethod(asyncOperation);
		});
		return new UnityWebRequestAwaiter(asyncOperation);
	}
}
