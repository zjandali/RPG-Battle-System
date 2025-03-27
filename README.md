# Astroblox Unity RPG Battle System

This project implements a simple auto-battle RPG system in Unity as specified in the Astroblox Unity Take Home assignment. The system features configurable agents with shared stats, a time-based action system, various action types and effects, and a simple UI for visualization.

## Development Process

### 1. Project Setup (0h 30m)
- Created project directory structure for Unity version 2022.3.13f1
- Set up folders for Scripts, Prefabs, Scenes, and ScriptableObjects
- Planned the implementation approach and component architecture

### 2. Agent Data Model (1h 00m)
- Created the base Agent class with shared stats (HP, attack, defense, speed)
- Implemented Player and Enemy derived classes with different behavior patterns
- Created AgentConfig ScriptableObject for configurable agent data
- Implemented Party system to manage groups of agents

### 3. Turn-based Action System (0h 45m)
- Implemented BattleManager to control battle flow
- Created time-based action queuing based on agent speed
- Developed framework for different action types
- Implemented action execution system

### 4. Action Types and Effects (0h 45m)
- Implemented various action types:
  - Direct damage with defense consideration
  - Damage over time with timed intervals
  - Direct healing
  - Healing over time with timed intervals
  - Buffs for attack, defense, and speed with duration
  - Debuffs for attack, defense, and speed with duration
- Created Effect system to manage active effects on agents

### 5. Battle UI and Visualization (0h 45m)
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
- Players prioritize healing allies when needed
- Enemies have configurable aggressiveness that affects action selection
- Battle runs without player intervention until one side wins

## Time Tracking

| Task | Time Spent | Description |
|------|------------|-------------|
| Project Setup | 0h 30m | Creating project structure and initial planning |
| Agent Data Model | 1h 00m | Implementing agent classes, stats, and party system |
| Turn-based Action System | 0h 45m | Creating battle manager and action queuing |
| Action Types and Effects | 0h 45m | Implementing various actions and effects |
| Battle UI and Visualization | 0h 45m | Creating UI components and visualization |
| Testing and Integration | 0h 45m | Testing and ensuring all systems work together |
| Documentation | 0h 30m | Documenting development process and design choices |
| **Total** | **4h 00m** | Complete implementation of the RPG battle system |
