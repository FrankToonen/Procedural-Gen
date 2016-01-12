using UnityEngine;
using System.Collections;

public class Part_Movement : CreaturePart
{
	public float moveSpeed;
	public float rotationSpeed;

	public Part_Movement (GameObject _parent)
		: base (_parent)
	{
		partName = "movement";
	}

	public override Texture2D CreateSprite (Texture2D texture)
	{
		return texture;
	}

	protected override void UpdatePart ()
	{
		base.UpdatePart ();
	}
}
