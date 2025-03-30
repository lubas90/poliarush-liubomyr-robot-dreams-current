using BehaviourTreeSystem.BehaviourStates;
using Dummies;
using MainMenu;
using StateMachineSystem;
using StateMachineSystem.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.AI;
using Health = MainMenu.Health;

namespace BehaviourTreeSystem
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private HealthIndicator _healthIndicator;
        [SerializeField] private EnemyData _data;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Transform _characterTransform;
        [SerializeField] private Health _health;
        [SerializeField] private GameObject _rootObject;
        [SerializeField] private Transform _meshRendererTransform;
        [SerializeField] private Transform _fallMark;
        [SerializeField] private Transform _weaponTransform;
        [SerializeField] private HitScanGun _hitScanGun;
        [SerializeField] private WeaponData _weaponData;
        [SerializeField] private Camera _camera;
        [SerializeField] private Playerdar _playerdar;
        
        [SerializeField, ReadOnly] private float _patrolStamina;
        [SerializeField, ReadOnly] private EnemyBehaviour _currentBehaviour;

        private INavPointProvider _navPointProvider;

        protected BehaviourTree _behaviourTree;
        protected StateMachine _behaviourMachine;
        
        public StateMachine BehaviourMachine => _behaviourMachine;
        
        public float PatrolStamina
        {
            get => _patrolStamina;
            set { _patrolStamina = Mathf.Clamp(value, 0, _data.MaxPatrolStamina); }
        }

        public EnemyData Data => _data;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        public INavPointProvider NavPointProvider => _navPointProvider;
        public CharacterController CharacterController => _characterController;
        public Transform CharacterTransform => _characterTransform;
        public IHealth Health => _health;
        public Playerdar Playerdar => _playerdar;
        public Transform FallMark => _fallMark;
        public GameObject RootObject => _rootObject;
        public Transform MeshRendererTransform => _meshRendererTransform;
        public Transform WeaponTransform => _weaponTransform;
        public HitScanGun HitScanGun => _hitScanGun;
        public WeaponData WeaponData => _weaponData;

        private void Awake()
        {
            _navMeshAgent.updatePosition = false;
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.avoidancePriority = Random.Range(0, 100);

            InitStateMachine();
            _behaviourMachine.OnStateChange += StateChangeHandler;
            
            InitBehaviourTree();

            _health.OnDeath += HealthDeathHandler;
        }

        protected virtual void InitStateMachine()
        {
            _behaviourMachine = new StateMachine();

            _behaviourMachine.AddState((byte)EnemyBehaviour.Deciding,
                new DecisionBehaviour(_behaviourMachine, (byte)EnemyBehaviour.Deciding, this));
            _behaviourMachine.AddState((byte)EnemyBehaviour.Idle,
                new IdleBehaviour(_behaviourMachine, (byte)EnemyBehaviour.Idle, this));
            _behaviourMachine.AddState((byte)EnemyBehaviour.Patrol,
                new PatrolBehaviour(_behaviourMachine, (byte)EnemyBehaviour.Patrol, this));
            _behaviourMachine.AddState((byte)EnemyBehaviour.Search,
                new SearchBehaviour(_behaviourMachine, (byte)EnemyBehaviour.Search, this));
            
            /*_behaviourMachine.AddState((byte)EnemyBehaviour.Attack,
                new AttackBehaviour(_behaviourMachine, (byte)EnemyBehaviour.Attack, this));*/
            _behaviourMachine.AddState((byte)EnemyBehaviour.GunAttack,
                new ShootBehaviour(_behaviourMachine, (byte)EnemyBehaviour.GunAttack, this));
            
            _behaviourMachine.AddState((byte)EnemyBehaviour.Death,
                new DeathBehaviour(_behaviourMachine, (byte)EnemyBehaviour.Death, this));
            _behaviourMachine.AddState((byte)EnemyBehaviour.MeleeAttack,
                new MeleeBehaviour(_behaviourMachine, (byte)EnemyBehaviour.MeleeAttack, this));
        }

        protected virtual void InitBehaviourTree()
        {
            BehaviourLeaf idleLeaf = new BehaviourLeaf((byte)EnemyBehaviour.Idle);
            BehaviourLeaf patrolLeaf = new BehaviourLeaf((byte)EnemyBehaviour.Patrol);

            BehaviourBranch patrolBranch = new BehaviourBranch(patrolLeaf, idleLeaf, PatrolStaminaCondition);

            BehaviourLeaf meleeAttack = new BehaviourLeaf((byte)EnemyBehaviour.MeleeAttack);
            BehaviourLeaf gunAttack = new BehaviourLeaf((byte)EnemyBehaviour.GunAttack);
            
            BehaviourBranch chooseAttack = new BehaviourBranch(meleeAttack, gunAttack, ChooseAttackCondition);
            BehaviourLeaf searchLeaf = new BehaviourLeaf((byte)EnemyBehaviour.Search);
            
            BehaviourBranch seesTarget = new BehaviourBranch(chooseAttack, searchLeaf, SeesTargetCondition);

            BehaviourBranch hasTarget = new BehaviourBranch(seesTarget, patrolBranch, HasTargetCondition);

            _behaviourTree = new BehaviourTree(hasTarget);

            ComputeBehaviour();
        }
        
        private void FixedUpdate()
        {
            _behaviourMachine.Update(Time.fixedDeltaTime);
        }

        private void StateChangeHandler(byte stateId)
        {
            _currentBehaviour = (EnemyBehaviour)stateId;
        }

        public void Initialize(INavPointProvider navPointProvider, Camera camera)
        {
            _navPointProvider = navPointProvider;
            _healthIndicator.Billboard.SetCamera(Camera.main);
        }

        public void ComputeBehaviour()
        {
            if (_behaviourTree == null)
                return;
            _behaviourMachine.SetState(_behaviourTree.GetBehaviourId());
        }

        public void RestorePatrolStamina()
        {
            _patrolStamina = _data.MaxPatrolStamina;
        }

        protected bool PatrolStaminaCondition()
        {
            return _patrolStamina > 0;
        }

        protected bool HasTargetCondition()
        {
            return _playerdar.HasTarget;
        }

        protected bool SeesTargetCondition()
        {
            return _playerdar.SeesTarget;
        }
        
        protected bool ChooseAttackCondition()
        {
            return _playerdar.MeleeTarget;
        }

        protected void HealthDeathHandler()
        {
            _behaviourTree = null;
            _behaviourMachine.ForceState((byte)EnemyBehaviour.Death);
            ServiceLocator.Instance.GetService<IHealthService>().RemoveCharacter(_health);
        }
    }
}