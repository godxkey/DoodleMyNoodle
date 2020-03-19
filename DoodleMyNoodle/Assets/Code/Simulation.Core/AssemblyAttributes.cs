using System.Runtime.CompilerServices;
using Unity.Entities;

[assembly: DisableAutoCreation]


[assembly: InternalsVisibleTo("Simulation.Core.SimInterface")]
[assembly: InternalsVisibleTo("Simulation.Core.ViewInterface")]
[assembly: InternalsVisibleTo("Simulation.Core.Editor")]
[assembly: InternalsVisibleTo("SimulationIO")]