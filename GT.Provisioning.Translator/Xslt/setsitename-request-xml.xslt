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
        <pnp:ProvisioningTemplate ID="gt-setsitename-template"
                                  Version="1"
                                  DisplayName="Set Sitename Template"
                                  Description=""
                                  BaseSiteTemplate="STS#0">
          <xsl:element name="pnp:WebSettings">
            <xsl:attribute name="Title">
              <xsl:value-of select="@name"/>
            </xsl:attribute>
          </xsl:element>
        </pnp:ProvisioningTemplate>
      </pnp:Templates>
    </pnp:Provisioning>
  </xsl:template>

</xsl:stylesheet>