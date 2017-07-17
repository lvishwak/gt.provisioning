<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>
  <xsl:template match="/">
    
  <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  
    
  </xsl:template>
  <xsl:template match="/">
        <xsl:text xml:space="default">
          <![CDATA[<pnp:Provisioning xmlns:pnp="http://schemas.dev.office.com/PnP/2016/05/ProvisioningSchema"
                  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                  xsi:schemaLocation="http://schemas.dev.office.com/PnP/2016/05/ProvisioningSchema ProvisioningSchema-2016-05.xsd">]]>
        </xsl:text>
        <xsl:text xml:space="default">
          <![CDATA[</pnp:Provisioning>]]>
        </xsl:text>
  </xsl:template>
</xsl:stylesheet>