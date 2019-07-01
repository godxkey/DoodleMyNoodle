using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationControllerClient : SimulationController
{
    SessionClientInterface _session;

    protected override void Awake()
    {
        base.Awake();

        pendingSimTicks = new SelfRegulatingDropper<NetMessageSimTick>(
            normalDeltaTime          : (float)Simulation.deltaTime,
            expectedQueueLength      : GameConstants.EXPECTED_CLIENT_SIM_TICK_QUEUE_LENGTH,
            speedIncreasePerExtraItem: GameConstants.CLIENT_SIM_TICK_CATCH_UP_FACTOR);
    }

    public override void OnGameReady()
    {
        base.OnGameReady();

        _session = OnlineService.clientInterface.sessionClientInterface;
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

        // fbessette:   we should probably store this tick in a queue that will later be executed. That way, we can keep
        //              the simulation ticking at a somewhat constant rate


        // Tick the simulation locally
        SimInput[] simInputs = new SimInput[tick.inputs.Length];
        for (int i = 0; i < tick.inputs.Length; i++)
        {
            simInputs[i] = tick.inputs[i].input;
        }

        SimTickData tickData = new SimTickData()
        {
            inputs = simInputs
        };

        Simulation.Tick(tickData);
    }

    private void FixedUpdate()
    {
        TickSimIfNeeded();
    }

    private void Update()
    {
        TickSimIfNeeded();
    }

    void TickSimIfNeeded()
    {
    }

    SelfRegulatingDropper<NetMessageSimTick> pendingSimTicks;
    float lastTickTime = -1;
}
