using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    // Setting up the Player as a singleton (assuming we're not doing multiplayer)
    public static Player Instance { get; private set; }

    // Config Player as publisher for selected counter change event
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance.");
        }
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        // Get the 2D input system vector and map to 3D game world vector
        var inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new(inputVector.x, 0, inputVector.y);

        // Caching the last non-zero move direction recorded so that we still
        // consider interaction possible even if we're no longer moving.
        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        // Test for the presence of an object in front of the player.
        // If it's a counter object, mark it as the selected counter.
        // (This doesn't mean we're interacting with it, just that if we choose
        // to by pressing the 'E' key then we know which counter instance to notify)
        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter counter))
            {
                if (counter != selectedCounter)
                {
                    SetSelectedCounter(counter);
                }
            }
            else
            {
                if (selectedCounter != null)
                {
                    SetSelectedCounter(null);
                }
            }
        }
        else
        {
            if (selectedCounter != null)
            {
                SetSelectedCounter(null);
            }
        }
    }

    private void HandleMovement()
    {
        // Get the 2D input system vector and map to 3D game world vector
        var inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new(inputVector.x, 0, inputVector.y);

        // Perform collision detection using a capsule cast
        // (preferred over ray cast for a player character)
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(
            transform.position,
            transform.position + Vector3.up * playerHeight,
            playerRadius,
            moveDir,
            moveDistance);

        // Determine if player should move if they're against an object like a wall
        // e.g. holding W + D keys against a wall just in front of them shouldn't stop them
        //      entirely, but instead apply the D key component of movement
        if (!canMove)
        {
            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(
                transform.position,
                transform.position + Vector3.up * playerHeight,
                playerRadius,
                moveDirX,
                moveDistance);

            if (canMove)
            {
                // Restrict movement to X direction
                moveDir = moveDirX;
            }
            else
            {
                // Attempt only Z movement (considering X movement not possible at this point)
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(
                    transform.position,
                    transform.position + Vector3.up * playerHeight,
                    playerRadius,
                    moveDirZ,
                    moveDistance);

                if (canMove)
                {
                    // Restrict movement to Z direction
                    moveDir = moveDirZ;
                }
            }

        }

        // Apply the update to the player's position & direction they're facing
        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    // Helper method for updating the selected counter and firing a selected counter change event 
    // to notify subscribers (i.e. all the counter instances in the world space in this case).
    // It's possible for selectedCounter to be null, which means no counter is being selected anymore.
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    // Helper methods for a KitchenObject to know which counter 
    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
