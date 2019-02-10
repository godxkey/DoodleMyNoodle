# Achievements Utility

Achievements information always need to be written inside an API. Same things goes in the Unity project.
You can do that inside a scritable object. you can set a name to identify the achiement and you also need 
to specify the ID for each platform. This object should be linked in the Achievement Service. 
This service, once initialize, give you the ability to Unlock any achievements whatever the platform
the game is running.

## Flags

Another system used to unlock Achievements without actually calling the Unlock function is in the work.
It works as flag system. Different gameplay element or listener goes and turn on a flag when the condition
they're monitoring or checking is completed. When every flag needed for an achievement to be unlock are on,
the achievement will automaticly be unlocked. This prevent too much logic and API calls in the gameplay code.