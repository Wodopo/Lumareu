using Prime31.StateKit;
using UnityEngine;
using Wodopo.Engine2D;

public class SusuController : MonoBehaviour
{
    public InputState inputState;
    //public PhysicsObject body;
    public Rigidbody2D body2D;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Setup")]
    public CharacterConfig defaultConfig;

    public Transform Checkpoint;

    [Header("States Setup")]
    public Idle idleState;
    public Walk walkState;
    public Jump jumpState;
    public Fall fallState;
    
    // Hashes
    [HideInInspector] public int WalkHash = Animator.StringToHash("Walking");
    [HideInInspector] public int GroundedHash = Animator.StringToHash("Grounded");
    [HideInInspector] public int JumpingHash = Animator.StringToHash("Jumping");
    [HideInInspector] public int YHash = Animator.StringToHash("Y");

    private SKStateMachine<SusuController> _machine;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        _machine = new SKStateMachine<SusuController>(this, idleState);
        _machine.addState(walkState);
        _machine.addState(jumpState);
        _machine.addState(fallState);
    }

    private void Update()
    {
        _machine.update(Time.deltaTime);

        animator.SetFloat(YHash, body2D.velocity.y);
    }

    private void OnDeath()
    {
        transform.position = Checkpoint.position;
    }
}

[System.Serializable]
public class Idle : SKState<SusuController>
{
    [Header("Inputs")]
    public Buttons moveRight = Buttons.Right;
    public Buttons moveLeft = Buttons.Left;
    public Buttons jump = Buttons.A;

    public override void begin()
    {
        _context.animator.SetBool(_context.GroundedHash, true);
    }

    public override void reason()
    {
        if (_context.inputState.GetButtonDown(jump))
            _machine.changeState<Jump>();

        if (_context.inputState.GetButton(moveRight) || _context.inputState.GetButton(moveLeft))
            _machine.changeState<Walk>();

        if (_context.body2D.velocity.y < 0.0f)
            _machine.changeState<Fall>();
    }

    public override void update(float deltaTime) { }
}

[System.Serializable]
public class Walk : SKState<SusuController>
{
    [Header("Inputs")]
    public Buttons moveRight = Buttons.Right;
    public Buttons moveLeft = Buttons.Left;
    public Buttons jump = Buttons.A;
        
    public override void begin()
    {
        _context.animator.SetBool(_context.WalkHash, true);
        _context.animator.SetBool(_context.GroundedHash, true);
    }

    public override void reason()
    {
        if (_context.inputState.GetButtonDown(jump))
            _machine.changeState<Jump>();

        if (!_context.inputState.GetButton(moveRight) && !_context.inputState.GetButton(moveLeft) && _context.body2D.velocity.x == 0.0f)
            _machine.changeState<Idle>();

        if (_context.body2D.velocity.y < 0.0f)
            _machine.changeState<Fall>();
    }

    public override void update(float deltaTime)
    {
        float desiredX = 0.0f;
        desiredX += _context.inputState.GetButton(moveRight) ? 1.0f : 0.0f;
        desiredX += _context.inputState.GetButton(moveLeft) ? -1.0f : 0.0f;
        desiredX *= _context.defaultConfig.horizontalGroundSpeed;

        Vector2 desiredV = new Vector2(desiredX, _context.body2D.velocity.y);
        _context.body2D.velocity = Vector2.MoveTowards(_context.body2D.velocity, desiredV, Time.deltaTime * _context.defaultConfig.horizontalGroundAcceleration);

        if (Mathf.Abs(desiredX) > 0.0f)
            _context.transform.localScale = Vector3.one + Vector3.right * (desiredX < 0.0f ? -2f : 0f);
    }

    public override void end()
    {
        _context.animator.SetBool(_context.WalkHash, false);
    }
}

[System.Serializable]
public class Jump : SKState<SusuController>
{
    [Header("Inputs")]
    public Buttons moveRight = Buttons.Right;
    public Buttons moveLeft = Buttons.Left;
    public Buttons jump = Buttons.A;
        
    public override void begin()
    {
        _context.body2D.velocity = new Vector2(_context.body2D.velocity.x, _context.defaultConfig.jumpForce);

        _context.animator.SetBool(_context.JumpingHash, true);
        _context.animator.SetBool(_context.GroundedHash, false);
    }

    public override void reason()
    {
        bool moving = _context.inputState.GetButton(moveRight) || _context.inputState.GetButton(moveLeft) || _context.body2D.velocity.x != 0.0f;
        if (_context.body2D.velocity.y == 0.0f)
        {
            if (moving)
                _machine.changeState<Walk>();
            else
                _machine.changeState<Idle>();
        }
    }
    
    public override void update(float deltaTime)
    {
        float desiredX = 0.0f;
        desiredX += _context.inputState.GetButton(moveRight) ? 1.0f : 0.0f;
        desiredX += _context.inputState.GetButton(moveLeft) ? -1.0f : 0.0f;
        desiredX *= _context.defaultConfig.horizontalAirSpeed;
        

        Vector2 desiredV = new Vector2(desiredX, _context.body2D.velocity.y);
        _context.body2D.velocity = Vector2.MoveTowards(_context.body2D.velocity, desiredV, Time.deltaTime * _context.defaultConfig.horizontalAirAcceleration);

        if (Mathf.Abs(desiredX) > 0.0f)
            _context.transform.localScale = Vector3.one + Vector3.right * (desiredX < 0.0f ? -2f : 0f);
    }

    public override void end()
    {
        _context.animator.SetBool(_context.JumpingHash, false);
    }
}

[System.Serializable]
public class Fall : SKState<SusuController>
{
    [Header("Inputs")]
    public Buttons moveRight = Buttons.Right;
    public Buttons moveLeft = Buttons.Left;
    public Buttons jump = Buttons.A;

    [Header("Setup")]
    public float canJumpTime = 0.1f;

    private float timer;
    
    public override void begin()
    {
        timer = 0.0f;
        _context.animator.SetBool(_context.GroundedHash, false);
    }

    public override void reason()
    {
        bool moving = _context.inputState.GetButton(moveRight) || _context.inputState.GetButton(moveLeft) || _context.body2D.velocity.x != 0.0f;

        if (_context.inputState.GetButtonDown(jump) && timer <= canJumpTime)
            _machine.changeState<Jump>();

        if (_context.body2D.velocity.y == 0.0f)
        {
            if (moving)
                _machine.changeState<Walk>();
            else
                _machine.changeState<Idle>();
        }
    }

    public override void update(float deltaTime)
    {
        float desiredX = 0.0f;
        desiredX += _context.inputState.GetButton(moveRight) ? 1.0f : 0.0f;
        desiredX += _context.inputState.GetButton(moveLeft) ? -1.0f : 0.0f;
        desiredX *= _context.defaultConfig.horizontalAirSpeed;

        Vector2 desiredV = new Vector2(desiredX, _context.body2D.velocity.y);
        _context.body2D.velocity = Vector2.MoveTowards(_context.body2D.velocity, desiredV, Time.deltaTime * _context.defaultConfig.horizontalAirAcceleration);

        if (Mathf.Abs(desiredX) > 0.0f)
            _context.transform.localScale = Vector3.one + Vector3.right * (desiredX < 0.0f ? -2f : 0f);

        timer += deltaTime;
    }
}
