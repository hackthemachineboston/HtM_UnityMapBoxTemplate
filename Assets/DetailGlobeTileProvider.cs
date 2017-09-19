using System.Collections.Generic;
using System.IO;

namespace Mapbox.Unity.Map
{
    using Mapbox.Utils;
    using Mapbox.Map;
    using Mapbox.Unity.Utilities;

    public class DetailGlobeTileProvider : AbstractTileProvider
    {
        public bool showHighResArea = true;
        public double highResN = 41.6;
        public double highResE = 145;
        public double highResS = 30;
        public double highResW = 129;
        public int fullZoom = 8;

        List<HashSet<CanonicalTileId>> overlappingTileCoverCache = new List<HashSet<CanonicalTileId>>();

        List<UnwrappedTileId> finalTiles = new List<UnwrappedTileId>();

        void AddTiles(Vector2dBounds bounds, int zoom)
        {
            var tileCover = TileCover.Get(bounds, zoom);

            if (zoom == fullZoom)
            {
                foreach (var tile in tileCover)
                    finalTiles.Add(new UnwrappedTileId(tile.Z, tile.X, tile.Y));
                return;
            }

            var overlappingTileCover = overlappingTileCoverCache[zoom - _map.Zoom];

            foreach (var tile in tileCover)
            {
                if (!overlappingTileCover.Contains(tile))
                    finalTiles.Add(new UnwrappedTileId(tile.Z, tile.X, tile.Y));
                else
                    AddTiles(Conversions.TileIdToBounds(tile.X, tile.Y, tile.Z), zoom + 1);
            }
        }

        internal override void OnInitialized()
        {
            // HACK: don't allow too many tiles to be requested.
            if (_map.Zoom > 5 || fullZoom > 8)
            {
                throw new System.Exception("Too many tiles! Use a lower zoom level!");
            }

            if (showHighResArea)
            {
                string cacheFilename = ((int)(highResN * highResE * highResS * highResW * fullZoom)).ToString() + ".cache";

                if (File.Exists(cacheFilename))
                {
                    using (BinaryReader reader = new BinaryReader(File.Open(cacheFilename, FileMode.Open)))
                    {
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; ++i)
                        {
                            int z = reader.ReadInt32();
                            int x = reader.ReadInt32();
                            int y = reader.ReadInt32();

                            AddTile(new UnwrappedTileId(z, x, y));
                        }
                    }

                    return;
                }

                var sw = new Vector2d(highResS, highResW);
                var ne = new Vector2d(highResN, highResE);
                var b = new Vector2dBounds(sw, ne);

                for (int i = _map.Zoom; i < fullZoom; ++i)
                    overlappingTileCoverCache.Add(TileCover.Get(b, i));

                AddTiles(Vector2dBounds.World(), _map.Zoom);

                using (BinaryWriter writer = new BinaryWriter(File.Open(cacheFilename, FileMode.Create)))
                {
                    writer.Write(finalTiles.Count);
                    foreach (var t in finalTiles)
                    {
                        writer.Write(t.Z);
                        writer.Write(t.X);
                        writer.Write(t.Y);

                        AddTile(t);
                    }
                }
            }
            else
            {
                foreach (var tile in TileCover.Get(Vector2dBounds.World(), _map.Zoom))
                    AddTile(new UnwrappedTileId(tile.Z, tile.X, tile.Y));
            }
        }
    }
}
