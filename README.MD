# Crazy Universe - Animal Sanctuary

## 1. Overview
A humorous WPF application where different animals live in a shared world with unique behaviors, interfaces, and randomized crazy actions.

## 2. How to Run
1. Clone the repository and open `CrazyUniverse.sln` in Visual Studio 2022.
2. Set `CrazyUniverse.WpfApp` as the startup project.
3. Press `F5` to build and run.

## 3. Class Hierarchy & Interfaces
* **`Animal`** (Abstract base class): Manages shared properties (`Name`, `EnergyLevel`), standard sound behavior, and the abstract `CrazyAction()`.
* **`Cow` & `Horse`**: Subclasses with unique sound and crazy actions.
* **`Monkey`**: Implements `IClimbable` for climbing abilities.
* **`Bird`**: Implements `IFlyable` for flight capabilities.

## 4. State & Crazy Actions
* **Validation**: Input names cannot be blank, and energy must remain between 0 and 100. Invalid entries trigger a safe exception message without crashing the app.
* **CrazyAction()**: Each animal triggers a distinct chaotic effect (e.g., cow producing chocolate milk, horse duplicating 100 times, monkey hacking systems, or bird initiating a fake countdown).

## 5. Use Cases
1. Add a new animal with valid name and energy values.
2. Enter invalid data (like empty names) to test safe error handling.
3. Select an animal and trigger its standard sound.
4. Execute `CrazyAction()` to modify the animal's state and view the log message.
5. Select a `Monkey` or `Bird` to enable and test interface-specific actions via the UI.

## 6. WPF UI Element Rationale
An **`Expander`** is used to neatly organize and toggle visibility for special interface abilities (`IClimbable` / `IFlyable`), keeping the main detail panel clean and uncluttered.