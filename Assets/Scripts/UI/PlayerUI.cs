using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image PassiveCooldown { get; private set; }
    public Image Ability1Cooldown{ get; private set; }
    public Image Ability2Cooldown{ get; private set; }
    public Image Ability3Cooldown { get; private set; }
    [SerializeField] private Image[] abilityImages;


    public void Init()
    {
        //If images aren't assign in inspector, assign them
        if (abilityImages.Length == 4)
        {
            return;
        }

        Image[] playerAbilityImages = GameObject.Find("Canvas/PlayerAbilites").GetComponentsInChildren<Image>().Skip(1).ToArray();

        PassiveCooldown = playerAbilityImages[0];
        Ability1Cooldown = playerAbilityImages[1];
        Ability2Cooldown = playerAbilityImages[2];
        Ability3Cooldown = playerAbilityImages[3];
        abilityImages = playerAbilityImages;
    }



    /// <summary>
    /// Returns an array of initialized images
    /// </summary>
    /// <returns>4 images</returns>
    public Image[] AbilityImages()
    {
        return abilityImages;
    }
  
}
