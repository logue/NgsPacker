# NGSPacker

本プログラムは、PSO2NGS のゲームデータファイルを解凍（アンパック）したり生成（パック）するものです。

動作には.net6.0 ランタイムが必要です。下記サイトからダウンロードしてください。
<https://dotnet.microsoft.com/download/dotnet>

## 主な機能

- パック：選択されたディレクトリから PSO2 用のデータファイル（ICE ファイル）を生成します。
- アンパック：選択した PSO2 のデータファイル（ICE ファイル）を解凍します。
- ファイルリストを出力：PSO2 のデータディレクトリ内のファイルと各データファイルに含まれるファイルを csv 形式で出力します。
- ファイルリストからアンパック：改行区切りのファイルリストに含まれる ice ファイルのみを一括アンパックします。

## 便利なリンク

- [PSO2 Modding Tutorial](http://www.pso-world.com/forums/showthread.php?237103-PSO2-Modding-Tutorial-2-0)
- [PSO2 File Directory](https://docs.google.com/spreadsheets/d/1GQwG49iYM1sgJhyAU5AWP-gboemzfIZjBGjTGEZSET4/edit?usp=sharing)
  - [PSO2-Modding](https://discord.com/invite/cV3QRkB) - Discord ※日本語情報あり
- [Arks-Layer](https://arks-layer.com/) - サードパーティー製ランチャーです。日本語や英語以外でプレイしたい場合や Mod を使用する場合必須です。
  - [Phantasy Star Fleet](https://discord.com/invite/pso2) - Discord
- [チェックサムファイル](https://ngs.logue.dev/data/d4455ebc2bef618f29106da7692ebc1a) - Mod を有効化するときに必要です。`pso2_bin/data/win32`にあるファイルと差し替えてください。（ArksLayer の Aida Enna 氏によるものです）
- [PSO2 Modding Database](https://pso2mod.com/)
  - [PSO2 Mod Manager](https://github.com/PolCPP/PSO2-Mod-Manager) - PSO2Mods.com 公式 Mod マネージャーですが長らくメンテナンスされていません。
- [PSO2NGS @ Nexus Mods](https://www.nexusmods.com/phantasystaronline2newgenesis)

## 免責事項

ゲームデータを改変する行為(Modding)は、同一性保持権の侵害にはあたらず、
日本国憲法第 21 条に定められる表現の自由、米国著作権法第 107 条に定められるフェアユース法の範疇内にあります。

しかしながら、Modding は潜在的にゲーム環境に損害を与える可能性がある行為であり、バンされる可能性があるということについて留意ください。
特に非公開の内部データの不特定多数の閲覧できるコミュニティでの流布や他作品への流用（モデルデータを変換して流用するなど）は、
上記の法律から明確に逸脱する行為であり、損害賠償を伴う訴訟になる場合があります。[^1]

その事によって生じたいかなる損害について、作者一同は一切の責任を負いかねます。
良識を持って利用し、起きうる損害については自己責任でお願いします。

[^1]: 詳細につきましては、公式サイトの[著作権侵害および不正行為について](https://pso2.jp/players/news/29881/)をご覧になってください。

## ライセンス

[MIT](./LICENSE)

本ツールは、[Shadowth117](https://github.com/Shadowth117)氏の[ZamboniLib](https://github.com/Shadowth117/ZamboniLib)および、[PizzaCrust](https://github.com/PizzaCrust)氏の[ooz-sys](https://github.com/PizzaCrust/ooz-sys)を使用しています。

『PSO2』で使用している著作物（画像、データ、音声、テキスト等）の一切の権利は、株式会社セガまたはそのライセンサーが保有しております。
