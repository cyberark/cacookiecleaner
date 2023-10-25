try {
    
    $inputFile = Read-Host "insert full directory of the har file -> "
    $outputFile = Read-Host "insert full path to the output file -> "

    if ($inputFile -match "\.har$") {
        $harData = Get-Content -Path $inputFile | ConvertFrom-Json

        
        foreach ($entry in $harData.log.entries) 
        {
            if ($entry.request -ne $null -and $entry.request.headers -ne $null) 
            {
                $entry.request.headers = $entry.request.headers | Where-Object { $_.name -notlike "*cookie*" }
            }
            if ($entry.response -ne $null -and $entry.response.headers -ne $null) {
                $entry.response.headers = $entry.response.headers | Where-Object { $_.name -notlike "*set-cookie*" }
            }
        }

        $harData | ConvertTo-Json -Depth 10 | Set-Content -Path $outputFile -Force

        Write-Host "Cookies cleared successfully"
    } 
    else 
    {
        Write-Host "File must have a .har extension"
    }
} 
catch 
{
    Write-Host "Error: $($_.Exception.Message)"
}
