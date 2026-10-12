Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports MethodSpec = com.squareup.javapoet.MethodSpec
Imports TypeName = com.squareup.javapoet.TypeName
Imports FileUtils = org.apache.commons.io.FileUtils
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports org.nd4j.codegen.api
Imports DocSection = org.nd4j.codegen.api.doc.DocSection
Imports DocTokens = org.nd4j.codegen.api.doc.DocTokens
Imports GenUtil = org.nd4j.codegen.util.GenUtil
import static org.nd4j.codegen.impl.java.Nd4jNamespaceGenerator.exactlyOne

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

Namespace org.nd4j.codegen.impl.java


	Public Class DocsGenerator

		' Markdown marker for start-end of code section
		Private Const MD_CODE As String = "```"
		' Javadoc constants which should be dropped or replaced for markdown generation
		Private Const JD_CODE As String = "{@code "
		Private Const JD_CODE_END As String = "}"
		Private Const JD_INPUT_TYPE As String = "%INPUT_TYPE%"

		Public Class JavaDocToMDAdapter
			Friend current As String

			Public Sub New(ByVal original As String)
				Me.current = original
			End Sub

			Public Overridable Function filter(ByVal pattern As String, ByVal replaceWith As String) As JavaDocToMDAdapter
				Dim result As String = StringUtils.replace(current, pattern, replaceWith)
				Me.current = result
				Return Me
			End Function

			Public Overrides Function ToString() As String
				Return current
			End Function
		End Class

		Private Shared Function generateMethodText(ByVal op As Op, ByVal s As Signature, ByVal isSameDiff As Boolean, ByVal isLoss As Boolean, ByVal withName As Boolean) As String
			Dim sb As New StringBuilder()
			Dim c As MethodSpec.Builder = MethodSpec.methodBuilder(GenUtil.ensureFirstIsNotCap(op.getOpName()))
			Dim params As IList(Of Parameter) = s.getParameters()
			Dim outs As IList(Of Output) = op.getOutputs()
			Dim retType As String = "void"

			If outs.Count = 1 Then
				retType = If(isSameDiff, "SDVariable", "INDArray")
			ElseIf outs.Count >= 1 Then
				retType = If(isSameDiff, "SDVariable[]", "INDArray[]")
			End If
			sb.Append(retType).Append(" ").Append(op.getOpName()).Append("(")
			Dim first As Boolean = True
			For Each param As Parameter In params
				If TypeOf param Is Arg Then
					Dim arg As Arg = CType(param, Arg)
					If Not first Then
						sb.Append(", ")
					ElseIf withName Then
						sb.Append("String name, ")
					End If
					Dim className As String
					If arg.getType() = DataType.ENUM Then
						className = GenUtil.ensureFirstIsCap(arg.name())
					Else
						Dim tu As TypeName = Nd4jNamespaceGenerator.getArgType(arg)
						className = tu.ToString()
					End If
					If className.Contains(".") Then
						className = className.Substring(className.LastIndexOf("."c)+1)
					End If
					sb.Append(className).Append(" ").Append(arg.name())
					first = False
				ElseIf TypeOf param Is Input Then
					Dim arg As Input = CType(param, Input)
					If Not first Then
						sb.Append(", ")
					ElseIf withName Then
						sb.Append("String name, ")
					End If
					sb.Append(If(isSameDiff, "SDVariable ", "INDArray ")).Append(arg.name())
					first = False
				ElseIf TypeOf param Is Config Then
					If Not first Then
						sb.Append(", ")
					End If
					Dim conf As Config = CType(param, Config)
					Dim name As String = conf.getName()
					sb.Append(name).Append(" ").Append(GenUtil.ensureFirstIsNotCap(name))
				End If
			Next param
			sb.Append(")")
			Return sb.ToString()
		End Function

		Private Shared Function buildDocSectionText(ByVal docSections As IList(Of DocSection)) As StringBuilder
			Dim sb As New StringBuilder()
			For Each ds As DocSection In docSections
				'if(ds.applies(Language.JAVA, CodeComponent.OP_CREATOR)){
				Dim text As String = ds.getText()
				Dim lines() As String = text.Split(vbLf, True)
				For i As Integer = 0 To lines.Length - 1
					If Not lines(i).EndsWith("<br>", StringComparison.Ordinal) Then
						Dim filteredLine As String = (New JavaDocToMDAdapter(lines(i))).filter(JD_CODE, "`").filter(JD_CODE_END, "`").filter(JD_INPUT_TYPE, "INDArray").ToString()

						lines(i) = filteredLine & Environment.NewLine
					End If
				Next i
				text = String.join(vbLf, lines)
				sb.Append(text).Append(Environment.NewLine)
				'}
			Next ds
			Return sb
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void generateDocs(int namespaceNum, NamespaceOps namespace, String docsDirectory, String basePackage) throws java.io.IOException
		Public Shared Sub generateDocs(ByVal namespaceNum As Integer, ByVal [namespace] As NamespaceOps, ByVal docsDirectory As String, ByVal basePackage As String)
			Dim outputDirectory As New File(docsDirectory)
			Dim sb As New StringBuilder()
			Dim headerName As String = [namespace].getName()
			If headerName.StartsWith("SD", StringComparison.Ordinal) Then
				headerName = headerName.Substring(2)
			End If

			' File Header for Gitbook
			sb.Append("---").Append(Environment.NewLine)
			sb.Append("title: ").Append(headerName).Append(Environment.NewLine)
			sb.Append("short_title: ").Append(headerName).Append(Environment.NewLine)
			sb.Append("description: ").Append(Environment.NewLine)
			sb.Append("category: Operations").Append(Environment.NewLine)
			sb.Append("weight: ").Append(namespaceNum * 10).Append(Environment.NewLine)
			sb.Append("---").Append(Environment.NewLine)

			Dim ops As IList(Of Op) = [namespace].getOps()

			ops.Sort(System.Collections.IComparer.comparing(AddressOf Op.getOpName))

			If ops.Count > 0 Then
				sb.Append("# Operation classes").Append(Environment.NewLine)
			End If
			For Each op As Op In ops
				sb.Append("## ").Append(op.getOpName()).Append(Environment.NewLine)
				Dim doc As IList(Of DocSection) = op.getDoc()
				If doc.Count > 0 Then
					Dim first As Boolean = True
					Dim ndSignatures As New StringBuilder()
					Dim sdSignatures As New StringBuilder()
					Dim sdNameSignatures As New StringBuilder()
					For Each s As Signature In op.getSignatures()
						If first Then
							Dim lang As Language = doc(0).getLanguage()
							sb.Append(MD_CODE).Append(If(lang.Equals(Language.ANY), Language.JAVA, lang)).Append(Environment.NewLine)
							first = False
						End If
						Dim ndCode As String = generateMethodText(op, s, False, False, False)
						ndSignatures.Append(ndCode).Append(Environment.NewLine)
						Dim sdCode As String = generateMethodText(op, s, True, False, False)
						sdSignatures.Append(sdCode).Append(Environment.NewLine)
						Dim withNameCode As String = generateMethodText(op, s, True, False, True)
						sdNameSignatures.Append(withNameCode).Append(Environment.NewLine)
					Next s
					sb.Append(ndSignatures.ToString())
					sb.Append(Environment.NewLine) ' New line between INDArray and SDVariable signatures

					sb.Append(sdSignatures.ToString())
					sb.Append(sdNameSignatures.ToString())

					sb.Append(MD_CODE).Append(Environment.NewLine)
					Dim tsb As StringBuilder = buildDocSectionText(doc)
					sb.Append(tsb.ToString())
					Dim l As IList(Of Signature) = op.getSignatures()
					Dim alreadySeen As ISet(Of Parameter) = New HashSet(Of Parameter)()
					For Each s As Signature In l
						Dim params As IList(Of Parameter) = s.getParameters()
						For Each p As Parameter In params
							If alreadySeen.Contains(p) Then
								Continue For
							End If
							alreadySeen.Add(p)
							If TypeOf p Is Input Then
								Dim i As Input = CType(p, Input)
								sb.Append("* **").Append(i.getName()).Append("** ").Append(" (").Append(i.getType()).Append(") - ").Append(If(i.getDescription() Is Nothing, "", DocTokens.processDocText(i.getDescription(), op, DocTokens.GenerationType.ND4J))).Append(Environment.NewLine)
							ElseIf TypeOf p Is Config Then
								Dim c As Config = CType(p, Config)
								sb.Append("* **").Append(c.getName()).Append("** - see ").Append("[").Append(c.getName()).Append("]").Append("(#").Append(toAnchor(c.getName())).Append(")").Append(Environment.NewLine)
							ElseIf TypeOf p Is Arg Then
								Dim arg As Arg = CType(p, Arg)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Count count = arg.getCount();
								Dim count As Count = arg.getCount()
								If count Is Nothing OrElse count.Equals(exactlyOne) Then
									sb.Append("* **").Append(arg.getName()).Append("** - ").Append(If(arg.getDescription() Is Nothing, "", DocTokens.processDocText(arg.getDescription(), op, DocTokens.GenerationType.ND4J))) '.append(System.lineSeparator());
								Else
									sb.Append("* **").Append(arg.getName()).Append("** - ").Append(If(arg.getDescription() Is Nothing, "", DocTokens.processDocText(arg.getDescription(), op, DocTokens.GenerationType.ND4J))).Append(" (Size: ").Append(count.ToString()).Append(")") '.append(System.lineSeparator());
								End If

								Dim defaultValue As Object = arg.defaultValue()
								If defaultValue IsNot Nothing Then
									sb.Append(" - default = ").Append(formatDefaultValue(defaultValue))
								End If

								sb.Append(Environment.NewLine)
							End If
						Next p
					Next s
					sb.Append(Environment.NewLine)
				End If
			Next op


			If [namespace].getConfigs().size() > 0 Then
				sb.Append("# Configuration Classes").Append(Environment.NewLine)
			End If
			For Each config As Config In [namespace].getConfigs()
				sb.Append("## ").Append(config.getName()).Append(Environment.NewLine)
				For Each i As Input In config.getInputs()
					sb.Append("* **").Append(i.getName()).Append("**- ").Append(i.getDescription()).Append(" (").Append(i.getType()).Append(" type)")
					If i.hasDefaultValue() AndAlso (i.defaultValue() IsNot Nothing) Then
						sb.Append(" Default value:").Append(formatDefaultValue(i.defaultValue())).Append(Environment.NewLine)
					Else
						sb.Append(Environment.NewLine)
					End If
				Next i
				For Each arg As Arg In config.getArgs()
					sb.Append("* **").Append(arg.getName()).Append("** ").Append("(").Append(arg.getType()).Append(") - ").Append(arg.getDescription())
					If arg.hasDefaultValue() AndAlso (arg.defaultValue() IsNot Nothing) Then
						sb.Append(" - default = ").Append(formatDefaultValue(arg.defaultValue())).Append(Environment.NewLine)
					Else
						sb.Append(Environment.NewLine)
					End If
				Next arg
				Dim tsb As StringBuilder = buildDocSectionText(config.getDoc())
				sb.Append(tsb.ToString())
				sb.Append(Environment.NewLine)
				For Each op As Op In ops
					If op.getConfigs().contains(config) Then
						sb.Append("Used in these ops: " & Environment.NewLine)
						Exit For
					End If
				Next op
				ops.Where(Function(op) op.getConfigs().contains(config)).ForEach(Function(op) sb.Append("[").Append(op.getOpName()).Append("]").Append("(#").Append(toAnchor(op.getOpName())).Append(")").Append(Environment.NewLine))

			Next config
			Dim outFile As New File(outputDirectory & "/operation-namespaces", "/" & [namespace].getName().ToLower() & ".md")
			FileUtils.writeStringToFile(outFile, sb.ToString(), StandardCharsets.UTF_8)
		End Sub

		Private Shared Function formatDefaultValue(ByVal v As Object) As String
			If v Is Nothing Then
				Return "null"
			ElseIf TypeOf v Is Integer() Then
				Return java.util.Arrays.toString(DirectCast(v, Integer()))
			ElseIf TypeOf v Is Long() Then
				Return java.util.Arrays.toString(DirectCast(v, Long()))
			ElseIf TypeOf v Is Single() Then
				Return java.util.Arrays.toString(DirectCast(v, Single()))
			ElseIf TypeOf v Is Double() Then
				Return java.util.Arrays.toString(DirectCast(v, Double()))
			ElseIf TypeOf v Is Boolean() Then
				Return java.util.Arrays.toString(DirectCast(v, Boolean()))
			ElseIf TypeOf v Is Input Then
				Return DirectCast(v, Input).getName()
			ElseIf TypeOf v Is org.nd4j.linalg.api.buffer.DataType Then
				Return "DataType." & v
			ElseIf TypeOf v Is LossReduce OrElse TypeOf v Is org.nd4j.autodiff.loss.LossReduce Then
				Return "LossReduce." & v
			Else
				Return v.ToString()
			End If
		End Function

		Private Shared Function toAnchor(ByVal name As String) As String
			Dim codepoints() As Integer = name.ToLower().codePoints().toArray()
			Dim type As Integer = Character.getType(codepoints(0))
			Dim anchor As New StringBuilder(New String(Character.toChars(codepoints(0))))
			For i As Integer = 1 To codepoints.Length - 1
				Dim curType As Integer = Character.getType(codepoints(i))
				If curType <> type Then
					anchor.Append("-")
				End If
				type = curType
				anchor.Append(New String(Character.toChars(codepoints(i))))
			Next i
			Return anchor.ToString()
		End Function
	End Class

End Namespace