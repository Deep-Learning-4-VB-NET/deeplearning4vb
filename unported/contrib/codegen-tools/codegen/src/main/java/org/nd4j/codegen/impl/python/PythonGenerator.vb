Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports org.nd4j.codegen.api
Imports DocTokens = org.nd4j.codegen.api.doc.DocTokens
Imports Generator = org.nd4j.codegen.api.generator.Generator
Imports GeneratorConfig = org.nd4j.codegen.api.generator.GeneratorConfig
Imports GenUtil = org.nd4j.codegen.util.GenUtil

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.codegen.impl.python


	''' <summary>
	''' This is a very simple, manual namespace generator
	''' We could of course use a templating library such as Freemarker, which woudl work fine - but:
	''' (a) on the one hand, it's overkill/unnecessary
	''' (b) on the other hand, may provide less flexibility than a manual implementation
	''' 
	''' </summary>
	Public Class PythonGenerator
		Implements Generator

		Private Const I4 As String = "    "

		Public Overrides Function language() As Language
			Return Language.PYTHON
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void generateNamespaceNd4j(NamespaceOps namespace, org.nd4j.codegen.api.generator.GeneratorConfig config, java.io.File directory, String fileName) throws java.io.IOException
		Public Overrides Sub generateNamespaceNd4j(ByVal [namespace] As NamespaceOps, ByVal config As GeneratorConfig, ByVal directory As File, ByVal fileName As String)


			Dim sb As New StringBuilder()
			sb.Append("class ").Append(GenUtil.ensureFirstIsCap([namespace].getName())).Append(":" & vbLf & vbLf)

			Dim ops As IList(Of Op) = New List(Of Op)()
			For Each o As Op In [namespace].getOps()
				If o.isAbstract() Then
					Continue For
				End If
				ops.Add(o)
			Next o

			'TODO: handle includes

			For Each o As Op In ops
				Dim s As String = generateMethod(o)
				sb.Append(GenUtil.addIndent(s, 4))
				sb.Append(vbLf & vbLf)
			Next o

			Dim f As New File(directory, GenUtil.ensureFirstIsCap([namespace].getName()) & ".py")
			Dim content As String = sb.ToString()

			FileUtils.writeStringToFile(f, content, StandardCharsets.UTF_8)
		End Sub

		Protected Friend Shared Function generateMethod(ByVal op As Op) As String
			Dim sb As New StringBuilder()
			sb.Append("@staticmethod" & vbLf).Append("def ").Append(GenUtil.ensureFirstIsNotCap(op.getOpName())).Append("(")

			'Add inputs to signature
			Dim firstArg As Boolean = True
			If op.getInputs() IsNot Nothing Then
				For Each i As Input In op.getInputs()
					If Not firstArg Then
						sb.Append(", ")
					End If

					sb.Append(i.getName())

					firstArg = False
				Next i
			End If


			'Add arguments and default args to signature

			sb.Append("):" & vbLf)

			Dim docString As String = genDocString(op)
			sb.Append(GenUtil.addIndent(docString, 4))

			sb.Append(I4).Append("# Execution code here" & vbLf)


			Return sb.ToString()
		End Function

		Protected Friend Shared Function genDocString(ByVal op As Op) As String
			'Following roughly: https://sphinxcontrib-napoleon.readthedocs.io/en/latest/example_google.html
			Dim sb As New StringBuilder()
			sb.Append("""""""").Append(op.getOpName()).Append(" operation").Append(vbLf & vbLf)
			If op.getInputs() IsNot Nothing Then
				sb.Append("Args:")
				sb.Append(vbLf)
				For Each i As Input In op.getInputs()
					sb.Append(I4).Append(i.getName()).Append(" (ndarray): ")
					If i.getDescription() IsNot Nothing Then
						sb.Append(DocTokens.processDocText(i.getDescription(), op, DocTokens.GenerationType.ND4J))
					End If
					sb.Append(vbLf)
				Next i
			End If

			sb.Append(vbLf)

			If op.getOutputs() IsNot Nothing Then
				sb.Append("Returns:" & vbLf)
				Dim o As IList(Of Output) = op.getOutputs()

				If o.Count = 1 Then
					sb.Append(I4).Append("ndarray: ").Append(o(0).getName())
					Dim d As String = o(0).getDescription()
					If d IsNot Nothing Then
						sb.Append(" - ").Append(DocTokens.processDocText(d, op, DocTokens.GenerationType.ND4J))
					End If
					sb.Append(vbLf)
				Else
					Throw New System.NotSupportedException("Not yet implemented: Python docstring generation for multiple output ops")
				End If
			End If

			If op.getArgs() IsNot Nothing Then
				'Args and default args
				Throw New System.NotSupportedException("Generating method with args not yet implemented")
			End If

			sb.Append("""""""" & vbLf)

			Return sb.ToString()
		End Function



'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void generateNamespaceSameDiff(NamespaceOps namespace, org.nd4j.codegen.api.generator.GeneratorConfig config, java.io.File directory, String fileName) throws java.io.IOException
		Public Overrides Sub generateNamespaceSameDiff(ByVal [namespace] As NamespaceOps, ByVal config As GeneratorConfig, ByVal directory As File, ByVal fileName As String)
			Throw New System.NotSupportedException("Not yet implemented")
		End Sub
	End Class

End Namespace