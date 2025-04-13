## Project Timeline
April 9, 2025 (Evening)

Initial setup of the scene
Added player controller
Installed missing TextMeshPro package
Cleaned up repository (removed unneeded idea files and added to .gitignore)

April 10, 2025

Planning phase
Designed core systems:

Pause menu functionality
Game state management
Camera system
Audio management



April 11, 2025

Began implementing story progression
Initially designed for a flexible system capable of handling hours of gameplay
Due to time constraints, pivoted to a simpler approach:

Implemented a Blackboard with hardcoded keys
Created basic interactable objects



April 12, 2025

Reassessed and refined the riddle progression system
Added additional sound effects
Planned remaining implementation tasks
Focused on completing as much as possible before deadline

April 13, 2025

Final polishing
Enhanced horror elements
Sound refinements

Environmental Puzzle Implementation
The "one room horror" puzzle focuses on three main elements:

Swords
Candelabras
Skulls

Players can move around the scene and interact with these objects to solve the puzzle.
Features Implemented

First-person player movement and interaction system
Object interaction mechanics
Basic puzzle progression tracking
Horror atmosphere elements:

Lighting effects
Sound design
Simple visual effects

## Future Improvements
Given more time, I would implement the following improvements:

Audio Management:

Create a source generation system for audio files with enum typing for safer referencing


Puzzle System:

Develop an editor for the riddle system with global constants
Improve flexibility for future puzzle expansions


Player Experience:

Enhance player movement to better fit the horror atmosphere
Implement proper sound mixing
Expand UI elements and interactions


Visual Enhancements:

More advanced shader effects
Additional VFX for interactions
Dynamic lighting changes tied to puzzle progression

## Creating a New Riddle Step
To add a new step to the puzzle chain, you would:

Define Constants: Add any new key identifiers in RiddleConstant class if needed
Create or Modify an Interactable Object:

Extend InteractableItem or InspectableItem based on behavior needs
Set the Key and Condition properties in the inspector or code


Chain Logic:

The Key property defines what value will be set in the blackboard when interacting with this object
The Condition property defines what must be true in the blackboard before this interaction can succeed
Example format: "previous_step_key=expected_value"

Chain Example:
```cs
// Object 1 (starting point)
// Key: "skull_picking" 
// Condition: "" (empty, so always available)
// When interacted with, sets "skull_picking" = "skull1_picked"

// Object 2 (depends on Object 1)
// Key: "fire_progress"
// Condition: "skull_picking=skull1_picked"
// When interacted with, sets "fire_progress" = "candle1_lit"

// Object 3 (depends on Object 2)
// Key: "sword_placement"
// Condition: "fire_progress=candle1_lit"
// When interacted with, sets "sword_placement" = "sword1_placed"

// Final object (completes the puzzle)
// Key: "what_done_is_done"
// Condition: "sword_placement=sword1_placed"
// When interacted with, sets "what_done_is_done" = "time_to_die"
```