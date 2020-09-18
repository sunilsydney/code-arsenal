<#
  .SYNOPSIS
  Gets a SAS token to an Event Hub, that can be used to submit events.

  .DESCRIPTION
  An Event Hub namespace in azure portal is required to use this script

  .PARAMETER EventHubURI
  URI of the event hub, in the format abc.servicebus.windows.net/xyz

  .PARAMETER AccessPolicyKey
  RootManageSharedAccessKey of the EventHub

  .EXAMPLE
  C:\Code\Scripts> .\Get_SAS_Token_Eventhub.ps1 -EventHubURI abc.servicebus.windows.net/xyz -AccessPolicyKey abcd123
#>


param (
    [Parameter(Mandatory = $true)][string]$EventHubURI,
    [Parameter(Mandatory = $true)][string]$AccessPolicyKey
)
$PSVersionTable.PSVersion 
[Reflection.Assembly]::LoadWithPartialName("System.Web")| out-null
$URI= $EventHubURI                       
$Access_Policy_Name="RootManageSharedAccessKey"
$Access_Policy_Key= $AccessPolicyKey;
#Token expires now + 1 hour
$Expires=([DateTimeOffset]::Now.ToUnixTimeSeconds())+ (60*60)
$SignatureString=[System.Web.HttpUtility]::UrlEncode($URI)+ "`n" + [string]$Expires
$SignatureString
$HMAC = New-Object System.Security.Cryptography.HMACSHA256
$HMAC.key = [Text.Encoding]::ASCII.GetBytes($Access_Policy_Key)
$Signature = $HMAC.ComputeHash([Text.Encoding]::ASCII.GetBytes($SignatureString))
$Signature = [Convert]::ToBase64String($Signature)
$SASToken = "SharedAccessSignature sr=" + [System.Web.HttpUtility]::UrlEncode($URI) + "&sig=" + [System.Web.HttpUtility]::UrlEncode($Signature) + "&se=" + $Expires + "&skn=" + $Access_Policy_Name
Write-Host "SAS Token: "
$SASToken