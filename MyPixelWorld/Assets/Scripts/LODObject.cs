using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODObject : MonoBehaviour
{
	/// <summary>
	/// Массив моделей
	/// </summary>
	public GameObject[] Models;

	/// <summary>
	/// Массив дистанций
	/// </summary>
	public int[] Dist;

	/// <summary>
	/// Ссылка на объект камера
	/// </summary>
	private GameObject Cam;

	/// <summary>
	/// Номер текущей подгружаемой модели
	/// </summary>
	private int NumModel;

	/// <summary>
	/// Дистанция до камеры
	/// </summary>
	private float CamDist;


    void Start()
    {
		// Изначально отключаем все модели объектов
		for(int i=0;i<Models.Length;i++)
		{
			Models[i].SetActive(false);
		}
		Cam = Camera.allCameras[0].gameObject;
    }

    void Update()
    {
        CamDist = Vector3.Distance(Cam.transform.position, this.transform.position);

		int LVL = -1;
		for(int i=0;i<Dist.Length;i++)
		{
			if(CamDist < Dist[i])
			{
				LVL = i;
				break;
			}
		}

		if(LVL == -1) LVL = Dist.Length-1;
		if(NumModel != LVL) SetLOD(LVL);
    }

	/// <summary>
	/// Установка уровня детализации модели
	/// </summary>
	/// <param name="LVL"></param>
	void SetLOD(int LVL)
	{
		Models[LVL].SetActive(true);
		Models[NumModel].SetActive(false);
		NumModel = LVL;
	}
}
