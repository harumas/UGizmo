# UGizmo

Highly efficient gizmo drawer for Unity.

[日本語版](https://github.com/harumas/UGizmo/blob/main/README_JA.md)

[![license](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE.md)
[![license](https://img.shields.io/badge/PR-welcome-green.svg)](https://github.com/HarumaroJP/UGizmo/pulls)
[![license](https://img.shields.io/badge/Unity-2021.3-green.svg)](#要件)

![UGizmo-02](https://github.com/harumas/UGizmo/assets/43531665/81bb8462-d39a-4009-a634-dea451ab5f17)

## Overview

UGizmo is a library that adds various features to the standard Gizmos class and allows calls at runtime and the Unity event functions.
Also, by using GPU instancing to reduce draw calls, you can draw gizmos efficiently.

## Key features

- Available to be called by event functions such as Update(), LateUpdate(), etc.
- Runtime support
- Draw call optimization using GPU instancing
- More than 30 gizmos

## Getting Started

### Requirements
- Unity 2021.3 or higher
- URP or HDRP (Built-in Render Pipeline is not supported)

### Installation
You can enter the following link in the Package Manager of Unity

```
https://github.com/harumas/UGizmo.git?path=src/UGizmo/Assets/UGizmo
```

or open Packages/manifest.json and add the following to the dependencies block

```json
{
    "dependencies": {
        "com.harumaro.ugizmo": "https://github.com/harumas/UGizmo.git?path=src/UGizmo/Assets/UGizmo"
    }
}
```

### Samples
It can be drawn by using the `UGizmos` class.  
In the standard `Gizmos` class, the matrix is specified using `Gizmos.matrix`, but in UGizmo, the position, rotation, and scale information is passed as method arguments.
Also, the color should not be specified using `Gizmos.color`, but directly passing the Color structure as the method argument.

```C#
using UnityEngine;

public class DrawCubeSample : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        UGizmos.DrawCube(Vector3.zero, Quaternion.Euler(45f, 45f, 45f), Vector3.one, Color.red);
    }
}
```

Open Package Manager > UGizmo > Samples to install samples for URP and HDRP.

![image](https://github.com/harumas/UGizmo/assets/43531665/a321d8b5-0ef9-40a5-b24b-cd901af8f8f1)

## Features

### 3D Primitives
| 名前 | 補足 |
| --- | --- |
| `DrawSphere()` | Draws a sphere. (double the number of vertices as standard) |
| `DrawWireSphere()` | Draws a wireframe sphere. (double the number of vertices as standard) |
| `DrawCube()` | Draws a cube. |
| `DrawWireCube()` | Draws a wireframe cube. |
| `DrawCapsule()` | Draws a capsule. |
| `DrawWireCapsule()` | Draws a wireframe capsule. |
| `DrawCylinder()` | Draws a cylinder. |
| `DrawWireCylinder()` | Draws a wireframe cylinder. |
| `DrawCone()` | Draws a cone. |
| `DrawWireCone()` | Draws a wireframe cone. |
| `DrawPlane()` | Draws a plane. |
| `DrawWirePlane()` | Draws a wireframe plane. |

### 2D Primitives
| 名前 | 補足 |
| --- | --- |
| `DrawCircle2D()` | Draws a circle. |
| `DrawWireCircle2D()` | Draws a wireframe circle. |
| `DrawBox2D()` | Draws a box. |
| `DrawWireBox2D()` | Draws a wireframe box. |
| `DrawTriangle2D()` | Draws a triangle. |
| `DrawWireTriangle2D()` | Draws a wireframe triangle. |
| `DrawCapsule2D()` | Draws a capsule. |
| `DrawWireCapsule2D()` | Draws a wireframe capsule. |

### Utilites
| 名前 | 補足 |
| --- | --- |
| `DrawPoint()` | Draws a point to the front. |
| `DrawLine()` | Draws a line. |
| `DrawLineList()` | Draws multiple lines between pairs of points. |
| `DrawLineStrip()` | Draws a line between each point within the specified span. |
| `DrawRay()` | Draws a line from start (starting point) to start + dir (starting point + direction) |
| `DrawFrustum()` | Draws the frustum of the camera. |
| `DrawDistance()` | Displays the distance from a to b. Distance is not displayed at runtime. |
| `DrawMeasure()` | Draws lines separated by regular intervals |
| `DrawArrow()` | Draws a 3D arrow. |
| `DrawArrow2d()` | Draws a 2D arrow. |
| `DrawFacingArrow2d()` | Draws a 2D arrow. Always face the camera. |
| `DrawWireArrow()` | Draws a 2D arrow with a wire body. |
| `DrawFacingWireArrow()` | Draws a 2D arrow with a wire body. Always face the camera. |

### 3D Physics
| 名前 | 補足 |
| --- | --- |
| `Raycast()` | Performs a raycast and draws the result. |
| `DrawRaycast()` | Draw the result of the raycast by passing a RaycastHit structure. |
| `Linecast()` | Performs a line cast and draws the result. |
| `DrawLinecast()` | Pass a RaycastHit structure to draw the result of the line cast. |
| `SphereCast()` | Performs a sphere cast and draws the result. |
| `DrawSphereCast()` | Pass a RaycastHit structure to draw the result of the sphere cast. |
| `BoxCast()` | Performs a box cast and draws the result. |
| `DrawBoxCast()` | Pass a RaycastHit structure to draw the result of the box cast. |
| `CapsuleCast()` | Performs a capsule cast and draws the result. |
| `DrawCapsuleCast()` | Pass a RaycastHit structure to draw the result of the capsule cast. |

### 2D Physics
| 名前 | 補足 |
| --- | --- |
| `Raycast2D()` | Performs a 2D raycast and draws the result. |
| `DrawRaycast2D()` | Pass a RaycastHit structure to draw the result of the raycast. |
| `Linecast2D()` | Performs a 2D linecast and draws the result. |
| `DrawLinecast2D()` | Pass a RaycastHit structure to draw the result of the line cast. |
| `SphereCast2D()` | Performs a 2D sphere cast and draws the result. |
| `DrawSphereCast2D()` | Pass a RaycastHit structure to draw the result of the sphere cast. |
| `BoxCast2D()` | Performs a 2D box cast and draws the result. |
| `DrawBoxCast2D()` | Pass a RaycastHit structure to draw the result of the box cast. |
| `CapsuleCast2D()` | Performs a 2D capsule cast and draws the result. |
| `DrawCapsuleCast2D()` | Pass a RaycastHit structure to draw the result of the capsule cast. |

## License
This library is released under the MIT License. 
[MIT License](/LICENSE.md)
