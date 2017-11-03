using UnityEngine;

namespace Wodopo.Engine2D
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PhysicsObject : MonoBehaviour
    {
        [Header("Debugging")]
        public bool debug = false;
        public Color debugColor = Color.red;

        [Header("Collisions")]
        public LayerMask collisionMask = 0;
        public int horizontalRayCount = 4;
        public int verticalRayCount = 4;
        protected float horizontalRaySpacing;
        protected float verticalRaySpacing;

        [Header("Properties")]
        public bool useGravity = true;
        public float gravityScale = 1.0f;

        protected const float skinWidth = 0.015f;
        
        [HideInInspector] public CollisionInfo collisionInfo;
        [HideInInspector] public BoxCollider2D box2D = null;
        protected RaycastOrigins raycastOrigins;

        public Vector2 Velocity;

        void Awake()
        {
            box2D = GetComponent<BoxCollider2D>();
            raycastOrigins = new RaycastOrigins();
            CalculateRaySpacing();
        }

        public void LateUpdate()
        {
            Move(Velocity * Time.deltaTime);

            if (collisionInfo.down || collisionInfo.up)
                Velocity.y = 0.0f;

            if (collisionInfo.left || collisionInfo.right)
                Velocity.x = 0.0f;

            if (useGravity)
                Velocity.y += Physics2D.gravity.y * gravityScale * Time.deltaTime;
        }

        public void Move(Vector2 deltaMovement)
        {
            raycastOrigins.UpdateRaycastOrigins(box2D.bounds);

            collisionInfo.Reset();

            if (deltaMovement.x != 0.0f)
                CheckHorizontalCollisions(ref deltaMovement);

            if (deltaMovement.y != 0.0f)
                CheckVerticalCollisions(ref deltaMovement);

            transform.Translate(deltaMovement, Space.World);
        }

        #region Raycasting and Collision
        private void CheckHorizontalCollisions(ref Vector2 translation)
        {
            float direction = Mathf.Sign(translation.x);
            float rayLenght = Mathf.Abs(translation.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (direction == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight);
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * direction, rayLenght, collisionMask);

                if (debug) { Debug.DrawRay(rayOrigin, Vector2.right * direction * rayLenght, debugColor); }
                
                if (hit && hit.distance > 0.0f)
                {
                    translation.x = Mathf.Max(hit.distance - skinWidth, 0.0f) * direction;
                    rayLenght = hit.distance;

                    collisionInfo.left = direction < 0.0f;
                    collisionInfo.right = direction > 0.0f;
                    
                }
            }
        }

        private void CheckVerticalCollisions(ref Vector2 translation)
        {
            float direction = Mathf.Sign(translation.y);
            float rayLenght = Mathf.Abs(translation.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (direction == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft);
                rayOrigin += Vector2.right * (verticalRaySpacing * i + translation.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * direction, rayLenght, collisionMask);

                if (debug) { Debug.DrawRay(rayOrigin, Vector2.up * direction * rayLenght, debugColor); }

                if (hit && hit.distance > 0.0f)
                {
                    translation.y = Mathf.Max(hit.distance - skinWidth, 0.0f) * direction;
                    rayLenght = hit.distance;

                    collisionInfo.up = direction > 0.0f;
                    collisionInfo.down = direction < 0.0f;
                }
            }
        }

        private void CalculateRaySpacing()
        {
            Bounds bounds = box2D.bounds;
            bounds.Expand(-2 * skinWidth);

            horizontalRayCount = Mathf.Max(horizontalRayCount, 2);
            verticalRayCount = Mathf.Max(verticalRayCount, 2);

            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        protected struct RaycastOrigins
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;

            public void UpdateRaycastOrigins(Bounds colliderBounds)
            {
                Bounds bounds = colliderBounds;
                bounds.Expand(-2 * skinWidth);

                bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
                bottomRight = new Vector2(bounds.max.x, bounds.min.y);
                topLeft = new Vector2(bounds.min.x, bounds.max.y);
                topRight = new Vector2(bounds.max.x, bounds.max.y);
            }
        }
        #endregion
        
        public struct CollisionInfo
        {
            public bool up;
            public bool down;
            public bool left;
            public bool right;
            
            public void Reset()
            {
                up = false;
                down = false;
                left = false;
                right = false;
            }
        }
    }
}
