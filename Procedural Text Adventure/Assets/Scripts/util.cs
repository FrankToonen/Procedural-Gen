using UnityEngine;
using System.Collections;

public static class util
{
	public static float ClampToNearest (float fromValue, float[] toValues)
	{
		int index = 0;
		float difference = float.MaxValue;

		for (int i = 0; i < toValues.Length; i++) {
			float newDifference = Mathf.Abs (toValues [i] - fromValue);
			if (newDifference < difference) {
				index = i;
				difference = newDifference;
			}
		}

		return toValues [index];
	}
}
