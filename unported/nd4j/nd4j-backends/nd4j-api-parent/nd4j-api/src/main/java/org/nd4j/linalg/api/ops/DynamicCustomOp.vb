Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Builder = lombok.Builder
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Onnx = onnx.Onnx
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports OpDescriptorHolder = org.nd4j.ir.OpDescriptorHolder
Imports OpNamespace = org.nd4j.ir.OpNamespace
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Lists = org.nd4j.shade.guava.collect.Lists
Imports Doubles = org.nd4j.shade.guava.primitives.Doubles
Imports Longs = org.nd4j.shade.guava.primitives.Longs
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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

Namespace org.nd4j.linalg.api.ops


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DynamicCustomOp extends org.nd4j.autodiff.functions.DifferentialFunction implements CustomOp
	Public Class DynamicCustomOp
		Inherits DifferentialFunction
		Implements CustomOp

'JAVA TO VB CONVERTER NOTE: The field opName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private opName_Conflict As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected List<org.nd4j.linalg.api.ndarray.INDArray> inputArguments = new ArrayList<>();
'JAVA TO VB CONVERTER NOTE: The field inputArguments was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend inputArguments_Conflict As IList(Of INDArray) = New List(Of INDArray)()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected List<org.nd4j.linalg.api.ndarray.INDArray> outputArguments = new ArrayList<>();
'JAVA TO VB CONVERTER NOTE: The field outputArguments was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend outputArguments_Conflict As IList(Of INDArray) = New List(Of INDArray)()


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected List<Double> tArguments = new ArrayList<>();
		Protected Friend tArguments As IList(Of Double) = New List(Of Double)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected List<Long> iArguments = new ArrayList<>();
		Protected Friend iArguments As IList(Of Long) = New List(Of Long)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected List<Boolean> bArguments = new ArrayList<>();
		Protected Friend bArguments As IList(Of Boolean) = New List(Of Boolean)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected List<org.nd4j.linalg.api.buffer.DataType> dArguments = new ArrayList<>();
		Protected Friend dArguments As IList(Of DataType) = New List(Of DataType)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected List<String> sArguments = new ArrayList<>();
		Protected Friend sArguments As IList(Of String) = New List(Of String)()


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected List<Integer> axis = new ArrayList<>();
		Protected Friend axis As IList(Of Integer) = New List(Of Integer)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected boolean inplaceCall;
		Protected Friend inplaceCall As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long hash;
		Private hash As Long
