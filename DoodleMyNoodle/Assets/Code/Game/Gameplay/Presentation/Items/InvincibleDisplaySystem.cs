using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using System.Collections.Generic;
using CCC.Fix2D;

public class InvincibleDisplaySystem : GamePresentationSystem<InvincibleDisplaySystem>
{
    public GameObject InvincibleBarPrefab;

    private List<GameObject> _invincibleInstances = new List<GameObject>();

    protected override void OnGamePresentationUpdate()
    {
        int invincibleBubbleAmount = 0;
        Cache.SimWorld.Entities.ForEach((ref InvincibleUntilTime invincibleUntilTime, ref FixTranslation pos) =>
        {
            if (Cache.SimWorld.Time.ElapsedTime < invincibleUntilTime.Time)
            {
                SetOrAddInvincibleBubble(invincibleBubbleAmount, pos.Value);
                invincibleBubbleAmount++;
            }
        });

        // Deactivate extra Invincible Bubble
        for (int i = invincibleBubbleAmount; i < _invincibleInstances.Count; i++)
        {
            _invincibleInstances[i].SetActive(false);
        }
    }

    private void SetOrAddInvincibleBubble(int index, fix3 position)
    {
        GameObject currentInvincibleBubble;
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
