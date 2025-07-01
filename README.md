[![CodeQL](https://github.com/AjayKMehta/Dappery/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/codeql-analysis.yml) [![Infer#](https://github.com/AjayKMehta/Dappery/actions/workflows/infersharp.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/infersharp.yml)

[![Run tests](https://github.com/AjayKMehta/Dappery/actions/workflows/test.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/test.yml) [![codecov](https://codecov.io/gh/AjayKMehta/Dappery/branch/master/graph/badge.svg?token=E9QRR0SLSK)](https://codecov.io/gh/AjayKMehta/Dappery)

[![Semgrep](https://github.com/AjayKMehta/Dappery/actions/workflows/semgrep.yml/badge.svg)](https://github.com/AjayKMehta/Dappery/actions/workflows/semgrep.yml)

**[![Open in Visual Studio Code](https://open.vscode.dev/badges/open-in-vscode.svg)](https://open.vscode.dev/AjayKMehta/Dappery)**

## Introduction

This repo started out as a fork to learn more about how to use MediatR. Over time, I have used this more as a playground to explore and test out new .NET tooling, functionality and set up CI pipelines using GitHub Actions.

## Run unit tests

The test projects use [TUnit](https://github.com/thomhurst/TUnit) as it is more performant compared to the alterntaives -- it uses source-generated tests and parallel execution by default.

:bulb: You can add `--no-build` to first command if you already built code.

```shell
dotnet test --configuration Release -- --coverage --coverage-output-format xml --coverage-output coverage.cobertura.xml --crashdump --hangdump
dotnet-coverage merge **/*/*.cobertura.xml -f cobertura -o ./coverage.cobertura.xml
```

## Package restore

When building for CI/CD, we want to keep package versions locked based on lock file(s). If you encounter `NU1403`, try doing the following ([source](https://github.com/NuGet/Home/issues/7921#issuecomment-478152479)):

```shell
dotnet nuget locals all --clear
git clean -xfd
git rm **/packages.lock.json -f
dotnet restore
```
