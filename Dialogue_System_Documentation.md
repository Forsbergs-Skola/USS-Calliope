# Dialogue System Documentation

## Overview

This document describes the dialogue system implementation for the USS Calliope Unity project. The system provides a flexible, data-driven approach to managing in-game conversations using Unity ScriptableObjects, an event-driven architecture, and integration with the project's UI management system.

## Architecture

The dialogue system follows a modular architecture with clear separation of concerns:

- **Data Layer**: ScriptableObjects define dialogue content (lines, conversations, catalogs)
- **Controller Layer**: `DialogueController` manages conversation lifecycle and lookup
- **Presentation Layer**: `DialogueCanvas` handles UI rendering and user interaction
- **Integration Layer**: Event system for decoupled communication between components

## Core Components

### 1. ScriptableObject Data Structures

#### `DialogueLineSO`
Represents a single line of dialogue spoken by a character.

**Location**: `Assets/Brad/Scripts/Dialogue/DialogueLineSO.cs`

**Properties**:
- `lineID` (string): Unique identifier (auto-generated GUID if empty)
- `speakerName` (string): Name of the character speaking
- `lineText` (string): The dialogue text (multiline support)
- `characterRevealInterval` (float): Time delay between character reveals (0.001-1.0 seconds)
- `portraitTexturePath` (string): Resource path to character portrait texture

**Features**:
- Automatic GUID generation in `OnValidate()` if `lineID` is empty
- Editor integration to mark asset as dirty when auto-generating IDs

#### `DialogueConversationSO`
Contains a sequence of dialogue lines that form a complete conversation.

**Location**: `Assets/Brad/Scripts/Dialogue/DialogueConversationSO.cs`

**Properties**:
- `convoID` (string): Unique conversation identifier (auto-generated GUID if empty)
- `lines` (List<DialogueLineSO>): Ordered list of dialogue lines

**Validation**:
- Ensures at least one line exists in the conversation
- Auto-generates GUID for `convoID` if empty

#### `ConversationCatalogSO`
Central registry of all available conversations in the game.

**Location**: `Assets/Brad/Scripts/Dialogue/ConversationCatalogSO.cs`

**Properties**:
- `conversations` (List<DialogueConversationSO>): Collection of all conversations

**Purpose**: Provides a single point of reference for all dialogue content, enabling efficient lookup and management.

### 2. Controller: `DialogueController`

**Location**: `Assets/Brad/Scripts/Dialogue/DialogueController.cs`

**Pattern**: Singleton (inherits from `Singleton<DialogueController>`)

**Responsibilities**:
- Maintains reference to `ConversationCatalogSO` containing all conversations
- Provides public API to start conversations by ID
- Listens to dialogue events for lifecycle management
- Manages dialogue canvas visibility

**Key Methods**:
- `StartConvoWithID(string convoID)`: Public API to initiate a conversation
  - Looks up conversation in catalog
  - Shows dialogue canvas via `UIController`
  - Launches conversation on the canvas

**[Event](Event_System_Documentation.md) Handlers**:
- `HandleConvoStartedEvent`: Placeholder for future conversation start logic
- `HandleConvoEndedEvent`: Removes dialogue canvas when conversation completes
- `HandleLineStartedEvent`: Placeholder for line start logic
- `HandleLineFinishedEvent`: Placeholder for line finish logic

### 3. UI Presentation: `DialogueCanvas`

**Location**: `Assets/Brad/Scripts/UI/DialogueCanvas.cs`

**Interface**: Implements `ICanvasUI` for integration with the UI management system

**[UI](UI_System_Documentation.md) Elements**:
- `dialogueText` (TMP_Text): Displays the dialogue line text
- `speakerNameText` (TMP_Text): Displays the speaker's name
- `continueButton` (Button): Button to advance to next line
- `portraitImage` (RawImage): Displays character portrait

**State Management**:
- `currentConvo`: Reference to active conversation
- `currentLineIdx`: Index tracking current line in conversation

**Key Features**:

1. **Game Pausing**: Automatically pauses game time when dialogue canvas is enabled, resumes when disabled
2. **Character-by-Character Reveal**: `LineRevealer` coroutine displays text character-by-character with configurable timing
3. **Portrait Loading**: Dynamically loads portrait textures from Resources folder
4. **Event Integration**: Triggers events at conversation and line boundaries

**Methods**:
- `LaunchConversation(DialogueConversationSO convo)`: Initiates a conversation
- `AdvanceDialogue()`: Progresses to next line or ends conversation
- `LineRevealer()`: Coroutine that animates text reveal
- `HandlePortrait(string resourcePath)`: Loads and displays character portrait

**Lifecycle**:
- `OnEnable()`: Pauses game, sets up button listener, hides continue button
- `OnDisable()`: Unloads unused assets, removes listeners, resumes game

### 4. [UI](UI_System_Documentation.md) Integration: `UIController`

**Location**: `Assets/Brad/Scripts/UI/UIController.cs`

The dialogue system integrates with the project's centralized UI management system through the `ICanvasUI` interface. This provides:

- Canvas lifecycle management (show/hide/remove)
- Sorting order management (foreground/background)
- Prefab-based instantiation
- Canvas state queries

