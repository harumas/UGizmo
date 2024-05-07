# UGizmo

Highly efficient gizmo renderer for Unity

[日本語版](https://github.com/harumas/UGizmo/blob/main/README_JA.md)

[![license](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE.md)
[![license](https://img.shields.io/badge/PR-welcome-green.svg)](https://github.com/HarumaroJP/UGizmo/pulls)
[![license](https://img.shields.io/badge/Unity-2021.3-green.svg)](#要件)

![UGizmo-02](https://github.com/harumas/UGizmo/assets/43531665/14cd2412-19fa-48de-94ae-47973ee5ca99)

## Overview

UGizmo uses GPU instancing to reduce draw calls and draw gizmos more efficiently.  
It also adds various features to the standard Gizmos class and supports its use at runtime.

## Key features

- Draw call optimization using GPU instancing
- Available to be called by event functions such as Update(), FixedUpdate(), etc.
- Runtime support
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
        "com.harumaron.ugizmo": "https://github.com/harumas/UGizmo.git?path=src/UGizmo/Assets/UGizmo"
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

![image](https://github.com/harumas/UGizmo/assets/43531665/5e1e4a5e-12d7-4144-9531-9d6de54097f9)

## Performance
This benchmark shows the number of drawings that can maintain 60 fps when the following methods are called within OnDrawGizmos.
Please refer to `UGizmo.Benchmark` in the project for the specific benchmark source.
```C#
Gizmos:
Gizmos.DrawSphere();
Gizmos.DrawCube();
Gizmos.DrawLine();

UGizmos:
UGizmos.DrawSphere();
UGizmos.DrawCube();
UGizmos.DrawLine();
```

![image](https://github.com/harumas/UGizmo/assets/43531665/3a7cde16-151a-4ea7-b4d4-66aebc431f6b)


## License
This library is released under the MIT License. 
[MIT License](/LICENSE.md)
