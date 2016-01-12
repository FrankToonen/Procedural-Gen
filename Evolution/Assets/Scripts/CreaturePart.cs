using UnityEngine;
using System.Collections;

[System.Serializable]
public class CreaturePart
{
	protected GameObject parent;
	public string partName;

	public CreaturePart (GameObject _parent)
	{
		parent = _parent;
	}

	public virtual Texture2D CreateSprite (Texture2D texture)
	{
		return texture;
	}

	protected virtual void UpdatePart ()
	{

	}
}
