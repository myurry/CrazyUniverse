# Crazy Universe - Animal Sanctuary

## 1. Overview
A humorous WPF application where different animals live in a shared world with unique behaviors, interfaces, and randomized crazy actions. The project incorporates typed repositories, dynamic greetings, and reactive behavior handling.

## 2. How to Run
1. Clone the repository and open the solution file (`.slnx`) in Visual Studio 2026.
2. Set `CrazyUniverse.WpfApp` as the startup project.
3. Press `F5` to build and run.

## 3. Architecture & Class Hierarchy
- **Repositories**: Distinct repositories for each animal type (`Horse`, `Cow`, `Monkey`, `Bird`), implementing generic `Repository<T>` and a non-generic `IAnimalRepository` interface.
- **Animal (Abstract base class)**: Manages shared properties (`Name`, `EnergyLevel`), standard sound behavior, and the abstract `CrazyAction()`. State encapsulation protects identity and energy mutations.
- **Subclasses**: 
  - `Cow` & `Horse`: Unique sounds and actions.
  - `Monkey`: Implements `IClimbable` and `ISwimmable`.
  - `Bird`: Implements `IFlyable`.

## 4. State, Greeting, & Reacting Logic
- **Validation**: Input names cannot be blank, and energy levels are safely bounded between 0 and 100.
- **Greeting & Reacting**: Animals feature reactive behaviors where actions and greetings trigger cross-entity notifications and event handling within the sanctuary ecosystem.
- **CrazyAction()**: Polymorphic execution of chaotic effects (e.g., chocolate milk production, cloning, system log wiping, or fake countdowns) relying on object polymorphism rather than large conditional type-checking blocks.

## 5. Use Cases
1. Add a new animal with a valid name and energy level into its respective repository tab.
2. Enter invalid data (like empty names) to test safe error handling.
3. Select an animal to trigger its standard sound and greeting.
4. Execute `CrazyAction()` to modify the animal's state, view real-time log updates, and trigger reactive behaviors.
5. Select a Monkey or Bird to enable and test interface-specific actions via the UI Expander panel.

## 6. WPF UI Element Rationale
An `Expander` is used to neatly organize and toggle visibility for special interface abilities (`IClimbable` / `IFlyable` / `ISwimmable`), keeping the main detail panel clean and uncluttered.
