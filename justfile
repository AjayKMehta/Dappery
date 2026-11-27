# Cross platform shebang:
shebang := if os() == 'windows' { 'pwsh.exe' } else { '/usr/bin/env pwsh' }

set default-list

set shell := ["pwsh", "-NoLogo", "-NoProfile", "-NonInteractive", "-CommandWithArgs"]

alias a := analyze
alias b := build
alias bnr := build-no-restore
alias fws := format-whitespace
alias gps := gcdump-process
alias lt := list-tools
alias r := restore
alias ra := roslynator-analyze
alias rap := roslynator-analyze-proj
alias rf := roslynator-fix
alias rfs := roslynator-find-symbol
alias rp := restore-proj
alias rt := restore-tools
alias run := run-tool
alias tc := trace-collect
alias tps := trace-process
alias tp := test-proj
alias thr := test-hot-reload
alias vf := verify-format

default_config := env("BUILD_CONFIG", "debug")
default_verbosity := env("BUILD_VERBOSITY", "normal")

# region Build

[arg("config", short="c", pattern="^(?i)(debug|release)$")]
[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[doc("Build (root solution).
Options:
   -c <config>       Case-insensitive config: debug, release
   -v <verbosity>    Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
   -- <options>      Arbitrary trailing options forwarded to dotnet build.")]
[group("build")]
build config=default_config verbosity=default_verbosity *OPTIONS:
    @dotnet build {{ OPTIONS }} -v {{ verbosity }} -c {{ config }}

[arg("config", short="c", pattern="^(?i)(debug|release)$")]
[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[doc("Build (root solution) without restoring dependencies first.
Options:
   -c <config>       Case-insensitive config: debug, release
   -v <verbosity>    Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
   -- <options>      Arbitrary trailing options forwarded to dotnet build.")]
[group("build")]
build-no-restore config=default_config verbosity=default_verbosity *OPTIONS:
    @dotnet build {{ OPTIONS }} --no-restore -v {{ verbosity }} -c {{ config }}

[arg("config", short="c", pattern="^(?i)(debug|release)$")]
[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[doc("Build selected project|solution|file.
Options:
   -c <config>       Case-insensitive config: debug, release
   -v <verbosity>    Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
   -- <options>      Arbitrary trailing options forwarded to dotnet build.")]
[group("build")]
[no-cd]
build-proj proj_or_sln config=default_config verbosity=default_verbosity *OPTIONS:
    @dotnet build {{ proj_or_sln }} {{ OPTIONS }} -v {{ verbosity }} -c {{ config }}

#endregion

# region Clean

# Clean build artifacts only. Untracked files are left untouched.
[group("clean")]
clean:
    dotnet clean

# Clean build artifacts AND untracked files/directories (destructive).
[group("clean")]
clean-all:
    dotnet clean && git clean -fd

[arg("config", short="c", pattern="^(?i)(debug|release)$")]
[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[doc("Clean selected project|solution|file.
Options:
   -c <config>       Case-insensitive config: debug, release
   -v <verbosity>    Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
   -- <options>      Arbitrary trailing options forwarded to dotnet clean.")]
[group("clean")]
[no-cd]
clean-proj proj_or_sln config=default_config verbosity=default_verbosity *OPTIONS:
    @dotnet clean {{ proj_or_sln }} {{ OPTIONS }} -v {{ verbosity }} -c {{ config }}

#endregion

# region Format

[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[doc("Format .NET code.
Options:
   -v <verbosity>    Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
   -- <options>      Arbitrary trailing options forwarded to dotnet format.")]
[group("format")]
format verbosity=default_verbosity *OPTIONS:
    @dotnet format {{ OPTIONS }} -v {{ verbosity }}

# Verify no formatting changes would be made.
[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[group("format")]
verify-format verbosity=default_verbosity:
    @dotnet format --verify-no-changes -v {{ verbosity }}

[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[arg("exclude", long="exclude")]
[arg("include", long="include")]
[doc("Run 3rd party analyzers and apply fixes.
Options:
   --include <include>     Comma separated list of file or folder paths to include. Path is relative to root.
   --exclude <exclude>     Comma separated list of file or folder paths to exclude. Path is relative to root.
   -v <verbosity>          Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
Example: just analyze --exclude src/Foo,tests/ -v d")]
[group("format")]
@analyze verbosity=default_verbosity include='' exclude='':
    . "{{ justfile_directory() }}\scripts\just-helpers.ps1"; \
    $values = [System.Collections.Generic.List[string]]::new(); \
    Add-PathFilterArguments $values "{{ include }}" "{{ exclude }}"; \
    dotnet format analyzers -v {{ verbosity }} $values

[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[arg("exclude", long="exclude")]
[arg("include", long="include")]
[doc("Run 3rd party analyzers and apply fixes.
Options:
   --include <include>     Comma separated list of file or folder paths to include. Path is relative to root.
   --exclude <exclude>     Comma separated list of file or folder paths to exclude. Path is relative to root.
   -v <verbosity>          Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
Example: just format-whitespace --include src/Foo,tests/ -v quiet")]
[group("format")]
@format-whitespace verbosity=default_verbosity include='' exclude='':
    . "{{ justfile_directory() }}\scripts\just-helpers.ps1"; \
    $values = [System.Collections.Generic.List[string]]::new(); \
    Add-PathFilterArguments $values "{{ include }}" "{{ exclude }}"; \
    dotnet format whitespace -v {{ verbosity }} $values

[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[arg("exclude", long="exclude")]
[arg("include", long="include")]
[doc("Run code style analyzers and apply fixes.
Options:
   --include <include>     Comma separated list of file or folder paths to include. Path is relative to root.
   --exclude <exclude>     Comma separated list of file or folder paths to exclude. Path is relative to root.
   -v <verbosity>          Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
Example: just style --include src/Foo,tests/ -v quiet")]
[group("format")]
@style verbosity=default_verbosity include='' exclude='':
    . "{{ justfile_directory() }}\scripts\just-helpers.ps1"; \
    $values = [System.Collections.Generic.List[string]]::new(); \
    Add-PathFilterArguments $values "{{ include }}" "{{ exclude }}"; \
    dotnet format style -v {{ verbosity }} $values

# endregion

# region Debugging

# Lists the dotnet processes that traces can be collected from.
[group("debug")]
trace-process:
    @dotnet trace ps

# Collects a diagnostic trace from a currently running process.
[arg("process-id", short="p")]
[group("debug")]
[windows]
trace-collect process-id *OPTIONS:
    @dotnet trace collect {{ OPTIONS }} --process-id {{ process-id }}

# Collects a diagnostic trace from a currently running process.
[arg("process-id", short="p")]
[group("debug")]
[linux]
trace-collect process-id *OPTIONS:
    @dotnet trace collect-linux {{ OPTIONS }} --process-id {{ process-id }}

# Generate a report into stdout from a previously generated trace
[group("debug")]
trace-report trace_file:
    @dotnet trace report {{ trace_file }}

# List pre-built tracing profiles
[group("debug")]
list-profiles:
    @dotnet trace list-profiles

# List the dotnet processes that dumps can be collected from
[group("debug")]
gcdump-process:
    @dotnet gcdump ps

# endregion

# region .NET Tools

# Restore local dotnet tools
[group("tools")]
restore-tools:
    @dotnet tool restore

# List local dotnet tools
[group("tools")]
list-tools:
    @dotnet tool list

# Run local dotnet tool
[group("tools")]
run-tool tool *OPTIONS:
    @dotnet tool run {{ tool }} {{ OPTIONS }}

# endregion

# region Packages

[group("packages")]
restore *OPTIONS:
    @dotnet restore {{ OPTIONS }}

[group("packages")]
[no-cd]
restore-proj proj_or_sln *OPTIONS:
    @dotnet restore {{ proj_or_sln }} {{ OPTIONS }}

[group("packages")]
[no-cd]
why proj_or_sln package:
    @dotnet nuget why {{ proj_or_sln }} {{ package }}

# endregion

# region roslynator

[group("roslynator")]
roslynator *OPTIONS:
    roslynator {{ OPTIONS }}

[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[arg("exclude", long="exclude")]
[arg("include", long="include")]
[doc("Analyzes root solution and reports diagnostics.
Options:
   --include <include>              Comma separated list of file or folder paths to include. Path is relative to root.
   --exclude <exclude>              Comma separated list of file or folder paths to exclude. Path is relative to root.
   -v <verbosity>                   Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
   -- <options>      Arbitrary trailing options forwarded to roslynator analyze.
Example: just roslynator-analyze --exclude tests -v d -- --severity-level error")]
[group("roslynator")]
@roslynator-analyze verbosity=default_verbosity include='' exclude='' *OPTIONS:
    . "{{ justfile_directory() }}\scripts\just-helpers.ps1"; \
    $values = [System.Collections.Generic.List[string]]::new(); \
    Add-PathFilterArguments $values "{{ include }}" "{{ exclude }}"; \
    roslynator analyze {{ OPTIONS }} -v {{ verbosity }} $values

[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[arg("exclude", long="exclude")]
[arg("include", long="include")]
[doc("Analyzes specified project/solution and reports diagnostics.
Options:
   <PROJ_OR_SLN>                    Path to one or more project/solution files.
   --include <include>              Comma separated list of relative file or folder paths to include.
   --exclude <exclude>              Comma separated list of relative file or folder paths to exclude.
   -v <verbosity>                   Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
   -- <options>      Arbitrary trailing options forwarded to dotnet build.
Example: just roslynator-analyze-proj <PROJ_OR_SLN> --exclude tests -v d ")]
[group("roslynator")]
[no-cd]
@roslynator-analyze-proj proj_or_sln verbosity=default_verbosity include='' exclude='' *OPTIONS:
    . "{{ justfile_directory() }}\scripts\just-helpers.ps1"; \
    $values = [System.Collections.Generic.List[string]]::new(); \
    Add-PathFilterArguments $values "{{ include }}" "{{ exclude }}"; \
    roslynator analyze {{ OPTIONS }} -v {{ verbosity }} $values {{ proj_or_sln }}

[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[arg("exclude", long="exclude")]
[arg("include", long="include")]
[arg("remove", long="remove", value="true")]
[arg("symbol-kind", long="symbol-kind")]
[arg("unused", long="unused", value="true")]
[doc("Analyzes root solution and reports diagnostics.
Options:
   --include <include>              Comma separated list of file or folder paths to include. Path is relative to root.
   --exclude <exclude>              Comma separated list of file or folder paths to exclude. Path is relative to root.
   -v <verbosity>                   Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
   --symbol-kind <symbol-kinds>    Comma separated list of symbol kinds to be included: class, delegate, enum, interface, struct, event, field, enum-field, const, method, property, indexer, member and type.
   --unused                         Search for unused symbols.
   --remove                         Remove found symbols' declarations.
   -- <options>      Arbitrary trailing options forwarded to roslynator find-symbol.
Example: just roslynator-find-symbol --symbol-kind class,interface -v d")]
[group("roslynator")]
@roslynator-find-symbol unused='' remove='' verbosity=default_verbosity include='' exclude='' symbol-kind='' *OPTIONS:
    . "{{ justfile_directory() }}\scripts\just-helpers.ps1"; \
    $values = [System.Collections.Generic.List[string]]::new(); \
    Add-PathFilterArguments $values "{{ include }}" "{{ exclude }}"; \
    if ("{{ symbol-kind }}" -ne '') { \
        $values.Add('--symbol-kind'); \
        $values.AddRange("{{ symbol-kind }}" -split ','); \
    }; \
    if ("{{ unused }}" -eq 'true') { \
        $values.Add('--unused'); \
    }; \
    if ("{{ remove }}" -eq 'true') { \
        $values.Add('--remove'); \
    }; \
    roslynator find-symbol {{ OPTIONS }} -v {{ verbosity }} $values

[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[arg("exclude", long="exclude")]
[arg("format", long="format", value="true")]
[arg("ignore-compiler-diagnostics", long="ignore-compiler-diagnostics", value="true")]
[arg("ignore-compiler-errors", long="ignore-compiler-errors", value="true")]
[arg("include", long="include")]
[doc("Fix diagnostics.
Options:
   --include <include>              Comma separated list of file or folder paths to include. Path is relative to root.
   --exclude <exclude>              Comma separated list of file or folder paths to exclude. Path is relative to root.
   -v <verbosity>                   Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
   --format                         Format documents.
   --ignore-compiler-diagnostics    Ignore compiler diagnostics.
   --ignore-compiler-errors         Ignore compiler errors.
   -- <options>      Arbitrary trailing options forwarded to roslynator fix.
Example: just roslynator-fix --ignore-compiler-diagnostics -v d")]
[group("roslynator")]
@roslynator-fix format='' ignore-compiler-diagnostics='' ignore-compiler-errors='' verbosity=default_verbosity include='' exclude='' *OPTIONS:
    . "{{ justfile_directory() }}\scripts\just-helpers.ps1"; \
    $values = [System.Collections.Generic.List[string]]::new(); \
    Add-PathFilterArguments $values "{{ include }}" "{{ exclude }}"; \
    if ("{{ format }}" -eq 'true') { \
        $values.Add('--format'); \
    }; \
    if ("{{ ignore-compiler-diagnostics }}" -eq 'true') { \
        $values.Add('--ignore-compiler-diagnostics'); \
    }; \
        if ("{{ ignore-compiler-errors }}" -eq 'true') { \
        $values.Add('--ignore-compiler-errors'); \
    }; \
    roslynator fix {{ OPTIONS }} -v {{ verbosity }} $values

# endregion

# region Test

# List tests
[arg("format", pattern="^(text|json)$")]
[arg("project", long="project")]
[group("test")]
@list-tests format='text' project='':
    $values = [System.Collections.Generic.List[string]]::new(); \
    if ("{{ project }}" -ne '') { \
        $values.Add('--project'); \
        $values.Add('{{ project }}'); \
    } \
    dotnet test --list-tests {{ format }} $values

[arg("config", short="c", pattern="^(?i)(debug|release)$")]
[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[arg("config-file", long="config-file")]
[arg("results-directory", long="results-directory")]
[arg("results-directory-layout", long="results-directory-layout", pattern="^flat|per-module")]
[arg("timeout", long="timeout", pattern="^\\d+(ms|s|m|h|d)|$")]
[doc("Test (root solution).
Options:
   -c <config>                Case-insensitive config: debug, release
   -v <verbosity>             Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
   --results-directory        The directory where the test results will be placed.
   --results-directory-layout Flat or per-module
   --config-file              Path for config file.
   --timeout                  Timeout for entire execution
   -- <options>               Arbitrary trailing options forwarded to dotnet test.
Example: just test -v detailed --results-directory TestResults -- --coverage")]
[group("test")]
@test config=default_config verbosity=default_verbosity config-file='' results-directory='TestResults' results-directory-layout="per-module" timeout='' *OPTIONS:
    $values = [System.Collections.Generic.List[string]]::new(); \
    if ("{{ config-file }}" -ne '') { \
        $values.Add('--config-file'); \
        $values.Add("{{ config-file }}"); \
    } \
    if ("{{ results-directory }}" -ne '') { \
        $values.Add('--results-directory'); \
        $values.Add("{{ results-directory }}"); \
    } \
    if ("{{ timeout }}" -ne '') { \
        $values.Add('--timeout'); \
        $values.Add('{{ timeout }}'); \
    } \
    dotnet test {{ OPTIONS }} -v {{ verbosity }} -c {{ config }} --results-directory-layout {{ results-directory-layout }} $values

[arg("config", short="c", pattern="^(?i)(debug|release)$")]
[arg("verbosity", short="v", pattern="^(?i)(q(uiet)?|m(inimal)?|n(ormal)?|d(etailed)?|diag(nostics)?)$")]
[arg("config-file", long="config-file")]
[arg("results-directory", long="results-directory")]
[arg("results-directory-layout", long="results-directory-layout", pattern="^flat|per-module")]
[arg("timeout", long="timeout", pattern="^\\d+(ms|s|m|h|d)|$")]
[doc("Test selected project|solution|file.
Options:
   -c <config>                Case-insensitive config: debug, release
   -v <verbosity>             Case-insensitive verbosity: q(uiet), m(inimal), n(ormal), d(etailed), diag(nostics)
   --results-directory        The directory where the test results will be placed.
   --results-directory-layout Flat or per-module
   --config-file              Path for config file.
   --timeout                  Timeout for entire execution
   -- <options>               Arbitrary trailing options forwarded to dotnet test.
Example: just test-proj tests/Dappery.Core.Tests/Dappery.Core.Tests.csproj -v detailed --results-directory TestResults -- --coverage")]
[group("test")]
[no-cd]
@test-proj proj config=default_config verbosity=default_verbosity config-file='' results-directory='TestResults' results-directory-layout="per-module" timeout='' *OPTIONS:
    $values = [System.Collections.Generic.List[string]]::new(); \
    if ("{{ config-file }}" -ne '') { \
        $values.Add('--config-file'); \
        $values.Add("{{ config-file }}"); \
    } \
    if ("{{ results-directory }}" -ne '') { \
        $values.Add('--results-directory'); \
        $values.Add("{{ results-directory }}"); \
    } \
    if ("{{ timeout }}" -ne '') { \
        $values.Add('--timeout'); \
        $values.Add('{{ timeout }}'); \
    } \
    dotnet test {{ OPTIONS }} -v {{ verbosity }} -c {{ config }} --results-directory-layout {{ results-directory-layout }} --project {{ proj }} $values

[arg("config", short="c", pattern="^(?i)(debug|release)$")]
[doc("Run tests with hot reload for selected project.
Sets TESTINGPLATFORM_HOTRELOAD_ENABLED=1 and runs the test project via dotnet run.
Options:
   -c <config>       Case-insensitive config: debug, release
   -- <options>      Arbitrary trailing options forwarded to dotnet run.
Example: just test-hot-reload tests/Dappery.Core.Tests/Dappery.Core.Tests.csproj -c Debug")]
[group("test")]
[no-cd]
test-hot-reload proj config=default_config *OPTIONS:
    $env:TESTINGPLATFORM_HOTRELOAD_ENABLED = '1'; dotnet run --project {{ proj }} -c {{ config }} {{ OPTIONS }}

# endregion
