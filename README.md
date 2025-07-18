For publishing Function App (Python) using basic Auth

1.
Delete the zip

2.
Recreate the zip
Compress-Archive -Path * -DestinationPath functionapp.zip -Force


3. Run below pwsh to publish using basic auth.
# Set credentials from publish profile
$user = '$suroutfuncapp-python'
$pass = 'redacted'

# Encode for Basic Auth
$bytes = [System.Text.Encoding]::ASCII.GetBytes("$user`:$pass")
$base64 = [Convert]::ToBase64String($bytes)
$headers = @{ Authorization = "Basic $base64" }

# Read ZIP file as byte array
$zipContent = [System.IO.File]::ReadAllBytes("functionapp.zip")

# Deploy to Azure using ZipDeploy
Invoke-RestMethod `
  -Uri "https://suroutfuncapp-python-dwdqejcyhxd0azbz.scm.canadacentral-01.azurewebsites.net/api/zipdeploy" `
  -Method POST `
  -Body $zipContent `
  -Headers $headers `
  -ContentType "application/zip"
