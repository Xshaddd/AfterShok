using Mirror;
using UnityEngine;


public class HeldItem : NetworkBehaviour
{
    [SyncVar] public uint ownerNetId;
    [SerializeField] float scale;

    [System.Serializable]
    public struct MountSettings
    {
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
    }
    public MountSettings mountSettings;

    public override void OnStartClient()
    {
        base.OnStartClient();
        TryAttach();
    }

    void TryAttach()
    {
        if (!NetworkClient.spawned.TryGetValue(ownerNetId, out var ownerNi)) return; 

        var owner = ownerNi.gameObject;
        var mount = owner.GetComponentInChildren<EquipMount>(true);
        if (mount == null) return;
        transform.SetParent(mount.transform, true);
        transform.localPosition = mountSettings.localPosition;
        transform.localRotation = Quaternion.Euler(mountSettings.localEulerAngles);
        transform.localScale = Vector3.one * scale;
    }
}
