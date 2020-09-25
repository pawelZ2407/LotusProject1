using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Construction : MonoBehaviour
{
    [SerializeField]
    public Transform BuildOptions;
    Transform RestPos;
    
    bool ShowingOptions;

    public int BlockSelectedResourceId;
    public int BlockSelected;

    public float buildCooldown;

    public LayerMask whatStopsMovement;
    InventorySystem Inventory;
    NetworkManager NetManager;
    // Start is called before the first frame update
    void Start()
    {
        NetManager = GameObject.Find("NetworkManager").transform.GetComponent<NetworkManager>();
        buildCooldown = 0;
        BlockSelectedResourceId = -1;
        BlockSelected = -1;
        ShowingOptions = false;
        Inventory = GameObject.Find("Player(Clone)").transform.GetComponent<InventorySystem>();
        RestPos = GameObject.Find("InventoryRest").transform;
        BuildOptions = transform.Find("BuildOptions").transform;
    }

    // Update is called once per frame
    void Update()
    {
        buildCooldown -= Time.deltaTime;
        ManageTouchs();
    }
    
    public void InventoryButton()
    {
        if (ShowingOptions)
        {
            BuildOptions.position = RestPos.position;
            ShowingOptions = false;
        }
        else if(!ShowingOptions)
        {
            BuildOptions.position = transform.position;
            ShowingOptions = true;
        }
    }

    public void SelectBlock(int blockResourceId)
    {
        if (BlockSelected != blockResourceId)
        {
            BlockSelected = blockResourceId;
            BlockSelectedResourceId = GetResourceId(BlockSelected);
        }
        else
        {
            BlockSelectedResourceId = -1;
            BlockSelected = -1;
        }
    }

    void ManageTouchs()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).position.x < Screen.width / 4 && Input.GetTouch(i).position.y < Screen.height / 2 || Input.GetTouch(i).position.x > 7 * (Screen.width / 8))
            {
            }
            else
            {
                if (ShowingOptions)
                {
                    Vector2 target = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                    Build(target);
                }
            }
        }
    }

    void Build(Vector2 target)
    {
        print("X" + target.x + "Y" + target.y);
        if (target.x > 0 && target.x - Mathf.Floor(target.x) > 0)
        {
            target.x = Mathf.Floor(target.x) + .5f;
        }
        if (target.y > 0 && target.y - Mathf.Floor(target.y) > 0)
        {
            target.y = Mathf.Floor(target.y) + .5f;
        }
        if (target.x < 0 && target.x + Mathf.Ceil(target.x) < 0)
        {
            target.x = Mathf.Ceil(target.x) - .5f;
        }
        if (target.y < 0 && target.y + Mathf.Ceil(target.y) < 0)
        {
            target.y = Mathf.Ceil(target.y) - .5f;
        }
        print("X" + target.x + "Y" + target.y);
        if (buildCooldown <= 0)
        {
            if (BlockSelectedResourceId == 0)
            {
                if (Inventory.Items[BlockSelectedResourceId] > 0)
                {
                    if (!Physics2D.OverlapCircle(target, 0.4f, whatStopsMovement))
                    {
                        Inventory.takeItem(BlockSelectedResourceId, 1);
                        NetManager.SendPacket("build"+BlockSelected+"|"+ target.x +"a"+ target.y);
                        GameObject block = Instantiate(NetManager.Items[BlockSelected], target, Quaternion.identity);
                        buildCooldown = 1f;
                    }
                }

            }
        }    
    }

    int GetResourceId(int Block)
    {
        if (Block == 2)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }
}
