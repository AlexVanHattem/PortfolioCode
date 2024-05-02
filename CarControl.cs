using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    // Input axis names
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    // Input values
    private float horizontalInput;
    private float verticalInput;

    // Car physics variables
    private float currentBreakForce;
    private float currentSteeringAngle;
    private float resetBrakeTorque = 0;
    private float driftFriction = 0.9f;

    // Motor settings
    public float motorForce;
    [SerializeField] private float brakeForce = 60000.0f;
    [SerializeField] private float maxSteerAngle;

    // Brake state
    private bool brake = false;

    #region WheelColliders
    // Wheel Colliders for physics simulation
    [SerializeField] private WheelCollider frontLeftCollider;
    [SerializeField] private WheelCollider frontRightCollider;
    [SerializeField] private WheelCollider backLeftCollider;
    [SerializeField] private WheelCollider backRightCollider;
    #endregion

    #region WheelTransform
    // Wheel Transforms for visual representation
    [SerializeField] private Transform frontLeftTransform;
    [SerializeField] private Transform frontRightTransform;
    [SerializeField] private Transform backLeftTransform;
    [SerializeField] private Transform backRightTransform;
    #endregion

    [SerializeField] private Vector3 centerOfMass;
    private Rigidbody _rb;

    private void Start()
    {
        // Set the center of mass for better physics behavior
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = centerOfMass;
    }

    private void FixedUpdate()
    {
        // Process inputs and apply car controls
        GetInput();
        HandleMotor();
        HandleBraking();
        HandleSteering();
        UpdateWheels();
        HandleDrift();
    }

    private void GetInput()
    {
        // Get horizontal and vertical input values
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
    }

    private void HandleMotor()
    {
        // Apply motor force based on vertical input
        backLeftCollider.motorTorque = verticalInput * motorForce;
        backRightCollider.motorTorque = verticalInput * motorForce;

        // Set brake force based on brake state
        currentBreakForce = brake ? brakeForce : 0f;
    }

    private void HandleBraking()
    {
        // Set brake state based on Space key input
        brake = Input.GetKey(KeyCode.Space);

        // Apply braking force if braking
        if (brake)
        {
            ApplyBraking();
        }
        else
        {
            // Reset brake torque when not braking
            frontLeftCollider.brakeTorque = resetBrakeTorque;
            frontRightCollider.brakeTorque = resetBrakeTorque;
            backLeftCollider.brakeTorque = resetBrakeTorque;
            backRightCollider.brakeTorque = resetBrakeTorque;
        }
    }

    private void ApplyBraking()
    {
        // Apply brake force to all wheel colliders
        frontLeftCollider.brakeTorque = currentBreakForce;
        frontRightCollider.brakeTorque = currentBreakForce;
        backLeftCollider.brakeTorque = currentBreakForce;
        backRightCollider.brakeTorque = currentBreakForce;
    }

    private void HandleSteering()
    {
        // Set steering angle based on horizontal input
        currentSteeringAngle = maxSteerAngle * horizontalInput;
        frontLeftCollider.steerAngle = currentSteeringAngle;
        frontRightCollider.steerAngle = currentSteeringAngle;
    }

    private void UpdateWheels()
    {
        // Update wheel transforms to match wheel colliders
        UpdateSingleWheel(frontLeftCollider, frontLeftTransform);
        UpdateSingleWheel(frontRightCollider, frontRightTransform);
        UpdateSingleWheel(backLeftCollider, backLeftTransform);
        UpdateSingleWheel(backRightCollider, backRightTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        // Update wheel position and rotation based on wheel collider
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void HandleDrift()
    {
        // Adjust sideways friction for drifting based on Left Shift key input
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Increase extremum slip for more drift
            WheelFrictionCurve wheelFrictionCurve = backLeftCollider.sidewaysFriction;
            wheelFrictionCurve.extremumSlip = driftFriction;
            backLeftCollider.sidewaysFriction = wheelFrictionCurve;
        }
        else
        {
            // Reset extremum slip when not drifting
            WheelFrictionCurve wheelFrictionCurve = backLeftCollider.sidewaysFriction;
            wheelFrictionCurve.extremumSlip = 0.2f;
            backLeftCollider.sidewaysFriction = wheelFrictionCurve;
        }
    }
}