**Integration Points**:
- `DialogueCanvas` implements `ICanvasUI` interface
- `DialogueController` uses `UIController.Instance` to show/remove dialogue canvas
- Canvas is registered with `EnumCanvasUIName.DIALOGUE`

## [Event System](Event_System_Documentation.md) Integration

The dialogue system uses an event-driven architecture for decoupled communication.

**Event Types** (defined in `UIEvents.cs`):
- `DialogueConvoStartedEvent` (StringPayloadEvent): Fired when conversation begins
- `DialogueConvoFinishedEvent` (StringPayloadEvent): Fired when conversation ends
- `DialogueLineStartedEvent` (StringPayloadEvent): Fired when a line begins displaying
- `DialogueLineFinishedEvent` (StringPayloadEvent): Fired when a line finishes displaying

**Event Flow**:
1. `DialogueCanvas.LaunchConversation()` triggers `DialogueConvoStartedEvent`
2. Each line triggers `DialogueLineStartedEvent` at start of reveal
3. Each line triggers `DialogueLineFinishedEvent` when reveal completes
4. `DialogueConvoFinishedEvent` triggers when last line is advanced

**Event Listeners**:
- `DialogueController` subscribes to all dialogue events for lifecycle management
- Other systems can subscribe to these events for gameplay integration (e.g., quest triggers, cutscenes)

## System Flow

### Starting a Conversation

1. External system calls `DialogueController.Instance.StartConvoWithID(convoID)`
2. `DialogueController` looks up conversation in `ConversationCatalogSO`
3. If found, `UIController` instantiates and shows dialogue canvas
4. `DialogueCanvas.LaunchConversation()` is called with conversation data
5. Conversation start event is triggered
6. First line begins displaying

### Displaying a Line

1. `AdvanceDialogue()` increments line index
2. Line data is extracted from `DialogueLineSO`
3. Portrait is loaded if path is provided
4. Speaker name is displayed
5. `LineRevealer` coroutine begins character-by-character reveal
6. Line start event is triggered
7. Continue button is hidden during reveal
8. When reveal completes, continue button is shown
9. Line finish event is triggered

### Ending a Conversation

1. User clicks continue on final line
2. `AdvanceDialogue()` detects end of conversation
3. Conversation finish event is triggered
4. `DialogueController` receives event and removes dialogue canvas
5. Game time resumes

## Asset Organization

Dialogue assets are organized in `Assets/Dialogue/`:

- **Conversation Catalog**: `Assets/Dialogue/ConversationCatalogSO.asset` - Main registry
- **Conversation Folders**: Each conversation has its own folder (e.g., `MyFirstConvo/`)
  - Contains the conversation ScriptableObject
  - Contains all dialogue line ScriptableObjects referenced by the conversation

**Example Structure**:
```
Assets/Dialogue/
├── ConversationCatalogSO.asset
└── MyFirstConvo/
    ├── FreshPrinceConvo.asset (DialogueConversationSO)
    ├── Line00.asset (DialogueLineSO)
    ├── Line01.asset (DialogueLineSO)
    └── ...
```

## Scene Integration

### Bootstrap Scene
The `Bootstrap.unity` scene contains:
- `Bootstrapper` GameObject: Manages game state and scene transitions
- `EventRelay` GameObject: Central event system hub
- `UIController` and `DialogueController` are instantiated as singletons

### Test Scene
`Assets/Brad/Junk/TestScene.unity` is used for testing dialogue functionality.

## Usage Example

```csharp
// Start a conversation from anywhere in the game
DialogueController.Instance.StartConvoWithID("7c81000f-94a1-4062-b5e4-bcca0bbe8f2b");
```

The system handles:
- Canvas instantiation
- Conversation lookup
- UI presentation
- User interaction
- Cleanup

## Design Patterns and Principles

1. **Singleton Pattern**: `DialogueController` uses singleton for global access
2. **ScriptableObject Pattern**: Data-driven design separates content from code
3. **Observer Pattern**: Event system enables decoupled communication
4. **Interface Segregation**: `ICanvasUI` interface for UI integration
5. **Separation of Concerns**: Clear boundaries between data, logic, and presentation

## Future Extensibility

The system is designed with extensibility in mind:

- Event handlers in `DialogueController` are placeholders for future features
- Commented fields in `DialogueLineSO` suggest planned features (quit flags, custom events)
- Event system allows other systems to react to dialogue without tight coupling
- Portrait system supports dynamic character art loading

## Technical Notes

- Uses `WaitForSecondsRealtime` for text reveal to work correctly when game is paused
- Resources folder is used for portrait loading (path: `Resources/DialoguePortraitTextures/`)
- GUID-based IDs ensure unique identification across project
- Editor integration (`OnValidate`) provides automatic ID generation
- Canvas sorting order managed through `UIController` constants

## Dependencies

- Unity TextMeshPro for text rendering
- Project's event system (`EventRelay`, `StringPayloadEvent`)
- Project's UI management system (`UIController`, `ICanvasUI`)
- Project's singleton base class
- `Bootstrapper` for game pause functionality

