using System;
using Unity.Entities;
using UnityEngineX;


#region Offer Group
/// <summary>Identifies a group of offers</summary>
public struct OfferGroupTag : IComponentData { }

/// <summary>Specifies who these offers are for</summary>
public struct OfferGroupAvailabilityData : IComponentData
{
    public bool OnlyAvailableInPitStop;
}

/// <summary>Specifies who these offers are for</summary>
public struct OfferGroupAssignedPawn : IBufferElementData
{
    public Entity Pawn;
}

/// <summary>List of all the offers in the group</summary>
public struct OfferGroupElement : IBufferElementData
{
    public Entity Offer;
}

/// <summary>Identifies a group of offers as rerollable (user can pay for a reroll)</summary>
public struct OfferGroupRerollableTag : IComponentData { }
#endregion

#region Offer
/// <summary>Identifies an offer</summary>
public struct OfferTag : IComponentData { }

/// <summary>(optional) If set on an offer, purchasing it will destroy the offer</summary>
public struct OfferDestroyAfterPurchaseTag : IComponentData { }

/// <summary>(optional) Price of an offer. If the component is not there, we consider the offer to be free</summary>
public struct OfferPrice : IComponentData
{
    public fix GoldPrice;
}

/// <summary>(optional) The item given to the player when purchased</summary>
public struct OfferItem : IComponentData
{
    public Entity ItemPrefab;
}

/// <summary>(optional) The XP given to the player when purchased</summary>
public struct OfferXPGain : IComponentData
{
    public fix Value;
}

/// <summary>(optional) Does this offer rerolls the offer group if purchased</summary>
public struct OfferRerollTag : IComponentData { }
#endregion

#region User Requests
/// <summary>Purchase offer</summary>
public struct SystemRequestPurchaseOffer : ISingletonBufferElementData
{
    public Entity Pawn;
    public Entity OfferGroup;
    public Entity Offer;
}
#endregion

//public struct SingletonElementOfferGroup : ISingletonBufferElementData
//{
//    public Entity OfferGroup;

//    public static implicit operator Entity(SingletonElementOfferGroup val) => val.OfferGroup;
//    public static implicit operator SingletonElementOfferGroup(Entity val) => new SingletonElementOfferGroup() { OfferGroup = val };
//}

public partial class OfferSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        var purchaseRequestsBuffer = GetSingletonBuffer<SystemRequestPurchaseOffer>();
        if (!purchaseRequestsBuffer.IsEmpty)
        {
            var purchaseRequests = purchaseRequestsBuffer.ToNativeArray(Unity.Collections.Allocator.Temp);
            purchaseRequestsBuffer.Clear();

            foreach (SystemRequestPurchaseOffer purchaseRequest in purchaseRequests)
            {
                ProcessPurchaseRequest(purchaseRequest);
            }
        }
    }

    private void ProcessPurchaseRequest(SystemRequestPurchaseOffer purchaseRequest)
    {
        if (!EntityManager.HasComponent<OfferGroupAssignedPawn>(purchaseRequest.OfferGroup) ||
            !EntityManager.HasComponent<OfferGroupElement>(purchaseRequest.OfferGroup))
        {
            Log.Warning("[OfferSystem] Purchase request failed: offer group is invalid or destroyed.");
            return;
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Is pawn allowed to access group?
        ////////////////////////////////////////////////////////////////////////////////////////
        {
            var assignedPawns = EntityManager.GetBuffer<OfferGroupAssignedPawn>(purchaseRequest.OfferGroup);

            bool pawnIsAllowed = false;
            foreach (var item in assignedPawns)
            {
                if (item.Pawn == purchaseRequest.Pawn)
                {
                    pawnIsAllowed = true;
                    break;
                }
            }

            if (!pawnIsAllowed)
            {
                Log.Warning("[OfferSystem] Purchase request failed: pawn cannot purchase from this offer group.");
                return;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Is offer in group ?
        ////////////////////////////////////////////////////////////////////////////////////////
        {
            var offerElements = EntityManager.GetBuffer<OfferGroupElement>(purchaseRequest.OfferGroup);
            bool offerIsInGroup = false;
            foreach (var item in offerElements)
            {
                if (item.Offer == purchaseRequest.Offer)
                {
                    offerIsInGroup = true;
                    break;
                }
            }

            if (!offerIsInGroup)
            {
                Log.Warning($"[OfferSystem] Purchase request failed: offer is not in group.");
                return;
            }

            if (!HasComponent<OfferTag>(purchaseRequest.Offer))
            {
                Log.Warning("[OfferSystem] Purchase request failed: offer is invalid or destroyed.");
                return;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Payment
        ////////////////////////////////////////////////////////////////////////////////////////
        {
            if (HasComponent<OfferPrice>(purchaseRequest.Offer))
            {
                var offerPrice = GetComponent<OfferPrice>(purchaseRequest.Offer);

                if (offerPrice.GoldPrice != 0 && !HasComponent<Gold>(purchaseRequest.Pawn))
                {
                    Log.Warning("[OfferSystem] Purchase request failed: pawn has no gold component.");
                    return;
                }

                var pawnGold = GetComponent<Gold>(purchaseRequest.Pawn);
                if (offerPrice.GoldPrice > pawnGold)
                {
                    Log.Warning($"[OfferSystem] Purchase request failed: pawn does not have enough gold ({pawnGold.Value} vs. {offerPrice.GoldPrice}).");
                    return;
                }

                // pay with gold
                pawnGold.Value -= offerPrice.GoldPrice;
                SetComponent(purchaseRequest.Pawn, pawnGold);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Grant item
        ////////////////////////////////////////////////////////////////////////////////////////
        {
            if (HasComponent<OfferItem>(purchaseRequest.Offer))
            {
                ItemTransation itemTransation = new ItemTransation()
                {
                    Destination = purchaseRequest.Pawn,
                    Item = GetComponent<OfferItem>(purchaseRequest.Offer).ItemPrefab,
                    Source = null,
                    Stacks = 1
                };
                CommonWrites.ExecuteItemTransaction(Accessor, itemTransation);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Grant XP
        ////////////////////////////////////////////////////////////////////////////////////////
        {
            if (HasComponent<OfferXPGain>(purchaseRequest.Offer))
            {
                if (HasComponent<XP>(purchaseRequest.Pawn))
                {
                    var pawnXP = GetComponent<XP>(purchaseRequest.Pawn);
                    pawnXP.Value += GetComponent<OfferXPGain>(purchaseRequest.Offer).Value;
                    SetComponent(purchaseRequest.Pawn, pawnXP);
                }
                else
                {
                    Log.Warning($"[OfferSystem] Purchase request partial failure: pawn does not have XP component.");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Process Reroll
        ////////////////////////////////////////////////////////////////////////////////////////
        {
            if (HasComponent<OfferRerollTag>(purchaseRequest.Offer))
            {
                // todo
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Destroy offer (if needed)
        ////////////////////////////////////////////////////////////////////////////////////////
        {
            if (HasComponent<OfferDestroyAfterPurchaseTag>(purchaseRequest.Offer))
            {
                EntityManager.DestroyEntity(purchaseRequest.Offer);

                // remove from group
                var offerElements = EntityManager.GetBuffer<OfferGroupElement>(purchaseRequest.OfferGroup);
                for (int i = offerElements.Length - 1; i >= 0; i--)
                {
                    if (!EntityManager.Exists(offerElements[i].Offer))
                        offerElements.RemoveAt(i);
                }
            }
        }
    }
}
