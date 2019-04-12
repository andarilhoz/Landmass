using System.Collections.Generic;
using UnityEngine;

namespace _scripts
{
    public class EndlessTerrain : MonoBehaviour
    {
        public const float maxViewDst = 300;
        public Transform viewer;

        public static Vector2 viewerPoistion;

        private int chunkSize;
        private int chunksVisibleInViewDistance;

        private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

        private void Start()
        {
            chunkSize = MapGenerator.mapChunkSize - 1;
            chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDst / chunkSize);
        }

        private void Update()
        {
            viewerPoistion = new Vector2(viewer.position.x, viewer.position.z);
        }

        private void UpdateVisibleChunks()
        {
            var currentChunkCoordX = Mathf.RoundToInt(viewerPoistion.x / chunkSize);
            var currentChunkCoordY = Mathf.RoundToInt(viewerPoistion.y / chunkSize);


            for (var yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
            {
                for (var xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
                {
                    var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if ( terrainChunkDictionary.ContainsKey(viewedChunkCoord) )
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    else
                    {
                        terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize));
                    }
                }
            }
        }

        public class TerrainChunk
        {
            private GameObject meshObject;
            private Vector2 position;
            private Bounds bounds;

            public TerrainChunk(Vector2 coord, int size)
            {
                position = coord * size;
                bounds = new Bounds(position, Vector2.one * size);
                var positionV3 = new Vector3(position.x, 0, position.y);
                meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);

                meshObject.transform.position = positionV3;
                meshObject.transform.localScale = Vector3.one * size / 10f;
                SetVisible(false);
            }

            public void UpdateTerrainChunk()
            {
                var viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPoistion));
                var visible = viewerDstFromNearestEdge <= maxViewDst;
                SetVisible(visible);
            }

            public void SetVisible(bool visible)
            {
                meshObject.SetActive(visible);
            }
        }
    }
}