'JAVA TO VB CONVERTER NOTE: The field outputVariables was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend outputVariables_Conflict() As SDVariable
		Private outputShapes As IList(Of LongShapeDescriptor)

		Public Sub New()
			iArguments = New List(Of Long)()
			tArguments = New List(Of Double)()
			bArguments = New List(Of Boolean)()
			dArguments = New List(Of DataType)()
			sArguments = New List(Of String)()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal arg As SDVariable)
			Me.New(sameDiff, wrapOrNull(arg))
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			Me.New(Nothing, sameDiff, args)
		End Sub

		Public Sub New(ByVal opName As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			MyBase.New(sameDiff, args)
			Me.opName_Conflict = opName
			iArguments = New List(Of Long)()
			tArguments = New List(Of Double)()
			bArguments = New List(Of Boolean)()
			dArguments = New List(Of DataType)()
			sArguments = New List(Of String)()
		End Sub

		Public Sub New(ByVal opName As String, ByVal input As INDArray, ByVal output As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments() As Integer)
			Me.New(opName, (If(input Is Nothing, Nothing, New INDArray()){input}), (If(output Is Nothing, Nothing, New INDArray()){output}), tArguments, iArguments)
		End Sub

		Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments() As Integer)
			Me.New(opName, inputs, outputs, tArguments, ArrayUtil.toList(iArguments))
		End Sub

		''' <summary>
		''' Initialize this custom op with all of the
		''' inputs, outputs, and respective
		''' arguments for execution
		''' </summary>
		''' <param name="opName">     the opName of the op to execute </param>
		''' <param name="inputs">     the inputs to the op </param>
		''' <param name="outputs">    the outputs of the op </param>
		''' <param name="tArguments"> the input float arguments </param>
		''' <param name="iArguments"> the input int arguments </param>
		Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray, ByVal tArguments As IList(Of Double), ByVal iArguments As IList(Of Integer))
			If inputs IsNot Nothing Then
				inputArguments_Conflict = New List(Of INDArray)(java.util.Arrays.asList(inputs))
			End If
			If outputs IsNot Nothing Then
				outputArguments_Conflict = New List(Of INDArray)(java.util.Arrays.asList(outputs))
			End If
			Me.opName_Conflict = opName
			If tArguments Is Nothing Then
				Me.tArguments = New List(Of Double)()
			Else
				Me.tArguments = tArguments
			End If
			Me.iArguments = New List(Of Long)()
			Me.sArguments = New List(Of String)()

			If iArguments IsNot Nothing Then
				For Each a As val In iArguments
					Me.iArguments.Add(a.longValue())
				Next a
			End If
			bArguments = New List(Of Boolean)()
			dArguments = New List(Of DataType)()
		End Sub

		''' <summary>
		''' Initialize this operation for execution (pre created ndarrays)
		''' </summary>
		''' <param name="inputs">  the inputs </param>
		''' <param name="outputs"> the outputs of the op, may be null </param>
		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
			Me.New(Nothing, inputs, outputs)
		End Sub


		''' <summary>
		''' Initialize this operation for execution (pre created ndarrays)
		''' </summary>
		''' <param name="opName">  the operation opName to use for invocation </param>
		''' <param name="inputs">  the inputs </param>
		''' <param name="outputs"> the outputs of the op, may be null </param>
		Public Sub New(ByVal opName As String, ByVal inputs() As INDArray, ByVal outputs() As INDArray)
			Me.New(opName, inputs, outputs, Lists.newArrayList(), Lists.newArrayList())
		End Sub

		''' <summary>
		''' Initialize this for <seealso cref="SameDiff"/> execution
		''' Any extra int or float arguments for operations
		''' must be added to the respective TArguments
		''' or IArguments lists upon construction
		''' </summary>
		''' <param name="opName">   the operation opName </param>
		''' <param name="sameDiff"> the samediff instance to use </param>
		''' <param name="args">     the arguments to use </param>
		''' <param name="inPlace">  whether the operation is in place or not </param>
		Public Sub New(ByVal opName As String, ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, inPlace, args)
			Me.opName_Conflict = opName
			iArguments = New List(Of Long)()
			tArguments = New List(Of Double)()
			bArguments = New List(Of Boolean)()
			dArguments = New List(Of DataType)()
			sArguments = New List(Of String)()
			Me.inplaceCall = inPlace
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			Me.New(Nothing, sameDiff, args, inPlace)
		End Sub

		Protected Friend Sub New(ByVal opName As String)
			Me.opName_Conflict = opName
			iArguments = New List(Of Long)()
			tArguments = New List(Of Double)()
			bArguments = New List(Of Boolean)()
			dArguments = New List(Of DataType)()
			sArguments = New List(Of String)()
		End Sub


		''' <summary>
		''' This method returns op opName as string
		''' 
		''' @return
		''' </summary>
		Public Overrides Function opName() As String Implements CustomOp.opName
			Return opName_Conflict
		End Function


		Public Overrides Function outputVariables() As SDVariable()
			Return outputVariables(If(getOwnName() IsNot Nothing, getOwnName(), opName()))
		End Function

		Public Overrides Function outputVariables(ByVal baseName As String) As SDVariable()
			If Me.outputVariables_Conflict Is Nothing Then
				Dim outputNames As val = sameDiff.getOutputsForOp(Me)
				'no need to dynamically create if already exists
				If outputNames IsNot Nothing Then
					outputVariables_Conflict = New SDVariable(outputNames.length - 1){}
					For i As Integer = 0 To outputVariables_Conflict.Length - 1
						outputVariables_Conflict(i) = sameDiff.getVariable(outputNames(i))
					Next i

					Return outputVariables_Conflict
				End If

				Dim newVars As val = sameDiff.generateOutputVariableForOp(Me, baseName, False) 'Also adds outgoing
				If InplaceCall Then
					If args().Length >= 1 Then
						Dim arr As val = args()(0).Arr
						If arr IsNot Nothing Then
							sameDiff.setArrayForVariable(newVars(0).name(), arr)
							addOutputArgument(arr)
						End If
					End If

					Return newVars
				End If

				outputVariables_Conflict = newVars
				If sameDiff.getOutputsForOp(Me) Is Nothing Then
					sameDiff.addOutgoingFor(outputVariables_Conflict, Me)
				End If
				Return newVars
			End If

			Return outputVariables_Conflict
		End Function

		''' <summary>
		''' This method returns LongHash of the opName()
		''' 
		''' @return
		''' </summary>
		Public Overridable Function opHash() As Long Implements CustomOp.opHash
			If hash = 0 Then
				Dim map As val = Nd4j.Executioner.getCustomOperations()
				Dim desc As val = map.get(opName())
				If desc Is Nothing Then
					Throw New ND4JIllegalStateException("Op name " & opName() & " is missing!")
				End If

				hash = desc.getHash()
			End If

			Return hash
		End Function

		Public Overridable Function numDArguments() As Integer Implements CustomOp.numDArguments
			Return dArguments.Count
		End Function

		Public Overridable Function numSArguments() As Integer Implements CustomOp.numSArguments
			Return If(sArguments Is Nothing, 0, sArguments.Count)
		End Function

		Public Overridable Function outputArguments() As IList(Of INDArray)
			Return outputArguments_Conflict
		End Function

		Public Overridable Function inputArguments() As IList(Of INDArray)
			Return inputArguments_Conflict
		End Function

		Public Overridable Function iArgs() As Long() Implements CustomOp.iArgs
			Return Longs.toArray(iArguments)
		End Function

		Public Overridable Function tArgs() As Double() Implements CustomOp.tArgs
			Return Doubles.toArray(tArguments)
		End Function

		Public Overridable Function dArgs() As DataType() Implements CustomOp.dArgs
			Return CType(dArguments, List(Of DataType)).ToArray()
		End Function

		Public Overridable Function sArgs() As String() Implements CustomOp.sArgs
			Return CType(sArguments, List(Of String)).ToArray()
		End Function

		Public Overridable Sub addIArgument(ParamArray ByVal arg() As Integer) Implements CustomOp.addIArgument
			For Each a As Long In arg
				iArguments.Add(a)
			Next a
		End Sub

		Public Overridable Sub addIArgument(ParamArray ByVal arg() As Long) Implements CustomOp.addIArgument
			For Each a As Long In arg
				iArguments.Add(a)
			Next a
		End Sub

		Private Sub addIArgument(ParamArray ByVal arg() As Integer?)
			For Each a As val In arg
				addIArgument(a.longValue())
			Next a
		End Sub

		Public Overridable Sub removeIArgument(ByVal arg As Integer?) Implements CustomOp.removeIArgument
			iArguments.RemoveAt(arg)
		End Sub

		Public Overridable Sub addSArgument(ParamArray ByVal args() As String) Implements CustomOp.addSArgument
			CType(sArguments, List(Of String)).AddRange(New List(Of String) From {args})
		End Sub

		Public Overridable Sub removeSArgument(ByVal argument As String) Implements CustomOp.removeSArgument
			sArguments.Remove(argument)
		End Sub

		Public Overridable Function getSArgument(ByVal index As Integer) As String Implements CustomOp.getSArgument
			Return sArguments(index)
		End Function

		Public Overridable Function getIArgument(ByVal index As Integer) As Long? Implements CustomOp.getIArgument
			Return iArguments(index)
		End Function

		Public Overridable Function numIArguments() As Integer Implements CustomOp.numIArguments
			Return If(iArguments Is Nothing, 0, iArguments.Count)
		End Function

		Public Overridable Function numBArguments() As Integer Implements CustomOp.numBArguments
			Return If(bArguments Is Nothing, 0, bArguments.Count)
		End Function

		Public Overridable Sub addTArgument(ParamArray ByVal arg() As Double) Implements CustomOp.addTArgument
			If arg IsNot Nothing Then
				addTArgument(Doubles.asList(arg).toArray(New Double?(arg.Length - 1){}))
			End If
		End Sub

		Public Overridable Sub addDArgument(ParamArray ByVal arg() As DataType) Implements CustomOp.addDArgument
			If dArguments Is Nothing Then
				dArguments = New List(Of DataType)()
			End If

			If arg IsNot Nothing Then
				CType(dArguments, List(Of DataType)).AddRange(New List(Of DataType) From {arg})
			End If
		End Sub

		Private Sub addTArgument(ParamArray ByVal arg() As Double?)
			CType(tArguments, List(Of Double)).AddRange(New List(Of Double) From {arg})
		End Sub

		Public Overridable Sub removeTArgument(ByVal arg As Double?) Implements CustomOp.removeTArgument
			tArguments.Remove(arg)
		End Sub

		Public Overridable Function getTArgument(ByVal index As Integer) As Double? Implements CustomOp.getTArgument
			Return tArguments(index)
		End Function

		Public Overridable Function numTArguments() As Integer Implements CustomOp.numTArguments
			Return If(tArguments Is Nothing, 0, tArguments.Count)
		End Function

		Public Overridable Sub addInputArgument(ParamArray ByVal arg() As INDArray) Implements CustomOp.addInputArgument
			For i As Integer = 0 To arg.Length - 1
				If arg(i) Is Nothing Then
					Throw New ND4JIllegalStateException("Input " & i & " was null!")
				End If
			Next i


			CType(inputArguments_Conflict, List(Of INDArray)).AddRange(New List(Of INDArray) From {arg})

			Dim args As val = If(sameDiff IsNot Nothing, Me.args(), Nothing)
			Dim arrsSoFar As val = inputArguments()
			'validate arrays passed in, keep in mind that
			'this is a cumulative algorithm so we should always
			'refresh the current list
			If args IsNot Nothing Then
				For i As Integer = 0 To args.length - 1

					' it's possible to get into situation where number of args > number of arrays AT THIS MOMENT
					If i >= arrsSoFar.size() Then
						Continue For
					End If

					If Not args(i).getShape().SequenceEqual(arrsSoFar.get(i).shape()) Then
						Throw New ND4JIllegalStateException("Illegal array passed in as argument [" & i & "]. Expected shape " & java.util.Arrays.toString(args(i).getShape()) & " and received array with shape " & java.util.Arrays.toString(arg(i).shape()))
					End If
				Next i
			End If
		End Sub

		Public Overridable Sub removeInputArgument(ByVal arg As INDArray) Implements CustomOp.removeInputArgument
			inputArguments_Conflict.Remove(arg)
		End Sub

		Public Overrides Function getInputArgument(ByVal index As Integer) As INDArray Implements CustomOp.getInputArgument
			If inputArguments_Conflict Is Nothing OrElse index >= inputArguments_Conflict.Count Then
				Return Nothing
			End If
			Return inputArguments(index)
		End Function

		Public Overridable Sub setInputArgument(ByVal index As Integer, ByVal input As INDArray)
			If index >= inputArguments_Conflict.Count Then
				Dim oldArgs As IList(Of INDArray) = inputArguments_Conflict
				inputArguments_Conflict = New List(Of INDArray)(index+1)
				CType(inputArguments_Conflict, List(Of INDArray)).AddRange(oldArgs)
				Do While inputArguments_Conflict.Count <= index
					inputArguments_Conflict.Add(Nothing)
				Loop
			End If
			inputArguments(index) = input
		End Sub

		Public Overridable WriteOnly Property InputArguments As INDArray()
			Set(ByVal inputs() As INDArray)
				inputArguments_Conflict.Clear()
				If inputs IsNot Nothing AndAlso inputs.Length > 0 Then
					Collections.addAll(inputArguments_Conflict, inputs)
				End If
			End Set
		End Property

		Public Overridable Sub setOutputArgument(ByVal index As Integer, ByVal output As INDArray)
			Do While index >= outputArguments_Conflict.Count
				'Resize list, in case we want to specify arrays not in order they are defined
				'For example, index 1 on empty list, then index 0
				outputArguments_Conflict.Add(Nothing)
			Loop
			outputArguments(index) = output
		End Sub

		Public Overridable Function numInputArguments() As Integer Implements CustomOp.numInputArguments
			Return inputArguments_Conflict.Count
		End Function

		Public Overridable Sub addOutputArgument(ParamArray ByVal arg() As INDArray) Implements CustomOp.addOutputArgument
			For i As Integer = 0 To arg.Length - 1
				If arg(i) Is Nothing Then
					Throw New ND4JIllegalStateException("Output " & i & " was null!")
				End If
			Next i
			CType(outputArguments_Conflict, List(Of INDArray)).AddRange(New List(Of INDArray) From {arg})
		End Sub

		Public Overridable Sub removeOutputArgument(ByVal arg As INDArray) Implements CustomOp.removeOutputArgument
			outputArguments_Conflict.Remove(arg)
		End Sub

		Public Overridable Function getOutputArgument(ByVal index As Integer) As INDArray Implements CustomOp.getOutputArgument
			If outputArguments_Conflict Is Nothing OrElse index >= outputArguments_Conflict.Count Then
				Return Nothing
			End If
			Return outputArguments(index)
		End Function

		Public Overridable Function numOutputArguments() As Integer Implements CustomOp.numOutputArguments
			Return outputArguments_Conflict.Count
		End Function


		Public Overrides Function opNum() As Integer
			Return CInt(opHash())
		End Function

		''' <summary>
		''' This method takes custom opname, and return Op DynamicCustomOpsBuilder instance
		''' </summary>
		''' <param name="opName">
		''' @return </param>
		Public Shared Function builder(ByVal opName As String) As DynamicCustomOpsBuilder
			Dim map As val = Nd4j.Executioner.getCustomOperations()
			Dim lcName As val = If(map.containsKey(opName), opName, opName.ToLower())
			Dim desc As val = map.get(lcName)

			If desc Is Nothing Then
				Throw New ND4JIllegalStateException("Unknown operations requested: [" & opName & "]")
			End If

			Return New DynamicCustomOpsBuilder(lcName, desc.getHash(), desc.getNumInputs(), desc.getNumOutputs(), desc.isAllowsInplace(), desc.getNumTArgs(), desc.getNumIArgs())
		End Function

		Public Overrides Function calculateOutputShape() As IList(Of LongShapeDescriptor)
			Return calculateOutputShape(Nothing)
		End Function

		Public Overrides Function calculateOutputShape(ByVal oc As OpContext) As IList(Of LongShapeDescriptor)
			Dim descriptor As val = Me.Descriptor
			If outputShapes IsNot Nothing AndAlso outputShapes.Count > 0 Then
				Return outputShapes
			End If

			If descriptor Is Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.InvalidOperationException("Could not find descriptor for op: " & opName() & (If(GetType(DynamicCustomOp) = Me.GetType(), "", " - class: " & Me.GetType().FullName)))
			End If


			'not fully initialized: missing integer args
			Dim nI As Integer = If(oc IsNot Nothing, oc.numIArguments(), numIArguments())
			If descriptor.getNumIArgs() >= 0 AndAlso nI < descriptor.getNumIArgs() Then
				If log.isTraceEnabled() Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.trace("Could not calculate output shape for op {}: not fully initialized ({} IArgs specified, " & "{} required)", Me.GetType().FullName, nI, descriptor.getNumIArgs())
				End If
				Return java.util.Collections.emptyList()
			End If


			'not fully initialized: missing floating point args
			Dim nT As Integer = If(oc IsNot Nothing, oc.numTArguments(), numTArguments())
			If descriptor.getNumTArgs() >= 0 AndAlso nT < descriptor.getNumTArgs() Then
				If log.isTraceEnabled() Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.trace("Could not calculate output shape for op {}: not fully initialized ({} TArgs specified, " & "{} required)", Me.GetType().FullName, nT, descriptor.getNumTArgs())
				End If
				Return java.util.Collections.emptyList()
			End If

			'not fully initialized: missing INDArray input args
			Dim nIn As Integer = If(oc IsNot Nothing, oc.numInputArguments(), numInputArguments())
			If descriptor.getNumInputs() >= 0 AndAlso nIn < descriptor.getNumInputs() Then
				If log.isTraceEnabled() Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.trace("Could not calculate output shape for op {}: not fully initialized ({} input (INDArray) args specified, " & "{} required)", Me.GetType().FullName, nIn, descriptor.getNumInputs())
				End If
				Return java.util.Collections.emptyList()
			End If

			Dim ret As IList(Of LongShapeDescriptor)
			If oc Is Nothing Then
				ret = Nd4j.Executioner.calculateOutputShape(Me)
			Else
				ret = Nd4j.Executioner.calculateOutputShape(Me, oc)
			End If
			Return ret
		End Function

		Public Overridable ReadOnly Property Descriptor As CustomOpDescriptor Implements CustomOp.getDescriptor
			Get
				Dim map As val = Nd4j.Executioner.getCustomOperations()
				Return map.get(opName())
			End Get
		End Property

		Public Overridable Sub assertValidForExecution() Implements CustomOp.assertValidForExecution
			Dim descriptor As val = Me.Descriptor
			If descriptor Is Nothing Then
				Throw New NoOpNameFoundException("No descriptor found for op name " & opName())
			End If

			If descriptor.getNumInputs() > 0 AndAlso numInputArguments() < descriptor.getNumInputs() Then
				If sameDiff Is Nothing Then
					Throw New ND4JIllegalStateException("Op [" & opName() & "] failure for [" & Me.getOwnName() & "]: Number of inputs is invalid for execution. " & numInputArguments() & " were provided but " & descriptor.getNumInputs() & " are required for execution")
				Else
					Dim inputNames() As String = sameDiff.getInputsForOp(Me)
					Dim arrayShapes(inputNames.Length - 1) As String
					For i As Integer = 0 To inputNames.Length - 1
						Dim arr As INDArray = sameDiff.getVariable(inputNames(i)).Arr
						arrayShapes(i) = (If(arr Is Nothing, "<no array present>", java.util.Arrays.toString(arr.shape())))
					Next i
					Throw New ND4JIllegalStateException("Op [" & opName() & "] failure for [" & Me.getOwnName() & "]: Number of inputs is invalid for execution. " & numInputArguments() & " were provided but " & descriptor.getNumInputs() & " are required for execution. Input variable names: " & java.util.Arrays.toString(inputNames) & ". Input variable array shapes: " & java.util.Arrays.toString(arrayShapes))
				End If
			End If

			If descriptor.getNumOutputs() > 0 AndAlso numOutputArguments() < descriptor.getNumOutputs() Then
				Throw New ND4JIllegalStateException("Op [" & opName() &"] failure for [" & Me.getOwnName() & "]: Number of outputs is invalid for execution. Specified [" & numOutputArguments() & "] but should be [" & descriptor.getNumOutputs() & "]")
			End If

			'< 0 means dynamic size
			If descriptor.getNumIArgs() >= 0 AndAlso numIArguments() < descriptor.getNumIArgs() Then
				Throw New ND4JIllegalStateException("Op [" & opName() &"] failure for [" & Me.getOwnName() & "]: Number of integer arguments is invalid for execution. Specified [" & numIArguments() & "] but should be [" & descriptor.getNumIArgs() & "]")
			End If

			If descriptor.getNumTArgs() >= 0 AndAlso numTArguments() < descriptor.getNumTArgs() Then
				Throw New ND4JIllegalStateException("Op [" & opName() & "] failure for [" & Me.getOwnName() & "]: Number of inputs is invalid for execution. Specified [" & numTArguments() & "] but should be [" & descriptor.getNumTArgs() & "]")
			End If

		End Sub

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.NotSupportedException("Please extend DynamicCustomOp.doDiff to support SameDiff backprop " & "operations. Op: " & Me.GetType().FullName)
		End Function

		Public Overrides Function ToString() As String
			Return opName()
		End Function

		Public Overridable Function bArgs() As Boolean() Implements CustomOp.bArgs
			Dim result As val = New Boolean(If(bArguments Is Nothing, 0, bArguments.Count) - 1){}

			For e As Integer = 0 To result.length - 1
				result(e) = bArguments(e)
			Next e

			Return result
		End Function

		Public Overridable Sub addBArgument(ParamArray ByVal arg() As Boolean) Implements CustomOp.addBArgument
			If arg IsNot Nothing Then
				For Each b As val In arg
					bArguments.Add(b)
				Next b
			End If
		End Sub

		Public Overridable Function getBArgument(ByVal index As Integer) As Boolean? Implements CustomOp.getBArgument
			Return bArguments(index)
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function


		Public Overrides Function opType() As Op.Type
			Return Op.Type.CUSTOM
		End Function

		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)

		End Sub

		Public Overrides Sub initFromOnnx(ByVal node As Onnx.NodeProto, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, Onnx.AttributeProto), ByVal graph As Onnx.GraphProto)

		End Sub

		Public Overrides Sub clearArrays() Implements CustomOp.clearArrays
			inputArguments_Conflict.Clear()
			outputArguments_Conflict.Clear()
		End Sub

		Protected Friend Shared Function wrapOrNull(ByVal [in] As SDVariable) As SDVariable()
			Return If([in] Is Nothing, Nothing, New SDVariable()){[in]}
		End Function

		Protected Friend Shared Function wrapOrNull(ByVal [in] As INDArray) As INDArray()
			Return If([in] Is Nothing, Nothing, New INDArray()){[in]}
		End Function

		Protected Friend Shared Function wrapFilterNull(Of T)(ParamArray ByVal [in]() As T) As T()
			Dim count As Integer = 0
			For i As Integer = 0 To [in].Length - 1
				If [in](i) IsNot Nothing Then
					count += 1
				End If
			Next i
			Dim [out]() As T = CType(Array.CreateInstance([in].GetType().GetElementType(), count), T())
			Dim j As Integer=0
			For i As Integer = 0 To [in].Length - 1
				If [in](i) IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[j++] = in[i];
					[out](j) = [in](i)
						j += 1
				End If
			Next i
			Return [out]
		End Function

		Public Class DynamicCustomOpsBuilder
			Protected Friend opName As String
			Protected Friend numInputs As Integer
