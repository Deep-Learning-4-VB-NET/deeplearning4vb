Imports System
Imports System.Collections.Generic
Imports System.Text
Imports ParserConfiguration = com.github.javaparser.ParserConfiguration
Imports StaticJavaParser = com.github.javaparser.StaticJavaParser
Imports CompilationUnit = com.github.javaparser.ast.CompilationUnit
Imports ConstructorDeclaration = com.github.javaparser.ast.body.ConstructorDeclaration
Imports FieldDeclaration = com.github.javaparser.ast.body.FieldDeclaration
Imports MethodCallExpr = com.github.javaparser.ast.expr.MethodCallExpr
Imports ResolvedConstructorDeclaration = com.github.javaparser.resolution.declarations.ResolvedConstructorDeclaration
Imports ResolvedFieldDeclaration = com.github.javaparser.resolution.declarations.ResolvedFieldDeclaration
Imports ResolvedParameterDeclaration = com.github.javaparser.resolution.declarations.ResolvedParameterDeclaration
Imports JavaSymbolSolver = com.github.javaparser.symbolsolver.JavaSymbolSolver
Imports CombinedTypeSolver = com.github.javaparser.symbolsolver.resolution.typesolvers.CombinedTypeSolver
Imports JavaParserTypeSolver = com.github.javaparser.symbolsolver.resolution.typesolvers.JavaParserTypeSolver
Imports ReflectionTypeSolver = com.github.javaparser.symbolsolver.resolution.typesolvers.ReflectionTypeSolver
Imports Log = com.github.javaparser.utils.Log
Imports SourceRoot = com.github.javaparser.utils.SourceRoot
Imports Builder = lombok.Builder
Imports Getter = lombok.Getter
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports ArgDescriptorProposal = org.nd4j.descriptor.proposal.ArgDescriptorProposal
Imports ArgDescriptorSource = org.nd4j.descriptor.proposal.ArgDescriptorSource
Imports OpType = org.nd4j.graph.OpType
Imports OpNamespace = org.nd4j.ir.OpNamespace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.linalg.api.ops
Imports BaseDynamicTransformOp = org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
Imports Reflections = org.reflections.Reflections
Imports org.nd4j.descriptor.proposal.impl.ArgDescriptorParserUtils

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

