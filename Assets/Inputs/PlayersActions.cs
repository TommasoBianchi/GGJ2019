// GENERATED AUTOMATICALLY FROM 'Assets/Inputs/PlayersActions.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class PlayersActions : InputActionAssetReference
{
    public PlayersActions()
    {
    }
    public PlayersActions(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // Gameplay
        m_Gameplay = asset.GetActionMap("Gameplay");
        m_Gameplay_Attack = m_Gameplay.GetAction("Attack");
        m_Gameplay_Defend = m_Gameplay.GetAction("Defend");
        m_Gameplay_Move = m_Gameplay.GetAction("Move");
        m_Gameplay_GrabEnd = m_Gameplay.GetAction("GrabEnd");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        if (m_GameplayActionsCallbackInterface != null)
        {
            Gameplay.SetCallbacks(null);
        }
        m_Gameplay = null;
        m_Gameplay_Attack = null;
        m_Gameplay_Defend = null;
        m_Gameplay_Move = null;
        m_Gameplay_GrabEnd = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        var GameplayCallbacks = m_GameplayActionsCallbackInterface;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
        Gameplay.SetCallbacks(GameplayCallbacks);
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // Gameplay
    private InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private InputAction m_Gameplay_Attack;
    private InputAction m_Gameplay_Defend;
    private InputAction m_Gameplay_Move;
    private InputAction m_Gameplay_GrabEnd;
    public struct GameplayActions
    {
        private PlayersActions m_Wrapper;
        public GameplayActions(PlayersActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Attack { get { return m_Wrapper.m_Gameplay_Attack; } }
        public InputAction @Defend { get { return m_Wrapper.m_Gameplay_Defend; } }
        public InputAction @Move { get { return m_Wrapper.m_Gameplay_Move; } }
        public InputAction @GrabEnd { get { return m_Wrapper.m_Gameplay_GrabEnd; } }
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                Attack.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                Attack.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                Attack.cancelled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                Defend.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDefend;
                Defend.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDefend;
                Defend.cancelled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDefend;
                Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                Move.cancelled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                GrabEnd.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGrabEnd;
                GrabEnd.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGrabEnd;
                GrabEnd.cancelled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGrabEnd;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                Attack.started += instance.OnAttack;
                Attack.performed += instance.OnAttack;
                Attack.cancelled += instance.OnAttack;
                Defend.started += instance.OnDefend;
                Defend.performed += instance.OnDefend;
                Defend.cancelled += instance.OnDefend;
                Move.started += instance.OnMove;
                Move.performed += instance.OnMove;
                Move.cancelled += instance.OnMove;
                GrabEnd.started += instance.OnGrabEnd;
                GrabEnd.performed += instance.OnGrabEnd;
                GrabEnd.cancelled += instance.OnGrabEnd;
            }
        }
    }
    public GameplayActions @Gameplay
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new GameplayActions(this);
        }
    }
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get

        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.GetControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get

        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.GetControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
}
public interface IGameplayActions
{
    void OnAttack(InputAction.CallbackContext context);
    void OnDefend(InputAction.CallbackContext context);
    void OnMove(InputAction.CallbackContext context);
    void OnGrabEnd(InputAction.CallbackContext context);
}
