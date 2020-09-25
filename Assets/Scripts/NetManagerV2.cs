using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class NetManagerV2 : MonoBehaviour
{

    public string server_address = "192.168.0.246";
    public int server_port = 9529;
    public TcpClient client;
    public NetworkStream dataStream;
    public string data;

    // Start is called before the first frame update
    void Start()
    {
        client = new TcpClient();
        client.NoDelay = true;
        client.Connect(server_address, server_port);
        dataStream = client.GetStream();
        print("Connected");
    }

    // Update is called once per frame
    void Update()
    {
        var buffsize = client.ReceiveBufferSize;
        byte[] instream = new byte[buffsize];
        dataStream.Read(instream, 0, buffsize);

        data = Encoding.ASCII.GetString(instream);
        print(data);        
    }
}
