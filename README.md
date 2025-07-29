#  A Bad Dream 

##  ゲーム概要

このゲームは、制作者自身がかつて見た「悪夢」をもとに制作したホラーゲームです。  
プレイヤーは、見知らぬ地下施設で目を覚ました主人公となり、敵を倒しながら武器や鍵を探し、脱出を目指します。

全体の雰囲気は、90年代のFPS『DOOM』に影響を受けたレトロなグラフィックと、抑圧感のある演出を重視しています。  
派手な演出は控えつつ、閉鎖的なマップや突然の敵出現により、じわじわと緊張感が高まるように工夫しました。

---

##  担当範囲

企画・プログラミング・レベルデザイン・UI設計など、すべてを一人で担当しました。

---

##  主な実装機能

- 一人称視点での移動システム

  関連ソースファイル：[PlayerController.cs](Assets/Script/Player/PlayerController.cs)
  
- 敵AI（追跡、攻撃、死亡後の処理）

  関連ソースファイル：[EnemyController.cs](Assets/Script/Enemy/EnemyController.cs)
  
- ノートや鍵、マップなどの収集要素とUI表示（IInteractableインターフェースで実装）

    関連ソースファイル：[IInteractable.cs](Assets/Script/UI/IInteractable.cs)、[InteractManager.cs](Assets/Script/Environment/InteractManager.cs)
  

- DOOMライクな照準方式（上下方向の敵を自動照準するバーチカルオートエイム）

  関連ソースファイル：[Weapon.cs](Assets/Script/Weapon/Weapon.cs)、[ShotGun.cs](Assets/Script/Weapon/ShotGun.cs)、[機能紹介GIF](GIF/Aiming.gif)
  
- 死亡演出とリスタート機能

   関連ソースファイル：[PlayerHealth.cs](Assets/Script/Player/PlayerHealth.cs)、[GameOverScreen.cs](Assets/Script/UI/GameOverScreen.cs)

- 2.5Dの敵キャラクター（8方向のスプライトで実装）とゲームオブジェクト

  関連ソースファイル：[EnemySpriteController.cs](Assets/Script/Enemy/EnemySpriteController.cs)、[BillBoard.cs](Assets/Script/Enemy/Billboard.cs)、[機能紹介GIF](GIF/2.5D.gif)
 
- メインメニューでの音量調整機能

  関連ソースファイル：[MainMenu.cs](Assets/Script/UI/MainMenu.cs)
  
- ポーズメニュー内の音量調整およびマウス感度設定機能

  関連ソースファイル：[PauseMenuLogic.cs](Assets/Script/UI/PauseMenuLogic.cs)

- ミニマップ表示
  
- チュートリアルUI（移動・インタラクト・戦闘の基本操作を案内）   
  

---

##  ソースコードについて

Unityプロジェクト全体を再構築可能な状態でアップロードしており、  
**ソースコード（C#スクリプト）は `Assets/Script` フォルダ**に保存されています。  

- 対応Unityバージョン：**Unity 6（2025.1以降）**
- 推奨IDE：**Visual Studio 2022**
---

##  デモ・ダウンロード

- ▶ [プレイ動画を見る](https://drive.google.com/file/d/1blGZ7nZ8jkOo6EIAxIkqrMNwAa6JZPh8/view?usp=sharing)  
- ⬇ [Windows版をダウンロード](https://drive.google.com/file/d/1p26A7FAy_04j5gGY6WEaMCnIOoErr35r/view?usp=sharing)

---

##  Screenshots

![screenshot4](https://github.com/user-attachments/assets/015f85aa-9b01-43ee-b9e7-bca01147407c)  
![screenshot2](https://github.com/user-attachments/assets/62b340c5-602d-4b1f-8668-23e52605d99e)
