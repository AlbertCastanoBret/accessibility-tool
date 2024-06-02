# Introduction
This technical document outlines the necessary steps to configure essential tools and libraries for developing games and applications in Unity. The libraries included in this document are the Universal Render Pipeline (URP) and the new Input System. The process to install the specific package for the developed tool is also described.

## Prerequisites
Before starting, ensure that you have Unity Hub and the latest version of the Unity editor installed. It is recommended to use version 2022.3.14f1 or later to ensure compatibility with the mentioned libraries.

## Installation of Necessary Libraries

### Universal Render Pipeline (URP)
To configure URP in your Unity project, follow these steps:
1. Open Unity Hub and create a new project or open the existing project where you want to install URP.
2. Install URP from the Package Manager:
   - Go to `Window > Package Manager`.
   - In the package list, select `Unity Registry`.
   - Search for `Universal RP` and click `Install`.

The URP asset that is imported by default already includes the necessary basic configurations. However, the developer can add their own URP asset as long as the rendering paths of the renderers are set to "Forward".

### Input System
To install and configure the new Input System, follow these steps:
1. Install the Input System from the Package Manager:
   - Go to `Window > Package Manager`.
   - In the package list, select `Unity Registry`.
   - Search for `Input System` and click `Install`.
2. Configure the Input System:
   - Once installed, Unity will prompt you to change the active input handling. Click `Yes` to switch to the new Input System.
   - Go to `Edit > Project Settings > Player`.
   - In the `Active Input Handling` section, select `Input System Package (New)` or `Both` if you want to maintain compatibility with the old input system.

## Installation of the Developed Tool Package
1. Prepare the package:
   - Ensure that the developed package is compressed into a .unitypackage file or available through a repository.
2. Install the package from a .unitypackage file:
   - Go to `Assets > Import Package > Custom Package`.
   - Select the .unitypackage file of your tool.
   - In the import window, make sure all checkboxes are selected and click `Import`.
