HES: Hazardous Extraction Systems

A rewrite of H.A.R.V.E.S.T.E.R, redoing core game mechanics, graphics, and architecture with the lessons learned from H.A.R.V.E.S.T.E.R

Developed with Unity 6.6.9f1.

To install this project, fork and clone it into a preferred directory. Then open Unity Hub and press "Add" -> "Add from disk". Then select the cloned directory.
Git LFS is required for installation.

This project currently uses Unity's Cinemachine 3 and TextMeshPro. FMOD integration will be done soon.

All "data" (names, stats, settings, etc.) are held within Unity's ScriptableObjects.
Globals are used to share real-time data (input, velocities, etc.) with the rest of the game.

=============================================================================================================================================================================================================================
=============================================================================================================================================================================================================================

**Game Design Plan (Rough Draft):**

Premise: Player is a blue-collar, expendable asteroid miner for the Shergeo Company. They must meet a certain quota every in-game day, or they are left unable to dock, suffocate, and die.
Keep in mind: This game should be friendly for HOTAS users. While this game will be a PC game and we expect the user to have a mouse (to at least use the monitor), we should minimize the need to use the mouse (and allow the monitor to be keybindable anyway).
(This is why many features rely on the radar rather than mouse control).

Movement:

ACCELERATION: If the ship is halted, there will be a small delay (at most 500 ms) where the engine "warms up". After that, add a massive initial "kick" as engines fire, and then add small continuous thrust. No max speed. Induce system/hull failures at "overspeeds".
MOMENTUM: Ship retains linear momentum at all times. Angular momentum is damped and will stop if inputs cease. So, close to Newtonian.
ROTATION: Using keyboard or mouse, player can orient ship in any direction, with a speed cap of course.
DECELLERATION: If no throttle is detected, and a "halt" key is pressed, play heavy thruster sound effects and camera shake as the ship rapidly tries to slow down in a physically possible fashion.


Navigation:

A waypoint system for holographic waypoints on the player's HUD to navigate them to downloaded spatial coordinates.
If a waypoint is far enough away, the player can initiate a warp. The player must be halted to initiate a warp. Movement is locked during warp initiation and the warp itself. A cutscene plays and transports the player to the new location, and recenters the world.
Warp initiation takes 2.7 seconds, leaving the player completely vulnerable and exposed if there were any immediate danger.

Data Downloads / Wrecks:

Scattered wrecks can be scanned to give either of the following:
 - Data logs (For lore purposes)
 - Navigation data (Waypoints)
 - Exclusive upgrades
 
Wrecks must be locked via radar, and the information needs to be "decrypted" and "reconstructed" during a loading screen. The signal should be noisy and alert any nearby enemies
If all possible pieces of data have been collected in 1 playthrough, wrecks should just spit out data logs categorized as garbled garbage.

Radar:

The player's radar should have 3 distinct modes:

Passive (Always on): Radar sweeps occasionally. Every 5 seconds, the closest enemy in front of you (if within range) gets highlighted/pinged. Can be combined with voice warning: "Warning. Armed Vessel Nearby"
SCAN: Select 1 and only 1 resource from a screen. A visual scan sweeps across the world in front of the player. All asteroids containing the designated target are briefly pinged/highlighted. Each scan alerts every enemy that gets scanned. "TYPE-X" is listed
as an available resource, but no specific Type is scannable.
DOG SWITCH: Scan in a small vertical zone in front of the nose. Anything moving 5 km/h or faster that the radar scans is locked immediately.
STILL SWITCH: Scan in a small vertical zone in front of the nose. Anything moving slower than 5 km/h gets locked immediately. 

(For both switches, can just do a boxcast, filter by speed, and find closest target.)

Laser:

Laser has 1 second charge up after being held down.
After charging, the laser cannon fires and begins shaking.
Flames and sparks at impact point.
Every tick (Maybe once every 5 seconds), a piece of ore breaks off.
Individual ore cannot be locked by radar (but should be able to be scanned).

Laser Intensity can be changed from 0 - 100%. Higher intensity increases mining speed. Higher intensity can also put a healthier ore into a critical state much faster. Higher intensities can also destroy ore before it falls off.
Lower intensities damage the asteroid without actually causing any ore to break off.

Tractor Beam:
Will auto-lock to the closest piece of free ore. Should be able to switch the selected ore via UI.
After firing, the ore gets pulled in. Once close enough, it disappears and is added to inventory with some sort of indicator.

Monitor:

A monitor the player can unfold and use at any time.
Acts as the visual interface for things such as radar scanning, navigation, data downloading, inventory management, etc.

