# Mage-Walker

This project was made for Mobile & Casual Game Design at KSU
Contributors:
--Kalil Masters
--Michael Leung
--Gage Standard

The goal for this game was to create an endless runner similar to Crossy Road and Frogger

-- Procedural Generation --

The world is a series of cubes that are generated based on a biome system

A section of biome named "Chunk" will be created with a specification of anywhere between 1 and the visible length
Different biomes have different qualities like water, roads, and grasslands

The map will automatically scroll and once the map scrolls far enough, the map will generate a new chunk outside the player's view, or it will delete a chunk that is past the players view

-- Map Scrolling --
The map will scroll and as the game progresses it will slowly get faster

Once the player moves from the starting position, the map will begin scrolling

Scripts can register transforms that they want to move in the scroll direction

The map scroller will then loop over every object in that list and move it a very small distance in the scroll direction every frame

Certain events can stop/slow the map scroller

-- Player Controler --

Movement:
The player can move up, down, left, or right and the actual body will auto-move in the selected direction if there is a valid space
The player controller will do a ray cast in the direction of the movement and find the space there

If the player falls behind while the map is scrolling or gets carried off the map by a log, they will immediately die

Lives:
By default, the player has 3 shields that act as health. When the player takes damage, they lose health.
If the player's shields are at 0 and they take damage, they die and the game ends

Abilities:
We created a couple of different abilities for the player to use and can be selected in the main menu
Primary abilities - 
  * Red Fire is a projectile and only does one damage but does explosion damage in a small radius
  * Blue Fire does more damage but does not do explosion damage
Secondary Abilities -
  * Nuke is a projectile that will instantly destroy anything in a large radius
  * Freeze will freeze any eligible object in a large radius around the player
  * Lightning is a projectile that once it hits an enemy, will do chain damage on nearby enemies and objects

-- Other --

Difficulty:
The player can select either easy mode or hard mode
  * Easy mode gives the player 3 shields, target-seeking projectiles
  * Hard mode gets rid of all the player's shields, aimed projectiles, and double health for enemies

Enemies:
Enemies will be spawned randomly every time a new chunk is made
At random intervals, the map will slow down to a stop and enemies will spawn around the player
The map will resume scrolling once the player successfully kills all living enemies

Enemies also use the same health system that the player uses

Light Blue Enemy -
  * Can take 2 Damage
  * Slow
Red Enemy -
  * Can only take 1 Damage
  * Very fast
