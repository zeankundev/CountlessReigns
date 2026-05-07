using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ItemType
{
    Buffs, // They only change the player stats (e.g. weapons, health, etc).
    Usables // Can be used in combat (e.g. poison syringes, etc)
}
[System.Serializable]
public class ChestContent
{
    public ItemType type;
    public string itemName;
    public float quantity;
    public Sprite itemImage;
    [Header("Buffs only")]
    public float healthRestore;
    public float damageIncrease;
    public float deltaDamageMultiplier;
    [Header("Usables only")]
    public string intenalCodeForUsables;
}

public class ChestController : MonoBehaviour
{
    private PlayerController player;
    private GameObject playerObject;
    [SerializeField]
    public List<ChestContent> chestContents = new List<ChestContent>();
    public float activationRadius = 1.5f;
    public Sprite openedChestSprite;
    private bool isOpened = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerObject.transform.position);
        if (distanceToPlayer <= activationRadius && !isOpened)
        {
            transform.Find("KeyPrompt").gameObject.SetActive(true);
            if (Keyboard.current.eKey.wasPressedThisFrame && !isOpened)
            {
                for (int i = 0; i < chestContents.Count; i++)
                {
                    if (chestContents[i].type == ItemType.Buffs)
                    {
                        player.GrantBuff(chestContents[i].healthRestore, chestContents[i].damageIncrease, chestContents[i].deltaDamageMultiplier, chestContents[i].quantity);
                    }
                    else if (chestContents[i].type == ItemType.Usables)
                    {
                        player.AddToInventory(chestContents[i].intenalCodeForUsables, chestContents[i].quantity);
                        // Handle usables based on internal code or other properties
                        Debug.Log($"Usable item obtained: {chestContents[i].itemName}");
                    }
                    GameObject.Find("Canvas").GetComponent<UIBridge>().LerpFromLeftToOriginalVectorItemShowing(chestContents[i].itemImage, chestContents[i].itemName + " " + chestContents[i].quantity + "x");
                }
                isOpened = true;
                gameObject.GetComponent<SpriteRenderer>().sprite = openedChestSprite;
                transform.Find("KeyPrompt").gameObject.SetActive(false);
            }
        }
        else
        {
            transform.Find("KeyPrompt").gameObject.SetActive(false);
        }
    }
}
