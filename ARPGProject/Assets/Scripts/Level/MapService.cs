using System.Collections.Generic;
using UnityEngine;
using Bebis;

public partial class MapService
{
    public const int NumTriesAbort = 100;

    public static IntVector3[] Directions = {
        new IntVector3(1, 0),
        new IntVector3(1, -1),
        new IntVector3(0, -1),
        new IntVector3(-1, -1),
        new IntVector3(-1, 0),
        new IntVector3(-1, 1),
        new IntVector3(0, 1),
        new IntVector3(1, 1)
    };

    public static List<IntVector3> GetPositionsWithinRadius(int minDistance, IntVector3 start, int radius) {
        List<TileNode> toBeVisited = new List<TileNode>();
        List<IntVector3> alreadyVisited = new List<IntVector3>();

        if (!LevelDataManager.Instance.IsWithinMap(start.x, start.y)) {
            CustomLogger.Warn(nameof(MapService), $"Starting position '{start}' is out of bounds!");
            return null;
        }

        List<IntVector3> traversableTargets = new List<IntVector3>();
        TileNode current = new TileNode() {
            X = start.x,
            Y = start.y,
            DistanceFromStart = 0
        };

        toBeVisited.Add(current);
        int count = 0;

        while (toBeVisited.Count != 0) {
            count++;
            current = toBeVisited[0];
            TryAddToList(minDistance, traversableTargets, current);
            toBeVisited.RemoveAt(0);
            alreadyVisited.Add(new IntVector3(current.X, current.Y));

            for (int i = 0; i < Directions.Length; i++) {
                int dirX = Directions[i].x;
                int dirY = Directions[i].y;
                int neighborX = current.X + Directions[i].x;
                int neighborY = current.Y + Directions[i].y;

                int distanceFromStart = DistanceFromStart(start.x, start.y, neighborX, neighborY);
                if (distanceFromStart > radius) {
                    continue;
                } // stay within the radius
                if(!LevelDataManager.Instance.IsWithinMap(neighborX, neighborY)) {
                    continue;
                } // don't check outside of map
                if (alreadyVisited.Contains(new IntVector3(neighborX, neighborY))) {
                    continue;
                } // don't re-attempt tiles we've already checked

                TileInfo info = LevelDataManager.Instance.TileInfos[neighborX][neighborY];
                bool _canTraverse = info != null && info.TileOccupant == null;

                if (!_canTraverse || ContainsNode(neighborX, neighborY, toBeVisited)) {
                    continue;
                }

                TileNode newNode = new TileNode() {
                    X = neighborX,
                    Y = neighborY,
                    DistanceFromStart = distanceFromStart
                };
                toBeVisited.Add(newNode);
            }
            if (count > NumTriesAbort) {
                CustomLogger.Error(nameof(MapService), $"{nameof(GetTraversableTiles)} Aborting after {count} steps!");
                break;
            }
        }

        return traversableTargets;
    }

    public static List<IntVector3> GetTraversableTiles(int radius, IntVector3 start, int minDistance = 0) {
        List<TileNode> toBeVisited = new List<TileNode>();
        List<IntVector3> alreadyVisited = new List<IntVector3>();

        if(!LevelDataManager.Instance.IsWithinMap(start.x, start.y)) {
            CustomLogger.Warn(nameof(MapService), $"Starting position '{start}' is out of bounds!");
            return null;
        }

        List<IntVector3> traversableTargets = new List<IntVector3>();
        TileNode current = new TileNode() {
            X = start.x,
            Y = start.y,
            DistanceFromStart = 0
        };

        toBeVisited.Add(current);
        int count = 0;

        while(toBeVisited.Count != 0) {
            count++;
            current = toBeVisited[0];
            TryAddToList(minDistance, traversableTargets, current);
            toBeVisited.RemoveAt(0);
            alreadyVisited.Add(new IntVector3(current.X, current.Y));

            for(int i = 0; i < Directions.Length; i++) {
                int dirX = Directions[i].x;
                int dirY = Directions[i].y;
                int neighborX = current.X + Directions[i].x;
                int neighborY = current.Y + Directions[i].y;
                IntVector3 neighbor = new IntVector3(neighborX, neighborY);

                int distanceFromStart = DistanceFromStart(start.x, start.y, neighborX, neighborY);
                if(distanceFromStart > radius) {
                    continue;
                }

                if (alreadyVisited.Contains(neighbor) || !LevelDataManager.Instance.IsWithinMap(neighbor.x, neighbor.y)) {
                    continue;
                }

                TileInfo info = LevelDataManager.Instance.TileInfos[neighborX][neighborY];
                bool _canTraverse = info != null && info.TileOccupant == null;

                // if this is a corner piece
                int sumOf = Mathf.Abs(dirX) + Mathf.Abs(dirY);
                if (sumOf == 2 && _canTraverse) {
                    // check if adjacent sides are open
                    TileInfo neighborTileX = LevelDataManager.Instance.TileInfos[current.X + dirX][current.Y];
                    TileInfo neighborTileY = LevelDataManager.Instance.TileInfos[current.X][current.Y + dirY];
                    // check if both tiles are available
                    if (neighborTileX != null) {
                        _canTraverse &= neighborTileX.TileOccupant == null;
                    }
                    if(neighborTileY != null) {
                        _canTraverse &= neighborTileY.TileOccupant == null;
                    }
                }

                if (ContainsNode(neighborX, neighborY, toBeVisited)) {
                    continue;
                }

                TileNode newNode = new TileNode() {
                    X = neighborX,
                    Y = neighborY,
                    DistanceFromStart = distanceFromStart
                };
                toBeVisited.Add(newNode);
            }
            if(count > NumTriesAbort) {
                CustomLogger.Error(nameof(MapService), $"{nameof(GetTraversableTiles)} Aborting after {count} steps!");
                break;
            }
        }

        return traversableTargets;
    }

    public static void TryAddToList(int minDistance, List<IntVector3> list, TileNode node) {
        if(minDistance <= 0) {
            list.Add(new IntVector3(node.X, node.Y));
            return;
        }
        if(node.DistanceFromStart >= minDistance) {
            list.Add(new IntVector3(node.X, node.Y));
        }
    }

    private static bool ContainsNode(int x, int y, List<TileNode> toBeVisited) {
        for (int i = 0; i < toBeVisited.Count; i++) {
            if (toBeVisited[i].X == x && toBeVisited[i].Y == y) {
                return true;
            }
        }
        return false;
    }
}
