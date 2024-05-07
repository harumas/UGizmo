# UGizmo

Highly efficient gizmo renderer for Unity

[![license](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE.md)
[![license](https://img.shields.io/badge/PR-welcome-green.svg)](https://github.com/HarumaroJP/UGizmo/pulls)
[![license](https://img.shields.io/badge/Unity-2021.3-green.svg)](#要件)

![UGizmo-02](https://github.com/harumas/UGizmo/assets/43531665/14cd2412-19fa-48de-94ae-47973ee5ca99)

## 概要

UGizmoはGPU instancingを用いてドローコールを削減することにより、ギズモを効率的に描画するライブラリです。  
また、標準のGizmosクラスに様々な機能を追加し、ランタイムでの利用もサポートしています。

## 特徴

- GPU instancingを用いたドローコールの最適化
- Update(), FixedUpdate()等のOnDrawGizmos()以外での描画サポート
- ランタイムで描画可能
- 30種類以上のギズモ

## 導入

### 必須要件
- Unity 2021.3以上
- URP or HDRP (Built-in Render Pipelineはサポートしていません)

### インストール
1. Window > Package ManagerからPackage Managerを開く
2. 「+」ボタン > Add package from git URL
3. 以下のURLを入力する

```
https://github.com/harumas/UGizmo.git?path=src/UGizmo/Assets/UGizmo
```

もしくはPackages/manifest.jsonを開き、dependenciesブロックに以下を追記してください。

```json
{
    "dependencies": {
        "com.harumaron.ugizmo": "https://github.com/harumas/UGizmo.git?path=src/UGizmo/Assets/UGizmo"
    }
}
```

### サンプル
`UGizmos`クラスを用いることで描画することができます。  
標準の`Gizmos`クラスでは、`Gizmos.matrix`を用いてマトリックスを指定しますが、UGizmoではメソッドの引数に、位置、回転、スケールの情報を渡します。
また、色の指定も`Gizmos.color`ではなく、メソッドの引数に直接Color構造体を渡してください。

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


Package Manager > UGizmo > Samplesを開くと、URPとHDRP向けのサンプルをインストールすることができます。

![image](https://github.com/harumas/UGizmo/assets/43531665/5e1e4a5e-12d7-4144-9531-9d6de54097f9)

## パフォーマンス
以下のメソッドをOnDrawGizmos内で呼び出す際に、60fpsを保つことが出来る描画回数を示したベンチマークです。
具体的なベンチマークのソースはプロジェクト内の`UGizmo.Benchmark`を参照してください。
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

## 機能リスト

### 3D Primitives
| 名前 | 補足 |
| --- | --- |
| `DrawSphere()` | 球体は従来の倍の頂点数 |
| `DrawWireSphere()` | 球体は従来の倍の頂点数 |
| `DrawCube()` | |
| `DrawWireCube()` | |
| `DrawCapsule()` | |
| `DrawWireCapsule()` | |
| `DrawCylinder()` | |
| `DrawWireCylinder()` | |
| `DrawCone()` | |
| `DrawWireCone()` | |
| `DrawPlane()` | |
| `DrawWirePlane()` | |

### 2D Primitives
| 名前 | 補足 |
| --- | --- |
| `DrawCircle2D()` | |
| `DrawWireCircle2D()` | |
| `DrawBox2D()` | |
| `DrawWireBox2D()` | |
| `DrawTriangle2D()` | |
| `DrawWireTriangle2D()` | |
| `DrawCapsule2D()` | |
| `DrawWireCapsule2D()` | |

### Utilites
| 名前 | 補足 |
| --- | --- |
| `DrawPoint()` | 球体が最前面に描画されます |
| `DrawLine()` | |
| `DrawLineList()` | |
| `DrawLineStrip()` | |
| `DrawRay()` | |
| `DrawFrustum()` | |
| `DrawDistance()` | aからbの距離を表示します。ランタイムでは距離は表示されません。 |
| `DrawMeasure()` | 一定間隔で区切られた線を描画します |
| `DrawArrow()` | |
| `DrawArrow2d()` | |
| `DrawFacingArrow2d()` | 常にカメラに向きます |
| `DrawWireArrow()` | |
| `DrawFacingWireArrow()` | 常にカメラに向きます |

### 3D Physics
| 名前 | 補足 |
| --- | --- |
| `Raycast()` | 実際にレイキャストを行い、結果を描画します。 |
| `DrawRaycast()` | RaycastHit構造体を渡して、レイキャストの結果を描画します。 |
| `Linecast()` |  |
| `DrawLinecast()` |  |
| `SphereCast()` |  |
| `DrawSphereCast()` |  |
| `BoxCast()` |  |
| `DrawBoxCast()` |  |
| `CapsuleCast()` |  |
| `DrawCapsuleCast()` |  |

### 2D Physics
| 名前 | 補足 |
| --- | --- |
| `Raycast2D()` | 実際にレイキャストを行い、結果を描画します。 |
| `DrawRaycast2D()` | RaycastHit構造体を渡して、レイキャストの結果を描画します。 |
| `Linecast2D()` |  |
| `DrawLinecast2D()` |  |
| `SphereCast2D()` |  |
| `DrawSphereCast2D()` |  |
| `BoxCast2D()` |  |
| `DrawBoxCast2D()` |  |
| `CapsuleCast2D()` |  |
| `DrawCapsuleCast2D()` |  |

## ライセンス
本ライブラリはMITライセンスで公開しています。  
[MIT License](/LICENSE.md)
