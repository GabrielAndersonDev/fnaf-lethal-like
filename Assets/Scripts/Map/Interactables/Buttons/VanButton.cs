using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum VanDestination
{
    Invalid = -2,
    None = -1,
    First,
    Van,
    Game,
    Shop,
    Max
}

public class VanButton : MapButton
{
    [SerializeField]
    TextMeshPro btnText;
    [SerializeField]
    EntranceSegment entranceSegment;

    VanDestination destination;

    bool isSelectable;

    private void Start()
    {
        // UI menu for button will be adapted to allow for choosing of shopping vs game start?
        VanBtnSceneCheck();
    }

    void VanBtnSceneCheck()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "VanScene":
                if (GameManager.Singleton.day.Value == 0)
                {
                    btnText.text = "Start Game";
                    destination = VanDestination.Game;
                    isSelectable = false;
                }
                else if (GameManager.Singleton.day.Value > 0)
                {
                    // change this to allow for swapping dest
                    btnText.text = "Next Day";
                    destination = VanDestination.Game;
                    isSelectable = true;
                }
                else
                {
                    Debug.LogError("Invalid day integer.");
                    Debug.Assert(false);
                }
                break;
            case "GameScene":
                btnText.text = "Return";
                destination = VanDestination.Van;
                isSelectable = false;
                break;
            case "ShoppingScene":
                btnText.text = "Return";
                destination = VanDestination.Van;
                isSelectable = false;
                break;
            default:
                Debug.Log("No scenes applicable, " + SceneManager.GetActiveScene().name);
                break;
        }
    }

    public override void ButtonInteract()
    {
        VanBtnSceneCheck();

        

        if (isSelectable)
        {
            Debug.LogWarning("Selectable is unimplimented, will allow for players to choose between going to shop and going to next destination");
            entranceSegment.PlayerItemCollisionCheck();
            NetworkScript.Singleton.RequestDestinationServerRpc(destination); 
        }
        else
        {
            Debug.Log("Will not be instant in the future, open up seperate UI menu where you can select?");
            entranceSegment.PlayerItemCollisionCheck();
            NetworkScript.Singleton.RequestDestinationServerRpc(destination);
        }
    }
}
