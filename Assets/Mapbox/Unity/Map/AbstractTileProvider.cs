namespace Mapbox.Unity.Map
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Map;

	public abstract class AbstractTileProvider : MonoBehaviour, ITileProvider
	{
		public event Action<UnwrappedTileId> OnTileAdded = delegate { };
		public event Action<UnwrappedTileId> OnTileRemoved = delegate { };

		protected IMap _map;

		protected List<UnwrappedTileId> _activeTiles;

		public void Initialize(IMap map)
		{
			_activeTiles = new List<UnwrappedTileId>();
			_map = map;
			OnInitialized();
		}

		protected void AddTile(UnwrappedTileId tile)
		{
			if (_activeTiles.Contains(tile))
			{
				return;
			}

			_activeTiles.Add(tile);
			OnTileAdded(tile);
		}

        protected void RemoveTile(UnwrappedTileId tile)
        {
            if (!_activeTiles.Contains(tile))
            {
                return;
            }

            _activeTiles.Remove(tile);
            OnTileRemoved(tile);
        }

        public void ClearTiles()
        {
            foreach (var t in _activeTiles)
            {
                OnTileRemoved(t);
            }

            _activeTiles.Clear();
        }

        internal abstract void OnInitialized();
	}
}
