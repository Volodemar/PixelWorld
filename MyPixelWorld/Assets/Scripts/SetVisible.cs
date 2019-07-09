using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVisible : MonoBehaviour
{
	public float ViewDistance = 50.0f;

	private Camera cam;
	private Renderer render;

	/// <summary>
	/// Инициализация объектов
	/// </summary>
	private void Awake()
	{
		render  = this.gameObject.GetComponent<Renderer>();
		cam		= Camera.main;
	}

	/// <summary>
	/// Объект невидимый для камеры
	/// </summary>
	//private void OnBecameInvisible()
	//{
	//	render.enabled = false;
	//}

	/// <summary>
	/// Объект видимый для камеры (не пашет после отключения видимости)
	/// </summary>
	//private void OnBecameVisible()
	//{
	//	render.enabled = true;
	//}

	//void Start()
	//{
	//}

	// Update is called once per frame
	//void Update()
	//{
	//	//cam = Camera.main;
	//	//for (int i = 0; i < this.gameObject.transform.childCount; i++)
	//	//{
	//	//	Transform CubeTransform = this.gameObject.transform.GetChild(i);
	//	//	float CamDist = Vector3.Distance(cam.transform.position, CubeTransform.position);
	//	//	if (CamDist < ViewDistance)
	//	//		CubeTransform.gameObject.SetActive(true);
	//	//	else
	//	//		CubeTransform.gameObject.SetActive(false);
	//	//}
	//}
}
