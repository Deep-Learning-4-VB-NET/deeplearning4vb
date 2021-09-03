Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports com.squareup.javapoet
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports NotNull = org.jetbrains.annotations.NotNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SDOps = org.nd4j.autodiff.samediff.ops.SDOps
Imports SDValidation = org.nd4j.autodiff.samediff.ops.SDValidation
Imports org.nd4j.codegen.api
Imports DocSection = org.nd4j.codegen.api.doc.DocSection
Imports DocTokens = org.nd4j.codegen.api.doc.DocTokens
Imports ConstraintCodeGenerator = org.nd4j.codegen.api.generator.ConstraintCodeGenerator
Imports GeneratorConfig = org.nd4j.codegen.api.generator.GeneratorConfig
Imports GenUtil = org.nd4j.codegen.util.GenUtil
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NDValidation = org.nd4j.linalg.factory.NDValidation
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Condition = org.nd4j.linalg.indexing.conditions.Condition

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



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Nd4jNamespaceGenerator
	Public Class Nd4jNamespaceGenerator
		Private Shared typeMapping As IDictionary(Of DataType, Type) = New Dictionary(Of DataType, Type)()
		Private Shared validationMapping As IDictionary(Of DataType, String) = New Dictionary(Of DataType, String)()
		Private Shared enumMapping As IDictionary(Of Arg, TypeName) = New Dictionary(Of Arg, TypeName)()
		Private Shared configMapping As IDictionary(Of Config, TypeName) = New Dictionary(Of Config, TypeName)()
		Public Shared exactlyOne As Count = New Exactly(1)
		Private Shared copyright As String = "/*******************************************************************************" & vbLf & " * Copyright (c) 2019-2020 Konduit K.K." & vbLf & " *" & vbLf & " * This program and the accompanying materials are made available under the" & vbLf & " * terms of the Apache License, Version 2.0 which is available at" & vbLf & " * https://www.apache.org/licenses/LICENSE-2.0." & vbLf & " *" & vbLf & " * Unless required by applicable law or agreed to in writing, software" & vbLf & " * distributed under the License is distributed on an ""AS IS"" BASIS, WITHOUT" & vbLf & " * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the" & vbLf & " * License for the specific language governing permissions and limitations" & vbLf & " * under the License." & vbLf & " *" & vbLf & " * SPDX-License-Identifier: Apache-2.0" & vbLf & " ******************************************************************************/" & vbLf
		Private Shared codeGenWarning As String = vbLf & "//================== GENERATED CODE - DO NOT MODIFY THIS FILE ==================" & vbLf & vbLf

		Shared Sub New()
			typeMapping(DataType.BOOL) = GetType(Boolean)
			typeMapping(DataType.FLOATING_POINT) = GetType(Double)
			typeMapping(DataType.NUMERIC) = GetType(Double)
			typeMapping(DataType.INT) = GetType(Integer)
			typeMapping(DataType.LONG) = GetType(Long)
			typeMapping(DataType.DATA_TYPE) = GetType(org.nd4j.linalg.api.buffer.DataType)
			typeMapping(DataType.LOSS_REDUCE) = GetType(org.nd4j.autodiff.loss.LossReduce)
			typeMapping(DataType.CONDITION) = GetType(Condition)

			validationMapping(DataType.BOOL) = "validateBool"
			validationMapping(DataType.FLOATING_POINT) = "validateFloatingPoint"
			validationMapping(DataType.NUMERIC) = "validateNumerical"
			validationMapping(DataType.INT) = "validateInteger"
			validationMapping(DataType.LONG) = "validateInteger"
		End Sub

		Private Shared constraintCodeGenerator As ConstraintCodeGenerator = New JavaConstraintCodeGenerator()

		Private Sub New()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void generate(NamespaceOps namespace, org.nd4j.codegen.api.generator.GeneratorConfig config, java.io.File outputDirectory, String className, String basePackage, String docsDirectory) throws java.io.IOException
		Public Shared Sub generate(ByVal [namespace] As NamespaceOps, ByVal config As GeneratorConfig, ByVal outputDirectory As File, ByVal className As String, ByVal basePackage As String, ByVal docsDirectory As String)
			'String basePackage = "org.nd4j.linalg.factory";

			generateEnums(outputDirectory, basePackage)
			generateConfigs(outputDirectory, basePackage)
			Try
				generateOpFactory([namespace], outputDirectory, className, basePackage, StringUtils.EMPTY)
			Catch e As Exception
				log.error(e.ToString())
			End Try
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void generate(NamespaceOps namespace, org.nd4j.codegen.api.generator.GeneratorConfig config, java.io.File outputDirectory, String className, String basePackage, String parentClass, String docsDirectory) throws java.io.IOException
		Public Shared Sub generate(ByVal [namespace] As NamespaceOps, ByVal config As GeneratorConfig, ByVal outputDirectory As File, ByVal className As String, ByVal basePackage As String, ByVal parentClass As String, ByVal docsDirectory As String)
			'String basePackage = "org.nd4j.linalg.factory";

			generateEnums(outputDirectory, basePackage)
			generateConfigs(outputDirectory, basePackage)
			Try
				generateOpFactory([namespace], outputDirectory, className, basePackage, parentClass)
			Catch e As Exception
				log.error(e.ToString())
			End Try
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void generateOpFactory(NamespaceOps namespace, java.io.File outputDirectory, String className, String basePackage, String parentClass) throws IOException, ClassNotFoundException
		Private Shared Sub generateOpFactory(ByVal [namespace] As NamespaceOps, ByVal outputDirectory As File, ByVal className As String, ByVal basePackage As String, ByVal parentClass As String)
			Dim isBaseSameDiff As Boolean = StringUtils.equals("SDBaseOps", className)
			Dim isSameDiff As Boolean = StringUtils.isNotEmpty(parentClass)
			Dim isLoss As Boolean = StringUtils.equals("SDLoss", className)

			Dim builder As TypeSpec.Builder = If(Not isSameDiff OrElse isBaseSameDiff, TypeSpec.classBuilder(className).addModifiers(Modifier.PUBLIC), TypeSpec.classBuilder(className).superclass(Type.GetType(parentClass)).addModifiers(Modifier.PUBLIC))

			If isSameDiff AndAlso Not isBaseSameDiff Then
				addSameDiffConstructor(builder)
			ElseIf isBaseSameDiff Then
				builder.addField(TypeName.get(GetType(SameDiff)), "sd", Modifier.PROTECTED)
				addBaseSameDiffConstructor(builder)
			Else
				addDefaultConstructor(builder)
			End If

			'Add ops
			[namespace].getOps().Where(Function(it) Not it.isAbstract()).OrderBy(System.Collections.IComparer.comparing(AddressOf Op.getOpName)).forEachOrdered(Sub(o) generateMethods(builder, o, isSameDiff, isLoss))


			Dim ts As TypeSpec = builder.build()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String opsPackage = basePackage + ".ops";
			Dim opsPackage As String = basePackage & ".ops"
			Dim jf As JavaFile = If(StringUtils.isEmpty(parentClass), JavaFile.builder(opsPackage, ts).addStaticImport(GetType(NDValidation), "isSameType").build(), JavaFile.builder(opsPackage, ts).addStaticImport(GetType(SDValidation), "isSameType").build())

			Dim sb As New StringBuilder()
			sb.Append(copyright)
			sb.Append(codeGenWarning)
			jf.writeTo(sb)

			Dim outFile As New File(outputDirectory, packageToDirectory(opsPackage) & "/" & className & ".java")
			FileUtils.writeStringToFile(outFile, sb.ToString(), StandardCharsets.UTF_8)
		End Sub

		Private Shared Function packageToDirectory(ByVal packageName As String) As String
			Return packageName.Replace(".", File.separator)
		End Function

		Private Shared Sub addDefaultConstructor(ByVal builder As TypeSpec.Builder)
			'Add private no-arg constructor
			Dim noArg As MethodSpec = MethodSpec.constructorBuilder().addModifiers(Modifier.PUBLIC).build()

			builder.addMethod(noArg)
		End Sub

		Private Shared Sub addBaseSameDiffConstructor(ByVal builder As TypeSpec.Builder)

			Dim ctor As MethodSpec = MethodSpec.constructorBuilder().addModifiers(Modifier.PUBLIC).addParameter(GetType(SameDiff), "sameDiff").addStatement("this.sd = sameDiff").build()

			builder.addMethod(ctor)
		End Sub

		Private Shared Sub addSameDiffConstructor(ByVal builder As TypeSpec.Builder)
			Dim ctor As MethodSpec = MethodSpec.constructorBuilder().addModifiers(Modifier.PUBLIC).addParameter(GetType(SameDiff), "sameDiff").addStatement("super(sameDiff)").build()

			builder.addMethod(ctor)
		End Sub

		Private Shared Sub generateMethods(ByVal builder As TypeSpec.Builder, ByVal op As Op, ByVal isSameDiff As Boolean, ByVal isLoss As Boolean)
			Dim l As IList(Of Signature) = op.getSignatures()
			For Each s As Signature In l
				builder.addMethod(signatureCreatorMethod(op, s, isSameDiff, False, isLoss))
				If isSameDiff Then
					builder.addMethod(signatureCreatorMethod(op, s, True, True, isLoss))
				End If
			Next s
		End Sub

		Private Shared Function signatureCreatorMethod(ByVal op As Op, ByVal s As Signature, ByVal isSameDiff As Boolean, ByVal withName As Boolean, ByVal isLoss As Boolean) As MethodSpec
			Dim c As MethodSpec.Builder = MethodSpec.methodBuilder(GenUtil.ensureFirstIsNotCap(op.getOpName())).addModifiers(Modifier.PUBLIC)
			enableVarargsOnLastArg(c, op, s)

			buildJavaDoc(op, s, c, withName)
			Dim inNames As IList(Of String) = buildParameters(c, op, s, isSameDiff, withName)
			buildConstraints(c, op.getConstraints())
			buildExecution(c, op, inNames, isSameDiff, withName, isLoss)

			Return c.build()
		End Function

		Private Shared Sub buildJavaDoc(ByVal op As Op, ByVal s As Signature, ByVal c As MethodSpec.Builder, ByVal withName As Boolean)
			'Method javadoc:
			Dim doc As IList(Of DocSection) = op.getDoc()
			If doc.Count > 0 Then
				For Each ds As DocSection In doc
					If ds.applies(Language.JAVA, CodeComponent.OP_CREATOR) Then
						Dim text As String = DocTokens.processDocText(ds.getText(), op, DocTokens.GenerationType.ND4J)
						'Add <br> tags at the end of each line, where none already exists
						Dim lines() As String = text.Split(vbLf, True)
						For i As Integer = 0 To lines.Length - 1
							If Not lines(i).EndsWith("<br>", StringComparison.Ordinal) Then
								lines(i) = lines(i) & "<br>"
							End If
						Next i
						text = String.join(vbLf, lines)
						c.addJavadoc(text & vbLf & vbLf)
					End If
				Next ds
			End If


			' Document Constraints:
			'TODO what if constraint is on default value arg/s - no point specifying them here...
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final List<Constraint> constraints = op.getConstraints();
			Dim constraints As IList(Of Constraint) = op.getConstraints()
			If constraints.Count > 0 Then
				c.addJavadoc("Inputs must satisfy the following constraints: <br>" & vbLf)
				For Each constraint As Constraint In constraints
					c.addJavadoc(constraint.getMessage() & ": " & constraintCodeGenerator.generateExpression(constraint.getCheck()) & "<br>" & vbLf)
				Next constraint

				c.addJavadoc(vbLf)
			End If
			If withName Then
				If op.getOutputs().size() = 1 AndAlso Not op.getOutputs().get(0).getMultiOutput() Then
					c.addJavadoc("@param name name May be null. Name for the output variable" & vbLf)
				Else
					c.addJavadoc("@param names names May be null. Arrays of names for the output variables." & vbLf)
				End If
			End If
			Dim params As IList(Of Parameter) = s.getParameters()
			If params.Count > 0 Then
				For Each p As Parameter In params
					If TypeOf p Is Input Then
						Dim i As Input = CType(p, Input)
						c.addJavadoc("@param " & i.getName() & " " & (If(i.getDescription() Is Nothing, "", DocTokens.processDocText(i.getDescription(), op, DocTokens.GenerationType.ND4J))) & " (" & i.getType() & " type)" & vbLf)
					ElseIf TypeOf p Is Arg Then
						Dim arg As Arg = CType(p, Arg)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Count count = arg.getCount();
						Dim count As Count = arg.getCount()
						If count Is Nothing OrElse count.Equals(exactlyOne) Then
							c.addJavadoc("@param " & arg.getName() & " " & (If(arg.getDescription() Is Nothing, "", DocTokens.processDocText(arg.getDescription(), op, DocTokens.GenerationType.ND4J))) & vbLf)
						Else
							c.addJavadoc("@param " & arg.getName() & " " & (If(arg.getDescription() Is Nothing, "", DocTokens.processDocText(arg.getDescription(), op, DocTokens.GenerationType.ND4J))) & " (Size: " & count.ToString() & ")" & vbLf)
						End If
					ElseIf TypeOf p Is Config Then
						Dim config As Config = CType(p, Config)
						c.addJavadoc("@param " & config.getName() & " Configuration Object" & vbLf)
					Else
						Throw New Exception("Unknown parameter type: " & p & " - " & p.GetType() & " - op = " & op.getOpName())
					End If
				Next p


			End If

			'Outputs:
			Dim outputs As IList(Of Output) = op.getOutputs()
			If outputs.Count > 0 Then
				If outputs.Count = 1 AndAlso Not outputs(0).getMultiOutput() Then
					Dim o As Output = outputs(0)
					c.addJavadoc("@return " & o.getName() & " " & (If(o.getDescription() Is Nothing, "", DocTokens.processDocText(o.getDescription(), op, DocTokens.GenerationType.ND4J))) & " (" & o.getType() & " type)" & vbLf)
				Else
					'throw new UnsupportedOperationException("Javadoc for multi-output ops not yet implemented");
					log.error("Javadoc for multi-output ops not yet implemented")
				End If
			End If
		End Sub

		Private Shared Function buildParameters(ByVal c As MethodSpec.Builder, ByVal op As Op, ByVal s As Signature, ByVal isSameDiff As Boolean, ByVal withName As Boolean) As IList(Of String)
			Dim inNames As IList(Of String) = New List(Of String)()

			Dim params As IList(Of Parameter) = s.getParameters()

			If op.getArgsFirst() Then
				'Assuming sort is stable (doesn't change order of equal elements)
				params.Sort(Function(p1,p2) Boolean.compare(TypeOf p1 Is Input, TypeOf p2 Is Input))
			End If

			If withName Then
				If op.getOutputs().size() = 1 AndAlso Not op.getOutputs().get(0).getMultiOutput() Then
					c.addParameter(GetType(String), "name")
				Else
					c.addParameter(GetType(String()), "names")
				End If
			End If
			If params.Count > 0 Then
				Dim pCount As Integer = 0
				For Each p As Parameter In params
					pCount += 1
					Dim isLast As Boolean = pCount = params.Count
					If TypeOf p Is Input Then
						Dim i As Input = CType(p, Input)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String inputName = i.getName();
						Dim inputName As String = i.getName()
						inNames.Add(inputName)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Count count = i.getCount();
						Dim count As Count = i.getCount()
						If count Is Nothing OrElse count.Equals(exactlyOne) Then
							'Single input
							If isSameDiff Then
								c.addParameter(GetType(SDVariable), inputName)
							Else
								c.addParameter(GetType(INDArray), inputName)
							End If
						Else
							'Array input
							If isSameDiff Then
								c.addParameter(GetType(SDVariable()), inputName).varargs(isLast)
							Else
								c.addParameter(GetType(INDArray()), inputName).varargs(isLast)
							End If
						End If
						' Check for parameter types
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final DataType paramType = i.getType();
						Dim paramType As DataType = i.getType()
						Dim validationName As String = validationMapping(paramType)
						If validationName IsNot Nothing Then
							c.addStatement(CodeBlock.of("$T.$L($S, $S, $L)",If(isSameDiff, GetType(SDValidation), GetType(NDValidation)), validationName, op.getOpName(), inputName, inputName))
						End If
						checkParameterCount(c, count, inputName)
					ElseIf TypeOf p Is Arg Then
						Dim arg As Arg = CType(p, Arg)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String argName = arg.getName();
						Dim argName As String = arg.getName()
						If argName.Length = 0 Then
							Throw New System.InvalidOperationException("Got null argument name for op " & op.getOpName())
						End If
						inNames.Add(argName)


