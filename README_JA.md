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
- リリースビルドをゼロコストにできる（描画呼び出しをILレベルで除去）
- 深度テストを有効化すればシーンのジオメトリにギズモを遮蔽させることも可能

### ドローコール最適化

標準のGizmosを利用した場合  
<img src="https://github.com/harumas/UGizmo/assets/43531665/34578ce8-c60a-4921-87fc-9aae99132c96" width="400px">

UGizmosを利用した場合  
<img src="https://github.com/harumas/UGizmo/assets/43531665/1cd23670-a982-4679-af8e-b3fa8f7d0bee" width="400px">


![image](https://github.com/harumas/UGizmo/assets/43531665/28b6614e-e610-4112-88f9-e0760babef3d)

### リリースビルドでのゼロコスト

すべての `UGizmos.DrawXXX` は `[Conditional]` 属性が付いているため、コンパイラが
**ILレベルで呼び出しごと除去**できます。呼び出しも引数も一切出力されないため、実行時コストも
JITも発生しません。`#if` で囲まなくても、ゲームコードにギズモ呼び出しをそのまま残せます。

以下のいずれかが定義されていれば描画は残ります：

| シンボル | 定義されるタイミング |
| --- | --- |
| `UNITY_EDITOR` | エディタ上 |
| `DEVELOPMENT_BUILD` | 開発ビルド（Development Build にチェック） |
| `UGIZMO` | **UGizmo の初回インポート時に自動で追加される**。リリースビルドでも最初から描画される |

そのため、リリースビルドでも既定でギズモが表示されます。除去してゼロコストにしたい場合は、
**Tools > UGizmo > Keep Gizmos in Release Builds** のチェックを外してください。Build Settings で
選択中のビルドターゲットに対して `UGIZMO` シンボルを追加・削除するため、Scripting Define Symbols
に手入力する必要はありません。

> [!NOTE]
> `UGIZMO` はビルドターゲットごとの設定で、自動追加されるのはインポート時に選択されていた
> ターゲットのみです。プラットフォームを切り替えた後は、新しいターゲットに対してメニューから
> 再度チェックを入れてください。

> [!NOTE]
> このシンボルが無いリリースビルドでは、呼び出し自体が存在しなくなるため、**エラーも警告も出ない
> まま何も描画されません**。ビルドでだけギズモが出ない場合は、まずここを確認してください。

> [!NOTE]
> 引数も呼び出しごと除去されるため、ギズモ呼び出しの引数に副作用を入れないでください
> （例: `UGizmos.DrawSphere(Step(), ...)`）。`UGizmos.Raycast` 等の物理ヘルパーは戻り値を
> 返すため常に実行され、描画部分のみが除去されます。

### 深度テスト

デフォルトでは、ギズモは手前に何があるかを無視します。ワイヤーフレームとポイントは常に手前に
描画され、ソリッド系のプリミティブは遮蔽された部分が薄く透けて表示されます。
`UGizmos.depthTest = true` を設定すると、すべてのギズモがシーンの深度バッファに従うようになり、
他のジオメトリに隠れた部分は**描画されなくなります**。

```csharp
UGizmos.depthTest = true;
```

これはグローバルな切り替えであり、描画呼び出しごとに個別指定するオプションはありません。
`false` に戻せば上記のデフォルトの見た目に戻ります。

> [!NOTE]
> `UGizmos.depthTest` は静的フィールドのため、スクリプトの再コンパイルでリセットされます。
> エディットモードで維持したい場合は、`[ExecuteAlways]` を付けたスクリプトから設定してください。

## 導入

### 必須要件
- Unity 2021.3以上
- Built-in Render Pipeline, URP, or HDRP

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

Package Manager > UGizmo > Samples を開くと、以下のサンプルをインストールできます。

| サンプル | 内容 |
| --- | --- |
| UGizmo Sample for URP | 全種類のギズモを配置した URP シーン |
| UGizmo Sample for HDRP | 同上の HDRP シーン |
| UGizmo Sample for Built-in Render Pipeline | 同上の Built-in Render Pipeline シーン |
| UGizmo Benchmark | 目標フレームレートを維持できる描画数を計測し、標準 `Gizmos` と比較します |

### どこから描画するか

`OnDrawGizmos` でも動作しますが、エディタ専用であることに加え、UGizmo がそのフレームの描画データを
収集した後に呼ばれるため、**1フレーム遅れて**表示されます。それが問題になる場合は
`UGizmos.OnDraw` を購読してください。レンダーループ内から毎フレーム1回発火するため遅延がなく、
再生時・エディットモード・ビルドのいずれでも同じように動作します。

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

`Update` や `LateUpdate` から描画する場合も、どちらも描画処理より前に実行されるため遅延はありません。

### ギズモを無効にする

`UGizmos.enabled` を false にすると、描画呼び出しはそのままに、収集と描画をまとめて停止できます。

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
| `DrawSector2D()` | セクター（扇形）を描画します。 |
| `DrawWireSector2D()` | ワイヤーフレームのセクター（扇形）を描画します。 |
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
