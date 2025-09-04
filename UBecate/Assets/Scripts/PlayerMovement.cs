using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement3D : MonoBehaviour
{
    // Variables públicas ajustables desde el Inspector
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpForce = 7f;
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.2f;
    
    // Componentes de referencia
    private Rigidbody rb;
    private Camera mainCamera;
    
    // Variables privadas
    private Vector3 moveDirection;
    private bool isGrounded;
    
    void Awake()
    {
        // Obtener referencias a los componentes
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        // Detectar entrada del jugador
        HandleInput();
        
        // Rotar el jugador hacia la dirección de movimiento
        RotatePlayer();
        
        // Manejar el salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }
    
    void FixedUpdate()
    {
        // Mover al jugador usando física (mejor para Rigidbody)
        MovePlayer();
    }
    
    void HandleInput()
    {
        // Obtener entrada horizontal y vertical (WASD o Joystick)
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        // Calcular dirección de movimiento basada en la cámara
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        
        // Ignorar componente Y para movimiento plano
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        // Calcular dirección final del movimiento
        moveDirection = (cameraForward * verticalInput + cameraRight * horizontalInput).normalized;
    }
    
    void MovePlayer()
    {
        // Aplicar movimiento solo si hay entrada
        if (moveDirection != Vector3.zero)
        {
            // Calcular velocidad deseada
            Vector3 targetVelocity = moveDirection * moveSpeed;
            
            // Mantener la velocidad vertical actual (para gravedad)
            targetVelocity.y = rb.velocity.y;
            
            // Aplicar la velocidad al Rigidbody
            rb.velocity = targetVelocity;
        }
        else
        {
            // Frenar el movimiento horizontal si no hay entrada
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }
    
    void RotatePlayer()
    {
        // Rotar solo si el jugador se está moviendo
        if (moveDirection != Vector3.zero)
        {
            // Calcular la rotación objetivo
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            
            // Suavizar la rotación
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
    
    void Jump()
    {
        // Aplicar fuerza vertical para el salto
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
    void CheckGround()
    {
        // Verificar si el jugador está en el suelo
        isGrounded = Physics.Raycast(
            transform.position, 
            Vector3.down, 
            groundCheckDistance, 
            groundLayer
        );
    }
    
    void OnDrawGizmos()
    {
        // Dibujar línea de debug para el chequeo de suelo
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}