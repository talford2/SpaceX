using UnityEngine;

namespace Effects
{
	public class SelfDestructor : MonoBehaviour
	{
		public float Cooldown;

		public bool StartOn;

		public void StartCooldown()
		{
			StartOn = true;
		}

		private void Update()
		{
			if (StartOn)
			{
				if (Cooldown >= 0)
				{
					Cooldown -= Time.deltaTime;
				}

				if (Cooldown < 0)
				{
					Destroy(gameObject);
				}
			}
		}
	}
}
