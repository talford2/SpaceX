using UnityEngine;

namespace Effects
{
	public class SelfDestructor : MonoBehaviour
	{
		public float Cooldown;

		public bool StartOn;

		private bool _isStarted;

		private float _runningCooldown;

		private void Awake()
		{
			_isStarted = StartOn;
			_runningCooldown = Cooldown;
		}

		public void StartCooldown()
		{
			_isStarted = true;
		}

		private void Update()
		{
			if (_isStarted)
			{
				if (_runningCooldown >= 0)
				{
					_runningCooldown -= Time.deltaTime;
				}

				if (_runningCooldown < 0)
				{
					Destroy(gameObject);
				}
			}
		}
	}
}
