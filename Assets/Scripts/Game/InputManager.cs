using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Game
{
	public class InputManager: MonoBehaviour
	{
		public static InputManager Singleton { get; internal set; }

		public PlayerInput PlayerInput;
		public EventSystem EventSystem;

        private void Awake()
        {
            if (Singleton != null)
            {
				Destroy(gameObject);
            }
			else
			{
				Singleton = this;
				DontDestroyOnLoad(gameObject);
            }
        }

		public void AssignPlayerInfo(Player player, Camera cam)
		{
			if (cam == null)
			{
				Debug.LogError("Attach Player Cam failed: cam is null.");
				Debug.Break();
				return;
            }

			PlayerInput.camera = cam;
			player.playerInput = PlayerInput;
        }

		public void SetActionMap(string mapName)
		{
			switch (mapName)
			{
				case "Player":
					if (PlayerInput.camera == null)
					{
                        Debug.LogError("Switching to Player action map failed: PlayerInput camera is null.");
                        Debug.Break();
						return;
                    }
					break;
				case "UI":
					break;
				default:
					Debug.LogError("SetActionMap failed: invalid mapName " + mapName);
					Debug.Break();
					return;
            }

			PlayerInput.SwitchCurrentActionMap(mapName);
        }

		public void SetFirstBtnSelected(GameObject obj)
		{
			if (obj == null)
			{
				Debug.LogError("SetFirstBtnSelected: gameobject provided is null.");
				Debug.Break();
				return;
			}

			EventSystem.firstSelectedGameObject = obj;
		}

		public void UnassignPlayerInfo(Player player)
		{
			player.playerInput = null;
			PlayerInput.camera = null;
        }

        public void ToggleInput(bool isEnabled)
		{
			if (PlayerInput == null)
			{
				Debug.LogError("ToggleInput failed: PlayerInput is null.");
				Debug.Break();
				return;
			}
			if (isEnabled)
			{
				PlayerInput.ActivateInput();
			}
			else
			{
				PlayerInput.DeactivateInput();
			}
        }
    }
}