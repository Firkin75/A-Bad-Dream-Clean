# 🎮 A Bad Dream - レトロホラーFPS

## 🧟 ゲーム概要

このゲームは、制作者自身がかつて見た「悪夢」をもとに制作したホラーゲームです。  
プレイヤーは、見知らぬ地下施設で目を覚ました主人公となり、敵を倒しながら武器や鍵を探し、脱出を目指します。

全体の雰囲気は、90年代のFPS『DOOM』に影響を受けたレトロなグラフィックと、抑圧感のある演出を重視しています。  
派手な演出は控えつつ、閉鎖的なマップや突然の敵出現により、じわじわと緊張感が高まるように工夫しました。

---

## 🧑‍💻 担当範囲

企画・プログラミング・レベルデザイン・UI設計など、すべてを一人で担当しました。

---

## 🔧 主な実装機能

- 一人称視点での移動・攻撃システム  
- 敵AI（追跡、攻撃、死亡後の処理）  [SourceCode](Assets/Scripts/Enemy/EnemyController.cs)
- ノートや鍵、マップなどの収集要素とUI表示（IInteractableインターフェースで実装）  
- イベントトリガーによるゲーム進行（敵出現、扉解錠など）  
- チュートリアルUI（移動・インタラクト・戦闘の基本操作を案内）  
- DOOMライクな照準方式（上下方向の敵を自動照準するバーチカルオートエイム）  
- 死亡演出とリスタート機能  
- ミニマップ表示  
- メインメニューでの音量調整機能  
- ポーズメニュー内の音量調整およびマウス感度設定機能  

---

## 💻 ソースコードについて

Unityプロジェクト全体を再構築可能な状態でアップロードしており、  
**ソースコード（C#スクリプト）は `Assets/Script` フォルダ**に保存されています。  

- 対応Unityバージョン：**Unity 6（2025.1以降）**
- 推奨IDE：**Visual Studio 2022**
---

## 🎥 デモ・ダウンロード

- ▶ [プレイ動画を見る](https://drive.google.com/file/d/1blGZ7nZ8jkOo6EIAxIkqrMNwAa6JZPh8/view?usp=sharing)  
- ⬇ [Windows版をダウンロード](https://drive.google.com/file/d/1p26A7FAy_04j5gGY6WEaMCnIOoErr35r/view?usp=sharing)

---

## 🖼️ Screenshots

![screenshot4](https://github.com/user-attachments/assets/015f85aa-9b01-43ee-b9e7-bca01147407c)  
![screenshot2](https://github.com/user-attachments/assets/62b340c5-602d-4b1f-8668-23e52605d99e)
