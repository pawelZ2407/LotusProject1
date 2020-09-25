using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Text;
using System;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public List<GameObject> Items = new List<GameObject>();

    static public TcpClient client;
    static public int server_port = 9529;
    static public string server_address;
    static public string Username;
    static public string Password;
    static public NetworkStream dataStream;
    static public bool OnlineMode;

    public bool Accepted;
    public string temp = "";

    public GameObject HostPlayer;
    public GameObject ClientPlayer;
    GameObject EnemyPlayer;

    Transform Player;

    Vector2 SpawnPoint;

    [SerializeField]
    static public int MyIndex;

    void Start()
    {
        Accepted = false;
        MyIndex = -1;
        if (server_address != null) { Connect(server_address); }

        if (SceneManager.GetActiveScene().name != "Menu")
        {
            if (!OnlineMode)
            {
                GameObject Spawn;
                Spawn = GameObject.Find("SpawnPoint");
                SpawnPoint = new Vector2(Spawn.transform.position.x, Spawn.transform.position.y);

                Player = Instantiate(HostPlayer, SpawnPoint, Quaternion.identity).transform;
            }
        }

    }

    void Update()
    {
        if (OnlineMode)
        {
            ReceivePackets();
        }

    }

    public void SinglePlayer()
    {
        OnlineMode = false;
        SceneManager.LoadScene("SampleScene");
    }

    public void MultiPlayer()
    {
        OnlineMode = true;
        GameObject TheInput;
        TheInput = GameObject.Find("Ip_Input").transform.Find("Text").gameObject;
        server_address = TheInput.GetComponent<Text>().text;
        TheInput = GameObject.Find("Username").transform.Find("Text").gameObject;
        Username = TheInput.GetComponent<Text>().text;
        TheInput = GameObject.Find("Password").transform.Find("Text").gameObject;
        Password = TheInput.GetComponent<Text>().text;
        SceneManager.LoadScene("SampleScene");

    }

    public void Connect(string address)
    {
        try
        {
            client = new TcpClient();
            client.NoDelay = true;
            client.Connect(address, server_port);
            dataStream = client.GetStream();
            print("Connected");
            SendPacket(Username + "*" + Password);
            print("Username sent.");
        }
        catch
        {
            print("Trying To Connect");
        }
    }

    public void Disconnect()
    {
        SendPacket("Disconnect:" + "pos" + Math.Round((double)Player.position.x, 1).ToString() + "a" + Math.Round((double)Player.position.y, 1).ToString());
        client.Close();
        client.Dispose();
        Application.Quit();
    }

    public void SendPacket(string msg)
    {
        string pack = ("[" + Username + ":" + msg + "]");
        try
        {
            char[] data = pack.ToCharArray();
            byte[] buffer = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                buffer[i] = (byte)data[i];
            }
            dataStream.Write(buffer, 0, buffer.Length);
            dataStream.Flush();
        }
        catch
        {
            print("Can't Send: " + pack);
        }
    }

    public bool Mode { get { return OnlineMode; } }

    public void ReceivePackets()
    {
        if (dataStream.DataAvailable == true)
        {
            byte[] buffer = new byte[1024];
            dataStream.Read(buffer, 0, buffer.Length);
            temp = Encoding.ASCII.GetString(buffer);
            temp = temp.Replace("[", String.Empty);
            string[] temps = temp.Split(']');

            for (int i = 0; i < temps.Length-1; i++)
            {
                temps[i].Replace("[", String.Empty);
                AnalyzePackets(temps[i]);
            }

            temp = String.Empty;
            Array.Clear(temps, 0, temps.Length);
        }
    }

    void AnalyzePackets(string temp)
    {
        print(temp);
        if (Accepted)
        {
            if (!temp.Contains("="))
            {
                try
                {
                    GameObject.Find((temp.Split(':')[0])).GetComponent<Player2Pos>().AnalyzePackets(temp.Split(':')[1]);
                    Player.GetComponent<InventorySystem>().AnalyzeMsg(temp);
                }
                catch
                {
                    EnemyPlayer = (GameObject)Instantiate(ClientPlayer, SpawnPoint, Quaternion.identity);
                    EnemyPlayer.name = temp.Split(':')[0];
                    EnemyPlayer.GetComponent<InventorySystem>().AnalyzeMsg(temp);
                }
            }

            if (temp.Contains("="))
            {
                string[] command = temp.Split('=');
                if (command[0] == "Join")
                {
                    EnemyPlayer = (GameObject)Instantiate(ClientPlayer, SpawnPoint, Quaternion.identity);
                    EnemyPlayer.name = command[1];
                }
                if (command[0] == "Pos")
                {
                    Vector2 receivedPosition = new Vector2(float.Parse(command[1].Split('a')[0]), float.Parse(command[1].Split('a')[1]));
                    if (Player != null)
                    {
                        Player.position = receivedPosition;
                    }
                    else
                    {
                        Player = Instantiate(HostPlayer, receivedPosition, Quaternion.identity).transform;
                    }
                }
                if (command[0] == "Items")
                {
                    Player.GetComponent<InventorySystem>().ReceiveItems(command[1]);
                }
            }
        }
        else
        {
            if (temp != "Accepted")
            {
                string[] values = temp.Split('|');
                GameObject Loaded = Instantiate(Items[int.Parse(values[0])], new Vector2(float.Parse(values[1]), float.Parse(values[2])), Quaternion.identity);
                Loaded.transform.name = values[0];
                if (Loaded.transform.tag == "PickUp")
                {
                    Loaded.transform.GetChild(0).transform.name = values[3];
                }
            }
            else
            {
                Accepted = true;
            }
        }
        
    }
}


