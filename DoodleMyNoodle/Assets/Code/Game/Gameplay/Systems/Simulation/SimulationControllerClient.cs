using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationControllerClient : SimulationController
{
    public int simTicksInQueue => _pendingSimTicks.queueLength;

    SessionClientInterface _session;
    SelfRegulatingDropper<NetMessageSimTick> _pendingSimTicks;

    protected override void Awake()
    {
        base.Awake();

        _pendingSimTicks = new SelfRegulatingDropper<NetMessageSimTick>(
            maximalCatchUpSpeed: GameConstants.CLIENT_SIM_TICK_MAX_CATCH_UP_SPEED,
            maximalExpectedTimeInQueue: GameConstants.CLIENT_SIM_TICK_MAX_EXPECTED_TIME_IN_QUEUE);
    }

    public override void OnGameReady()
    {
        base.OnGameReady();

        _session = OnlineService.clientInterface.SessionClientInterface;
        _session.RegisterNetMessageReceiver<NetMessageSimTick>(OnNetMessageSimTick);
    }

    public override void OnSafeDestroy()
    {
        base.OnSafeDestroy();

        _session?.UnregisterNetMessageReceiver<NetMessageSimTick>(OnNetMessageSimTick);
        _session = null;
    }

    public override void SubmitInput(SimInput input)
    {
        if (input == null)
        {
            DebugService.LogError("Trying to submit a null input");
            return;
        }

        _session.SendNetMessageToServer(new NetMessageInputSubmission()
        {
            submissionId = InputSubmissionId.Generate(),
            input = input
        });
    }

    void OnNetMessageSimTick(NetMessageSimTick tick, INetworkInterfaceConnection source)
    {
        // The server has sent a tick message
        _pendingSimTicks.Enqueue(tick, (float)SimulationConstants.TIME_STEP);
    }

    private void FixedUpdate()
    {
        if (!SimulationView.IsRunningOrReadyToRun)
            return;

        _pendingSimTicks.Update(Time.fixedDeltaTime);

        SimulationView.UpdateSceneLoads();

        while (SimulationView.CanBeTicked && _pendingSimTicks.TryDrop(out NetMessageSimTick tick))
        {
            if(SimulationView.TickId != tick.tickId)
            {
                SimulationView.ForceSetTickId(tick.tickId);
                DebugService.LogWarning($"[Temporary Hack] We forcefully set the next simulation's stick at {tick.tickId} to match with the server. " +
                    $"This should eventually be replaced by the 'join in progress' feature. NB: This message should only appear once per session!");
            }
            ExecuteTick(tick);
        }
    }

    void ExecuteTick(NetMessageSimTick tick)
    {
        SimInput[] simInputs = new SimInput[tick.inputs.Length];
        for (int i = 0; i < tick.inputs.Length; i++)
        {
            simInputs[i] = tick.inputs[i].input;
        }

        SimTickData tickData = new SimTickData()
        {
            inputs = simInputs
        };

        SimulationView.Tick(tickData);
    }
}
