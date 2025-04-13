# AE Horror Prototype

A first-person horror game prototype with focus on interaction and state management systems.

## What's Implemented

- **Player Interaction System**: Raycast-based object detection with crosshair feedback
- **Input System**: Context-aware input handling using Unity's Input System
- **Camera Effects**: Custom Cinemachine behaviors for horror atmosphere
- **Game State Machine**: Clean separation between gameplay modes
- **Ghost Chase**: Basic jumpscare and game over sequence
- **Utilities Manager**: Centralized reference and helper functions
- **Delivery Zone**: Simple object placement puzzle system

## Planned Features

- Refactor Utilities into proper services
- RiddleSystem needs improvements
- Complete Game Over screen UI
- Add a reusable Sequence System for scripted moments
- Polish interaction feedback (visual, audio)
- Expand dialogue system with conditions
- Create developer tools for faster testing

## Design Choices

- **State Management**: Separated game states keep code clean and testable
- **Camera System**: Built on Cinemachine to leverage Unity's tools
- **Input Handling**: Action maps for different contexts (gameplay, inspection, UI)
- **Event System**: Loose coupling through events rather than direct references
- **Quick Prototyping**: Some systems built for speed, marked for later refactoring