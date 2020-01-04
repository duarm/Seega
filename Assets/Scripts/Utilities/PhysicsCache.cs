using System.Collections.Generic;
using UnityEngine;

namespace Kurenaiz.Utilities.Physics
{
	//From: Unity 2D Gamekit
	/// <summary>
	/// This class is used as a cache of components on the same gameobjects as colliders.
	/// The intention is that when a collider is found - by raycast or otherwise - the components being sought by those raycasts can be referenced without the need for GetComponent calls.
	/// </summary>
	public class PhysicsCache : MonoBehaviour
	{
		Dictionary<Collider2D, TileField> m_TileFieldCache = new Dictionary<Collider2D, TileField> ();

		void Awake () => PopulateColliderDictionary (m_TileFieldCache);

		protected void PopulateColliderDictionary<TComponent> (Dictionary<Collider2D, TComponent> dict)
		where TComponent : Component
		{
			TComponent[] components = FindObjectsOfType<TComponent> ();

			for (int i = 0; i < components.Length; i++)
			{
				Collider2D[] componentColliders = components[i].GetComponents<Collider2D> ();

				for (int j = 0; j < componentColliders.Length; j++)
				{
					dict.Add (componentColliders[j], components[i]);
				}
			}
		}

		public bool ColliderHasTileField (Collider2D collider)
		{
			return m_TileFieldCache.ContainsKey (collider);
		}

		public bool TryGetTileField (Collider2D collider, out TileField tileField)
		{
			return m_TileFieldCache.TryGetValue (collider, out tileField);
		}
	}
}