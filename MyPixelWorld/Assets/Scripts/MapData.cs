using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData {

	/// <summary>
	/// Высота ландшафта в координатной точке
	/// </summary>
	public float[,] Height;

	/// <summary>
	/// Может пригодиться для оптимизации
	/// </summary>
	public float Min { get; set; }
	public float Max { get; set; }

	/// <summary>
	/// Хранилище объектов для быстрого доступа по ссылкам
	/// </summary>
	private DataObj[,,] Data;

	/// <summary>
	/// Инициализация
	/// </summary>
	/// <param name="width"> Ширина X</param>
	/// <param name="length">Длмина Z</param>
	/// <param name="height">Высота Y</param>
	public MapData(int width, int length, int height)
	{
		Height = new float[width, length];
		Min = float.MaxValue;
		Max = float.MinValue;
		Data = new DataObj[width, height, length];
	}

	/// <summary>
	/// Добавление объекта в данные
	/// </summary>
	/// <param name="x">Координата объекта</param>
	/// <param name="y">Координата объекта</param>
	/// <param name="z">Координата объекта</param>
	/// <param name="obj">Ссылка на объект</param>
	public void SetData(int x, int y, int z, ref GameObject obj)
	{
		DataObj data = new DataObj();
		data.obj	= obj;
		data.Exists = true;
		Data[x,y,z] = data;
	}

	/// <summary>
	/// Удаляем объект из данных для экономии памяти
	/// </summary>
	/// <param name="x">Координата объекта</param>
	/// <param name="y">Координата объекта</param>
	/// <param name="z">Координата объекта</param>
	public void DelData(int x, int y, int z)
	{
		DataObj data = new DataObj();
		data.obj	= null;
		data.Exists = false;
		Data[x,y,z] = data;
	}

	/// <summary>
	/// Извлечение объекта из данных
	/// </summary>
	/// <param name="x">Координата объекта</param>
	/// <param name="y">Координата объекта</param>
	/// <param name="z">Координата объекта</param>
	/// <returns>Вернет null если объекта нету.</returns>
	public GameObject GetData(int x, int y, int z)
	{
		if(Data[x,y,z].Exists)
			return Data[x,y,z].obj;
		else
			return null;
	}

	/// <summary>
	/// Возвращаетп ризнак существует ли объект в данной точке
	/// </summary>
	/// <param name="x">Координата объекта</param>
	/// <param name="y">Координата объекта</param>
	/// <param name="z">Координата объекта</param>
	/// <returns>Вернет false если объека нету.</returns>
	public bool Exists(int x, int y, int z)
	{
		if(Data[x,y,z] != null)
			return Data[x,y,z].Exists;
		else
			return false;
	}
}

/// <summary>
/// Класс хранилище объектов игрового мира
/// </summary>
public class DataObj
{
	/// <summary>
	/// Объект игрового мира
	/// </summary>
	public GameObject obj	{ get; set; }

	/// <summary>
	/// Быстрый признак существования объекта в сетке мира
	/// </summary>
	public bool Exists		{ get; set; } = false;

	/// <summary>
	/// Конструктор
	/// </summary>
	public DataObj()
	{
		obj		= null;
		Exists	= false;
	}
}
