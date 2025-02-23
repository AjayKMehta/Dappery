[![CodeQL](https://github.com/AjayKMehta/Dappery/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/codeql-analysis.yml) [![Infer#](https://github.com/AjayKMehta/Dappery/actions/workflows/infersharp.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/infersharp.yml)

[![Run tests](https://github.com/AjayKMehta/Dappery/actions/workflows/test.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/test.yml) [![codecov](https://codecov.io/gh/AjayKMehta/Dappery/branch/master/graph/badge.svg?token=E9QRR0SLSK)](https://codecov.io/gh/AjayKMehta/Dappery)

[![Semgrep](https://github.com/AjayKMehta/Dappery/actions/workflows/semgrep.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/semgrep.yml)

**[![Open in Visual Studio Code](https://open.vscode.dev/badges/open-in-vscode.svg)](https://open.vscode.dev/AjayKMehta/Dappery)**

## Run unit tests

### coverlet

When running tests with [code coverage using Microsoft Testing Platform](https://xunit.net/docs/getting-started/v3/code-coverage-with-mtp), the standard Coverlet experience is not supported.

### Microsoft.CodeCoverage

Running `dotnet test --collect "Code Coverage"` at the solution level now automatically merges code coverage for all your test projects. See [here](https://devblogs.microsoft.com/dotnet/whats-new-in-our-code-coverage-tooling) for more information.

```shell
# These 2 commands do the same thing:
dotnet test --no-build --collect "Code Coverage;Format=cobertura"
dotnet-coverage collect -f cobertura -o report.cobertura.xml "dotnet test --no-build"
```

## Display code coverage in VS Code:

> [!WARNING]
> :bulb: This currently only works with `coverlet`. Attempting to use `Microsoft.CodeCoverage` will silently fail.

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
