using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Управление персонажем
/// </summary>
public class PlayerControl : MonoBehaviour
{
	/// <summary>
	/// Скорость пермещения
	/// </summary>
	public float Speed = 0.3f;

	/// <summary>
	/// Действующая гравитация
	/// </summary>
	public float Gravity = 10f;

	/// <summary>
	/// Сила прыжка
	/// </summary>
	public float JumpForce = 3f;

	/// <summary>
	/// Чувствительность мыши
	/// </summary>
	public float sensitivity = 5f;

	/// <summary>
	/// Максимальный угол поворота головы вверх
	/// </summary>
	public float maxRotationY = 60.0f;

	/// <summary>
	/// Максимальный угол поворота головы вниз
	/// </summary>
	public float minRotationY = -60.0f;

	private float rotationX;
	private float rotationY;
	private float jSpeed = 0;
	private CharacterController controller;
	private float h;
	private float v;
	private Vector3 direction;
	private Transform head;

	private void Awake()
	{
		// На объекте персонаж, должен быть компонент CharacterController
		controller	= this.GetComponent<CharacterController>();

		// Объект персонажа голова, следует за перемещением мышки
		head		= this.transform;
	}

	private void Update()
	{
		// Считываем оси ввода данных
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");

		// Если персонаж на земле сбрасываем скорость прыжка в 0 и можем выполнить новый прыжок 
		if(controller.isGrounded)
		{
			jSpeed = 0;
			if(Input.GetButtonDown("Jump"))
			{
				jSpeed = JumpForce;
			}
		}

		// Если был выполнен прыжок то постоянная гравитация будет поглощать скорость перемещения вверх
		jSpeed = jSpeed - Gravity * Time.fixedDeltaTime;

		// Поворот головы (камеры если скрипт на камере) за движением мышкой
		rotationX = head.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
		rotationY += Input.GetAxis("Mouse Y") * sensitivity;
		rotationY = Mathf.Clamp (rotationY, minRotationY, maxRotationY);
		head.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

		// Движение в направлении взгляда
		Vector3 direction = new Vector3(h * Speed, 0, v * Speed);						//Смещение в локальных координатах
		direction = head.TransformDirection(direction);									//Трансформация смещения в глобальные координаты
		direction = new Vector3(direction.x, jSpeed * Time.fixedDeltaTime, direction.z);		//Добавление гравитации в глобальных координатах

		// Плавное перемещение персонажа в новую позицию
		controller.Move(direction);
	}
}
