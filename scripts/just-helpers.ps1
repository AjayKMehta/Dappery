function Add-PathFilterArguments {
    param (
        [System.Collections.Generic.List[string]] $Arguments,
        [AllowEmptyString()]
        [string] $Include,
        [AllowEmptyString()]
        [string] $Exclude
    )

    if ($Include -ne '') {
        $Arguments.Add('--include')
        $Arguments.AddRange($Include -split ',')
    }

    if ($Exclude -ne '') {
        $Arguments.Add('--exclude')
        $Arguments.AddRange($Exclude -split ',')
    }
}
