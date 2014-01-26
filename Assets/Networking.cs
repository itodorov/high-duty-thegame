using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using System;

public class Networking : MonoBehaviour {

    TcpClient tcpClient = new TcpClient();
    StringBuilder recvBuffer = new StringBuilder();

    public CubeBehavior[] players;

	// Use this for initialization
	void Start () {
        Debug.Log("Connecting");
        tcpClient.Connect("10.10.10.100", 3001); 
	}
	
	// Update is called once per frame
	void Update () {
        if (!tcpClient.Connected)
        {
            //Debug.Log("tcp is not connected");
            return;
        }
        byte[] data = new byte[tcpClient.ReceiveBufferSize];
        NetworkStream ourStream = tcpClient.GetStream();
        if (ourStream.DataAvailable)
        {
            int bytesRead = ourStream.Read(data, 0, tcpClient.ReceiveBufferSize);
            string receivedData = Encoding.UTF8.GetString(data, 0, bytesRead);
            recvBuffer.Append(receivedData);
            Debug.Log("Received some data " + receivedData);
            if (receivedData.Contains("$"))
            {
                OnMessageReceived();
            }
        }

        string s = "";
        foreach (CubeBehavior player in this.players)
        {
            if (player != null && player.scoreChanged)
            {
                player.scoreChanged = false;
                s += "score " + player.playerNumber + " " + player.playerScore * 10 + "$";
            }
        }

        if (s.Length > 0)
        {
            Debug.Log("Sending " + s);
            byte[] payload = Encoding.UTF8.GetBytes(s);
            ourStream.Write(payload, 0, payload.Length);
        }
    }


    private void OnMessageReceived()
    {
        if (recvBuffer.Length == 0)
        {
            return;
        }

        string[] messages = recvBuffer.ToString().Split('$');
        for (int i = 0; i < messages.Length - 1; i++)
        {
            HandleMessage(messages[i]);
        }

        if (messages.Length > 1)
        {
            recvBuffer.Remove(0, recvBuffer.Length);
            recvBuffer.Append(messages[messages.Length - 1]);
        }
    }

    private void HandleMessage(string message)
    {
        Debug.Log("Received message : " + message);
        string command = message.Substring(0, message.Length - 2);
        string playerNumber = message.Substring(message.Length - 1);
        foreach (CubeBehavior player in this.players)
        {
            if (player != null && player.playerNumber.ToString() == playerNumber)
            {
                player.SendCommand(command);
            }
        }
    }


}