'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Count count = arg.getCount();
						Dim count As Count = arg.getCount()
						Dim type As TypeName = getArgType(arg)
						If type Is Nothing Then
							Throw New System.InvalidOperationException("No type mapping has been specified for type " & arg.getType() & " (op=" & op.getOpName() & ", arg=" & arg.getName() & ")")
						End If
						c.addParameter(type, argName)

						checkParameterCount(c, count, argName)
					ElseIf TypeOf p Is Config Then
						Dim config As Config = CType(p, Config)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String configName = config.getName();
						Dim configName As String = config.getName()
						inNames.Add(configName)
						c.addParameter(configMapping(config), config.name())
					Else
						Throw New System.InvalidOperationException("Unknown parameter type: " & p & " - " & p.GetType())
					End If

				Next p
			End If

			Return inNames
		End Function

		Public Shared Function getArgType(ByVal arg As Arg) As TypeName
			Dim argType As DataType = arg.getType()
			Dim count As Count = arg.getCount()
			Dim type As TypeName
			If argType = DataType.ENUM Then
				type = enumMapping(arg)
				If type Is Nothing Then
					Throw New System.InvalidOperationException(arg & " is using an unregistered ENUM. This is probably a bug.")
				End If
			Else
				If Not typeMapping.ContainsKey(argType) Then
					Return Nothing
				End If
				type = TypeName.get(typeMapping(argType))
			End If

			If Not (count Is Nothing OrElse count.Equals(exactlyOne)) Then
				' array Arg
				type = ArrayTypeName.of(type)
			End If
			Return type
		End Function

		Private Shared Sub buildConstraints(ByVal c As MethodSpec.Builder, ByVal constraints As IList(Of Constraint))
			If constraints.Count = 0 Then
				Return
			End If

			'TODO not all contsraints apply to all signatures?

			' Don't materialize the Backend Constraints
			For Each constraint As Constraint In constraints.Where(Function(it) Not (TypeOf it Is BackendConstraint)).ToList()
				c.addStatement(CodeBlock.of("$T.checkArgument($L, $S)", GetType(Preconditions), constraintCodeGenerator.generateExpression(constraint.getCheck()), constraint.getMessage()))
			Next constraint
		End Sub

		Private Shared Sub buildExecution(ByVal c As MethodSpec.Builder, ByVal op As Op, ByVal inNames As IList(Of String), ByVal isSameDiff As Boolean, ByVal withName As Boolean, ByVal isLoss As Boolean)
			Dim singleOut As Boolean = op.getOutputs().size() = 1 AndAlso Not op.getOutputs().get(0).getMultiOutput()
			If singleOut Then
				If isSameDiff Then
					c.returns(GetType(SDVariable))
				Else
					c.returns(GetType(INDArray))
				End If
			Else
				If isSameDiff Then
					c.returns(GetType(SDVariable()))
				Else
					c.returns(GetType(INDArray()))
				End If
			End If

			' We have to pass all parameters, always. But not all signatures will be taking all parameters.
			' inNames tells us which parameters this signatures has. For all others we want to pass default values
			Dim parameters As IList(Of String) = op.allParameters().OrderBy(Function(p1,p2)
			If p1.isVararg() Then
				Return 1
			ElseIf p2.isVararg() Then
				Return -1
			End If
			Return 0
			End Function).Select(Function(it)
			If inNames.Contains(it.name()) Then
				Return it.name()
			Else
				If Not it.hasDefaultValue() Then
					Throw New System.InvalidOperationException("The parameter " & it.name() & " has no default value, but is also not part of " & inNames.ToString())
				End If
				Return anyToCode(it, it.defaultValue())
			End If
			End Function).ToList()

			'Op execution:
			Dim sb As New StringBuilder()
			If isSameDiff Then
				If withName Then
					If singleOut Then
						sb.Append("SDVariable out = ")
					Else
						sb.Append("SDVariable[] out = ")
					End If

					sb.Append(" new ").Append(op.getJavaPackage()).Append(".").Append(If(op.getJavaOpClass() Is Nothing, GenUtil.ensureFirstIsCap(op.getOpName()), op.getJavaOpClass())).Append("(sd,").Append(String.join(", ", parameters)).Append(")")

					If singleOut Then
						sb.Append(".outputVariable()")
					Else
						sb.Append(".outputVariables()")
					End If

					c.addStatement(sb.ToString())
					If isLoss Then
						c.addStatement("out.markAsLoss()")
					End If

					If singleOut Then
						c.addStatement("return sd.updateVariableNameAndReference(out, name)")
					Else
						c.addStatement("return sd.updateVariableNamesAndReferences(out, names)")
					End If
				Else
					If isLoss Then
						sb.Append("SDVariable out = new ").Append(op.getJavaPackage()).Append(".").Append(If(op.getJavaOpClass() Is Nothing, GenUtil.ensureFirstIsCap(op.getOpName()), op.getJavaOpClass())).Append("(sd,").Append(String.join(", ", parameters)).Append(")")
					Else
						sb.Append("return new ").Append(op.getJavaPackage()).Append(".").Append(If(op.getJavaOpClass() Is Nothing, GenUtil.ensureFirstIsCap(op.getOpName()), op.getJavaOpClass())).Append("(sd,").Append(String.join(", ", parameters)).Append(")")
					End If
						'if (!op.getLegacy()) {
						If singleOut Then
							sb.Append(".outputVariable()")
						Else
							sb.Append(".outputVariables()")
						End If
						'}
					c.addStatement(sb.ToString())
					If isLoss Then
						c.addStatement("out.markAsLoss()")
						c.addStatement("return out")
					End If
				End If
			 Else
				sb.Append("return $T.exec(new ").Append(op.getJavaPackage()).Append(".").Append(If(op.getJavaOpClass() Is Nothing, GenUtil.ensureFirstIsCap(op.getOpName()), op.getJavaOpClass())).Append("(").Append(String.join(", ", parameters)).Append("))")
				If Not op.getLegacy() AndAlso singleOut Then 'Note: legacy ops Nd4j.exec(Op) returns INDArray; Nd4j.exec(CustomOp) returns INDArray[]
					sb.Append("[0]")
				End If

				c.addStatement(sb.ToString(), GetType(Nd4j))
			 End If
		End Sub

		Private Shared Sub enableVarargsOnLastArg(ByVal c As MethodSpec.Builder, ByVal op As Op, ByVal s As Signature)
			Dim p As IList(Of Parameter) = s.getParameters()
			If p.Count > 0 Then
				Dim lastP As Parameter = p(p.Count - 1)
				If TypeOf lastP Is Arg Then
					Dim arg As Arg = CType(lastP, Arg)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Count count = arg.getCount();
					Dim count As Count = arg.getCount()
					If count IsNot Nothing AndAlso Not count.Equals(exactlyOne) Then
						c.varargs(True)
					End If
				End If
			End If
		End Sub

		Private Shared Function countToJava(ByVal count As Count, ByVal paramName As String) As String
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String paramLength = paramName + ".length";
			Dim paramLength As String = paramName & ".length"
			If TypeOf count Is Exactly Then
				Return paramLength & " == " & CType(count, Exactly).getCount()
			ElseIf TypeOf count Is AtLeast Then
				Return paramLength & " >= " & CType(count, AtLeast).getMin()
			ElseIf TypeOf count Is AtMost Then
				Return paramLength & " <= " & CType(count, AtMost).getMax()
			ElseIf TypeOf count Is Range Then
				Return CType(count, Range).getFrom() & " <= " & paramLength & " && " & paramLength & " <= " & CType(count, Range).getTo()
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.ArgumentException("Can not deal with Count of type " & count.GetType().FullName)
			End If
		End Function

		Private Shared Sub checkParameterCount(ByVal c As MethodSpec.Builder, ByVal count As Count, ByVal paramName As String)
			' Check for parameter counts
			If count IsNot Nothing AndAlso Not count.Equals(exactlyOne) Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String errorMessage = paramName + " has incorrect size/length. Expected: " + countToJava(count, paramName) + ", got %s";
				Dim errorMessage As String = paramName & " has incorrect size/length. Expected: " & countToJava(count, paramName) & ", got %s"
				If TypeOf count Is Exactly Then
					c.addStatement(CodeBlock.of("$T.checkArgument($L.length == $L, $S, $L)", GetType(Preconditions), paramName, CType(count, Exactly).getCount(), errorMessage, paramName & ".length"))
				ElseIf TypeOf count Is AtLeast Then
					c.addStatement(CodeBlock.of("$T.checkArgument($L.length >= $L, $S, $L)", GetType(Preconditions), paramName, CType(count, AtLeast).getMin(), errorMessage, paramName & ".length"))
				ElseIf TypeOf count Is AtMost Then
					c.addStatement(CodeBlock.of("$T.checkArgument($L.length <= $L, $S, $L)", GetType(Preconditions), paramName, CType(count, AtMost).getMax(), errorMessage, paramName & ".length"))
				ElseIf TypeOf count Is Range Then
					c.addStatement(CodeBlock.of("$T.checkArgument($L.length >= $L && $L.length <= $L, $S, $L)", GetType(Preconditions), paramName, CType(count, Range).getFrom(), paramName, CType(count, Range).getTo(), errorMessage, paramName & ".length"))
				End If
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void generateEnums(java.io.File outputDirectory, String basePackage) throws java.io.IOException
		Private Shared Sub generateEnums(ByVal outputDirectory As File, ByVal basePackage As String)
			For Each it As Arg In Registry.INSTANCE.enums()
				generateEnum(outputDirectory, "org.nd4j.enums", it)
			Next it
		End Sub

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
			sb.Append(retType & " " & op.getOpName() & "(")
			Dim first As Boolean = True
			For Each param As Parameter In params
				If TypeOf param Is Arg Then
					Dim arg As Arg = CType(param, Arg)
					If Not first Then
						sb.Append(",")
					ElseIf withName Then
						sb.Append("String name,")
					End If
					Dim tu As TypeName = getArgType(arg)
					sb.Append(tu.ToString() & " " & arg.name())
					first = False
				ElseIf TypeOf param Is Input Then
					Dim arg As Input = CType(param, Input)
					If Not first Then
						sb.Append(",")
					ElseIf withName Then
						sb.Append("String name,")
					End If
					sb.Append((If(isSameDiff, "SDVariable ", "INDArray ")) + arg.name())
					first = False
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
						lines(i) = lines(i) & Environment.NewLine
					End If
				Next i
				text = String.join(vbLf, lines)
				sb.Append(text & Environment.NewLine)
				'}
			Next ds
			Return sb
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void generateDocs(NamespaceOps namespace, java.io.File outputDirectory, String basePackage) throws java.io.IOException
		Private Shared Sub generateDocs(ByVal [namespace] As NamespaceOps, ByVal outputDirectory As File, ByVal basePackage As String)
			Dim sb As New StringBuilder()
			sb.Append("#  Namespace " & [namespace].getName() & Environment.NewLine)
			Dim ops As IList(Of Op) = [namespace].getOps()
			For Each op As Op In ops
				sb.Append("## <a name=" & """").Append(op.name()).Append(""">").Append(op.name()).Append("</a>").Append(Environment.NewLine)
				Dim doc As IList(Of DocSection) = op.getDoc()
				If doc.Count > 0 Then
					Dim first As Boolean = True
					For Each s As Signature In op.getSignatures()
						If first Then
							sb.Append("````" & doc(0).getLanguage() & Environment.NewLine)
							first = False
						End If
						Dim ndCode As String = generateMethodText(op, s, False, False, False)
						sb.Append(ndCode).Append(Environment.NewLine)
						Dim sdCode As String = generateMethodText(op, s, True, False, False)
						sb.Append(sdCode).Append(Environment.NewLine)
						Dim withNameCode As String = generateMethodText(op, s, True, False, True)
						sb.Append(withNameCode).Append(Environment.NewLine)
					Next s
					sb.Append("````").Append(Environment.NewLine)
					Dim tsb As StringBuilder = buildDocSectionText(doc)
					sb.Append(tsb.ToString())
					Dim l As IList(Of Signature) = op.getSignatures()
					For Each s As Signature In l
						Dim params As IList(Of Parameter) = s.getParameters()
						For Each p As Parameter In params
							If TypeOf p Is Input Then
								Dim i As Input = CType(p, Input)
								sb.Append("* " & i.getName() & " " & (If(i.getDescription() Is Nothing, "", DocTokens.processDocText(i.getDescription(), op, DocTokens.GenerationType.ND4J))) & " (" & i.getType() & " type)" & Environment.NewLine)
							ElseIf TypeOf p Is Arg Then
								Dim arg As Arg = CType(p, Arg)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Count count = arg.getCount();
								Dim count As Count = arg.getCount()
								If count Is Nothing OrElse count.Equals(exactlyOne) Then
									sb.Append("* " & arg.getName() & " " & (If(arg.getDescription() Is Nothing, "", DocTokens.processDocText(arg.getDescription(), op, DocTokens.GenerationType.ND4J))) & Environment.NewLine)
								Else
									sb.Append("* " & arg.getName() & " " & (If(arg.getDescription() Is Nothing, "", DocTokens.processDocText(arg.getDescription(), op, DocTokens.GenerationType.ND4J))) & " (Size: " & count.ToString() & Environment.NewLine)
								End If
							End If
						Next p
					Next s
					sb.Append(Environment.NewLine)
					tsb = buildDocSectionText(doc)
					sb.Append(tsb.ToString())
				End If
			Next op

			For Each config As Config In Registry.INSTANCE.configs()
				sb.Append("## " & config.getName() & Environment.NewLine)
				Dim first As Boolean = True
				For Each i As Input In config.getInputs()
					If first Then
						sb.Append("````" & Environment.NewLine)
						first = False
					End If
					sb.Append("* " & i.getName() & " " & i.getDescription() & " (" & i.getType() & " type)" & Environment.NewLine)
				Next i
				For Each arg As Arg In config.getArgs()
					If first Then
						sb.Append("````" & Environment.NewLine)
						first = False
					End If
					sb.Append("* " & arg.getName() & " " & " (" & arg.getType() & " type)" & Environment.NewLine)
				Next arg
				Dim tsb As StringBuilder = buildDocSectionText(config.getDoc())
				sb.Append(tsb.ToString())
				sb.Append("````" & Environment.NewLine)
				ops.Where(Function(op) op.getConfigs().contains(config)).ForEach(Function(op) sb.Append("[" & op.getOpName() & "]" & "(#" & op.getOpName() & ")" & Environment.NewLine))
			Next config
			Dim outFile As New File(outputDirectory & "/ops", "/namespace-" & [namespace].getName() & ".md")
			FileUtils.writeStringToFile(outFile, sb.ToString(), StandardCharsets.UTF_8)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void generateEnum(java.io.File outputDirectory, String targetPackage, Arg arg) throws java.io.IOException
		Private Shared Sub generateEnum(ByVal outputDirectory As File, ByVal targetPackage As String, ByVal arg As Arg)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String className = org.nd4j.codegen.util.GenUtil.ensureFirstIsCap(arg.name());
			Dim className As String = GenUtil.ensureFirstIsCap(arg.name())
			enumMapping(arg) = ClassName.get(targetPackage, className)

			Dim builder As TypeSpec.Builder = TypeSpec.enumBuilder(className).addModifiers(Modifier.PUBLIC).addJavadoc(CodeBlock.of(arg.getDescription()))

			For Each possibleValue As String In arg.getPossibleValues()
				builder.addEnumConstant(possibleValue)
			Next possibleValue

			Dim ts As TypeSpec = builder.build()

			Dim jf As JavaFile = JavaFile.builder(targetPackage, ts).build()


			Dim sb As New StringBuilder()
			sb.Append(copyright)
			sb.Append(codeGenWarning)
			jf.writeTo(sb)

			Dim outFile As New File(outputDirectory, packageToDirectory(targetPackage) & "/" & className & ".java")
			FileUtils.writeStringToFile(outFile, sb.ToString(), StandardCharsets.UTF_8)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void generateConfigs(java.io.File outputDirectory, String basePackage) throws java.io.IOException
		Private Shared Sub generateConfigs(ByVal outputDirectory As File, ByVal basePackage As String)
			For Each config As Config In Registry.INSTANCE.configs()
				generateConfig(outputDirectory, basePackage & ".configs", config)
			Next config
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void generateConfig(java.io.File outputDirectory, String targetPackage, Config config) throws java.io.IOException
		Private Shared Sub generateConfig(ByVal outputDirectory As File, ByVal targetPackage As String, ByVal config As Config)
			If config.getJavaClassOverride() IsNot Nothing AndAlso Not config.getJavaClassOverride().isEmpty() Then
				'Java class override means "don't generate, use the existing one instead"
				Dim c As String = config.getJavaClassOverride()
				Dim idx As Integer = c.LastIndexOf("."c)
				Dim pkg As String = c.Substring(0, idx)
				Dim className As String = c.Substring(idx+1)
				configMapping(config) = ClassName.get(pkg, className)
				Return
			End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String className = org.nd4j.codegen.util.GenUtil.ensureFirstIsCap(config.name());
			Dim className As String = GenUtil.ensureFirstIsCap(config.name())
			configMapping(config) = ClassName.get(targetPackage, className)

			' Build Config Builder Class
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final TypeSpec.Builder sdb = TypeSpec.classBuilder("SdBuilder").addModifiers(javax.lang.model.element.Modifier.@STATIC, javax.lang.model.element.Modifier.@PUBLIC);
			Dim sdb As TypeSpec.Builder = TypeSpec.classBuilder("SdBuilder").addModifiers(Modifier.STATIC, Modifier.PUBLIC)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final TypeSpec.Builder ndb = TypeSpec.classBuilder("NdBuilder").addModifiers(javax.lang.model.element.Modifier.@STATIC, javax.lang.model.element.Modifier.@PUBLIC);
			Dim ndb As TypeSpec.Builder = TypeSpec.classBuilder("NdBuilder").addModifiers(Modifier.STATIC, Modifier.PUBLIC)

			For Each input As Input In config.getInputs()
				addConfigBuilderParam(className, sdb, input.getName(), input.getType(), [getType](TypeName.get(GetType(SDVariable)), input.getCount()), input.getDescription(), input.getCount())
				addConfigBuilderParam(className, ndb, input.getName(), input.getType(), [getType](TypeName.get(GetType(INDArray)), input.getCount()), input.getDescription(), input.getCount())
			Next input

			For Each arg As Arg In config.getArgs()
				addConfigBuilderParam(className, sdb, arg.getName(), Nothing, getArgType(arg), arg.getDescription(), arg.getCount())
				addConfigBuilderParam(className, ndb, arg.getName(), Nothing, getArgType(arg), arg.getDescription(), arg.getCount())
			Next arg

			Dim parts As New List(Of String)()
			Dim parameters As New List(Of Object)()
			For Each input As Input In config.getInputs()
				parts.Add("$L")
				parameters.Add(If(input.hasDefaultValue(), input.name() & " == null ? " & CType(input.defaultValue(), Input).getName() & " : " & input.name(), input.name()))
			Next input
			For Each input As Arg In config.getArgs()
				parts.Add("$L")
				parameters.Add(If(input.hasDefaultValue(), input.name() & " == null ? " & anyToCode(input, input.defaultValue()) &" : " & input.name(), input.name()))
			Next input
			parameters.Insert(0, className)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final MethodSpec.Builder build = MethodSpec.methodBuilder("build").addModifiers(javax.lang.model.element.Modifier.@PUBLIC).returns(ClassName.bestGuess(className));
			Dim build As MethodSpec.Builder = MethodSpec.methodBuilder("build").addModifiers(Modifier.PUBLIC).returns(ClassName.bestGuess(className))
			buildConstraints(build, config.getConstraints())
			build.addStatement("return new $N(" & (String.join(", ", parts)) & ")", parameters.ToArray())

			sdb.addMethod(build.build())
			ndb.addMethod(build.build())


'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final TypeSpec ndBuilder = ndb.build();
			Dim ndBuilder As TypeSpec = ndb.build()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final TypeSpec sdBuilder = sdb.build();
			Dim sdBuilder As TypeSpec = sdb.build()


			' Build Config Holder Class
			Dim holder As TypeSpec.Builder = TypeSpec.classBuilder(className).addModifiers(Modifier.PUBLIC)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final MethodSpec.Builder ndConstructorBuilder = MethodSpec.constructorBuilder().addModifiers(javax.lang.model.element.Modifier.@PRIVATE);
			Dim ndConstructorBuilder As MethodSpec.Builder = MethodSpec.constructorBuilder().addModifiers(Modifier.PRIVATE)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final MethodSpec.Builder sdConstructorBuilder = MethodSpec.constructorBuilder().addModifiers(javax.lang.model.element.Modifier.@PRIVATE);
			Dim sdConstructorBuilder As MethodSpec.Builder = MethodSpec.constructorBuilder().addModifiers(Modifier.PRIVATE)


			For Each input As Input In config.getInputs()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String inputName = org.nd4j.codegen.util.GenUtil.ensureFirstIsCap(input.getName());
				Dim inputName As String = GenUtil.ensureFirstIsCap(input.getName())
				addConfigParam(holder, ndConstructorBuilder, "nd" & inputName, [getType](TypeName.get(GetType(INDArray)), input.getCount()), input.getDescription(), True)
				addConfigParam(holder, sdConstructorBuilder, "sd" & inputName, [getType](TypeName.get(GetType(SDVariable)), input.getCount()), input.getDescription(), True)
			Next input

			For Each arg As Arg In config.getArgs()
				addConfigParam(holder, ndConstructorBuilder, arg.getName(), getArgType(arg), arg.getDescription(), True)
				addConfigParam(holder, sdConstructorBuilder, arg.getName(), getArgType(arg), arg.getDescription(), False)
			Next arg
			holder.addMethod(sdConstructorBuilder.build())
			holder.addMethod(ndConstructorBuilder.build())

			holder.addMethod(MethodSpec.methodBuilder("sdBuilder").addModifiers(Modifier.STATIC, Modifier.PUBLIC).addStatement("return new $N()", sdBuilder.name).returns(ClassName.bestGuess(sdBuilder.name)).build())
			holder.addType(sdBuilder)
			holder.addMethod(MethodSpec.methodBuilder("ndBuilder").addModifiers(Modifier.STATIC, Modifier.PUBLIC).addStatement("return new $N()", ndBuilder.name).returns(ClassName.bestGuess(ndBuilder.name)).build())
			holder.addType(ndBuilder)

			' add javadoc
			'Method javadoc:
			Dim doc As IList(Of DocSection) = config.getDoc()
			If doc.Count > 0 Then
				For Each ds As DocSection In doc
					If ds.applies(Language.JAVA, CodeComponent.OP_CREATOR) Then
						Dim text As String = ds.getText()
						'Add <br> tags at the end of each line, where none already exists
						Dim lines() As String = text.Split(vbLf, True)
						For i As Integer = 0 To lines.Length - 1
							If Not lines(i).EndsWith("<br>", StringComparison.Ordinal) Then
								lines(i) = lines(i) & "<br>"
							End If
						Next i
						text = String.join(vbLf, lines)
						holder.addJavadoc(text & vbLf & vbLf)
					End If
				Next ds
			End If


			' Document Constraints:
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final List<Constraint> constraints = config.getConstraints();
			Dim constraints As IList(Of Constraint) = config.getConstraints()
			If constraints.Count > 0 Then
				holder.addJavadoc("Inputs must satisfy the following constraints: <br>" & vbLf)
				For Each constraint As Constraint In constraints
					holder.addJavadoc(constraint.getMessage() & ": " & constraintCodeGenerator.generateExpression(constraint.getCheck()) & "<br>" & vbLf)
				Next constraint

				holder.addJavadoc(vbLf)
			End If

			Dim ts As TypeSpec = holder.build()


			Dim jf As JavaFile = JavaFile.builder(targetPackage, ts).build()


			Dim sb As New StringBuilder()
			sb.Append(copyright)
			sb.Append(codeGenWarning)
			jf.writeTo(sb)

			Dim outFile As New File(outputDirectory, packageToDirectory(targetPackage) & "/" & className & ".java")
			FileUtils.writeStringToFile(outFile, sb.ToString(), StandardCharsets.UTF_8)
		End Sub

		Private Shared Sub addConfigParam(ByVal builder As TypeSpec.Builder, ByVal constructorBuilder As MethodSpec.Builder, ByVal paramName As String, ByVal paramType As TypeName, ByVal paramDescription As String, ByVal addField As Boolean)
			If addField Then
				' Add param fields
				builder.addField(paramType, paramName, Modifier.PRIVATE)

				' Add param getters
				builder.addMethod(generateGetter(paramType, paramName, paramDescription, False))
			End If

			' Add param constructor parameters
			constructorBuilder.addParameter(paramType, paramName, Modifier.FINAL)
			constructorBuilder.addStatement("this.$L = $L", paramName, paramName)
		End Sub

		Private Shared Sub addConfigBuilderParam(ByVal configClassName As String, ByVal builder As TypeSpec.Builder, ByVal paramName As String, ByVal inputType As DataType, ByVal paramType As TypeName, ByVal paramDescription As String, ByVal count As Count)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String builderName = builder.build().name;
			Dim builderName As String = builder.build().name
			' Add param fields
			builder.addField(paramType.box(), paramName, Modifier.PRIVATE)

			' Add param getters
			builder.addMethod(generateGetter(paramType, paramName, paramDescription, True))

			' Add param setter
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final MethodSpec.Builder setter = MethodSpec.methodBuilder(paramName).addParameter(paramType, paramName).addModifiers(javax.lang.model.element.Modifier.@PUBLIC);
			Dim setter As MethodSpec.Builder = MethodSpec.methodBuilder(paramName).addParameter(paramType, paramName).addModifiers(Modifier.PUBLIC)
			checkParameterCount(setter, count, paramName)
			If inputType IsNot Nothing Then
				If builderName.Equals("SdBuilder") Then
					setter.addStatement("$T.$L($S, $S, $L)", GetType(SDValidation), validationMapping(inputType), "Config: " & configClassName, paramName, paramName)
				ElseIf builderName.Equals("NdBuilder") Then
					setter.addStatement("$T.$L($S, $S, $L)", GetType(NDValidation), validationMapping(inputType), "Config: " & configClassName, paramName, paramName)
				Else
					Throw New System.ArgumentException("Unknown Builder Type " & builderName)
				End If
			End If
			setter.addStatement("this.$L = $L", paramName, paramName).addStatement("return this").returns(ClassName.bestGuess(builderName))

			If count IsNot Nothing AndAlso Not count.Equals(exactlyOne) Then
				setter.varargs(True)
			End If

			If paramDescription IsNot Nothing Then
				setter.addJavadoc(paramDescription)
			End If
			builder.addMethod(setter.build())
		End Sub

		Private Shared Function [getType](ByVal typeVariable As TypeName, ByVal count As Count) As TypeName
			If count IsNot Nothing AndAlso Not count.Equals(exactlyOne) Then
				Return ArrayTypeName.of(typeVariable)
			Else
				Return typeVariable
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NotNull private static MethodSpec generateGetter(TypeName typeVariable, String paramName, String paramDescription, boolean fluent)
		Private Shared Function generateGetter(ByVal typeVariable As TypeName, ByVal paramName As String, ByVal paramDescription As String, ByVal fluent As Boolean) As MethodSpec
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final MethodSpec.Builder getter = MethodSpec.methodBuilder((fluent ? paramName : "get" + org.nd4j.codegen.util.GenUtil.ensureFirstIsCap(paramName))).addModifiers(javax.lang.model.element.Modifier.@PUBLIC).returns(typeVariable);
			Dim getter As MethodSpec.Builder = MethodSpec.methodBuilder((If(fluent, paramName, "get" & GenUtil.ensureFirstIsCap(paramName)))).addModifiers(Modifier.PUBLIC).returns(typeVariable)
			If paramDescription IsNot Nothing Then
				getter.addJavadoc(paramDescription)
			End If
			getter.addStatement("return this.$L", paramName)
			Return getter.build()
		End Function

		Private Shared Function anyToCode(ByVal parameter As Parameter, ByVal v As Object) As String
			If v Is Nothing Then
				Return "null"
			ElseIf TypeOf v Is Integer() Then
				Return "new int[]" & java.util.Arrays.toString(DirectCast(v, Integer())).Replace("[", "{").Replace("]", "}")
			ElseIf TypeOf v Is Long() Then
				Return "new long[]" & java.util.Arrays.toString(DirectCast(v, Long())).Replace("[", "{").Replace("]", "}")
			ElseIf TypeOf v Is Single() Then
				Return "new float[]" & java.util.Arrays.toString(DirectCast(v, Single())).Replace("[", "{").Replace("]", "}")
			ElseIf TypeOf v Is Double() Then
				Return "new double[]" & java.util.Arrays.toString(DirectCast(v, Double())).Replace("[", "{").Replace("]", "}")
			ElseIf TypeOf v Is Boolean() Then
				Return "new boolean[]" & java.util.Arrays.toString(DirectCast(v, Boolean())).Replace("[", "{").Replace("]", "}")
			ElseIf TypeOf v Is Input Then
				Return DirectCast(v, Input).getName()
			ElseIf TypeOf v Is org.nd4j.linalg.api.buffer.DataType Then
				Return "DataType." & v
			ElseIf TypeOf v Is LossReduce OrElse TypeOf v Is org.nd4j.autodiff.loss.LossReduce Then
				Return "org.nd4j.autodiff.loss.LossReduce." & v
			ElseIf TypeOf parameter Is Arg AndAlso CType(parameter, Arg).getType() = DataType.ENUM Then
				Return GenUtil.ensureFirstIsCap(parameter.name()) & "." & v.ToString()
			Else
				Return v.ToString()
			End If
		End Function
	End Class

End Namespace