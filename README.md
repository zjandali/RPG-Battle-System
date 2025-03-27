# Astroblox Unity RPG Battle System

Breakdown of how I implemented the RPG battle system in Unity as specified in the Astroblox Unity Take Home assignment. 
## Development Process

### 1. Project Setup (0h 15m)
- Downloaded Unity 2022.3.13f1
- Created project directory structure for Unity version 2022.3.13f1
- Planned the implementation approach and component architecture

### 2. Agent Data Model (1h 15m)
- Created the base Agent class with shared stats (HP, attack, defense, speed)
- Implemented Player and Enemy derived classes with different behavior patterns
- Created AgentConfig ScriptableObject for configurable agent data
- Implemented Party system to manage groups of agents

### 3. Turn-based Action System (1h 00m)
- Implemented BattleManager to control battle flow
- Created time-based action queuing based on agent speed
- Developed framework for different action types
- Implemented action execution system

### 4. Action Types and Effects (1h 00m)
- Implemented various action types:
  - Direct damage with defense consideration
  - Damage over time with timed intervals
  - Direct healing
  - Healing over time with timed intervals
  - Buffs for attack, defense, and speed with duration
  - Debuffs for attack, defense, and speed with duration
- Created Effect system to manage active effects on agents

### 5. Battle UI and Visualization (1h 15m)
- Created BattleUIManager to handle overall UI
- Implemented AgentUI component for individual agent visualization
- Added health bars and action readiness indicators
- Created visualization for buff/debuff effects with timers
- Implemented battle result UI for win/loss conditions

### 6. Testing and Integration (0h 45m)
- Created default agent configurations for testing
- Implemented BattleSystemTest script to verify functionality
- Created UIPrefabCreator to generate UI elements programmatically
- Developed MainSceneSetup for automatic scene configuration
- Verified all systems work together properly

## Design Choices

### Object-Oriented Architecture
I chose a robust object-oriented design with clear separation of concerns:
- **Base Agent Class**: Provides shared functionality for all agents
- **Player/Enemy Classes**: Extend base agent with specific behavior patterns
- **Effect System**: Handles all time-based effects with a common interface
- **Action System**: Provides a framework for different action types

### Scriptable Objects for Configuration
Used ScriptableObjects for agent configuration to allow:
- Easy creation of different agent types
- Reusable configurations
- Separation of data from behavior

### Component-Based Design
Implemented a component-based architecture for extensibility:
- Each major system is a separate component
- Components communicate through well-defined interfaces
- Easy to add new features or modify existing ones

### Observer Pattern for Events
Used the observer pattern for battle events and UI updates:
- BattleManager broadcasts events (battle start, end, victory, defeat)
- UI components subscribe to relevant events
- Agent components broadcast state changes (damaged, healed, buffed, debuffed)
- Loose coupling between systems

### Auto-Battle System
Implemented an auto-battle system as specified:
- Agents automatically choose actions based on their available action types
- Agents prioritize healing allies when needed
- Battle runs without player intervention until one side wins

## Time Tracking

| Task | Time Spent | Description |
|------|------------|-------------|
| Project Setup | 0h 15m | Creating project directory structure and initial planning |
| Agent Data Model | 1h 15m | Implementing agent classes, stats, and party system including Player and Enemy behavior patterns |
| Turn-based Action System | 1h 00m | Creating battle manager and action queuing with time-based mechanics |
| Action Types and Effects | 1h 00m | Implementing various actions and effects including DoT and HoT systems |
| Battle UI and Visualization | 1h 15m | Creating UI components, battle timer, health bars, and battle result visualization |
| Testing and Integration | 0h 45m | Testing, debugging, and ensuring all systems work together properly |
| **Total** | **5h 30m** | Complete implementation of the RPG battle system |

## Technical Implementation Details

### Battle Timer System
The battle system includes a sophisticated timer that:
- Tracks elapsed battle time with millisecond precision
- Supports time scaling for different battle speeds
- Provides formatted time display (MM:SS.MS)
- Integrates with the main UI canvas system

### UI System Architecture
- Uses TextMeshPro for high-quality text rendering
- Dynamically creates UI elements through the UIPrefabCreator
- Implements separate containers for player and enemy UI elements
- Shows real-time battle state including health, effects, and action readiness

## Dependencies
- Unity 2022.3.13f1
- Unity UI package (com.unity.ugui@1.0.0) - For creating the game interface
- TextMesh Pro (com.unity.textmeshpro@3.0.9) - For text rendering and formatting

## Setup Instructions
1. Clone this repository
2. Open the project in Unity 2022.3.13f1
3. Open the main scene from Scenes/BattleScene
4. Press Play to observe the auto-battle system in action

