using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOnlineRoleChoice : MonoBehaviour
{
    [SerializeField] float _roleTransitionTimeout = 10f;

    [Header("Client Specific")]
    [SerializeField] SceneInfo _onlineSessionChoice;

    [Header("Server Specific")]
    [SerializeField] SceneInfo _onlineSessionCreation;

    float _requestTimeoutRemaining;

    public void LaunchClient()
    {
        OnlineService.RequestRole(OnlineRole.Client);

        _requestTimeoutRemaining = _roleTransitionTimeout;
        WaitSpinnerService.Enable(this);
    }

    public void LaunchServer()
    {
        OnlineService.RequestRole(OnlineRole.Server);

        _requestTimeoutRemaining = _roleTransitionTimeout;
        WaitSpinnerService.Enable(this);
    }

    void Start()
    {
        // TODO: read from exec arguments
    }

    void Update()
    {
        if (OnlineService.isChangingRole)
        {
            _requestTimeoutRemaining -= Time.deltaTime;

            if (_requestTimeoutRemaining < 0)
            {
                WaitSpinnerService.Disable(this);
                DebugService.LogError("[OnlineRoleChoice] Timeout: Failed to change role within " + _roleTransitionTimeout + " seconds.");
            }
        }
        else
        {
            if (IsRoleChangeComplete())
            {
                WaitSpinnerService.Disable(this);
                OnRoleChangeComplete();
            }
        }
    }

    bool IsRoleChangeComplete()
    {
        if (OnlineService.isChangingRole)
            return false;

        switch (OnlineService.targetRole)
        {
            case OnlineRole.Client:
                return OnlineService.clientInterface != null;
            case OnlineRole.Server:
                return OnlineService.serverInterface != null;
            default:
                return false;
        }
    }

    void OnRoleChangeComplete()
    {
        if(OnlineService.currentRole == OnlineRole.Client)
        {
            // Client flow: the client needs to pick a session to join
            SceneService.Load(_onlineSessionChoice);
        }
        else
        {
            // Server flow: the server has to create a session that players will be able to join
            SceneService.Load(_onlineSessionCreation);
        }
    }

    // IDEA: we could manage this high-level game flow with a gameflow manager
    // ex: 
    /* 
     * -start-
     * OnlineRoleChoice
     *      pick client -> goto OnlineSessionChoice
     *      pick server -> goto OnlineSessionCreation
     *      pick back -> exit game
     * 
     * OnlineSessionChoice
     *      pick session -> join game
     *      pick back -> goto OnlineRoleChoice
     * 
     * OnlineSessionCreation
     *      pick sessionCreate -> join game
     *      pick back -> goto OnlineRoleChoice
     *      
     * etc.
     * 
     * 
     * This could be setup in a node-like environement in the editor
     * 
     */
}
