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

あるいはPackages/manifest.jsonを開き、dependenciesブロックに以下を追記

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


## ライセンス
本ライブラリはMITライセンスで公開しています。  
[MIT License](/LICENSE.md)
