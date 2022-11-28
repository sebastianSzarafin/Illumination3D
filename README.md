# GK-Proj2
<p align="center">
  <img width="700" alt="1startup" src="https://user-images.githubusercontent.com/74315304/204162780-a3772a1f-4c2e-4c08-94f6-16bc0c598e25.png">
</p>
  
## Introduction
This is a project for *Computer Graphics 2022* course on Computer Science course. The application visualizes a 3D surface (in this example a hemisphere), which is illuminated by a simulated sun. The sun is orbiting back and forth above the object.
  
---
## Usage
User may interact with an app using the menu placed in a left column of the main window which has following functionalities:
- loading .obj file from user's computer
- showing mesh connecting all the displayed object's vertices 
<img width="300" alt="4mesh" src="https://user-images.githubusercontent.com/74315304/204166486-da63379b-e7e5-43d3-8ccf-2d94594161a8.png">

- overlapping the displayed object with an image
<img width="300" alt="5image" src="https://user-images.githubusercontent.com/74315304/204166621-c13a4db0-48c4-49c3-9a6d-9f64248cb048.png">

- changing the displayed object's normal map
<img width="300" alt="6normalmap" src="https://user-images.githubusercontent.com/74315304/204166656-820911d0-4d88-4835-878b-a649742ea35a.png">

- changing parameters used in painting the displayed object (more info in <b>Painting</b> section) <br>
- changing the speed of displayed object redraw method <br>
- playing/pausing the sun simulation and also modyfying it's height (z coordinate) <br>
- setting the displayed object's and sun's base color <br>
- switching between 2 ways of calculating pixel's color (more info in <b>Painting</b> section)
<p float="left">
<img width="200" alt="2interpolation" title="Interpolation" src="https://user-images.githubusercontent.com/74315304/204167449-5258c220-748e-4d62-a426-e12b475895e7.png">
&nbsp&nbsp&nbsp&nbsp
<img width="200" alt="3explicit" title="Explicit designation" src="https://user-images.githubusercontent.com/74315304/204167453-0715b918-c416-4d42-bb5e-01bfa4c254c4.png">
 <p>
   
 ---
## Painting
//TODO
   
   
---
## Algorithm
The *DrawObj* function from ObjFunctions.cs is the main part of the project. It involves the efficient scan line algorithm with edge bucket sort for filling out the object with a color.
   
   
## Assumptions
* Displayed object has to have it's functional respresentation $z = f(x,y)$ meaning it can be drawn in a 2D plane.