Namespace org.nd4j.descriptor.proposal.impl


	Public Class JavaSourceArgDescriptorSource
		Implements ArgDescriptorSource


		Private sourceRoot As SourceRoot
		Private nd4jOpsRootDir As File
		Private weight As Double

		''' <summary>
		'''     void addTArgument(double... arg);
		''' 
		'''     void addIArgument(int... arg);
		''' 
		'''     void addIArgument(long... arg);
		''' 
		'''     void addBArgument(boolean... arg);
		''' 
		'''     void addDArgument(DataType... arg);
		''' </summary>

		Public Const ADD_T_ARGUMENT_INVOCATION As String = "addTArgument"
		Public Const ADD_I_ARGUMENT_INVOCATION As String = "addIArgument"
		Public Const ADD_B_ARGUMENT_INVOCATION As String = "addBArgument"
		Public Const ADD_D_ARGUMENT_INVOCATION As String = "addDArgument"
		Public Const ADD_INPUT_ARGUMENT As String = "addInputArgument"
		Public Const ADD_OUTPUT_ARGUMENT As String = "addOutputArgument"
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private Map<String, org.nd4j.ir.OpNamespace.OpDescriptor.OpDeclarationType> opTypes;
		Private opTypes As IDictionary(Of String, OpNamespace.OpDescriptor.OpDeclarationType)
		Shared Sub New()
			Log.setAdapter(New Log.StandardOutStandardErrorAdapter())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public JavaSourceArgDescriptorSource(java.io.File nd4jApiRootDir,double weight)
		Public Sub New(ByVal nd4jApiRootDir As File, ByVal weight As Double)
			Me.sourceRoot = initSourceRoot(nd4jApiRootDir)
			Me.nd4jOpsRootDir = nd4jApiRootDir
			If opTypes Is Nothing Then
				opTypes = New Dictionary(Of String, OpNamespace.OpDescriptor.OpDeclarationType)()
			End If

			Me.weight = weight
		End Sub

		Public Overridable Function doReflectionsExtraction() As IDictionary(Of String, IList(Of ArgDescriptorProposal))
			Dim ret As IDictionary(Of String, IList(Of ArgDescriptorProposal)) = New Dictionary(Of String, IList(Of ArgDescriptorProposal))()

			Dim reflections As New Reflections("org.nd4j")
			Dim subTypesOf As ISet(Of Type) = reflections.getSubTypesOf(GetType(DifferentialFunction))
			Dim subTypesOfOp As ISet(Of Type) = reflections.getSubTypesOf(GetType(CustomOp))
			Dim allClasses As ISet(Of Type) = New HashSet(Of Type)()
			allClasses.addAll(subTypesOf)
			allClasses.addAll(subTypesOfOp)
			Dim opNamesForDifferentialFunction As ISet(Of String) = New HashSet(Of String)()


			For Each clazz As Type In allClasses
				If Modifier.isAbstract(clazz.getModifiers()) OrElse clazz.IsInterface Then
					Continue For
				End If

				processClazz(ret, opNamesForDifferentialFunction, clazz)

			Next clazz


			Return ret
		End Function

		Private Sub processClazz(ByVal ret As IDictionary(Of String, IList(Of ArgDescriptorProposal)), ByVal opNamesForDifferentialFunction As ISet(Of String), ByVal clazz As Type)
			Try
				Dim funcInstance As Object = System.Activator.CreateInstance(clazz)
				Dim name As String = Nothing

				If TypeOf funcInstance Is DifferentialFunction Then
					Dim differentialFunction As DifferentialFunction = DirectCast(funcInstance, DifferentialFunction)
					name = differentialFunction.opName()
				ElseIf TypeOf funcInstance Is CustomOp Then
					Dim customOp As CustomOp = DirectCast(funcInstance, CustomOp)
					name = customOp.opName()
				End If


				If name Is Nothing Then
					Return
				End If
				opNamesForDifferentialFunction.Add(name)
				If Not (TypeOf funcInstance Is DynamicCustomOp) Then
					opTypes(name) = OpNamespace.OpDescriptor.OpDeclarationType.LEGACY_XYZ
				Else
					opTypes(name) = OpNamespace.OpDescriptor.OpDeclarationType.CUSTOM_OP_IMPL
				End If


'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Dim fileName As String = clazz.FullName.Replace(".",File.separator)
				Dim fileBuilder As New StringBuilder()
				fileBuilder.Append(fileName)
				fileBuilder.Append(".java")
				Dim paramIndicesCount As New CounterMap(Of Pair(Of String, OpNamespace.ArgDescriptor.ArgType), Integer)()

				' Our sample is in the root of this directory, so no package name.
				Dim cu As CompilationUnit = sourceRoot.parse(clazz.Assembly.GetName().Name, clazz.Name & ".java")
				cu.findAll(GetType(MethodCallExpr)).forEach(Sub(method)
				Dim methodInvoked As String = method.getNameAsString()
				Dim indexed As New AtomicInteger(0)
				If methodInvoked.Equals(ADD_T_ARGUMENT_INVOCATION) Then
					method.getArguments().forEach(Sub(argument)
						If argument.isNameExpr() Then
							paramIndicesCount.incrementCount(Pair.of(argument.asNameExpr().getNameAsString(), OpNamespace.ArgDescriptor.ArgType.DOUBLE),indexed.get(),100.0)
						ElseIf argument.isMethodCallExpr() Then
							If argument.asMethodCallExpr().getName().ToString().Equals("ordinal") Then
								paramIndicesCount.incrementCount(Pair.of(argument.asNameExpr().getNameAsString(), OpNamespace.ArgDescriptor.ArgType.DOUBLE),indexed.get(),100.0)
							End If
						End If
						indexed.incrementAndGet()
					End Sub)
				ElseIf methodInvoked.Equals(ADD_B_ARGUMENT_INVOCATION) Then
					method.getArguments().forEach(Sub(argument)
						If argument.isNameExpr() Then
							paramIndicesCount.incrementCount(Pair.of(argument.asNameExpr().getNameAsString(), OpNamespace.ArgDescriptor.ArgType.BOOL),indexed.get(),100.0)
						ElseIf argument.isMethodCallExpr() Then
							If argument.asMethodCallExpr().getName().Equals("ordinal") Then
								paramIndicesCount.incrementCount(Pair.of(argument.asNameExpr().getNameAsString(), OpNamespace.ArgDescriptor.ArgType.BOOL),indexed.get(),100.0)
							End If
						End If
						indexed.incrementAndGet()
					End Sub)
				ElseIf methodInvoked.Equals(ADD_I_ARGUMENT_INVOCATION) Then
					method.getArguments().forEach(Sub(argument)
						If argument.isNameExpr() Then
							paramIndicesCount.incrementCount(Pair.of(argument.asNameExpr().getNameAsString(), OpNamespace.ArgDescriptor.ArgType.INT64),indexed.get(),100.0)
						ElseIf argument.isMethodCallExpr() Then
							If argument.asMethodCallExpr().getName().ToString().Equals("ordinal") Then
								paramIndicesCount.incrementCount(Pair.of(argument.ToString().Replace(".ordinal()",""), OpNamespace.ArgDescriptor.ArgType.INT64),indexed.get(),100.0)
							End If
						End If
						indexed.incrementAndGet()
					End Sub)
				ElseIf methodInvoked.Equals(ADD_D_ARGUMENT_INVOCATION) Then
					method.getArguments().forEach(Sub(argument)
						If argument.isNameExpr() Then
							paramIndicesCount.incrementCount(Pair.of(argument.asNameExpr().getNameAsString(), OpNamespace.ArgDescriptor.ArgType.DATA_TYPE),indexed.get(),100.0)
						ElseIf argument.isMethodCallExpr() Then
							If argument.asMethodCallExpr().getName().ToString().Equals("ordinal") Then
								paramIndicesCount.incrementCount(Pair.of(argument.ToString().Replace(".ordinal()",""), OpNamespace.ArgDescriptor.ArgType.DATA_TYPE),indexed.get(),100.0)
							End If
						End If
						indexed.incrementAndGet()
					End Sub)
				ElseIf methodInvoked.Equals(ADD_INPUT_ARGUMENT) Then
					method.getArguments().forEach(Sub(argument)
						If argument.isNameExpr() Then
							paramIndicesCount.incrementCount(Pair.of(argument.asNameExpr().getNameAsString(), OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR),indexed.get(),100.0)
						ElseIf argument.isMethodCallExpr() Then
							If argument.asMethodCallExpr().getName().ToString().Equals("ordinal") Then
								paramIndicesCount.incrementCount(Pair.of(argument.ToString().Replace(".ordinal()",""), OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR),indexed.get(),100.0)
							End If
						End If
						indexed.incrementAndGet()
					End Sub)
				ElseIf methodInvoked.Equals(ADD_OUTPUT_ARGUMENT) Then
					method.getArguments().forEach(Sub(argument)
						If argument.isNameExpr() Then
							paramIndicesCount.incrementCount(Pair.of(argument.asNameExpr().getNameAsString(), OpNamespace.ArgDescriptor.ArgType.OUTPUT_TENSOR),indexed.get(),100.0)
						ElseIf argument.isMethodCallExpr() Then
							If argument.asMethodCallExpr().getName().ToString().Equals("ordinal") Then
								paramIndicesCount.incrementCount(Pair.of(argument.ToString().Replace(".ordinal()",""), OpNamespace.ArgDescriptor.ArgType.OUTPUT_TENSOR),indexed.get(),100.0)
							End If
						End If
						indexed.incrementAndGet()
					End Sub)
				End If
				End Sub)




				Dim collect As IList(Of ResolvedConstructorDeclaration) = cu.findAll(GetType(ConstructorDeclaration)).Select(Function(input) input.resolve()).Where(Function(constructor) constructor.getNumberOfParams() > 0).Distinct().ToList()

				'only process final constructor with all arguments for indexing purposes
				Dim constructorArgCount As New Counter(Of ResolvedConstructorDeclaration)()
				collect.Where(Function(input) input IsNot Nothing).ForEach(Sub(constructor)
				constructorArgCount.incrementCount(constructor,constructor.getNumberOfParams())
				End Sub)

				If constructorArgCount.argMax() IsNot Nothing Then
					collect = New List(Of ResolvedConstructorDeclaration) From {constructorArgCount.argMax()}
				End If

				Dim argDescriptorProposals As IList(Of ArgDescriptorProposal) = ret(name)
				If argDescriptorProposals Is Nothing Then
					argDescriptorProposals = New List(Of ArgDescriptorProposal)()
					ret(name) = argDescriptorProposals
				End If

				Dim parameters As ISet(Of ResolvedParameterDeclaration) = New LinkedHashSet(Of ResolvedParameterDeclaration)()

				Dim floatIdx As Integer = 0
				Dim inputIdx As Integer = 0
				Dim outputIdx As Integer = 0
				Dim intIdx As Integer = 0
				Dim boolIdx As Integer = 0

				For Each parameterDeclaration As ResolvedConstructorDeclaration In collect
					floatIdx = 0
					inputIdx = 0
					outputIdx = 0
					intIdx = 0
					boolIdx = 0
					Dim i As Integer = 0
					Do While i < parameterDeclaration.getNumberOfParams()
						Dim param As ResolvedParameterDeclaration = parameterDeclaration.getParam(i)
						Dim argType As OpNamespace.ArgDescriptor.ArgType = argTypeForParam(param)
						If isValidParam(param) Then
							parameters.Add(param)
							Select Case argType.innerEnumValue
								Case OpNamespace.ArgDescriptor.ArgType.InnerEnum.INPUT_TENSOR
									paramIndicesCount.incrementCount(Pair.of(param.getName(),argType), inputIdx, 100.0)
									inputIdx += 1
								Case OpNamespace.ArgDescriptor.ArgType.InnerEnum.INT64, INT32
									paramIndicesCount.incrementCount(Pair.of(param.getName(), OpNamespace.ArgDescriptor.ArgType.INT64), intIdx, 100.0)
									intIdx += 1
								Case OpNamespace.ArgDescriptor.ArgType.InnerEnum.DOUBLE, FLOAT
									paramIndicesCount.incrementCount(Pair.of(param.getName(), OpNamespace.ArgDescriptor.ArgType.FLOAT), floatIdx, 100.0)
									paramIndicesCount.incrementCount(Pair.of(param.getName(), OpNamespace.ArgDescriptor.ArgType.DOUBLE), floatIdx, 100.0)
									floatIdx += 1
								Case OpNamespace.ArgDescriptor.ArgType.InnerEnum.BOOL
									paramIndicesCount.incrementCount(Pair.of(param.getName(),argType), boolIdx, 100.0)
									boolIdx += 1
								Case OpNamespace.ArgDescriptor.ArgType.InnerEnum.OUTPUT_TENSOR
									paramIndicesCount.incrementCount(Pair.of(param.getName(),argType), outputIdx, 100.0)
									outputIdx += 1
								Case OpNamespace.ArgDescriptor.ArgType.InnerEnum.UNRECOGNIZED
									i += 1
									Continue Do

							End Select

						End If
						i += 1
					Loop
				Next parameterDeclaration

				floatIdx = 0
				inputIdx = 0
				outputIdx = 0
				intIdx = 0
				boolIdx = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java stream collectors are not converted by Java to VB Converter:
				Dim typesAndParams As ISet(Of IList(Of Pair(Of String, String))) = parameters.Select(Function(collectedParam) Pair.of(collectedParam.describeType(), collectedParam.getName())).collect(Collectors.groupingBy(Function(input) input.getSecond())).entrySet().stream().Select(Function(inputPair) inputPair.getValue()).collect(Collectors.toSet())


				Dim constructorNamesEncountered As ISet(Of String) = New HashSet(Of String)()
				Dim finalArgDescriptorProposals As IList(Of ArgDescriptorProposal) = argDescriptorProposals
				typesAndParams.forEach(Sub(listOfTypesAndNames)
				listOfTypesAndNames.forEach(Sub(parameter)
					If typeNameOrArrayOfTypeNameMatches(parameter.getFirst(),GetType(SDVariable).FullName,GetType(INDArray).FullName) Then
						constructorNamesEncountered.Add(parameter.getValue())
						If outputNames.Contains(parameter.getValue()) Then
							Dim counter As Counter(Of Integer) = paramIndicesCount.getCounter(Pair.of(parameter.getSecond(), OpNamespace.ArgDescriptor.ArgType.OUTPUT_TENSOR))
							If counter IsNot Nothing Then
								finalArgDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(99.0 * (If(counter Is Nothing, 1, counter.size()))).sourceOfProposal("java").descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.OUTPUT_TENSOR).setName(parameter.getSecond()).setIsArray(parameter.getFirst().contains("[]") OrElse parameter.getFirst().contains("...")).setArgIndex(counter.argMax()).build()).build())
							End If
						Else
							Dim counter As Counter(Of Integer) = paramIndicesCount.getCounter(Pair.of(parameter.getSecond(), OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR))
							If counter IsNot Nothing Then
								finalArgDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(99.0 * (If(counter Is Nothing, 1, counter.size()))).sourceOfProposal("java").descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName(parameter.getSecond()).setIsArray(parameter.getFirst().contains("[]") OrElse parameter.getFirst().contains("...")).setArgIndex(counter.argMax()).build()).build())
							End If
						End If
					ElseIf typeNameOrArrayOfTypeNameMatches(parameter.getFirst(),GetType(Integer).FullName,GetType(Long).FullName,GetType(Integer).FullName,GetType(Long).FullName) OrElse paramIsEnum(parameter.getFirst()) Then
						constructorNamesEncountered.Add(parameter.getValue())
						Dim counter As Counter(Of Integer) = paramIndicesCount.getCounter(Pair.of(parameter.getSecond(), OpNamespace.ArgDescriptor.ArgType.INT64))
						If counter IsNot Nothing Then
							finalArgDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(99.0 * (If(counter Is Nothing, 1, counter.size()))).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName(parameter.getSecond()).setIsArray(parameter.getFirst().contains("[]") OrElse parameter.getFirst().contains("...")).setArgIndex(counter.argMax()).build()).build())
						End If
					ElseIf typeNameOrArrayOfTypeNameMatches(parameter.getFirst(),GetType(Single).FullName,GetType(Double).FullName,GetType(Float).FullName,GetType(Double).FullName) Then
						constructorNamesEncountered.Add(parameter.getValue())
						Dim counter As Counter(Of Integer) = paramIndicesCount.getCounter(Pair.of(parameter.getSecond(), OpNamespace.ArgDescriptor.ArgType.FLOAT))
						If counter IsNot Nothing Then
							finalArgDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(99.0 * (If(counter Is Nothing, 1, (If(counter Is Nothing, 1, counter.size()))))).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DOUBLE).setName(parameter.getSecond()).setIsArray(parameter.getFirst().contains("[]")).setArgIndex(counter.argMax()).build()).build())
						End If
					ElseIf typeNameOrArrayOfTypeNameMatches(parameter.getFirst(),GetType(Boolean).FullName,GetType(Boolean).FullName) Then
						constructorNamesEncountered.Add(parameter.getValue())
						Dim counter As Counter(Of Integer) = paramIndicesCount.getCounter(Pair.of(parameter.getSecond(), OpNamespace.ArgDescriptor.ArgType.BOOL))
						If counter IsNot Nothing Then
							finalArgDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(99.0 * (If(counter Is Nothing, 1, (If(counter Is Nothing, 1, counter.size()))))).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.BOOL).setName(parameter.getSecond()).setIsArray(parameter.getFirst().contains("[]")).setArgIndex(counter.argMax()).build()).build())
						End If
					End If
				End Sub)
				End Sub)




				Dim fields As IList(Of ResolvedFieldDeclaration) = cu.findAll(GetType(FieldDeclaration)).Select(Function(input) getResolve(input)).Where(Function(input) input IsNot Nothing AndAlso Not input.isStatic()).ToList()
				floatIdx = 0
				inputIdx = 0
				outputIdx = 0
				intIdx = 0
				boolIdx = 0

				For Each field As ResolvedFieldDeclaration In fields
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					If Not constructorNamesEncountered.Contains(field.getName()) AndAlso typeNameOrArrayOfTypeNameMatches(field.getType().describe(),GetType(SDVariable).FullName,GetType(INDArray).FullName) Then
						If outputNames.Contains(field.getName()) Then
							argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(99.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.OUTPUT_TENSOR).setName(field.getName()).setIsArray(field.getType().describe().contains("[]")).setArgIndex(outputIdx).build()).build())
							outputIdx += 1
						ElseIf Not constructorNamesEncountered.Contains(field.getName()) Then
							argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(99.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName(field.getName()).setIsArray(field.getType().describe().contains("[]")).setArgIndex(inputIdx).build()).build())
							inputIdx += 1
						End If
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					ElseIf Not constructorNamesEncountered.Contains(field.getName()) AndAlso typeNameOrArrayOfTypeNameMatches(field.getType().describe(),GetType(Integer).FullName,GetType(Long).FullName,GetType(Long).FullName,GetType(Integer).FullName) Then
						argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(99.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName(field.getName()).setIsArray(field.getType().describe().contains("[]")).setArgIndex(intIdx).build()).build())
						intIdx += 1
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					ElseIf Not constructorNamesEncountered.Contains(field.getName()) AndAlso typeNameOrArrayOfTypeNameMatches(field.getType().describe(),GetType(Double).FullName,GetType(Single).FullName,GetType(Double).FullName,GetType(Float).FullName) Then
						argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(99.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DOUBLE).setName(field.getName()).setIsArray(field.getType().describe().contains("[]")).setArgIndex(floatIdx).build()).build())
						floatIdx += 1
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					ElseIf Not constructorNamesEncountered.Contains(field.getName()) AndAlso typeNameOrArrayOfTypeNameMatches(field.getType().describe(),GetType(Boolean).FullName,GetType(Boolean).FullName) Then
						argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(99.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.BOOL).setName(field.getName()).setIsArray(field.getType().describe().contains("[]")).setArgIndex(boolIdx).build()).build())
						boolIdx += 1
					End If
				Next field

				If TypeOf funcInstance Is BaseReduceOp OrElse TypeOf funcInstance Is BaseReduceBoolOp OrElse TypeOf funcInstance Is BaseReduceSameOp Then
					If Not containsProposalWithDescriptorName("keepDims",argDescriptorProposals) Then
						argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.BOOL).setName("keepDims").setIsArray(False).setArgIndex(boolIdx).build()).build())

						argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("dimensions").setIsArray(False).setArgIndex(1).build()).build())
					End If

					If TypeOf funcInstance Is BaseTransformBoolOp Then
						Dim baseTransformBoolOp As BaseTransformBoolOp = DirectCast(funcInstance, BaseTransformBoolOp)
						If baseTransformBoolOp.OpType = Op.Type.PAIRWISE_BOOL Then
							If numProposalsWithType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR,argDescriptorProposals) < 2 Then
								argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("y").setIsArray(False).setArgIndex(1).build()).build())
							End If
						End If
					End If


					If Not containsProposalWithDescriptorName("dimensions",argDescriptorProposals) Then
						argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("dimensions").setIsArray(True).setArgIndex(0).build()).build())

					End If
				End If

				If TypeOf funcInstance Is BaseDynamicTransformOp Then
					If Not containsProposalWithDescriptorName("inPlace",argDescriptorProposals) Then
						argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.BOOL).setName("inPlace").setIsArray(False).setArgIndex(boolIdx).build()).build())
					End If
				End If

				'hard coded case, impossible to parse from as the code exists today, and it doesn't exist anywhere in the libnd4j code base
				If name.Contains("maxpool2d") Then
					If Not containsProposalWithDescriptorName("extraParam0",argDescriptorProposals) Then
						argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("extraParam0").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("extraParam0").setIsArray(False).setArgIndex(9).build()).build())
					End If
				End If

				If name.Contains("scatter_update") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("indices").setIsArray(False).setArgIndex(2).build()).build())

				End If


				If name.Contains("fill") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("shape").setIsArray(False).setArgIndex(0).build()).build())

					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("result").setIsArray(False).setArgIndex(1).build()).build())

				End If

				If name.Contains("loop_cond") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("input").setIsArray(False).setArgIndex(0).build()).build())

				End If


				If name.Equals("top_k") Then
					If Not containsProposalWithDescriptorName("sorted",argDescriptorProposals) Then
						argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("sorted").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("sorted").setIsArray(False).setArgIndex(0).build()).build())
					End If
				End If

				'dummy output tensor
				If name.Equals("next_iteration") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(0).setArgType(OpNamespace.ArgDescriptor.ArgType.OUTPUT_TENSOR).setName("output").build()).build())
				End If

				If Not containsOutputTensor(argDescriptorProposals) Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("z").proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.OUTPUT_TENSOR).setName("z").setIsArray(False).setArgIndex(0).build()).build())
				End If

				If name.Equals("gather") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("axis").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("axis").setIsArray(False).setArgIndex(0).build()).build())
				End If

				If name.Equals("pow") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("pow").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("pow").setIsArray(False).setArgIndex(1).build()).build())
				End If

				If name.Equals("concat") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("isDynamicAxis").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.BOOL).setName("isDynamicAxis").setIsArray(False).setArgIndex(0).build()).build())
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("concatDimension").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("isDynamicAxis").setIsArray(False).setArgIndex(1).build()).build())
				End If

				If name.Equals("merge") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(99999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(0).setIsArray(True).setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("inputs").build()).build())
				End If



				If name.Equals("split") OrElse name.Equals("split_v") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("numSplit").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("numSplit").setIsArray(False).setArgIndex(0).build()).build())
				End If

				If name.Equals("reshape") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("shape").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("shape").setIsArray(False).setArgIndex(0).build()).build())

					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("shape").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR).setName("shape").setIsArray(False).setArgIndex(1).build()).build())

				End If

				If name.Equals("create") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DATA_TYPE).setName("outputType").setIsArray(False).setArgIndex(0).build()).build())

					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("order").setIsArray(False).setArgIndex(0).build()).build())
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("java").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("outputType").setIsArray(False).setArgIndex(1).build()).build())
				End If

				If name.Equals("eye") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("numRows").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("numRows").setIsArray(False).setArgIndex(0).build()).build())

					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("numCols").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("numCols").setIsArray(False).setArgIndex(1).build()).build())

					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("batchDimension").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("batchDimension").setIsArray(True).setArgIndex(2).build()).build())

					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("dataType").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName("dataType").setIsArray(False).setArgIndex(3).build()).build())


					argDescriptorProposals.Add(ArgDescriptorProposal.builder().sourceOfProposal("dataType").proposalWeight(Double.MaxValue).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgType(OpNamespace.ArgDescriptor.ArgType.DOUBLE).setName("dataType").setIsArray(True).setArgIndex(0).build()).build())
				End If

				If name.Equals("while") OrElse name.Equals("enter") OrElse name.Equals("exit") OrElse name.Equals("next_iteration") OrElse name.Equals("loop_cond") OrElse name.Equals("switch") OrElse name.Equals("While") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(0).setArgType(OpNamespace.ArgDescriptor.ArgType.STRING).setName("frameName").build()).build())
				End If

				If name.Equals("resize_bilinear") Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(99999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(0).setArgType(OpNamespace.ArgDescriptor.ArgType.BOOL).setName("alignCorners").build()).build())

					argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(99999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(1).setArgType(OpNamespace.ArgDescriptor.ArgType.BOOL).setName("halfPixelCenters").build()).build())
				End If

				If TypeOf funcInstance Is BaseTransformSameOp OrElse TypeOf funcInstance Is BaseTransformOp OrElse TypeOf funcInstance Is BaseDynamicTransformOp Then
					argDescriptorProposals.Add(ArgDescriptorProposal.builder().proposalWeight(9999.0).descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(0).setArgType(OpNamespace.ArgDescriptor.ArgType.DATA_TYPE).setName("dataType").build()).build())
				End If


			Catch e As Exception
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			End Try
		End Sub


		Private Shared Function getResolve(ByVal input As FieldDeclaration) As ResolvedFieldDeclaration
			Try
				Return input.resolve()
			Catch e As Exception
				Return Nothing
			End Try
		End Function


		Private Function initSourceRoot(ByVal nd4jApiRootDir As File) As SourceRoot
			Dim typeSolver As New CombinedTypeSolver()
			typeSolver.add(New ReflectionTypeSolver(False))
			typeSolver.add(New JavaParserTypeSolver(nd4jApiRootDir))
			Dim symbolSolver As New JavaSymbolSolver(typeSolver)
			StaticJavaParser.getConfiguration().setSymbolResolver(symbolSolver)
			Dim sourceRoot As New SourceRoot(nd4jApiRootDir.toPath(),(New ParserConfiguration()).setSymbolResolver(symbolSolver))
			Return sourceRoot
		End Function

		Public Overridable ReadOnly Property Proposals As IDictionary(Of String, IList(Of ArgDescriptorProposal))
			Get
				Return doReflectionsExtraction()
			End Get
		End Property

		Public Overridable Function typeFor(ByVal name As String) As OpNamespace.OpDescriptor.OpDeclarationType Implements ArgDescriptorSource.typeFor
			Return opTypes(name)
		End Function
	End Class

End Namespace