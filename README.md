# LSDView
Data file viewer for LSD Dream Emulator.

## Usage
- LSDView can load and view many files from PlayStation 1 game 'LSD: Dream Emulator'
- Currently supported file formats are:
  - TMD (PlayStation models)
  - TIM (PlayStation textures)
  - TIX (Archives of TIM files, used to load collections of textures into VRAM)
  - LBD (Sections of levels in LSD, also contains models in level with animations)
  - MOM (Containers for 3D meshes and their animations)
- Controls:
  - The 3D view can be rotated (in an Arcball fashion) by clicking and dragging
  - Scroll wheel can be used to zoom in and out
  - Flycam mode can be toggled with Z
  - Flycam mode allows you to fly around with W, A, S, D, and arrow keys
  
## Dependencies
- https://github.com/stefangordon/ObjParser
- https://github.com/Figglewatts/libLSD

## Quick start
1. Clone the repo
2. Clone https://github.com/Figglewatts/libLSD
3. Build LibLSD
4. Clone https://github.com/stefangordon/ObjParser
5. Build ObjParser
4. Build LSDView
5. Place LibLSD.dll and ObjParser.dll next to LSDView.exe in the build folder
