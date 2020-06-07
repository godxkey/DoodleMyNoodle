using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using System.Collections.Generic;

public class InvincibleDisplayManagementSystem : GameMonoBehaviour
{
    public GameObject InvincibleBarPrefab;

    private List<GameObject> _invincibleInstances = new List<GameObject>();

    public override void OnGameUpdate()
    {
        int invincibleBubbleAmount = 0;
        GameMonoBehaviourHelpers.GetSimulationWorld().Entities.ForEach((ref Invincible entityHealth, ref FixTranslation entityTranslation) =>
        {
            SetOrAddInvincibleBubble(invincibleBubbleAmount, entityTranslation.Value);

            invincibleBubbleAmount++;
        });

        // Deactivate extra Invincible Bubble
        for (int i = invincibleBubbleAmount; i < _invincibleInstances.Count; i++)
        {
            _invincibleInstances[i].SetActive(false);
        }
    }

    private void SetOrAddInvincibleBubble(int index, fix3 position)
    {
        GameObject currentInvincibleBubble = null;
        if (_invincibleInstances.Count <= index)
        {
            currentInvincibleBubble = Instantiate(InvincibleBarPrefab);
            _invincibleInstances.Add(currentInvincibleBubble);
        }
        else
        {
            currentInvincibleBubble = _invincibleInstances[index];
        }

        currentInvincibleBubble.transform.position = (position + new fix3(0, 0, 0)).ToUnityVec();
        currentInvincibleBubble.SetActive(true);
    }
}
