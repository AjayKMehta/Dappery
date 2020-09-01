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