using UnityEngine;
using TMPro;
using Mirror;
using kcp2k;

public class JoinUIHandler : MonoBehaviour
{
    [SerializeField] NetworkingManager networkingManager;
    [SerializeField] TMP_InputField hostPortField;
    [SerializeField] TMP_InputField joinPortField;
    [SerializeField] TMP_InputField joinIPField;
    [SerializeField] Canvas JoinUICanvas;
    [SerializeField] TextMeshProUGUI joinStatus;
    [SerializeField] KcpTransport transport;

    bool isConnectionAttempted = false;

    public void HandleHost()
    {
        networkingManager.OnStartHostEvent += HideJoinUI;

        if (ushort.TryParse(hostPortField.text, out ushort port))
        {
            transport.Port = port;
        }

        networkingManager.StartHost();
    }

    public void HandleJoin()
    {
        transport.OnClientConnected += HideJoinUI;
        transport.OnClientDisconnected += SetStatus;
        if (joinIPField.text == null) return;

        networkingManager.networkAddress = joinIPField.text;

        if (joinIPField.text != null && ushort.TryParse(joinPortField.text, out ushort port))
        {
            transport.Port = port;
        }
        else return;

        joinStatus.text = "Connecting...";
        isConnectionAttempted = true;
        networkingManager.StartClient();
    }

    void SetStatus()
    {
        if (isConnectionAttempted)
        {
            joinStatus.text = "Connection failed";
        }
    }

    void HideJoinUI()
    {
        JoinUICanvas.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        NetworkClient.OnConnectedEvent -= HideJoinUI;
        NetworkClient.OnDisconnectedEvent -= SetStatus;
    }
}
