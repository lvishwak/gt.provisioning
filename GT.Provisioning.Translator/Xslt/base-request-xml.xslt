﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="/">
      <pnp:Provisioning xmlns:pnp="http://schemas.dev.office.com/PnP/2016/05/ProvisioningSchema"></pnp:Provisioning>
    </xsl:template>
</xsl:stylesheet>
