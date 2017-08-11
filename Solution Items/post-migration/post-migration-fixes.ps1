# Perform Post Migration Fixes on Office 365 Site
#
# Description
# -----------------------------------------------------------------------------
# This script is used to fix below issues after migration
# 1. Update top navigation
# 2. Update Team space web by applying PnP template (teamspace-template.xml) 
# 3. Update Client space web by applying PnP template (clientspace-template.xml)
# 4. Update Vendor space web by applying PnP template (vendorspace-template.xml)
#
# Command line arguments
# -----------------------------------------------------------------------------
# None

# clear current console
#clear

# Input file path
$csvInputFileName = "InputTargetUrls.csv";
$csvInputFile     = $PSScriptRoot + "\" + $csvInputFileName;

# Log file name and path
$LogTime     = Get-Date -Format "MM-dd-yyyy_hh-mm-ss"
$logFilePath = $PSScriptRoot + "\Logs_"  + "\PostOperation_" + $LogTime + ".log"

# credentials to connect to Office 365 site
$userName               = "svc_shpt_admin1@us.gt.com"
$userPassword           = "Ul8Hyrcwe"
$passwordAsSecureString = ConvertTo-SecureString -String $userPassword -AsPlainText -Force
$credentialObject       = new-object -typename System.Management.Automation.PSCredential -argumentlist $userName, $passwordAsSecureString

# provisioning template file paths
$rootWebTemplate     = $PSScriptRoot + "\rootweb-template.xml"
$teamSpaceTemplate   = $PSScriptRoot + "\teamspace-template.xml"
$clientSpaceTemplate = $PSScriptRoot + "\clientspace-template.xml"
$venderSpaceTemplate = $PSScriptRoot + "\vendorspace-template.xml"

function Log{
    [CmdletBinding()]
    Param
    (
        [string] $text
    )

	$text | Out-File -FilePath $logFilePath -Append	
	Write-Host $text
}

function Perform-PostOperation {
    [CmdletBinding()]
    Param 
    (    
        [Parameter(Mandatory=$true, Position=1)]             
        [Microsoft.SharePoint.Client.Web] $web
    )
    
    #get subsites
    $subwebs = Get-PnPSubWebs
    foreach ($subweb in $subwebs) 
    {
        Write-Host "Processing: " $subweb.Url        
        
        Connect-PnPOnline -Url $subweb.Url -Credentials $credentialObject
        if($subweb.Url.ToLower().Contains("clientspace"))
        {            
            # apply provisioning template
            Apply-PnPProvisioningTemplate -Path $clientSpaceTemplate
        }
        
        if( $subweb.Url.ToLower().Contains("vendorspace"))
        {            
            Apply-PnPProvisioningTemplate -Path $venderSpaceTemplate            
        }

        Perform-PostOperation $subweb        
    }   
}

# Load the XML script
Write-Host "Loading post-migration-fixes script..." -ForeGroundColor Green

# import csv file and read each row
$csv = import-csv $csvInputFile
foreach($row in $csv)
{
    try
    {
        # get target Office 365 site collection url
        $siteURL = $row.targetURL
        
        # Connect to Office 365 site 
        Connect-PnPOnline -Url $siteURL -Credentials $credentialObject
                
        # apply provisioning template
        Apply-PnPProvisioningTemplate -Path $rootWebTemplate
                
        # processing team space
        $subwebs = Get-PnPSubWebs
        foreach ($subweb in $subwebs) 
        {
            Write-Host $subweb.Url

            Connect-PnPOnline -Url $subweb.Url -Credentials $credentialObject

            # Remove Records Management links
            #Log "Removing records management links from quick launch"
            Remove-PnPNavigationNode -Title "Records Management" -Location QuickLaunch -Force

            # apply teamspace provisioning template            
            Apply-PnPProvisioningTemplate -Path $teamSpaceTemplate

            Perform-PostOperation $subweb
        }
    }
    catch
    {
      Log "Errors found:`n$_" 
    }
}