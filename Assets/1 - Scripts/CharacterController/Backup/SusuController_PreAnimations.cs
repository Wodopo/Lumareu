using Prime31.StateKit;
using UnityEngine;
using Wodopo.Engine2D;

namespace Backup
{
    public class SusuController : MonoBehaviour
    {
        public InputState inputState;
        public PhysicsObject body;
        public Animator animator;
        public SpriteRenderer spriteRenderer;

        [Header("States Setup")]
        public Idle idleState;
        public Walk walkState;
        public Jump jumpState;
        public WallGrab wallGrabState;
        public Fall fallState;
        public WallJump wallJumpState;

        // Hashes
        public int WalkHash = Animator.StringToHash("Walking");
        public int GroundedHash = Animator.StringToHash("Grounded");
        public int JumpingHash = Animator.StringToHash("Jumping");
        public int YHash = Animator.StringToHash("Y");

        private SKStateMachine<SusuController> _machine;


        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            _machine = new SKStateMachine<SusuController>(this, idleState);
            _machine.addState(walkState);
            _machine.addState(jumpState);
            _machine.addState(wallGrabState);
            _machine.addState(fallState);
            _machine.addState(wallJumpState);
        }

        private void Update()
        {
            if (Mathf.Abs(body.Velocity.x) > 0.0f)
                transform.localScale = Vector3.one + Vector3.right * (body.Velocity.x < 0.0f ? -2f : 0f);
            //spriteRenderer.flipX = body.Velocity.x < 0.0f;
            _machine.update(Time.deltaTime);

            animator.SetFloat(YHash, body.Velocity.y);
        }
    }

    [System.Serializable]
    public class Walk : SKState<SusuController>
    {
        [Header("Inputs")]
        public Buttons moveRight = Buttons.Right;
        public Buttons moveLeft = Buttons.Left;
        public Buttons jump = Buttons.A;

        [Header("Setup")]
        public float xSpeed = 10.0f;
        public float xAcceleration = 50.0f;

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
            desiredX *= xSpeed;

            _context.body.Velocity.x = Mathf.MoveTowards(_context.body.Velocity.x, desiredX, Time.deltaTime * xAcceleration);
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

        [Header("Setup")]
        public float jumpForce = 22.0f;
        public float xSpeed = 10.0f;
        public float xAcceleration = 50.0f;

        private float gravityScale;
        private bool jumpReleased;

        public override void begin()
        {
            _context.body.Velocity.y = jumpForce;
            gravityScale = _context.body.gravityScale;
            jumpReleased = false;

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
            else if ((_context.body.collisionInfo.left || _context.body.collisionInfo.right) && _context.body.Velocity.y < 0.0f)
                _machine.changeState<WallGrab>();
        }

        public override void update(float deltaTime)
        {
            float desiredX = 0.0f;
            desiredX += _context.inputState.GetButton(moveRight) ? 1.0f : 0.0f;
            desiredX += _context.inputState.GetButton(moveLeft) ? -1.0f : 0.0f;
            desiredX *= xSpeed;

            if (_context.inputState.GetButtonUp(jump) || _context.inputState.GetButtonHoldTime(jump) > 0.5f)
                jumpReleased = true;

            if (_context.body.Velocity.y > 0.0f)
            {
                if (!jumpReleased)
                    _context.body.gravityScale = Mathf.Max(_context.body.gravityScale - Time.deltaTime * 5.0f, gravityScale * 0.4f);
            }
            else
                _context.body.gravityScale = gravityScale * 1.4f;

            _context.body.Velocity.x = Mathf.MoveTowards(_context.body.Velocity.x, desiredX, Time.deltaTime * xAcceleration);
        }

        public override void end()
        {
            _context.animator.SetBool(_context.JumpingHash, false);
            _context.body.gravityScale = gravityScale;
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
    public class WallGrab : SKState<SusuController>
    {
        [Header("Inputs")]
        public Buttons moveRight = Buttons.Right;
        public Buttons moveLeft = Buttons.Left;
        public Buttons jump = Buttons.A;

        [Header("Setup")]
        public float holdTime = 0.1f;
        public float gravityMultiplier = 0.2f;

        private bool leftCollision;
        private float timer;

        public override void begin()
        {
            //_context.body.useGravity = false;
            _context.body.gravityScale *= gravityMultiplier;
            _context.body.Velocity = Vector2.zero;

            leftCollision = _context.body.collisionInfo.left;
            timer = 0.0f;
        }

        public override void reason()
        {
            bool rightBtn = _context.inputState.GetButton(moveRight);
            bool leftBtn = _context.inputState.GetButton(moveLeft);
            bool jumpBtn = _context.inputState.GetButtonDown(jump);

            if (_context.body.collisionInfo.down)
                _machine.changeState<Walk>();

            if (jumpBtn)
            {
                if ((leftCollision && rightBtn) ||
                    (!leftCollision && leftBtn))
                    _machine.changeState<Jump>();
                else
                    _machine.changeState<WallJump>();
            }
            else if (timer > holdTime)
                _machine.changeState<Fall>();
        }

        public override void update(float deltaTime)
        {
            bool rightBtn = _context.inputState.GetButton(moveRight);
            bool leftBtn = _context.inputState.GetButton(moveLeft);

            if ((leftCollision && leftBtn) ||
                (!leftCollision && rightBtn))
            {
                timer = 0.0f;
            }
            else
            {
                timer += deltaTime;
            }
        }

        public override void end()
        {
            //_context.body.useGravity = true;
            _context.body.gravityScale /= gravityMultiplier;
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
        public float xSpeed = 10.0f;
        public float xAcceleration = 50.0f;
        public float canJumpTime = 0.1f;

        public float timer;

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

            else if ((_context.body.collisionInfo.left || _context.body.collisionInfo.right) && _context.body.Velocity.y < 0.0f)
                _machine.changeState<WallGrab>();

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
            desiredX *= xSpeed;

            _context.body.Velocity.x = Mathf.MoveTowards(_context.body.Velocity.x, desiredX, Time.deltaTime * xAcceleration);

            timer += deltaTime;
        }
    }

    [System.Serializable]
    public class WallJump : SKState<SusuController>
    {
        [Header("Inputs")]
        public Buttons moveRight = Buttons.Right;
        public Buttons moveLeft = Buttons.Left;

        [Header("Setup")]
        public float jumpForce = 22.0f;
        public float horizontalForce = 2.0f;
        public float xSpeed = 5.0f;
        public float xAcceleration = 50.0f;

        public override void begin()
        {
            _context.body.Velocity.y = jumpForce;
            _context.body.Velocity.x = (_context.inputState.GetButton(moveLeft) ? -1.0f : 1.0f) * horizontalForce;
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
            else if ((_context.body.collisionInfo.left || _context.body.collisionInfo.right) && _context.body.Velocity.y < 0.0f)
                _machine.changeState<WallGrab>();
        }

        public override void update(float deltaTime)
        {
            float desiredX = 0.0f;
            desiredX += _context.inputState.GetButton(moveRight) ? 1.0f : 0.0f;
            desiredX += _context.inputState.GetButton(moveLeft) ? -1.0f : 0.0f;
            desiredX *= xSpeed;

            _context.body.Velocity.x = Mathf.MoveTowards(_context.body.Velocity.x, desiredX, Time.deltaTime * xAcceleration);
        }
    }
}
