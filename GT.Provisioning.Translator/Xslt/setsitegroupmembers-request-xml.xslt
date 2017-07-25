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
            <xsl:value-of select="@id"/>
          </pnp:Parameter>
        </pnp:Parameters>
      </pnp:Preferences>
      <pnp:Templates ID="gt-templates">
        <pnp:ProvisioningTemplate ID="gt-setsitegroupmembers-template"
                                  Version="1"
                                  DisplayName="Set SiteGroupMembers Template"
                                  Description=""
                                  BaseSiteTemplate="STS#0">          
          <pnp:Providers>
            <pnp:Provider Enabled="true"
                          HandlerType="GT.Provisioning.Core.ExtensibilityProviders.RoleAssignmentExtensibilityHandler, GT.Provisioning.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
              <pnp:Configuration>
                <GroupAssignmentConfiguration xmlns="http://schemas.sogeti.com/GroupAssignmentConfiguration">
                  <GroupAssignments>
                    <xsl:apply-templates select="GroupAssignments"></xsl:apply-templates>
                  </GroupAssignments>
                </GroupAssignmentConfiguration>
              </pnp:Configuration>
            </pnp:Provider>
          </pnp:Providers>
        </pnp:ProvisioningTemplate>
      </pnp:Templates>
    </pnp:Provisioning>
  </xsl:template>

  <xsl:template match="GroupAssignments" name="GroupAssignments">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="GroupAssignment" name="GroupAssignment">
    <xsl:element name="GroupAssignment">
      <xsl:attribute name="group">
        <xsl:value-of select="@group"/>
      </xsl:attribute>
      <xsl:apply-templates select="Users"></xsl:apply-templates>
    </xsl:element>
  </xsl:template>

  <xsl:template match="Users" name="Users">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="User" name="User">
    <xsl:element name="User">
      <xsl:attribute name="loginName">
        <xsl:value-of select="@loginName"/>
      </xsl:attribute>
      <xsl:attribute name="action">
        <xsl:value-of select="@action"/>
      </xsl:attribute>
    </xsl:element>
  </xsl:template>

</xsl:stylesheet>