[![CodeQL](https://github.com/AjayKMehta/Dappery/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/codeql-analysis.yml) [![Infer#](https://github.com/AjayKMehta/Dappery/actions/workflows/infersharp.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/infersharp.yml)

[![Run tests](https://github.com/AjayKMehta/Dappery/actions/workflows/test.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/test.yml) [![codecov](https://codecov.io/gh/AjayKMehta/Dappery/branch/master/graph/badge.svg?token=E9QRR0SLSK)](https://codecov.io/gh/AjayKMehta/Dappery)

[![Open in Visual Studio Code](https://open.vscode.dev/badges/open-in-vscode.svg)](https://open.vscode.dev/AjayKMehta/Dappery)

## Run unit tests

### coverlet

```shell
dotnet test --collect:"XPlat Code Coverage" /p:CoverletOutputFormat=cobertura
```

### Microsoft.CodeCoverage

Running `dotnet test --collect "Code Coverage"` at the solution level now automatically merges code coverage for all your test projects. See [here](https://devblogs.microsoft.com/dotnet/whats-new-in-our-code-coverage-tooling) for more information.

```shell
# dotnet test --collect "Code Coverage;Format=cobertura"
dotnet test -s .runsettings 
```

> :bulb: This doesn't work: `dotnet-coverage collect -f cobertura -o report.cobertura.xml "dotnet test -s .runsettings"`.

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

## Package restore

When building for CI/CD, we want to keep package versions locked based on lock file(s). If you encounter `NU1403`, try doing the following ([source](https://github.com/NuGet/Home/issues/7921#issuecomment-478152479)):

```shell
dotnet nuget locals all --clear
git clean -xfd
git rm **/packages.lock.json -f
dotnet restore
```
