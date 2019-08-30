<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
    version="2.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    >
  <xsl:template match="/Resource">
    <html>
      <head>
        <meta charset="UTF-8"/>
        <title></title>
        <link rel="stylesheet" type="text/css" href="index.css"/>
      </head>
      <body>
        <h1>Index of <xsl:value-of select="Path"/></h1>

        <xsl:if test="Path = '/'">
          <div class="div-actions">
            <a href="Reindex">Update Index</a>
          </div>
        </xsl:if>

        <div class="div-index">
          <table>
            <thead>
              <tr>
                <th class="col col-name">Name</th>
                <th class="col col-size">Size</th>
                <th class="col col-date">Date</th>
              </tr>
            </thead>
          </table>
          <hr/>
          <table>
            <tbody>

              <xsl:if test="Path != '/'">
                <tr>
                  <td class="col col-name">
                    <a href="..">..</a>
                  </td>
                  <td class="col col-size"></td>
                  <td class="col col-date"></td>
                </tr>
              </xsl:if>

              <xsl:for-each select="Items/Item">
                <xsl:element name="tr">

                  <xsl:choose>
                    <xsl:when test="IsFolder = 'true'">
                      <xsl:attribute name="class">folder</xsl:attribute>
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:attribute name="class">file</xsl:attribute>
                    </xsl:otherwise>
                  </xsl:choose>

                  <td class="col col-name">
                    <xsl:choose>

                      <xsl:when test="IsFolder = 'true'">
                        <xsl:element name="a">
                          <xsl:attribute name="href">
                            <xsl:value-of select="concat(Name, '/')"/>
                          </xsl:attribute>
                          <xsl:value-of select="concat(Name, '/')"/>
                        </xsl:element>
                      </xsl:when>

                      <xsl:otherwise>
                        <xsl:element name="a">
                          <xsl:attribute name="href">
                            <xsl:value-of select="Name"/>
                          </xsl:attribute>
                          <xsl:value-of select="Name"/>
                        </xsl:element>
                      </xsl:otherwise>

                    </xsl:choose>
                  </td>

                  <td class="col col-size">
                    <xsl:if test="IsFolder = 'false'">
                      <xsl:value-of select="Length"/>
                    </xsl:if>
                  </td>

                  <td class="col col-date">
                    <xsl:value-of select="
                      concat(
                        substring(Date, 1, 10),
                        ' ',
                        substring(Date, 12, 8)
                      )
                    "/>
                  </td>

                </xsl:element>
              </xsl:for-each>

            </tbody>
          </table>
          <hr/>
        </div>

      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>