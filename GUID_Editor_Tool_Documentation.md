## GUID Generator Tool

**File**: `GuidGeneratorTool.cs`  
**Type**: `UnityEditor.EditorWindow`  

### Purpose

- Provide a quick, editor-only way to generate **new GUID strings** for use as gameplay/data identifiers.
- Allow copying the generated GUID to the system clipboard.
- Allow logging the GUID to the Unity Console **once per GUID** (prevents duplicate console spam during the same window session).

### 1) Open window

- **Given** Unity Editor is running  
- **When** the user clicks `Tools > GUID Generator`  
- **Then** a window titled **“GUID Generator”** opens (or focuses if already open).

### 2) Generate GUID

- **Given** the window is open  
- **When** the user clicks **Generate GUID**  
- **Then** `guid` is set to `System.Guid.NewGuid().ToString()` and displayed in the UI.

### 3) Display GUID

- **Given** `guid` is non-empty  
- **When** the UI draws  
- **Then** the GUID is visible in a selectable label so the user can highlight/copy it manually.

### 4) Copy GUID

- **Given** `guid` is non-empty  
- **When** the user clicks **Copy GUID**  
- **Then** `EditorGUIUtility.systemCopyBuffer` becomes the current GUID.

- **Given** `guid` is empty  
- **When** the UI draws  
- **Then** the copy/log buttons are not shown (tool early-returns until a GUID exists).

### 5) Log GUID (single-use per GUID)

- **Given** `guid` is non-empty  
- **And** the current GUID has not been logged yet in this window session  
- **When** the user clicks **Log to console**  
- **Then**:
  - The GUID is logged via `Debug.Log(guid)`.
  - The GUID is added to an in-memory `loggedIds` list.

- **Given** the current GUID is already present in `loggedIds`  
- **When** the UI draws  
- **Then** the **Log to console** button is not shown (prevents duplicate logs).

### 6) Session reset

- **Given** one or more GUIDs have been logged  
- **When** the window is closed/disabled (`OnDisable`)  
- **Then** `loggedIds` is cleared (no persistence between sessions).

