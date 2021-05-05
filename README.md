# ErratumAI
My contribution as the main protagonist for the Erratum game

Please note this is not a final product as I am still working on the project, and is a demonstration of how my code may look like.

The way it works:
In Erratums case the player's vision is what determines how the enemy should act.
 - VisionCone therefore must be on the player for it all to work, as it calculates the players view angle and the flashlight mechanic requires the player to be facing the enemy head on.
AI:
- The Ai script requires the players location and a gameobject that it considers it's eyes to look out of.
- The AI will follow empty gameobjects as waypoints that need to be set up prior to use.
- it checks if there is a layer of "obstacle" inbetween the player and the enemy so it does not act unless in plain sight.
- "NavmeshSourcetag" and "LocalNavMeshBuilder" were sourced from here: https://github.com/Unity-Technologies/NavMeshComponents/tree/master/Assets/Examples/Scripts
  - they were originally for procedurally generated maps so the AI could walk on them.
  - In our usecase for the second level of our game I used it to keep the enemy from entering rooms if the lights are on.

The Enemy can :
 - Patrol (Walk)
 - Chase (Crawl)
 - Watch (Stand)
 - Teleport
VisionCone is one of the older and most in need of refractoring as it could potentially be just implemented straight into the AI.
It did NOT happen however as we were pressed by time and the game to be more functional was more important than cleaner looking and behaving code.
