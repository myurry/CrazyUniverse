# Crazy Universe - Vol.3 CPU Simulation Guide

This guide outlines how to test and verify the multi-object CPU simulation features, error resilience, and task cancellation introduced in Vol.3.

## Prerequisites
1. Launch the application (`CrazyUniverse.WpfApp`).
2. Ensure you have at least **3 animals** in your repositories (the updated starter seed provides 5 of each type by default).

---

## Testing Instructions

### 1. Sequential Mode Demonstration
* **Steps:** 
  1. Check the selection boxes (`IsSelectedForSimulation`) for **at least 3 animals** in the list.
  2. Select a workload size from the dropdown (e.g., *Heavy / >5s (300M)*).
  3. Click **Run Sequential**.
* **Expected Result:** The activity log displays `[Report - Sequential Mode]`, listing each animal's name, final state, success status, and individual duration one after another, followed by the total execution time.

### 2. Parallel Mode Demonstration
* **Steps:**
  1. Keep the same 3+ animals checked.
  2. Click **Run Parallel**.
* **Expected Result:** The activity log displays `[Report - Parallel Mode]`. Notice that the **Total Execution Time** is significantly faster than the sequential run because all computations execute concurrently via `Task.Run` and `Task.WhenAll`. The UI remains fully responsive throughout.

### 3. Error Handling Demonstration
* **Steps:**
  1. Add a new animal (or rename one) with the name `"Error"` (or ensure one of your selected animals triggers a simulated exception).
  2. Select this animal along with 2 healthy animals.
  3. Run either Sequential or Parallel mode.
* **Expected Result:** The report captures the exception message for `"ErrorTest"` (`IsSuccessful = false`), but **successfully finishes** the remaining healthy animal computations without crashing the app.

### 4. Cancellation Demonstration
* **Steps:**
  1. Select 3 or more animals.
  2. Choose a larger workload (e.g., *Massive (500M)*).
  3. Click **Run Parallel** (or Sequential).
  4. Immediately click the **Cancel** button while it is running.
* **Expected Result:** The shared `CancellationToken` aborts the remaining CPU cycles cleanly, stopping execution and logging the cancellation notice.