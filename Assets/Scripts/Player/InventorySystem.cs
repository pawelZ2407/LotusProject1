using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InventorySystem : MonoBehaviour
{
    public List<int> Items;
    public List<Transform> ItemHolders;
    public List<Sprite> Sprites = new List<Sprite>();

    public bool ShowingInv;
    public bool IsLoaded = false;

    public GameObject BasePickUp;
    public GameObject ItemDestroyer;
    public NetworkManager NetManager;

    private void Start()
    {
        NetManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        if (!NetManager.Mode)
        {
            for (int i = 0; i < GameObject.Find("InventoryUI").transform.Find("Items").childCount; i++)
            {
                ItemHolders.Add(GameObject.Find("InventoryUI").transform.Find("Items").GetChild(i));
            }

            ShowingInv = false;
            IsLoaded = true;
        }
    }

    //Assign Variables--------------
    void QuickBegin()
    {
        for (int i = 0; i < GameObject.Find("InventoryUI").transform.Find("Items").childCount; i++)
        {
            ItemHolders.Add(GameObject.Find("InventoryUI").transform.Find("Items").GetChild(i));
        }

        ShowingInv = false;
        IsLoaded = true;
    }

    //Did someone take the LOOT!?---
    public void AnalyzeMsg(string temp)
    {
        string[] withoutName = temp.Split(':');
        if (temp.Contains("pick")) 
        {
            List<float> xy = new List<float>();
            xy.Add(float.Parse((withoutName[1].Split('w')[1]).Split('a')[0]));
            xy.Add(float.Parse((withoutName[1].Split('w')[1]).Split('a')[1]));
            GameObject destroyer = (GameObject)Instantiate(ItemDestroyer, new Vector2(xy[0],xy[1]), Quaternion.identity);
        }
        if (temp.Contains("drop"))
        {
            Transform Dropper = GameObject.Find(withoutName[0]).transform;
            List<float> xy = new List<float>();
            GameObject OnlineDrop = (GameObject)Instantiate(BasePickUp, new Vector2(Dropper.position.x + 1, Dropper.position.y), Quaternion.identity);
            OnlineDrop.GetComponent<SpriteRenderer>().sprite = GameObject.Find("temp.Split('p')[1].Split('h')[0]").transform.GetComponent<SpriteRenderer>().sprite;
            OnlineDrop.transform.name = temp.Split('p')[1].Split('h')[0];
            OnlineDrop.transform.GetChild(0).name = (withoutName[1].Split('p')[1]).Split('h')[1];
        }
    }

    //Buttons-----------------------
    public void InvButton()
    {
        if (!ShowingInv)
        {
            GameObject.Find("InventoryUI").transform.position = GameObject.Find("Inventory").transform.position;
            ShowingInv = true;
        }
        else
        {
            GameObject.Find("InventoryUI").transform.position = GameObject.Find("InventoryRest").transform.position;
            ShowingInv = false;
        }
    }
    public void DropItem(int Slot)
    {
        //Feeling charitative?------
        NetManager.SendPacket("drop" + ItemHolders[Slot].GetChild(2).name + "h" + Items[int.Parse(ItemHolders[Slot].GetChild(2).name)].ToString() + "|" + Math.Round((double)transform.position.x + 1, 1).ToString() + "a" + Math.Round((double)transform.position.y, 1).ToString());

        //Actually Drop It----------
        GameObject Dropped = (GameObject)Instantiate(BasePickUp, new Vector2(transform.position.x + 1, transform.position.y - 0.5f), Quaternion.identity);
        Dropped.transform.name = ItemHolders[Slot].GetChild(2).name;
        Dropped.GetComponent<SpriteRenderer>().sprite = ItemHolders[Slot].GetChild(1).GetComponent<Image>().sprite;
        Dropped.transform.GetChild(0).name = Items[int.Parse(ItemHolders[Slot].GetChild(2).name)].ToString();

        //Remove From Inventory-----
        ItemHolders[Slot].GetChild(0).GetComponent<Text>().text = (0).ToString();
        ItemHolders[Slot].GetChild(1).GetComponent<Image>().sprite = null;
        Items[int.Parse(ItemHolders[Slot].GetChild(2).name)] = 0;
        ItemHolders[Slot].GetChild(2).name = (-1).ToString();
    }
    public void StoreItem(int id, Sprite Psprite)
    {
 //       for (int i = 0; i < Items.Count; i++)
//        {
            if (Items[id] > 0)
            {
                for (int y = 0; y < ItemHolders.Count; y++)
                {
                    if (ItemHolders[y].GetChild(2).name == id.ToString())
                    {
                        ItemHolders[y].GetChild(0).GetComponent<Text>().text = Items[id].ToString();
                        break;
                    }
                    if (y == 7)
                    {
                        for (int o = 0; o < ItemHolders.Count; o++)
                        {
                            if (ItemHolders[o].GetChild(0).GetComponent<Text>().text == "0")
                            {
                                ItemHolders[o].GetChild(0).GetComponent<Text>().text = Items[id].ToString();
                                ItemHolders[o].GetChild(1).GetComponent<Image>().sprite = Psprite;
                                ItemHolders[o].GetChild(2).name = id.ToString();
                                break;
                            }
                        }
                    }

                }
 //           }
        }
    }

    //Pickup Item------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "PickUp")
        {
            //Pick up
            Items[int.Parse(collision.transform.name)] += int.Parse(collision.transform.GetChild(0).name);
            StoreItem(int.Parse(collision.transform.name), collision.transform.GetComponent<SpriteRenderer>().sprite);

            //Show your greed to the rest
            NetManager.SendPacket("pick" + collision.transform.name + "w" + Math.Round((double)collision.transform.position.x, 1).ToString() + "a" + Math.Round((double)collision.transform.position.y, 1).ToString() + "h" + collision.transform.GetChild(0).name);

            Destroy(collision.gameObject);
        }
    }

    //Other scripts looting you----
    public void takeItem(int ItemId, int Ammount)
    {
        if (Items[ItemId] >= Ammount)
        {
            NetManager.SendPacket("spend" + ItemId + "|-" + Ammount);
            Items[ItemId] -= Ammount;
            for (int i = 0; i < ItemHolders.Count; i++)
            {
                print(i);
                if (ItemHolders[i].GetChild(2).name == ItemId.ToString())
                {
                    ItemHolders[i].GetChild(0).GetComponent<Text>().text = Items[ItemId].ToString();
                    if (Items[ItemId] == 0)
                    {
                        ItemHolders[i].GetChild(1).GetComponent<Image>().sprite = null;
                        ItemHolders[i].GetChild(2).name = (-1).ToString();
                    }
                    break;
                }
            }
        }
    }

    //SERVER GIVING LOOT??!!
    public void ReceiveItems(string Parameters)
    {
        if (!IsLoaded)
        {
            QuickBegin();
        }
        string[] items = Parameters.Split('|');
        //[Items=   0a10|1a5  ]
        for (int i = 0; i < items.Length; i++)
        {
            int itemId = int.Parse(items[i].Split('a')[0]);
            int Ammount = int.Parse(items[i].Split('a')[1]);
            for (int x = 0; x < (itemId + 1) - Items.Count; x++)
            {
                Items.Add(0);
            }

            Items[itemId] += Ammount;
            StoreItem(itemId, GameObject.Find(itemId.ToString()).transform.GetComponent<SpriteRenderer>().sprite);
        }
    }
}
