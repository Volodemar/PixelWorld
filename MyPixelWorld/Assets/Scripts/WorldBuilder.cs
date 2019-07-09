using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
	public GameObject inGameObject;

	public int WorldMaxX = 60;
	public int WorldMaxZ = 60;
	public int WorldMaxY = 60;
	public int DrawDist	 = 10;
	public float RefreshTimer = 3;
	public GameObject Player;

	private WorldData MatrixWorld;
	private Vector3   DrawLastPosition;
	private float	  refreshTimer;

	/// <summary>
	/// Инициализация объектов
	/// </summary>
	private void Awake()
	{
		DrawLastPosition = Player.transform.position;
	}

	/// <summary>
	/// Создание матрицы мира
	/// </summary>
	private void Start()
	{
		//Генерируем мир
		WorldGenerator(WorldMaxX,WorldMaxY,WorldMaxZ);

		//Перемещаем игрока в начальную точку
		int CenterX = WorldMaxX/2;
		int CenterZ = WorldMaxZ/2;
		Vector3 PlayerNewPos = new Vector3(CenterX, MatrixWorld.GetMaxHeight(CenterX, CenterZ)+1, CenterZ);

		//Создаем блоки в поле действия персонажа
		DrawWorld(PlayerNewPos, DrawDist);

			//Переместим игрока в позицию
			Player.transform.position = PlayerNewPos;

			print("Всего ячеек для объектов" + MatrixWorld.matrix.Length);
		}

	/// <summary>
	/// Перестройка мира при изменении положения персонажа
	/// </summary>
	private void Update()
	{
		refreshTimer = refreshTimer - Time.deltaTime;
		DrawWorld(Player.transform.position, DrawDist);
	}

	private void WorldGenerator(int x_max, int y_max, int z_max)
	{
		MatrixWorld = new WorldData(x_max, y_max, z_max); 

		// Заполняем массив простым шумом 0..1
		Perlin2D P2D = new Perlin2D(2);

		// Заполняем данными карту
		for (int x = 0; x < MatrixWorld.matrix.GetLength(0)-1; x++)
		{
			for (int z = 0; z < MatrixWorld.matrix.GetLength(2)-1; z++)
			{
				// Параметры обтекаемости мира
				float fx = (float)x / 250 * 5.0f;
				float fz = (float)z / 250 * 5.0f;

				// Получаем шум Перлина
				float fy = P2D.Noise(fx,fz); // , 3, 0.1f, 0.5f);
				
				// Вариативность высоты
				int y = Mathf.RoundToInt(fy*50) < 0 ? 0 : Mathf.RoundToInt(fy*50);

				// Сохраняем данные в карту
				// 1 значит земля
				MatrixWorld.matrix[x,y,z] = 1.0f;
			}
		}	
	}

	private void DrawWorld(Vector3 PlayerPos, int Dist)
	{
		DrawWorld((int)PlayerPos.x, (int)PlayerPos.y, (int)PlayerPos.z, Dist);
	}

	private void DrawWorld(int CentrX, int CentrY, int CentrZ, int Dist)
	{
		//print("Всего объектов на сцене: " + this.transform.childCount.ToString());

		Vector3 CenterPos = new Vector3(CentrX, CentrY, CentrZ);
		if((int)Vector3.Magnitude(CenterPos - DrawLastPosition) > 1 && refreshTimer < 0)
		{
			DrawLastPosition    = CenterPos;
			refreshTimer		= RefreshTimer;

			//Определяем минимальные и максимальные значения координат объектов
			int x_min = (CentrX - Dist < 0) ? 0 : CentrX - Dist;
			int x_max = (CentrX + Dist > MatrixWorld.matrix.GetLength(0)) ? MatrixWorld.matrix.GetLength(0) : CentrX + Dist;
			int y_min = (CentrY - Dist < 0) ? 0 : CentrY - Dist;
			int y_max = (CentrY + Dist > MatrixWorld.matrix.GetLength(1)) ? MatrixWorld.matrix.GetLength(1) : CentrY + Dist;
			int z_min = (CentrZ - Dist < 0) ? 0 : CentrZ - Dist;
			int z_max = (CentrZ + Dist > MatrixWorld.matrix.GetLength(2)) ? MatrixWorld.matrix.GetLength(2) : CentrZ + Dist;

			//Центр в расчете перерисовки
			Vector3 posCentr  = new Vector3(CentrX, CentrY, CentrZ);

			//Отключаем объекты за зоной активности
			//for(int i=0;i<this.transform.childCount;i++)
			//{
			//	int distOut = (int)Vector3.Magnitude(posCentr - this.transform.GetChild(i).transform.position);
			//	if(distOut > Dist)
			//	{
			//		// Отключаем куб
			//		MatrixWorld.AddDeactiveObject(this.transform.GetChild(i).gameObject);
			//	}
			//}

			//Создаем объекты игрового мира или достаем из отключенных
			for (int x = x_min; x < x_max; x++)
			{
				for (int z = z_min; z < z_max; z++)
				{
					for (int y = y_min; y < y_max; y++)
					{
						// Сравним растояние между объектом и позицией центра
						Vector3 posObject = new Vector3(x,y,z);
						int dist = (int)Vector3.Magnitude(posCentr - posObject);

						if(dist < Dist)
						{
							if(MatrixWorld.matrix[x,y,z] == 1)
							{
								// Создаем куб
								MatrixWorld.CreateObject(posObject, inGameObject, this.transform); 
							}
						}
					}
				}
			}
		}
	}
}