Asteroids:
Each asteroid has a composition. The vast majority of them should be made of nothing useful (Dust, trash, ancient metals, etc).
Asteroids that contain "common ores" / "quota ores" should exist in each waypoint.
Asteroids that contain rarer ores should have higher monetary value, of course. Enough to incentivize searching for them and making them special.
Higher-value ores should be rarer. By default, they should also not be scannable.
Asteroids can reach a critical state if they get too small and are heated by the laser, and explode, causing damage to all nearby ships (including the player, of course), and destroying the entire asteroid itself.
Asteroids can be labeled by the scanner or the analyzer as just "TYPE-X" asteroids (with X being some positive integer). Each asteroid type will have at least 1 ore type it must contain, but you cannot tell which ones it is until you mine it yourself.
Crap asteroids should visually look distinct from ones that contain ore, meaning you can tell when a crap asteroid is crap just by looking at it. Useful ones should have things sticking out of them, or look meaningfully different in general.
Keep in mind that what makes analysis important is the existence of TYPE-X asteroids, where their contents are ambiguous until analyzed, even if the appearance lets you identify which TYPE it is.

Depending on the composition, each asteroid will require a certain laser intensity range.
Instead of cross-referencing with a book, there should be a procedure.
More heat-resistant ores mean you need a higher intensity.
Weaker ores mean you need a lower one.
Internally, the exact percentage should be a weighted normalized average, and a +-15% safe interval should be made within that range.
Anything above can mean the asteroid reaches critical state unexpectedly and alarmingly fast (At such a speed that it almost requires pure reflexes to stop it), which adds pressure.

World Generation:

Everything is saved between days. When an asteroid is destroyed, it's gone forever. When an enemy is killed, it is gone forever (however, that doesn't mean new ones can't spawn. Any given location may repopulate). Each waypoint should keep track of and save
all of the data within its own children.

Waypoints should be hand-tuned. While exact object positions and types may be randomized, their clustering, enemy spawn behavior, and available ore types should be based on pre-defined parameters. Wrecks should have fixed contents and positions.
If none of the player's downloaded waypoints contain ore that is needed for their quota, Shergeo will notify them of this and lend them navigational data for a pre-defined waypoint. This waypoint should be extremely dangerous; most of the ore should
be trash, the only "valuable" ores should be the ones needed, and there are no wrecks. This should, in theory, incentivize the player to search for wrecks and have a diverse collection of waypoints to choose from.

Small fields of asteroids scattered across large spaces should be the norm. The world shouldn't just be 1 giant asteroid field where everything is right next to each other (Unless there's a really good reason for 1 waypoint to be doing that)
1 in 20 clusters may have 0 useful ore in them (entirely crap).

Debris System:

