Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports org.nd4j.codegen.api
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

Namespace org.nd4j.codegen.impl.cpp


	''' <summary>
	''' A very simple, manual CPP generator
	''' As per Python, this could be implemented using a templating library such as freemarker
	''' </summary>
	Public Class CppGenerator
		Implements Generator

		Public Overrides Function language() As Language
			Return Language.CPP
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void generateNamespaceNd4j(NamespaceOps namespace, org.nd4j.codegen.api.generator.GeneratorConfig config, java.io.File directory, String fileName) throws java.io.IOException
		Public Overrides Sub generateNamespaceNd4j(ByVal [namespace] As NamespaceOps, ByVal config As GeneratorConfig, ByVal directory As File, ByVal fileName As String)

			Dim sb As New StringBuilder()

			sb.Append("#include <NDArrayFactory.h>" & vbLf & vbLf).Append("namespace nd4j {" & vbLf)

			append(4, sb, "namespace " & [namespace].getName().ToLower())

			Dim ops As IList(Of Op) = New List(Of Op)()
			For Each o As Op In [namespace].getOps()
				If o.isAbstract() Then
					Continue For
				End If
				ops.Add(o)
			Next o

			'TODO: handle includes

			For Each o As Op In ops
				Dim s As String = generateFunction(o)
				sb.Append(GenUtil.addIndent(s, 8))
				sb.Append(vbLf)
			Next o

			append(4, sb, "}")
			sb.Append("}")

			'TODO generate header also

			Dim [out] As String = sb.ToString()
			Dim outFile As New File(directory, GenUtil.ensureFirstIsCap([namespace].getName()) & ".cpp")
			FileUtils.writeStringToFile(outFile, [out], StandardCharsets.UTF_8)
		End Sub

		Protected Friend Shared Sub append(ByVal indent As Integer, ByVal sb As StringBuilder, ByVal line As String)
			sb.Append(GenUtil.repeat(" ", indent)).Append(line).Append(vbLf)
		End Sub

		Protected Friend Shared Function generateFunction(ByVal op As Op) As String
			Dim sb As New StringBuilder()

			Dim outputs As IList(Of Output) = op.getOutputs()
			Dim singleOut As Boolean = outputs.Count = 1
			If singleOut Then
				sb.Append("NDArray* ")
			Else
				Throw New System.NotSupportedException("Multi-output op generation not yet implemented")
			End If

			sb.Append(GenUtil.ensureFirstIsNotCap(op.getOpName())).Append("(")

			'Add inputs to signature
			Dim firstArg As Boolean = True
			If op.getInputs() IsNot Nothing Then
				For Each i As Input In op.getInputs()
					If Not firstArg Then
						sb.Append(", ")
					End If

					sb.Append("NDArray* ").Append(i.getName())

					firstArg = False
				Next i
			End If


			'Add arguments and default args to signature
			sb.Append("):" & vbLf)


			sb.Append("    Context c(1);" & vbLf)
			Dim j As Integer=0
			For Each i As Input In op.getInputs()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: sb.append("    c.setInputArray(").append(j++).append(", ").append(i.getName()).append(");" + vbLf);
				sb.Append("    c.setInputArray(").Append(j).Append(", ").Append(i.getName()).Append(");" & vbLf)
					j += 1
			Next i

			sb.Append(vbLf & "    //TODO: args" & vbLf & vbLf)


			sb.Append("    nd4j::ops::").Append(op.getLibnd4jOpName()).Append(" op;" & vbLf)

			sb.Append("    ShapeList shapeList({")
			j = 0
			For Each i As Input In op.getInputs()
				If j > 0 Then
					sb.Append(",")
				End If
				sb.Append(i.getName())
				j += 1
			Next i

			sb.Append("});" & vbLf & vbLf).Append("    auto outShape = op.calculateOutputShape(&shapeList, c);" & vbLf)

			sb.Append("    auto out = nullptr;  //TODO" & vbLf & vbLf).Append("    op.exec(c);" & vbLf).Append("    delete shapes;" & vbLf)

			sb.Append("    return out;" & vbLf).Append("}" & vbLf)


			Return sb.ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void generateNamespaceSameDiff(NamespaceOps namespace, org.nd4j.codegen.api.generator.GeneratorConfig config, java.io.File directory, String fileName) throws java.io.IOException
		Public Overrides Sub generateNamespaceSameDiff(ByVal [namespace] As NamespaceOps, ByVal config As GeneratorConfig, ByVal directory As File, ByVal fileName As String)
			Throw New System.NotSupportedException()
		End Sub
	End Class

End Namespace