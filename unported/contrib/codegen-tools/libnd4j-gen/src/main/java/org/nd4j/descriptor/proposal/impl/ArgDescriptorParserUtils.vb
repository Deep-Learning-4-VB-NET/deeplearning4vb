Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports MethodCallExpr = com.github.javaparser.ast.expr.MethodCallExpr
Imports ResolvedMethodDeclaration = com.github.javaparser.resolution.declarations.ResolvedMethodDeclaration
Imports ResolvedParameterDeclaration = com.github.javaparser.resolution.declarations.ResolvedParameterDeclaration
Imports val = lombok.val
Imports LevenshteinDistance = org.apache.commons.text.similarity.LevenshteinDistance
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.common.primitives
Imports OpDeclarationDescriptor = org.nd4j.descriptor.OpDeclarationDescriptor
Imports ArgDescriptorProposal = org.nd4j.descriptor.proposal.ArgDescriptorProposal
Imports OpNamespace = org.nd4j.ir.OpNamespace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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


	Public Class ArgDescriptorParserUtils
		Public Const DEFAULT_OUTPUT_FILE As String = "op-ir.proto"
		Public Shared ReadOnly numberPattern As Pattern = Pattern.compile("\([\d]+\)")


		Public Const ARGUMENT_ENDING_PATTERN As String = "\([\w\d+-\\*\/]+\);"
		Public Const ARGUMENT_PATTERN As String = "\([\w\d+-\\*\/]+\)"

		Public Const ARRAY_ASSIGNMENT As String = "\w+\[[a-zA-Z]+\]\s*=\s*[A-Z]+_[A-Z]+\(\s*[\d\w+-\/\*\s]+\);"

		Public Shared ReadOnly bannedMaxIndexOps As ISet(Of String) = New HashSetAnonymousInnerClass()

		Private Class HashSetAnonymousInnerClass
			Inherits HashSet(Of String)

			Public Sub New()

				Me.add("embedding_lookup")
				Me.add("stack")
			End Sub

		End Class

		Public Shared ReadOnly bannedIndexChangeOps As ISet(Of String) = New HashSetAnonymousInnerClass2()

		Private Class HashSetAnonymousInnerClass2
			Inherits HashSet(Of String)

			Public Sub New()

				Me.add("gemm")
				Me.add("mmul")
				Me.add("matmul")
			End Sub

		End Class


		Public Shared ReadOnly cppTypes As ISet(Of String) = New HashSetAnonymousInnerClass3()

		Private Class HashSetAnonymousInnerClass3
			Inherits HashSet(Of String)

			Public Sub New()

				Me.add("int")
				Me.add("bool")
				Me.add("auto")
				Me.add("string")
				Me.add("float")
				Me.add("double")
				Me.add("char")
				Me.add("class")
				Me.add("uint")
			End Sub

		End Class

		Public Shared ReadOnly fieldNameFilters As ISet(Of String) = New HashSetAnonymousInnerClass4()

		Private Class HashSetAnonymousInnerClass4
			Inherits HashSet(Of String)

			Public Sub New()

				Me.add("sameDiff")
				Me.add("xVertexId")
				Me.add("yVertexId")
				Me.add("zVertexId")
				Me.add("extraArgs")
				Me.add("extraArgz")
				Me.add("dimensionz")
				Me.add("scalarValue")
				Me.add("dimensions")
				Me.add("jaxis")
				Me.add("inPlace")
			End Sub

		End Class

		Public Shared ReadOnly fieldNameFiltersDynamicCustomOps As ISet(Of String) = New HashSetAnonymousInnerClass5()

		Private Class HashSetAnonymousInnerClass5
			Inherits HashSet(Of String)

			Public Sub New()

				Me.add("sameDiff")
				Me.add("xVertexId")
				Me.add("yVertexId")
				Me.add("zVertexId")
				Me.add("extraArgs")
				Me.add("extraArgz")
				Me.add("dimensionz")
				Me.add("scalarValue")
				Me.add("jaxis")
				Me.add("inPlace")
				Me.add("inplaceCall")
			End Sub

		End Class

		Public Shared equivalentAttributeNames As IDictionary(Of String, String) = New HashMapAnonymousInnerClass()

		Private Class HashMapAnonymousInnerClass
			Inherits Dictionary(Of String, String)

			Public Sub New()

				Me.put("axis","dimensions")
				Me.put("dimensions","axis")
				Me.put("jaxis","dimensions")
				Me.put("dimensions","jaxis")
				Me.put("inplaceCall","inPlace")
				Me.put("inPlace","inplaceCall")
			End Sub

		End Class


		Public Shared dimensionNames As ISet(Of String) = New HashSetAnonymousInnerClass6()

		Private Class HashSetAnonymousInnerClass6
			Inherits HashSet(Of String)

			Public Sub New()

				Me.add("jaxis")
				Me.add("axis")
				Me.add("dimensions")
				Me.add("dimensionz")
				Me.add("dim")
				Me.add("axisVector")
				Me.add("axesI")
				Me.add("ax")
				Me.add("dims")
				Me.add("axes")
				Me.add("axesVector")
			End Sub

		End Class

		Public Shared inputNames As ISet(Of String) = New HashSetAnonymousInnerClass7()

		Private Class HashSetAnonymousInnerClass7
			Inherits HashSet(Of String)

			Public Sub New()

				Me.add("input")
				Me.add("inputs")
				Me.add("i_v")
				Me.add("x")
				Me.add("in")
				Me.add("args")
				Me.add("i_v1")
				Me.add("first")
				Me.add("layerInput")
				Me.add("in1")
				Me.add("arrays")
			End Sub

		End Class
		Public Shared input2Names As ISet(Of String) = New HashSetAnonymousInnerClass8()

		Private Class HashSetAnonymousInnerClass8
			Inherits HashSet(Of String)

			Public Sub New()

				Me.add("y")
				Me.add("i_v2")
				Me.add("second")
				Me.add("in2")
			End Sub

		End Class

		Public Shared outputNames As ISet(Of String) = New HashSetAnonymousInnerClass9()

		Private Class HashSetAnonymousInnerClass9
			Inherits HashSet(Of String)

			Public Sub New()

				Me.add("output")
				Me.add("outputs")
				Me.add("out")
				Me.add("result")
				Me.add("z")
				Me.add("outputArrays")
			End Sub

		End Class


		Public Shared inplaceNames As ISet(Of String) = New HashSetAnonymousInnerClass10()

		Private Class HashSetAnonymousInnerClass10
			Inherits HashSet(Of String)

			Public Sub New()

				Me.add("inPlace")
				Me.add("inplaceCall")
			End Sub

		End Class


		Public Shared Function argTypeForParam(ByVal parameterDeclaration As ResolvedParameterDeclaration) As OpNamespace.ArgDescriptor.ArgType
			Dim type As String = parameterDeclaration.describeType()
			Dim isEnum As Boolean = False
			Try
				isEnum = Type.GetType(parameterDeclaration.asParameter().describeType()).IsEnum
			Catch e As ClassNotFoundException

			End Try

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			If type.Contains(GetType(INDArray).FullName) OrElse type.Contains(GetType(SDVariable).FullName) Then
				If Not outputNames.Contains(parameterDeclaration.getName()) Then
					Return OpNamespace.ArgDescriptor.ArgType.INPUT_TENSOR
				Else
					Return OpNamespace.ArgDescriptor.ArgType.OUTPUT_TENSOR
				End If
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			ElseIf type.Contains(GetType(Double).FullName) OrElse type.Contains(GetType(Single).FullName) OrElse type.Contains(GetType(Float).FullName) OrElse type.Contains(GetType(Double).FullName) Then
				Return OpNamespace.ArgDescriptor.ArgType.DOUBLE
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			ElseIf type.Contains(GetType(Integer).FullName) OrElse type.Contains(GetType(Long).FullName) OrElse type.Contains(GetType(Integer).FullName) OrElse type.Contains(GetType(Long).FullName) OrElse isEnum Then
				Return OpNamespace.ArgDescriptor.ArgType.INT64
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			ElseIf type.Contains(GetType(Boolean).FullName) OrElse type.Contains(GetType(Boolean).FullName) Then
				Return OpNamespace.ArgDescriptor.ArgType.BOOL
			Else
				Return OpNamespace.ArgDescriptor.ArgType.UNRECOGNIZED
			End If
		End Function


		Public Shared Function paramIsEnum(ByVal paramType As String) As Boolean
			Try
				Return Type.GetType(paramType).IsEnum
			Catch e As ClassNotFoundException
				Return False
			End Try
		End Function


		Public Shared Function paramIsEnum(ByVal param As ResolvedParameterDeclaration) As Boolean
			Return paramIsEnum(param.describeType())
		End Function


		Public Shared Function isValidParam(ByVal param As ResolvedParameterDeclaration) As Boolean
			Dim describedClassIsEnum As Boolean = False
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Dim ret As Boolean = param.describeType().contains(GetType(INDArray).FullName) OrElse param.describeType().contains(GetType(Boolean).FullName) OrElse param.describeType().contains(GetType(Boolean).FullName) OrElse param.describeType().contains(GetType(SDVariable).FullName) OrElse param.describeType().contains(GetType(Integer).FullName) OrElse param.describeType().contains(GetType(Integer).FullName) OrElse param.describeType().contains(GetType(Double).FullName) OrElse param.describeType().contains(GetType(Double).FullName) OrElse param.describeType().contains(GetType(Single).FullName) OrElse param.describeType().contains(GetType(Float).FullName) OrElse param.describeType().contains(GetType(Long).FullName) OrElse param.describeType().contains(GetType(Long).FullName)
			Try
				describedClassIsEnum = Type.GetType(param.asParameter().describeType()).IsEnum
			Catch e As ClassNotFoundException

			End Try
			Return ret OrElse describedClassIsEnum
		End Function

		Public Shared Function tryResolve(ByVal methodCallExpr As MethodCallExpr) As ResolvedMethodDeclaration
			Try
				Return methodCallExpr.resolve()
			Catch e As Exception

			End Try
			Return Nothing
		End Function

		Public Shared Function typeNameOrArrayOfTypeNameMatches(ByVal typeName As String, ParamArray ByVal types() As String) As Boolean
			Dim ret As Boolean = False
			For Each type As String In types
				ret = typeName.Equals(type) OrElse typeName.Equals(type & "...") OrElse typeName.Equals(type & "[]") OrElse ret

			Next type

			Return ret
		End Function


		Public Shared Function equivalentAttribute(ByVal comp1 As OpNamespace.ArgDescriptor, ByVal comp2 As OpNamespace.ArgDescriptor) As Boolean
			If equivalentAttributeNames.ContainsKey(comp1.Name) Then
				Return equivalentAttributeNames(comp1.Name).Equals(comp2.Name)
			End If

			If equivalentAttributeNames.ContainsKey(comp2.Name) Then
				Return equivalentAttributeNames(comp2.Name).Equals(comp1.Name)
			End If
			Return False
		End Function

		Public Shared Function argsListContainsEquivalentAttribute(ByVal argDescriptors As IList(Of OpNamespace.ArgDescriptor), ByVal [to] As OpNamespace.ArgDescriptor) As Boolean
			For Each argDescriptor As OpNamespace.ArgDescriptor In argDescriptors
				If argDescriptor.ArgType = [to].ArgType AndAlso equivalentAttribute(argDescriptor,[to]) Then
					Return True
				End If
			Next argDescriptor

			Return False
		End Function

		Public Shared Function argsListContainsSimilarArg(ByVal argDescriptors As IList(Of OpNamespace.ArgDescriptor), ByVal [to] As OpNamespace.ArgDescriptor, ByVal threshold As Integer) As Boolean
			For Each argDescriptor As OpNamespace.ArgDescriptor In argDescriptors
				If argDescriptor.ArgType = [to].ArgType AndAlso LevenshteinDistance.getDefaultInstance().apply(argDescriptor.Name.ToLower(),[to].Name.ToLower()) <= threshold Then
					Return True
				End If
			Next argDescriptor

			Return False
		End Function

		Public Shared Function mergeDescriptorsOfSameIndex(ByVal one As OpNamespace.ArgDescriptor, ByVal two As OpNamespace.ArgDescriptor) As OpNamespace.ArgDescriptor
			If one.ArgIndex <> two.ArgIndex Then
				Throw New System.ArgumentException("Argument indices for both arg descriptors were not the same. First one was " & one.ArgIndex & " and second was " & two.ArgIndex)
			End If

			If one.ArgType <> two.ArgType Then
				Throw New System.ArgumentException("Merging two arg descriptors requires both be the same type. First one was " & one.ArgType.name() & " and second one was " & two.ArgType.name())
			End If

			Dim newDescriptor As OpNamespace.ArgDescriptor.Builder = OpNamespace.ArgDescriptor.newBuilder()
			'arg indices will be the same
			newDescriptor.ArgIndex = one.ArgIndex
			newDescriptor.ArgType = one.ArgType
			If Not isValidIdentifier(one.Name) AndAlso Not isValidIdentifier(two.Name) Then
				newDescriptor.Name = "arg" & newDescriptor.ArgIndex
			ElseIf Not isValidIdentifier(one.Name) Then
				newDescriptor.Name = two.Name
			Else
				newDescriptor.Name = one.Name
			End If


			Return newDescriptor.build()
		End Function

		Public Shared Function isValidIdentifier(ByVal input As String) As Boolean
			If input Is Nothing OrElse input.Length = 0 Then
				Return False
			End If

			For i As Integer = 0 To input.Length - 1
				If Not Character.isJavaIdentifierPart(input.Chars(i)) Then
					Return False
				End If
			Next i

			If cppTypes.Contains(input) Then
				Return False
			End If

			Return True
		End Function

		Public Shared Function containsOutputTensor(ByVal proposals As ICollection(Of ArgDescriptorProposal)) As Boolean
			For Each proposal As ArgDescriptorProposal In proposals
				If proposal.getDescriptor().getArgType() = OpNamespace.ArgDescriptor.ArgType.OUTPUT_TENSOR Then
					Return True
				End If
			Next proposal

			Return False
		End Function


		Public Shared Function getDescriptorWithName(ByVal name As String, ByVal proposals As ICollection(Of ArgDescriptorProposal)) As OpNamespace.ArgDescriptor
			For Each proposal As ArgDescriptorProposal In proposals
				If proposal.getDescriptor().getName().Equals(name) Then
					Return proposal.getDescriptor()
				End If
			Next proposal

			Return Nothing
		End Function


		Public Shared Function numProposalsWithType(ByVal argType As OpNamespace.ArgDescriptor.ArgType, ByVal proposals As ICollection(Of ArgDescriptorProposal)) As Integer
			Dim count As Integer = 0
			For Each proposal As ArgDescriptorProposal In proposals
				If proposal.getDescriptor().getArgType() = argType Then
					count += 1
				End If
			Next proposal

			Return count
		End Function

		Public Shared Function containsProposalWithDescriptorName(ByVal name As String, ByVal proposals As ICollection(Of ArgDescriptorProposal)) As Boolean
			For Each proposal As ArgDescriptorProposal In proposals
				If proposal.getDescriptor().getName().Equals(name) Then
					Return True
				End If
			Next proposal

			Return False
		End Function

		Public Overridable Function updateOpDescriptor(ByVal opDescriptor As OpNamespace.OpDescriptor, ByVal declarationDescriptor As OpDeclarationDescriptor, ByVal argsByIIndex As IList(Of String), ByVal int64 As OpNamespace.ArgDescriptor.ArgType) As IList(Of ArgDescriptorProposal)
			Dim copyValuesInt As IList(Of OpNamespace.ArgDescriptor) = addArgDescriptors(opDescriptor, declarationDescriptor, argsByIIndex, int64)
			Dim proposals As IList(Of ArgDescriptorProposal) = New List(Of ArgDescriptorProposal)()

			Return proposals
		End Function

		Public Shared Function addArgDescriptors(ByVal opDescriptor As OpNamespace.OpDescriptor, ByVal declarationDescriptor As OpDeclarationDescriptor, ByVal argsByTIndex As IList(Of String), ByVal argType As OpNamespace.ArgDescriptor.ArgType) As IList(Of OpNamespace.ArgDescriptor)
			Dim copyValuesFloat As IList(Of OpNamespace.ArgDescriptor) = New List(Of OpNamespace.ArgDescriptor)(opDescriptor.getArgDescriptorList())
			For i As Integer = 0 To argsByTIndex.Count - 1
				Dim argDescriptor As OpNamespace.ArgDescriptor = OpNamespace.ArgDescriptor.newBuilder().setArgType(argType).setName(argsByTIndex(i)).setArgIndex(i).setArgOptional(If(declarationDescriptor IsNot Nothing AndAlso i <= declarationDescriptor.getTArgs(), False, True)).build()
				copyValuesFloat.Add(argDescriptor)

			Next i
			Return copyValuesFloat
		End Function

		Public Shared Function argIndexForCsv(ByVal line As String) As IDictionary(Of String, Integer)
			Dim ret As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			Dim lineSplit() As String = line.Split(",", True)
			For i As Integer = 0 To lineSplit.Length - 1
				ret(lineSplit(i)) = i
			Next i

			Return ret
		End Function

		Public Shared Function extractArgFromJava(ByVal line As String) As Integer?
			Dim matcher As Matcher = numberPattern.matcher(line)
			If Not matcher.find() Then
				Throw New System.ArgumentException("No number found for line " & line)
			End If

			Return Integer.Parse(matcher.group())
		End Function

		Public Shared Function extractArgFromCpp(ByVal line As String, ByVal argType As String) As Integer?
			Dim matcher As Matcher = Pattern.compile(argType & "\([\d]+\)").matcher(line)
			If Not matcher.find() Then
				'Generally not resolvable
				Return -1
			End If

			If matcher.groupCount() > 1 Then
				Throw New System.ArgumentException("Line contains more than 1 index")
			End If

			Try
				Return Integer.Parse(matcher.group().replace("(","").replace(")","").replace(argType,""))
			Catch e As System.FormatException
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
				Return -1
			End Try
		End Function

		Public Shared Function getAllFields(ByVal clazz As Type) As IList(Of System.Reflection.FieldInfo)
			If clazz Is Nothing Then
				Return java.util.Collections.emptyList()
			End If

			Dim result As IList(Of System.Reflection.FieldInfo) = New List(Of System.Reflection.FieldInfo)(getAllFields(clazz.BaseType))
			Dim filteredFields As IList(Of System.Reflection.FieldInfo) = clazz.GetFields(BindingFlags.DeclaredOnly Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance).Where(Function(f) Modifier.isPublic(f.getModifiers()) OrElse Modifier.isProtected(f.getModifiers())).ToList()
			CType(result, List(Of System.Reflection.FieldInfo)).AddRange(filteredFields)
			Return result
		End Function

		Public Shared Function removeBracesFromDeclarationMacro(ByVal line As String, ByVal nameOfMacro As String) As String
			line = line.Replace(nameOfMacro & "(", "")
			line = line.Replace(")", "")
			line = line.Replace("{", "")
			line = line.Replace(";","")
			Return line
		End Function

		Public Shared Sub addNameToList(ByVal line As String, ByVal list As IList(Of String), ByVal argIndices As IList(Of Integer), ByVal argType As String)
			Dim split() As String = line.Split(" = ", True)
			Dim arrSplit() As String = split(0).Split(" ", True)
			'type + name
			Dim name As String = arrSplit(arrSplit.Length - 1)
			Preconditions.checkState(name.Length > 0)
			If Not list.Contains(name) Then
				list.Add(name)
			End If

			Dim index As Integer? = extractArgFromCpp(line,argType)
			If index IsNot Nothing Then
				argIndices.Add(index)
			End If
		End Sub

		Public Shared Sub addArrayNameToList(ByVal line As String, ByVal list As IList(Of String), ByVal argIndices As IList(Of Integer), ByVal argType As String)
			Dim split() As String = line.Split(" = ", True)
			Dim arrSplit() As String = split(0).Split(" ", True)
			'type + name
			Dim name As String = arrSplit(arrSplit.Length - 1)
			Preconditions.checkState(name.Length > 0)
			If Not list.Contains(name) Then
				list.Add(name)
			End If
			'arrays are generally appended to the end
			Dim index As Integer? = - 1
			If index IsNot Nothing Then
				argIndices.Add(index)
			End If
		End Sub

		Public Shared Sub standardizeTypes(ByVal input As IList(Of ArgDescriptorProposal))
			input.ForEach(Sub(proposal)
			If proposal.getDescriptor().getArgType() = OpNamespace.ArgDescriptor.ArgType.BOOL AndAlso proposal.getDescriptor().getConvertBoolToInt() Then
				Dim newDescriptor As OpNamespace.ArgDescriptor = OpNamespace.ArgDescriptor.newBuilder().setArgIndex(proposal.getDescriptor().getArgIndex()).setArgType(OpNamespace.ArgDescriptor.ArgType.INT64).setName(proposal.getDescriptor().getName()).build()
				proposal.setDescriptor(newDescriptor)
			End If
			End Sub)
		End Sub

		Public Shared Function aggregateProposals(ByVal listOfProposals As IList(Of ArgDescriptorProposal)) As ArgDescriptorProposal
			Dim descriptorBuilder As val = OpNamespace.ArgDescriptor.newBuilder()
			Dim mostLikelyIndex As New Counter(Of Integer)()

			Dim aggregatedWeight As New AtomicDouble(0.0)
			listOfProposals.ForEach(Sub(proposal)
			mostLikelyIndex.incrementCount(proposal.getDescriptor().getArgIndex(),1.0)
			aggregatedWeight.addAndGet(proposal.getProposalWeight())
			descriptorBuilder.setName(proposal.getDescriptor().getName())
			descriptorBuilder.setIsArray(proposal.getDescriptor().getIsArray())
			descriptorBuilder.setArgType(proposal.getDescriptor().getArgType())
			descriptorBuilder.setConvertBoolToInt(proposal.getDescriptor().getConvertBoolToInt())
			descriptorBuilder.setIsArray(proposal.getDescriptor().getIsArray())
			End Sub)

			'set the index after computing the most likely index
			descriptorBuilder.setArgIndex(mostLikelyIndex.argMax())

			Return ArgDescriptorProposal.builder().descriptor(descriptorBuilder.build()).proposalWeight(aggregatedWeight.doubleValue()).build()
		End Function

		Public Shared Function standardizeNames(ByVal toStandardize As IDictionary(Of String, IList(Of ArgDescriptorProposal)), ByVal opName As String) As IDictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), OpNamespace.ArgDescriptor)
			Dim ret As IDictionary(Of String, IList(Of ArgDescriptorProposal)) = New Dictionary(Of String, IList(Of ArgDescriptorProposal))()
			Dim dimensionProposals As IDictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), IList(Of ArgDescriptorProposal)) = New Dictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), IList(Of ArgDescriptorProposal))()
			Dim inPlaceProposals As IDictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), IList(Of ArgDescriptorProposal)) = New Dictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), IList(Of ArgDescriptorProposal))()
			Dim inputsProposals As IDictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), IList(Of ArgDescriptorProposal)) = New Dictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), IList(Of ArgDescriptorProposal))()
			Dim inputs2Proposals As IDictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), IList(Of ArgDescriptorProposal)) = New Dictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), IList(Of ArgDescriptorProposal))()
			Dim outputsProposals As IDictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), IList(Of ArgDescriptorProposal)) = New Dictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), IList(Of ArgDescriptorProposal))()

			toStandardize.SetOfKeyValuePairs().forEach(Sub(entry)
			If entry.getKey().isEmpty() Then
				Throw New System.InvalidOperationException("Name must not be empty!")
			End If
			If dimensionNames.Contains(entry.getKey()) Then
				extractProposals(dimensionProposals, entry)
			ElseIf inplaceNames.Contains(entry.getKey()) Then
				extractProposals(inPlaceProposals, entry)
			ElseIf inputNames.Contains(entry.getKey()) Then
				extractProposals(inputsProposals, entry)
			ElseIf input2Names.Contains(entry.getKey()) Then
				extractProposals(inputs2Proposals, entry)
			ElseIf outputNames.Contains(entry.getKey()) Then
				extractProposals(outputsProposals, entry)
			Else
				ret(entry.getKey()) = entry.getValue()
			End If
			End Sub)


			''' <summary>
			''' Two core ops have issues:
			''' argmax and cumsum both have the same issue
			''' other boolean attributes are present
			''' that are converted to ints and get clobbered
			''' by dimensions having the wrong index
			''' 
			''' For argmax, exclusive gets clobbered. It should have index 0,
			''' but for some reason dimensions gets index 0.
			''' </summary>


			''' <summary>
			''' TODO: make this method return name/type
			''' combinations rather than just name/single list.
			''' </summary>
			If dimensionProposals.Count > 0 Then
				' List<Pair<Pair<Integer, OpNamespace.ArgDescriptor.ArgType>, ArgDescriptorProposal>> d
				computeAggregatedProposalsPerType(ret, dimensionProposals, "dimensions")
			End If

			If inPlaceProposals.Count > 0 Then
				computeAggregatedProposalsPerType(ret, inPlaceProposals, "inPlace")
			End If

			If inputsProposals.Count > 0 Then
				computeAggregatedProposalsPerType(ret, inputsProposals, "input")

			End If

			If inputs2Proposals.Count > 0 Then
				computeAggregatedProposalsPerType(ret, inputs2Proposals, "y")

			End If

			If outputsProposals.Count > 0 Then
				computeAggregatedProposalsPerType(ret, outputsProposals, "outputs")
			End If

			Dim ret2 As IDictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), OpNamespace.ArgDescriptor) = New Dictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), OpNamespace.ArgDescriptor)()
			Dim proposalsByType As New CounterMap(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), ArgDescriptorProposal)()
			ret.Values.forEach(Sub(input)
			input.forEach(Sub(proposal1)
				proposalsByType.incrementCount(Pair.of(proposal1.getDescriptor().getArgIndex(),proposal1.getDescriptor().getArgType()),proposal1,proposal1.getProposalWeight())
			End Sub)
			End Sub)

			ret.Clear()
			proposalsByType.keySet().ForEach(Sub(argTypeIndexPair)
			Dim proposal As val = proposalsByType.getCounter(argTypeIndexPair).argMax()
			Dim name As val = proposal.getDescriptor().getName()
			Dim proposalsForName As IList(Of ArgDescriptorProposal)
			If Not ret.ContainsKey(name) Then
				proposalsForName = New List(Of ArgDescriptorProposal)()
				ret(name) = proposalsForName
			Else
				proposalsForName = ret(name)
			End If
			proposalsForName.Add(proposal)
			ret(proposal.getDescriptor().getName()) = proposalsForName
			End Sub)

			ret.forEach(Sub(name,proposals)
			Dim proposalsGroupedByType As val = proposals.collect(Collectors.groupingBy(Function(proposal) proposal.getDescriptor().getArgType()))
			Dim maxProposalsForEachType As IList(Of ArgDescriptorProposal) = New List(Of ArgDescriptorProposal)()
			proposalsGroupedByType.forEach(Sub(type,proposalGroupByType)
				Dim proposalsCounter As New Counter(Of ArgDescriptorProposal)()
				proposalGroupByType.forEach(Sub(proposalByType)
					proposalsCounter.incrementCount(proposalByType,proposalByType.getProposalWeight())
				End Sub)
				maxProposalsForEachType.Add(proposalsCounter.argMax())
			End Sub)
			proposals = maxProposalsForEachType
			Dim collected As val = proposals.collect(Collectors.groupingBy(Function(input) Pair.of(input.getDescriptor().getArgIndex(),input.getDescriptor().getArgType()))).entrySet().stream().Select(Function(input) Pair.of(input.getKey(), aggregateProposals(input.getValue()).getDescriptor())).ToDictionary(Function(pair) pair.getKey(),Function(pair) pair.getValue())
			Dim groupedByType As val = collected.entrySet().collect(Collectors.groupingBy(Function(input) input.getKey().getRight()))
			groupedByType.forEach(Sub(argType,list)
				Dim numGreaterThanNegativeOne As Integer = list.Select(Function(input)If(input.getKey().getFirst() >= 0, 1, 0)).Aggregate(0,Function(a,b) a + b)
				If numGreaterThanNegativeOne > 1 Then
					Throw New System.InvalidOperationException("Name of " & name & " with type " & argType & " not aggregated properly.")
				End If
			End Sub)
			Dim arrEntries As val = collected.entrySet().Where(Function(pair) pair.getValue().getIsArray()).ToList()
			If Not arrEntries.isEmpty() Then
				Dim initialType As val = arrEntries.get(0).getValue().getArgType()
				Dim allSameType As val = New AtomicBoolean(True)
				Dim negativeOnePresent As val = New AtomicBoolean(False)
				arrEntries.forEach(Sub(entry)
					allSameType.set(allSameType.get() AndAlso entry.getValue().getArgType() = initialType)
					negativeOnePresent.set(negativeOnePresent.get() OrElse entry.getValue().getArgIndex() = -1)
					If negativeOnePresent.get() Then
						collected.remove(entry.getKey())
					End If
				End Sub)
				If allSameType.get() AndAlso negativeOnePresent.get() Then
					collected.put(Pair.of(-1,initialType), OpNamespace.ArgDescriptor.newBuilder().setArgType(initialType).setArgIndex(-1).setIsArray(True).setName(arrEntries.get(0).getValue().getName()).build())
				End If
			End If
			ret2.PutAll(collected)
			End Sub)

			Dim maxIndex As IDictionary(Of OpNamespace.ArgDescriptor.ArgType, Integer) = New Dictionary(Of OpNamespace.ArgDescriptor.ArgType, Integer)()
			If Not bannedMaxIndexOps.Contains(opName) Then
				ret2.forEach(Sub(key,value)
				If Not maxIndex.ContainsKey(key.getRight()) Then
					maxIndex(key.getValue()) = key.getFirst()
				Else
					maxIndex(key.getValue()) = Math.Max(key.getFirst(),maxIndex(key.getValue()))
				End If
				End Sub)
			End If

			'update -1 values to be valid indices relative to whatever the last index is when an array is found
			'and -1 is present
			Dim updateValues As IDictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), OpNamespace.ArgDescriptor) = New Dictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), OpNamespace.ArgDescriptor)()
			Dim removeKeys As ISet(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType)) = New HashSet(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType))()
			If Not bannedMaxIndexOps.Contains(opName) Then
				ret2.forEach(Sub(key,value)
				If value.getArgIndex() < 0 Then
					removeKeys.Add(key)
					Dim maxIdx As Integer = maxIndex(value.getArgType())
					updateValues(Pair.of(maxIdx + 1,value.getArgType())) = OpNamespace.ArgDescriptor.newBuilder().setName(value.getName()).setIsArray(value.getIsArray()).setArgType(value.getArgType()).setArgIndex(maxIdx + 1).setConvertBoolToInt(value.getConvertBoolToInt()).build()
				End If
				End Sub)
			End If

			removeKeys.forEach(Function(key) ret2.Remove(key))
			ret2.PutAll(updateValues)
			Return ret2
		End Function

		Private Shared Sub computeAggregatedProposalsPerType(ByVal ret As IDictionary(Of String, IList(Of ArgDescriptorProposal)), ByVal dimensionProposals As IDictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), IList(Of ArgDescriptorProposal)), ByVal name As String)
			Dim dimensions As IList(Of ArgDescriptorProposal) = dimensionProposals.SetOfKeyValuePairs().Select(Function(indexTypeAndList)
			If indexTypeAndList.getValue().isEmpty() Then
				Throw New System.InvalidOperationException("Unable to compute aggregated proposals for an empty list")
			End If
			Dim template As OpNamespace.ArgDescriptor = indexTypeAndList.getValue().get(0).getDescriptor()
			Dim idx As Integer = indexTypeAndList.getKey().getFirst()
			Dim type As OpNamespace.ArgDescriptor.ArgType = indexTypeAndList.getKey().getRight()
			Return Pair.of(indexTypeAndList.getKey(), ArgDescriptorProposal.builder().sourceOfProposal("computedAggregate").descriptor(OpNamespace.ArgDescriptor.newBuilder().setArgIndex(idx).setArgType(type).setName(name).setIsArray(template.IsArray OrElse idx < 0).build()).proposalWeight(indexTypeAndList.getValue().collect(Collectors.summingDouble(Function(input) input.getProposalWeight()))).build())
			End Function).Select(Function(input) input.getSecond()).ToList()


			ret(name) = dimensions
		End Sub

		Private Shared Sub extractProposals(ByVal inPlaceProposals As IDictionary(Of Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType), IList(Of ArgDescriptorProposal)), ByVal entry As KeyValuePair(Of String, IList(Of ArgDescriptorProposal)))
			entry.Value.forEach(Sub(proposal)
			Dim proposals As IList(Of ArgDescriptorProposal) = Nothing
			If Not inPlaceProposals.ContainsKey(extractKey(proposal)) Then
				proposals = New List(Of ArgDescriptorProposal)()
				inPlaceProposals(extractKey(proposal)) = proposals
			Else
				proposals = inPlaceProposals(extractKey(proposal))
			End If
			proposals.Add(proposal)
			inPlaceProposals(extractKey(proposal)) = proposals
			End Sub)
		End Sub

		''' <summary>
		''' Extract a key reflecting index and type of arg descriptor </summary>
		''' <param name="proposal"> the input proposal
		''' @return </param>
		Public Shared Function extractKey(ByVal proposal As ArgDescriptorProposal) As Pair(Of Integer, OpNamespace.ArgDescriptor.ArgType)
			Return Pair.of(proposal.getDescriptor().getArgIndex(),proposal.getDescriptor().getArgType())
		End Function


		Public Shared Function proposalsAllSameType(ByVal proposals As IList(Of ArgDescriptorProposal)) As Boolean
			Dim firstType As OpNamespace.ArgDescriptor.ArgType = proposals(0).getDescriptor().getArgType()
			For Each proposal As ArgDescriptorProposal In proposals
				If proposal.getDescriptor().getArgType() <> firstType Then
					Return False
				End If
			Next proposal

			Return True
		End Function


		Private Shared Function mergeProposals(ByVal ret As IDictionary(Of String, IList(Of ArgDescriptorProposal)), ByVal dimensionsList As IList(Of ArgDescriptorProposal), ByVal argType As OpNamespace.ArgDescriptor.ArgType, ByVal nameOfArgDescriptor As String) As IList(Of ArgDescriptorProposal)
			Dim priorityWeight As Double = 0.0
			Dim newProposalBuilder As ArgDescriptorProposal.ArgDescriptorProposalBuilder = ArgDescriptorProposal.builder()
			Dim indexCounter As New Counter(Of Integer)()
			Dim proposalsOutsideType As IList(Of ArgDescriptorProposal) = New List(Of ArgDescriptorProposal)()
			Dim allArrayType As Boolean = True
			For Each argDescriptorProposal As ArgDescriptorProposal In dimensionsList
				allArrayType = argDescriptorProposal.getDescriptor().getIsArray() AndAlso allArrayType
				'handle arrays separately
				If argDescriptorProposal.getDescriptor().getArgType() = argType Then
					indexCounter.incrementCount(argDescriptorProposal.getDescriptor().getArgIndex(),1)
					priorityWeight += argDescriptorProposal.getProposalWeight()
				ElseIf argDescriptorProposal.getDescriptor().getArgType() <> argType Then
					proposalsOutsideType.Add(argDescriptorProposal)
				End If
			Next argDescriptorProposal

			dimensionsList.Clear()
			'don't add a list if one is not present
			If Not indexCounter.Empty Then
				newProposalBuilder.proposalWeight(priorityWeight).descriptor(OpNamespace.ArgDescriptor.newBuilder().setName(nameOfArgDescriptor).setArgType(argType).setIsArray(allArrayType).setArgIndex(indexCounter.argMax()).build())

				dimensionsList.Add(newProposalBuilder.build())
				ret(nameOfArgDescriptor) = dimensionsList
			End If

			'standardize the names
			proposalsOutsideType.ForEach(Sub(proposalOutsideType)
			proposalOutsideType.setDescriptor(OpNamespace.ArgDescriptor.newBuilder().setName(nameOfArgDescriptor).setArgType(proposalOutsideType.getDescriptor().getArgType()).setArgIndex(proposalOutsideType.getDescriptor().getArgIndex()).setIsArray(proposalOutsideType.getDescriptor().getIsArray()).setConvertBoolToInt(proposalOutsideType.getDescriptor().getConvertBoolToInt()).build())
			End Sub)


			Return proposalsOutsideType
		End Function


		Public Shared Function matchesArrayArgDeclaration(ByVal testLine As String) As Boolean
			Dim ret As Boolean = Pattern.matches(ARRAY_ASSIGNMENT,testLine)
			Return ret
		End Function

		Public Shared Function matchesArgDeclaration(ByVal argType As String, ByVal testLine As String) As Boolean
			Dim matcher As Matcher = Pattern.compile(argType & ARGUMENT_ENDING_PATTERN).matcher(testLine)
			Dim argOnly As Matcher = Pattern.compile(argType & ARGUMENT_PATTERN).matcher(testLine)
			' Matcher arrArg = Pattern.compile(argType + ARGUMENT_PATTERN)
			Dim ret As Boolean = matcher.find()
			Dim argOnlyResult As Boolean = argOnly.find()
			Return ret OrElse testLine.Contains("?") AndAlso argOnlyResult OrElse testLine.Contains("static_cast") AndAlso argOnlyResult OrElse (testLine.Contains("))") AndAlso argOnlyResult AndAlso Not testLine.Contains("if") AndAlso Not testLine.Contains("REQUIRE_TRUE")) AndAlso Not testLine.Contains("->rankOf()") OrElse (testLine.Contains("==") AndAlso argOnlyResult AndAlso Not testLine.Contains("if") AndAlso Not testLine.Contains("REQUIRE_TRUE")) AndAlso Not testLine.Contains("->rankOf()") OrElse (testLine.Contains("(" & argType) AndAlso argOnlyResult AndAlso Not testLine.Contains("if") AndAlso Not testLine.Contains("REQUIRE_TRUE")) AndAlso Not testLine.Contains("->rankOf()") OrElse (testLine.Contains("->") AndAlso argOnlyResult AndAlso Not testLine.Contains("if") AndAlso Not testLine.Contains("REQUIRE_TRUE")) AndAlso Not testLine.Contains("->rankOf()")
		End Function

	End Class

End Namespace