public class WorldBuilderOld : MonoBehaviour
{
	public GameObject inBox;

	/// <summary>
	/// Матрица мира х,у - плоскость, z - высота кубика [x,y,z]
	/// </summary>
	public Box[,,] WorldBoxs = new Box[20,20,20];	

	System.Random rnd = new System.Random();

	/// <summary>
	/// Конструктор мира
	/// </summary>
	//public WorldBuilder()
	//{
	//	WorldGenerate();
	//}


	public double Noise(int x)
	{
		x = (x << 13) ^ x;
		return ( 1.0 - ( (x * (x * x * 15731 + 789221) + 1376312589) /*& 7fffffff*/) / 1073741824.0); 
	}

	public double SmoothNoise(double x)
	{
		return this.Noise((int)x)/2  +  Noise((int)x-1)/4  +  Noise((int)x+1)/4;
	}
 
	// Линейная интерполяция (быстрая)
	public double Linear_Interpolate(double a, double b, double x)
	{
		return  a*(1-x) + b*x;
	}

	// Косинусная интерполяция (понапряжнее)
	public double Cosine_Interpolate(double a, double b, double x)
	{
		double ft = x * 3.1415927;
		double f = (1 - Math.Cos(ft)) * 0.5f;
		return  a*(1-f) + b*f;
	}

	public double InterpolatedNoise(double x)
	{
      int	 intX    = (int)x;
      double fractionalX = x - intX;
      double v1 = SmoothNoise(intX);
      double v2 = SmoothNoise(intX + 1);
      return Linear_Interpolate(v1 , v2 , fractionalX);
	}

	//public double SmoothNoise_2D(int x, int y)    
	//{
	//	double corners = ( Noise(x-1, y-1)+Noise(x+1, y-1)+Noise(x-1, y+1)+Noise(x+1, y+1) ) / 16;
	//	double sides   = ( Noise(x-1, y)  +Noise(x+1, y)  +Noise(x, y-1)  +Noise(x, y+1) ) /  8;
	//	double center  =  Noise(x, y) / 4;
	//	return corners + sides + center;
	//}

	void ViewNoice()
	{
		string sx = "";
		for(int x=0;x<10;x++)
		{
			sx = sx+", "+this.SmoothNoise(x).ToString();			
		}
		Debug.Log(sx);
	}