Random debris can spawn (outside of the player's view) and move in a collision course with the player for ~2.5 seconds (no matter the player's speed). Damage should be major if collision occurs. The player should be given a warning the moment it spawns and a red arrow
pointing to its origin. Debris cannot spawn intersecting any geometry (if debris attempts to spawn too many times and can't, switch to a smaller-sized one until it can).

Health System:
The player's ship's hull has a health system. It can go down from obvious means (Missiles, collisions, etc). When health reaches 0, the hull reaches "Critical Hull State," which prompts alarms and fires and stuff like that.
During critical hull state, anything that would deal 10 or more damage will kill the player immediately. Death is experienced as a white flashbang, ringing in the ears, camera shake, and a time slowdown, followed up with a death screen.
After death, the player can choose to return to the menu, quit the game, or restart the day.
If damage was excessive enough, skip the Critical Hull State and just kill the player (Like a 1000 km/h crash into an object, or a nuclear detonation at point-blank, or a scripted death)

Quota:

Up to 4 different ores should be extracted per day to some quantity each. If the player does not meet the quota under their oxygen timer, they die.

Oxygen System:

There is a time limit for each day: the oxygen timer. Oxygen does not deplete at a flat rate. Oxygen can increase the rate of consumption during:
 - Laser utilization
 - Critical Hull State
 - Mining radioactive materials (Life support systems get strained)
 - Engine Use
 
 1 minute before oxygen runs out, warnings begin to play.
 
Ad system:

Every 3-5 minutes, "Optincast Radio" will play a commercial from a predetermined sequence of comedic ads that follow a chronological order and build the game's story.
If the player docks while an ad is playing, the ad will play again the next day.
Once an important/final ad is reached, the next day will load up a major event/milestone.

Briefing System:
Every day, a random briefing should be provided. During major plot points, a customized briefing will be certain to show up.
 
Progression System:

Progress should implicitly be locked behind how many ads the player has listened to. Enough ad time should have occurred to force several in-game days to have passed, which subsequently should have given the player time to actually play the game and buy upgrades.
The incentive to work fast should come from the oxygen timer, not speedrunning the game itself. However, working faster and better means more upgrades in the long term.
To further incentivize playing the game well, meeting quotas under a stricter time limit and/or bringing more ore than expected can earn you a "promotion", in which your deduction rate decreases to as much as 75% (depending on how many promotions are received).
Every time the player docks, the day is considered complete.

Upgrades/Hub/The Market:

Once the player docks, they can sell their entire ore inventory at a 95% deduction rate with a randomly generated list of arbitrary fees.
Money can be used to buy ship repairs and upgrades.
The player is allowed to go into debt for a repair if they so choose to do so.
Upgrades can consist of:
Faster/More Responsive Thrusters
Faster laser
More efficient laser
More efficient engines
Weapons (Like missiles)
Longer scanning range
Faster scanning speed
Ability to scan rarer ores
Camouflage (hides from enemies)
Silent decrypter
Silent scanner
Max Oxygen Supply
Warp Speed
Instant Warp Initiation
Nuclear-Safe Inventory System (Removes O2 debuffs when nuclear materials are in the inventory)
Etc

Enemies:

Currently modeled:

MISSILEBOT: A rogue AI-controlled ship that patrols random areas, looking for a signal. Once they find the player, they rush in straight lines and fire missiles at them, which can be outmaneuvered.
ALECKRO MERCENARY: A man-piloted stealth ship that hides behind asteroids as it tries to ambush its prey. Once spotted, they will give a quick radio quip, freeze, open their guns, and begin firing while aiming at you. When they are not firing, they will
move with extreme speed and precision to try and ram you (collision damage).
REDACTOR: A boss ship. This does not care about the player specifically. This is deployed by Shergeo whenever they want to clean out their own rogue AIs. 20 flak cannons shoot at everything on sight. Will shoot asteroids too and destroy them for better
sightlines. Deploys kamikaze drone swarms to seek out and damage any ships they find. Has an on-board jammer that prevents warps for a temporary amount of time while it's there. The REDACTOR is the only enemy that can warp to the player, and is given a small
non-intrusive cutscene whenever it is deployed. The only way to survive is to run and hide until warp ends.
MININGBOT: Visually similar to a MISSILEBOT, but completely harmless. These ships simply look for asteroids and mine them peacefully. They will run away if you fire your laser at them. Some of them will mine asteroids to a critical state, damaging themselves
and potentially creating physical chain reactions as the explosive force pushes other asteroids into other asteroids and so on.

When a dogfight is initiated, music and a warning begin to play. The player can try to outrun them or to lock them with their laser and burn them until they either run away or explode.

Camera System:
First-person cockpit view by default. Cinematic cameras should be available (Like an attached camera, or a panning camera from a specific angle) with adjustable rotation, position (for attached only), and zoom.
Default Camera must have some sort of rig so that the ship has "weight". Responsive camera shake should be accessible, and multiple camera shake calls at the same time (or in close intervals) should work.

Inventory System:

The player has limited inventory space. Each ore type can be jettisoned if needed (with chooseable quantity via slider). 
Each ore has its own mass as well. Will increase ship inertia and possibly rotation speed.
Inventory should be small to force resource management and punish blindly extracting unneeded ores without analysis.

Asteroid Analysis:
All locked asteroids can be analyzed to find their exact composition (Unless it is a Type asteroid) and predicted value.
Sometimes, the computer may need to be calibrated before analysis. It will prompt you for what type of asteroid this is. This is when asteroid visuals will become useful.
If you get it wrong, it will simply give you the ore names for the kinds of ores of the asteroid type you picked.

Handbook:

The player can access a handbook for important and unintuitive information, along with an "ore guide" so they can determine weight, value, pros and cons, etc.
Ore guide should not include Type-X asteroids. Only what the definition means. The differences between them should be discovered by the player.

Radioactivity:

Radioactive materials will exist. Their explosions are much larger than normal, and their values are the highest. They also increase the oxygen utilization rate when in the inventory.

The Union's Deal:

During the halfway point of the game, the player should get a radio message from The Union (anti-Shergeo faction), incentivizing them to find nuclear material and sell it to them for a 0 deduction fee. When this deal is eventually taken, the player
is branded a traitor, and we move on to the 2nd phase of the game.

Threat Level:

Spending too much time in a waypoint can increase threat level. Enemies begin to know that you go there often, and their spawn rate will increase. Leaving the area for 2+ days can drop the threat level back to 0.

The Debris Storm:

With each passing day, larger debris sizes become available, and their spawn rate will increase. There will be some sort of cap (Find it by fine-tuning)

EVENTS (Happen at random chance. Most events shouldn't be able to happen at the same time. If no events occur in a day, increase the chances until one does happen the next day, and then reset them back to their default values. All events should have some kind of
piece of radio dialogue, or a briefing that immediately precedes it.):

Solar Storms:

Random occurrences that happen in asteroid fields (i.e., not the Shergeo docking station).
Radars no longer work. All enemies become rammers. Lasers can only be shot facing the same direction as the nose. Navigation no longer works. The monitor can no longer be used. 
Any data downloads/warp attempts are immediately halted.
Radar locks are dropped.
A solar storm can last for a few minutes.
A solar storm cannot occur if oxygen is below a certain threshold.

REDACTOR events:

The REDACTOR can just come. This is a random event that can happen if the player isn't completely brand new. Can only happen at most once per day, and cannot warp out of a waypoint once it gets there.

Bounty Events:
Kill a rogue Missilebot for a bounty reward upon docking. This comes with its own unique briefing. Aleckro ships are included once in the Union phase.

Bring Your Daughter To Work Day Event (Can only happen at most once per playthrough. And even then, it should be rare. Comes with its own briefing):

All rocks become pink and crying baby sounds play on repeat inside of the cockpit.

Containment Breach Event (Cannot go with solar storm):

A dozen rogue AIs are spawned within near proximity of the player.

999 Fault Codes Event (Cannot go with solar storm):

All indicators give wrong numbers and values until you halt and restart your ship (which takes around 20-30 seconds)

Non-Lethal Ruptured Brain Aneurysm:
Ship AI will telegraph, "Warning: Ruptured Brain Aneurysm Detected. Deploying Emergency Medical Nanobot Swarm."
For about 30 seconds, the screen may randomly fade to black and back to normal, and random, extremely jarring post-processing effects are applied.
All inputs are delayed by 0.2 seconds for the duration of the event.

Sudden Hail Event:

30 pieces of debris in 30 seconds. Dodge them all.

Charity Event (Requires own briefing):

Promise to collect an extra ore for your quota to bail out a co-worker who couldn't meet their own quota. You will be charged 15% less in deductions if you do. If you deny the offer, the game will label you as a murderer.

Heat Storm:

Red tint on screen with vignette.
Engines create more thrust and become more responsive
Asteroids heat faster and burn hotter.
Safe laser intensity intervals shift lower.
Affects the waypoint for the rest of the day.
After spending more than 3 minutes at the affected waypoint, the AC turns on (O2 drain).

Microscopic Organism Swarm Event (Cannot happen with solar storm):
The game naturally has atmospheric fog.
This greatly increases fog density. Can barely see in front of the cockpit.
Affects the waypoint for the rest of the day.

Engine Fire Event:

Can happen if the engine is pushed too hard or after damage is taken.
Player must shut off the engine temporarily to extinguish it.
This is the only event where O2 efficiency is increased.

POST SHERGEO CHANGES:

The STILL switch gets removed because it starts with the letter S, and The Union hates Shergeo. All asteroids now must be moving to be locked. The player will be given proximity fused rockets to cause asteroids to move. This will also damage them.
Spawning takes place as a cutscene from their Mothership. Not undocking from a Shergeo space station.
The waypoint they spawn you at may or may not be a place you have never downloaded. It can be completely random.
Instead of needing to meet a quota, you just mine as much as you can. Progress at this point is measured based on a "Grand Quota" (Based on how many radioactive materials you gather over a series of days). However, meeting a daily quota will earn you rewards,
like "Repair Stamps" for free repairs.
Ads are also based on grand-quota progress.
To end a run, call the mothership. This will alert every nearby MISSILEBOT, as it takes 2 minutes for the mothership to arrive. If oxygen reaches 2.5 minutes, automatically call the mothership. The player must actually dock with the mothership for the day to complete.

After the grand quota is over, we initiate the ending

Input System:

Unity default input system. All keybinds should be bindable, of course, with any input device.
Pictures/icons for all input keys for the most common devices should be available for use in settings menus and hint/tutorial pop-ups.

Tutorial System:

A tutorial manager and a trigger system work together to progress the player via radio messages through an interactive sequence to familiarize them with the game. Skip-aheads and going in the wrong direction (which can also be detected with physical triggers)
should stop the speaker and transition seamlessly into a line meant to respond to their actions. Should also support pop-ups and hints to show key binds or advice after messages or if the player seems to be stuck.

Game Load Sequence:

Splash Screen -> Loading Screen -> Main Menu -> Choose tutorial, settings, new game, resume game, credits, or quit game -> Level Load -> Data Load -> Day -> Dock -> Market -> Day ...

KEEP TRACK OF ALL BORROWED ASSETS AND LICENSES:

Skybox: alexandr-melas on cgtrader. Royalty-free license. Available for use in commercial works.
Viper/Hornet Font: 0x408 on GitHub. MIT license. 
