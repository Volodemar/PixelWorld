using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Perlin2D
{
    byte[] permutationTable;

    public Perlin2D(int seed = 0)
    {
        var rand = new System.Random(seed);
        permutationTable = new byte[1024];
        rand.NextBytes(permutationTable);
    }

	/// <summary>
	/// Возвращает псевдослучайные градиентные вектора (случайно направленные из окружающих точек во вне)
	/// </summary>
	/// <param name="x">Координы для которых псевдослучайное число одинаковое</param>
	/// <param name="y">Координы для которых псевдослучайное число одинаковое</param>
	/// <returns>Возвращает вектор градиента</returns>
    private float[] GetPseudoRandomGradientVector(int x, int y)
    {
        int v = (int)(((x * 1836311903) ^ (y * 2971215073) + 4807526976) & 1023);
        v = permutationTable[v]&3;

        switch (v)
        {
            case 0:  return new float[]{  1, 0 };
            case 1:  return new float[]{ -1, 0 };
            case 2:  return new float[]{  0, 1 };
            default: return new float[]{  0,-1 };
        }
    }

    static float QunticCurve(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

	/// <summary>
	/// Функция интерполяции
	/// </summary>
	/// <param name="a">Число А</param>
	/// <param name="b">Число Б</param>
	/// <param name="t">Смещение от А до Б</param>
	/// <returns></returns>
    static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

	/// <summary>
	/// Скалярное произведение векторов
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
    static float Dot(float[] a, float[] b)
    {
        return a[0] * b[0] + a[1] * b[1];
    }

	/// <summary>
	/// Сама функция расчета шума Перлина
	/// </summary>
	/// <param name="fx">Координата</param>
	/// <param name="fy">Координата</param>
	/// <returns>Случайное число от 0 до 1</returns>
    public float Noise(float fx, float fy)
    {
        int left = (int)System.Math.Floor(fx);
        int top  = (int)System.Math.Floor(fy);
        float pointInQuadX = fx - left;
        float pointInQuadY = fy - top;

        float[] topLeftGradient     = GetPseudoRandomGradientVector(left,   top  );
        float[] topRightGradient    = GetPseudoRandomGradientVector(left+1, top  );
        float[] bottomLeftGradient  = GetPseudoRandomGradientVector(left,   top+1);
        float[] bottomRightGradient = GetPseudoRandomGradientVector(left+1, top+1);

        float[] distanceToTopLeft     = new float[]{ pointInQuadX,   pointInQuadY   };
        float[] distanceToTopRight    = new float[]{ pointInQuadX-1, pointInQuadY   };
        float[] distanceToBottomLeft  = new float[]{ pointInQuadX,   pointInQuadY-1 };
        float[] distanceToBottomRight = new float[]{ pointInQuadX-1, pointInQuadY-1 };

        float tx1 = Dot(distanceToTopLeft,     topLeftGradient);
        float tx2 = Dot(distanceToTopRight,    topRightGradient);
        float bx1 = Dot(distanceToBottomLeft,  bottomLeftGradient);
        float bx2 = Dot(distanceToBottomRight, bottomRightGradient);

        pointInQuadX = QunticCurve(pointInQuadX);
        pointInQuadY = QunticCurve(pointInQuadY);

        float tx = Lerp(tx1, tx2, pointInQuadX);
        float bx = Lerp(bx1, bx2, pointInQuadX);
        float tb = Lerp(tx, bx, pointInQuadY);

        return tb;
    }

	/// <summary>
	/// Функция шума перлина с октавами и частотой и упорством
	/// </summary>
	/// <param name="fx">Координата по оси Х</param>
	/// <param name="fy">Координата по оси Y</param>
	/// <param name="octaves">Количество октав (количество интерполяций)</param>
	/// <param name="persistence">Упорство</param>
	/// <returns>Возвращает значение шума для данных координат</returns>
    public float Noise(float fx, float fy, int octaves, float amplitude = 1.0f, float persistence = 0.5f)
    {
        float max = 0;
        float result = 0;

        while (octaves-- > 0)
        {
            max			+= amplitude;
            result		+= Noise(fx, fy) * amplitude;
            amplitude	*= persistence;
            fx *= 2;
            fy *= 2;
        }

        return result/max;
    }
}
