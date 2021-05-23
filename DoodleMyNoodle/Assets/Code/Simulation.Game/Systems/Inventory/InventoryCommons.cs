using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public struct ItemTransation
{
    public Entity? Source;
    public Entity? Destination;
    public Entity Item;
    public int Stacks;
}

public struct ItemTransationBatch
{
    public Entity? Source;
    public Entity? Destination;
    public NativeArray<(Entity item, int stacks)> ItemsAndStacks;
    public NativeArray<ItemTransactionResult> OutResults;
}

public enum ItemTransactionResultType
{
    Failed_BadTransationRequest,
    Failed_ItemNotFoundInSource,
    Failed_SourceStack0,
    Failed_UniqueItemAlreadyInDestination,
    Failed_DestinationFull,
    Failed_SourceInvalid,
    Failed_DestinationInvalid,
    Success
}

public struct ItemTransactionResult
{
    public ItemTransactionResultType Type;
    public int StacksTransfered;

    public bool HasFailed => Type != ItemTransactionResultType.Success;
    public bool HasSucceeded => Type == ItemTransactionResultType.Success;

    public ItemTransactionResult(ItemTransactionResultType failureType)
    {
        Type = failureType;
        StacksTransfered = 0;
    }

    public ItemTransactionResult(int stackTransfered)
    {
        Type = ItemTransactionResultType.Success;
        StacksTransfered = stackTransfered;
    }
}

internal partial class CommonWrites
{
    public static ItemTransactionResult DecrementItem(ISimWorldReadWriteAccessor accessor, Entity item, Entity source, int stacks = 1)
    {
        ItemTransation transaction = new ItemTransation()
        {
            Source = source,
            Destination = null,
            Stacks = stacks,
            Item = item,
        };

        return ExecuteItemTransaction(accessor, transaction);
    }

    public static ItemTransactionResult IncrementItem(ISimWorldReadWriteAccessor accessor, Entity item, Entity destination, int stacks = 1)
    {
        ItemTransation transaction = new ItemTransation()
        {
            Source = null,
            Destination = destination,
            Stacks = stacks,
            Item = item,
        };

        return ExecuteItemTransaction(accessor, transaction);
    }

    public static ItemTransactionResult MoveItemAll(ISimWorldReadWriteAccessor accessor, Entity item, Entity source, Entity destination)
    {
        return MoveItem(accessor, item, source, destination, int.MaxValue);
    }

    public static ItemTransactionResult MoveItem(ISimWorldReadWriteAccessor accessor, Entity item, Entity source, Entity destination, int stacks = 1)
    {
        ItemTransation transaction = new ItemTransation()
        {
            Source = source,
            Destination = destination,
            Stacks = stacks,
            Item = item,
        };

        return ExecuteItemTransaction(accessor, transaction);
    }

    public static void ExecuteItemTransaction(ISimWorldReadWriteAccessor accessor, ItemTransationBatch transaction)
    {
        int destinationCap, sourceCap;
        DynamicBuffer<InventoryItemReference>? sourceBuffer, destinationBuffer;
        GetTransationInfo(accessor, transaction.Source, out sourceCap, out sourceBuffer);
        GetTransationInfo(accessor, transaction.Destination, out destinationCap, out destinationBuffer);

        for (int i = 0; i < transaction.ItemsAndStacks.Length; i++)
        {
            var report = ExecuteItemTransaction_Internal(accessor,
                item: transaction.ItemsAndStacks[i].item,
                source: sourceBuffer,
                destination: destinationBuffer,
                stacks: transaction.ItemsAndStacks[i].stacks,
                sourceCapacity: sourceCap,
                destinationCapacity: destinationCap);

            if (transaction.OutResults.IsCreated && transaction.OutResults.Length > i)
            {
                transaction.OutResults[i] = report;
            }
        }
    }

    public static ItemTransactionResult ExecuteItemTransaction(ISimWorldReadWriteAccessor accessor, ItemTransation transaction)
    {
        int destinationCap, sourceCap;
        DynamicBuffer<InventoryItemReference>? sourceBuffer, destinationBuffer;
        GetTransationInfo(accessor, transaction.Source, out sourceCap, out sourceBuffer);
        GetTransationInfo(accessor, transaction.Destination, out destinationCap, out destinationBuffer);

        return ExecuteItemTransaction_Internal(accessor,
            item: transaction.Item,
            source: sourceBuffer,
            destination: destinationBuffer,
            stacks: transaction.Stacks,
            sourceCapacity: sourceCap,
            destinationCapacity: destinationCap);
    }

