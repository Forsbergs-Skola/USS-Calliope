## GUID Generator Tool

**File**: `GuidGeneratorTool.cs`  
**Type**: `UnityEditor.EditorWindow`  
**File Path**: `Editor/GUID Generator`

### Technical Intent

- Generate **GUID strings** inside the editor using `System.Guid.NewGuid()`.
- Maintain the **current GUID value** in window state for subsequent operations (copy, log).
- Track which GUIDs have already been written to the console in this window instance to **avoid repeated logs** of the same identifier.

### Internal State & Structure

- **Fields**
  - `string guid`  
    - Holds the currently generated GUID string.  
    - Empty string (`""`) means “no GUID generated yet”.
  - `List<string> loggedIds`  
    - In-memory cache of GUIDs that have already been logged via `Debug.Log`.  
    - Lifetime is tied to the editor window instance; cleared on `OnDisable`.
- **Construction / Opening**
  - `[MenuItem("Tools/GUID Generator")]` exposes `OpenWindow()`, which calls `GetWindow<GuidGeneratorTool>("GUID Generator")`.  
    - Reuses an existing instance if present; otherwise, creates a new one.

---

## TDD / Expected Behavior 

### 1) Open window

- **Given** Unity Editor is running  
- **When** the user clicks `Tools > GUID Generator`  
- **Then** a window titled **“GUID Generator”** opens (or focuses if already open).

### 2) Generate GUID (state mutation)

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
