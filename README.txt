VPF0 Viewer and Converter by freem (http://www.ajworld.net/)
================================================================================
Requires .NET 2.0 framework. Might work in Wine on Linux and Mac OS X.

================================================================================
Introduction
================================================================================
This program allows you to view VPF0 files ripped from Fire Pro Returns's
FPR.ROM file, as well as export the images to PNG.

This version is WIP for various reasons, including, but not limited to:
* not knowing what all the bytes of the VPF0 format do
* not having a good way to extract VPF0 files from FPR.ROM
* certain images have odd colors; this is especially notable in 8bpp images.

================================================================================
Usage
================================================================================
Upon opening the program, you will see a menu bar and a blank window.

Here is a crude ASCII diagram of the main window:
+------------------------------+
|@ VPF0 Viewer              _#X|
+------------------------------+
| File  Help                   | <- Menu bar
+------------------------------+
|                              |
|                              |
|                              |
|        image  display        |
|                              |
|                              |
|                              |
+------------------------------+

[File menu]
Open - Select a VPF0 file for opening.
Save as PNG - Export the currently loaded image to a PNG file.
Exit - Get out of here!

[Help menu]
Only contains the About dialog.

================================================================================
Changelog
================================================================================
legend:
! bugfix
+ addition
* notice/neutral
- removal
~ internal thing

v0.0.2 - (in development)
! Fixed bug when trying to save a PNG without loading a file.
+ Added support for 24bpp VPF0 images.
+ Allow for dragging and dropping files onto the window.
+ Added the filename to the title bar after opening a file.
+ Added a statusbar, which also notes the open file and its BPP mode.

v0.0.1 - 2016/10/22
* Initial release, proof of concept.

================================================================================
Planned Features
================================================================================
* PNG -> VPF0 conversion???

================================================================================
License
================================================================================
This program is released under the MIT license.
Please see the LICENSE file for more information.
