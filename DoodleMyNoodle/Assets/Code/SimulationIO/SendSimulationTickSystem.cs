using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace SimulationControl
{
    [DisableAutoCreation]
    [UpdateAfter(typeof(ConstructSimulationTickSystem))]
    [UpdateBefore(typeof(TickSimulationSystem))]
    public class SendSimulationTickSystem : ComponentSystem
    {
        public List<SimTickData> ConstructedTicks = new List<SimTickData>();

        private SessionInterface GetSession() => OnlineService.OnlineInterface?.SessionInterface;

        protected override void OnUpdate()
        {
            foreach (SimTickData tick in ConstructedTicks)
            {
                SendTickToConnectedPlayers(tick);
            }
            ConstructedTicks.Clear();
        }

        void SendTickToConnectedPlayers(SimTickData tick)
        {
            var session = GetSession();

            if(session == null)
            {
                Debug.LogWarning("Cannot send sim ticks to clients because no session interface was found.");
                return;
            }

            NetMessageSimTick netMessage = new NetMessageSimTick()
            {
                TickData = tick
            };

            session.SendNetMessage(netMessage, OnlineService.OnlineInterface.SessionInterface.Connections);
        }
    }

}