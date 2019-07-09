using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Синглтон, сборщик всех данных
/// </summary>
public class WorldData : MonoBehaviour
{
	public static WorldData Instance;

	/// <summary>
	/// Уничтожение объекта после завершения
	/// </summary>
	void Awake()
	{
		if (Instance == null)
		{
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}

	public Data[,,] DataObj;
	public float[,,] matrix;
	public bool[,,]  exists;

	public WorldData(int max_x, int max_y, int max_z)
	{
		matrix  = new float[max_x, max_y, max_z];
		exists  = new bool[max_x, max_y, max_z];
		DataObj	= new Data[max_x, max_y, max_z];
	}

	public void CreateObject(Vector3 posObject,  GameObject gameObject, Transform parentObject)
	{
		int x = (int)posObject.x;
		int y = (int)posObject.y;
		int z = (int)posObject.z;
		
		if(DataObj[x,y,z] == null)
		{
			DataObj[x,y,z] = new Data();
			DataObj[x,y,z].GO = Instantiate(gameObject, posObject, Quaternion.identity, parentObject);
			DataObj[x,y,z].GO.name = $"Cube {x},{y},{z}";
			DataObj[x,y,z].GO.SetActive(true);
		}
	}

	public void SetActive(int x, int y, int z)
	{
		if(DataObj[x,y,z].GO != null)
		{
			DataObj[x,y,z].GO.SetActive(true);
		}
	}

	public void SetDeactive(int x, int y, int z)
	{
		if(DataObj[x,y,z].GO != null)
		{
			DataObj[x,y,z].GO.SetActive(false);
		}
	}

	/// <summary>
	/// Возвращает высоту максимально высогоко блока
	/// </summary>
	/// <param name="x">Координата плоскости</param>
	/// <param name="z">Координата плоскости</param>
	/// <returns>Высота верхнего блока</returns>
	public int GetMaxHeight(int x, int z)
	{
		int res = 0;
		for(int y=0; y<matrix.GetLength(1);y++)
		{
			if(matrix[x,y,z] != 0)
			{
				res = y;
			}
		}

		return res;
	}
}

/// <summary>
/// Класс хранилище объектов игрового мира
/// </summary>
public class Data : MonoBehaviour
{
	/// <summary>
	/// Пока просто поле тип куба
	/// </summary>
	public GameObject GO { get; set; }

	/// <summary>
	/// Быстрый признак существования объекта в сетке мира
	/// </summary>
	public bool Exists { get; set; } = false;

	/// <summary>
	/// Конструктор
	/// </summary>
	public Data()
	{
		GO = null;
		//Prefab = Instantiate(Resources.Load<>("PREBALPATH"));
		Exists = false;
	}
}

/// <summary>
/// Префабы как правило используемые в мире
/// </summary>
public class WModels
{
	public GameObject Model {get;set;}
}