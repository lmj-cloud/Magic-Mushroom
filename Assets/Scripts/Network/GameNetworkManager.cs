using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameNetworkManager : MonoBehaviour
{
    [Header("Connection")]
    [SerializeField] private string defaultAddress = "127.0.0.1";
    [SerializeField] private ushort port = 7777;

    [Header("Players")]
    [SerializeField, Min(1)] private int maxPlayers = 4;

    public event Action<ulong> ClientConnected;
    public event Action<ulong> ClientDisconnected;

    public bool IsRunning => networkManager != null && networkManager.IsListening;
    public int ConnectedPlayerCount =>
        networkManager == null ? 0 : networkManager.ConnectedClientsIds.Count;

    private NetworkManager networkManager;
    private UnityTransport transport;

    void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        transport = GetComponent<UnityTransport>();

        networkManager.NetworkConfig.ConnectionApproval = true;
    }

    private void OnEnable()
    {
        networkManager.ConnectionApprovalCallback += ApproveConnection;
        networkManager.OnClientConnectedCallback += HandleClientConnected;
        networkManager.OnClientDisconnectCallback += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        if (networkManager == null)
            return;

        networkManager.ConnectionApprovalCallback -= ApproveConnection;
        networkManager.OnClientConnectedCallback -= HandleClientConnected;
        networkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
    }

    //이 컴퓨터가 서버와 플레이어 역할을 함께 합니다.
    public bool StartHost()
    {
        if (!CanStart())
            return false;

        // 0.0.0.0은 이 컴퓨터의 모든 네트워크 어댑터에서 접속을 받는다는 뜻입니다.
        transport.SetConnectionData(defaultAddress, port, "0.0.0.0");
        bool started = networkManager.StartHost();
        Debug.Log(started ? $"Host 시작: Port {port}" : "Host 시작 실패");
        return started;
    }

    //입력한 Host IP로 접속합니다.
    public bool StartClient(string hostAddress)
    {
        if (!CanStart())
            return false;

        if (string.IsNullOrWhiteSpace(hostAddress))
            hostAddress = defaultAddress;

        transport.SetConnectionData(hostAddress.Trim(), port);
        bool started = networkManager.StartClient();
        Debug.Log(started ? $"Client 접속 시도: {hostAddress}:{port}" : "Client 시작 실패");
        return started;
    }

    //Host 또는 Client 접속을 종료합니다.
    public void Shutdown()
    {
        if (IsRunning)
            networkManager.Shutdown();
    }

    private bool CanStart()
    {
        if (IsRunning)
        {
            Debug.LogWarning("이미 네트워크가 실행 중입니다.");
            return false;
        }

        return true;
    }

    private void ApproveConnection(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        bool hasRoom = networkManager.ConnectedClientsIds.Count < maxPlayers;

        response.Approved = hasRoom;
        response.CreatePlayerObject = hasRoom;
        response.Pending = false;
        response.Reason = hasRoom ? string.Empty : "방의 최대 인원은 4명입니다.";
    }

    private void HandleClientConnected(ulong clientId)
    {
        Debug.Log($"플레이어 접속: {clientId} ({ConnectedPlayerCount}/{maxPlayers})");
        ClientConnected?.Invoke(clientId);
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        Debug.Log($"플레이어 접속 종료: {clientId}");
        ClientDisconnected?.Invoke(clientId);
    }

    private void OnApplicationQuit()
    {
        Shutdown();
    }
}
