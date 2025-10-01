# CSV ランダムジェネレーター

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![codecov](https://codecov.io/gh/symrsonline/csv-random-generator/branch/master/graph/badge.svg)](https://codecov.io/gh/symrsonline/csv-random-generator)
[![CI](https://github.com/symrsonline/csv-random-generator/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/symrsonline/csv-random-generator/actions/workflows/ci-cd.yml)

このプロジェクトは、指定された行数と列数のランダムなCSVファイルを生成し、指定されたフォルダに出力します。

## 使用方法

以下のコマンドでプログラムを実行します:

```bash
dotnet run --rows <行数> --cols <列数> --folder <フォルダ> --output <出力ファイル> --sort-column <ソート列>
```

オプションは順不同で指定可能。ソート列は0から始まる列インデックス、省略可能。

### 例

```bash
dotnet run --cols 10 --rows 100 --folder ./output --output data.csv --sort-column 2
```

これにより、100行10列のランダムデータを`./output`フォルダに生成し、3列目でソートして出力します。

ヘルプを表示するには:

```bash
dotnet run -- --help
```

## 要件

- .NET 8.0 以降

## CI/CD

このプロジェクトはGitHub Actionsを使用してCI/CDを実装しています。

- **ビルドとテスト**: masterブランチへのプッシュまたはプルリクエストで自動的にビルドとテストが実行されます。
- **リリース**: GitHub Releasesを作成すると、Windows用の自己完結型exeファイルが自動的にビルドされ、リリースにアップロードされます。

## テスト

プロジェクトにはxUnitを使用したユニットテストが含まれています。テストを実行するには:

```bash
dotnet test
```

テストはCSV生成の行数列数確認とソート機能を検証します。

## データ型

プログラムは列ごとに固定された型でランダムデータを生成します。以下のものがあります:
- 整数 (0-100)
- 倍精度浮動小数点数 (0-100、小数点以下2桁)
- 文字列 (ランダムな大文字アルファベット、5-10文字)
- 日時 (yyyy/MM/dd HH:mm:ss形式、ランダムな日時)

## ライセンス

このプロジェクトはMITライセンスの下で公開されています。詳細は[LICENSE](LICENSE)ファイルを参照してください。