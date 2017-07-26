<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

  <xsl:output
    method="xml"
    omit-xml-declaration="yes"
    encoding="utf-8"
    indent="yes"/>

  <xsl:template match="/">
    <xsl:apply-templates />
  </xsl:template>
  
  <xsl:template match="Site">
    <pnp:Provisioning xmlns:pnp="http://schemas.dev.office.com/PnP/2016/05/ProvisioningSchema">
      <pnp:Preferences Generator="OfficeDevPnP.Core, Version=2.6.1608.0, Culture=neutral, PublicKeyToken=3751622786b357c2">
        <pnp:Parameters>
          <pnp:Parameter Key="siteid" Required="true">
            <xsl:value-of select="@sourceSiteId"/>
          </pnp:Parameter>
        </pnp:Parameters>
      </pnp:Preferences>
      <pnp:Templates ID="gt-templates">
        <pnp:ProvisioningTemplate ID="gt-posttaxsymphony-template"
                                  Version="1"
                                  DisplayName="Post TaxSymphony Template"
                                  Description=""
                                  BaseSiteTemplate="STS#0">
          <pnp:Security>
            <pnp:SiteGroups>
              <xsl:for-each select="GroupAssignments/GroupAssignment">
                <xsl:element name="pnp:SiteGroup">
                  <xsl:attribute name="Title">
                    <xsl:value-of select="@group"/>
                  </xsl:attribute>
                  <xsl:attribute name="Owner">
                    <xsl:value-of select="@owner"/>
                  </xsl:attribute>
                  <xsl:for-each select="Users">
                    <xsl:element name="pnp:Members">
                      <xsl:for-each select="User">
                        <xsl:element name="pnp:User">
                          <xsl:attribute name="Name">
                            <xsl:value-of select="@loginName"/>
                          </xsl:attribute>
                        </xsl:element>
                      </xsl:for-each>
                    </xsl:element>
                  </xsl:for-each>
                </xsl:element>
              </xsl:for-each>
            </pnp:SiteGroups>
          </pnp:Security>
          <pnp:Providers>
            <pnp:Provider Enabled="true"
                          HandlerType="GT.Provisioning.Core.ExtensibilityProviders.WebExtensibilityHandler, GT.Provisioning.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
              <WebProviderConfiguration xmlns="http://schemas.sogeti.com/webconfiguration">
                <webs>
                  <web Title="Team Space"
                       Description="Team space"
                       Url="teamspace"
                       Language="1033"
                       InheritPermissions="true"
                       InheritNavigation="true"
                       BaseTemplate="STS#0"
                       PnPTemplate="gt-teamspace-pre-template.xml">
                    <RoleAssignments>
                      <RoleAssignment Principal="Level 1" RoleDefinition="Read" />
                      <RoleAssignment Principal="Level 4" RoleDefinition="Read" />
                    </RoleAssignments>
                  </web>
                  <web Title="Client Space"
                       Description="Client space"
                       Url="clientspace"
                       Language="1033"
                       InheritPermissions="true"
                       InheritNavigation="true"
                       BaseTemplate="STS#0"
                       PnPTemplate="gt-clientspace-post-template.xml">
                    <RoleAssignments>
                      <RoleAssignment Principal="Level 1" RoleDefinition="Read" />
                      <RoleAssignment Principal="Level 4" RoleDefinition="Read" />
                    </RoleAssignments>
                  </web>
                </webs>
              </WebProviderConfiguration>
            </pnp:Provider>
            <pnp:Provider Enabled="true"
                          HandlerType="GT.Provisioning.Core.ExtensibilityProviders.RoleAssignmentExtensibilityHandler, GT.Provisioning.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
              <pnp:Configuration>
                <Migrate xmlns="http://schemas.sogeti.com/migratelibraryconfiguration">
                  <xsl:apply-templates select="Lists"></xsl:apply-templates>
                </Migrate>
              </pnp:Configuration>
            </pnp:Provider>
          </pnp:Providers>
        </pnp:ProvisioningTemplate>
      </pnp:Templates>
    </pnp:Provisioning>
  </xsl:template>
</xsl:stylesheet>