using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using static Unity.MathematicsX.mathX;

public static class TilePhysics
{
    public struct Line2D
    {
        private fix _a;
        private fix _b;

        public Line2D(fix2 start, fix2 end)
        {
            _a = (end.y - start.y) / (end.x - start.x);
            _b = start.y - (_a * start.x);
        }

        public fix SolveY(fix x) => _a * x + _b;
        public fix SolveX(fix y) => (y - _b) / _a;
    }

    public static void FindCrossedTiles(fix2 start, fix2 end, fix bevel, NativeList<int2> crossedTiles)
    {
        int2 tileStart = int2((int)start.x, (int)start.y);
        int2 tileEnd = int2((int)end.x, (int)end.y);

        // Same tile optim
        if (tileStart.Equals(tileEnd))
        {
            crossedTiles.Add(tileStart);
            return;
        }

        // Only 1 axis optim
        int2 v = tileEnd - tileStart;
        if (v.x == 0)
        {
            AppendVerticalTiles(crossedTiles, tileStart.x, tileStart.y, tileEnd.y);
            return;
        }
        if (v.y == 0)
        {
            AppendHorizontalTiles(crossedTiles, tileStart.y, tileStart.x, tileEnd.x);
            return;
        }

        bool rotateOptim = abs(v.y) > abs(v.x);

        if (rotateOptim)
        {
            start = rotate90(start);
            end = rotate90(end);
        }

        if (bevel > 0)
        {
            FindCrossedTilesInternal_WithBevel(start, end, bevel, crossedTiles);
        }
        else
        {
            FindCrossedTilesInternal_NoBevel(start, end, crossedTiles);
        }

        if (rotateOptim)
        {
            for (int i = 0; i < crossedTiles.Length; i++)
            {
                crossedTiles[i] = rotate270(crossedTiles[i]);
            }
        }

        return;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static fix2 rotate90(fix2 v) => fix2(v.y, -v.x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int2 rotate270(int2 v) => int2(-v.y - 1, v.x);

    private static void FindCrossedTilesInternal_NoBevel(fix2 start, fix2 end, NativeList<int2> crossedTiles)
    {
        Line2D line = new Line2D(start, end);

        int vDir = (int)sign(end.y - start.y);

        int y = vDir == 1 ? floorToInt(start.y) : ceilToInt(start.y);
        int endY = vDir == 1 ? floorToInt(end.y) : ceilToInt(end.y);
        int yOffset = vDir == 1 ? 0 : -1;

        int x1 = (int)start.x;
        int x2;

        while (y != endY)
        {

            x2 = (int)line.SolveX(y + vDir);

            AppendHorizontalTiles(crossedTiles, y + yOffset, x1, x2);

            x1 = x2;
            y += vDir;
        }

        x2 = (int)end.x;

        AppendHorizontalTiles(crossedTiles, y + yOffset, x1, x2);
    }

    private static void FindCrossedTilesInternal_WithBevel(fix2 start, fix2 end, fix bevel, NativeList<int2> crossedTiles)
    {
        Line2D line = new Line2D(start, end);
        int startX = (int)start.x;
        int endX = (int)end.x;

        int hDir = sign(endX - startX);
        int m = startX;

        fix x1;
        fix x2;

        if (hDir == 1)
        {
            x1 = start.x;
            x2 = max(start.x, (m + 1) - bevel);
        }
        else
        {
            x1 = min(start.x, m + bevel);
            x2 = start.x;
        }

        while (m != endX)
        {
            AppendVerticalTiles(crossedTiles, m, (int)line.SolveY(x1), (int)line.SolveY(x2));

            m += hDir;

            x1 = m + bevel;
            x2 = (m + 1) - bevel;
        }


        if (hDir == 1)
        {
            x1 = min(x1, end.x);
            x2 = end.x;
        }
        else
        {
            x1 = end.x;
            x2 = max(x2, end.x);
        }

        AppendVerticalTiles(crossedTiles, m, (int)line.SolveY(x1), (int)line.SolveY(x2));
    }

    private static void AppendVerticalTiles(NativeList<int2> tiles, int x, int yBegin, int yEnd)
    {
        int y = yBegin;
        int step = sign(yEnd - yBegin);

        tiles.Add(int2(x, y));

        while (y != yEnd)
        {
            y += step;
            tiles.Add(int2(x, y));
        }
    }

    private static void AppendHorizontalTiles(NativeList<int2> tiles, int y, int xBegin, int xEnd)
    {
        int x = xBegin;
        int step = sign(xEnd - xBegin);

        tiles.Add(int2(x, y));

        while (x != xEnd)
        {
            x += step;
            tiles.Add(int2(x, y));
        }
    }
}