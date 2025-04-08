# UGizmo

Highly efficient gizmo drawer for Unity

[![license](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE.md)
[![license](https://img.shields.io/badge/PR-welcome-green.svg)](https://github.com/HarumaroJP/UGizmo/pulls)
[![license](https://img.shields.io/badge/Unity-2021.3-green.svg)](#要件)

![UGizmo-02](https://github.com/harumas/UGizmo/assets/43531665/81bb8462-d39a-4009-a634-dea451ab5f17)

## 概要

UGizmoは、標準のGizmosクラスに様々な機能を追加し、ランタイムやイベント関数での呼び出しを可能にしたライブラリです。  
また、GPU instancingを用いてドローコールを削減することにより、効率的にギズモを描画することができます。 

## 特徴

- Update(), LateUpdate()等のUnityのイベント関数での呼び出しをサポート
- ランタイムで描画可能
- GPU instancingを用いたドローコールの最適化
- 30種類以上のギズモ

### ドローコール最適化

標準のGizmosを利用した場合  
<img src="https://github.com/harumas/UGizmo/assets/43531665/34578ce8-c60a-4921-87fc-9aae99132c96" width="400px">

UGizmosを利用した場合  
<img src="https://github.com/harumas/UGizmo/assets/43531665/1cd23670-a982-4679-af8e-b3fa8f7d0bee" width="400px">


![image](https://github.com/harumas/UGizmo/assets/43531665/28b6614e-e610-4112-88f9-e0760babef3d)

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
        "com.harumaro.ugizmo": "https://github.com/harumas/UGizmo.git?path=src/UGizmo/Assets/UGizmo"
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

![image](https://github.com/harumas/UGizmo/assets/43531665/a321d8b5-0ef9-40a5-b24b-cd901af8f8f1)


## 機能リスト

### 3D Primitives
| 名前 | 補足 |
| --- | --- |
| `DrawSphere()` | 球体を描画します。 (従来の倍の頂点数) |
| `DrawWireSphere()` | ワイヤーフレームの球体を描画します。(従来の倍の頂点数) |
| `DrawCube()` | キューブを描画します。 |
| `DrawWireCube()` | ワイヤーキューブを描画します。 |
| `DrawCapsule()` | カプセルを描画します。 |
| `DrawWireCapsule()` | ワイヤーフレームのカプセルを描画します。 |
| `DrawCylinder()` | シリンダーを描画します。 |
| `DrawWireCylinder()` | ワイヤーフレームのシリンダーを描画します。 |
| `DrawCone()` | コーンを描画します。 |
| `DrawWireCone()` |  ワイヤーフレームのコーンを描画します。  |
| `DrawPlane()` | プレーンを描画します。 |
| `DrawWirePlane()` | ワイヤーフレームのプレーンを描画します。 |

### 2D Primitives
| 名前 | 補足 |
| --- | --- |
| `DrawCircle2D()` | 円を描画します。 |
| `DrawWireCircle2D()` | ワイヤーフレームの円を描画します。|
| `DrawBox2D()` | ボックスを描画します。 |
| `DrawWireBox2D()` | ワイヤーフレームのボックスを描画します。 |
| `DrawTriangle2D()` | 三角形を描画します。 |
| `DrawWireTriangle2D()` | ワイヤーフレームの三角形を描画します。  |
| `DrawCapsule2D()` | カプセルを描画します。 |
| `DrawWireCapsule2D()` | ワイヤーフレームのカプセルを描画します。 |

### Utilites
| 名前 | 補足 |
| --- | --- |
| `DrawPoint()` | ポイントを描画します。最前面に描画されます。 |
| `DrawLine()` | ラインを描画します。 |
| `DrawLineList()` | 点のペアの間に複数のラインを描画します。 |
| `DrawLineStrip()` | 指定されたスパン内の各点の間に線を描画します。 |
| `DrawRay()` | start （開始地点）から start + dir （開始地点＋方向）までラインを描画します |
| `DrawFrustum()` | カメラの錐台を描画します。 |
| `DrawDistance()` | aからbの距離を表示します。ランタイムでは距離は表示されません。 |
| `DrawMeasure()` | 一定間隔で区切られた線を描画します |
| `DrawArrow()` | 3Dの矢印を描画します。 |
| `DrawArrow2d()` | 2Dの矢印を描画します。 |
| `DrawFacingArrow2d()` | 2Dの矢印を描画します。常にカメラに向きます。|
| `DrawWireArrow()` | ワイヤーボディの2Dの矢印を描画します。|
| `DrawFacingWireArrow()` | ワイヤーボディの2Dの矢印を描画します。常にカメラに向きます。 |

### 3D Physics
| 名前 | 補足 |
| --- | --- |
| `Raycast()` | レイキャストを行い、結果を描画します。 |
| `DrawRaycast()` | RaycastHit構造体を渡して、レイキャストの結果を描画します。 |
| `Linecast()` | ラインキャストを行い、結果を描画します。 |
| `DrawLinecast()` | RaycastHit構造体を渡して、ラインキャストの結果を描画します。 |
| `SphereCast()` | スフィアキャストを行い、結果を描画します。|
| `DrawSphereCast()` | RaycastHit構造体を渡して、スフィアキャストの結果を描画します。|
| `BoxCast()` | ボックスキャストを行い、結果を描画します。 |
| `DrawBoxCast()` | RaycastHit構造体を渡して、ボックスキャストの結果を描画します。 |
| `CapsuleCast()` | カプセルキャストを行い、結果を描画します。 |
| `DrawCapsuleCast()` | RaycastHit構造体を渡して、カプセルキャストの結果を描画します。 |

### 2D Physics
| 名前 | 補足 |
| --- | --- |
| `Raycast2D()` | 2Dのレイキャストを行い、結果を描画します。 |
| `DrawRaycast2D()` | RaycastHit構造体を渡して、レイキャストの結果を描画します。 |
| `Linecast2D()` | 2Dのラインキャストを行い、結果を描画します。 |
| `DrawLinecast2D()` | RaycastHit構造体を渡して、ラインキャストの結果を描画します。 |
| `SphereCast2D()` | 2Dのスフィアキャストを行い、結果を描画します。|
| `DrawSphereCast2D()` | RaycastHit構造体を渡して、スフィアキャストの結果を描画します。|
| `BoxCast2D()` | 2Dのボックスキャストを行い、結果を描画します。 |
| `DrawBoxCast2D()` | RaycastHit構造体を渡して、ボックスキャストの結果を描画します。 |
| `CapsuleCast2D()` | 2Dのカプセルキャストを行い、結果を描画します。 |
| `DrawCapsuleCast2D()` | RaycastHit構造体を渡して、カプセルキャストの結果を描画します。 |

## ライセンス
本ライブラリはMITライセンスで公開しています。  
[MIT License](/LICENSE)
