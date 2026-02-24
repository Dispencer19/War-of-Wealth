using UnityEngine;

public class Cooldown : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 0.3f;
    private float _nextRefreshTime;

    public bool IsCoolingDown => Time.time < _nextRefreshTime;
    public void StartCooldown() => _nextRefreshTime = Time.time + cooldownTime;
}
