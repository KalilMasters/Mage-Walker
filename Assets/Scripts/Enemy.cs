using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, IDamageable, IFreezable, ILiving
{
    [field: SerializeField] public float YOffset { get; private set; }
    [field: SerializeField] public bool IsFrozen { get; private set; } = false;

    [SerializeField] int hardcoreHealth;
    public bool IsAlive { get; private set; } = true;

    [SerializeField] LayerMask moveMask;

    private CharacterController _player;
    private Animator _animator;
    private Collider _collider;
    private ShieldManager _shieldManager;

    private string A_Walk = "Walk Forward";
    private string A_Run = "Run Forward";

    private UnityEvent<bool> OnStunned = new();

    [SerializeField] IdleState _idleState;
    [SerializeField] TargetPlayer _targetState;

    [SerializeReference] EnemyState _currentState;
    

    private void Update()
    {
        _currentState?.Update();
    }

    public virtual bool Damage(string owner, DamageType type)
    {
        if (gameObject.name.ToLower().Contains(owner.ToLower())) return false;
        Kill(owner);
        return true;
    }
    void OnTakeDamage(string source)
    {
        _animator.SetTrigger("Take Damage");
        OnStunned?.Invoke(true);
    }
    public virtual void Kill(string source)
    {
        SwitchState(new DeathState());

        ScoreSystem.Instance.AddPoints(5);
        IsAlive = false;
        _collider.enabled = false;
    }

    public void SwitchState(EnemyState newState)
    {
        _currentState?.OnExitState();

        newState.self = this;

        _currentState = newState;

        _currentState.OnEnterState();
    }
    private void Awake()
    {
        _player = FindObjectOfType<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponentInChildren<Collider>();
        _shieldManager = GetComponent<ShieldManager>();

        if (_shieldManager)
        {
            if (MapManager.IsHardMode)
                _shieldManager.SetMaxHitPoints(hardcoreHealth);
            _shieldManager.OnRealDamageTaken += Kill;
            _shieldManager.OnShieldDamageTaken += OnTakeDamage;
            _shieldManager.SetToMax();
        }
        SwitchState(_targetState);
    }
    private void OnEnable()
    {
        MapManager.Instance.RegisterEnemy(this);
    }
    private void OnDisable()
    {
        MapManager.Instance.UnRegisterEnemy(this);

    }
    public void OnDamageAnimDone() => OnStunned?.Invoke(false);
    public void Freeze()
    {
        IsFrozen = true;
        _animator.speed = 0;
    }
    public void UnFreeze()
    {
        IsFrozen = false;
        _animator.speed = 1;
    }
    public void Death()
    {
        GameObject.Destroy(gameObject);
    }

    float GetYHeight()
    {
        Vector3 checkSpot = transform.position.SetY(5);
        if (Physics.Raycast(checkSpot, Vector3.down, out RaycastHit hit, float.MaxValue, moveMask, QueryTriggerInteraction.Ignore))
            if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("MoveSpace")))
                return hit.point.y;
        return transform.position.y - 0.5f;
    }

    [System.Serializable]
    public abstract class EnemyState
    {
        public Enemy self;
        protected Transform transform => self.transform;
        protected Vector3 position { get => transform.position; set => transform.position = value; }
        public virtual void OnEnterState()
        {
            if (self == null)
                throw new System.Exception("Assign enemy");
        }
        public virtual void Update()
        {
            if (self == null)
                throw new System.Exception("Assign enemy");
        }
        public virtual void OnExitState()
        {
            if (self == null)
                throw new System.Exception("Assign enemy");
        }
        protected virtual void ResetAnimation()
        {
            self._animator.SetBool(self.A_Walk, false);
            self._animator.SetBool(self.A_Run, false);
        }
    }
    [System.Serializable]
    public class EmptyState : EnemyState
    {
        public override void OnEnterState()
        {
            base.OnEnterState();
            ResetAnimation();
        }
    }
    [System.Serializable]
    public abstract class AttackState : EnemyState
    {
        public UnityEvent OnAttackStart, OnAttackFinish;
    }
    [System.Serializable]
    public class IdleState : EnemyState
    {
        public override void OnEnterState()
        {
            base.OnEnterState();

            self._animator.SetBool(self.A_Walk, false);
            self._animator.SetBool(self.A_Run, false);
        }
    }

    [System.Serializable]
    public class TargetPlayer : EnemyState
    {
        [SerializeField] private FloatContainer MovementSpeed;
        [SerializeField] bool isStunned = false;
        
        public override void OnEnterState()
        {
            base.OnEnterState();
            self._animator.SetBool(self.A_Walk, false);
            self._animator.SetBool(self.A_Run, true);
            self.OnStunned.AddListener(SetStun);
        }
        public override void Update()
        {
            base.Update();

            if (self._player == null || !self._player.gameObject.activeInHierarchy)
            {
                self.SwitchState(self._idleState);
                return;
            }
            if (isStunned) return;
            if (self.IsFrozen) return;
            if (!self.IsAlive) return;

            position = Vector3.MoveTowards(transform.position, self._player.transform.position, MovementSpeed.Value * Time.deltaTime);
            position = position.SetY(self.GetYHeight());
            Vector3 lookAtPosition = self._player.transform.position.SetY(transform.position.y - self.YOffset);
            transform.LookAt(lookAtPosition);
        }

        public override void OnExitState()
        {
            base.OnExitState();
            self.OnStunned.RemoveListener(SetStun);
            self._animator.SetBool(self.A_Run, false);
        }
        void SetStun(bool on)
        {
            isStunned = on;
            self._animator.SetBool(self.A_Run, !isStunned);
        }
    }

    public class DeathState : EnemyState
    {
        public override void OnEnterState()
        {
            base.OnEnterState();
            ResetAnimation();
            self._animator.SetTrigger("Die");
            //self.gameObject.component
        }
    }
}
