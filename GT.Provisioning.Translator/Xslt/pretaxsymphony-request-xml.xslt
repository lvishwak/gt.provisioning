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
        <pnp:ProvisioningTemplate ID="gt-pretaxsymphony-template"
                                  Version="1"
                                  DisplayName="Pre TaxSymphony Template"
                                  Description=""
                                  BaseSiteTemplate="STS#0">
          <xsl:if test="GroupAssignments">
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
          </xsl:if>
          <xsl:if test="TeamSpace">
            <pnp:Providers>
              <pnp:Provider Enabled="true"
                            HandlerType="GT.Provisioning.Core.ExtensibilityProviders.WebExtensibilityHandler, GT.Provisioning.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
                <pnp:Configuration>
                  <xsl:call-template name="teamSpace"></xsl:call-template>
                </pnp:Configuration>
              </pnp:Provider>
            </pnp:Providers>
          </xsl:if>
        </pnp:ProvisioningTemplate>
      </pnp:Templates>
    </pnp:Provisioning>
  </xsl:template>

  <xsl:template match="TeamSpace" name="teamSpace">
    <WebProviderConfiguration xmlns="http://schemas.sogeti.com/webconfiguration">
      <webs>
        <web Title="Team Space"
             Description="Team space"
             Url="clientnumber">
          <xsl:if test="TeamSpace/Lists">
            <Lists>
              <xsl:for-each select="TeamSpace/Lists/List">
                <xsl:element name="List">
                  <xsl:attribute name="name">
                    <xsl:value-of select="@name"/>
                  </xsl:attribute>
                  <xsl:attribute name="url">
                    <xsl:value-of select="@url"/>
                  </xsl:attribute>
                  <xsl:attribute name="templateType">
                    <xsl:value-of select="@templateType"/>
                  </xsl:attribute>
                  <xsl:element name="Folders">
                    <xsl:for-each select="Folders/Folder">
                      <xsl:element name="Folder">
                        <xsl:attribute name="name">
                          <xsl:value-of select="@name"/>
                        </xsl:attribute>
                      </xsl:element>
                    </xsl:for-each>
                  </xsl:element>
                </xsl:element>
              </xsl:for-each>
            </Lists>
          </xsl:if>
          <xsl:if test="TeamSpace/Properties">
            <Properties>
              <xsl:for-each select="TeamSpace/Properties/Property">
                <xsl:element name="Property">
                  <xsl:attribute name="name">
                    <xsl:value-of select="@name"/>
                  </xsl:attribute>
                  <xsl:attribute name="value">
                    <xsl:value-of select="@value"/>
                  </xsl:attribute>
                </xsl:element>
              </xsl:for-each>
            </Properties>
          </xsl:if>
        </web>
      </webs>
    </WebProviderConfiguration>
  </xsl:template>

</xsl:stylesheet>