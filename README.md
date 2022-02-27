[![CodeQL](https://github.com/AjayKMehta/Dappery/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/codeql-analysis.yml) [![Infer#](https://github.com/AjayKMehta/Dappery/actions/workflows/infersharp.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/infersharp.yml)

[![Run tests](https://github.com/AjayKMehta/Dappery/actions/workflows/test.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/test.yml) [![codecov](https://codecov.io/gh/AjayKMehta/Dappery/branch/master/graph/badge.svg?token=E9QRR0SLSK)](https://codecov.io/gh/AjayKMehta/Dappery)

[![Open in Visual Studio Code](https://open.vscode.dev/badges/open-in-vscode.svg)](https://open.vscode.dev/AjayKMehta/Dappery)

## Run unit tests

```shell
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

## Display code coverage in VS Code:

1. Install [Coverage Gutters](https://marketplace.visualstudio.com/items?itemName=ryanluker.vscode-coverage-gutters) if not already installed.
2. Make sure the setting `coverage-gutters.coverageFileNames` in `settings.json` includes `"coverage.cobertura.xml"` as shown below:

    ```json
    "coverage-gutters.coverageFileNames": [
        "lcov.info",
        "cov.xml",
        "coverage.xml",
        "jacoco.xml",
        "coverage.cobertura.xml"
    ]
    ```

3. From Command Palette, select **Coverage Gutters: Display Coverage** after you have run unit tests (see [previous section](#run-unit-tests)).
