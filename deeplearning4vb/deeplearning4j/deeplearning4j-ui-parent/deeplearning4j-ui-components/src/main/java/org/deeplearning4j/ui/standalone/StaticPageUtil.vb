Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Configuration = freemarker.template.Configuration
Imports Template = freemarker.template.Template
Imports TemplateExceptionHandler = freemarker.template.TemplateExceptionHandler
Imports Version = freemarker.template.Version
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Component = org.deeplearning4j.ui.api.Component
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports MapperFeature = org.nd4j.shade.jackson.databind.MapperFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.ui.standalone


	Public Class StaticPageUtil

		Private Sub New()
		End Sub

		''' <summary>
		''' Given the specified components, render them to a stand-alone HTML page (which is returned as a String)
		''' </summary>
		''' <param name="components"> Components to render </param>
		''' <returns> Stand-alone HTML page, as a String </returns>
		Public Shared Function renderHTML(ByVal components As ICollection(Of Component)) As String
			Return renderHTML(components.ToArray())
		End Function

		''' <summary>
		''' Given the specified components, render them to a stand-alone HTML page (which is returned as a String)
		''' </summary>
		''' <param name="components"> Components to render </param>
		''' <returns> Stand-alone HTML page, as a String </returns>
		Public Shared Function renderHTML(ParamArray ByVal components() As Component) As String
			Try
				Return renderHTMLContent(components)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String renderHTMLContent(org.deeplearning4j.ui.api.Component... components) throws Exception
		Public Shared Function renderHTMLContent(ParamArray ByVal components() As Component) As String

			Dim mapper As New ObjectMapper()
			mapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			mapper.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			mapper.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
			mapper.enable(SerializationFeature.INDENT_OUTPUT)

			Dim cfg As New Configuration(New Version(2, 3, 23))

			' Where do we load the templates from:
			cfg.setClassForTemplateLoading(GetType(StaticPageUtil), "")

			' Some other recommended settings:
			cfg.setIncompatibleImprovements(New Version(2, 3, 23))
			cfg.setDefaultEncoding("UTF-8")
			cfg.setLocale(Locale.US)
			cfg.setTemplateExceptionHandler(TemplateExceptionHandler.RETHROW_HANDLER)

			Dim cpr As New ClassPathResource("assets/dl4j-ui.js")
			Dim scriptContents As String = IOUtils.toString(cpr.InputStream, "UTF-8")

			Dim pageElements As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim list As IList(Of ComponentObject) = New List(Of ComponentObject)()
			Dim i As Integer = 0
			For Each c As Component In components
				list.Add(New ComponentObject(i.ToString(), mapper.writeValueAsString(c)))
				i += 1
			Next c
			pageElements("components") = list
			pageElements("scriptcontent") = scriptContents


			Dim template As Template = cfg.getTemplate("staticpage.ftl")
			Dim stringWriter As Writer = New StringWriter()
			template.process(pageElements, stringWriter)

			Return stringWriter.ToString()
		End Function

		''' <summary>
		''' A version of <seealso cref="renderHTML(Component...)"/> that exports the resulting HTML to the specified path.
		''' </summary>
		''' <param name="outputPath"> Output path </param>
		''' <param name="components"> Components to render </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void saveHTMLFile(String outputPath, org.deeplearning4j.ui.api.Component... components) throws java.io.IOException
		Public Shared Sub saveHTMLFile(ByVal outputPath As String, ParamArray ByVal components() As Component)
			saveHTMLFile(New File(outputPath))
		End Sub

		''' <summary>
		''' A version of <seealso cref="renderHTML(Component...)"/> that exports the resulting HTML to the specified File.
		''' </summary>
		''' <param name="outputFile"> Output path </param>
		''' <param name="components"> Components to render </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void saveHTMLFile(java.io.File outputFile, org.deeplearning4j.ui.api.Component... components) throws java.io.IOException
		Public Shared Sub saveHTMLFile(ByVal outputFile As File, ParamArray ByVal components() As Component)
			FileUtils.writeStringToFile(outputFile, renderHTML(components))
		End Sub
	End Class

End Namespace