    private static void GetTransationInfo(ISimWorldReadWriteAccessor accessor, Entity? sourceOrDestination, out int cap, out DynamicBuffer<InventoryItemReference>? buffer)
    {
        cap = accessor.HasComponent<InventoryCapacity>(sourceOrDestination.GetValueOrDefault())
            ? accessor.GetComponent<InventoryCapacity>(sourceOrDestination.Value) : default;

        buffer = accessor.HasComponent<InventoryItemReference>(sourceOrDestination.GetValueOrDefault())
            ? (DynamicBuffer<InventoryItemReference>?)accessor.GetBuffer<InventoryItemReference>(sourceOrDestination.Value) : null;
    }

    /// <param name="accessor"></param>
    /// <param name="item">The item to transfer</param>
    /// <param name="source">The source inventory</param>
    /// <param name="destination">The destination inventory</param>
    /// <param name="stacks">How many stacks to move. Use -1 to specify 'all' stacks</param>
    /// <param name="destinationCapacity">The capacity of the destination inventory</param>
    private static ItemTransactionResult ExecuteItemTransaction_Internal(ISimWorldReadWriteAccessor accessor
        , Entity item
        , DynamicBuffer<InventoryItemReference>? source
        , DynamicBuffer<InventoryItemReference>? destination
        , int stacks
        , int sourceCapacity
        , int destinationCapacity
        )
    {
        if (stacks == 0)
            return new ItemTransactionResult(stackTransfered: 0);

        if (stacks < 0) // invert source and destination ?
        {
            var temp1 = source;
            var temp2 = sourceCapacity;
            source = destination;
            sourceCapacity = destinationCapacity;
            destination = temp1;
            destinationCapacity = temp2;
            stacks = -stacks;
        }

        if (!accessor.Exists(item))
            return new ItemTransactionResult(ItemTransactionResultType.Failed_BadTransationRequest);

        if (source != null && !source.Value.IsCreated)
            return new ItemTransactionResult(ItemTransactionResultType.Failed_SourceInvalid);

        if (destination != null && !destination.Value.IsCreated)
            return new ItemTransactionResult(ItemTransactionResultType.Failed_DestinationInvalid);

        var sourceIndex = -1;
        var destinationIndex = -1;
        var itemStackable = accessor.GetComponent<StackableFlag>(item);
        var sourceBuffer = source.GetValueOrDefault();
        var destinationBuffer = destination.GetValueOrDefault();

        if (sourceBuffer.IsCreated) // Find item in source
        {
            for (int i = 0; i < sourceBuffer.Length; i++)
            {
                if (sourceBuffer[i].ItemEntity == item)
                {
                    sourceIndex = i;
                    break;
                }
            }
        }

        if (destinationBuffer.IsCreated) // Find item in destination
        {
            for (int i = 0; i < destinationBuffer.Length; i++)
            {
                if (destinationBuffer[i].ItemEntity == item)
                {
                    destinationIndex = i;
                    break;
                }
            }
        }

        if (sourceBuffer.IsCreated && sourceIndex == -1)
        {
            return new ItemTransactionResult(ItemTransactionResultType.Failed_ItemNotFoundInSource);
        }

        if (destinationBuffer.IsCreated && destinationIndex == -1 && destinationBuffer.Length >= destinationCapacity)
        {
            return new ItemTransactionResult(ItemTransactionResultType.Failed_DestinationFull);
        }

        // cap stacks
        if (sourceIndex >= 0)
        {
            stacks = min(stacks, sourceBuffer[sourceIndex].Stacks);
            if (stacks == 0)
                return new ItemTransactionResult(ItemTransactionResultType.Failed_SourceStack0);
        }

        if (!itemStackable)
        {
            stacks = 1;
        }

        // check unique item is not about to be added twice
        if (!itemStackable && destinationIndex != -1 && destinationBuffer[destinationIndex].Stacks > 0)
        {
            return new ItemTransactionResult(ItemTransactionResultType.Failed_UniqueItemAlreadyInDestination);
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Perform Transaction
        ////////////////////////////////////////////////////////////////////////////////////////

        // Remove from source
        if (sourceIndex != -1)
        {
            // decrease stacks
            var sourceEntry = sourceBuffer[sourceIndex];
            sourceEntry.Stacks -= stacks;
            sourceBuffer[sourceIndex] = sourceEntry;

            if (sourceEntry.Stacks <= 0) // remove from source if 0 left
            {
                // remove from source
                sourceBuffer.RemoveAt(sourceIndex);
            }
        }

        if (destinationBuffer.IsCreated)
        {
            // not found ? add new reference
            if (destinationIndex == -1)
            {
                destinationBuffer.Add(new InventoryItemReference() { ItemEntity = item, Stacks = 0 });
                destinationIndex = destinationBuffer.Length - 1;
            }

            // increase stacks
            var destinationEntry = destinationBuffer[destinationIndex];
            destinationEntry.Stacks += stacks;
            destinationBuffer[destinationIndex] = destinationEntry;
        }

        return new ItemTransactionResult(stacks);
    }
}