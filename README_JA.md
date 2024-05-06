# UGizmo

Highly efficient gizmo renderer for Unity

[![license](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE.md)
[![license](https://img.shields.io/badge/PR-welcome-green.svg)](https://github.com/HarumaroJP/UGizmo/pulls)
[![license](https://img.shields.io/badge/Unity-2021.3-green.svg)](#要件)

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
- URP or HDRP (Built-in Render Pipelineはまだサポートしていません)

### インストール
1. Window > Package ManagerからPackage Managerを開く
2. 「+」ボタン > Add package from git URL
3. 以下のURLを入力する

```
https://github.com/HarumaroJP/UGizmo.git?path=src/UGizmo/Assets/UGizmo
```

あるいはPackages/manifest.jsonを開き、dependenciesブロックに以下を追記

```json
{
    "dependencies": {
        "com.harumaro.ugizmo": "https://github.com/HarumaroJP/UGizmo.git?path=src/UGizmo/Assets/UGizmo"
    }
}
```

### サンプル
Package Manager > 

![image](https://github.com/HarumaroJP/UGizmo/assets/43531665/5e1e4a5e-12d7-4144-9531-9d6de54097f9)


## ライセンス
本ライブラリはMITライセンスで公開しています。  
[MIT License](/LICENSE.md)
