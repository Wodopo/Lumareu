using Prime31.StateKit;
using UnityEngine;
using Wodopo.Engine2D;

public class SusuController : MonoBehaviour
{
    public InputState inputState;
    public PhysicsObject body;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Setup")]
    public float xGroundSpeed = 5.0f;
    public float xAirSpeed = 5.0f;
    public float jumpForce = 15.0f;
    public float xGroundAcceleration = 50.0f;
    public float xAirAcceleration = 50.0f;

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

        animator.SetFloat(YHash, body.Velocity.y);
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

        if (!_context.body.collisionInfo.down)
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

        if (!_context.inputState.GetButton(moveRight) && !_context.inputState.GetButton(moveLeft) && _context.body.Velocity.x == 0.0f)
            _machine.changeState<Idle>();
        
        if (!_context.body.collisionInfo.down)
            _machine.changeState<Fall>();
    }

    public override void update(float deltaTime)
    {
        float desiredX = 0.0f;
        desiredX += _context.inputState.GetButton(moveRight) ? 1.0f : 0.0f;
        desiredX += _context.inputState.GetButton(moveLeft) ? -1.0f : 0.0f;
        desiredX *= _context.xGroundSpeed;

        _context.body.Velocity.x = Mathf.MoveTowards(_context.body.Velocity.x, desiredX, Time.deltaTime * _context.xGroundAcceleration);

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
        _context.body.Velocity.y = _context.jumpForce;

        _context.animator.SetBool(_context.JumpingHash, true);
        _context.animator.SetBool(_context.GroundedHash, false);
    }

    public override void reason()
    {
        bool moving = _context.inputState.GetButton(moveRight) || _context.inputState.GetButton(moveLeft) || _context.body.Velocity.x != 0.0f;
        if (_context.body.collisionInfo.down)
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
        desiredX *= _context.xAirSpeed;
        
        _context.body.Velocity.x = Mathf.MoveTowards(_context.body.Velocity.x, desiredX, Time.deltaTime * _context.xAirAcceleration);

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
        bool moving = _context.inputState.GetButton(moveRight) || _context.inputState.GetButton(moveLeft) || _context.body.Velocity.x != 0.0f;

        if (_context.inputState.GetButtonDown(jump) && timer <= canJumpTime)
            _machine.changeState<Jump>();

        if (_context.body.collisionInfo.down)
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
        desiredX *= _context.xAirSpeed;

        _context.body.Velocity.x = Mathf.MoveTowards(_context.body.Velocity.x, desiredX, Time.deltaTime * _context.xAirAcceleration);

        if (Mathf.Abs(desiredX) > 0.0f)
            _context.transform.localScale = Vector3.one + Vector3.right * (desiredX < 0.0f ? -2f : 0f);

        timer += deltaTime;
    }
}
