using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics
{
    public class Tilemap
    {
        private readonly Tileset _tileset;
        private readonly int[] _tiles;

        /// <summary>
        /// Gets the total number of rows in this tilemap.
        /// </summary>
        public int Rows { get; }

        /// <summary>
        /// Gets the total number of columns in this tilemap.
        /// </summary>
        public int Columns { get; }

        /// <summary>
        /// Gets the total number of tiles in this tilemap.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets or Sets the scale factor to draw each tile at.
        /// </summary>
        public Vector2 Scale { get; set; }

        /// <summary>
        /// Gets the width, in pixels, each tile is drawn at.
        /// </summary>
        public float TileWidth => _tileset.TileWidth * Scale.X;

        /// <summary>
        /// Gets the height, in pixels, each tile is drawn at.
        /// </summary>
        public float TileHeight => _tileset.TileHeight * Scale.Y;

        /// <summary>
        /// Creates a new tilemap.
        /// </summary>
        /// <param name="tileset">The tileset used by this tilemap.</param>
        /// <param name="columns">The total number of columns in this tilemap.</param>
        /// <param name="rows">The total number of rows in this tilemap.</param>
        public Tilemap(Tileset tileset, int columns, int rows)
        {
            _tileset = tileset;
            Rows = rows;
            Columns = columns;
            Count = Columns * Rows;
            Scale = Vector2.One;
            _tiles = new int[Count];
        }

        /// <summary>
        /// Sets the tile at the given index in this tilemap to use the tile from
        /// the tileset at the specified tileset id.
        /// </summary>
        public void SetTile(int index, int tilesetID)
        {
            _tiles[index] = tilesetID;
        }

        /// <summary>
        /// Sets the tile at the given column and row in this tilemap to use the tile
        /// from the tileset at the specified tileset id.
        /// </summary>
        public void SetTile(int column, int row, int tilesetID)
        {
            int index = row * Columns + column;
            SetTile(index, tilesetID);
        }
        public int GetTileId(int column, int row)
        {
        int index = row * Columns + column;
        return _tiles[index];
        }

        /// <summary>
        /// Gets the texture region of the tile from this tilemap at the specified index.
        /// </summary>
        public TextureRegion GetTile(int index)
        {
            return _tileset.GetTile(_tiles[index]);
        }

        /// <summary>
        /// Gets the texture region of the tile from this tilemap at the specified
        /// column and row.
        /// </summary>
        public TextureRegion GetTile(int column, int row)
        {
            int index = row * Columns + column;
            return GetTile(index);
        }

        /// <summary>
        /// Draws this tilemap using the given sprite batch.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Count; i++)
            {
                int tilesetIndex = _tiles[i];
                TextureRegion tile = _tileset.GetTile(tilesetIndex);

                int x = i % Columns;
                int y = i / Columns;

                Vector2 position = new Vector2(x * TileWidth, y * TileHeight);
                tile.Draw(spriteBatch, position, Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 1.0f);
            }
        }

        /// <summary>
        /// Creates a new tilemap based on a tilemap xml configuration file.
        /// </summary>
        public static Tilemap FromFile(ContentManager content, string filename)
        {
            string filePath = Path.Combine(content.RootDirectory, filename);

            using (Stream stream = TitleContainer.OpenStream(filePath))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    XDocument doc = XDocument.Load(reader);
                    XElement root = doc.Root;

                    XElement tilesetElement = root.Element("Tileset");

                    string regionAttribute = tilesetElement.Attribute("region").Value;
                    string[] split = regionAttribute.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    int x = int.Parse(split[0]);
                    int y = int.Parse(split[1]);
                    int width = int.Parse(split[2]);
                    int height = int.Parse(split[3]);

                    int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
                    int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
                    string contentPath = tilesetElement.Value;

                    Texture2D texture = content.Load<Texture2D>(contentPath);
                    TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);
                    Tileset tileset = new Tileset(textureRegion, tileWidth, tileHeight);

                    XElement tilesElement = root.Element("Tiles");
                    string[] rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

                    Tilemap tilemap = new Tilemap(tileset, columnCount, rows.Length);

                    for (int row = 0; row < rows.Length; row++)
                    {
                        string[] columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        for (int column = 0; column < columnCount; column++)
                        {
                            int tilesetIndex = int.Parse(columns[column]);
                            tilemap.SetTile(column, row, tilesetIndex);
                        }
                    }

                    return tilemap;
                }
            }
        }
    }
}