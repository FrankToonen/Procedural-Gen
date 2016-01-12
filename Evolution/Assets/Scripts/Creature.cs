using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creature : MonoBehaviour
{
	public List<CreaturePart> parts;

	public Texture2D creatureTexture;

	void Start ()
	{
		GenerateCreature ();
	}

	void GenerateCreature ()
	{
		parts = new List<CreaturePart> ();

		parts.Add (new Part_Body (gameObject));
		parts.Add (new Part_Eye (gameObject));
		parts.Add (new Part_Movement (gameObject));

		CreateSprite ();
	}

	void CreateSprite ()
	{
		foreach (CreaturePart part in parts) {
			creatureTexture = part.CreateSprite (creatureTexture);
		}

		Sprite sprite = Sprite.Create (creatureTexture, new Rect (0, 0, creatureTexture.width, creatureTexture.height), new Vector2 (0.5f, 0.5f));
		GetComponent<SpriteRenderer> ().sprite = sprite;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			GenerateCreature ();
		}
	}
}
