using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
	public float LiveDist = 100.0f;

	private Camera cam;
	private float timer;
	private float dist;

	private void Awake()
	{
		cam = Camera.main;
		timer = 3.0f;
	}

	private void Update()
	{
		timer = timer - Time.fixedDeltaTime;
		if(timer < 0)
		{
			//Проверяем растояние до игрока, удаляем объект если игрок ушел слишном далеко
			dist = Vector3.Magnitude(this.transform.position - cam.transform.position);
			if(dist > LiveDist)
			{
				//Destroy(this.gameObject);
				this.gameObject.SetActive(false);
				//WorldData.Instance.AddDeactiveObject((int)this.transform.position.x, (int)this.transform.position.y, (int)this.transform.position.z);
			}

			timer = 3.0f;
		}
	}
}
