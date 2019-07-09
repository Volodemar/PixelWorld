using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
	/// <summary>
	/// Объект для клонирования и визуализации шума (простой примитив например Куб)
	/// </summary>
	public GameObject inGameObject;

	/// <summary>
	/// Дистанция до камеры при которой создаются блоки
	/// </summary>
	public float ViewDistance = 100.0f;

	//private int[] x = new int[100];

	/// <summary>
	/// Генератор случайных чисел
	/// </summary>
	System.Random rnd = new System.Random();

	/// <summary>
	/// Данные мира
	/// </summary>
	public MapData mapData;

	/// <summary>
	/// Время через которое обновляется мир 
	/// </summary>
	public	float TimeRefrash = 10.0f;

	/// <summary>
	/// Предыдущая позиция камеры при отрисовке
	/// </summary>
	private Vector3 lastCamPos;
	private Camera cam;
	private float timeRefrash;
	
	/// <summary>
	/// Инициализация объектов
	/// </summary>
	private void Awake()
	{
		cam = Camera.main;
		timeRefrash = TimeRefrash;
	}

	/// <summary>
	/// Статическая генерация
	/// </summary>
	private void FixedUpdate()
	{

	}

	/// <summary>
	/// Старт программы
	/// </summary>
	void Start()
    {
		// Заполняем массив простым шумом 0..1
		Perlin2D P2D = new Perlin2D(2);
		mapData = new MapData(512,512, 250);

		cam.transform.position = new Vector3(mapData.Height.GetLength(0)/2, 20, mapData.Height.GetLength(1)/2);

		// Заполняем данными карту
		for (int x = 0; x < mapData.Height.GetLength(0)-1; x++)
		{
			for (int z = 0; z < mapData.Height.GetLength(1)-1; z++)
			{
				// Параметры обтекаемости мира
				float fx = (float)x / 250 * 5.0f;
				float fz = (float)z / 250 * 5.0f;

				// Получаем шум Перлина
				float fy = P2D.Noise(fx,fz); // , 3, 0.1f, 0.5f);
				
				// Вариативность высоты
				int y = Mathf.RoundToInt(fy*50) < 0 ? 0 : Mathf.RoundToInt(fy*50);

				// Сохраняем данные в карту
				mapData.Height[x,z] = y;

				// Создаем куб в видимой области
				CreateNewCubes(x, y, z, ViewDistance);
			}
		}		

		// Перемещаем камеру на верхний куб
		cam.transform.position = new Vector3(cam.transform.position.x, mapData.Height[(int)cam.transform.position.x, (int)cam.transform.position.z]+5 ,cam.transform.position.z);
		// Оптимизация: сохраним первую позицию камеры при отрисовке
		lastCamPos = Camera.main.transform.position;
	}

	/// <summary>
	/// Проверяет, что объект в области видимости
	/// </summary>
	/// <param name="x">Координата объекта</param>
	/// <param name="y">Координата объекта</param>
	/// <param name="z">Координата объекта</param>
	/// <param name="viewDistance">Дистанция видимости</param>
	private void CreateNewCubes(int x, int y, int z, float viewDistance)
	{
		if(!mapData.Exists(x,y,z))
		{
			//В области пользователя создаем активные кубы
			float x_min = cam.transform.position.x - viewDistance;
			float x_max = cam.transform.position.x + ViewDistance;
			float y_min = cam.transform.position.y - viewDistance;
			float y_max = cam.transform.position.y + ViewDistance;
			if((x_min < x && x < x_max) && (y_min < y && y < y_max))
			{
				// Создаем куб
				Vector3 vCubePos = new Vector3(x, y, z);
				GameObject newCube = Instantiate(inGameObject, vCubePos, Quaternion.identity, this.transform);
				newCube.name = $"Cube Vector {x},{mapData.Height[x, z]},{z}";
				newCube.gameObject.SetActive(true);

				// Записываем в дату созданный куб
				mapData.SetData(x,y,z, ref newCube);
			}
		}
	}

	void Update()
    {
		// Обработка мира относительно новой позиции игрока в равные участки времени
		if(timeRefrash < 0)
		{
			Vector3 curentPos = cam.transform.position;

			if(lastCamPos != curentPos && 1==0) //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!)
			{
				// Сохраняем позицию прошлого обновления мира
				lastCamPos = curentPos;

				// Обновляем мир в радиусе видимости
				#region
					// Расчет минимальных и максимальных значений 
					int x_min = (curentPos.x - ViewDistance < 0) ? 0 : (int)(curentPos.x - ViewDistance);
					int z_min = (curentPos.z - ViewDistance < 0) ? 0 : (int)(curentPos.z - ViewDistance);
					int x_max = (curentPos.x + ViewDistance > mapData.Height.GetLength(0) - 1) ? mapData.Height.GetLength(0) - 1 : (int)(curentPos.x + ViewDistance);
					int z_max = (curentPos.z + ViewDistance > mapData.Height.GetLength(1) - 1) ? mapData.Height.GetLength(1) - 1 : (int)(curentPos.z + ViewDistance);

					// Скрыть оставшиеся позади
					int countCubes = this.transform.childCount;

					print($"Сейчас кубов: {countCubes}");

					for (int i = 0; i < countCubes; i++)
					{
						GameObject child = this.transform.GetChild(i).gameObject;
						if(child.transform.position.x < x_min)
							if(child.transform.position.x > x_max)
								if(child.transform.position.z < z_min)
									if(child.transform.position.z > x_max)
									{
										this.transform.GetChild(i).gameObject.SetActive(false);
										//mapData.DelData((int)child.transform.position.x, (int)child.transform.position.y, (int)child.transform.position.z);
										//Destroy(this.transform.GetChild(i));
									}
					}

					// Считываем данные мира и устанавливаем активность
					for (int x = x_min; x < x_max; x++)
					{
						for (int z = z_min; z < z_max; z++)
						{
							int y = (int)mapData.Height[x, z];
							if (mapData.Exists(x, y, z))
							{
								mapData.GetData(x, y, z).SetActive(true);
							}
							else
							{
								CreateNewCubes(x, y, z, ViewDistance);
							}
						}
					}

					// Если объект дальше видимости активный, то выключаем его 
					//int NotView = (int)ViewDistance*2;
					//int x_min2 = (curentPos.x - NotView < 0) ? 0 : (int)(curentPos.x - NotView);
					//int z_min2 = (curentPos.z - NotView < 0) ? 0 : (int)(curentPos.z - NotView);
					//int x_max2 = (curentPos.x + NotView > mapData.Height.GetLength(0) - 1) ? mapData.Height.GetLength(0) - 1 : (int)(curentPos.x + NotView);
					//int z_max2 = (curentPos.z + NotView > mapData.Height.GetLength(1) - 1) ? mapData.Height.GetLength(1) - 1 : (int)(curentPos.z + NotView);

					// Считываем данные мира и снимаем активность
					//for (int x = 0; x < mapData.Height.GetLength(0) - 1; x++)
					//{
					//	for (int z = 0; z < mapData.Height.GetLength(1) - 1; z++)
					//	{
					//		if((x < x_min || x > x_max) && (z < z_min || z > z_max))
					//		{
					//			int y = (int)mapData.Height[x, z];
					//			if (mapData.Exists(x, y, z))
					//			{
					//				GameObject go = mapData.GetData(x, y, z);
					//				go.SetActive(false);
					//			}								
					//		}
					//	}
					//}

				#endregion

			}

			timeRefrash = TimeRefrash;
		}
		else
		{
			timeRefrash -= Time.fixedDeltaTime;
		}
    }


	/// <summary>
	/// Генерирует массив значений 0..1
	/// </summary>
	/// <param name="len">Длина массива</param>
	/// <returns>Возвращает массив типа double</returns>
	private float[] Noise(int len)
	{
		float[] res = new float[len];
		for(int i=0;i<res.Length;i++)
		{
			res[i] = (float)rnd.NextDouble();
		}

		return res;
	}

	/// <summary>
	/// Увеличесние частоты
	/// </summary>
	/// <param name="yn">Массив вершин</param>
	/// <param name="freqency">Частота</param>
	public float[] FrequencyNoise(float[] yn, int freqency)
	{
		float[] res = new float[yn.Length];
		for(int x=0;x<yn.Length;x++)
		{
			res[x] = Mathf.RoundToInt(yn[x]*freqency);
		}
		return res;
	}

	/// <summary>
	/// Увеличесние частоты
	/// </summary>
	/// <param name="yn">Массив вершин</param>
	/// <param name="freqency">Частота</param>
	public float[,] FrequencyNoise2D(float[,] yn, int freqency)
	{
		float[,] res = new float[yn.GetLength(0), yn.GetLength(1)];
		for(int x=0;x<yn.GetLength(0);x++)
		{
			for(int z=0;z<yn.GetLength(1);z++)
			{
				res[x,z] = Mathf.RoundToInt(yn[x,z]*freqency);
			}
		}
		return res;
	}

	/// <summary>
	/// Увеличесние частоты
	/// </summary>
	/// <param name="yn">Массив вершин</param>
	/// <param name="freqency">Частота</param>
	public int[] RoundNoise(float[] yn, int freqency)
	{
		int[] res = new int[yn.Length];
		for(int x=0;x<yn.Length;x++)
		{
			res[x] = Mathf.RoundToInt(yn[x]*freqency);
		}
		return res;
	}

	/// <summary>
	/// Сглаживание масиива
	/// </summary>
	/// <param name="xn">Массив шума 0..1</param>
	/// <returns>Возвращает сглаженный шум</returns>
	public double[] SmoothNoise(double[] xn)
	{
		double[] res = new double[xn.Length];
		for(int i=0;i<xn.Length;i++)
		{
			double a = (i == 0) ? xn[i+1]/4 : xn[i-1]/4;
			double b = (i == xn.Length-1) ? xn[i-1]/4 : xn[i+1]/4;
			res[i] = a + xn[i]/2 + b;
		}
		return res;
	}

	//Тест случайное построение горных ландшафтов без матиматики
	public int[] RndMount(double[] yn, int cut)
	{
		List<int> res = new List<int>();
		for (int x = 0; x < yn.Length-1; x++)
		{
			int a = (int)yn[x];
			int b = (int)yn[x+1];
			res.Add(a);
			if(a==b || Mathf.Abs(a-b) <= cut)
				continue;

			if(a < b)
			{
				int c = a;
				do
				{
					c = c + rnd.Next(1,cut+1);
					res.Add(c);
				}
				while(c<b);
			}
			
			if(b < a)
			{
				int c = a;
				do
				{
					c = c - rnd.Next(1,cut+1);
					res.Add(c);
				}
				while(c>b);
			}
		}

		return res.ToArray();
	}

	/// <summary>
	/// Линейная интерполяция массива
	/// </summary>
	/// <param name="xn">Массив</param>
	/// <returns>Возвращает линейно интерполированный массив вершин</returns>
	public float[] LineInterpolate(float[] yn, int steps)
	{
		List<float> res = new List<float>();
		for (int x = 0; x < yn.Length - 1; x++)
		{
			Vector2 v2a = new Vector2(x,yn[x]);
			Vector2 v2b = new Vector2(x+1,yn[x+1]);
			res.Add(yn[x]);
			float step = 1/(float)steps;
			for(float s = 0; s<1; s=s+step)
			{
				Vector2 v2c = Vector2.Lerp(v2a, v2b, s);
				res.Add(v2c.y);
			}
		}

		return res.ToArray();
	}

	/// <summary>
	/// Генерирует массив значений 0..1
	/// </summary>
	/// <param name="lenX">Длина массива плоскости по X</param>
	/// <param name="lenZ">Длина массива плоскости по Z</param>
	/// <returns>Возвращает массив типа double</returns>
	private float[,] Noise2D(int lenX, int lenZ)
	{
		int scale = 5;
		float[,] res = new float[lenX, lenZ];
		for(int x=0;x<res.GetLength(0);x=x+scale)
		{
			for(int z=0;z<res.GetLength(1);z=z+scale)
			{
				res[x,z] = (float)rnd.NextDouble();
			}
		}

		return res;
	}

	/// <summary>
	/// Сглаживание масиива плоского шума
	/// </summary>
	/// <param name="y2D">Массив шума 0..1</param>
	/// <returns>Возвращает сглаженный шум</returns>
	public float[,] SmoothNoise2D(float[,] y2D)
	{
		float[,] res = new float[y2D.GetLength(0), y2D.GetLength(1)];
		for(int x=0;x<y2D.GetLength(0);x++)
		{
			int xx = x;
			if(x == 0) xx = x+1;
			if(x == y2D.GetLength(0)-1) xx = x-1;
			for(int z=0;z<y2D.GetLength(1);z++)
			{
				int zz = z;
				if(z == 0) zz = z+1;
				if(z == y2D.GetLength(1)-1) zz = z-1;

				float corners	= ( y2D[xx-1,zz-1]+y2D[xx+1,zz-1]+y2D[xx-1,zz+1]+y2D[xx+1,zz+1] ) / 16;
				float sides	= ( y2D[xx-1,zz]  +y2D[xx+1,zz]  +y2D[xx,zz-1]  +y2D[xx,zz+1]) / 8;
				float center	= y2D[xx,zz] / 4;
				res[x,z] = corners + sides + center;
			}
		}
		return res;
	}

	/// <summary>
	/// Линейная интерполяция массива плоскости
	/// </summary>
	/// <param name="y2D">Массив высот плоскости</param>
	/// <returns>Возвращает линейно интерполированный массив высот плоскости</returns>
	public Vector3[] LineInterpolate2D(float[,] y2D, int steps)
	{
		List<Vector3> res = new List<Vector3>();
		for(int x=1;x<y2D.GetLength(0)-1;x++)
		{
			for(int z=1;z<y2D.GetLength(1)-1;z++)
			{
				//C  D
				//A  B

				Vector3 v3a = new Vector3(x,   y2D[x,   z],   z);
				Vector3 v3b = new Vector3(x+1, y2D[x+1, z],   z);
				Vector3 v3c = new Vector3(x,   y2D[x,   z+1], z+1);
				Vector3 v3d = new Vector3(x+1, y2D[x+1, z+1], z+1);

				res.Add(v3a);
				float[] y2Dab = LineInterpolate(new float[] {v3a.y, v3b.y} , steps);
				float[] y2Dcd = LineInterpolate(new float[] {v3c.y, v3d.y} , steps);
				for(int n=0; n<y2Dab.Length;n++)
				{
					res.Add(new Vector3(n, y2Dab[n], z));
					//float[] rowMatrix = LineInterpolate(new float[] {y2Dab[n], y2Dcd[n]} , steps);
					//for(int i=0;i<rowMatrix.Length;i++)
					//{
					//	res.Add(new Vector3(x, rowMatrix[i], n));
					//}
				}
			}
		}
		return res.ToArray();
	}

        public float power = 30.0f;
        public float scale = 10.0f;
        private Vector2 startPoint = new Vector2(0f, 0f);

	void MakeNoiseInMesh()
	{
		MeshFilter mf = GetComponent<MeshFilter>(); // Ищем mesh
		Vector3[] vertices = mf.mesh.vertices; // Получаем его вершины
		for (int i = 0; i < vertices.Length; i++)
		{
			float x = startPoint.x + vertices[i].x * scale; // X координата вершины
			float z = startPoint.y + vertices[i].z * scale; // Z координата вершины
			vertices[i].y = (Mathf.PerlinNoise(x, z) - 0.5f) * power;  // Задаём высоту для точки с вышеуказанными координатами
		}
		mf.mesh.vertices = vertices; // Присваиваем вершины
		mf.mesh.RecalculateBounds(); // Обновляем вершины
		mf.mesh.RecalculateNormals(); // Обновляем нормали
	}


}