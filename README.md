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
- Can be made zero-cost in release builds (draw calls are stripped at the IL level)
- Optional depth testing so gizmos can be occluded by scene geometry

### Draw call optimization

Using Standard Gizmos  
<img src="https://github.com/harumas/UGizmo/assets/43531665/34578ce8-c60a-4921-87fc-9aae99132c96" width="400px">

Using UGizmos  
<img src="https://github.com/harumas/UGizmo/assets/43531665/1cd23670-a982-4679-af8e-b3fa8f7d0bee" width="400px">


![image](https://github.com/harumas/UGizmo/assets/43531665/28b6614e-e610-4112-88f9-e0760babef3d)

### Zero-cost in release builds

All `UGizmos.DrawXXX` calls are marked with `[Conditional]`, so the compiler can remove them
**at the IL level**. The call and its arguments are never emitted, so there is no runtime cost
and no JIT — you can leave gizmo calls in your gameplay code without guarding them with `#if`.

Draw calls are kept in the following cases (any one is enough):

| Symbol | When it is defined |
| --- | --- |
| `UNITY_EDITOR` | In the Editor |
| `DEVELOPMENT_BUILD` | Development builds ("Development Build" checked) |
| `UGIZMO` | **Added for you when UGizmo is first imported**, so release builds draw gizmos out of the box |

Gizmos are therefore on by default in release builds too. To strip them and get the zero-cost
behaviour, untick **Tools > UGizmo > Keep Gizmos in Release Builds**. The menu adds or removes the
`UGIZMO` symbol for the build target currently selected in Build Settings, so you never have to
type it into Scripting Define Symbols by hand.

> [!NOTE]
> `UGIZMO` is a per-build-target setting, and it is added only for the target selected at import
> time. After switching platform, re-tick the menu item for the new target.

> [!NOTE]
> A release build without this symbol draws nothing and reports no error or warning while doing so,
> because the calls no longer exist. If gizmos are missing only in a build, check this first.

> [!NOTE]
> Because the arguments are stripped together with the call, do not put side effects inside
> a gizmo call (e.g. `UGizmos.DrawSphere(Step(), ...)`). Physics helpers such as
> `UGizmos.Raycast` return a value, so they always run; only their visualization is stripped.

### Depth testing

By default, gizmos ignore what is in front of them: wireframes and points always draw on top, and
solid primitives show their occluded parts faintly. Set `UGizmos.depthTest = true` to make every
gizmo respect the scene depth buffer instead, so anything behind other geometry is simply not
drawn.

```csharp
UGizmos.depthTest = true;
```

It is a global switch — there is no per-draw-call option. `false` restores the default look
described above.

> [!NOTE]
> `UGizmos.depthTest` is a static field, so it resets when scripts recompile. Assign it from a
> script with `[ExecuteAlways]` if you want it to hold in edit mode.

## Getting Started

### Requirements
- Unity 2021.3 or higher
- Built-in Render Pipeline, URP, or HDRP

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

Open Package Manager > UGizmo > Samples to install the samples:

| Sample | Contents |
| --- | --- |
| UGizmo Sample for URP | Every gizmo type, in a URP scene |
| UGizmo Sample for HDRP | The same, in an HDRP scene |
| UGizmo Sample for Built-in Render Pipeline | The same, in a Built-in Render Pipeline scene |
| UGizmo Benchmark | Measures how many gizmos hold a target frame rate, and compares that against the built-in `Gizmos` class |

### Where to draw from

`OnDrawGizmos` works, but it is editor-only, and it runs after UGizmo has already collected the
frame, so its gizmos appear one frame late. Subscribe to `UGizmos.OnDraw` instead when that
matters — it fires once per frame from inside the render loop, with no lag, and works the same in
play mode, edit mode and builds.

```C#
using UnityEngine;

[ExecuteAlways]
public class DrawCubeSample : MonoBehaviour
{
    private void OnEnable() => UGizmos.OnDraw += Draw;
    private void OnDisable() => UGizmos.OnDraw -= Draw;

    private void Draw(Camera camera)
    {
        UGizmos.DrawCube(Vector3.zero, Quaternion.Euler(45f, 45f, 45f), Vector3.one, Color.red);
    }
}
```

Drawing from `Update` or `LateUpdate` also has no lag, because both run before rendering.

### Turning gizmos off

`UGizmos.enabled` disables all tracking and rendering at runtime, without touching your draw calls.

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
| `DrawSector2D()` | Draws a sector (pie slice). |
| `DrawWireSector2D()` | Draws a wireframe sector (pie slice). |
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
[MIT License](/LICENSE)
