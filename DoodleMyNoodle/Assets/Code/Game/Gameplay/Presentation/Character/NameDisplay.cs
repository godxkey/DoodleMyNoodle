using System;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class NameDisplay : GamePresentationBehaviour
{
    [SerializeField] private TextMeshPro _nameText;

    private Entity _currentEntity = Entity.Null;

    protected override void OnGamePresentationUpdate()
    {
        if(_currentEntity == Entity.Null)
        {
            Vector3 currentWorldPosition = transform.parent.transform.position;
            fix3 position = new fix3() { x = (fix)currentWorldPosition.x, y = (fix)currentWorldPosition.y, z = (fix)currentWorldPosition.z };

            SimWorld.Entities.ForEach((Entity entity, ref Name name, ref FixTranslation translation) =>
            {
                if (translation.Value == position)
                {
                    _currentEntity = entity;
                }
            });
        }

        if (_currentEntity != Entity.Null)
        {
            _nameText.text = SimWorld.GetComponentData<Name>(_currentEntity).Value.ToString();
        }
    }
}