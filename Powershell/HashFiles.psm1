function OutHashList 
{
param (
    [CmdletBinding()]

    [Parameter(Position=0,Mandatory=$true)]
    [string] $directory,
    [switch] $Recurse,
    [switch] $FullPath,
    [string[]] $Include,
    [Parameter()]
    [string] $Algorithm = 'MD5'
)

    $Files = Get-ChildItem $directory -Recurse:$Recurse.IsPresent -Include $Include

    $output = foreach($file in $Files)
    {
        $hashOutput = Get-FileHash $file.FullName -Algorithm $Algorithm 

        if ($null -ne $hashOutput) 
        {
            if($FullPath.IsPresent)
            {
                "$($hashOutput.Hash) $($hashOutput.Path)"
            }
            else 
            {
                "$($hashOutput.Hash) $(Split-Path $hashOutput.Path -leaf)"
            }
        }
    }

    $output | Format-Table -GroupBy Path | Out-File Hash.md5
}