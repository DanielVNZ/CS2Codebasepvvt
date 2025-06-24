using System;
using System.Collections;
using UnityEngine;

namespace Game.Debug;

public class TestScenarioHelperUnhandledException : MonoBehaviour
{
	private void Start()
	{
		((MonoBehaviour)this).StartCoroutine(CoThrowUnhandledException());
	}

	private IEnumerator CoThrowUnhandledException()
	{
		yield return (object)new WaitForSeconds(5f);
		throw new Exception("TestScenarioHelperUnhandledException");
	}
}