'JAVA TO VB CONVERTER NOTE: The field numOutputs was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend numOutputs_Conflict As Integer
			Protected Friend numTArguments As Integer
			Protected Friend numIArguments As Integer
			Protected Friend numBArguments As Integer
			Protected Friend numSArguments As Integer
			Protected Friend inplaceCall As Boolean
			Protected Friend inplaceAllowed As Boolean
			Protected Friend opHash As Long
			Protected Friend outputShapes As IList(Of LongShapeDescriptor) = New List(Of LongShapeDescriptor)()

			Friend inputArguments As IList(Of INDArray) = New List(Of INDArray)()
			Friend outputArguments As IList(Of INDArray) = New List(Of INDArray)()
			Friend tArguments As IList(Of Double) = New List(Of Double)()
			Friend iArguments As IList(Of Long) = New List(Of Long)()
			Friend dArguments As IList(Of DataType) = New List(Of DataType)()
			Friend bArguments As IList(Of Boolean) = New List(Of Boolean)()
			Friend sArguments As IList(Of String) = New List(Of String)()

			Protected Friend Sub New(ByVal opName As String, ByVal hash As Long, ByVal numInputs As Integer, ByVal numOutputs As Integer, ByVal inplaceAllowed As Boolean, ByVal numTArguments As Integer, ByVal numIArguments As Integer)
				Me.New(opName,hash,numInputs,numOutputs,inplaceAllowed,numTArguments,numIArguments,0)
			End Sub

			Protected Friend Sub New(ByVal opName As String, ByVal hash As Long, ByVal numInputs As Integer, ByVal numOutputs As Integer, ByVal inplaceAllowed As Boolean, ByVal numTArguments As Integer, ByVal numIArguments As Integer, ByVal numSArguments As Integer)
				Me.opHash = hash
				Me.opName = opName
				Me.numInputs = numInputs
				Me.numOutputs_Conflict = numOutputs
				Me.numIArguments = numIArguments
				Me.numTArguments = numTArguments
				Me.numSArguments = numSArguments
				Me.inplaceAllowed = inplaceAllowed
			End Sub

			''' <summary>
			''' This method
			''' takes arbitrary number of input INDArrays in, as Op input
			''' Note that this ACCUMULATES arguments. You are able to call this method
			''' multiple times and it will add arguments to a list.
			''' PLEASE NOTE: this method does NOT validate lengths/shapes.
			''' </summary>
			''' <param name="inputs">
			''' @return </param>
			Public Overridable Function addInputs(ParamArray ByVal inputs() As INDArray) As DynamicCustomOpsBuilder
				' if we have positive value as numInputs - we should ensure equal amount of arguments
				If numInputs >= 0 Then
					If inputs Is Nothing Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects at least " & numInputs & " arguments. Null was passed instead.")
					End If

					If numInputs > inputs.Length Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects at least " & numInputs & " arguments, but " & inputs.Length & " was passed to constructor")
					End If
				End If

				For Each [in] As val In inputs
					inputArguments.Add([in])
				Next [in]

				Return Me
			End Function

			''' <summary>
			''' This method takes arbitrary number of
			''' output INDArrays in, to store operation result
			''' Note that this ACCUMULATES arguments. You are able to call this method
			''' multiple times and it will add arguments to a list.
			''' PLEASE NOTE: this method does NOT validate lengths/shapes.
			''' </summary>
			''' <param name="outputs">
			''' @return </param>
			Public Overridable Function addOutputs(ParamArray ByVal outputs() As INDArray) As DynamicCustomOpsBuilder
				If numOutputs_Conflict >= 0 Then
					If outputs Is Nothing Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects at least " & numOutputs_Conflict & " arguments. Null was passed instead.")
					End If

					If numOutputs_Conflict > outputs.Length Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects at least " & numOutputs_Conflict & " arguments, but " & outputs.Length & " was passed to constructor")
					End If
				End If

				For Each [in] As val In outputs
					outputArguments.Add([in])
				Next [in]

				Return Me
			End Function


			''' <summary>
			''' Whether an op call is in place or not.
			''' </summary>
			''' <param name="reallyCall">
			''' @return </param>
			Public Overridable Function callInplace(ByVal reallyCall As Boolean) As DynamicCustomOpsBuilder
				If reallyCall AndAlso Not inplaceAllowed Then
					Throw New ND4JIllegalStateException("Requested op can't be called inplace")
				End If

				Me.inplaceCall = reallyCall
				Return Me
			End Function

			''' <summary>
			''' This method takes arbitrary number of Integer arguments for op,
			''' Note that this ACCUMULATES arguments. You are able to call this method
			''' multiple times and it will add arguments to a list.
			''' PLEASE NOTE: this method does NOT validate values.
			''' </summary>
			''' <param name="iargs">
			''' @return </param>
			Public Overridable Function addIntegerArguments(ByVal iargs As IList(Of Integer)) As DynamicCustomOpsBuilder
				If numIArguments >= 0 Then
					If iargs Is Nothing Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects " & numIArguments & " integer arguments. Null was passed instead.")
					End If

					If numIArguments > iargs.Count Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects at least " & numIArguments & " integer arguments, but " & iargs.Count & " was passed to constructor")
					End If
				End If

				For Each [in] As val In iargs
					iArguments.Add([in].longValue())
				Next [in]

				Return Me
			End Function

			''' <summary>
			''' This method takes arbitrary number of String arguments for op,
			''' Note that this ACCUMULATES arguments. You are able to call this method
			''' multiple times and it will add arguments to a list.
			''' PLEASE NOTE: this method does NOT validate values.
			''' </summary>
			''' <param name="sArgs">
			''' @return </param>
			Public Overridable Function addStringArguments(ByVal sArgs As IList(Of String)) As DynamicCustomOpsBuilder
				If numSArguments >= 0 Then
					If sArgs Is Nothing Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects " & numSArguments & " string arguments. Null was passed instead.")
					End If

					If numSArguments > sArgs.Count Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects at least " & numSArguments & " string arguments, but " & sArgs.Count & " was passed to constructor")
					End If
				End If

				For Each [in] As val In sArgs
					sArguments.Add([in])
				Next [in]

				Return Me
			End Function

			''' <summary>
			''' This method takes arbitrary number of String arguments for op,
			''' Note that this ACCUMULATES arguments. You are able to call this method
			''' multiple times and it will add arguments to a list.
			''' PLEASE NOTE: this method does NOT validate values.
			''' </summary>
			''' <param name="arg">
			''' @return </param>
			Public Overridable Function addStringArguments(ByVal arg As String) As DynamicCustomOpsBuilder
				If numSArguments <> 1 AndAlso numSArguments > 0 Then
					Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects " & numSArguments & " string arguments. One arg was passed instead.")
				End If

				sArguments.Add(arg)

				Return Me
			End Function

			''' <summary>
			''' This method takes arbitrary number of String arguments for op,
			''' Note that this ACCUMULATES arguments. You are able to call this method
			''' multiple times and it will add arguments to a list.
			''' PLEASE NOTE: this method does NOT validate values.
			''' </summary>
			''' <param name="sArgs">
			''' @return </param>
			Public Overridable Function addStringArguments(ParamArray ByVal sArgs() As String) As DynamicCustomOpsBuilder
				If numSArguments >= 0 Then
					If sArgs Is Nothing Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects at least " & numSArguments & " integer arguments. Null was passed instead.")
					End If

					If numSArguments > sArgs.Length Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects at least " & numSArguments & " integer arguments, but " & sArgs.Length & " was passed to constructor")
					End If
				End If

				For Each [in] As val In sArgs
					sArguments.Add([in])
				Next [in]

				Return Me
			End Function

			''' <summary>
			''' This method takes arbitrary number of Integer arguments for op,
			''' Note that this ACCUMULATES arguments. You are able to call this method
			''' multiple times and it will add arguments to a list.
			''' PLEASE NOTE: this method does NOT validate values.
			''' </summary>
			''' <param name="arg">
			''' @return </param>
			Public Overridable Function addIntegerArguments(ByVal arg As Long) As DynamicCustomOpsBuilder
				If numIArguments <> 1 AndAlso numIArguments > 0 Then
					Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects " & numIArguments & " integer arguments. One arg was passed instead.")
				End If

				iArguments.Add(arg)

				Return Me
			End Function

			''' <summary>
			''' This method takes arbitrary number of Integer arguments for op,
			''' Note that this ACCUMULATES arguments. You are able to call this method
			''' multiple times and it will add arguments to a list.
			''' PLEASE NOTE: this method does NOT validate values.
			''' </summary>
			''' <param name="iargs">
			''' @return </param>
			Public Overridable Function addIntegerArguments(ParamArray ByVal iargs() As Integer) As DynamicCustomOpsBuilder
				If numIArguments >= 0 Then
					If iargs Is Nothing Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects at least " & numIArguments & " integer arguments. Null was passed instead.")
					End If

					If numIArguments > iargs.Length Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects at least " & numIArguments & " integer arguments, but " & iargs.Length & " was passed to constructor")
					End If
				End If

				For Each [in] As val In iargs
					iArguments.Add(CLng([in]))
				Next [in]

				Return Me
			End Function

			''' <summary>
			''' This method takes arbitrary number of Integer arguments for op,
			''' Note that this ACCUMULATES arguments. You are able to call this method
			''' multiple times and it will add arguments to a list.
			''' PLEASE NOTE: this method does NOT validate values.
			''' </summary>
			''' <param name="bargs">
			''' @return </param>
			Public Overridable Function addBooleanArguments(ParamArray ByVal bargs() As Boolean) As DynamicCustomOpsBuilder
				For Each [in] As val In bargs
					bArguments.Add([in])
				Next [in]

				Return Me
			End Function

			''' <summary>
			''' This method takes arbitrary number of Double arguments for op,
			''' Note that this ACCUMULATES arguments. You are able to call this method
			''' multiple times and it will add arguments to a list.
			''' PLEASE NOTE: this method does NOT validate values.
			''' 
			''' @return
			''' </summary>
			Public Overridable Function addFloatingPointArguments(ParamArray ByVal targs() As Double?) As DynamicCustomOpsBuilder
				If numTArguments >= 0 Then
					If targs Is Nothing Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects at least " & numTArguments & " integer arguments. Null was passed instead.")
					End If

					If numTArguments > targs.Length Then
						Throw New ND4JIllegalStateException("CustomOp [" & opName & "] expects at least " & numTArguments & " integer arguments, but " & targs.Length & " was passed to constructor")
					End If
				End If

				For Each [in] As val In targs
					tArguments.Add([in])
				Next [in]

				Return Me
			End Function


			''' <summary>
			''' Adds an oup
			''' </summary>
			''' <param name="shape">
			''' @return </param>
	'        
	'        public DynamicCustomOpsBuilder addOutputShape(int[] shape) {
	'            this.outputShapes.add(ArrayUtil.toLongArray(shape));
	'            return this;
	'        }
	'
	'        public DynamicCustomOpsBuilder addOutputShape(long[] shape) {
	'            this.outputShapes.add(shape);
	'            return this;
	'        }
	'

			Public Overridable Function addOutputShape(ByVal shape As LongShapeDescriptor) As DynamicCustomOpsBuilder
				Me.outputShapes.Add(shape)
				Return Me
			End Function


			Public Overridable Function build() As DynamicCustomOp
				' Eventually we probably will lift this restriction
				'if (!inplaceCall && outputArguments.size() == 0)
				'    throw new ND4JIllegalStateException("If operation is not-inplace, it must have outputs defined");

				Dim result As val = New DynamicCustomOp(opName)
				result.inputArguments = inputArguments
				result.outputArguments = outputArguments
				result.iArguments = iArguments
				result.tArguments = tArguments
				result.bArguments = bArguments
				result.dArguments = dArguments
				result.inplaceCall = inplaceCall
				result.hash = opHash
				result.outputShapes = outputShapes

				Return result
			End Function

			Public Overridable ReadOnly Property NumOutputs As Integer
				Get
					Return -1
				End Get
			End Property
		End Class

		Public Overrides Function mappingsForFunction() As IDictionary(Of String, IDictionary(Of String, PropertyMapping))
			Return MyBase.mappingsForFunction()
		End Function

		Public Overrides WriteOnly Property PropertiesForFunction As IDictionary(Of String, Object)
			Set(ByVal properties As IDictionary(Of String, Object))
				MyBase.PropertiesForFunction = properties
			End Set
		End Property

		Public Overrides Function getValue(ByVal [property] As System.Reflection.FieldInfo) As Object
			Return MyBase.getValue([property])
		End Function

		Public Overrides Sub setValueFor(ByVal target As System.Reflection.FieldInfo, ByVal value As Object)

		End Sub


		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			Dim opDescriptor As OpNamespace.OpDescriptor = OpDescriptorHolder.descriptorForOpName(opName())
			Dim ret As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
			For Each argDescriptor As OpNamespace.ArgDescriptor In opDescriptor.getArgDescriptorList()
				Select Case argDescriptor.ArgType
					Case [STRING]
						If argDescriptor.ArgIndex < numSArguments() Then
							ret(argDescriptor.Name) = getSArgument(argDescriptor.ArgIndex)
						End If
					Case BOOL
						If argDescriptor.ArgIndex < numBArguments() Then
							ret(argDescriptor.Name) = getBArgument(argDescriptor.ArgIndex)
						End If
					Case FLOAT, [DOUBLE]
						If argDescriptor.ArgIndex < numTArguments() Then
							ret(argDescriptor.Name) = getTArgument(argDescriptor.ArgIndex)
						End If
					Case INT32, INT64
						If argDescriptor.ArgIndex < numIArguments() Then
							ret(argDescriptor.Name) = getIArgument(argDescriptor.ArgIndex)
						End If
					Case DATA_TYPE
						If argDescriptor.ArgIndex < numDArguments() Then
							ret(argDescriptor.Name) = dArguments(argDescriptor.ArgIndex)
						End If
				End Select
			Next argDescriptor
			Return ret
		End Function
	End Class

End Namespace