	/// <summary>
	/// Создание матрицы мира
	/// </summary>
	private void WorldGenerate()
	{
		// Список слоев мира
		Layer[] layers = new Layer[4];

		// Создаем начальный слой 'Дно'
		layers[0] = new Layer();
		layers[0].LayerName = "Дно";
		layers[0].MinHeight = 0;
		layers[0].MaxHeight = 0;
		layers[0].Add(new Box("Дно", Color.clear, 1, 100, inBox));

		// Создаем слой 'Недра'
		layers[1] = new Layer();
		layers[1].LayerName = "Недра";
		layers[1].MinHeight = 1;
		layers[1].MaxHeight = 3;
		layers[1].Add(new Box("Камень", Color.gray,  50, 90, inBox));
		layers[1].Add(new Box("Лава",	Color.red,	 10, 10, inBox));
		layers[1].Add(new Box("Алмаз",	Color.blue,	 0,  1, inBox));
		layers[1].Add(new Box("Пусто",	Color.white, 5,  1, inBox));

		// Создаем слой 'Средний'
		layers[2] = new Layer();
		layers[2].LayerName = "Средний";
		layers[2].MinHeight = 4;
		layers[2].MaxHeight = 10;
		layers[2].Add(new Box("Камень", Color.gray,  20, 80, inBox));
		layers[2].Add(new Box("Лава",	Color.red,	 5,  1, inBox));
		layers[2].Add(new Box("Земля",	Color.green, 0,  5, inBox));
		layers[2].Add(new Box("Уголь",	Color.black, 5,  10, inBox));
		layers[2].Add(new Box("Пусто",	Color.white, 10, 10, inBox));

		// Создаем слой 'Земля'
		layers[3] = new Layer();
		layers[3].LayerName = "Земля";
		layers[3].MinHeight = 10;
		layers[3].MaxHeight = 15;
		layers[3].Add(new Box("Земля",	Color.green, 20, 90, inBox));
		layers[3].Add(new Box("Камень", Color.gray,  10, 10, inBox));
		layers[3].Add(new Box("Уголь",	Color.black, 1,  1, inBox));
		layers[3].Add(new Box("Пусто",	Color.white, 10, 10, inBox));

		// Заполняем матрицу мира
		for (int x=0;x<WorldBoxs.GetLength(0);x++)
		{
			for(int z=0;z<WorldBoxs.GetLength(1);z++)
			{
				for(int y=0;y<WorldBoxs.GetLength(2);y++)
				{
					// Пустота изначально
					WorldBoxs[x,y,z] = new Box("Пусто", Color.white, 1, 1, null);

					foreach (Layer layer in layers)
					{
						// Выбираем нужный слой по высоте мира
						if (layer.MinHeight <= y && layer.MaxHeight >= y)
						{
							// Перебираем кубы слоя c сортировкой по возростанию по редкости выпадения
							foreach (Box box in layer.GetBoxes().OrderBy(ind => ind.Percent).ToArray<Box>())
							{
								// Проверяем какие кубики находятся рядом, если рядом есть кубики то к вероятности добавляется магнетизм
								int CountBoxIdentity = 1;
								if(x > 0 && y > 0 && z > 0 && x < WorldBoxs.GetLength(0)-1 && y < WorldBoxs.GetLength(2)-1 && z < WorldBoxs.GetLength(1)-1)
								{
									if(WorldBoxs[x, y-1, z] != null && WorldBoxs[x, y-1, z].Name == box.Name)
										CountBoxIdentity++;
									if(WorldBoxs[x-1, y-1, z] != null && WorldBoxs[x, y-1, z].Name == box.Name)
										CountBoxIdentity++;
									if(WorldBoxs[x-1, y-1, z-1] != null && WorldBoxs[x, y-1, z].Name == box.Name)
										CountBoxIdentity++;
									if(WorldBoxs[x+1, y-1, z-1] != null && WorldBoxs[x, y-1, z].Name == box.Name)
										CountBoxIdentity++;
									if(WorldBoxs[x-1, y-1, z+1] != null && WorldBoxs[x, y-1, z].Name == box.Name)
										CountBoxIdentity++;
								}

								// Проверяем если случайность попала в промежуток от 0 до box.Percent то вставляем Box
								if (rnd.Next(0,101) <= box.Percent + box.Magnetic * CountBoxIdentity)
								{
									WorldBoxs[x, y, z] = box;
									break;
								}
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Отображаем мир на экране
	/// </summary>
	private void WorldDraw()
	{
		//Отображение мира
		for (int x = 0; x < WorldBoxs.GetLength(0); x++)
		{
			for (int y = 0; y < WorldBoxs.GetLength(1); y++)
			{
				for (int z = 0; z < WorldBoxs.GetLength(2); z++)
				{
					if (WorldBoxs[x, y, z].Name != "Пусто")
					{
						if(WorldBoxs[x, y, z].GameObject != null)
						{
							GameObject newCube = Instantiate(WorldBoxs[x, y, z].GameObject, new Vector3(x, y, z), Quaternion.identity, this.transform);
						}
						else
						{
							GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
							Renderer RenderCube = Cube.GetComponent<Renderer>();
							Cube.transform.position = new Vector3(x, y, z); // * 1.1f; - растояние между кубиками
							RenderCube.material.color = WorldBoxs[x, y, z].Color;
						}
					}
				}
			}
		}
	}

    /// <summary>
	/// Старт игры
	/// </summary>
    void Start()
    {
		ViewNoice();

		// Генерируем новый мир
		//WorldGenerate();

		// Отображаем сгенерированный мир
		//WorldDraw();
	}
}

/// <summary>
/// Структура слои, содержит данные по уровню слоя и кубы в этом слое
/// </summary>

public class Layer
{
	/// <summary>
	/// Наименование слоя
	/// </summary>
	public string LayerName {get;set;}

	/// <summary>
	/// Минимальная высота в мировом пространстве кубов 
	/// </summary>
	public int MinHeight {get;set;}

	/// <summary>
	/// Максимальная высота в мировом пространстве кубов
	/// </summary>
	public int MaxHeight {get;set;}

	/// <summary>
	/// Кубы в слое
	/// </summary>
	public Box[] boxes = new Box[0];

	/// <summary>
	/// Индексатор для доступа к процедурам записи
	/// </summary>
	/// <param name="index">Индекс записи</param>
	//public Box this[int index]
	//{
	//	get {return boxes[index];}
	//	set {boxes[index] = value;}
	//}

	/// <summary>
	/// Счетчик следующий номер куба
	/// </summary>
	private int NextIndex { get; set; }

	/// <summary>
	/// Количество кубов в слое.
	/// </summary>
	public int Count
	{
		get
		{
			if (boxes == null)
				return 0;
			else
				return boxes.Length;
		}
		private set { this.Count = value; }
	}

	/// <summary>
	/// Конструктуор создание слоя
	/// </summary>
	public Layer()
	{
		this.boxes		= new Box[0];
		this.NextIndex	= 0;
	}

	/// <summary>
	/// Возвращает все кубы слоя
	/// </summary>
	/// <returns></returns>
	public Box[] GetBoxes()
	{
		return boxes;
	}

	/// <summary>
	/// Добавление куба в слой
	/// </summary>
	/// <param name="newRecord">запись</param>
	public void Add(Box newBox)
	{
		//Проверка, что такого контрагента нет в репозитории
		foreach (Box box in this.boxes)
		{
			if (box.Name == newBox.Name)
				return;
		}

		if (this.boxes.Length <= this.NextIndex)
			Array.Resize(ref this.boxes, this.boxes.Length + 1);
		this.boxes[this.NextIndex] = newBox;
		this.NextIndex++;
	}
}

/// <summary>
/// Структура кубы
/// </summary>
[Serializable]

public class Box
{
	/// <summary>
	/// Наименование куба
	/// </summary>
	public string Name {get;set;}

	/// <summary>
	/// Цвет куба
	/// </summary>
	public Color Color {get;set;}

	/// <summary>
	/// Примагничивание одинаковых
	/// </summary>
	public int Magnetic {get;set;}

	/// <summary>
	/// Процент 1..100 частота нахождения
	/// </summary>
	public int Percent {get;set;}

	/// <summary>
	/// Префаб куба
	/// </summary>
	public GameObject GameObject {get;set;}

	/// <summary>
	/// Создание куба
	/// </summary>
	/// <param name="Name">Название куба</param>
	/// <param name="Color">Цвет куба</param>
	/// <param name="Magnetic">Примагничивание одинаковых</param>
	/// <param name="Percent">Процент 1..100 частота нахождения</param>
	/// <param name="GameObject">Префаб куба</param>
	public Box(string Name, Color Color, int Magnetic, int Percent, GameObject GameObject)
	{
		this.Name		= Name;
		this.Color		= Color;
		this.Magnetic	= Magnetic;
		this.Percent	= Percent;
		this.GameObject = GameObject;
	}
}
