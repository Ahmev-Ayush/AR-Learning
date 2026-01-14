using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    // ... [Keep Axel and Wheel structs the same as before] ...
    public enum Axel { Front, Rear }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public GameObject wheelEffectObj;
        public ParticleSystem smokeParticle;
        public Axel axel;
    }

    [Header("Input Settings")]
    [SerializeField] private PlayerInput playerInput;
    
    [Header("Car Settings")]
    public float maxAcceleration = 1500.0f;
    public float brakeAcceleration = 50.0f;
    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;
    public Vector3 _centerOfMass;
    public List<Wheel> wheels;

    private float moveInput;
    private float steerInput;
    private bool isBraking;

    // Mobile specific values
    private float mobileMove;
    private float mobileSteer;
    private bool mobileBrake;

    private Rigidbody carRb;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;
    }

    #region Input Methods
    // New Input System Callbacks (Keyboard/Gamepad)
    public void OnMove(InputValue value)
    {
        Vector2 inputVec = value.Get<Vector2>();
        moveInput = inputVec.y;
        steerInput = inputVec.x;
    }

    public void OnBrake(InputValue value)
    {
        isBraking = value.isPressed;
    }

    // UI Button Callbacks (Mobile)
    public void SetMobileMove(float value) => mobileMove = value;
    public void SetMobileSteer(float value) => mobileSteer = value;
    public void SetMobileBrake(bool value) => mobileBrake = value;
    #endregion

    void Update()
    {
        AnimateWheels();
        WheelEffects();
    }

    void FixedUpdate()
    {
        // Combine inputs: uses whichever is currently active (Mobile or System)
        float finalMove = Mathf.Clamp(moveInput + mobileMove, -1f, 1f);
        float finalSteer = Mathf.Clamp(steerInput + mobileSteer, -1f, 1f);
        bool finalBrake = isBraking || mobileBrake;

        if (Mathf.Abs(finalMove) > 0.01f || Mathf.Abs(finalSteer) > 0.01f || finalBrake)
        {
            Debug.Log($"CarController Input -> Accelerate: {finalMove}, Brake: {finalBrake}, Steer: {finalSteer}");
        }

        ApplyMove(finalMove);
        ApplySteer(finalSteer);
        ApplyBrake(finalBrake, finalMove);
    }

    void ApplyMove(float move)
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = move * 600 * maxAcceleration * Time.fixedDeltaTime;
        }
    }

    void ApplySteer(float steer)
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _targetSteerAngle = steer * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _targetSteerAngle, 0.6f);
            }
        }
    }

    void ApplyBrake(bool brake, float move)
    {
        // Brake if button is pressed OR if no input is given (Auto-brake)
        if (brake || move == 0)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.fixedDeltaTime;
            }
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }

    // ... [Keep AnimateWheels and WheelEffects the same as previous response] ...
    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }

    void WheelEffects()
    {
        bool brakeCondition = (isBraking || mobileBrake);
        foreach (var wheel in wheels)
        {
            bool processEffects = brakeCondition && wheel.axel == Axel.Rear && wheel.wheelCollider.isGrounded && carRb.linearVelocity.magnitude >= 10.0f;
            if (wheel.wheelEffectObj) wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = processEffects;
            if (processEffects && wheel.smokeParticle) wheel.smokeParticle.Emit(1);
        }
    }
}