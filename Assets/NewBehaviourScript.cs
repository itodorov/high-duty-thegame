using UnityEngine;
using System.Collections;
using System.Text;
using System.Net.Sockets;

public class NewBehaviourScript : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		
		SetupServer ();
		
		InitGameObject();
		
	}

	const int Port = 9090;
	TcpListener listener;
	TcpClient ourTCP_Client;
	NetworkStream ourStream;

	private void SetupServer ()
	{
		System.Net.IPAddress serverAddress = System.Net.IPAddress.Parse ("127.0.0.1");
		
		Debug.Log ("Initializing Server...");
		listener = new TcpListener (serverAddress, Port);
		Debug.Log ("Server Initialized.");
		listener.Start ();
		Debug.Log ("Server Started.");
		ourTCP_Client = listener.AcceptTcpClient ();
		Debug.Log ("Listening for Cleints...");
		ourStream = ourTCP_Client.GetStream ();
		Debug.Log ("Client Found.");
		byte[] data = new byte[ourTCP_Client.ReceiveBufferSize];
		Debug.Log ("Client Data Accepted.");
		int bytesRead = ourStream.Read (data, 0, System.Convert.ToInt32 (ourTCP_Client.ReceiveBufferSize));
		Debug.Log ("Data Received.");
		Debug.Log ("Received: " + Encoding.ASCII.GetString (data, 0, bytesRead));
	}
	
	private GameObject m_Cube;
	
	private void InitGameObject()
	{
		m_Cube = Instantiate(Resources.Load("Cube")) as GameObject;
		m_Cube.transform.localPosition = new Vector3(0.0f,2.0f,0.0f);
	}

	// Update is called once per frame
	int bytesRead;
	byte[] data;
	
	void Update ()
	{
		
		ourStream = ourTCP_Client.GetStream ();
		//Debug.Log ("Update: Client Found.");
		data = new byte[ourTCP_Client.ReceiveBufferSize];
		//Debug.Log ("Update: Client Data Accepted.");
		bytesRead = ourStream.Read (data, 0, System.Convert.ToInt32 (ourTCP_Client.ReceiveBufferSize));
		//Debug.Log ("Update: Data Received.");
		//Debug.Log ("Update: Received: " + Encoding.ASCII.GetString (data, 0, bytesRead));
		//string input = (Encoding.ASCII.GetString (data, 0, bytesRead)).ToString();
		HandleKinectInput((Encoding.ASCII.GetString (data, 0, bytesRead)).ToString());		
	}
	
	private void HandleKinectInput(string input)
	{
		if (input == "Action1")
		{
			m_Cube.transform.RotateAround(new Vector3(0.0f,1.0f,0.0f), 5.0f);
		}
	}
}
