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
    Failed_ItemHasNoSimAssetId,
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
    private struct ItemTransationData
    {
        public Entity Item;
        public Entity? Source;
        public Entity? Destination;
        public DynamicBuffer<InventoryItemReference>? SourceBuffer;
        public DynamicBuffer<InventoryItemReference>? DestinationBuffer;
        public int Stacks;
        public int SourceCapacity;
        public int DestinationCapacity;
    }

    public static ItemTransactionResult DecrementItem(ISimGameWorldReadWriteAccessor accessor, Entity item, Entity source, int stacks = 1)
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

    public static ItemTransactionResult IncrementItem(ISimGameWorldReadWriteAccessor accessor, Entity item, Entity destination, int stacks = 1)
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

    public static ItemTransactionResult MoveItemAll(ISimGameWorldReadWriteAccessor accessor, Entity item, Entity source, Entity destination)
    {
        return MoveItem(accessor, item, source, destination, int.MaxValue);
    }

    public static ItemTransactionResult MoveItem(ISimGameWorldReadWriteAccessor accessor, Entity item, Entity source, Entity destination, int stacks = 1)
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

    public static void ExecuteItemTransaction(ISimGameWorldReadWriteAccessor accessor, ItemTransationBatch transaction)
    {
        int destinationCap, sourceCap;
        DynamicBuffer<InventoryItemReference>? sourceBuffer, destinationBuffer;

        GetCap(accessor, transaction.Source, out sourceCap);
        GetBuffer(accessor, transaction.Source, out sourceBuffer);

        GetCap(accessor, transaction.Destination, out destinationCap);
        GetBuffer(accessor, transaction.Destination, out destinationBuffer);

        ItemTransationData transationData = new ItemTransationData()
        {
            SourceBuffer = sourceBuffer,
            DestinationBuffer = destinationBuffer,
            SourceCapacity = sourceCap,
            DestinationCapacity = destinationCap,
            Source = transaction.Source,
            Destination = transaction.Destination,
        };

        for (int i = 0; i < transaction.ItemsAndStacks.Length; i++)
        {
            transationData.Item = transaction.ItemsAndStacks[i].item;
            transationData.Stacks = transaction.ItemsAndStacks[i].stacks;

            var report = ExecuteItemTransaction_Internal(accessor, ref transationData);

            if (transaction.OutResults.IsCreated && transaction.OutResults.Length > i)
            {
                transaction.OutResults[i] = report;
            }
        }
    }

    public static ItemTransactionResult ExecuteItemTransaction(ISimGameWorldReadWriteAccessor accessor, ItemTransation transaction)
    {
        int sourceCap, destinationCap;
        DynamicBuffer<InventoryItemReference>? sourceBuffer, destinationBuffer;

        GetCap(accessor, transaction.Source, out sourceCap);
        GetBuffer(accessor, transaction.Source, out sourceBuffer);

        GetCap(accessor, transaction.Destination, out destinationCap);
        GetBuffer(accessor, transaction.Destination, out destinationBuffer);


        ItemTransationData transationData = new ItemTransationData()
        {
            Item = transaction.Item,
            SourceBuffer = sourceBuffer,
            DestinationBuffer = destinationBuffer,
            Stacks = transaction.Stacks,
            SourceCapacity = sourceCap,
            DestinationCapacity = destinationCap,
            Source = transaction.Source,
            Destination = transaction.Destination,
        };

        return ExecuteItemTransaction_Internal(accessor, ref transationData);
    }

    private static void GetCap(ISimGameWorldReadWriteAccessor accessor, Entity? sourceOrDestination, out int cap)
    {
        cap = accessor.HasComponent<InventoryCapacity>(sourceOrDestination.GetValueOrDefault())
            ? accessor.GetComponent<InventoryCapacity>(sourceOrDestination.Value) : default;
    }

    private static void GetBuffer(ISimGameWorldReadWriteAccessor accessor, Entity? sourceOrDestination, out DynamicBuffer<InventoryItemReference>? buffer)
    {
        buffer = accessor.HasComponent<InventoryItemReference>(sourceOrDestination.GetValueOrDefault())
            ? (DynamicBuffer<InventoryItemReference>?)accessor.GetBuffer<InventoryItemReference>(sourceOrDestination.Value) : null;
    }

    /// <param name="accessor"></param>
    /// <param name="item">The item to transfer</param>
    /// <param name="source">The source inventory</param>
    /// <param name="destination">The destination inventory</param>
    /// <param name="stacks">How many stacks to move. Use -1 to specify 'all' stacks</param>
    /// <param name="destinationCapacity">The capacity of the destination inventory</param>
    private static ItemTransactionResult ExecuteItemTransaction_Internal(ISimGameWorldReadWriteAccessor accessor, ref ItemTransationData data)
    {
        if (data.Stacks == 0)
            return new ItemTransactionResult(stackTransfered: 0);

        if (data.Stacks < 0) // invert source and destination ?
        {
            var temp1 = data.SourceBuffer;
            var temp2 = data.SourceCapacity;
            data.SourceBuffer = data.DestinationBuffer;
            data.SourceCapacity = data.DestinationCapacity;
            data.DestinationBuffer = temp1;
            data.DestinationCapacity = temp2;
            data.Stacks = -data.Stacks;
        }

        if (!accessor.Exists(data.Item))
            return new ItemTransactionResult(ItemTransactionResultType.Failed_BadTransationRequest);

        if (data.SourceBuffer != null && !data.SourceBuffer.Value.IsCreated)
            return new ItemTransactionResult(ItemTransactionResultType.Failed_SourceInvalid);

        if (data.DestinationBuffer != null && !data.DestinationBuffer.Value.IsCreated)
            return new ItemTransactionResult(ItemTransactionResultType.Failed_DestinationInvalid);

        if (!accessor.HasComponent<SimAssetId>(data.Item))
            return new ItemTransactionResult(ItemTransactionResultType.Failed_ItemHasNoSimAssetId);

        var itemAssetId = accessor.GetComponent<SimAssetId>(data.Item);
        var sourceIndex = -1;
        var destinationIndex = -1;
        var itemStackable = accessor.GetComponent<StackableFlag>(data.Item);
        var sourceBuffer = data.SourceBuffer.GetValueOrDefault();
        var destinationBuffer = data.DestinationBuffer.GetValueOrDefault();

        if (sourceBuffer.IsCreated) // Find item in source
        {
            for (int i = 0; i < sourceBuffer.Length; i++)
            {
                if (accessor.TryGetComponent(sourceBuffer[i].ItemEntity, out SimAssetId assetId) && assetId == itemAssetId)
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
                if (accessor.TryGetComponent(destinationBuffer[i].ItemEntity, out SimAssetId assetId) && assetId == itemAssetId)
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

        if (destinationBuffer.IsCreated && destinationIndex == -1 && destinationBuffer.Length >= data.DestinationCapacity)
        {
            return new ItemTransactionResult(ItemTransactionResultType.Failed_DestinationFull);
        }

        // cap stacks
        if (sourceIndex >= 0)
        {
            data.Stacks = min(data.Stacks, sourceBuffer[sourceIndex].Stacks);
            if (data.Stacks == 0)
                return new ItemTransactionResult(ItemTransactionResultType.Failed_SourceStack0);
        }

        if (!itemStackable)
        {
            data.Stacks = 1;
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
            sourceEntry.Stacks -= data.Stacks;
            sourceBuffer[sourceIndex] = sourceEntry;

            if (sourceEntry.Stacks <= 0) // remove from source if 0 left
            {
                // remove from source
                sourceBuffer.RemoveAt(sourceIndex);

                // Destroy item
                accessor.DestroyEntity(sourceEntry.ItemEntity);
                UpdateBuffersAfterStructuraleChange(ref data);
            }
        }

        if (destinationBuffer.IsCreated)
        {
            // not found ? add new reference
            if (destinationIndex == -1)
            {
                // Instantiate item copy
                var itemCopy = accessor.Instantiate(data.Item);
                UpdateBuffersAfterStructuraleChange(ref data);
                
                accessor.SetComponent(itemCopy, new Owner() { Value = data.Destination.Value });

                destinationBuffer.Add(new InventoryItemReference() { ItemEntity = itemCopy, Stacks = 0 });
                destinationIndex = destinationBuffer.Length - 1;
            }

            // increase stacks
            var destinationEntry = destinationBuffer[destinationIndex];
            destinationEntry.Stacks += data.Stacks;
            destinationBuffer[destinationIndex] = destinationEntry;
        }

        return new ItemTransactionResult(data.Stacks);

        void UpdateBuffersAfterStructuraleChange(ref ItemTransationData data)
        {
            GetBuffer(accessor, data.Source, out data.SourceBuffer);
            GetBuffer(accessor, data.Destination, out data.DestinationBuffer);
            sourceBuffer = data.SourceBuffer.GetValueOrDefault();
            destinationBuffer = data.DestinationBuffer.GetValueOrDefault();
        }
    }
}